using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class VmpcKsa3Engine : VmpcEngine
	{
		public override string AlgorithmName
		{
			get
			{
				return "VMPC-KSA3";
			}
		}

		protected override void InitKey(byte[] keyBytes, byte[] ivBytes)
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
			for (int l = 0; l < 768; l++)
			{
				this.s = this.P[(int)(this.s + this.P[l & 255] + keyBytes[l % keyBytes.Length] & 255)];
				byte b3 = this.P[l & 255];
				this.P[l & 255] = this.P[(int)(this.s & 255)];
				this.P[(int)(this.s & 255)] = b3;
			}
			this.n = 0;
		}
	}
}
