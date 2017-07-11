using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Security
{
	public sealed class SignerUtilities
	{
		internal static readonly IDictionary algorithms;

		internal static readonly IDictionary oids;

		public static ICollection Algorithms
		{
			get
			{
				return SignerUtilities.oids.Keys;
			}
		}

		private SignerUtilities()
		{
		}

		static SignerUtilities()
		{
			SignerUtilities.algorithms = Platform.CreateHashtable();
			SignerUtilities.oids = Platform.CreateHashtable();
			SignerUtilities.algorithms["MD2WITHRSA"] = "MD2withRSA";
			SignerUtilities.algorithms["MD2WITHRSAENCRYPTION"] = "MD2withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.MD2WithRsaEncryption.Id] = "MD2withRSA";
			SignerUtilities.algorithms["MD4WITHRSA"] = "MD4withRSA";
			SignerUtilities.algorithms["MD4WITHRSAENCRYPTION"] = "MD4withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.MD4WithRsaEncryption.Id] = "MD4withRSA";
			SignerUtilities.algorithms["MD5WITHRSA"] = "MD5withRSA";
			SignerUtilities.algorithms["MD5WITHRSAENCRYPTION"] = "MD5withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.MD5WithRsaEncryption.Id] = "MD5withRSA";
			SignerUtilities.algorithms["SHA1WITHRSA"] = "SHA-1withRSA";
			SignerUtilities.algorithms["SHA1WITHRSAENCRYPTION"] = "SHA-1withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id] = "SHA-1withRSA";
			SignerUtilities.algorithms["SHA-1WITHRSA"] = "SHA-1withRSA";
			SignerUtilities.algorithms["SHA224WITHRSA"] = "SHA-224withRSA";
			SignerUtilities.algorithms["SHA224WITHRSAENCRYPTION"] = "SHA-224withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.Sha224WithRsaEncryption.Id] = "SHA-224withRSA";
			SignerUtilities.algorithms["SHA-224WITHRSA"] = "SHA-224withRSA";
			SignerUtilities.algorithms["SHA256WITHRSA"] = "SHA-256withRSA";
			SignerUtilities.algorithms["SHA256WITHRSAENCRYPTION"] = "SHA-256withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id] = "SHA-256withRSA";
			SignerUtilities.algorithms["SHA-256WITHRSA"] = "SHA-256withRSA";
			SignerUtilities.algorithms["SHA384WITHRSA"] = "SHA-384withRSA";
			SignerUtilities.algorithms["SHA384WITHRSAENCRYPTION"] = "SHA-384withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.Sha384WithRsaEncryption.Id] = "SHA-384withRSA";
			SignerUtilities.algorithms["SHA-384WITHRSA"] = "SHA-384withRSA";
			SignerUtilities.algorithms["SHA512WITHRSA"] = "SHA-512withRSA";
			SignerUtilities.algorithms["SHA512WITHRSAENCRYPTION"] = "SHA-512withRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.Sha512WithRsaEncryption.Id] = "SHA-512withRSA";
			SignerUtilities.algorithms["SHA-512WITHRSA"] = "SHA-512withRSA";
			SignerUtilities.algorithms["PSSWITHRSA"] = "PSSwithRSA";
			SignerUtilities.algorithms["RSASSA-PSS"] = "PSSwithRSA";
			SignerUtilities.algorithms[PkcsObjectIdentifiers.IdRsassaPss.Id] = "PSSwithRSA";
			SignerUtilities.algorithms["RSAPSS"] = "PSSwithRSA";
			SignerUtilities.algorithms["SHA1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
			SignerUtilities.algorithms["SHA-1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
			SignerUtilities.algorithms["SHA1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
			SignerUtilities.algorithms["SHA-1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
			SignerUtilities.algorithms["SHA224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
			SignerUtilities.algorithms["SHA-224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
			SignerUtilities.algorithms["SHA224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
			SignerUtilities.algorithms["SHA-224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
			SignerUtilities.algorithms["SHA256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
			SignerUtilities.algorithms["SHA-256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
			SignerUtilities.algorithms["SHA256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
			SignerUtilities.algorithms["SHA-256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
			SignerUtilities.algorithms["SHA384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
			SignerUtilities.algorithms["SHA-384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
			SignerUtilities.algorithms["SHA384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
			SignerUtilities.algorithms["SHA-384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
			SignerUtilities.algorithms["SHA512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
			SignerUtilities.algorithms["SHA-512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
			SignerUtilities.algorithms["SHA512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
			SignerUtilities.algorithms["SHA-512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
			SignerUtilities.algorithms["RIPEMD128WITHRSA"] = "RIPEMD128withRSA";
			SignerUtilities.algorithms["RIPEMD128WITHRSAENCRYPTION"] = "RIPEMD128withRSA";
			SignerUtilities.algorithms[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128.Id] = "RIPEMD128withRSA";
			SignerUtilities.algorithms["RIPEMD160WITHRSA"] = "RIPEMD160withRSA";
			SignerUtilities.algorithms["RIPEMD160WITHRSAENCRYPTION"] = "RIPEMD160withRSA";
			SignerUtilities.algorithms[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160.Id] = "RIPEMD160withRSA";
			SignerUtilities.algorithms["RIPEMD256WITHRSA"] = "RIPEMD256withRSA";
			SignerUtilities.algorithms["RIPEMD256WITHRSAENCRYPTION"] = "RIPEMD256withRSA";
			SignerUtilities.algorithms[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256.Id] = "RIPEMD256withRSA";
			SignerUtilities.algorithms["NONEWITHRSA"] = "RSA";
			SignerUtilities.algorithms["RSAWITHNONE"] = "RSA";
			SignerUtilities.algorithms["RAWRSA"] = "RSA";
			SignerUtilities.algorithms["RAWRSAPSS"] = "RAWRSASSA-PSS";
			SignerUtilities.algorithms["NONEWITHRSAPSS"] = "RAWRSASSA-PSS";
			SignerUtilities.algorithms["NONEWITHRSASSA-PSS"] = "RAWRSASSA-PSS";
			SignerUtilities.algorithms["NONEWITHDSA"] = "NONEwithDSA";
			SignerUtilities.algorithms["DSAWITHNONE"] = "NONEwithDSA";
			SignerUtilities.algorithms["RAWDSA"] = "NONEwithDSA";
			SignerUtilities.algorithms["DSA"] = "SHA-1withDSA";
			SignerUtilities.algorithms["DSAWITHSHA1"] = "SHA-1withDSA";
			SignerUtilities.algorithms["DSAWITHSHA-1"] = "SHA-1withDSA";
			SignerUtilities.algorithms["SHA/DSA"] = "SHA-1withDSA";
			SignerUtilities.algorithms["SHA1/DSA"] = "SHA-1withDSA";
			SignerUtilities.algorithms["SHA-1/DSA"] = "SHA-1withDSA";
			SignerUtilities.algorithms["SHA1WITHDSA"] = "SHA-1withDSA";
			SignerUtilities.algorithms["SHA-1WITHDSA"] = "SHA-1withDSA";
			SignerUtilities.algorithms[X9ObjectIdentifiers.IdDsaWithSha1.Id] = "SHA-1withDSA";
			SignerUtilities.algorithms["DSAWITHSHA224"] = "SHA-224withDSA";
			SignerUtilities.algorithms["DSAWITHSHA-224"] = "SHA-224withDSA";
			SignerUtilities.algorithms["SHA224/DSA"] = "SHA-224withDSA";
			SignerUtilities.algorithms["SHA-224/DSA"] = "SHA-224withDSA";
			SignerUtilities.algorithms["SHA224WITHDSA"] = "SHA-224withDSA";
			SignerUtilities.algorithms["SHA-224WITHDSA"] = "SHA-224withDSA";
			SignerUtilities.algorithms[NistObjectIdentifiers.DsaWithSha224.Id] = "SHA-224withDSA";
			SignerUtilities.algorithms["DSAWITHSHA256"] = "SHA-256withDSA";
			SignerUtilities.algorithms["DSAWITHSHA-256"] = "SHA-256withDSA";
			SignerUtilities.algorithms["SHA256/DSA"] = "SHA-256withDSA";
			SignerUtilities.algorithms["SHA-256/DSA"] = "SHA-256withDSA";
			SignerUtilities.algorithms["SHA256WITHDSA"] = "SHA-256withDSA";
			SignerUtilities.algorithms["SHA-256WITHDSA"] = "SHA-256withDSA";
			SignerUtilities.algorithms[NistObjectIdentifiers.DsaWithSha256.Id] = "SHA-256withDSA";
			SignerUtilities.algorithms["DSAWITHSHA384"] = "SHA-384withDSA";
			SignerUtilities.algorithms["DSAWITHSHA-384"] = "SHA-384withDSA";
			SignerUtilities.algorithms["SHA384/DSA"] = "SHA-384withDSA";
			SignerUtilities.algorithms["SHA-384/DSA"] = "SHA-384withDSA";
			SignerUtilities.algorithms["SHA384WITHDSA"] = "SHA-384withDSA";
			SignerUtilities.algorithms["SHA-384WITHDSA"] = "SHA-384withDSA";
			SignerUtilities.algorithms[NistObjectIdentifiers.DsaWithSha384.Id] = "SHA-384withDSA";
			SignerUtilities.algorithms["DSAWITHSHA512"] = "SHA-512withDSA";
			SignerUtilities.algorithms["DSAWITHSHA-512"] = "SHA-512withDSA";
			SignerUtilities.algorithms["SHA512/DSA"] = "SHA-512withDSA";
			SignerUtilities.algorithms["SHA-512/DSA"] = "SHA-512withDSA";
			SignerUtilities.algorithms["SHA512WITHDSA"] = "SHA-512withDSA";
			SignerUtilities.algorithms["SHA-512WITHDSA"] = "SHA-512withDSA";
			SignerUtilities.algorithms[NistObjectIdentifiers.DsaWithSha512.Id] = "SHA-512withDSA";
			SignerUtilities.algorithms["NONEWITHECDSA"] = "NONEwithECDSA";
			SignerUtilities.algorithms["ECDSAWITHNONE"] = "NONEwithECDSA";
			SignerUtilities.algorithms["ECDSA"] = "SHA-1withECDSA";
			SignerUtilities.algorithms["SHA1/ECDSA"] = "SHA-1withECDSA";
			SignerUtilities.algorithms["SHA-1/ECDSA"] = "SHA-1withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA1"] = "SHA-1withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA-1"] = "SHA-1withECDSA";
			SignerUtilities.algorithms["SHA1WITHECDSA"] = "SHA-1withECDSA";
			SignerUtilities.algorithms["SHA-1WITHECDSA"] = "SHA-1withECDSA";
			SignerUtilities.algorithms[X9ObjectIdentifiers.ECDsaWithSha1.Id] = "SHA-1withECDSA";
			SignerUtilities.algorithms[TeleTrusTObjectIdentifiers.ECSignWithSha1.Id] = "SHA-1withECDSA";
			SignerUtilities.algorithms["SHA224/ECDSA"] = "SHA-224withECDSA";
			SignerUtilities.algorithms["SHA-224/ECDSA"] = "SHA-224withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA224"] = "SHA-224withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA-224"] = "SHA-224withECDSA";
			SignerUtilities.algorithms["SHA224WITHECDSA"] = "SHA-224withECDSA";
			SignerUtilities.algorithms["SHA-224WITHECDSA"] = "SHA-224withECDSA";
			SignerUtilities.algorithms[X9ObjectIdentifiers.ECDsaWithSha224.Id] = "SHA-224withECDSA";
			SignerUtilities.algorithms["SHA256/ECDSA"] = "SHA-256withECDSA";
			SignerUtilities.algorithms["SHA-256/ECDSA"] = "SHA-256withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA256"] = "SHA-256withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA-256"] = "SHA-256withECDSA";
			SignerUtilities.algorithms["SHA256WITHECDSA"] = "SHA-256withECDSA";
			SignerUtilities.algorithms["SHA-256WITHECDSA"] = "SHA-256withECDSA";
			SignerUtilities.algorithms[X9ObjectIdentifiers.ECDsaWithSha256.Id] = "SHA-256withECDSA";
			SignerUtilities.algorithms["SHA384/ECDSA"] = "SHA-384withECDSA";
			SignerUtilities.algorithms["SHA-384/ECDSA"] = "SHA-384withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA384"] = "SHA-384withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA-384"] = "SHA-384withECDSA";
			SignerUtilities.algorithms["SHA384WITHECDSA"] = "SHA-384withECDSA";
			SignerUtilities.algorithms["SHA-384WITHECDSA"] = "SHA-384withECDSA";
			SignerUtilities.algorithms[X9ObjectIdentifiers.ECDsaWithSha384.Id] = "SHA-384withECDSA";
			SignerUtilities.algorithms["SHA512/ECDSA"] = "SHA-512withECDSA";
			SignerUtilities.algorithms["SHA-512/ECDSA"] = "SHA-512withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA512"] = "SHA-512withECDSA";
			SignerUtilities.algorithms["ECDSAWITHSHA-512"] = "SHA-512withECDSA";
			SignerUtilities.algorithms["SHA512WITHECDSA"] = "SHA-512withECDSA";
			SignerUtilities.algorithms["SHA-512WITHECDSA"] = "SHA-512withECDSA";
			SignerUtilities.algorithms[X9ObjectIdentifiers.ECDsaWithSha512.Id] = "SHA-512withECDSA";
			SignerUtilities.algorithms["RIPEMD160/ECDSA"] = "RIPEMD160withECDSA";
			SignerUtilities.algorithms["ECDSAWITHRIPEMD160"] = "RIPEMD160withECDSA";
			SignerUtilities.algorithms["RIPEMD160WITHECDSA"] = "RIPEMD160withECDSA";
			SignerUtilities.algorithms[TeleTrusTObjectIdentifiers.ECSignWithRipeMD160.Id] = "RIPEMD160withECDSA";
			SignerUtilities.algorithms["GOST-3410"] = "GOST3410";
			SignerUtilities.algorithms["GOST-3410-94"] = "GOST3410";
			SignerUtilities.algorithms["GOST3411WITHGOST3410"] = "GOST3410";
			SignerUtilities.algorithms[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94.Id] = "GOST3410";
			SignerUtilities.algorithms["ECGOST-3410"] = "ECGOST3410";
			SignerUtilities.algorithms["ECGOST-3410-2001"] = "ECGOST3410";
			SignerUtilities.algorithms["GOST3411WITHECGOST3410"] = "ECGOST3410";
			SignerUtilities.algorithms[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001.Id] = "ECGOST3410";
			SignerUtilities.oids["MD2withRSA"] = PkcsObjectIdentifiers.MD2WithRsaEncryption;
			SignerUtilities.oids["MD4withRSA"] = PkcsObjectIdentifiers.MD4WithRsaEncryption;
			SignerUtilities.oids["MD5withRSA"] = PkcsObjectIdentifiers.MD5WithRsaEncryption;
			SignerUtilities.oids["SHA-1withRSA"] = PkcsObjectIdentifiers.Sha1WithRsaEncryption;
			SignerUtilities.oids["SHA-224withRSA"] = PkcsObjectIdentifiers.Sha224WithRsaEncryption;
			SignerUtilities.oids["SHA-256withRSA"] = PkcsObjectIdentifiers.Sha256WithRsaEncryption;
			SignerUtilities.oids["SHA-384withRSA"] = PkcsObjectIdentifiers.Sha384WithRsaEncryption;
			SignerUtilities.oids["SHA-512withRSA"] = PkcsObjectIdentifiers.Sha512WithRsaEncryption;
			SignerUtilities.oids["PSSwithRSA"] = PkcsObjectIdentifiers.IdRsassaPss;
			SignerUtilities.oids["SHA-1withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
			SignerUtilities.oids["SHA-224withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
			SignerUtilities.oids["SHA-256withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
			SignerUtilities.oids["SHA-384withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
			SignerUtilities.oids["SHA-512withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
			SignerUtilities.oids["RIPEMD128withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128;
			SignerUtilities.oids["RIPEMD160withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160;
			SignerUtilities.oids["RIPEMD256withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256;
			SignerUtilities.oids["SHA-1withDSA"] = X9ObjectIdentifiers.IdDsaWithSha1;
			SignerUtilities.oids["SHA-1withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha1;
			SignerUtilities.oids["SHA-224withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha224;
			SignerUtilities.oids["SHA-256withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha256;
			SignerUtilities.oids["SHA-384withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha384;
			SignerUtilities.oids["SHA-512withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha512;
			SignerUtilities.oids["GOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94;
			SignerUtilities.oids["ECGOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001;
		}

		public static DerObjectIdentifier GetObjectIdentifier(string mechanism)
		{
			if (mechanism == null)
			{
				throw new ArgumentNullException("mechanism");
			}
			mechanism = Platform.ToUpperInvariant(mechanism);
			string text = (string)SignerUtilities.algorithms[mechanism];
			if (text != null)
			{
				mechanism = text;
			}
			return (DerObjectIdentifier)SignerUtilities.oids[mechanism];
		}

		public static Asn1Encodable GetDefaultX509Parameters(DerObjectIdentifier id)
		{
			return SignerUtilities.GetDefaultX509Parameters(id.Id);
		}

		public static Asn1Encodable GetDefaultX509Parameters(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			algorithm = Platform.ToUpperInvariant(algorithm);
			string text = (string)SignerUtilities.algorithms[algorithm];
			if (text == null)
			{
				text = algorithm;
			}
			if (text == "PSSwithRSA")
			{
				return SignerUtilities.GetPssX509Parameters("SHA-1");
			}
			if (text.EndsWith("withRSAandMGF1"))
			{
				string digestName = text.Substring(0, text.Length - "withRSAandMGF1".Length);
				return SignerUtilities.GetPssX509Parameters(digestName);
			}
			return DerNull.Instance;
		}

		private static Asn1Encodable GetPssX509Parameters(string digestName)
		{
			AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(DigestUtilities.GetObjectIdentifier(digestName), DerNull.Instance);
			AlgorithmIdentifier maskGenAlgorithm = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, algorithmIdentifier);
			int digestSize = DigestUtilities.GetDigest(digestName).GetDigestSize();
			return new RsassaPssParameters(algorithmIdentifier, maskGenAlgorithm, new DerInteger(digestSize), new DerInteger(1));
		}

		public static ISigner GetSigner(DerObjectIdentifier id)
		{
			return SignerUtilities.GetSigner(id.Id);
		}

		public static ISigner GetSigner(string algorithm)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			algorithm = Platform.ToUpperInvariant(algorithm);
			string text = (string)SignerUtilities.algorithms[algorithm];
			if (text == null)
			{
				text = algorithm;
			}
			if (text.Equals("RSA"))
			{
				return new RsaDigestSigner(new NullDigest(), null);
			}
			if (text.Equals("MD2withRSA"))
			{
				return new RsaDigestSigner(new MD2Digest());
			}
			if (text.Equals("MD4withRSA"))
			{
				return new RsaDigestSigner(new MD4Digest());
			}
			if (text.Equals("MD5withRSA"))
			{
				return new RsaDigestSigner(new MD5Digest());
			}
			if (text.Equals("SHA-1withRSA"))
			{
				return new RsaDigestSigner(new Sha1Digest());
			}
			if (text.Equals("SHA-224withRSA"))
			{
				return new RsaDigestSigner(new Sha224Digest());
			}
			if (text.Equals("SHA-256withRSA"))
			{
				return new RsaDigestSigner(new Sha256Digest());
			}
			if (text.Equals("SHA-384withRSA"))
			{
				return new RsaDigestSigner(new Sha384Digest());
			}
			if (text.Equals("SHA-512withRSA"))
			{
				return new RsaDigestSigner(new Sha512Digest());
			}
			if (text.Equals("RIPEMD128withRSA"))
			{
				return new RsaDigestSigner(new RipeMD128Digest());
			}
			if (text.Equals("RIPEMD160withRSA"))
			{
				return new RsaDigestSigner(new RipeMD160Digest());
			}
			if (text.Equals("RIPEMD256withRSA"))
			{
				return new RsaDigestSigner(new RipeMD256Digest());
			}
			if (text.Equals("RAWRSASSA-PSS"))
			{
				return PssSigner.CreateRawSigner(new RsaBlindedEngine(), new Sha1Digest());
			}
			if (text.Equals("PSSwithRSA"))
			{
				return new PssSigner(new RsaBlindedEngine(), new Sha1Digest());
			}
			if (text.Equals("SHA-1withRSAandMGF1"))
			{
				return new PssSigner(new RsaBlindedEngine(), new Sha1Digest());
			}
			if (text.Equals("SHA-224withRSAandMGF1"))
			{
				return new PssSigner(new RsaBlindedEngine(), new Sha224Digest());
			}
			if (text.Equals("SHA-256withRSAandMGF1"))
			{
				return new PssSigner(new RsaBlindedEngine(), new Sha256Digest());
			}
			if (text.Equals("SHA-384withRSAandMGF1"))
			{
				return new PssSigner(new RsaBlindedEngine(), new Sha384Digest());
			}
			if (text.Equals("SHA-512withRSAandMGF1"))
			{
				return new PssSigner(new RsaBlindedEngine(), new Sha512Digest());
			}
			if (text.Equals("NONEwithDSA"))
			{
				return new DsaDigestSigner(new DsaSigner(), new NullDigest());
			}
			if (text.Equals("SHA-1withDSA"))
			{
				return new DsaDigestSigner(new DsaSigner(), new Sha1Digest());
			}
			if (text.Equals("SHA-224withDSA"))
			{
				return new DsaDigestSigner(new DsaSigner(), new Sha224Digest());
			}
			if (text.Equals("SHA-256withDSA"))
			{
				return new DsaDigestSigner(new DsaSigner(), new Sha256Digest());
			}
			if (text.Equals("SHA-384withDSA"))
			{
				return new DsaDigestSigner(new DsaSigner(), new Sha384Digest());
			}
			if (text.Equals("SHA-512withDSA"))
			{
				return new DsaDigestSigner(new DsaSigner(), new Sha512Digest());
			}
			if (text.Equals("NONEwithECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new NullDigest());
			}
			if (text.Equals("SHA-1withECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new Sha1Digest());
			}
			if (text.Equals("SHA-224withECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new Sha224Digest());
			}
			if (text.Equals("SHA-256withECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new Sha256Digest());
			}
			if (text.Equals("SHA-384withECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new Sha384Digest());
			}
			if (text.Equals("SHA-512withECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new Sha512Digest());
			}
			if (text.Equals("RIPEMD160withECDSA"))
			{
				return new DsaDigestSigner(new ECDsaSigner(), new RipeMD160Digest());
			}
			if (text.Equals("SHA1WITHECNR"))
			{
				return new DsaDigestSigner(new ECNRSigner(), new Sha1Digest());
			}
			if (text.Equals("SHA224WITHECNR"))
			{
				return new DsaDigestSigner(new ECNRSigner(), new Sha224Digest());
			}
			if (text.Equals("SHA256WITHECNR"))
			{
				return new DsaDigestSigner(new ECNRSigner(), new Sha256Digest());
			}
			if (text.Equals("SHA384WITHECNR"))
			{
				return new DsaDigestSigner(new ECNRSigner(), new Sha384Digest());
			}
			if (text.Equals("SHA512WITHECNR"))
			{
				return new DsaDigestSigner(new ECNRSigner(), new Sha512Digest());
			}
			if (text.Equals("GOST3410"))
			{
				return new Gost3410DigestSigner(new Gost3410Signer(), new Gost3411Digest());
			}
			if (text.Equals("ECGOST3410"))
			{
				return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411Digest());
			}
			if (text.Equals("SHA1WITHRSA/ISO9796-2"))
			{
				return new Iso9796d2Signer(new RsaBlindedEngine(), new Sha1Digest(), true);
			}
			if (text.Equals("MD5WITHRSA/ISO9796-2"))
			{
				return new Iso9796d2Signer(new RsaBlindedEngine(), new MD5Digest(), true);
			}
			if (text.Equals("RIPEMD160WITHRSA/ISO9796-2"))
			{
				return new Iso9796d2Signer(new RsaBlindedEngine(), new RipeMD160Digest(), true);
			}
			if (text.EndsWith("/X9.31"))
			{
				string text2 = text.Substring(0, text.Length - "/X9.31".Length);
				int num = text2.IndexOf("WITH");
				if (num > 0)
				{
					int num2 = num + "WITH".Length;
					string algorithm2 = text2.Substring(0, num);
					IDigest digest = DigestUtilities.GetDigest(algorithm2);
					string text3 = text2.Substring(num2, text2.Length - num2);
					if (text3.Equals("RSA"))
					{
						IAsymmetricBlockCipher cipher = new RsaBlindedEngine();
						return new X931Signer(cipher, digest);
					}
				}
			}
			throw new SecurityUtilityException("Signer " + algorithm + " not recognised.");
		}

		public static string GetEncodingName(DerObjectIdentifier oid)
		{
			return (string)SignerUtilities.algorithms[oid.Id];
		}
	}
}
