using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Security
{
	public sealed class DigestUtilities
	{
		private enum DigestAlgorithm
		{
			GOST3411,
			MD2,
			MD4,
			MD5,
			RIPEMD128,
			RIPEMD160,
			RIPEMD256,
			RIPEMD320,
			SHA_1,
			SHA_224,
			SHA_256,
			SHA_384,
			SHA_512,
			SHA_512_224,
			SHA_512_256,
			SHA3_224,
			SHA3_256,
			SHA3_384,
			SHA3_512,
			TIGER,
			WHIRLPOOL
		}

		private static readonly IDictionary algorithms;

		private static readonly IDictionary oids;

		public static ICollection Algorithms
		{
			get
			{
				return DigestUtilities.oids.Keys;
			}
		}

		private DigestUtilities()
		{
		}

		static DigestUtilities()
		{
			DigestUtilities.algorithms = Platform.CreateHashtable();
			DigestUtilities.oids = Platform.CreateHashtable();
			((DigestUtilities.DigestAlgorithm)Enums.GetArbitraryValue(typeof(DigestUtilities.DigestAlgorithm))).ToString();
			DigestUtilities.algorithms[PkcsObjectIdentifiers.MD2.Id] = "MD2";
			DigestUtilities.algorithms[PkcsObjectIdentifiers.MD4.Id] = "MD4";
			DigestUtilities.algorithms[PkcsObjectIdentifiers.MD5.Id] = "MD5";
			DigestUtilities.algorithms["SHA1"] = "SHA-1";
			DigestUtilities.algorithms[OiwObjectIdentifiers.IdSha1.Id] = "SHA-1";
			DigestUtilities.algorithms["SHA224"] = "SHA-224";
			DigestUtilities.algorithms[NistObjectIdentifiers.IdSha224.Id] = "SHA-224";
			DigestUtilities.algorithms["SHA256"] = "SHA-256";
			DigestUtilities.algorithms[NistObjectIdentifiers.IdSha256.Id] = "SHA-256";
			DigestUtilities.algorithms["SHA384"] = "SHA-384";
			DigestUtilities.algorithms[NistObjectIdentifiers.IdSha384.Id] = "SHA-384";
			DigestUtilities.algorithms["SHA512"] = "SHA-512";
			DigestUtilities.algorithms[NistObjectIdentifiers.IdSha512.Id] = "SHA-512";
			DigestUtilities.algorithms["SHA512/224"] = "SHA-512/224";
			DigestUtilities.algorithms[NistObjectIdentifiers.IdSha512_224.Id] = "SHA-512/224";
			DigestUtilities.algorithms["SHA512/256"] = "SHA-512/256";
			DigestUtilities.algorithms[NistObjectIdentifiers.IdSha512_256.Id] = "SHA-512/256";
			DigestUtilities.algorithms["RIPEMD-128"] = "RIPEMD128";
			DigestUtilities.algorithms[TeleTrusTObjectIdentifiers.RipeMD128.Id] = "RIPEMD128";
			DigestUtilities.algorithms["RIPEMD-160"] = "RIPEMD160";
			DigestUtilities.algorithms[TeleTrusTObjectIdentifiers.RipeMD160.Id] = "RIPEMD160";
			DigestUtilities.algorithms["RIPEMD-256"] = "RIPEMD256";
			DigestUtilities.algorithms[TeleTrusTObjectIdentifiers.RipeMD256.Id] = "RIPEMD256";
			DigestUtilities.algorithms["RIPEMD-320"] = "RIPEMD320";
			DigestUtilities.algorithms[CryptoProObjectIdentifiers.GostR3411.Id] = "GOST3411";
			DigestUtilities.oids["MD2"] = PkcsObjectIdentifiers.MD2;
			DigestUtilities.oids["MD4"] = PkcsObjectIdentifiers.MD4;
			DigestUtilities.oids["MD5"] = PkcsObjectIdentifiers.MD5;
			DigestUtilities.oids["SHA-1"] = OiwObjectIdentifiers.IdSha1;
			DigestUtilities.oids["SHA-224"] = NistObjectIdentifiers.IdSha224;
			DigestUtilities.oids["SHA-256"] = NistObjectIdentifiers.IdSha256;
			DigestUtilities.oids["SHA-384"] = NistObjectIdentifiers.IdSha384;
			DigestUtilities.oids["SHA-512"] = NistObjectIdentifiers.IdSha512;
			DigestUtilities.oids["SHA-512/224"] = NistObjectIdentifiers.IdSha512_224;
			DigestUtilities.oids["SHA-512/256"] = NistObjectIdentifiers.IdSha512_256;
			DigestUtilities.oids["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
			DigestUtilities.oids["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
			DigestUtilities.oids["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
			DigestUtilities.oids["GOST3411"] = CryptoProObjectIdentifiers.GostR3411;
		}

		public static DerObjectIdentifier GetObjectIdentifier(string mechanism)
		{
			if (mechanism == null)
			{
				throw new ArgumentNullException("mechanism");
			}
			mechanism = Platform.ToUpperInvariant(mechanism);
			string text = (string)DigestUtilities.algorithms[mechanism];
			if (text != null)
			{
				mechanism = text;
			}
			return (DerObjectIdentifier)DigestUtilities.oids[mechanism];
		}

		public static IDigest GetDigest(DerObjectIdentifier id)
		{
			return DigestUtilities.GetDigest(id.Id);
		}

		public static IDigest GetDigest(string algorithm)
		{
			string text = Platform.ToUpperInvariant(algorithm);
			string text2 = (string)DigestUtilities.algorithms[text];
			if (text2 == null)
			{
				text2 = text;
			}
			try
			{
				switch ((DigestUtilities.DigestAlgorithm)Enums.GetEnumValue(typeof(DigestUtilities.DigestAlgorithm), text2))
				{
				case DigestUtilities.DigestAlgorithm.GOST3411:
				{
					IDigest result = new Gost3411Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.MD2:
				{
					IDigest result = new MD2Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.MD4:
				{
					IDigest result = new MD4Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.MD5:
				{
					IDigest result = new MD5Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.RIPEMD128:
				{
					IDigest result = new RipeMD128Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.RIPEMD160:
				{
					IDigest result = new RipeMD160Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.RIPEMD256:
				{
					IDigest result = new RipeMD256Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.RIPEMD320:
				{
					IDigest result = new RipeMD320Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_1:
				{
					IDigest result = new Sha1Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_224:
				{
					IDigest result = new Sha224Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_256:
				{
					IDigest result = new Sha256Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_384:
				{
					IDigest result = new Sha384Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_512:
				{
					IDigest result = new Sha512Digest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_512_224:
				{
					IDigest result = new Sha512tDigest(224);
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA_512_256:
				{
					IDigest result = new Sha512tDigest(256);
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA3_224:
				{
					IDigest result = new Sha3Digest(224);
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA3_256:
				{
					IDigest result = new Sha3Digest(256);
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA3_384:
				{
					IDigest result = new Sha3Digest(384);
					return result;
				}
				case DigestUtilities.DigestAlgorithm.SHA3_512:
				{
					IDigest result = new Sha3Digest(512);
					return result;
				}
				case DigestUtilities.DigestAlgorithm.TIGER:
				{
					IDigest result = new TigerDigest();
					return result;
				}
				case DigestUtilities.DigestAlgorithm.WHIRLPOOL:
				{
					IDigest result = new WhirlpoolDigest();
					return result;
				}
				}
			}
			catch (ArgumentException)
			{
			}
			throw new SecurityUtilityException("Digest " + text2 + " not recognised.");
		}

		public static string GetAlgorithmName(DerObjectIdentifier oid)
		{
			return (string)DigestUtilities.algorithms[oid.Id];
		}

		public static byte[] CalculateDigest(string algorithm, byte[] input)
		{
			IDigest digest = DigestUtilities.GetDigest(algorithm);
			digest.BlockUpdate(input, 0, input.Length);
			return DigestUtilities.DoFinal(digest);
		}

		public static byte[] DoFinal(IDigest digest)
		{
			byte[] array = new byte[digest.GetDigestSize()];
			digest.DoFinal(array, 0);
			return array;
		}

		public static byte[] DoFinal(IDigest digest, byte[] input)
		{
			digest.BlockUpdate(input, 0, input.Length);
			return DigestUtilities.DoFinal(digest);
		}
	}
}
