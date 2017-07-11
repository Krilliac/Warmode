using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class VmpcMac : IMac
	{
		private byte g;

		private byte n;

		private byte[] P;

		private byte s;

		private byte[] T;

		private byte[] workingIV;

		private byte[] workingKey;

		private byte x1;

		private byte x2;

		private byte x3;

		private byte x4;

		public virtual string AlgorithmName
		{
			get
			{
				return "VMPC-MAC";
			}
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			for (int i = 1; i < 25; i++)
			{
				this.s = this.P[(int)(this.s + this.P[(int)(this.n & 255)] & 255)];
				this.x4 = this.P[(int)(this.x4 + this.x3) + i & 255];
				this.x3 = this.P[(int)(this.x3 + this.x2) + i & 255];
				this.x2 = this.P[(int)(this.x2 + this.x1) + i & 255];
				this.x1 = this.P[(int)(this.x1 + this.s) + i & 255];
				this.T[(int)(this.g & 31)] = (this.T[(int)(this.g & 31)] ^ this.x1);
				this.T[(int)(this.g + 1 & 31)] = (this.T[(int)(this.g + 1 & 31)] ^ this.x2);
				this.T[(int)(this.g + 2 & 31)] = (this.T[(int)(this.g + 2 & 31)] ^ this.x3);
				this.T[(int)(this.g + 3 & 31)] = (this.T[(int)(this.g + 3 & 31)] ^ this.x4);
				this.g = (this.g + 4 & 31);
				byte b = this.P[(int)(this.n & 255)];
				this.P[(int)(this.n & 255)] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b;
				this.n = (this.n + 1 & 255);
			}
			for (int j = 0; j < 768; j++)
			{
				this.s = this.P[(int)(this.s + this.P[j & 255] + this.T[j & 31] & 255)];
				byte b2 = this.P[j & 255];
				this.P[j & 255] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b2;
			}
			byte[] array = new byte[20];
			for (int k = 0; k < 20; k++)
			{
				this.s = this.P[(int)(this.s + this.P[k & 255] & 255)];
				array[k] = this.P[(int)(this.P[(int)(this.P[(int)(this.s & 255)] & 255)] + 1 & 255)];
				byte b3 = this.P[k & 255];
				this.P[k & 255] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b3;
			}
			Array.Copy(array, 0, output, outOff, array.Length);
			this.Reset();
			return array.Length;
		}

		public virtual int GetMacSize()
		{
			return 20;
		}

		public virtual void Init(ICipherParameters parameters)
		{
			if (!(parameters is ParametersWithIV))
			{
				throw new ArgumentException("VMPC-MAC Init parameters must include an IV", "parameters");
			}
			ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
			KeyParameter keyParameter = (KeyParameter)parametersWithIV.Parameters;
			if (!(parametersWithIV.Parameters is KeyParameter))
			{
				throw new ArgumentException("VMPC-MAC Init parameters must include a key", "parameters");
			}
			this.workingIV = parametersWithIV.GetIV();
			if (this.workingIV == null || this.workingIV.Length < 1 || this.workingIV.Length > 768)
			{
				throw new ArgumentException("VMPC-MAC requires 1 to 768 bytes of IV", "parameters");
			}
			this.workingKey = keyParameter.GetKey();
			this.Reset();
		}

		private void initKey(byte[] keyBytes, byte[] ivBytes)
		{
			this.s = 0;
			this.P = new byte[256];
			for (int i = 0; i < 256; i++)
			{
				this.P[i] = (byte)i;
			}
			for (int j = 0; j < 768; j++)
			{
				this.s = this.P[(int)(this.s + this.P[j & 255] + keyBytes[j % keyBytes.Length] & 255)];
				byte b = this.P[j & 255];
				this.P[j & 255] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b;
			}
			for (int k = 0; k < 768; k++)
			{
				this.s = this.P[(int)(this.s + this.P[k & 255] + ivBytes[k % ivBytes.Length] & 255)];
				byte b2 = this.P[k & 255];
				this.P[k & 255] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b2;
			}
			this.n = 0;
		}

		public virtual void Reset()
		{
			this.initKey(this.workingKey, this.workingIV);
			this.g = (this.x1 = (this.x2 = (this.x3 = (this.x4 = (this.n = 0)))));
			this.T = new byte[32];
			for (int i = 0; i < 32; i++)
			{
				this.T[i] = 0;
			}
		}

		public virtual void Update(byte input)
		{
			this.s = this.P[(int)(this.s + this.P[(int)(this.n & 255)] & 255)];
			byte b = input ^ this.P[(int)(this.P[(int)(this.P[(int)(this.s & 255)] & 255)] + 1 & 255)];
			this.x4 = this.P[(int)(this.x4 + this.x3 & 255)];
			this.x3 = this.P[(int)(this.x3 + this.x2 & 255)];
			this.x2 = this.P[(int)(this.x2 + this.x1 & 255)];
			this.x1 = this.P[(int)(this.x1 + this.s + b & 255)];
			this.T[(int)(this.g & 31)] = (this.T[(int)(this.g & 31)] ^ this.x1);
			this.T[(int)(this.g + 1 & 31)] = (this.T[(int)(this.g + 1 & 31)] ^ this.x2);
			this.T[(int)(this.g + 2 & 31)] = (this.T[(int)(this.g + 2 & 31)] ^ this.x3);
			this.T[(int)(this.g + 3 & 31)] = (this.T[(int)(this.g + 3 & 31)] ^ this.x4);
			this.g = (this.g + 4 & 31);
			byte b2 = this.P[(int)(this.n & 255)];
			this.P[(int)(this.n & 255)] = this.P[(int)(this.s & 255)];
			this.P[(int)(this.s & 255)] = b2;
			this.n = (this.n + 1 & 255);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			if (inOff + len > input.Length)
			{
				throw new DataLengthException("input buffer too short");
			}
			for (int i = 0; i < len; i++)
			{
				this.Update(input[i]);
			}
		}
	}
}
