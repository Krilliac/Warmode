using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Eac;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	internal class CmsSignedHelper
	{
		internal static readonly CmsSignedHelper Instance;

		private static readonly IDictionary encryptionAlgs;

		private static readonly IDictionary digestAlgs;

		private static readonly IDictionary digestAliases;

		private static void AddEntries(DerObjectIdentifier oid, string digest, string encryption)
		{
			string id = oid.Id;
			CmsSignedHelper.digestAlgs.Add(id, digest);
			CmsSignedHelper.encryptionAlgs.Add(id, encryption);
		}

		static CmsSignedHelper()
		{
			CmsSignedHelper.Instance = new CmsSignedHelper();
			CmsSignedHelper.encryptionAlgs = Platform.CreateHashtable();
			CmsSignedHelper.digestAlgs = Platform.CreateHashtable();
			CmsSignedHelper.digestAliases = Platform.CreateHashtable();
			CmsSignedHelper.AddEntries(NistObjectIdentifiers.DsaWithSha224, "SHA224", "DSA");
			CmsSignedHelper.AddEntries(NistObjectIdentifiers.DsaWithSha256, "SHA256", "DSA");
			CmsSignedHelper.AddEntries(NistObjectIdentifiers.DsaWithSha384, "SHA384", "DSA");
			CmsSignedHelper.AddEntries(NistObjectIdentifiers.DsaWithSha512, "SHA512", "DSA");
			CmsSignedHelper.AddEntries(OiwObjectIdentifiers.DsaWithSha1, "SHA1", "DSA");
			CmsSignedHelper.AddEntries(OiwObjectIdentifiers.MD4WithRsa, "MD4", "RSA");
			CmsSignedHelper.AddEntries(OiwObjectIdentifiers.MD4WithRsaEncryption, "MD4", "RSA");
			CmsSignedHelper.AddEntries(OiwObjectIdentifiers.MD5WithRsa, "MD5", "RSA");
			CmsSignedHelper.AddEntries(OiwObjectIdentifiers.Sha1WithRsa, "SHA1", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.MD2WithRsaEncryption, "MD2", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.MD4WithRsaEncryption, "MD4", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.MD5WithRsaEncryption, "MD5", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.Sha1WithRsaEncryption, "SHA1", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.Sha224WithRsaEncryption, "SHA224", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.Sha256WithRsaEncryption, "SHA256", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.Sha384WithRsaEncryption, "SHA384", "RSA");
			CmsSignedHelper.AddEntries(PkcsObjectIdentifiers.Sha512WithRsaEncryption, "SHA512", "RSA");
			CmsSignedHelper.AddEntries(X9ObjectIdentifiers.ECDsaWithSha1, "SHA1", "ECDSA");
			CmsSignedHelper.AddEntries(X9ObjectIdentifiers.ECDsaWithSha224, "SHA224", "ECDSA");
			CmsSignedHelper.AddEntries(X9ObjectIdentifiers.ECDsaWithSha256, "SHA256", "ECDSA");
			CmsSignedHelper.AddEntries(X9ObjectIdentifiers.ECDsaWithSha384, "SHA384", "ECDSA");
			CmsSignedHelper.AddEntries(X9ObjectIdentifiers.ECDsaWithSha512, "SHA512", "ECDSA");
			CmsSignedHelper.AddEntries(X9ObjectIdentifiers.IdDsaWithSha1, "SHA1", "DSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_1, "SHA1", "ECDSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_224, "SHA224", "ECDSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_256, "SHA256", "ECDSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_384, "SHA384", "ECDSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_512, "SHA512", "ECDSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_1, "SHA1", "RSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_256, "SHA256", "RSA");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_RSA_PSS_SHA_1, "SHA1", "RSAandMGF1");
			CmsSignedHelper.AddEntries(EacObjectIdentifiers.id_TA_RSA_PSS_SHA_256, "SHA256", "RSAandMGF1");
			CmsSignedHelper.encryptionAlgs.Add(X9ObjectIdentifiers.IdDsa.Id, "DSA");
			CmsSignedHelper.encryptionAlgs.Add(PkcsObjectIdentifiers.RsaEncryption.Id, "RSA");
			CmsSignedHelper.encryptionAlgs.Add(TeleTrusTObjectIdentifiers.TeleTrusTRsaSignatureAlgorithm, "RSA");
			CmsSignedHelper.encryptionAlgs.Add(X509ObjectIdentifiers.IdEARsa.Id, "RSA");
			CmsSignedHelper.encryptionAlgs.Add(CmsSignedGenerator.EncryptionRsaPss, "RSAandMGF1");
			CmsSignedHelper.encryptionAlgs.Add(CryptoProObjectIdentifiers.GostR3410x94.Id, "GOST3410");
			CmsSignedHelper.encryptionAlgs.Add(CryptoProObjectIdentifiers.GostR3410x2001.Id, "ECGOST3410");
			CmsSignedHelper.encryptionAlgs.Add("1.3.6.1.4.1.5849.1.6.2", "ECGOST3410");
			CmsSignedHelper.encryptionAlgs.Add("1.3.6.1.4.1.5849.1.1.5", "GOST3410");
			CmsSignedHelper.digestAlgs.Add(PkcsObjectIdentifiers.MD2.Id, "MD2");
			CmsSignedHelper.digestAlgs.Add(PkcsObjectIdentifiers.MD4.Id, "MD4");
			CmsSignedHelper.digestAlgs.Add(PkcsObjectIdentifiers.MD5.Id, "MD5");
			CmsSignedHelper.digestAlgs.Add(OiwObjectIdentifiers.IdSha1.Id, "SHA1");
			CmsSignedHelper.digestAlgs.Add(NistObjectIdentifiers.IdSha224.Id, "SHA224");
			CmsSignedHelper.digestAlgs.Add(NistObjectIdentifiers.IdSha256.Id, "SHA256");
			CmsSignedHelper.digestAlgs.Add(NistObjectIdentifiers.IdSha384.Id, "SHA384");
			CmsSignedHelper.digestAlgs.Add(NistObjectIdentifiers.IdSha512.Id, "SHA512");
			CmsSignedHelper.digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD128.Id, "RIPEMD128");
			CmsSignedHelper.digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD160.Id, "RIPEMD160");
			CmsSignedHelper.digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD256.Id, "RIPEMD256");
			CmsSignedHelper.digestAlgs.Add(CryptoProObjectIdentifiers.GostR3411.Id, "GOST3411");
			CmsSignedHelper.digestAlgs.Add("1.3.6.1.4.1.5849.1.2.1", "GOST3411");
			CmsSignedHelper.digestAliases.Add("SHA1", new string[]
			{
				"SHA-1"
			});
			CmsSignedHelper.digestAliases.Add("SHA224", new string[]
			{
				"SHA-224"
			});
			CmsSignedHelper.digestAliases.Add("SHA256", new string[]
			{
				"SHA-256"
			});
			CmsSignedHelper.digestAliases.Add("SHA384", new string[]
			{
				"SHA-384"
			});
			CmsSignedHelper.digestAliases.Add("SHA512", new string[]
			{
				"SHA-512"
			});
		}

		internal string GetDigestAlgName(string digestAlgOid)
		{
			string text = (string)CmsSignedHelper.digestAlgs[digestAlgOid];
			if (text != null)
			{
				return text;
			}
			return digestAlgOid;
		}

		internal string[] GetDigestAliases(string algName)
		{
			string[] array = (string[])CmsSignedHelper.digestAliases[algName];
			if (array != null)
			{
				return (string[])array.Clone();
			}
			return new string[0];
		}

		internal string GetEncryptionAlgName(string encryptionAlgOid)
		{
			string text = (string)CmsSignedHelper.encryptionAlgs[encryptionAlgOid];
			if (text != null)
			{
				return text;
			}
			return encryptionAlgOid;
		}

		internal IDigest GetDigestInstance(string algorithm)
		{
			IDigest digest;
			try
			{
				digest = DigestUtilities.GetDigest(algorithm);
			}
			catch (SecurityUtilityException ex)
			{
				string[] array = this.GetDigestAliases(algorithm);
				for (int i = 0; i < array.Length; i++)
				{
					string algorithm2 = array[i];
					try
					{
						digest = DigestUtilities.GetDigest(algorithm2);
						return digest;
					}
					catch (SecurityUtilityException)
					{
					}
				}
				throw ex;
			}
			return digest;
		}

		internal ISigner GetSignatureInstance(string algorithm)
		{
			return SignerUtilities.GetSigner(algorithm);
		}

		internal IX509Store CreateAttributeStore(string type, Asn1Set certSet)
		{
			IList list = Platform.CreateArrayList();
			if (certSet != null)
			{
				foreach (Asn1Encodable asn1Encodable in certSet)
				{
					try
					{
						Asn1Object asn1Object = asn1Encodable.ToAsn1Object();
						if (asn1Object is Asn1TaggedObject)
						{
							Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Object;
							if (asn1TaggedObject.TagNo == 2)
							{
								list.Add(new X509V2AttributeCertificate(Asn1Sequence.GetInstance(asn1TaggedObject, false).GetEncoded()));
							}
						}
					}
					catch (Exception e)
					{
						throw new CmsException("can't re-encode attribute certificate!", e);
					}
				}
			}
			IX509Store result;
			try
			{
				result = X509StoreFactory.Create("AttributeCertificate/" + type, new X509CollectionStoreParameters(list));
			}
			catch (ArgumentException e2)
			{
				throw new CmsException("can't setup the X509Store", e2);
			}
			return result;
		}

		internal IX509Store CreateCertificateStore(string type, Asn1Set certSet)
		{
			IList list = Platform.CreateArrayList();
			if (certSet != null)
			{
				this.AddCertsFromSet(list, certSet);
			}
			IX509Store result;
			try
			{
				result = X509StoreFactory.Create("Certificate/" + type, new X509CollectionStoreParameters(list));
			}
			catch (ArgumentException e)
			{
				throw new CmsException("can't setup the X509Store", e);
			}
			return result;
		}

		internal IX509Store CreateCrlStore(string type, Asn1Set crlSet)
		{
			IList list = Platform.CreateArrayList();
			if (crlSet != null)
			{
				this.AddCrlsFromSet(list, crlSet);
			}
			IX509Store result;
			try
			{
				result = X509StoreFactory.Create("CRL/" + type, new X509CollectionStoreParameters(list));
			}
			catch (ArgumentException e)
			{
				throw new CmsException("can't setup the X509Store", e);
			}
			return result;
		}

		private void AddCertsFromSet(IList certs, Asn1Set certSet)
		{
			X509CertificateParser x509CertificateParser = new X509CertificateParser();
			foreach (Asn1Encodable asn1Encodable in certSet)
			{
				try
				{
					Asn1Object asn1Object = asn1Encodable.ToAsn1Object();
					if (asn1Object is Asn1Sequence)
					{
						certs.Add(x509CertificateParser.ReadCertificate(asn1Object.GetEncoded()));
					}
				}
				catch (Exception e)
				{
					throw new CmsException("can't re-encode certificate!", e);
				}
			}
		}

		private void AddCrlsFromSet(IList crls, Asn1Set crlSet)
		{
			X509CrlParser x509CrlParser = new X509CrlParser();
			foreach (Asn1Encodable asn1Encodable in crlSet)
			{
				try
				{
					crls.Add(x509CrlParser.ReadCrl(asn1Encodable.GetEncoded()));
				}
				catch (Exception e)
				{
					throw new CmsException("can't re-encode CRL!", e);
				}
			}
		}

		internal AlgorithmIdentifier FixAlgID(AlgorithmIdentifier algId)
		{
			if (algId.Parameters == null)
			{
				return new AlgorithmIdentifier(algId.ObjectID, DerNull.Instance);
			}
			return algId;
		}
	}
}
