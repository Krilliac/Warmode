using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class BaseDigestCalculator : IDigestCalculator
	{
		private readonly byte[] digest;

		internal BaseDigestCalculator(byte[] digest)
		{
			this.digest = digest;
		}

		public byte[] GetDigest()
		{
			return Arrays.Clone(this.digest);
		}
	}
}
