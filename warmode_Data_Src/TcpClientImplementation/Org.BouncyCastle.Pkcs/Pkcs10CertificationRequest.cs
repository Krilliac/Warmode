using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Pkcs
{
	public class Pkcs10CertificationRequest : CertificationRequest
	{
		protected static readonly IDictionary algorithms;

		protected static readonly IDictionary exParams;

		protected static readonly IDictionary keyAlgorithms;

		protected static readonly IDictionary oids;

		protected static readonly ISet noParams;

		static Pkcs10CertificationRequest()
		{
			Pkcs10CertificationRequest.algorithms = Platform.CreateHashtable();
			Pkcs10CertificationRequest.exParams = Platform.CreateHashtable();
			Pkcs10CertificationRequest.keyAlgorithms = Platform.CreateHashtable();
			Pkcs10CertificationRequest.oids = Platform.CreateHashtable();
			Pkcs10CertificationRequest.noParams = new HashSet();
			Pkcs10CertificationRequest.algorithms.Add("MD2WITHRSAENCRYPTION", new DerObjectIdentifier("1.2.840.113549.1.1.2"));
			Pkcs10CertificationRequest.algorithms.Add("MD2WITHRSA", new DerObjectIdentifier("1.2.840.113549.1.1.2"));
			Pkcs10CertificationRequest.algorithms.Add("MD5WITHRSAENCRYPTION", new DerObjectIdentifier("1.2.840.113549.1.1.4"));
			Pkcs10CertificationRequest.algorithms.Add("MD5WITHRSA", new DerObjectIdentifier("1.2.840.113549.1.1.4"));
			Pkcs10CertificationRequest.algorithms.Add("RSAWITHMD5", new DerObjectIdentifier("1.2.840.113549.1.1.4"));
			Pkcs10CertificationRequest.algorithms.Add("SHA1WITHRSAENCRYPTION", new DerObjectIdentifier("1.2.840.113549.1.1.5"));
			Pkcs10CertificationRequest.algorithms.Add("SHA1WITHRSA", new DerObjectIdentifier("1.2.840.113549.1.1.5"));
			Pkcs10CertificationRequest.algorithms.Add("SHA224WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA224WITHRSA", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA256WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA256WITHRSA", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA384WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA512WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
			Pkcs10CertificationRequest.algorithms.Add("SHA1WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			Pkcs10CertificationRequest.algorithms.Add("SHA224WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			Pkcs10CertificationRequest.algorithms.Add("SHA256WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			Pkcs10CertificationRequest.algorithms.Add("SHA384WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			Pkcs10CertificationRequest.algorithms.Add("SHA512WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
			Pkcs10CertificationRequest.algorithms.Add("RSAWITHSHA1", new DerObjectIdentifier("1.2.840.113549.1.1.5"));
			Pkcs10CertificationRequest.algorithms.Add("RIPEMD128WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
			Pkcs10CertificationRequest.algorithms.Add("RIPEMD128WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
			Pkcs10CertificationRequest.algorithms.Add("RIPEMD160WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
			Pkcs10CertificationRequest.algorithms.Add("RIPEMD160WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
			Pkcs10CertificationRequest.algorithms.Add("RIPEMD256WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
			Pkcs10CertificationRequest.algorithms.Add("RIPEMD256WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
			Pkcs10CertificationRequest.algorithms.Add("SHA1WITHDSA", new DerObjectIdentifier("1.2.840.10040.4.3"));
			Pkcs10CertificationRequest.algorithms.Add("DSAWITHSHA1", new DerObjectIdentifier("1.2.840.10040.4.3"));
			Pkcs10CertificationRequest.algorithms.Add("SHA224WITHDSA", NistObjectIdentifiers.DsaWithSha224);
			Pkcs10CertificationRequest.algorithms.Add("SHA256WITHDSA", NistObjectIdentifiers.DsaWithSha256);
			Pkcs10CertificationRequest.algorithms.Add("SHA384WITHDSA", NistObjectIdentifiers.DsaWithSha384);
			Pkcs10CertificationRequest.algorithms.Add("SHA512WITHDSA", NistObjectIdentifiers.DsaWithSha512);
			Pkcs10CertificationRequest.algorithms.Add("SHA1WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha1);
			Pkcs10CertificationRequest.algorithms.Add("SHA224WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
			Pkcs10CertificationRequest.algorithms.Add("SHA256WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
			Pkcs10CertificationRequest.algorithms.Add("SHA384WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
			Pkcs10CertificationRequest.algorithms.Add("SHA512WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha512);
			Pkcs10CertificationRequest.algorithms.Add("ECDSAWITHSHA1", X9ObjectIdentifiers.ECDsaWithSha1);
			Pkcs10CertificationRequest.algorithms.Add("GOST3411WITHGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
			Pkcs10CertificationRequest.algorithms.Add("GOST3410WITHGOST3411", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
			Pkcs10CertificationRequest.algorithms.Add("GOST3411WITHECGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
			Pkcs10CertificationRequest.algorithms.Add("GOST3411WITHECGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
			Pkcs10CertificationRequest.algorithms.Add("GOST3411WITHGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
			Pkcs10CertificationRequest.oids.Add(new DerObjectIdentifier("1.2.840.113549.1.1.5"), "SHA1WITHRSA");
			Pkcs10CertificationRequest.oids.Add(PkcsObjectIdentifiers.Sha224WithRsaEncryption, "SHA224WITHRSA");
			Pkcs10CertificationRequest.oids.Add(PkcsObjectIdentifiers.Sha256WithRsaEncryption, "SHA256WITHRSA");
			Pkcs10CertificationRequest.oids.Add(PkcsObjectIdentifiers.Sha384WithRsaEncryption, "SHA384WITHRSA");
			Pkcs10CertificationRequest.oids.Add(PkcsObjectIdentifiers.Sha512WithRsaEncryption, "SHA512WITHRSA");
			Pkcs10CertificationRequest.oids.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94, "GOST3411WITHGOST3410");
			Pkcs10CertificationRequest.oids.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001, "GOST3411WITHECGOST3410");
			Pkcs10CertificationRequest.oids.Add(new DerObjectIdentifier("1.2.840.113549.1.1.4"), "MD5WITHRSA");
			Pkcs10CertificationRequest.oids.Add(new DerObjectIdentifier("1.2.840.113549.1.1.2"), "MD2WITHRSA");
			Pkcs10CertificationRequest.oids.Add(new DerObjectIdentifier("1.2.840.10040.4.3"), "SHA1WITHDSA");
			Pkcs10CertificationRequest.oids.Add(X9ObjectIdentifiers.ECDsaWithSha1, "SHA1WITHECDSA");
			Pkcs10CertificationRequest.oids.Add(X9ObjectIdentifiers.ECDsaWithSha224, "SHA224WITHECDSA");
			Pkcs10CertificationRequest.oids.Add(X9ObjectIdentifiers.ECDsaWithSha256, "SHA256WITHECDSA");
			Pkcs10CertificationRequest.oids.Add(X9ObjectIdentifiers.ECDsaWithSha384, "SHA384WITHECDSA");
			Pkcs10CertificationRequest.oids.Add(X9ObjectIdentifiers.ECDsaWithSha512, "SHA512WITHECDSA");
			Pkcs10CertificationRequest.oids.Add(OiwObjectIdentifiers.Sha1WithRsa, "SHA1WITHRSA");
			Pkcs10CertificationRequest.oids.Add(OiwObjectIdentifiers.DsaWithSha1, "SHA1WITHDSA");
			Pkcs10CertificationRequest.oids.Add(NistObjectIdentifiers.DsaWithSha224, "SHA224WITHDSA");
			Pkcs10CertificationRequest.oids.Add(NistObjectIdentifiers.DsaWithSha256, "SHA256WITHDSA");
			Pkcs10CertificationRequest.keyAlgorithms.Add(PkcsObjectIdentifiers.RsaEncryption, "RSA");
			Pkcs10CertificationRequest.keyAlgorithms.Add(X9ObjectIdentifiers.IdDsa, "DSA");
			Pkcs10CertificationRequest.noParams.Add(X9ObjectIdentifiers.ECDsaWithSha1);
			Pkcs10CertificationRequest.noParams.Add(X9ObjectIdentifiers.ECDsaWithSha224);
			Pkcs10CertificationRequest.noParams.Add(X9ObjectIdentifiers.ECDsaWithSha256);
			Pkcs10CertificationRequest.noParams.Add(X9ObjectIdentifiers.ECDsaWithSha384);
			Pkcs10CertificationRequest.noParams.Add(X9ObjectIdentifiers.ECDsaWithSha512);
			Pkcs10CertificationRequest.noParams.Add(X9ObjectIdentifiers.IdDsaWithSha1);
			Pkcs10CertificationRequest.noParams.Add(NistObjectIdentifiers.DsaWithSha224);
			Pkcs10CertificationRequest.noParams.Add(NistObjectIdentifiers.DsaWithSha256);
			Pkcs10CertificationRequest.noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
			Pkcs10CertificationRequest.noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
			AlgorithmIdentifier hashAlgId = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);
			Pkcs10CertificationRequest.exParams.Add("SHA1WITHRSAANDMGF1", Pkcs10CertificationRequest.CreatePssParams(hashAlgId, 20));
			AlgorithmIdentifier hashAlgId2 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha224, DerNull.Instance);
			Pkcs10CertificationRequest.exParams.Add("SHA224WITHRSAANDMGF1", Pkcs10CertificationRequest.CreatePssParams(hashAlgId2, 28));
			AlgorithmIdentifier hashAlgId3 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256, DerNull.Instance);
			Pkcs10CertificationRequest.exParams.Add("SHA256WITHRSAANDMGF1", Pkcs10CertificationRequest.CreatePssParams(hashAlgId3, 32));
			AlgorithmIdentifier hashAlgId4 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha384, DerNull.Instance);
			Pkcs10CertificationRequest.exParams.Add("SHA384WITHRSAANDMGF1", Pkcs10CertificationRequest.CreatePssParams(hashAlgId4, 48));
			AlgorithmIdentifier hashAlgId5 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha512, DerNull.Instance);
			Pkcs10CertificationRequest.exParams.Add("SHA512WITHRSAANDMGF1", Pkcs10CertificationRequest.CreatePssParams(hashAlgId5, 64));
		}

		private static RsassaPssParameters CreatePssParams(AlgorithmIdentifier hashAlgId, int saltSize)
		{
			return new RsassaPssParameters(hashAlgId, new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, hashAlgId), new DerInteger(saltSize), new DerInteger(1));
		}

		protected Pkcs10CertificationRequest()
		{
		}

		public Pkcs10CertificationRequest(byte[] encoded) : base((Asn1Sequence)Asn1Object.FromByteArray(encoded))
		{
		}

		public Pkcs10CertificationRequest(Asn1Sequence seq) : base(seq)
		{
		}

		public Pkcs10CertificationRequest(Stream input) : base((Asn1Sequence)Asn1Object.FromStream(input))
		{
		}

		public Pkcs10CertificationRequest(string signatureAlgorithm, X509Name subject, AsymmetricKeyParameter publicKey, Asn1Set attributes, AsymmetricKeyParameter signingKey)
		{
			if (signatureAlgorithm == null)
			{
				throw new ArgumentNullException("signatureAlgorithm");
			}
			if (subject == null)
			{
				throw new ArgumentNullException("subject");
			}
			if (publicKey == null)
			{
				throw new ArgumentNullException("publicKey");
			}
			if (publicKey.IsPrivate)
			{
				throw new ArgumentException("expected public key", "publicKey");
			}
			if (!signingKey.IsPrivate)
			{
				throw new ArgumentException("key for signing must be private", "signingKey");
			}
			string text = Platform.ToUpperInvariant(signatureAlgorithm);
			DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)Pkcs10CertificationRequest.algorithms[text];
			if (derObjectIdentifier == null)
			{
				try
				{
					derObjectIdentifier = new DerObjectIdentifier(text);
				}
				catch (Exception innerException)
				{
					throw new ArgumentException("Unknown signature type requested", innerException);
				}
			}
			if (Pkcs10CertificationRequest.noParams.Contains(derObjectIdentifier))
			{
				this.sigAlgId = new AlgorithmIdentifier(derObjectIdentifier);
			}
			else if (Pkcs10CertificationRequest.exParams.Contains(text))
			{
				this.sigAlgId = new AlgorithmIdentifier(derObjectIdentifier, (Asn1Encodable)Pkcs10CertificationRequest.exParams[text]);
			}
			else
			{
				this.sigAlgId = new AlgorithmIdentifier(derObjectIdentifier, DerNull.Instance);
			}
			SubjectPublicKeyInfo pkInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
			this.reqInfo = new CertificationRequestInfo(subject, pkInfo, attributes);
			ISigner signer = SignerUtilities.GetSigner(signatureAlgorithm);
			signer.Init(true, signingKey);
			try
			{
				byte[] derEncoded = this.reqInfo.GetDerEncoded();
				signer.BlockUpdate(derEncoded, 0, derEncoded.Length);
			}
			catch (Exception innerException2)
			{
				throw new ArgumentException("exception encoding TBS cert request", innerException2);
			}
			this.sigBits = new DerBitString(signer.GenerateSignature());
		}

		public AsymmetricKeyParameter GetPublicKey()
		{
			return PublicKeyFactory.CreateKey(this.reqInfo.SubjectPublicKeyInfo);
		}

		public bool Verify()
		{
			return this.Verify(this.GetPublicKey());
		}

		public bool Verify(AsymmetricKeyParameter publicKey)
		{
			ISigner signer;
			try
			{
				signer = SignerUtilities.GetSigner(Pkcs10CertificationRequest.GetSignatureName(this.sigAlgId));
			}
			catch (Exception ex)
			{
				string text = (string)Pkcs10CertificationRequest.oids[this.sigAlgId.ObjectID];
				if (text == null)
				{
					throw ex;
				}
				signer = SignerUtilities.GetSigner(text);
			}
			this.SetSignatureParameters(signer, this.sigAlgId.Parameters);
			signer.Init(false, publicKey);
			try
			{
				byte[] derEncoded = this.reqInfo.GetDerEncoded();
				signer.BlockUpdate(derEncoded, 0, derEncoded.Length);
			}
			catch (Exception exception)
			{
				throw new SignatureException("exception encoding TBS cert request", exception);
			}
			return signer.VerifySignature(this.sigBits.GetBytes());
		}

		private void SetSignatureParameters(ISigner signature, Asn1Encodable asn1Params)
		{
			if (asn1Params != null && !(asn1Params is Asn1Null) && signature.AlgorithmName.EndsWith("MGF1"))
			{
				throw Platform.CreateNotImplementedException("signature algorithm with MGF1");
			}
		}

		internal static string GetSignatureName(AlgorithmIdentifier sigAlgId)
		{
			Asn1Encodable parameters = sigAlgId.Parameters;
			if (parameters != null && !(parameters is Asn1Null) && sigAlgId.ObjectID.Equals(PkcsObjectIdentifiers.IdRsassaPss))
			{
				RsassaPssParameters instance = RsassaPssParameters.GetInstance(parameters);
				return Pkcs10CertificationRequest.GetDigestAlgName(instance.HashAlgorithm.ObjectID) + "withRSAandMGF1";
			}
			return sigAlgId.ObjectID.Id;
		}

		private static string GetDigestAlgName(DerObjectIdentifier digestAlgOID)
		{
			if (PkcsObjectIdentifiers.MD5.Equals(digestAlgOID))
			{
				return "MD5";
			}
			if (OiwObjectIdentifiers.IdSha1.Equals(digestAlgOID))
			{
				return "SHA1";
			}
			if (NistObjectIdentifiers.IdSha224.Equals(digestAlgOID))
			{
				return "SHA224";
			}
			if (NistObjectIdentifiers.IdSha256.Equals(digestAlgOID))
			{
				return "SHA256";
			}
			if (NistObjectIdentifiers.IdSha384.Equals(digestAlgOID))
			{
				return "SHA384";
			}
			if (NistObjectIdentifiers.IdSha512.Equals(digestAlgOID))
			{
				return "SHA512";
			}
			if (TeleTrusTObjectIdentifiers.RipeMD128.Equals(digestAlgOID))
			{
				return "RIPEMD128";
			}
			if (TeleTrusTObjectIdentifiers.RipeMD160.Equals(digestAlgOID))
			{
				return "RIPEMD160";
			}
			if (TeleTrusTObjectIdentifiers.RipeMD256.Equals(digestAlgOID))
			{
				return "RIPEMD256";
			}
			if (CryptoProObjectIdentifiers.GostR3411.Equals(digestAlgOID))
			{
				return "GOST3411";
			}
			return digestAlgOID.Id;
		}
	}
}
