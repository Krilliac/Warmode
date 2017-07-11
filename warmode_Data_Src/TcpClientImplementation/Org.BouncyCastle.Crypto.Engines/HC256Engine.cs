using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class HC256Engine : IStreamCipher
	{
		private uint[] p = new uint[1024];

		private uint[] q = new uint[1024];

		private uint cnt;

		private byte[] key;

		private byte[] iv;

		private bool initialised;

		private byte[] buf = new byte[4];

		private int idx;

		public virtual string AlgorithmName
		{
			get
			{
				return "HC-256";
			}
		}

		private uint Step()
		{
			uint num = this.cnt & 1023u;
			uint result;
			if (this.cnt < 1024u)
			{
				uint num2 = this.p[(int)((UIntPtr)(num - 3u & 1023u))];
				uint num3 = this.p[(int)((UIntPtr)(num - 1023u & 1023u))];
				this.p[(int)((UIntPtr)num)] += this.p[(int)((UIntPtr)(num - 10u & 1023u))] + (HC256Engine.RotateRight(num2, 10) ^ HC256Engine.RotateRight(num3, 23)) + this.q[(int)((UIntPtr)((num2 ^ num3) & 1023u))];
				num2 = this.p[(int)((UIntPtr)(num - 12u & 1023u))];
				result = (this.q[(int)((UIntPtr)(num2 & 255u))] + this.q[(int)((UIntPtr)((num2 >> 8 & 255u) + 256u))] + this.q[(int)((UIntPtr)((num2 >> 16 & 255u) + 512u))] + this.q[(int)((UIntPtr)((num2 >> 24 & 255u) + 768u))] ^ this.p[(int)((UIntPtr)num)]);
			}
			else
			{
				uint num4 = this.q[(int)((UIntPtr)(num - 3u & 1023u))];
				uint num5 = this.q[(int)((UIntPtr)(num - 1023u & 1023u))];
				this.q[(int)((UIntPtr)num)] += this.q[(int)((UIntPtr)(num - 10u & 1023u))] + (HC256Engine.RotateRight(num4, 10) ^ HC256Engine.RotateRight(num5, 23)) + this.p[(int)((UIntPtr)((num4 ^ num5) & 1023u))];
				num4 = this.q[(int)((UIntPtr)(num - 12u & 1023u))];
				result = (this.p[(int)((UIntPtr)(num4 & 255u))] + this.p[(int)((UIntPtr)((num4 >> 8 & 255u) + 256u))] + this.p[(int)((UIntPtr)((num4 >> 16 & 255u) + 512u))] + this.p[(int)((UIntPtr)((num4 >> 24 & 255u) + 768u))] ^ this.q[(int)((UIntPtr)num)]);
			}
			this.cnt = (this.cnt + 1u & 2047u);
			return result;
		}

		private void Init()
		{
			if (this.key.Length != 32 && this.key.Length != 16)
			{
				throw new ArgumentException("The key must be 128/256 bits long");
			}
			if (this.iv.Length < 16)
			{
				throw new ArgumentException("The IV must be at least 128 bits long");
			}
			if (this.key.Length != 32)
			{
				byte[] destinationArray = new byte[32];
				Array.Copy(this.key, 0, destinationArray, 0, this.key.Length);
				Array.Copy(this.key, 0, destinationArray, 16, this.key.Length);
				this.key = destinationArray;
			}
			if (this.iv.Length < 32)
			{
				byte[] array = new byte[32];
				Array.Copy(this.iv, 0, array, 0, this.iv.Length);
				Array.Copy(this.iv, 0, array, this.iv.Length, array.Length - this.iv.Length);
				this.iv = array;
			}
			this.cnt = 0u;
			uint[] array2 = new uint[2560];
			for (int i = 0; i < 32; i++)
			{
				array2[i >> 2] |= (uint)((uint)this.key[i] << 8 * (i & 3));
			}
			for (int j = 0; j < 32; j++)
			{
				array2[(j >> 2) + 8] |= (uint)((uint)this.iv[j] << 8 * (j & 3));
			}
			for (uint num = 16u; num < 2560u; num += 1u)
			{
				uint num2 = array2[(int)((UIntPtr)(num - 2u))];
				uint num3 = array2[(int)((UIntPtr)(num - 15u))];
				array2[(int)((UIntPtr)num)] = (HC256Engine.RotateRight(num2, 17) ^ HC256Engine.RotateRight(num2, 19) ^ num2 >> 10) + array2[(int)((UIntPtr)(num - 7u))] + (HC256Engine.RotateRight(num3, 7) ^ HC256Engine.RotateRight(num3, 18) ^ num3 >> 3) + array2[(int)((UIntPtr)(num - 16u))] + num;
			}
			Array.Copy(array2, 512, this.p, 0, 1024);
			Array.Copy(array2, 1536, this.q, 0, 1024);
			for (int k = 0; k < 4096; k++)
			{
				this.Step();
			}
			this.cnt = 0u;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			ICipherParameters cipherParameters = parameters;
			if (parameters is ParametersWithIV)
			{
				this.iv = ((ParametersWithIV)parameters).GetIV();
				cipherParameters = ((ParametersWithIV)parameters).Parameters;
			}
			else
			{
				this.iv = new byte[0];
			}
			if (cipherParameters is KeyParameter)
			{
				this.key = ((KeyParameter)cipherParameters).GetKey();
				this.Init();
				this.initialised = true;
				return;
			}
			throw new ArgumentException("Invalid parameter passed to HC256 init - " + parameters.GetType().Name, "parameters");
		}

		private byte GetByte()
		{
			if (this.idx == 0)
			{
				Pack.UInt32_To_LE(this.Step(), this.buf);
			}
			byte result = this.buf[this.idx];
			this.idx = (this.idx + 1 & 3);
			return result;
		}

		public virtual void ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
		{
			if (!this.initialised)
			{
				throw new InvalidOperationException(this.AlgorithmName + " not initialised");
			}
			Check.DataLength(input, inOff, len, "input buffer too short");
			Check.OutputLength(output, outOff, len, "output buffer too short");
			for (int i = 0; i < len; i++)
			{
				output[outOff + i] = (input[inOff + i] ^ this.GetByte());
			}
		}

		public virtual void Reset()
		{
			this.idx = 0;
			this.Init();
		}

		public virtual byte ReturnByte(byte input)
		{
			return input ^ this.GetByte();
		}

		private static uint RotateRight(uint x, int bits)
		{
			return x >> bits | x << -bits;
		}
	}
}
