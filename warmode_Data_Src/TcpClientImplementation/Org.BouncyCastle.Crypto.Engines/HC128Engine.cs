using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class HC128Engine : IStreamCipher
	{
		private uint[] p = new uint[512];

		private uint[] q = new uint[512];

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
				return "HC-128";
			}
		}

		private static uint F1(uint x)
		{
			return HC128Engine.RotateRight(x, 7) ^ HC128Engine.RotateRight(x, 18) ^ x >> 3;
		}

		private static uint F2(uint x)
		{
			return HC128Engine.RotateRight(x, 17) ^ HC128Engine.RotateRight(x, 19) ^ x >> 10;
		}

		private uint G1(uint x, uint y, uint z)
		{
			return (HC128Engine.RotateRight(x, 10) ^ HC128Engine.RotateRight(z, 23)) + HC128Engine.RotateRight(y, 8);
		}

		private uint G2(uint x, uint y, uint z)
		{
			return (HC128Engine.RotateLeft(x, 10) ^ HC128Engine.RotateLeft(z, 23)) + HC128Engine.RotateLeft(y, 8);
		}

		private static uint RotateLeft(uint x, int bits)
		{
			return x << bits | x >> -bits;
		}

		private static uint RotateRight(uint x, int bits)
		{
			return x >> bits | x << -bits;
		}

		private uint H1(uint x)
		{
			return this.q[(int)((UIntPtr)(x & 255u))] + this.q[(int)((UIntPtr)((x >> 16 & 255u) + 256u))];
		}

		private uint H2(uint x)
		{
			return this.p[(int)((UIntPtr)(x & 255u))] + this.p[(int)((UIntPtr)((x >> 16 & 255u) + 256u))];
		}

		private static uint Mod1024(uint x)
		{
			return x & 1023u;
		}

		private static uint Mod512(uint x)
		{
			return x & 511u;
		}

		private static uint Dim(uint x, uint y)
		{
			return HC128Engine.Mod512(x - y);
		}

		private uint Step()
		{
			uint num = HC128Engine.Mod512(this.cnt);
			uint result;
			if (this.cnt < 512u)
			{
				this.p[(int)((UIntPtr)num)] += this.G1(this.p[(int)((UIntPtr)HC128Engine.Dim(num, 3u))], this.p[(int)((UIntPtr)HC128Engine.Dim(num, 10u))], this.p[(int)((UIntPtr)HC128Engine.Dim(num, 511u))]);
				result = (this.H1(this.p[(int)((UIntPtr)HC128Engine.Dim(num, 12u))]) ^ this.p[(int)((UIntPtr)num)]);
			}
			else
			{
				this.q[(int)((UIntPtr)num)] += this.G2(this.q[(int)((UIntPtr)HC128Engine.Dim(num, 3u))], this.q[(int)((UIntPtr)HC128Engine.Dim(num, 10u))], this.q[(int)((UIntPtr)HC128Engine.Dim(num, 511u))]);
				result = (this.H2(this.q[(int)((UIntPtr)HC128Engine.Dim(num, 12u))]) ^ this.q[(int)((UIntPtr)num)]);
			}
			this.cnt = HC128Engine.Mod1024(this.cnt + 1u);
			return result;
		}

		private void Init()
		{
			if (this.key.Length != 16)
			{
				throw new ArgumentException("The key must be 128 bits long");
			}
			this.cnt = 0u;
			uint[] array = new uint[1280];
			for (int i = 0; i < 16; i++)
			{
				array[i >> 2] |= (uint)((uint)this.key[i] << 8 * (i & 3));
			}
			Array.Copy(array, 0, array, 4, 4);
			int num = 0;
			while (num < this.iv.Length && num < 16)
			{
				array[(num >> 2) + 8] |= (uint)((uint)this.iv[num] << 8 * (num & 3));
				num++;
			}
			Array.Copy(array, 8, array, 12, 4);
			for (uint num2 = 16u; num2 < 1280u; num2 += 1u)
			{
				array[(int)((UIntPtr)num2)] = HC128Engine.F2(array[(int)((UIntPtr)(num2 - 2u))]) + array[(int)((UIntPtr)(num2 - 7u))] + HC128Engine.F1(array[(int)((UIntPtr)(num2 - 15u))]) + array[(int)((UIntPtr)(num2 - 16u))] + num2;
			}
			Array.Copy(array, 256, this.p, 0, 512);
			Array.Copy(array, 768, this.q, 0, 512);
			for (int j = 0; j < 512; j++)
			{
				this.p[j] = this.Step();
			}
			for (int k = 0; k < 512; k++)
			{
				this.q[k] = this.Step();
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
			throw new ArgumentException("Invalid parameter passed to HC128 init - " + parameters.GetType().Name, "parameters");
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
	}
}
