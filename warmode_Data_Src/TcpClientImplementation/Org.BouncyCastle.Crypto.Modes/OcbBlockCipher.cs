using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class OcbBlockCipher : IAeadBlockCipher
	{
		private const int BLOCK_SIZE = 16;

		private readonly IBlockCipher hashCipher;

		private readonly IBlockCipher mainCipher;

		private bool forEncryption;

		private int macSize;

		private byte[] initialAssociatedText;

		private IList L;

		private byte[] L_Asterisk;

		private byte[] L_Dollar;

		private byte[] KtopInput;

		private byte[] Stretch = new byte[24];

		private byte[] OffsetMAIN_0 = new byte[16];

		private byte[] hashBlock;

		private byte[] mainBlock;

		private int hashBlockPos;

		private int mainBlockPos;

		private long hashBlockCount;

		private long mainBlockCount;

		private byte[] OffsetHASH;

		private byte[] Sum;

		private byte[] OffsetMAIN = new byte[16];

		private byte[] Checksum;

		private byte[] macBlock;

		public virtual string AlgorithmName
		{
			get
			{
				return this.mainCipher.AlgorithmName + "/OCB";
			}
		}

		public OcbBlockCipher(IBlockCipher hashCipher, IBlockCipher mainCipher)
		{
			if (hashCipher == null)
			{
				throw new ArgumentNullException("hashCipher");
			}
			if (hashCipher.GetBlockSize() != 16)
			{
				throw new ArgumentException("must have a block size of " + 16, "hashCipher");
			}
			if (mainCipher == null)
			{
				throw new ArgumentNullException("mainCipher");
			}
			if (mainCipher.GetBlockSize() != 16)
			{
				throw new ArgumentException("must have a block size of " + 16, "mainCipher");
			}
			if (!hashCipher.AlgorithmName.Equals(mainCipher.AlgorithmName))
			{
				throw new ArgumentException("'hashCipher' and 'mainCipher' must be the same algorithm");
			}
			this.hashCipher = hashCipher;
			this.mainCipher = mainCipher;
		}

		public virtual IBlockCipher GetUnderlyingCipher()
		{
			return this.mainCipher;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			bool flag = this.forEncryption;
			this.forEncryption = forEncryption;
			this.macBlock = null;
			byte[] array;
			KeyParameter keyParameter;
			if (parameters is AeadParameters)
			{
				AeadParameters aeadParameters = (AeadParameters)parameters;
				array = aeadParameters.GetNonce();
				this.initialAssociatedText = aeadParameters.GetAssociatedText();
				int num = aeadParameters.MacSize;
				if (num < 64 || num > 128 || num % 8 != 0)
				{
					throw new ArgumentException("Invalid value for MAC size: " + num);
				}
				this.macSize = num / 8;
				keyParameter = aeadParameters.Key;
			}
			else
			{
				if (!(parameters is ParametersWithIV))
				{
					throw new ArgumentException("invalid parameters passed to OCB");
				}
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				array = parametersWithIV.GetIV();
				this.initialAssociatedText = null;
				this.macSize = 16;
				keyParameter = (KeyParameter)parametersWithIV.Parameters;
			}
			this.hashBlock = new byte[16];
			this.mainBlock = new byte[forEncryption ? 16 : (16 + this.macSize)];
			if (array == null)
			{
				array = new byte[0];
			}
			if (array.Length > 15)
			{
				throw new ArgumentException("IV must be no more than 15 bytes");
			}
			if (keyParameter != null)
			{
				this.hashCipher.Init(true, keyParameter);
				this.mainCipher.Init(forEncryption, keyParameter);
				this.KtopInput = null;
			}
			else if (flag != forEncryption)
			{
				throw new ArgumentException("cannot change encrypting state without providing key.");
			}
			this.L_Asterisk = new byte[16];
			this.hashCipher.ProcessBlock(this.L_Asterisk, 0, this.L_Asterisk, 0);
			this.L_Dollar = OcbBlockCipher.OCB_double(this.L_Asterisk);
			this.L = Platform.CreateArrayList();
			this.L.Add(OcbBlockCipher.OCB_double(this.L_Dollar));
			int num2 = this.ProcessNonce(array);
			int num3 = num2 % 8;
			int num4 = num2 / 8;
			if (num3 == 0)
			{
				Array.Copy(this.Stretch, num4, this.OffsetMAIN_0, 0, 16);
			}
			else
			{
				for (int i = 0; i < 16; i++)
				{
					uint num5 = (uint)this.Stretch[num4];
					uint num6 = (uint)this.Stretch[++num4];
					this.OffsetMAIN_0[i] = (byte)(num5 << num3 | num6 >> 8 - num3);
				}
			}
			this.hashBlockPos = 0;
			this.mainBlockPos = 0;
			this.hashBlockCount = 0L;
			this.mainBlockCount = 0L;
			this.OffsetHASH = new byte[16];
			this.Sum = new byte[16];
			Array.Copy(this.OffsetMAIN_0, 0, this.OffsetMAIN, 0, 16);
			this.Checksum = new byte[16];
			if (this.initialAssociatedText != null)
			{
				this.ProcessAadBytes(this.initialAssociatedText, 0, this.initialAssociatedText.Length);
			}
		}

		protected virtual int ProcessNonce(byte[] N)
		{
			byte[] array = new byte[16];
			Array.Copy(N, 0, array, array.Length - N.Length, N.Length);
			array[0] = (byte)(this.macSize << 4);
			byte[] expr_32_cp_0 = array;
			int expr_32_cp_1 = 15 - N.Length;
			expr_32_cp_0[expr_32_cp_1] |= 1;
			int result = (int)(array[15] & 63);
			byte[] expr_50_cp_0 = array;
			int expr_50_cp_1 = 15;
			expr_50_cp_0[expr_50_cp_1] &= 192;
			if (this.KtopInput == null || !Arrays.AreEqual(array, this.KtopInput))
			{
				byte[] array2 = new byte[16];
				this.KtopInput = array;
				this.hashCipher.ProcessBlock(this.KtopInput, 0, array2, 0);
				Array.Copy(array2, 0, this.Stretch, 0, 16);
				for (int i = 0; i < 8; i++)
				{
					this.Stretch[16 + i] = (array2[i] ^ array2[i + 1]);
				}
			}
			return result;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual byte[] GetMac()
		{
			return Arrays.Clone(this.macBlock);
		}

		public virtual int GetOutputSize(int len)
		{
			int num = len + this.mainBlockPos;
			if (this.forEncryption)
			{
				return num + this.macSize;
			}
			if (num >= this.macSize)
			{
				return num - this.macSize;
			}
			return 0;
		}

		public virtual int GetUpdateOutputSize(int len)
		{
			int num = len + this.mainBlockPos;
			if (!this.forEncryption)
			{
				if (num < this.macSize)
				{
					return 0;
				}
				num -= this.macSize;
			}
			return num - num % 16;
		}

		public virtual void ProcessAadByte(byte input)
		{
			this.hashBlock[this.hashBlockPos] = input;
			if (++this.hashBlockPos == this.hashBlock.Length)
			{
				this.ProcessHashBlock();
			}
		}

		public virtual void ProcessAadBytes(byte[] input, int off, int len)
		{
			for (int i = 0; i < len; i++)
			{
				this.hashBlock[this.hashBlockPos] = input[off + i];
				if (++this.hashBlockPos == this.hashBlock.Length)
				{
					this.ProcessHashBlock();
				}
			}
		}

		public virtual int ProcessByte(byte input, byte[] output, int outOff)
		{
			this.mainBlock[this.mainBlockPos] = input;
			if (++this.mainBlockPos == this.mainBlock.Length)
			{
				this.ProcessMainBlock(output, outOff);
				return 16;
			}
			return 0;
		}

		public virtual int ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
		{
			int num = 0;
			for (int i = 0; i < len; i++)
			{
				this.mainBlock[this.mainBlockPos] = input[inOff + i];
				if (++this.mainBlockPos == this.mainBlock.Length)
				{
					this.ProcessMainBlock(output, outOff + num);
					num += 16;
				}
			}
			return num;
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			byte[] array = null;
			if (!this.forEncryption)
			{
				if (this.mainBlockPos < this.macSize)
				{
					throw new InvalidCipherTextException("data too short");
				}
				this.mainBlockPos -= this.macSize;
				array = new byte[this.macSize];
				Array.Copy(this.mainBlock, this.mainBlockPos, array, 0, this.macSize);
			}
			if (this.hashBlockPos > 0)
			{
				OcbBlockCipher.OCB_extend(this.hashBlock, this.hashBlockPos);
				this.UpdateHASH(this.L_Asterisk);
			}
			if (this.mainBlockPos > 0)
			{
				if (this.forEncryption)
				{
					OcbBlockCipher.OCB_extend(this.mainBlock, this.mainBlockPos);
					OcbBlockCipher.Xor(this.Checksum, this.mainBlock);
				}
				OcbBlockCipher.Xor(this.OffsetMAIN, this.L_Asterisk);
				byte[] array2 = new byte[16];
				this.hashCipher.ProcessBlock(this.OffsetMAIN, 0, array2, 0);
				OcbBlockCipher.Xor(this.mainBlock, array2);
				Check.OutputLength(output, outOff, this.mainBlockPos, "Output buffer too short");
				Array.Copy(this.mainBlock, 0, output, outOff, this.mainBlockPos);
				if (!this.forEncryption)
				{
					OcbBlockCipher.OCB_extend(this.mainBlock, this.mainBlockPos);
					OcbBlockCipher.Xor(this.Checksum, this.mainBlock);
				}
			}
			OcbBlockCipher.Xor(this.Checksum, this.OffsetMAIN);
			OcbBlockCipher.Xor(this.Checksum, this.L_Dollar);
			this.hashCipher.ProcessBlock(this.Checksum, 0, this.Checksum, 0);
			OcbBlockCipher.Xor(this.Checksum, this.Sum);
			this.macBlock = new byte[this.macSize];
			Array.Copy(this.Checksum, 0, this.macBlock, 0, this.macSize);
			int num = this.mainBlockPos;
			if (this.forEncryption)
			{
				Check.OutputLength(output, outOff, num + this.macSize, "Output buffer too short");
				Array.Copy(this.macBlock, 0, output, outOff + num, this.macSize);
				num += this.macSize;
			}
			else if (!Arrays.ConstantTimeAreEqual(this.macBlock, array))
			{
				throw new InvalidCipherTextException("mac check in OCB failed");
			}
			this.Reset(false);
			return num;
		}

		public virtual void Reset()
		{
			this.Reset(true);
		}

		protected virtual void Clear(byte[] bs)
		{
			if (bs != null)
			{
				Array.Clear(bs, 0, bs.Length);
			}
		}

		protected virtual byte[] GetLSub(int n)
		{
			while (n >= this.L.Count)
			{
				this.L.Add(OcbBlockCipher.OCB_double((byte[])this.L[this.L.Count - 1]));
			}
			return (byte[])this.L[n];
		}

		protected virtual void ProcessHashBlock()
		{
			this.UpdateHASH(this.GetLSub(OcbBlockCipher.OCB_ntz(this.hashBlockCount += 1L)));
			this.hashBlockPos = 0;
		}

		protected virtual void ProcessMainBlock(byte[] output, int outOff)
		{
			Check.DataLength(output, outOff, 16, "Output buffer too short");
			if (this.forEncryption)
			{
				OcbBlockCipher.Xor(this.Checksum, this.mainBlock);
				this.mainBlockPos = 0;
			}
			OcbBlockCipher.Xor(this.OffsetMAIN, this.GetLSub(OcbBlockCipher.OCB_ntz(this.mainBlockCount += 1L)));
			OcbBlockCipher.Xor(this.mainBlock, this.OffsetMAIN);
			this.mainCipher.ProcessBlock(this.mainBlock, 0, this.mainBlock, 0);
			OcbBlockCipher.Xor(this.mainBlock, this.OffsetMAIN);
			Array.Copy(this.mainBlock, 0, output, outOff, 16);
			if (!this.forEncryption)
			{
				OcbBlockCipher.Xor(this.Checksum, this.mainBlock);
				Array.Copy(this.mainBlock, 16, this.mainBlock, 0, this.macSize);
				this.mainBlockPos = this.macSize;
			}
		}

		protected virtual void Reset(bool clearMac)
		{
			this.hashCipher.Reset();
			this.mainCipher.Reset();
			this.Clear(this.hashBlock);
			this.Clear(this.mainBlock);
			this.hashBlockPos = 0;
			this.mainBlockPos = 0;
			this.hashBlockCount = 0L;
			this.mainBlockCount = 0L;
			this.Clear(this.OffsetHASH);
			this.Clear(this.Sum);
			Array.Copy(this.OffsetMAIN_0, 0, this.OffsetMAIN, 0, 16);
			this.Clear(this.Checksum);
			if (clearMac)
			{
				this.macBlock = null;
			}
			if (this.initialAssociatedText != null)
			{
				this.ProcessAadBytes(this.initialAssociatedText, 0, this.initialAssociatedText.Length);
			}
		}

		protected virtual void UpdateHASH(byte[] LSub)
		{
			OcbBlockCipher.Xor(this.OffsetHASH, LSub);
			OcbBlockCipher.Xor(this.hashBlock, this.OffsetHASH);
			this.hashCipher.ProcessBlock(this.hashBlock, 0, this.hashBlock, 0);
			OcbBlockCipher.Xor(this.Sum, this.hashBlock);
		}

		protected static byte[] OCB_double(byte[] block)
		{
			byte[] array = new byte[16];
			int num = OcbBlockCipher.ShiftLeft(block, array);
			byte[] expr_18_cp_0 = array;
			int expr_18_cp_1 = 15;
			expr_18_cp_0[expr_18_cp_1] ^= (byte)(135 >> (1 - num << 3));
			return array;
		}

		protected static void OCB_extend(byte[] block, int pos)
		{
			block[pos] = 128;
			while (++pos < 16)
			{
				block[pos] = 0;
			}
		}

		protected static int OCB_ntz(long x)
		{
			if (x == 0L)
			{
				return 64;
			}
			int num = 0;
			ulong num2 = (ulong)x;
			while ((num2 & 1uL) == 0uL)
			{
				num++;
				num2 >>= 1;
			}
			return num;
		}

		protected static int ShiftLeft(byte[] block, byte[] output)
		{
			int num = 16;
			uint num2 = 0u;
			while (--num >= 0)
			{
				uint num3 = (uint)block[num];
				output[num] = (byte)(num3 << 1 | num2);
				num2 = (num3 >> 7 & 1u);
			}
			return (int)num2;
		}

		protected static void Xor(byte[] block, byte[] val)
		{
			for (int i = 15; i >= 0; i--)
			{
				int expr_0C_cp_1 = i;
				block[expr_0C_cp_1] ^= val[i];
			}
		}
	}
}
