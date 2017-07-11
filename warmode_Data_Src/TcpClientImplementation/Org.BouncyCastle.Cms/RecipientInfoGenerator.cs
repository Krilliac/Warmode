using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Cms
{
	internal interface RecipientInfoGenerator
	{
		RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random);
	}
}
