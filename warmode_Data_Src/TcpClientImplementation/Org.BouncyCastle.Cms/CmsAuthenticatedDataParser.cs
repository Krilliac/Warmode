using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsAuthenticatedDataParser : CmsContentInfoParser
	{
		internal RecipientInformationStore _recipientInfoStore;

		internal AuthenticatedDataParser authData;

		private AlgorithmIdentifier macAlg;

		private byte[] mac;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable authAttrs;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable unauthAttrs;

		private bool authAttrNotRead;

		private bool unauthAttrNotRead;

		public AlgorithmIdentifier MacAlgorithmID
		{
			get
			{
				return this.macAlg;
			}
		}

		public string MacAlgOid
		{
			get
			{
				return this.macAlg.ObjectID.Id;
			}
		}

		public Asn1Object MacAlgParams
		{
			get
			{
				Asn1Encodable parameters = this.macAlg.Parameters;
				if (parameters != null)
				{
					return parameters.ToAsn1Object();
				}
				return null;
			}
		}

		public CmsAuthenticatedDataParser(byte[] envelopedData) : this(new MemoryStream(envelopedData, false))
		{
		}

		public CmsAuthenticatedDataParser(Stream envelopedData) : base(envelopedData)
		{
			this.authAttrNotRead = true;
			this.authData = new AuthenticatedDataParser((Asn1SequenceParser)this.contentInfo.GetContent(16));
			Asn1Set instance = Asn1Set.GetInstance(this.authData.GetRecipientInfos().ToAsn1Object());
			this.macAlg = this.authData.GetMacAlgorithm();
			ContentInfoParser enapsulatedContentInfo = this.authData.GetEnapsulatedContentInfo();
			CmsReadable readable = new CmsProcessableInputStream(((Asn1OctetStringParser)enapsulatedContentInfo.GetContent(4)).GetOctetStream());
			CmsSecureReadable secureReadable = new CmsEnvelopedHelper.CmsAuthenticatedSecureReadable(this.macAlg, readable);
			this._recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(instance, secureReadable);
		}

		public RecipientInformationStore GetRecipientInfos()
		{
			return this._recipientInfoStore;
		}

		public byte[] GetMac()
		{
			if (this.mac == null)
			{
				this.GetAuthAttrs();
				this.mac = this.authData.GetMac().GetOctets();
			}
			return Arrays.Clone(this.mac);
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable GetAuthAttrs()
		{
			if (this.authAttrs == null && this.authAttrNotRead)
			{
				Asn1SetParser asn1SetParser = this.authData.GetAuthAttrs();
				this.authAttrNotRead = false;
				if (asn1SetParser != null)
				{
					Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
					IAsn1Convertible asn1Convertible;
					while ((asn1Convertible = asn1SetParser.ReadObject()) != null)
					{
						Asn1SequenceParser asn1SequenceParser = (Asn1SequenceParser)asn1Convertible;
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							asn1SequenceParser.ToAsn1Object()
						});
					}
					this.authAttrs = new Org.BouncyCastle.Asn1.Cms.AttributeTable(new DerSet(asn1EncodableVector));
				}
			}
			return this.authAttrs;
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable GetUnauthAttrs()
		{
			if (this.unauthAttrs == null && this.unauthAttrNotRead)
			{
				Asn1SetParser asn1SetParser = this.authData.GetUnauthAttrs();
				this.unauthAttrNotRead = false;
				if (asn1SetParser != null)
				{
					Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
					IAsn1Convertible asn1Convertible;
					while ((asn1Convertible = asn1SetParser.ReadObject()) != null)
					{
						Asn1SequenceParser asn1SequenceParser = (Asn1SequenceParser)asn1Convertible;
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							asn1SequenceParser.ToAsn1Object()
						});
					}
					this.unauthAttrs = new Org.BouncyCastle.Asn1.Cms.AttributeTable(new DerSet(asn1EncodableVector));
				}
			}
			return this.unauthAttrs;
		}
	}
}
