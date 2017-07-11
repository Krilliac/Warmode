using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Cms
{
	public class CmsAuthenticatedGenerator : CmsEnvelopedGenerator
	{
		public CmsAuthenticatedGenerator()
		{
		}

		public CmsAuthenticatedGenerator(SecureRandom rand) : base(rand)
		{
		}
	}
}
