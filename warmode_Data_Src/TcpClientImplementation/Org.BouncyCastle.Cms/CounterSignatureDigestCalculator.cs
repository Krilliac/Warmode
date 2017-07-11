using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class CounterSignatureDigestCalculator : IDigestCalculator
	{
		private readonly string alg;

		private readonly byte[] data;

		internal CounterSignatureDigestCalculator(string alg, byte[] data)
		{
			this.alg = alg;
			this.data = data;
		}

		public byte[] GetDigest()
		{
			IDigest digestInstance = CmsSignedHelper.Instance.GetDigestInstance(this.alg);
			return DigestUtilities.DoFinal(digestInstance, this.data);
		}
	}
}
