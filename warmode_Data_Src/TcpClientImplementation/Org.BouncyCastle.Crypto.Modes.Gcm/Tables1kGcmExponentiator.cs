using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	public class Tables1kGcmExponentiator : IGcmExponentiator
	{
		private IList lookupPowX2;

		public void Init(byte[] x)
		{
			uint[] array = GcmUtilities.AsUints(x);
			if (this.lookupPowX2 != null && Arrays.AreEqual(array, (uint[])this.lookupPowX2[0]))
			{
				return;
			}
			this.lookupPowX2 = Platform.CreateArrayList(8);
			this.lookupPowX2.Add(array);
		}

		public void ExponentiateX(long pow, byte[] output)
		{
			uint[] x = GcmUtilities.OneAsUints();
			int num = 0;
			while (pow > 0L)
			{
				if ((pow & 1L) != 0L)
				{
					this.EnsureAvailable(num);
					GcmUtilities.Multiply(x, (uint[])this.lookupPowX2[num]);
				}
				num++;
				pow >>= 1;
			}
			GcmUtilities.AsBytes(x, output);
		}

		private void EnsureAvailable(int bit)
		{
			int num = this.lookupPowX2.Count;
			if (num <= bit)
			{
				uint[] array = (uint[])this.lookupPowX2[num - 1];
				do
				{
					array = Arrays.Clone(array);
					GcmUtilities.Multiply(array, array);
					this.lookupPowX2.Add(array);
				}
				while (++num <= bit);
			}
		}
	}
}
