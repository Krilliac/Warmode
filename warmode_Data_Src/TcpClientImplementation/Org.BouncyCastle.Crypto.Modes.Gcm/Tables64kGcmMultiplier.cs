using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	public class Tables64kGcmMultiplier : IGcmMultiplier
	{
		private byte[] H;

		private uint[][][] M;

		public void Init(byte[] H)
		{
			if (this.M == null)
			{
				this.M = new uint[16][][];
			}
			else if (Arrays.AreEqual(this.H, H))
			{
				return;
			}
			this.H = Arrays.Clone(H);
			this.M[0] = new uint[256][];
			this.M[0][0] = new uint[4];
			this.M[0][128] = GcmUtilities.AsUints(H);
			for (int i = 64; i >= 1; i >>= 1)
			{
				uint[] array = (uint[])this.M[0][i + i].Clone();
				GcmUtilities.MultiplyP(array);
				this.M[0][i] = array;
			}
			int num = 0;
			while (true)
			{
				for (int j = 2; j < 256; j += j)
				{
					for (int k = 1; k < j; k++)
					{
						uint[] array2 = (uint[])this.M[num][j].Clone();
						GcmUtilities.Xor(array2, this.M[num][k]);
						this.M[num][j + k] = array2;
					}
				}
				if (++num == 16)
				{
					break;
				}
				this.M[num] = new uint[256][];
				this.M[num][0] = new uint[4];
				for (int l = 128; l > 0; l >>= 1)
				{
					uint[] array3 = (uint[])this.M[num - 1][l].Clone();
					GcmUtilities.MultiplyP8(array3);
					this.M[num][l] = array3;
				}
			}
		}

		public void MultiplyH(byte[] x)
		{
			uint[] array = new uint[4];
			for (int num = 0; num != 16; num++)
			{
				uint[] array2 = this.M[num][(int)x[num]];
				array[0] ^= array2[0];
				array[1] ^= array2[1];
				array[2] ^= array2[2];
				array[3] ^= array2[3];
			}
			Pack.UInt32_To_BE(array, x, 0);
		}
	}
}
