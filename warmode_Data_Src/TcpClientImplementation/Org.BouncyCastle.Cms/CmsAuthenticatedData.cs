using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsAuthenticatedData
	{
		internal RecipientInformationStore recipientInfoStore;

		internal ContentInfo contentInfo;

		private AlgorithmIdentifier macAlg;

		private Asn1Set authAttrs;

		private Asn1Set unauthAttrs;

		private byte[] mac;

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

		public ContentInfo ContentInfo
		{
			get
			{
				return this.contentInfo;
			}
		}

		public CmsAuthenticatedData(byte[] authData) : this(CmsUtilities.ReadContentInfo(authData))
		{
		}

		public CmsAuthenticatedData(Stream authData) : this(CmsUtilities.ReadContentInfo(authData))
		{
		}

		public CmsAuthenticatedData(ContentInfo contentInfo)
		{
			this.contentInfo = contentInfo;
			AuthenticatedData instance = AuthenticatedData.GetInstance(contentInfo.Content);
			Asn1Set recipientInfos = instance.RecipientInfos;
			this.macAlg = instance.MacAlgorithm;
			ContentInfo encapsulatedContentInfo = instance.EncapsulatedContentInfo;
			CmsReadable readable = new CmsProcessableByteArray(Asn1OctetString.GetInstance(encapsulatedContentInfo.Content).GetOctets());
			CmsSecureReadable secureReadable = new CmsEnvelopedHelper.CmsAuthenticatedSecureReadable(this.macAlg, readable);
			this.recipientInfoStore = CmsEnvelopedHelper.BuildRecipientInformationStore(recipientInfos, secureReadable);
			this.authAttrs = instance.AuthAttrs;
			this.mac = instance.Mac.GetOctets();
			this.unauthAttrs = instance.UnauthAttrs;
		}

		public byte[] GetMac()
		{
			return Arrays.Clone(this.mac);
		}

		public RecipientInformationStore GetRecipientInfos()
		{
			return this.recipientInfoStore;
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable GetAuthAttrs()
		{
			if (this.authAttrs == null)
			{
				return null;
			}
			return new Org.BouncyCastle.Asn1.Cms.AttributeTable(this.authAttrs);
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable GetUnauthAttrs()
		{
			if (this.unauthAttrs == null)
			{
				return null;
			}
			return new Org.BouncyCastle.Asn1.Cms.AttributeTable(this.unauthAttrs);
		}

		public byte[] GetEncoded()
		{
			return this.contentInfo.GetEncoded();
		}
	}
}
