using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	internal class KeyTransRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private TbsCertificateStructure recipientTbsCert;

		private AsymmetricKeyParameter recipientPublicKey;

		private Asn1OctetString subjectKeyIdentifier;

		private SubjectPublicKeyInfo info;

		internal X509Certificate RecipientCert
		{
			set
			{
				this.recipientTbsCert = CmsUtilities.GetTbsCertificateStructure(value);
				this.recipientPublicKey = value.GetPublicKey();
				this.info = this.recipientTbsCert.SubjectPublicKeyInfo;
			}
		}

		internal AsymmetricKeyParameter RecipientPublicKey
		{
			set
			{
				this.recipientPublicKey = value;
				try
				{
					this.info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(this.recipientPublicKey);
				}
				catch (IOException)
				{
					throw new ArgumentException("can't extract key algorithm from this key");
				}
			}
		}

		internal Asn1OctetString SubjectKeyIdentifier
		{
			set
			{
				this.subjectKeyIdentifier = value;
			}
		}

		internal KeyTransRecipientInfoGenerator()
		{
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] key = contentEncryptionKey.GetKey();
			AlgorithmIdentifier algorithmID = this.info.AlgorithmID;
			IWrapper wrapper = KeyTransRecipientInfoGenerator.Helper.CreateWrapper(algorithmID.ObjectID.Id);
			wrapper.Init(true, new ParametersWithRandom(this.recipientPublicKey, random));
			byte[] str = wrapper.Wrap(key, 0, key.Length);
			RecipientIdentifier rid;
			if (this.recipientTbsCert != null)
			{
				IssuerAndSerialNumber id = new IssuerAndSerialNumber(this.recipientTbsCert.Issuer, this.recipientTbsCert.SerialNumber.Value);
				rid = new RecipientIdentifier(id);
			}
			else
			{
				rid = new RecipientIdentifier(this.subjectKeyIdentifier);
			}
			return new RecipientInfo(new KeyTransRecipientInfo(rid, algorithmID, new DerOctetString(str)));
		}
	}
}
