using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class Gost28147Mac : IMac
	{
		private const int blockSize = 8;

		private const int macSize = 4;

		private int bufOff;

		private byte[] buf;

		private byte[] mac;

		private bool firstStep = true;

		private int[] workingKey;

		private byte[] S = new byte[]
		{
			9,
			6,
			3,
			2,
			8,
			11,
			1,
			7,
			10,
			4,
			14,
			15,
			12,
			0,
			13,
			5,
			3,
			7,
			14,
			9,
			8,
			10,
			15,
			0,
			5,
			2,
			6,
			12,
			11,
			4,
			13,
			1,
			14,
			4,
			6,
			2,
			11,
			3,
			13,
			8,
			12,
			15,
			5,
			10,
			0,
			7,
			1,
			9,
			14,
			7,
			10,
			12,
			13,
			1,
			3,
			9,
			0,
			2,
			11,
			4,
			15,
			8,
			5,
			6,
			11,
			5,
			1,
			9,
			8,
			13,
			15,
			0,
			14,
			4,
			2,
			3,
			12,
			7,
			10,
			6,
			3,
			10,
			13,
			12,
			1,
			2,
			0,
			11,
			7,
			5,
			9,
			4,
			8,
			15,
			14,
			6,
			1,
			13,
			2,
			9,
			7,
			10,
			6,
			0,
			8,
			12,
			4,
			5,
			15,
			3,
			11,
			14,
			11,
			10,
			15,
			5,
			0,
			12,
			14,
			8,
			6,
			2,
			3,
			9,
			1,
			7,
			13,
			4
		};

		public string AlgorithmName
		{
			get
			{
				return "Gost28147Mac";
			}
		}

		public Gost28147Mac()
		{
			this.mac = new byte[8];
			this.buf = new byte[8];
			this.bufOff = 0;
		}

		private static int[] generateWorkingKey(byte[] userKey)
		{
			if (userKey.Length != 32)
			{
				throw new ArgumentException("Key length invalid. Key needs to be 32 byte - 256 bit!!!");
			}
			int[] array = new int[8];
			for (int num = 0; num != 8; num++)
			{
				array[num] = Gost28147Mac.bytesToint(userKey, num * 4);
			}
			return array;
		}

		public void Init(ICipherParameters parameters)
		{
			this.Reset();
			this.buf = new byte[8];
			if (parameters is ParametersWithSBox)
			{
				ParametersWithSBox parametersWithSBox = (ParametersWithSBox)parameters;
				parametersWithSBox.GetSBox().CopyTo(this.S, 0);
				if (parametersWithSBox.Parameters != null)
				{
					this.workingKey = Gost28147Mac.generateWorkingKey(((KeyParameter)parametersWithSBox.Parameters).GetKey());
					return;
				}
				return;
			}
			else
			{
				if (parameters is KeyParameter)
				{
					this.workingKey = Gost28147Mac.generateWorkingKey(((KeyParameter)parameters).GetKey());
					return;
				}
				throw new ArgumentException("invalid parameter passed to Gost28147 init - " + parameters.GetType().Name);
			}
		}

		public int GetMacSize()
		{
			return 4;
		}

		private int gost28147_mainStep(int n1, int key)
		{
			int num = key + n1;
			int num2 = (int)this.S[num & 15];
			num2 += (int)this.S[16 + (num >> 4 & 15)] << 4;
			num2 += (int)this.S[32 + (num >> 8 & 15)] << 8;
			num2 += (int)this.S[48 + (num >> 12 & 15)] << 12;
			num2 += (int)this.S[64 + (num >> 16 & 15)] << 16;
			num2 += (int)this.S[80 + (num >> 20 & 15)] << 20;
			num2 += (int)this.S[96 + (num >> 24 & 15)] << 24;
			num2 += (int)this.S[112 + (num >> 28 & 15)] << 28;
			int num3 = num2 << 11;
			int num4 = (int)((uint)num2 >> 21);
			return num3 | num4;
		}

		private void gost28147MacFunc(int[] workingKey, byte[] input, int inOff, byte[] output, int outOff)
		{
			int num = Gost28147Mac.bytesToint(input, inOff);
			int num2 = Gost28147Mac.bytesToint(input, inOff + 4);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num3 = num;
					num = (num2 ^ this.gost28147_mainStep(num, workingKey[j]));
					num2 = num3;
				}
			}
			Gost28147Mac.intTobytes(num, output, outOff);
			Gost28147Mac.intTobytes(num2, output, outOff + 4);
		}

		private static int bytesToint(byte[] input, int inOff)
		{
			return (int)((long)((long)input[inOff + 3] << 24) & (long)((ulong)-16777216)) + ((int)input[inOff + 2] << 16 & 16711680) + ((int)input[inOff + 1] << 8 & 65280) + (int)(input[inOff] & 255);
		}

		private static void intTobytes(int num, byte[] output, int outOff)
		{
			output[outOff + 3] = (byte)(num >> 24);
			output[outOff + 2] = (byte)(num >> 16);
			output[outOff + 1] = (byte)(num >> 8);
			output[outOff] = (byte)num;
		}

		private static byte[] CM5func(byte[] buf, int bufOff, byte[] mac)
		{
			byte[] array = new byte[buf.Length - bufOff];
			Array.Copy(buf, bufOff, array, 0, mac.Length);
			for (int num = 0; num != mac.Length; num++)
			{
				array[num] ^= mac[num];
			}
			return array;
		}

		public void Update(byte input)
		{
			if (this.bufOff == this.buf.Length)
			{
				byte[] array = new byte[this.buf.Length];
				Array.Copy(this.buf, 0, array, 0, this.mac.Length);
				if (this.firstStep)
				{
					this.firstStep = false;
				}
				else
				{
					array = Gost28147Mac.CM5func(this.buf, 0, this.mac);
				}
				this.gost28147MacFunc(this.workingKey, array, 0, this.mac, 0);
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
			int num = 8 - this.bufOff;
			if (len > num)
			{
				Array.Copy(input, inOff, this.buf, this.bufOff, num);
				byte[] array = new byte[this.buf.Length];
				Array.Copy(this.buf, 0, array, 0, this.mac.Length);
				if (this.firstStep)
				{
					this.firstStep = false;
				}
				else
				{
					array = Gost28147Mac.CM5func(this.buf, 0, this.mac);
				}
				this.gost28147MacFunc(this.workingKey, array, 0, this.mac, 0);
				this.bufOff = 0;
				len -= num;
				inOff += num;
				while (len > 8)
				{
					array = Gost28147Mac.CM5func(input, inOff, this.mac);
					this.gost28147MacFunc(this.workingKey, array, 0, this.mac, 0);
					len -= 8;
					inOff += 8;
				}
			}
			Array.Copy(input, inOff, this.buf, this.bufOff, len);
			this.bufOff += len;
		}

		public int DoFinal(byte[] output, int outOff)
		{
			while (this.bufOff < 8)
			{
				this.buf[this.bufOff++] = 0;
			}
			byte[] array = new byte[this.buf.Length];
			Array.Copy(this.buf, 0, array, 0, this.mac.Length);
			if (this.firstStep)
			{
				this.firstStep = false;
			}
			else
			{
				array = Gost28147Mac.CM5func(this.buf, 0, this.mac);
			}
			this.gost28147MacFunc(this.workingKey, array, 0, this.mac, 0);
			Array.Copy(this.mac, this.mac.Length / 2 - 4, output, outOff, 4);
			this.Reset();
			return 4;
		}

		public void Reset()
		{
			Array.Clear(this.buf, 0, this.buf.Length);
			this.bufOff = 0;
			this.firstStep = true;
		}
	}
}
