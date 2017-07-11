using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Iana;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Security
{
	public sealed class GeneratorUtilities
	{
		private static readonly IDictionary kgAlgorithms;

		private static readonly IDictionary kpgAlgorithms;

		private static readonly IDictionary defaultKeySizes;

		private GeneratorUtilities()
		{
		}

		static GeneratorUtilities()
		{
			GeneratorUtilities.kgAlgorithms = Platform.CreateHashtable();
			GeneratorUtilities.kpgAlgorithms = Platform.CreateHashtable();
			GeneratorUtilities.defaultKeySizes = Platform.CreateHashtable();
			GeneratorUtilities.AddKgAlgorithm("AES", new object[]
			{
				"AESWRAP"
			});
			GeneratorUtilities.AddKgAlgorithm("AES128", new object[]
			{
				"2.16.840.1.101.3.4.2",
				NistObjectIdentifiers.IdAes128Cbc,
				NistObjectIdentifiers.IdAes128Cfb,
				NistObjectIdentifiers.IdAes128Ecb,
				NistObjectIdentifiers.IdAes128Ofb,
				NistObjectIdentifiers.IdAes128Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("AES192", new object[]
			{
				"2.16.840.1.101.3.4.22",
				NistObjectIdentifiers.IdAes192Cbc,
				NistObjectIdentifiers.IdAes192Cfb,
				NistObjectIdentifiers.IdAes192Ecb,
				NistObjectIdentifiers.IdAes192Ofb,
				NistObjectIdentifiers.IdAes192Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("AES256", new object[]
			{
				"2.16.840.1.101.3.4.42",
				NistObjectIdentifiers.IdAes256Cbc,
				NistObjectIdentifiers.IdAes256Cfb,
				NistObjectIdentifiers.IdAes256Ecb,
				NistObjectIdentifiers.IdAes256Ofb,
				NistObjectIdentifiers.IdAes256Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("BLOWFISH", new object[]
			{
				"1.3.6.1.4.1.3029.1.2"
			});
			GeneratorUtilities.AddKgAlgorithm("CAMELLIA", new object[]
			{
				"CAMELLIAWRAP"
			});
			GeneratorUtilities.AddKgAlgorithm("CAMELLIA128", new object[]
			{
				NttObjectIdentifiers.IdCamellia128Cbc,
				NttObjectIdentifiers.IdCamellia128Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("CAMELLIA192", new object[]
			{
				NttObjectIdentifiers.IdCamellia192Cbc,
				NttObjectIdentifiers.IdCamellia192Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("CAMELLIA256", new object[]
			{
				NttObjectIdentifiers.IdCamellia256Cbc,
				NttObjectIdentifiers.IdCamellia256Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("CAST5", new object[]
			{
				"1.2.840.113533.7.66.10"
			});
			GeneratorUtilities.AddKgAlgorithm("CAST6", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("DES", new object[]
			{
				OiwObjectIdentifiers.DesCbc,
				OiwObjectIdentifiers.DesCfb,
				OiwObjectIdentifiers.DesEcb,
				OiwObjectIdentifiers.DesOfb
			});
			GeneratorUtilities.AddKgAlgorithm("DESEDE", new object[]
			{
				"DESEDEWRAP",
				"TDEA",
				OiwObjectIdentifiers.DesEde
			});
			GeneratorUtilities.AddKgAlgorithm("DESEDE3", new object[]
			{
				PkcsObjectIdentifiers.DesEde3Cbc,
				PkcsObjectIdentifiers.IdAlgCms3DesWrap
			});
			GeneratorUtilities.AddKgAlgorithm("GOST28147", new object[]
			{
				"GOST",
				"GOST-28147",
				CryptoProObjectIdentifiers.GostR28147Cbc
			});
			GeneratorUtilities.AddKgAlgorithm("HC128", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("HC256", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("IDEA", new object[]
			{
				"1.3.6.1.4.1.188.7.1.1.2"
			});
			GeneratorUtilities.AddKgAlgorithm("NOEKEON", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("RC2", new object[]
			{
				PkcsObjectIdentifiers.RC2Cbc,
				PkcsObjectIdentifiers.IdAlgCmsRC2Wrap
			});
			GeneratorUtilities.AddKgAlgorithm("RC4", new object[]
			{
				"ARC4",
				"1.2.840.113549.3.4"
			});
			GeneratorUtilities.AddKgAlgorithm("RC5", new object[]
			{
				"RC5-32"
			});
			GeneratorUtilities.AddKgAlgorithm("RC5-64", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("RC6", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("RIJNDAEL", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("SALSA20", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("SEED", new object[]
			{
				KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap,
				KisaObjectIdentifiers.IdSeedCbc
			});
			GeneratorUtilities.AddKgAlgorithm("SERPENT", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("SKIPJACK", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("TEA", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("TWOFISH", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("VMPC", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("VMPC-KSA3", new object[0]);
			GeneratorUtilities.AddKgAlgorithm("XTEA", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("MD2", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("MD4", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("MD5", new object[]
			{
				IanaObjectIdentifiers.HmacMD5
			});
			GeneratorUtilities.AddHMacKeyGenerator("SHA1", new object[]
			{
				PkcsObjectIdentifiers.IdHmacWithSha1,
				IanaObjectIdentifiers.HmacSha1
			});
			GeneratorUtilities.AddHMacKeyGenerator("SHA224", new object[]
			{
				PkcsObjectIdentifiers.IdHmacWithSha224
			});
			GeneratorUtilities.AddHMacKeyGenerator("SHA256", new object[]
			{
				PkcsObjectIdentifiers.IdHmacWithSha256
			});
			GeneratorUtilities.AddHMacKeyGenerator("SHA384", new object[]
			{
				PkcsObjectIdentifiers.IdHmacWithSha384
			});
			GeneratorUtilities.AddHMacKeyGenerator("SHA512", new object[]
			{
				PkcsObjectIdentifiers.IdHmacWithSha512
			});
			GeneratorUtilities.AddHMacKeyGenerator("SHA512/224", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("SHA512/256", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("SHA3-224", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("SHA3-256", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("SHA3-384", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("SHA3-512", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("RIPEMD128", new object[0]);
			GeneratorUtilities.AddHMacKeyGenerator("RIPEMD160", new object[]
			{
				IanaObjectIdentifiers.HmacRipeMD160
			});
			GeneratorUtilities.AddHMacKeyGenerator("TIGER", new object[]
			{
				IanaObjectIdentifiers.HmacTiger
			});
			GeneratorUtilities.AddKpgAlgorithm("DH", new object[]
			{
				"DIFFIEHELLMAN"
			});
			GeneratorUtilities.AddKpgAlgorithm("DSA", new object[0]);
			GeneratorUtilities.AddKpgAlgorithm("EC", new object[]
			{
				X9ObjectIdentifiers.DHSinglePassStdDHSha1KdfScheme
			});
			GeneratorUtilities.AddKpgAlgorithm("ECDH", new object[]
			{
				"ECIES"
			});
			GeneratorUtilities.AddKpgAlgorithm("ECDHC", new object[0]);
			GeneratorUtilities.AddKpgAlgorithm("ECMQV", new object[]
			{
				X9ObjectIdentifiers.MqvSinglePassSha1KdfScheme
			});
			GeneratorUtilities.AddKpgAlgorithm("ECDSA", new object[0]);
			GeneratorUtilities.AddKpgAlgorithm("ECGOST3410", new object[]
			{
				"ECGOST-3410",
				"GOST-3410-2001"
			});
			GeneratorUtilities.AddKpgAlgorithm("ELGAMAL", new object[0]);
			GeneratorUtilities.AddKpgAlgorithm("GOST3410", new object[]
			{
				"GOST-3410",
				"GOST-3410-94"
			});
			GeneratorUtilities.AddKpgAlgorithm("RSA", new object[]
			{
				"1.2.840.113549.1.1.1"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(64, new string[]
			{
				"DES"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(80, new string[]
			{
				"SKIPJACK"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(128, new string[]
			{
				"AES128",
				"BLOWFISH",
				"CAMELLIA128",
				"CAST5",
				"DESEDE",
				"HC128",
				"HMACMD2",
				"HMACMD4",
				"HMACMD5",
				"HMACRIPEMD128",
				"IDEA",
				"NOEKEON",
				"RC2",
				"RC4",
				"RC5",
				"SALSA20",
				"SEED",
				"TEA",
				"XTEA",
				"VMPC",
				"VMPC-KSA3"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(160, new string[]
			{
				"HMACRIPEMD160",
				"HMACSHA1"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(192, new string[]
			{
				"AES",
				"AES192",
				"CAMELLIA192",
				"DESEDE3",
				"HMACTIGER",
				"RIJNDAEL",
				"SERPENT"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(224, new string[]
			{
				"HMACSHA224"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(256, new string[]
			{
				"AES256",
				"CAMELLIA",
				"CAMELLIA256",
				"CAST6",
				"GOST28147",
				"HC256",
				"HMACSHA256",
				"RC5-64",
				"RC6",
				"TWOFISH"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(384, new string[]
			{
				"HMACSHA384"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(512, new string[]
			{
				"HMACSHA512"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(224, new string[]
			{
				"HMACSHA512/224"
			});
			GeneratorUtilities.AddDefaultKeySizeEntries(256, new string[]
			{
				"HMACSHA512/256"
			});
		}

		private static void AddDefaultKeySizeEntries(int size, params string[] algorithms)
		{
			for (int i = 0; i < algorithms.Length; i++)
			{
				string key = algorithms[i];
				GeneratorUtilities.defaultKeySizes.Add(key, size);
			}
		}

		private static void AddKgAlgorithm(string canonicalName, params object[] aliases)
		{
			GeneratorUtilities.kgAlgorithms[canonicalName] = canonicalName;
			for (int i = 0; i < aliases.Length; i++)
			{
				object obj = aliases[i];
				GeneratorUtilities.kgAlgorithms[obj.ToString()] = canonicalName;
			}
		}

		private static void AddKpgAlgorithm(string canonicalName, params object[] aliases)
		{
			GeneratorUtilities.kpgAlgorithms[canonicalName] = canonicalName;
			for (int i = 0; i < aliases.Length; i++)
			{
				object obj = aliases[i];
				GeneratorUtilities.kpgAlgorithms[obj.ToString()] = canonicalName;
			}
		}

		private static void AddHMacKeyGenerator(string algorithm, params object[] aliases)
		{
			string text = "HMAC" + algorithm;
			GeneratorUtilities.kgAlgorithms[text] = text;
			GeneratorUtilities.kgAlgorithms["HMAC-" + algorithm] = text;
			GeneratorUtilities.kgAlgorithms["HMAC/" + algorithm] = text;
			for (int i = 0; i < aliases.Length; i++)
			{
				object obj = aliases[i];
				GeneratorUtilities.kgAlgorithms[obj.ToString()] = text;
			}
		}

		internal static string GetCanonicalKeyGeneratorAlgorithm(string algorithm)
		{
			return (string)GeneratorUtilities.kgAlgorithms[Platform.ToUpperInvariant(algorithm)];
		}

		internal static string GetCanonicalKeyPairGeneratorAlgorithm(string algorithm)
		{
			return (string)GeneratorUtilities.kpgAlgorithms[Platform.ToUpperInvariant(algorithm)];
		}

		public static CipherKeyGenerator GetKeyGenerator(DerObjectIdentifier oid)
		{
			return GeneratorUtilities.GetKeyGenerator(oid.Id);
		}

		public static CipherKeyGenerator GetKeyGenerator(string algorithm)
		{
			string canonicalKeyGeneratorAlgorithm = GeneratorUtilities.GetCanonicalKeyGeneratorAlgorithm(algorithm);
			if (canonicalKeyGeneratorAlgorithm == null)
			{
				throw new SecurityUtilityException("KeyGenerator " + algorithm + " not recognised.");
			}
			int num = GeneratorUtilities.FindDefaultKeySize(canonicalKeyGeneratorAlgorithm);
			if (num == -1)
			{
				throw new SecurityUtilityException(string.Concat(new string[]
				{
					"KeyGenerator ",
					algorithm,
					" (",
					canonicalKeyGeneratorAlgorithm,
					") not supported."
				}));
			}
			if (canonicalKeyGeneratorAlgorithm == "DES")
			{
				return new DesKeyGenerator(num);
			}
			if (canonicalKeyGeneratorAlgorithm == "DESEDE" || canonicalKeyGeneratorAlgorithm == "DESEDE3")
			{
				return new DesEdeKeyGenerator(num);
			}
			return new CipherKeyGenerator(num);
		}

		public static IAsymmetricCipherKeyPairGenerator GetKeyPairGenerator(DerObjectIdentifier oid)
		{
			return GeneratorUtilities.GetKeyPairGenerator(oid.Id);
		}

		public static IAsymmetricCipherKeyPairGenerator GetKeyPairGenerator(string algorithm)
		{
			string canonicalKeyPairGeneratorAlgorithm = GeneratorUtilities.GetCanonicalKeyPairGeneratorAlgorithm(algorithm);
			if (canonicalKeyPairGeneratorAlgorithm == null)
			{
				throw new SecurityUtilityException("KeyPairGenerator " + algorithm + " not recognised.");
			}
			if (canonicalKeyPairGeneratorAlgorithm == "DH")
			{
				return new DHKeyPairGenerator();
			}
			if (canonicalKeyPairGeneratorAlgorithm == "DSA")
			{
				return new DsaKeyPairGenerator();
			}
			if (canonicalKeyPairGeneratorAlgorithm.StartsWith("EC"))
			{
				return new ECKeyPairGenerator(canonicalKeyPairGeneratorAlgorithm);
			}
			if (canonicalKeyPairGeneratorAlgorithm == "ELGAMAL")
			{
				return new ElGamalKeyPairGenerator();
			}
			if (canonicalKeyPairGeneratorAlgorithm == "GOST3410")
			{
				return new Gost3410KeyPairGenerator();
			}
			if (canonicalKeyPairGeneratorAlgorithm == "RSA")
			{
				return new RsaKeyPairGenerator();
			}
			throw new SecurityUtilityException(string.Concat(new string[]
			{
				"KeyPairGenerator ",
				algorithm,
				" (",
				canonicalKeyPairGeneratorAlgorithm,
				") not supported."
			}));
		}

		internal static int GetDefaultKeySize(DerObjectIdentifier oid)
		{
			return GeneratorUtilities.GetDefaultKeySize(oid.Id);
		}

		internal static int GetDefaultKeySize(string algorithm)
		{
			string canonicalKeyGeneratorAlgorithm = GeneratorUtilities.GetCanonicalKeyGeneratorAlgorithm(algorithm);
			if (canonicalKeyGeneratorAlgorithm == null)
			{
				throw new SecurityUtilityException("KeyGenerator " + algorithm + " not recognised.");
			}
			int num = GeneratorUtilities.FindDefaultKeySize(canonicalKeyGeneratorAlgorithm);
			if (num == -1)
			{
				throw new SecurityUtilityException(string.Concat(new string[]
				{
					"KeyGenerator ",
					algorithm,
					" (",
					canonicalKeyGeneratorAlgorithm,
					") not supported."
				}));
			}
			return num;
		}

		private static int FindDefaultKeySize(string canonicalName)
		{
			if (!GeneratorUtilities.defaultKeySizes.Contains(canonicalName))
			{
				return -1;
			}
			return (int)GeneratorUtilities.defaultKeySizes[canonicalName];
		}
	}
}
