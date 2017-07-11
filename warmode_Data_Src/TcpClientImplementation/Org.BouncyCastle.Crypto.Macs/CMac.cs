using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class CMac : IMac
	{
		private const byte CONSTANT_128 = 135;

		private const byte CONSTANT_64 = 27;

		private byte[] ZEROES;

		private byte[] mac;

		private byte[] buf;

		private int bufOff;

		private IBlockCipher cipher;

		private int macSize;

		private byte[] L;

		private byte[] Lu;

		private byte[] Lu2;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.AlgorithmName;
			}
		}

		public CMac(IBlockCipher cipher) : this(cipher, cipher.GetBlockSize() * 8)
		{
		}

		public CMac(IBlockCipher cipher, int macSizeInBits)
		{
			if (macSizeInBits % 8 != 0)
			{
				throw new ArgumentException("MAC size must be multiple of 8");
			}
			if (macSizeInBits > cipher.GetBlockSize() * 8)
			{
				throw new ArgumentException("MAC size must be less or equal to " + cipher.GetBlockSize() * 8);
			}
			if (cipher.GetBlockSize() != 8 && cipher.GetBlockSize() != 16)
			{
				throw new ArgumentException("Block size must be either 64 or 128 bits");
			}
			this.cipher = new CbcBlockCipher(cipher);
			this.macSize = macSizeInBits / 8;
			this.mac = new byte[cipher.GetBlockSize()];
			this.buf = new byte[cipher.GetBlockSize()];
			this.ZEROES = new byte[cipher.GetBlockSize()];
			this.bufOff = 0;
		}

		private static int ShiftLeft(byte[] block, byte[] output)
		{
			int num = block.Length;
			uint num2 = 0u;
			while (--num >= 0)
			{
				uint num3 = (uint)block[num];
				output[num] = (byte)(num3 << 1 | num2);
				num2 = (num3 >> 7 & 1u);
			}
			return (int)num2;
		}

		private static byte[] DoubleLu(byte[] input)
		{
			byte[] array = new byte[input.Length];
			int num = CMac.ShiftLeft(input, array);
			int num2 = (input.Length == 16) ? 135 : 27;
			byte[] expr_2D_cp_0 = array;
			int expr_2D_cp_1 = input.Length - 1;
			expr_2D_cp_0[expr_2D_cp_1] ^= (byte)(num2 >> (1 - num << 3));
			return array;
		}

		public void Init(ICipherParameters parameters)
		{
			if (parameters is KeyParameter)
			{
				this.cipher.Init(true, parameters);
				this.L = new byte[this.ZEROES.Length];
				this.cipher.ProcessBlock(this.ZEROES, 0, this.L, 0);
				this.Lu = CMac.DoubleLu(this.L);
				this.Lu2 = CMac.DoubleLu(this.Lu);
			}
			else if (parameters != null)
			{
				throw new ArgumentException("CMac mode only permits key to be set.", "parameters");
			}
			this.Reset();
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

		public void BlockUpdate(byte[] inBytes, int inOff, int len)
		{
			if (len < 0)
			{
				throw new ArgumentException("Can't have a negative input length!");
			}
			int blockSize = this.cipher.GetBlockSize();
			int num = blockSize - this.bufOff;
			if (len > num)
			{
				Array.Copy(inBytes, inOff, this.buf, this.bufOff, num);
				this.cipher.ProcessBlock(this.buf, 0, this.mac, 0);
				this.bufOff = 0;
				len -= num;
				inOff += num;
				while (len > blockSize)
				{
					this.cipher.ProcessBlock(inBytes, inOff, this.mac, 0);
					len -= blockSize;
					inOff += blockSize;
				}
			}
			Array.Copy(inBytes, inOff, this.buf, this.bufOff, len);
			this.bufOff += len;
		}

		public int DoFinal(byte[] outBytes, int outOff)
		{
			int blockSize = this.cipher.GetBlockSize();
			byte[] array;
			if (this.bufOff == blockSize)
			{
				array = this.Lu;
			}
			else
			{
				new ISO7816d4Padding().AddPadding(this.buf, this.bufOff);
				array = this.Lu2;
			}
			for (int i = 0; i < this.mac.Length; i++)
			{
				byte[] expr_4C_cp_0 = this.buf;
				int expr_4C_cp_1 = i;
				expr_4C_cp_0[expr_4C_cp_1] ^= array[i];
			}
			this.cipher.ProcessBlock(this.buf, 0, this.mac, 0);
			Array.Copy(this.mac, 0, outBytes, outOff, this.macSize);
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
