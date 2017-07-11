using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Modes
{
	public class EaxBlockCipher : IAeadBlockCipher
	{
		private enum Tag : byte
		{
			N,
			H,
			C
		}

		private SicBlockCipher cipher;

		private bool forEncryption;

		private int blockSize;

		private IMac mac;

		private byte[] nonceMac;

		private byte[] associatedTextMac;

		private byte[] macBlock;

		private int macSize;

		private byte[] bufBlock;

		private int bufOff;

		private bool cipherInitialized;

		private byte[] initialAssociatedText;

		public virtual string AlgorithmName
		{
			get
			{
				return this.cipher.GetUnderlyingCipher().AlgorithmName + "/EAX";
			}
		}

		public EaxBlockCipher(IBlockCipher cipher)
		{
			this.blockSize = cipher.GetBlockSize();
			this.mac = new CMac(cipher);
			this.macBlock = new byte[this.blockSize];
			this.associatedTextMac = new byte[this.mac.GetMacSize()];
			this.nonceMac = new byte[this.mac.GetMacSize()];
			this.cipher = new SicBlockCipher(cipher);
		}

		public virtual IBlockCipher GetUnderlyingCipher()
		{
			return this.cipher;
		}

		public virtual int GetBlockSize()
		{
			return this.cipher.GetBlockSize();
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			byte[] array;
			ICipherParameters parameters2;
			if (parameters is AeadParameters)
			{
				AeadParameters aeadParameters = (AeadParameters)parameters;
				array = aeadParameters.GetNonce();
				this.initialAssociatedText = aeadParameters.GetAssociatedText();
				this.macSize = aeadParameters.MacSize / 8;
				parameters2 = aeadParameters.Key;
			}
			else
			{
				if (!(parameters is ParametersWithIV))
				{
					throw new ArgumentException("invalid parameters passed to EAX");
				}
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				array = parametersWithIV.GetIV();
				this.initialAssociatedText = null;
				this.macSize = this.mac.GetMacSize() / 2;
				parameters2 = parametersWithIV.Parameters;
			}
			this.bufBlock = new byte[forEncryption ? this.blockSize : (this.blockSize + this.macSize)];
			byte[] array2 = new byte[this.blockSize];
			this.mac.Init(parameters2);
			array2[this.blockSize - 1] = 0;
			this.mac.BlockUpdate(array2, 0, this.blockSize);
			this.mac.BlockUpdate(array, 0, array.Length);
			this.mac.DoFinal(this.nonceMac, 0);
			this.cipher.Init(true, new ParametersWithIV(null, this.nonceMac));
			this.Reset();
		}

		private void InitCipher()
		{
			if (this.cipherInitialized)
			{
				return;
			}
			this.cipherInitialized = true;
			this.mac.DoFinal(this.associatedTextMac, 0);
			byte[] array = new byte[this.blockSize];
			array[this.blockSize - 1] = 2;
			this.mac.BlockUpdate(array, 0, this.blockSize);
		}

		private void CalculateMac()
		{
			byte[] array = new byte[this.blockSize];
			this.mac.DoFinal(array, 0);
			for (int i = 0; i < this.macBlock.Length; i++)
			{
				this.macBlock[i] = (this.nonceMac[i] ^ this.associatedTextMac[i] ^ array[i]);
			}
		}

		public virtual void Reset()
		{
			this.Reset(true);
		}

		private void Reset(bool clearMac)
		{
			this.cipher.Reset();
			this.mac.Reset();
			this.bufOff = 0;
			Array.Clear(this.bufBlock, 0, this.bufBlock.Length);
			if (clearMac)
			{
				Array.Clear(this.macBlock, 0, this.macBlock.Length);
			}
			byte[] array = new byte[this.blockSize];
			array[this.blockSize - 1] = 1;
			this.mac.BlockUpdate(array, 0, this.blockSize);
			this.cipherInitialized = false;
			if (this.initialAssociatedText != null)
			{
				this.ProcessAadBytes(this.initialAssociatedText, 0, this.initialAssociatedText.Length);
			}
		}

		public virtual void ProcessAadByte(byte input)
		{
			if (this.cipherInitialized)
			{
				throw new InvalidOperationException("AAD data cannot be added after encryption/decryption processing has begun.");
			}
			this.mac.Update(input);
		}

		public virtual void ProcessAadBytes(byte[] inBytes, int inOff, int len)
		{
			if (this.cipherInitialized)
			{
				throw new InvalidOperationException("AAD data cannot be added after encryption/decryption processing has begun.");
			}
			this.mac.BlockUpdate(inBytes, inOff, len);
		}

		public virtual int ProcessByte(byte input, byte[] outBytes, int outOff)
		{
			this.InitCipher();
			return this.Process(input, outBytes, outOff);
		}

		public virtual int ProcessBytes(byte[] inBytes, int inOff, int len, byte[] outBytes, int outOff)
		{
			this.InitCipher();
			int num = 0;
			for (int num2 = 0; num2 != len; num2++)
			{
				num += this.Process(inBytes[inOff + num2], outBytes, outOff + num);
			}
			return num;
		}

		public virtual int DoFinal(byte[] outBytes, int outOff)
		{
			this.InitCipher();
			int num = this.bufOff;
			byte[] array = new byte[this.bufBlock.Length];
			this.bufOff = 0;
			if (this.forEncryption)
			{
				Check.OutputLength(outBytes, outOff, num + this.macSize, "Output buffer too short");
				this.cipher.ProcessBlock(this.bufBlock, 0, array, 0);
				Array.Copy(array, 0, outBytes, outOff, num);
				this.mac.BlockUpdate(array, 0, num);
				this.CalculateMac();
				Array.Copy(this.macBlock, 0, outBytes, outOff + num, this.macSize);
				this.Reset(false);
				return num + this.macSize;
			}
			if (num < this.macSize)
			{
				throw new InvalidCipherTextException("data too short");
			}
			Check.OutputLength(outBytes, outOff, num - this.macSize, "Output buffer too short");
			if (num > this.macSize)
			{
				this.mac.BlockUpdate(this.bufBlock, 0, num - this.macSize);
				this.cipher.ProcessBlock(this.bufBlock, 0, array, 0);
				Array.Copy(array, 0, outBytes, outOff, num - this.macSize);
			}
			this.CalculateMac();
			if (!this.VerifyMac(this.bufBlock, num - this.macSize))
			{
				throw new InvalidCipherTextException("mac check in EAX failed");
			}
			this.Reset(false);
			return num - this.macSize;
		}

		public virtual byte[] GetMac()
		{
			byte[] array = new byte[this.macSize];
			Array.Copy(this.macBlock, 0, array, 0, this.macSize);
			return array;
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
			return num - num % this.blockSize;
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

		private int Process(byte b, byte[] outBytes, int outOff)
		{
			this.bufBlock[this.bufOff++] = b;
			if (this.bufOff == this.bufBlock.Length)
			{
				Check.OutputLength(outBytes, outOff, this.blockSize, "Output buffer is too short");
				int result;
				if (this.forEncryption)
				{
					result = this.cipher.ProcessBlock(this.bufBlock, 0, outBytes, outOff);
					this.mac.BlockUpdate(outBytes, outOff, this.blockSize);
				}
				else
				{
					this.mac.BlockUpdate(this.bufBlock, 0, this.blockSize);
					result = this.cipher.ProcessBlock(this.bufBlock, 0, outBytes, outOff);
				}
				this.bufOff = 0;
				if (!this.forEncryption)
				{
					Array.Copy(this.bufBlock, this.blockSize, this.bufBlock, 0, this.macSize);
					this.bufOff = this.macSize;
				}
				return result;
			}
			return 0;
		}

		private bool VerifyMac(byte[] mac, int off)
		{
			int num = 0;
			for (int i = 0; i < this.macSize; i++)
			{
				num |= (int)(this.macBlock[i] ^ mac[off + i]);
			}
			return num == 0;
		}
	}
}
