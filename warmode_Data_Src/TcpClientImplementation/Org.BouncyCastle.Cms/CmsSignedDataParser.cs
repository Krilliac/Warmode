using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsSignedDataParser : CmsContentInfoParser
	{
		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private SignedDataParser _signedData;

		private DerObjectIdentifier _signedContentType;

		private CmsTypedStream _signedContent;

		private IDictionary _digests;

		private ISet _digestOids;

		private SignerInformationStore _signerInfoStore;

		private Asn1Set _certSet;

		private Asn1Set _crlSet;

		private bool _isCertCrlParsed;

		private IX509Store _attributeStore;

		private IX509Store _certificateStore;

		private IX509Store _crlStore;

		public int Version
		{
			get
			{
				return this._signedData.Version.Value.IntValue;
			}
		}

		public ISet DigestOids
		{
			get
			{
				return new HashSet(this._digestOids);
			}
		}

		public DerObjectIdentifier SignedContentType
		{
			get
			{
				return this._signedContentType;
			}
		}

		public CmsSignedDataParser(byte[] sigBlock) : this(new MemoryStream(sigBlock, false))
		{
		}

		public CmsSignedDataParser(CmsTypedStream signedContent, byte[] sigBlock) : this(signedContent, new MemoryStream(sigBlock, false))
		{
		}

		public CmsSignedDataParser(Stream sigData) : this(null, sigData)
		{
		}

		public CmsSignedDataParser(CmsTypedStream signedContent, Stream sigData) : base(sigData)
		{
			try
			{
				this._signedContent = signedContent;
				this._signedData = SignedDataParser.GetInstance(this.contentInfo.GetContent(16));
				this._digests = Platform.CreateHashtable();
				this._digestOids = new HashSet();
				Asn1SetParser digestAlgorithms = this._signedData.GetDigestAlgorithms();
				IAsn1Convertible asn1Convertible;
				while ((asn1Convertible = digestAlgorithms.ReadObject()) != null)
				{
					AlgorithmIdentifier instance = AlgorithmIdentifier.GetInstance(asn1Convertible.ToAsn1Object());
					try
					{
						string id = instance.ObjectID.Id;
						string digestAlgName = CmsSignedDataParser.Helper.GetDigestAlgName(id);
						if (!this._digests.Contains(digestAlgName))
						{
							this._digests[digestAlgName] = CmsSignedDataParser.Helper.GetDigestInstance(digestAlgName);
							this._digestOids.Add(id);
						}
					}
					catch (SecurityUtilityException)
					{
					}
				}
				ContentInfoParser encapContentInfo = this._signedData.GetEncapContentInfo();
				Asn1OctetStringParser asn1OctetStringParser = (Asn1OctetStringParser)encapContentInfo.GetContent(4);
				if (asn1OctetStringParser != null)
				{
					CmsTypedStream cmsTypedStream = new CmsTypedStream(encapContentInfo.ContentType.Id, asn1OctetStringParser.GetOctetStream());
					if (this._signedContent == null)
					{
						this._signedContent = cmsTypedStream;
					}
					else
					{
						cmsTypedStream.Drain();
					}
				}
				this._signedContentType = ((this._signedContent == null) ? encapContentInfo.ContentType : new DerObjectIdentifier(this._signedContent.ContentType));
			}
			catch (IOException ex)
			{
				throw new CmsException("io exception: " + ex.Message, ex);
			}
		}

		public SignerInformationStore GetSignerInfos()
		{
			if (this._signerInfoStore == null)
			{
				this.PopulateCertCrlSets();
				IList list = Platform.CreateArrayList();
				IDictionary dictionary = Platform.CreateHashtable();
				foreach (object current in this._digests.Keys)
				{
					dictionary[current] = DigestUtilities.DoFinal((IDigest)this._digests[current]);
				}
				try
				{
					Asn1SetParser signerInfos = this._signedData.GetSignerInfos();
					IAsn1Convertible asn1Convertible;
					while ((asn1Convertible = signerInfos.ReadObject()) != null)
					{
						SignerInfo instance = SignerInfo.GetInstance(asn1Convertible.ToAsn1Object());
						string digestAlgName = CmsSignedDataParser.Helper.GetDigestAlgName(instance.DigestAlgorithm.ObjectID.Id);
						byte[] digest = (byte[])dictionary[digestAlgName];
						list.Add(new SignerInformation(instance, this._signedContentType, null, new BaseDigestCalculator(digest)));
					}
				}
				catch (IOException ex)
				{
					throw new CmsException("io exception: " + ex.Message, ex);
				}
				this._signerInfoStore = new SignerInformationStore(list);
			}
			return this._signerInfoStore;
		}

		public IX509Store GetAttributeCertificates(string type)
		{
			if (this._attributeStore == null)
			{
				this.PopulateCertCrlSets();
				this._attributeStore = CmsSignedDataParser.Helper.CreateAttributeStore(type, this._certSet);
			}
			return this._attributeStore;
		}

		public IX509Store GetCertificates(string type)
		{
			if (this._certificateStore == null)
			{
				this.PopulateCertCrlSets();
				this._certificateStore = CmsSignedDataParser.Helper.CreateCertificateStore(type, this._certSet);
			}
			return this._certificateStore;
		}

		public IX509Store GetCrls(string type)
		{
			if (this._crlStore == null)
			{
				this.PopulateCertCrlSets();
				this._crlStore = CmsSignedDataParser.Helper.CreateCrlStore(type, this._crlSet);
			}
			return this._crlStore;
		}

		private void PopulateCertCrlSets()
		{
			if (this._isCertCrlParsed)
			{
				return;
			}
			this._isCertCrlParsed = true;
			try
			{
				this._certSet = CmsSignedDataParser.GetAsn1Set(this._signedData.GetCertificates());
				this._crlSet = CmsSignedDataParser.GetAsn1Set(this._signedData.GetCrls());
			}
			catch (IOException e)
			{
				throw new CmsException("problem parsing cert/crl sets", e);
			}
		}

		public CmsTypedStream GetSignedContent()
		{
			if (this._signedContent == null)
			{
				return null;
			}
			Stream stream = this._signedContent.ContentStream;
			foreach (IDigest readDigest in this._digests.Values)
			{
				stream = new DigestStream(stream, readDigest, null);
			}
			return new CmsTypedStream(this._signedContent.ContentType, stream);
		}

		public static Stream ReplaceSigners(Stream original, SignerInformationStore signerInformationStore, Stream outStr)
		{
			CmsSignedDataStreamGenerator cmsSignedDataStreamGenerator = new CmsSignedDataStreamGenerator();
			CmsSignedDataParser cmsSignedDataParser = new CmsSignedDataParser(original);
			cmsSignedDataStreamGenerator.AddSigners(signerInformationStore);
			CmsTypedStream signedContent = cmsSignedDataParser.GetSignedContent();
			bool flag = signedContent != null;
			Stream stream = cmsSignedDataStreamGenerator.Open(outStr, cmsSignedDataParser.SignedContentType.Id, flag);
			if (flag)
			{
				Streams.PipeAll(signedContent.ContentStream, stream);
			}
			cmsSignedDataStreamGenerator.AddAttributeCertificates(cmsSignedDataParser.GetAttributeCertificates("Collection"));
			cmsSignedDataStreamGenerator.AddCertificates(cmsSignedDataParser.GetCertificates("Collection"));
			cmsSignedDataStreamGenerator.AddCrls(cmsSignedDataParser.GetCrls("Collection"));
			stream.Close();
			return outStr;
		}

		public static Stream ReplaceCertificatesAndCrls(Stream original, IX509Store x509Certs, IX509Store x509Crls, IX509Store x509AttrCerts, Stream outStr)
		{
			CmsSignedDataStreamGenerator cmsSignedDataStreamGenerator = new CmsSignedDataStreamGenerator();
			CmsSignedDataParser cmsSignedDataParser = new CmsSignedDataParser(original);
			cmsSignedDataStreamGenerator.AddDigests(cmsSignedDataParser.DigestOids);
			CmsTypedStream signedContent = cmsSignedDataParser.GetSignedContent();
			bool flag = signedContent != null;
			Stream stream = cmsSignedDataStreamGenerator.Open(outStr, cmsSignedDataParser.SignedContentType.Id, flag);
			if (flag)
			{
				Streams.PipeAll(signedContent.ContentStream, stream);
			}
			if (x509AttrCerts != null)
			{
				cmsSignedDataStreamGenerator.AddAttributeCertificates(x509AttrCerts);
			}
			if (x509Certs != null)
			{
				cmsSignedDataStreamGenerator.AddCertificates(x509Certs);
			}
			if (x509Crls != null)
			{
				cmsSignedDataStreamGenerator.AddCrls(x509Crls);
			}
			cmsSignedDataStreamGenerator.AddSigners(cmsSignedDataParser.GetSignerInfos());
			stream.Close();
			return outStr;
		}

		private static Asn1Set GetAsn1Set(Asn1SetParser asn1SetParser)
		{
			if (asn1SetParser != null)
			{
				return Asn1Set.GetInstance(asn1SetParser.ToAsn1Object());
			}
			return null;
		}
	}
}
