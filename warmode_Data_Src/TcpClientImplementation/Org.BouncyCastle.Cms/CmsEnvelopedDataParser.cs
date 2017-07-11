using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsEnvelopedDataParser : CmsContentInfoParser
	{
		internal RecipientInformationStore recipientInfoStore;

		internal EnvelopedDataParser envelopedData;

		private AlgorithmIdentifier _encAlg;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable _unprotectedAttributes;

		private bool _attrNotRead;

		public AlgorithmIdentifier EncryptionAlgorithmID
		{
			get
			{
				return this._encAlg;
			}
		}

		public string EncryptionAlgOid
		{
			get
			{
				return this._encAlg.ObjectID.Id;
			}
		}

		public Asn1Object EncryptionAlgParams
		{
			get
			{
				Asn1Encodable parameters = this._encAlg.Parameters;
				if (parameters != null)
				{
					return parameters.ToAsn1Object();
				}
				return null;
			}
		}

		public CmsEnvelopedDataParser(byte[] envelopedData) : this(new MemoryStream(envelopedData, false))
		{
		}

		public CmsEnvelopedDataParser(Stream envelopedData) : base(envelopedData)
		{
			this._attrNotRead = true;
			this.envelopedData = new EnvelopedDataParser((Asn1SequenceParser)this.contentInfo.GetContent(16));
			Asn1Set instance = Asn1Set.GetInstance(this.envelopedData.GetRecipientInfos().ToAsn1Object());
			EncryptedContentInfoParser encryptedContentInfo = this.envelopedData.GetEncryptedContentInfo();
			this._encAlg = encryptedContentInfo.ContentEncryptionAlgorithm;
			CmsReadable readable = new CmsProcessableInputStream(((Asn1OctetStringParser)encryptedContentInfo.GetEncryptedContent(4)).GetOctetStream());
			CmsSecureReadable secureReadable = new CmsEnvelopedHelper.CmsEnvelopedSecureReadable(this._encAlg, readable);
			this.recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(instance, secureReadable);
		}

		public RecipientInformationStore GetRecipientInfos()
		{
			return this.recipientInfoStore;
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable GetUnprotectedAttributes()
		{
			if (this._unprotectedAttributes == null && this._attrNotRead)
			{
				Asn1SetParser unprotectedAttrs = this.envelopedData.GetUnprotectedAttrs();
				this._attrNotRead = false;
				if (unprotectedAttrs != null)
				{
					Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
					IAsn1Convertible asn1Convertible;
					while ((asn1Convertible = unprotectedAttrs.ReadObject()) != null)
					{
						Asn1SequenceParser asn1SequenceParser = (Asn1SequenceParser)asn1Convertible;
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							asn1SequenceParser.ToAsn1Object()
						});
					}
					this._unprotectedAttributes = new Org.BouncyCastle.Asn1.Cms.AttributeTable(new DerSet(asn1EncodableVector));
				}
			}
			return this._unprotectedAttributes;
		}
	}
}
