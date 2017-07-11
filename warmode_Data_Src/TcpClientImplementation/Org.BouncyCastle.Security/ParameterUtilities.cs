using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Security
{
	public sealed class ParameterUtilities
	{
		private static readonly IDictionary algorithms;

		private static readonly IDictionary basicIVSizes;

		private ParameterUtilities()
		{
		}

		static ParameterUtilities()
		{
			ParameterUtilities.algorithms = Platform.CreateHashtable();
			ParameterUtilities.basicIVSizes = Platform.CreateHashtable();
			ParameterUtilities.AddAlgorithm("AES", new object[]
			{
				"AESWRAP"
			});
			ParameterUtilities.AddAlgorithm("AES128", new object[]
			{
				"2.16.840.1.101.3.4.2",
				NistObjectIdentifiers.IdAes128Cbc,
				NistObjectIdentifiers.IdAes128Cfb,
				NistObjectIdentifiers.IdAes128Ecb,
				NistObjectIdentifiers.IdAes128Ofb,
				NistObjectIdentifiers.IdAes128Wrap
			});
			ParameterUtilities.AddAlgorithm("AES192", new object[]
			{
				"2.16.840.1.101.3.4.22",
				NistObjectIdentifiers.IdAes192Cbc,
				NistObjectIdentifiers.IdAes192Cfb,
				NistObjectIdentifiers.IdAes192Ecb,
				NistObjectIdentifiers.IdAes192Ofb,
				NistObjectIdentifiers.IdAes192Wrap
			});
			ParameterUtilities.AddAlgorithm("AES256", new object[]
			{
				"2.16.840.1.101.3.4.42",
				NistObjectIdentifiers.IdAes256Cbc,
				NistObjectIdentifiers.IdAes256Cfb,
				NistObjectIdentifiers.IdAes256Ecb,
				NistObjectIdentifiers.IdAes256Ofb,
				NistObjectIdentifiers.IdAes256Wrap
			});
			ParameterUtilities.AddAlgorithm("BLOWFISH", new object[]
			{
				"1.3.6.1.4.1.3029.1.2"
			});
			ParameterUtilities.AddAlgorithm("CAMELLIA", new object[]
			{
				"CAMELLIAWRAP"
			});
			ParameterUtilities.AddAlgorithm("CAMELLIA128", new object[]
			{
				NttObjectIdentifiers.IdCamellia128Cbc,
				NttObjectIdentifiers.IdCamellia128Wrap
			});
			ParameterUtilities.AddAlgorithm("CAMELLIA192", new object[]
			{
				NttObjectIdentifiers.IdCamellia192Cbc,
				NttObjectIdentifiers.IdCamellia192Wrap
			});
			ParameterUtilities.AddAlgorithm("CAMELLIA256", new object[]
			{
				NttObjectIdentifiers.IdCamellia256Cbc,
				NttObjectIdentifiers.IdCamellia256Wrap
			});
			ParameterUtilities.AddAlgorithm("CAST5", new object[]
			{
				"1.2.840.113533.7.66.10"
			});
			ParameterUtilities.AddAlgorithm("CAST6", new object[0]);
			ParameterUtilities.AddAlgorithm("DES", new object[]
			{
				OiwObjectIdentifiers.DesCbc,
				OiwObjectIdentifiers.DesCfb,
				OiwObjectIdentifiers.DesEcb,
				OiwObjectIdentifiers.DesOfb
			});
			ParameterUtilities.AddAlgorithm("DESEDE", new object[]
			{
				"DESEDEWRAP",
				"TDEA",
				OiwObjectIdentifiers.DesEde,
				PkcsObjectIdentifiers.IdAlgCms3DesWrap
			});
			ParameterUtilities.AddAlgorithm("DESEDE3", new object[]
			{
				PkcsObjectIdentifiers.DesEde3Cbc
			});
			ParameterUtilities.AddAlgorithm("GOST28147", new object[]
			{
				"GOST",
				"GOST-28147",
				CryptoProObjectIdentifiers.GostR28147Cbc
			});
			ParameterUtilities.AddAlgorithm("HC128", new object[0]);
			ParameterUtilities.AddAlgorithm("HC256", new object[0]);
			ParameterUtilities.AddAlgorithm("IDEA", new object[]
			{
				"1.3.6.1.4.1.188.7.1.1.2"
			});
			ParameterUtilities.AddAlgorithm("NOEKEON", new object[0]);
			ParameterUtilities.AddAlgorithm("RC2", new object[]
			{
				PkcsObjectIdentifiers.RC2Cbc,
				PkcsObjectIdentifiers.IdAlgCmsRC2Wrap
			});
			ParameterUtilities.AddAlgorithm("RC4", new object[]
			{
				"ARC4",
				"1.2.840.113549.3.4"
			});
			ParameterUtilities.AddAlgorithm("RC5", new object[]
			{
				"RC5-32"
			});
			ParameterUtilities.AddAlgorithm("RC5-64", new object[0]);
			ParameterUtilities.AddAlgorithm("RC6", new object[0]);
			ParameterUtilities.AddAlgorithm("RIJNDAEL", new object[0]);
			ParameterUtilities.AddAlgorithm("SALSA20", new object[0]);
			ParameterUtilities.AddAlgorithm("SEED", new object[]
			{
				KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap,
				KisaObjectIdentifiers.IdSeedCbc
			});
			ParameterUtilities.AddAlgorithm("SERPENT", new object[0]);
			ParameterUtilities.AddAlgorithm("SKIPJACK", new object[0]);
			ParameterUtilities.AddAlgorithm("TEA", new object[0]);
			ParameterUtilities.AddAlgorithm("TWOFISH", new object[0]);
			ParameterUtilities.AddAlgorithm("VMPC", new object[0]);
			ParameterUtilities.AddAlgorithm("VMPC-KSA3", new object[0]);
			ParameterUtilities.AddAlgorithm("XTEA", new object[0]);
			ParameterUtilities.AddBasicIVSizeEntries(8, new string[]
			{
				"BLOWFISH",
				"DES",
				"DESEDE",
				"DESEDE3"
			});
			ParameterUtilities.AddBasicIVSizeEntries(16, new string[]
			{
				"AES",
				"AES128",
				"AES192",
				"AES256",
				"CAMELLIA",
				"CAMELLIA128",
				"CAMELLIA192",
				"CAMELLIA256",
				"NOEKEON",
				"SEED"
			});
		}

		private static void AddAlgorithm(string canonicalName, params object[] aliases)
		{
			ParameterUtilities.algorithms[canonicalName] = canonicalName;
			for (int i = 0; i < aliases.Length; i++)
			{
				object obj = aliases[i];
				ParameterUtilities.algorithms[obj.ToString()] = canonicalName;
			}
		}

		private static void AddBasicIVSizeEntries(int size, params string[] algorithms)
		{
			for (int i = 0; i < algorithms.Length; i++)
			{
				string key = algorithms[i];
				ParameterUtilities.basicIVSizes.Add(key, size);
			}
		}

		public static string GetCanonicalAlgorithmName(string algorithm)
		{
			return (string)ParameterUtilities.algorithms[Platform.ToUpperInvariant(algorithm)];
		}

		public static KeyParameter CreateKeyParameter(DerObjectIdentifier algOid, byte[] keyBytes)
		{
			return ParameterUtilities.CreateKeyParameter(algOid.Id, keyBytes, 0, keyBytes.Length);
		}

		public static KeyParameter CreateKeyParameter(string algorithm, byte[] keyBytes)
		{
			return ParameterUtilities.CreateKeyParameter(algorithm, keyBytes, 0, keyBytes.Length);
		}

		public static KeyParameter CreateKeyParameter(DerObjectIdentifier algOid, byte[] keyBytes, int offset, int length)
		{
			return ParameterUtilities.CreateKeyParameter(algOid.Id, keyBytes, offset, length);
		}

		public static KeyParameter CreateKeyParameter(string algorithm, byte[] keyBytes, int offset, int length)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			string canonicalAlgorithmName = ParameterUtilities.GetCanonicalAlgorithmName(algorithm);
			if (canonicalAlgorithmName == null)
			{
				throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
			}
			if (canonicalAlgorithmName == "DES")
			{
				return new DesParameters(keyBytes, offset, length);
			}
			if (canonicalAlgorithmName == "DESEDE" || canonicalAlgorithmName == "DESEDE3")
			{
				return new DesEdeParameters(keyBytes, offset, length);
			}
			if (canonicalAlgorithmName == "RC2")
			{
				return new RC2Parameters(keyBytes, offset, length);
			}
			return new KeyParameter(keyBytes, offset, length);
		}

		public static ICipherParameters GetCipherParameters(DerObjectIdentifier algOid, ICipherParameters key, Asn1Object asn1Params)
		{
			return ParameterUtilities.GetCipherParameters(algOid.Id, key, asn1Params);
		}

		public static ICipherParameters GetCipherParameters(string algorithm, ICipherParameters key, Asn1Object asn1Params)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			string canonicalAlgorithmName = ParameterUtilities.GetCanonicalAlgorithmName(algorithm);
			if (canonicalAlgorithmName == null)
			{
				throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
			}
			byte[] array = null;
			try
			{
				int num = ParameterUtilities.FindBasicIVSize(canonicalAlgorithmName);
				if (num != -1 || canonicalAlgorithmName == "RIJNDAEL" || canonicalAlgorithmName == "SKIPJACK" || canonicalAlgorithmName == "TWOFISH")
				{
					array = ((Asn1OctetString)asn1Params).GetOctets();
				}
				else if (canonicalAlgorithmName == "CAST5")
				{
					array = Cast5CbcParameters.GetInstance(asn1Params).GetIV();
				}
				else if (canonicalAlgorithmName == "IDEA")
				{
					array = IdeaCbcPar.GetInstance(asn1Params).GetIV();
				}
				else if (canonicalAlgorithmName == "RC2")
				{
					array = RC2CbcParameter.GetInstance(asn1Params).GetIV();
				}
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("Could not process ASN.1 parameters", innerException);
			}
			if (array != null)
			{
				return new ParametersWithIV(key, array);
			}
			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}

		public static Asn1Encodable GenerateParameters(DerObjectIdentifier algID, SecureRandom random)
		{
			return ParameterUtilities.GenerateParameters(algID.Id, random);
		}

		public static Asn1Encodable GenerateParameters(string algorithm, SecureRandom random)
		{
			if (algorithm == null)
			{
				throw new ArgumentNullException("algorithm");
			}
			string canonicalAlgorithmName = ParameterUtilities.GetCanonicalAlgorithmName(algorithm);
			if (canonicalAlgorithmName == null)
			{
				throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
			}
			int num = ParameterUtilities.FindBasicIVSize(canonicalAlgorithmName);
			if (num != -1)
			{
				return ParameterUtilities.CreateIVOctetString(random, num);
			}
			if (canonicalAlgorithmName == "CAST5")
			{
				return new Cast5CbcParameters(ParameterUtilities.CreateIV(random, 8), 128);
			}
			if (canonicalAlgorithmName == "IDEA")
			{
				return new IdeaCbcPar(ParameterUtilities.CreateIV(random, 8));
			}
			if (canonicalAlgorithmName == "RC2")
			{
				return new RC2CbcParameter(ParameterUtilities.CreateIV(random, 8));
			}
			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}

		private static Asn1OctetString CreateIVOctetString(SecureRandom random, int ivLength)
		{
			return new DerOctetString(ParameterUtilities.CreateIV(random, ivLength));
		}

		private static byte[] CreateIV(SecureRandom random, int ivLength)
		{
			byte[] array = new byte[ivLength];
			random.NextBytes(array);
			return array;
		}

		private static int FindBasicIVSize(string canonicalName)
		{
			if (!ParameterUtilities.basicIVSizes.Contains(canonicalName))
			{
				return -1;
			}
			return (int)ParameterUtilities.basicIVSizes[canonicalName];
		}
	}
}
