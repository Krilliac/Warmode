using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class VmpcEngine : IStreamCipher
	{
		protected byte n;

		protected byte[] P;

		protected byte s;

		protected byte[] workingIV;

		protected byte[] workingKey;

		public virtual string AlgorithmName
		{
			get
			{
				return "VMPC";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is ParametersWithIV))
			{
				throw new ArgumentException("VMPC Init parameters must include an IV");
			}
			ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
			if (!(parametersWithIV.Parameters is KeyParameter))
			{
				throw new ArgumentException("VMPC Init parameters must include a key");
			}
			KeyParameter keyParameter = (KeyParameter)parametersWithIV.Parameters;
			this.workingIV = parametersWithIV.GetIV();
			if (this.workingIV == null || this.workingIV.Length < 1 || this.workingIV.Length > 768)
			{
				throw new ArgumentException("VMPC requires 1 to 768 bytes of IV");
			}
			this.workingKey = keyParameter.GetKey();
			this.InitKey(this.workingKey, this.workingIV);
		}

		protected virtual void InitKey(byte[] keyBytes, byte[] ivBytes)
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

		public virtual void ProcessBytes(byte[] input, int inOff, int len, byte[] output, int outOff)
		{
			Check.DataLength(input, inOff, len, "input buffer too short");
			Check.OutputLength(output, outOff, len, "output buffer too short");
			for (int i = 0; i < len; i++)
			{
				this.s = this.P[(int)(this.s + this.P[(int)(this.n & 255)] & 255)];
				byte b = this.P[(int)(this.P[(int)(this.P[(int)(this.s & 255)] & 255)] + 1 & 255)];
				byte b2 = this.P[(int)(this.n & 255)];
				this.P[(int)(this.n & 255)] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b2;
				this.n = (this.n + 1 & 255);
				output[i + outOff] = (input[i + inOff] ^ b);
			}
		}

		public virtual void Reset()
		{
			this.InitKey(this.workingKey, this.workingIV);
		}

		public virtual byte ReturnByte(byte input)
		{
			this.s = this.P[(int)(this.s + this.P[(int)(this.n & 255)] & 255)];
			byte b = this.P[(int)(this.P[(int)(this.P[(int)(this.s & 255)] & 255)] + 1 & 255)];
			byte b2 = this.P[(int)(this.n & 255)];
			this.P[(int)(this.n & 255)] = this.P[(int)(this.s & 255)];
			this.P[(int)(this.s & 255)] = b2;
			this.n = (this.n + 1 & 255);
			return input ^ b;
		}
	}
}
