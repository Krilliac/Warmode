using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsEnvelopedData
	{
		internal RecipientInformationStore recipientInfoStore;

		internal ContentInfo contentInfo;

		private AlgorithmIdentifier encAlg;

		private Asn1Set unprotectedAttributes;

		public AlgorithmIdentifier EncryptionAlgorithmID
		{
			get
			{
				return this.encAlg;
			}
		}

		public string EncryptionAlgOid
		{
			get
			{
				return this.encAlg.ObjectID.Id;
			}
		}

		public ContentInfo ContentInfo
		{
			get
			{
				return this.contentInfo;
			}
		}

		public CmsEnvelopedData(byte[] envelopedData) : this(CmsUtilities.ReadContentInfo(envelopedData))
		{
		}

		public CmsEnvelopedData(Stream envelopedData) : this(CmsUtilities.ReadContentInfo(envelopedData))
		{
		}

		public CmsEnvelopedData(ContentInfo contentInfo)
		{
			this.contentInfo = contentInfo;
			EnvelopedData instance = EnvelopedData.GetInstance(contentInfo.Content);
			Asn1Set recipientInfos = instance.RecipientInfos;
			EncryptedContentInfo encryptedContentInfo = instance.EncryptedContentInfo;
			this.encAlg = encryptedContentInfo.ContentEncryptionAlgorithm;
			CmsReadable readable = new CmsProcessableByteArray(encryptedContentInfo.EncryptedContent.GetOctets());
			CmsSecureReadable secureReadable = new CmsEnvelopedHelper.CmsEnvelopedSecureReadable(this.encAlg, readable);
			this.recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(recipientInfos, secureReadable);
			this.unprotectedAttributes = instance.UnprotectedAttrs;
		}

		public RecipientInformationStore GetRecipientInfos()
		{
			return this.recipientInfoStore;
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable GetUnprotectedAttributes()
		{
			if (this.unprotectedAttributes == null)
			{
				return null;
			}
			return new Org.BouncyCastle.Asn1.Cms.AttributeTable(this.unprotectedAttributes);
		}

		public byte[] GetEncoded()
		{
			return this.contentInfo.GetEncoded();
		}
	}
}
