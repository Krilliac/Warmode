using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class CcmBlockCipher : IAeadBlockCipher
	{
		private static readonly int BlockSize = 16;

		private readonly IBlockCipher cipher;

		private readonly byte[] macBlock;

		private bool forEncryption;

		private byte[] nonce;

		private byte[] initialAssociatedText;

		private int macSize;

		private ICipherParameters keyParam;

		private readonly MemoryStream associatedText = new MemoryStream();

		private readonly MemoryStream data = new MemoryStream();

		public virtual string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName + "/CCM";
			}
		}

		public CcmBlockCipher(IBlockCipher cipher)
		{
			this.cipher = cipher;
			this.macBlock = new byte[CcmBlockCipher.BlockSize];
			if (cipher.GetBlockSize() != CcmBlockCipher.BlockSize)
			{
				throw new ArgumentException("cipher required with a block size of " + CcmBlockCipher.BlockSize + ".");
			}
		}

		public virtual IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			ICipherParameters cipherParameters;
			if (parameters is AeadParameters)
			{
				AeadParameters aeadParameters = (AeadParameters)parameters;
				this.nonce = aeadParameters.GetNonce();
				this.initialAssociatedText = aeadParameters.GetAssociatedText();
				this.macSize = aeadParameters.MacSize / 8;
				cipherParameters = aeadParameters.Key;
			}
			else
			{
				if (!(parameters is ParametersWithIV))
				{
					throw new ArgumentException("invalid parameters passed to CCM");
				}
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				this.nonce = parametersWithIV.GetIV();
				this.initialAssociatedText = null;
				this.macSize = this.macBlock.Length / 2;
				cipherParameters = parametersWithIV.Parameters;
			}
			if (cipherParameters != null)
			{
				this.keyParam = cipherParameters;
			}
			if (this.nonce == null || this.nonce.Length < 7 || this.nonce.Length > 13)
			{
				throw new ArgumentException("nonce must have length from 7 to 13 octets");
			}
			this.Reset();
		}

		public virtual int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public virtual void ProcessAadByte(byte input)
		{
			this.associatedText.WriteByte(input);
		}

		public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
		{
			this.associatedText.Write(inBytes, inOff, len);
		}

		public virtual int ProcessByte(byte input, byte[] outBytes, int outOff)
		{
			this.data.WriteByte(input);
			return 0;
		}

		public virtual int ProcessBytes(byte[] inBytes, int inOff, int inLen, byte[] outBytes, int outOff)
		{
			Check.DataLength(inBytes, inOff, inLen, "Input buffer too short");
			this.data.Write(inBytes, inOff, inLen);
			return 0;
		}

		public virtual int DoFinal(byte[] outBytes, int outOff)
		{
			int result = this.ProcessPacket(this.data.GetBuffer(), 0, (int)this.data.Position, outBytes, outOff);
			this.Reset();
			return result;
		}

		public virtual void Reset()
		{
			this.cipher.Reset();
			this.associatedText.SetLength(0L);
			this.data.SetLength(0L);
		}

		public virtual byte[] GetMac()
		{
			return Arrays.CopyOfRange(this.macBlock, 0, this.macSize);
		}

		public virtual int GetUpdateOutputSize(int len)
		{
			return 0;
		}

		public virtual int GetOutputSize(int len)
		{
			int num = (int)this.data.Length + len;
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

		public virtual byte[] ProcessPacket(byte[] input, int inOff, int inLen)
		{
			byte[] array;
			if (this.forEncryption)
			{
				array = new byte[inLen + this.macSize];
			}
			else
			{
				if (inLen < this.macSize)
				{
					throw new InvalidCipherTextException("data too short");
				}
				array = new byte[inLen - this.macSize];
			}
			this.ProcessPacket(input, inOff, inLen, array, 0);
			return array;
		}

		public virtual int ProcessPacket(byte[] input, int inOff, int inLen, byte[] output, int outOff)
		{
			if (this.keyParam == null)
			{
				throw new InvalidOperationException("CCM cipher unitialized.");
			}
			int num = this.nonce.Length;
			int num2 = 15 - num;
			if (num2 < 4)
			{
				int num3 = 1 << 8 * num2;
				if (inLen >= num3)
				{
					throw new InvalidOperationException("CCM packet too large for choice of q.");
				}
			}
			byte[] array = new byte[CcmBlockCipher.BlockSize];
			array[0] = (byte)(num2 - 1 & 7);
			this.nonce.CopyTo(array, 1);
			IBlockCipher blockCipher = new SicBlockCipher(this.cipher);
			blockCipher.Init(this.forEncryption, new ParametersWithIV(this.keyParam, array));
			int i = inOff;
			int num4 = outOff;
			int num5;
			if (this.forEncryption)
			{
				num5 = inLen + this.macSize;
				Check.OutputLength(output, outOff, num5, "Output buffer too short.");
				this.calculateMac(input, inOff, inLen, this.macBlock);
				blockCipher.ProcessBlock(this.macBlock, 0, this.macBlock, 0);
				while (i < inOff + inLen - CcmBlockCipher.BlockSize)
				{
					blockCipher.ProcessBlock(input, i, output, num4);
					num4 += CcmBlockCipher.BlockSize;
					i += CcmBlockCipher.BlockSize;
				}
				byte[] array2 = new byte[CcmBlockCipher.BlockSize];
				Array.Copy(input, i, array2, 0, inLen + inOff - i);
				blockCipher.ProcessBlock(array2, 0, array2, 0);
				Array.Copy(array2, 0, output, num4, inLen + inOff - i);
				Array.Copy(this.macBlock, 0, output, outOff + inLen, this.macSize);
			}
			else
			{
				if (inLen < this.macSize)
				{
					throw new InvalidCipherTextException("data too short");
				}
				num5 = inLen - this.macSize;
				Check.OutputLength(output, outOff, num5, "Output buffer too short.");
				Array.Copy(input, inOff + num5, this.macBlock, 0, this.macSize);
				blockCipher.ProcessBlock(this.macBlock, 0, this.macBlock, 0);
				for (int num6 = this.macSize; num6 != this.macBlock.Length; num6++)
				{
					this.macBlock[num6] = 0;
				}
				while (i < inOff + num5 - CcmBlockCipher.BlockSize)
				{
					blockCipher.ProcessBlock(input, i, output, num4);
					num4 += CcmBlockCipher.BlockSize;
					i += CcmBlockCipher.BlockSize;
				}
				byte[] array3 = new byte[CcmBlockCipher.BlockSize];
				Array.Copy(input, i, array3, 0, num5 - (i - inOff));
				blockCipher.ProcessBlock(array3, 0, array3, 0);
				Array.Copy(array3, 0, output, num4, num5 - (i - inOff));
				byte[] b = new byte[CcmBlockCipher.BlockSize];
				this.calculateMac(output, outOff, num5, b);
				if (!Arrays.ConstantTimeAreEqual(this.macBlock, b))
				{
					throw new InvalidCipherTextException("mac check in CCM failed");
				}
			}
			return num5;
		}

		private int calculateMac(byte[] data, int dataOff, int dataLen, byte[] macBlock)
		{
			IMac mac = new CbcBlockCipherMac(this.cipher, this.macSize * 8);
			mac.Init(this.keyParam);
			byte[] array = new byte[16];
			if (this.HasAssociatedText())
			{
				byte[] expr_37_cp_0 = array;
				int expr_37_cp_1 = 0;
				expr_37_cp_0[expr_37_cp_1] |= 64;
			}
			byte[] expr_4D_cp_0 = array;
			int expr_4D_cp_1 = 0;
			expr_4D_cp_0[expr_4D_cp_1] |= (byte)(((mac.GetMacSize() - 2) / 2 & 7) << 3);
			byte[] expr_70_cp_0 = array;
			int expr_70_cp_1 = 0;
			expr_70_cp_0[expr_70_cp_1] |= (byte)(15 - this.nonce.Length - 1 & 7);
			Array.Copy(this.nonce, 0, array, 1, this.nonce.Length);
			int i = dataLen;
			int num = 1;
			while (i > 0)
			{
				array[array.Length - num] = (byte)(i & 255);
				i >>= 8;
				num++;
			}
			mac.BlockUpdate(array, 0, array.Length);
			if (this.HasAssociatedText())
			{
				int associatedTextLength = this.GetAssociatedTextLength();
				int num2;
				if (associatedTextLength < 65280)
				{
					mac.Update((byte)(associatedTextLength >> 8));
					mac.Update((byte)associatedTextLength);
					num2 = 2;
				}
				else
				{
					mac.Update(255);
					mac.Update(254);
					mac.Update((byte)(associatedTextLength >> 24));
					mac.Update((byte)(associatedTextLength >> 16));
					mac.Update((byte)(associatedTextLength >> 8));
					mac.Update((byte)associatedTextLength);
					num2 = 6;
				}
				if (this.initialAssociatedText != null)
				{
					mac.BlockUpdate(this.initialAssociatedText, 0, this.initialAssociatedText.Length);
				}
				if (this.associatedText.Position > 0L)
				{
					mac.BlockUpdate(this.associatedText.GetBuffer(), 0, (int)this.associatedText.Position);
				}
				num2 = (num2 + associatedTextLength) % 16;
				if (num2 != 0)
				{
					for (int j = num2; j < 16; j++)
					{
						mac.Update(0);
					}
				}
			}
			mac.BlockUpdate(data, dataOff, dataLen);
			return mac.DoFinal(macBlock, 0);
		}

		private int GetAssociatedTextLength()
		{
			return (int)this.associatedText.Length + ((this.initialAssociatedText == null) ? 0 : this.initialAssociatedText.Length);
		}

		private bool HasAssociatedText()
		{
			return this.GetAssociatedTextLength() > 0;
		}
	}
}
