using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class IsaacEngine : IStreamCipher
	{
		private static readonly int sizeL = 8;

		private static readonly int stateArraySize = IsaacEngine.sizeL << 5;

		private uint[] engineState;

		private uint[] results;

		private uint a;

		private uint b;

		private uint c;

		private int index;

		private byte[] keyStream = new byte[IsaacEngine.stateArraySize << 2];

		private byte[] workingKey;

		private bool initialised;

		public virtual string AlgorithmName
		{
			get
			{
				return "ISAAC";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to ISAAC Init - " + parameters.GetType().Name, "parameters");
			}
			KeyParameter keyParameter = (KeyParameter)parameters;
			this.setKey(keyParameter.GetKey());
		}

		public virtual byte ReturnByte(byte input)
		{
			if (this.index == 0)
			{
				this.isaac();
				this.keyStream = Pack.UInt32_To_BE(this.results);
			}
			byte result = this.keyStream[this.index] ^ input;
			this.index = (this.index + 1 & 1023);
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
				if (this.index == 0)
				{
					this.isaac();
					this.keyStream = Pack.UInt32_To_BE(this.results);
				}
				output[i + outOff] = (this.keyStream[this.index] ^ input[i + inOff]);
				this.index = (this.index + 1 & 1023);
			}
		}

		public virtual void Reset()
		{
			this.setKey(this.workingKey);
		}

		private void setKey(byte[] keyBytes)
		{
			this.workingKey = keyBytes;
			if (this.engineState == null)
			{
				this.engineState = new uint[IsaacEngine.stateArraySize];
			}
			if (this.results == null)
			{
				this.results = new uint[IsaacEngine.stateArraySize];
			}
			for (int i = 0; i < IsaacEngine.stateArraySize; i++)
			{
				this.engineState[i] = (this.results[i] = 0u);
			}
			this.a = (this.b = (this.c = 0u));
			this.index = 0;
			byte[] array = new byte[keyBytes.Length + (keyBytes.Length & 3)];
			Array.Copy(keyBytes, 0, array, 0, keyBytes.Length);
			for (int i = 0; i < array.Length; i += 4)
			{
				this.results[i >> 2] = Pack.LE_To_UInt32(array, i);
			}
			uint[] array2 = new uint[IsaacEngine.sizeL];
			for (int i = 0; i < IsaacEngine.sizeL; i++)
			{
				array2[i] = 2654435769u;
			}
			for (int i = 0; i < 4; i++)
			{
				this.mix(array2);
			}
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < IsaacEngine.stateArraySize; j += IsaacEngine.sizeL)
				{
					for (int k = 0; k < IsaacEngine.sizeL; k++)
					{
						array2[k] += ((i < 1) ? this.results[j + k] : this.engineState[j + k]);
					}
					this.mix(array2);
					for (int k = 0; k < IsaacEngine.sizeL; k++)
					{
						this.engineState[j + k] = array2[k];
					}
				}
			}
			this.isaac();
			this.initialised = true;
		}

		private void isaac()
		{
			this.b += (this.c += 1u);
			for (int i = 0; i < IsaacEngine.stateArraySize; i++)
			{
				uint num = this.engineState[i];
				switch (i & 3)
				{
				case 0:
					this.a ^= this.a << 13;
					break;
				case 1:
					this.a ^= this.a >> 6;
					break;
				case 2:
					this.a ^= this.a << 2;
					break;
				case 3:
					this.a ^= this.a >> 16;
					break;
				}
				this.a += this.engineState[i + 128 & 255];
				uint num2 = this.engineState[i] = this.engineState[(int)(num >> 2 & 255u)] + this.a + this.b;
				this.results[i] = (this.b = this.engineState[(int)(num2 >> 10 & 255u)] + num);
			}
		}

		private void mix(uint[] x)
		{
			x[0] ^= x[1] << 11;
			x[3] += x[0];
			x[1] += x[2];
			x[1] ^= x[2] >> 2;
			x[4] += x[1];
			x[2] += x[3];
			x[2] ^= x[3] << 8;
			x[5] += x[2];
			x[3] += x[4];
			x[3] ^= x[4] >> 16;
			x[6] += x[3];
			x[4] += x[5];
			x[4] ^= x[5] << 10;
			x[7] += x[4];
			x[5] += x[6];
			x[5] ^= x[6] >> 4;
			x[0] += x[5];
			x[6] += x[7];
			x[6] ^= x[7] << 8;
			x[1] += x[6];
			x[7] += x[0];
			x[7] ^= x[0] >> 9;
			x[2] += x[7];
			x[0] += x[1];
		}
	}
}
