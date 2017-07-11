using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class ISO9797Alg3Mac : IMac
	{
		private byte[] mac;

		private byte[] buf;

		private int bufOff;

		private IBlockCipher cipher;

		private IBlockCipherPadding padding;

		private int macSize;

		private KeyParameter lastKey2;

		private KeyParameter lastKey3;

		public string AlgorithmName
		{
			get
			{
				return "ISO9797Alg3";
			}
		}

		public ISO9797Alg3Mac(IBlockCipher cipher) : this(cipher, cipher.GetBlockSize() * 8, null)
		{
		}

		public ISO9797Alg3Mac(IBlockCipher cipher, IBlockCipherPadding padding) : this(cipher, cipher.GetBlockSize() * 8, padding)
		{
		}

		public ISO9797Alg3Mac(IBlockCipher cipher, int macSizeInBits) : this(cipher, macSizeInBits, null)
		{
		}

		public ISO9797Alg3Mac(IBlockCipher cipher, int macSizeInBits, IBlockCipherPadding padding)
		{
			if (macSizeInBits % 8 != 0)
			{
				throw new ArgumentException("MAC size must be multiple of 8");
			}
			if (!(cipher is DesEngine))
			{
				throw new ArgumentException("cipher must be instance of DesEngine");
			}
			this.cipher = new CbcBlockCipher(cipher);
			this.padding = padding;
			this.macSize = macSizeInBits / 8;
			this.mac = new byte[cipher.GetBlockSize()];
			this.buf = new byte[cipher.GetBlockSize()];
			this.bufOff = 0;
		}

		public void Init(ICipherParameters parameters)
		{
			this.Reset();
			if (!(parameters is KeyParameter) && !(parameters is ParametersWithIV))
			{
				throw new ArgumentException("parameters must be an instance of KeyParameter or ParametersWithIV");
			}
			KeyParameter keyParameter;
			if (parameters is KeyParameter)
			{
				keyParameter = (KeyParameter)parameters;
			}
			else
			{
				keyParameter = (KeyParameter)((ParametersWithIV)parameters).Parameters;
			}
			byte[] key = keyParameter.GetKey();
			KeyParameter parameters2;
			if (key.Length == 16)
			{
				parameters2 = new KeyParameter(key, 0, 8);
				this.lastKey2 = new KeyParameter(key, 8, 8);
				this.lastKey3 = parameters2;
			}
			else
			{
				if (key.Length != 24)
				{
					throw new ArgumentException("Key must be either 112 or 168 bit long");
				}
				parameters2 = new KeyParameter(key, 0, 8);
				this.lastKey2 = new KeyParameter(key, 8, 8);
				this.lastKey3 = new KeyParameter(key, 16, 8);
			}
			if (parameters is ParametersWithIV)
			{
				this.cipher.Init(true, new ParametersWithIV(parameters2, ((ParametersWithIV)parameters).GetIV()));
				return;
			}
			this.cipher.Init(true, parameters2);
		}

		public int GetMacSize()
		{
			return this.macSize;
		}

		public void Update(byte input)
		{
			if (this.bufOff == this.buf.Length)
			{
				this.cipher.ProcessBlock(this.buf, 0, this.mac, 0);
				this.bufOff = 0;
			}
			this.buf[this.bufOff++] = input;
		}

		public void BlockUpdate(byte[] input, int inOff, int len)
		{
			if (len < 0)
			{
				throw new ArgumentException("Can't have a negative input length!");
			}
			int blockSize = this.cipher.GetBlockSize();
			int num = 0;
			int num2 = blockSize - this.bufOff;
			if (len > num2)
			{
				Array.Copy(input, inOff, this.buf, this.bufOff, num2);
				num += this.cipher.ProcessBlock(this.buf, 0, this.mac, 0);
				this.bufOff = 0;
				len -= num2;
				inOff += num2;
				while (len > blockSize)
				{
					num += this.cipher.ProcessBlock(input, inOff, this.mac, 0);
					len -= blockSize;
					inOff += blockSize;
				}
			}
			Array.Copy(input, inOff, this.buf, this.bufOff, len);
			this.bufOff += len;
		}

		public int DoFinal(byte[] output, int outOff)
		{
			int blockSize = this.cipher.GetBlockSize();
			if (this.padding == null)
			{
				while (this.bufOff < blockSize)
				{
					this.buf[this.bufOff++] = 0;
				}
			}
			else
			{
				if (this.bufOff == blockSize)
				{
					this.cipher.ProcessBlock(this.buf, 0, this.mac, 0);
					this.bufOff = 0;
				}
				this.padding.AddPadding(this.buf, this.bufOff);
			}
			this.cipher.ProcessBlock(this.buf, 0, this.mac, 0);
			DesEngine desEngine = new DesEngine();
			desEngine.Init(false, this.lastKey2);
			desEngine.ProcessBlock(this.mac, 0, this.mac, 0);
			desEngine.Init(true, this.lastKey3);
			desEngine.ProcessBlock(this.mac, 0, this.mac, 0);
			Array.Copy(this.mac, 0, output, outOff, this.macSize);
			this.Reset();
			return this.macSize;
		}

		public void Reset()
		{
			Array.Clear(this.buf, 0, this.buf.Length);
			this.bufOff = 0;
			this.cipher.Reset();
		}
	}
}
