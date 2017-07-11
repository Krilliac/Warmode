using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsSignedData
	{
		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private readonly CmsProcessable signedContent;

		private SignedData signedData;

		private ContentInfo contentInfo;

		private SignerInformationStore signerInfoStore;

		private IX509Store attrCertStore;

		private IX509Store certificateStore;

		private IX509Store crlStore;

		private IDictionary hashes;

		public int Version
		{
			get
			{
				return this.signedData.Version.Value.IntValue;
			}
		}

		[Obsolete("Use 'SignedContentType' property instead.")]
		public string SignedContentTypeOid
		{
			get
			{
				return this.signedData.EncapContentInfo.ContentType.Id;
			}
		}

		public DerObjectIdentifier SignedContentType
		{
			get
			{
				return this.signedData.EncapContentInfo.ContentType;
			}
		}

		public CmsProcessable SignedContent
		{
			get
			{
				return this.signedContent;
			}
		}

		public ContentInfo ContentInfo
		{
			get
			{
				return this.contentInfo;
			}
		}

		private CmsSignedData(CmsSignedData c)
		{
			this.signedData = c.signedData;
			this.contentInfo = c.contentInfo;
			this.signedContent = c.signedContent;
			this.signerInfoStore = c.signerInfoStore;
		}

		public CmsSignedData(byte[] sigBlock) : this(CmsUtilities.ReadContentInfo(new MemoryStream(sigBlock, false)))
		{
		}

		public CmsSignedData(CmsProcessable signedContent, byte[] sigBlock) : this(signedContent, CmsUtilities.ReadContentInfo(new MemoryStream(sigBlock, false)))
		{
		}

		public CmsSignedData(IDictionary hashes, byte[] sigBlock) : this(hashes, CmsUtilities.ReadContentInfo(sigBlock))
		{
		}

		public CmsSignedData(CmsProcessable signedContent, Stream sigData) : this(signedContent, CmsUtilities.ReadContentInfo(sigData))
		{
		}

		public CmsSignedData(Stream sigData) : this(CmsUtilities.ReadContentInfo(sigData))
		{
		}

		public CmsSignedData(CmsProcessable signedContent, ContentInfo sigData)
		{
			this.signedContent = signedContent;
			this.contentInfo = sigData;
			this.signedData = SignedData.GetInstance(this.contentInfo.Content);
		}

		public CmsSignedData(IDictionary hashes, ContentInfo sigData)
		{
			this.hashes = hashes;
			this.contentInfo = sigData;
			this.signedData = SignedData.GetInstance(this.contentInfo.Content);
		}

		public CmsSignedData(ContentInfo sigData)
		{
			this.contentInfo = sigData;
			this.signedData = SignedData.GetInstance(this.contentInfo.Content);
			if (this.signedData.EncapContentInfo.Content != null)
			{
				this.signedContent = new CmsProcessableByteArray(((Asn1OctetString)this.signedData.EncapContentInfo.Content).GetOctets());
			}
		}

		public SignerInformationStore GetSignerInfos()
		{
			if (this.signerInfoStore == null)
			{
				IList list = Platform.CreateArrayList();
				Asn1Set signerInfos = this.signedData.SignerInfos;
				foreach (object current in signerInfos)
				{
					SignerInfo instance = SignerInfo.GetInstance(current);
					DerObjectIdentifier contentType = this.signedData.EncapContentInfo.ContentType;
					if (this.hashes == null)
					{
						list.Add(new SignerInformation(instance, contentType, this.signedContent, null));
					}
					else
					{
						byte[] digest = (byte[])this.hashes[instance.DigestAlgorithm.ObjectID.Id];
						list.Add(new SignerInformation(instance, contentType, null, new BaseDigestCalculator(digest)));
					}
				}
				this.signerInfoStore = new SignerInformationStore(list);
			}
			return this.signerInfoStore;
		}

		public IX509Store GetAttributeCertificates(string type)
		{
			if (this.attrCertStore == null)
			{
				this.attrCertStore = CmsSignedData.Helper.CreateAttributeStore(type, this.signedData.Certificates);
			}
			return this.attrCertStore;
		}

		public IX509Store GetCertificates(string type)
		{
			if (this.certificateStore == null)
			{
				this.certificateStore = CmsSignedData.Helper.CreateCertificateStore(type, this.signedData.Certificates);
			}
			return this.certificateStore;
		}

		public IX509Store GetCrls(string type)
		{
			if (this.crlStore == null)
			{
				this.crlStore = CmsSignedData.Helper.CreateCrlStore(type, this.signedData.CRLs);
			}
			return this.crlStore;
		}

		public byte[] GetEncoded()
		{
			return this.contentInfo.GetEncoded();
		}

		public static CmsSignedData ReplaceSigners(CmsSignedData signedData, SignerInformationStore signerInformationStore)
		{
			CmsSignedData cmsSignedData = new CmsSignedData(signedData);
			cmsSignedData.signerInfoStore = signerInformationStore;
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (SignerInformation signerInformation in signerInformationStore.GetSigners())
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					CmsSignedData.Helper.FixAlgID(signerInformation.DigestAlgorithmID)
				});
				asn1EncodableVector2.Add(new Asn1Encodable[]
				{
					signerInformation.ToSignerInfo()
				});
			}
			Asn1Set asn1Set = new DerSet(asn1EncodableVector);
			Asn1Set asn1Set2 = new DerSet(asn1EncodableVector2);
			Asn1Sequence asn1Sequence = (Asn1Sequence)signedData.signedData.ToAsn1Object();
			asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[]
			{
				asn1Sequence[0],
				asn1Set
			});
			for (int num = 2; num != asn1Sequence.Count - 1; num++)
			{
				asn1EncodableVector2.Add(new Asn1Encodable[]
				{
					asn1Sequence[num]
				});
			}
			asn1EncodableVector2.Add(new Asn1Encodable[]
			{
				asn1Set2
			});
			cmsSignedData.signedData = SignedData.GetInstance(new BerSequence(asn1EncodableVector2));
			cmsSignedData.contentInfo = new ContentInfo(cmsSignedData.contentInfo.ContentType, cmsSignedData.signedData);
			return cmsSignedData;
		}

		public static CmsSignedData ReplaceCertificatesAndCrls(CmsSignedData signedData, IX509Store x509Certs, IX509Store x509Crls, IX509Store x509AttrCerts)
		{
			if (x509AttrCerts != null)
			{
				throw Platform.CreateNotImplementedException("Currently can't replace attribute certificates");
			}
			CmsSignedData cmsSignedData = new CmsSignedData(signedData);
			Asn1Set certificates = null;
			try
			{
				Asn1Set asn1Set = CmsUtilities.CreateBerSetFromList(CmsUtilities.GetCertificatesFromStore(x509Certs));
				if (asn1Set.Count != 0)
				{
					certificates = asn1Set;
				}
			}
			catch (X509StoreException e)
			{
				throw new CmsException("error getting certificates from store", e);
			}
			Asn1Set crls = null;
			try
			{
				Asn1Set asn1Set2 = CmsUtilities.CreateBerSetFromList(CmsUtilities.GetCrlsFromStore(x509Crls));
				if (asn1Set2.Count != 0)
				{
					crls = asn1Set2;
				}
			}
			catch (X509StoreException e2)
			{
				throw new CmsException("error getting CRLs from store", e2);
			}
			SignedData signedData2 = signedData.signedData;
			cmsSignedData.signedData = new SignedData(signedData2.DigestAlgorithms, signedData2.EncapContentInfo, certificates, crls, signedData2.SignerInfos);
			cmsSignedData.contentInfo = new ContentInfo(cmsSignedData.contentInfo.ContentType, cmsSignedData.signedData);
			return cmsSignedData;
		}
	}
}
