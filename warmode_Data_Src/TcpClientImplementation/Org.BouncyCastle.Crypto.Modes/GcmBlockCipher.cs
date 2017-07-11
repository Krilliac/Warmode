using Org.BouncyCastle.Crypto.Modes.Gcm;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class GcmBlockCipher : IAeadBlockCipher
	{
		private const int BlockSize = 16;

		private readonly IBlockCipher cipher;

		private readonly IGcmMultiplier multiplier;

		private IGcmExponentiator exp;

		private bool forEncryption;

		private int macSize;

		private byte[] nonce;

		private byte[] initialAssociatedText;

		private byte[] H;

		private byte[] J0;

		private byte[] bufBlock;

		private byte[] macBlock;

		private byte[] S;

		private byte[] S_at;

		private byte[] S_atPre;

		private byte[] counter;

		private int bufOff;

		private ulong totalLength;

		private byte[] atBlock;

		private int atBlockPos;

		private ulong atLength;

		private ulong atLengthPre;

		public virtual string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName + "/GCM";
			}
		}

		public GcmBlockCipher(IBlockCipher c) : this(c, null)
		{
		}

		public GcmBlockCipher(IBlockCipher c, IGcmMultiplier m)
		{
			if (c.GetBlockSize() != 16)
			{
				throw new ArgumentException("cipher required with a block size of " + 16 + ".");
			}
			if (m == null)
			{
				m = new Tables8kGcmMultiplier();
			}
			this.cipher = c;
			this.multiplier = m;
		}

		public IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			this.macBlock = null;
			KeyParameter keyParameter;
			if (parameters is AeadParameters)
			{
				AeadParameters aeadParameters = (AeadParameters)parameters;
				this.nonce = aeadParameters.GetNonce();
				this.initialAssociatedText = aeadParameters.GetAssociatedText();
				int num = aeadParameters.MacSize;
				if (num < 32 || num > 128 || num % 8 != 0)
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
					throw new ArgumentException("invalid parameters passed to GCM");
				}
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				this.nonce = parametersWithIV.GetIV();
				this.initialAssociatedText = null;
				this.macSize = 16;
				keyParameter = (KeyParameter)parametersWithIV.Parameters;
			}
			int num2 = forEncryption ? 16 : (16 + this.macSize);
			this.bufBlock = new byte[num2];
			if (this.nonce == null || this.nonce.Length < 1)
			{
				throw new ArgumentException("IV must be at least 1 byte");
			}
			if (keyParameter != null)
			{
				this.cipher.Init(true, keyParameter);
				this.H = new byte[16];
				this.cipher.ProcessBlock(this.H, 0, this.H, 0);
				this.multiplier.Init(this.H);
				this.exp = null;
			}
			else if (this.H == null)
			{
				throw new ArgumentException("Key must be specified in initial init");
			}
			this.J0 = new byte[16];
			if (this.nonce.Length == 12)
			{
				Array.Copy(this.nonce, 0, this.J0, 0, this.nonce.Length);
				this.J0[15] = 1;
			}
			else
			{
				this.gHASH(this.J0, this.nonce, this.nonce.Length);
				byte[] array = new byte[16];
				Pack.UInt64_To_BE((ulong)((long)this.nonce.Length * 8L), array, 8);
				this.gHASHBlock(this.J0, array);
			}
			this.S = new byte[16];
			this.S_at = new byte[16];
			this.S_atPre = new byte[16];
			this.atBlock = new byte[16];
			this.atBlockPos = 0;
			this.atLength = 0uL;
			this.atLengthPre = 0uL;
			this.counter = Arrays.Clone(this.J0);
			this.bufOff = 0;
			this.totalLength = 0uL;
			if (this.initialAssociatedText != null)
			{
				this.ProcessAadBytes(this.initialAssociatedText, 0, this.initialAssociatedText.Length);
			}
		}

		public virtual byte[] GetMac()
		{
			return Arrays.Clone(this.macBlock);
		}

		public virtual int GetOutputSize(int len)
		{
			int num = len + this.bufOff;
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
			int num = len + this.bufOff;
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
			this.atBlock[this.atBlockPos] = input;
			if (++this.atBlockPos == 16)
			{
				this.gHASHBlock(this.S_at, this.atBlock);
				this.atBlockPos = 0;
				this.atLength += 16uL;
			}
		}

		public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
		{
			for (int i = 0; i < len; i++)
			{
				this.atBlock[this.atBlockPos] = inBytes[inOff + i];
				if (++this.atBlockPos == 16)
				{
					this.gHASHBlock(this.S_at, this.atBlock);
					this.atBlockPos = 0;
					this.atLength += 16uL;
				}
			}
		}

		private void InitCipher()
		{
			if (this.atLength > 0uL)
			{
				Array.Copy(this.S_at, 0, this.S_atPre, 0, 16);
				this.atLengthPre = this.atLength;
			}
			if (this.atBlockPos > 0)
			{
				this.gHASHPartial(this.S_atPre, this.atBlock, 0, this.atBlockPos);
				this.atLengthPre += (ulong)this.atBlockPos;
			}
			if (this.atLengthPre > 0uL)
			{
				Array.Copy(this.S_atPre, 0, this.S, 0, 16);
			}
		}

		public virtual int ProcessByte(byte input, byte[] output, int outOff)
		{
			this.bufBlock[this.bufOff] = input;
			if (++this.bufOff == this.bufBlock.Length)
			{
				this.OutputBlock(output, outOff);
				return 16;
			}
			return 0;
		}

		public virtual int ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
		{
			if (input.Length < inOff + len)
			{
				throw new DataLengthException("Input buffer too short");
			}
			int num = 0;
			for (int i = 0; i < len; i++)
			{
				this.bufBlock[this.bufOff] = input[inOff + i];
				if (++this.bufOff == this.bufBlock.Length)
				{
					this.OutputBlock(output, outOff + num);
					num += 16;
				}
			}
			return num;
		}

		private void OutputBlock(byte[] output, int offset)
		{
			Check.OutputLength(output, offset, 16, "Output buffer too short");
			if (this.totalLength == 0uL)
			{
				this.InitCipher();
			}
			this.gCTRBlock(this.bufBlock, output, offset);
			if (this.forEncryption)
			{
				this.bufOff = 0;
				return;
			}
			Array.Copy(this.bufBlock, 16, this.bufBlock, 0, this.macSize);
			this.bufOff = this.macSize;
		}

		public int DoFinal(byte[] output, int outOff)
		{
			if (this.totalLength == 0uL)
			{
				this.InitCipher();
			}
			int num = this.bufOff;
			if (this.forEncryption)
			{
				Check.OutputLength(output, outOff, num + this.macSize, "Output buffer too short");
			}
			else
			{
				if (num < this.macSize)
				{
					throw new InvalidCipherTextException("data too short");
				}
				num -= this.macSize;
				Check.OutputLength(output, outOff, num, "Output buffer too short");
			}
			if (num > 0)
			{
				this.gCTRPartial(this.bufBlock, 0, num, output, outOff);
			}
			this.atLength += (ulong)this.atBlockPos;
			if (this.atLength > this.atLengthPre)
			{
				if (this.atBlockPos > 0)
				{
					this.gHASHPartial(this.S_at, this.atBlock, 0, this.atBlockPos);
				}
				if (this.atLengthPre > 0uL)
				{
					GcmUtilities.Xor(this.S_at, this.S_atPre);
				}
				long pow = (long)(this.totalLength * 8uL + 127uL >> 7);
				byte[] array = new byte[16];
				if (this.exp == null)
				{
					this.exp = new Tables1kGcmExponentiator();
					this.exp.Init(this.H);
				}
				this.exp.ExponentiateX(pow, array);
				GcmUtilities.Multiply(this.S_at, array);
				GcmUtilities.Xor(this.S, this.S_at);
			}
			byte[] array2 = new byte[16];
			Pack.UInt64_To_BE(this.atLength * 8uL, array2, 0);
			Pack.UInt64_To_BE(this.totalLength * 8uL, array2, 8);
			this.gHASHBlock(this.S, array2);
			byte[] array3 = new byte[16];
			this.cipher.ProcessBlock(this.J0, 0, array3, 0);
			GcmUtilities.Xor(array3, this.S);
			int num2 = num;
			this.macBlock = new byte[this.macSize];
			Array.Copy(array3, 0, this.macBlock, 0, this.macSize);
			if (this.forEncryption)
			{
				Array.Copy(this.macBlock, 0, output, outOff + this.bufOff, this.macSize);
				num2 += this.macSize;
			}
			else
			{
				byte[] array4 = new byte[this.macSize];
				Array.Copy(this.bufBlock, num, array4, 0, this.macSize);
				if (!Arrays.ConstantTimeAreEqual(this.macBlock, array4))
				{
					throw new InvalidCipherTextException("mac check in GCM failed");
				}
			}
			this.Reset(false);
			return num2;
		}

		public virtual void Reset()
		{
			this.Reset(true);
		}

		private void Reset(bool clearMac)
		{
			this.cipher.Reset();
			this.S = new byte[16];
			this.S_at = new byte[16];
			this.S_atPre = new byte[16];
			this.atBlock = new byte[16];
			this.atBlockPos = 0;
			this.atLength = 0uL;
			this.atLengthPre = 0uL;
			this.counter = Arrays.Clone(this.J0);
			this.bufOff = 0;
			this.totalLength = 0uL;
			if (this.bufBlock != null)
			{
				Arrays.Fill(this.bufBlock, 0);
			}
			if (clearMac)
			{
				this.macBlock = null;
			}
			if (this.initialAssociatedText != null)
			{
				this.ProcessAadBytes(this.initialAssociatedText, 0, this.initialAssociatedText.Length);
			}
		}

		private void gCTRBlock(byte[] block, byte[] output, int outOff)
		{
			byte[] nextCounterBlock = this.GetNextCounterBlock();
			GcmUtilities.Xor(nextCounterBlock, block);
			Array.Copy(nextCounterBlock, 0, output, outOff, 16);
			this.gHASHBlock(this.S, this.forEncryption ? nextCounterBlock : block);
			this.totalLength += 16uL;
		}

		private void gCTRPartial(byte[] buf, int off, int len, byte[] output, int outOff)
		{
			byte[] nextCounterBlock = this.GetNextCounterBlock();
			GcmUtilities.Xor(nextCounterBlock, buf, off, len);
			Array.Copy(nextCounterBlock, 0, output, outOff, len);
			this.gHASHPartial(this.S, this.forEncryption ? nextCounterBlock : buf, 0, len);
			this.totalLength += (ulong)len;
		}

		private void gHASH(byte[] Y, byte[] b, int len)
		{
			for (int i = 0; i < len; i += 16)
			{
				int len2 = Math.Min(len - i, 16);
				this.gHASHPartial(Y, b, i, len2);
			}
		}

		private void gHASHBlock(byte[] Y, byte[] b)
		{
			GcmUtilities.Xor(Y, b);
			this.multiplier.MultiplyH(Y);
		}

		private void gHASHPartial(byte[] Y, byte[] b, int off, int len)
		{
			GcmUtilities.Xor(Y, b, off, len);
			this.multiplier.MultiplyH(Y);
		}

		private byte[] GetNextCounterBlock()
		{
			for (int i = 15; i >= 12; i--)
			{
				byte[] expr_11_cp_0 = this.counter;
				int expr_11_cp_1 = i;
				if ((expr_11_cp_0[expr_11_cp_1] += 1) != 0)
				{
					break;
				}
			}
			byte[] array = new byte[16];
			this.cipher.ProcessBlock(this.counter, 0, array, 0);
			return array;
		}
	}
}
