using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
	public class BasicGcmExponentiator : IGcmExponentiator
	{
		private uint[] x;

		public void Init(byte[] x)
		{
			this.x = GcmUtilities.AsUints(x);
		}

		public void ExponentiateX(long pow, byte[] output)
		{
			uint[] array = GcmUtilities.OneAsUints();
			if (pow > 0L)
			{
				uint[] y = Arrays.Clone(this.x);
				do
				{
					if ((pow & 1L) != 0L)
					{
						GcmUtilities.Multiply(array, y);
					}
					GcmUtilities.Multiply(y, y);
					pow >>= 1;
				}
				while (pow > 0L);
			}
			GcmUtilities.AsBytes(array, output);
		}
	}
}
