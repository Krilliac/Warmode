using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public class CmsSignedGenerator
	{
		public static readonly string Data;

		public static readonly string DigestSha1;

		public static readonly string DigestSha224;

		public static readonly string DigestSha256;

		public static readonly string DigestSha384;

		public static readonly string DigestSha512;

		public static readonly string DigestMD5;

		public static readonly string DigestGost3411;

		public static readonly string DigestRipeMD128;

		public static readonly string DigestRipeMD160;

		public static readonly string DigestRipeMD256;

		public static readonly string EncryptionRsa;

		public static readonly string EncryptionDsa;

		public static readonly string EncryptionECDsa;

		public static readonly string EncryptionRsaPss;

		public static readonly string EncryptionGost3410;

		public static readonly string EncryptionECGost3410;

		private static readonly string EncryptionECDsaWithSha1;

		private static readonly string EncryptionECDsaWithSha224;

		private static readonly string EncryptionECDsaWithSha256;

		private static readonly string EncryptionECDsaWithSha384;

		private static readonly string EncryptionECDsaWithSha512;

		private static readonly ISet noParams;

		private static readonly IDictionary ecAlgorithms;

		internal IList _certs = Platform.CreateArrayList();

		internal IList _crls = Platform.CreateArrayList();

		internal IList _signers = Platform.CreateArrayList();

		internal IDictionary _digests = Platform.CreateHashtable();

		protected readonly SecureRandom rand;

		static CmsSignedGenerator()
		{
			CmsSignedGenerator.Data = CmsObjectIdentifiers.Data.Id;
			CmsSignedGenerator.DigestSha1 = OiwObjectIdentifiers.IdSha1.Id;
			CmsSignedGenerator.DigestSha224 = NistObjectIdentifiers.IdSha224.Id;
			CmsSignedGenerator.DigestSha256 = NistObjectIdentifiers.IdSha256.Id;
			CmsSignedGenerator.DigestSha384 = NistObjectIdentifiers.IdSha384.Id;
			CmsSignedGenerator.DigestSha512 = NistObjectIdentifiers.IdSha512.Id;
			CmsSignedGenerator.DigestMD5 = PkcsObjectIdentifiers.MD5.Id;
			CmsSignedGenerator.DigestGost3411 = CryptoProObjectIdentifiers.GostR3411.Id;
			CmsSignedGenerator.DigestRipeMD128 = TeleTrusTObjectIdentifiers.RipeMD128.Id;
			CmsSignedGenerator.DigestRipeMD160 = TeleTrusTObjectIdentifiers.RipeMD160.Id;
			CmsSignedGenerator.DigestRipeMD256 = TeleTrusTObjectIdentifiers.RipeMD256.Id;
			CmsSignedGenerator.EncryptionRsa = PkcsObjectIdentifiers.RsaEncryption.Id;
			CmsSignedGenerator.EncryptionDsa = X9ObjectIdentifiers.IdDsaWithSha1.Id;
			CmsSignedGenerator.EncryptionECDsa = X9ObjectIdentifiers.ECDsaWithSha1.Id;
			CmsSignedGenerator.EncryptionRsaPss = PkcsObjectIdentifiers.IdRsassaPss.Id;
			CmsSignedGenerator.EncryptionGost3410 = CryptoProObjectIdentifiers.GostR3410x94.Id;
			CmsSignedGenerator.EncryptionECGost3410 = CryptoProObjectIdentifiers.GostR3410x2001.Id;
			CmsSignedGenerator.EncryptionECDsaWithSha1 = X9ObjectIdentifiers.ECDsaWithSha1.Id;
			CmsSignedGenerator.EncryptionECDsaWithSha224 = X9ObjectIdentifiers.ECDsaWithSha224.Id;
			CmsSignedGenerator.EncryptionECDsaWithSha256 = X9ObjectIdentifiers.ECDsaWithSha256.Id;
			CmsSignedGenerator.EncryptionECDsaWithSha384 = X9ObjectIdentifiers.ECDsaWithSha384.Id;
			CmsSignedGenerator.EncryptionECDsaWithSha512 = X9ObjectIdentifiers.ECDsaWithSha512.Id;
			CmsSignedGenerator.noParams = new HashSet();
			CmsSignedGenerator.ecAlgorithms = Platform.CreateHashtable();
			CmsSignedGenerator.noParams.Add(CmsSignedGenerator.EncryptionDsa);
			CmsSignedGenerator.noParams.Add(CmsSignedGenerator.EncryptionECDsaWithSha1);
			CmsSignedGenerator.noParams.Add(CmsSignedGenerator.EncryptionECDsaWithSha224);
			CmsSignedGenerator.noParams.Add(CmsSignedGenerator.EncryptionECDsaWithSha256);
			CmsSignedGenerator.noParams.Add(CmsSignedGenerator.EncryptionECDsaWithSha384);
			CmsSignedGenerator.noParams.Add(CmsSignedGenerator.EncryptionECDsaWithSha512);
			CmsSignedGenerator.ecAlgorithms.Add(CmsSignedGenerator.DigestSha1, CmsSignedGenerator.EncryptionECDsaWithSha1);
			CmsSignedGenerator.ecAlgorithms.Add(CmsSignedGenerator.DigestSha224, CmsSignedGenerator.EncryptionECDsaWithSha224);
			CmsSignedGenerator.ecAlgorithms.Add(CmsSignedGenerator.DigestSha256, CmsSignedGenerator.EncryptionECDsaWithSha256);
			CmsSignedGenerator.ecAlgorithms.Add(CmsSignedGenerator.DigestSha384, CmsSignedGenerator.EncryptionECDsaWithSha384);
			CmsSignedGenerator.ecAlgorithms.Add(CmsSignedGenerator.DigestSha512, CmsSignedGenerator.EncryptionECDsaWithSha512);
		}

		protected CmsSignedGenerator() : this(new SecureRandom())
		{
		}

		protected CmsSignedGenerator(SecureRandom rand)
		{
			this.rand = rand;
		}

		protected string GetEncOid(AsymmetricKeyParameter key, string digestOID)
		{
			string text;
			if (key is RsaKeyParameters)
			{
				if (!((RsaKeyParameters)key).IsPrivate)
				{
					throw new ArgumentException("Expected RSA private key");
				}
				text = CmsSignedGenerator.EncryptionRsa;
			}
			else if (key is DsaPrivateKeyParameters)
			{
				if (!digestOID.Equals(CmsSignedGenerator.DigestSha1))
				{
					throw new ArgumentException("can't mix DSA with anything but SHA1");
				}
				text = CmsSignedGenerator.EncryptionDsa;
			}
			else if (key is ECPrivateKeyParameters)
			{
				ECPrivateKeyParameters eCPrivateKeyParameters = (ECPrivateKeyParameters)key;
				string algorithmName = eCPrivateKeyParameters.AlgorithmName;
				if (algorithmName == "ECGOST3410")
				{
					text = CmsSignedGenerator.EncryptionECGost3410;
				}
				else
				{
					text = (string)CmsSignedGenerator.ecAlgorithms[digestOID];
					if (text == null)
					{
						throw new ArgumentException("can't mix ECDSA with anything but SHA family digests");
					}
				}
			}
			else
			{
				if (!(key is Gost3410PrivateKeyParameters))
				{
					throw new ArgumentException("Unknown algorithm in CmsSignedGenerator.GetEncOid");
				}
				text = CmsSignedGenerator.EncryptionGost3410;
			}
			return text;
		}

		internal static AlgorithmIdentifier GetEncAlgorithmIdentifier(DerObjectIdentifier encOid, Asn1Encodable sigX509Parameters)
		{
			if (CmsSignedGenerator.noParams.Contains(encOid.Id))
			{
				return new AlgorithmIdentifier(encOid);
			}
			return new AlgorithmIdentifier(encOid, sigX509Parameters);
		}

		protected internal virtual IDictionary GetBaseParameters(DerObjectIdentifier contentType, AlgorithmIdentifier digAlgId, byte[] hash)
		{
			IDictionary dictionary = Platform.CreateHashtable();
			if (contentType != null)
			{
				dictionary[CmsAttributeTableParameter.ContentType] = contentType;
			}
			dictionary[CmsAttributeTableParameter.DigestAlgorithmIdentifier] = digAlgId;
			dictionary[CmsAttributeTableParameter.Digest] = hash.Clone();
			return dictionary;
		}

		protected internal virtual Asn1Set GetAttributeSet(Org.BouncyCastle.Asn1.Cms.AttributeTable attr)
		{
			if (attr != null)
			{
				return new DerSet(attr.ToAsn1EncodableVector());
			}
			return null;
		}

		public void AddCertificates(IX509Store certStore)
		{
			CollectionUtilities.AddRange(this._certs, CmsUtilities.GetCertificatesFromStore(certStore));
		}

		public void AddCrls(IX509Store crlStore)
		{
			CollectionUtilities.AddRange(this._crls, CmsUtilities.GetCrlsFromStore(crlStore));
		}

		public void AddAttributeCertificates(IX509Store store)
		{
			try
			{
				foreach (IX509AttributeCertificate iX509AttributeCertificate in store.GetMatches(null))
				{
					this._certs.Add(new DerTaggedObject(false, 2, AttributeCertificate.GetInstance(Asn1Object.FromByteArray(iX509AttributeCertificate.GetEncoded()))));
				}
			}
			catch (Exception e)
			{
				throw new CmsException("error processing attribute certs", e);
			}
		}

		public void AddSigners(SignerInformationStore signerStore)
		{
			foreach (SignerInformation signerInformation in signerStore.GetSigners())
			{
				this._signers.Add(signerInformation);
				this.AddSignerCallback(signerInformation);
			}
		}

		public IDictionary GetGeneratedDigests()
		{
			return Platform.CreateHashtable(this._digests);
		}

		internal virtual void AddSignerCallback(SignerInformation si)
		{
		}

		internal static SignerIdentifier GetSignerIdentifier(X509Certificate cert)
		{
			return new SignerIdentifier(CmsUtilities.GetIssuerAndSerialNumber(cert));
		}

		internal static SignerIdentifier GetSignerIdentifier(byte[] subjectKeyIdentifier)
		{
			return new SignerIdentifier(new DerOctetString(subjectKeyIdentifier));
		}
	}
}
