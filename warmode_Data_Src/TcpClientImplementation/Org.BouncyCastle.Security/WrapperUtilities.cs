using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Security
{
	public sealed class WrapperUtilities
	{
		private enum WrapAlgorithm
		{
			AESWRAP,
			CAMELLIAWRAP,
			DESEDEWRAP,
			RC2WRAP,
			SEEDWRAP,
			DESEDERFC3211WRAP,
			AESRFC3211WRAP,
			CAMELLIARFC3211WRAP
		}

		private class BufferedCipherWrapper : IWrapper
		{
			private readonly IBufferedCipher cipher;

			private bool forWrapping;

			public string AlgorithmName
			{
				get
				{
					return this.cipher.AlgorithmName;
				}
			}

			public BufferedCipherWrapper(IBufferedCipher cipher)
			{
				this.cipher = cipher;
			}

			public void Init(bool forWrapping, ICipherParameters parameters)
			{
				this.forWrapping = forWrapping;
				this.cipher.Init(forWrapping, parameters);
			}

			public byte[] Wrap(byte[] input, int inOff, int length)
			{
				if (!this.forWrapping)
				{
					throw new InvalidOperationException("Not initialised for wrapping");
				}
				return this.cipher.DoFinal(input, inOff, length);
			}

			public byte[] Unwrap(byte[] input, int inOff, int length)
			{
				if (this.forWrapping)
				{
					throw new InvalidOperationException("Not initialised for unwrapping");
				}
				return this.cipher.DoFinal(input, inOff, length);
			}
		}

		private static readonly IDictionary algorithms;

		private WrapperUtilities()
		{
		}

		static WrapperUtilities()
		{
			WrapperUtilities.algorithms = Platform.CreateHashtable();
			((WrapperUtilities.WrapAlgorithm)Enums.GetArbitraryValue(typeof(WrapperUtilities.WrapAlgorithm))).ToString();
			WrapperUtilities.algorithms[NistObjectIdentifiers.IdAes128Wrap.Id] = "AESWRAP";
			WrapperUtilities.algorithms[NistObjectIdentifiers.IdAes192Wrap.Id] = "AESWRAP";
			WrapperUtilities.algorithms[NistObjectIdentifiers.IdAes256Wrap.Id] = "AESWRAP";
			WrapperUtilities.algorithms[NttObjectIdentifiers.IdCamellia128Wrap.Id] = "CAMELLIAWRAP";
			WrapperUtilities.algorithms[NttObjectIdentifiers.IdCamellia192Wrap.Id] = "CAMELLIAWRAP";
			WrapperUtilities.algorithms[NttObjectIdentifiers.IdCamellia256Wrap.Id] = "CAMELLIAWRAP";
			WrapperUtilities.algorithms[PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id] = "DESEDEWRAP";
			WrapperUtilities.algorithms["TDEAWRAP"] = "DESEDEWRAP";
			WrapperUtilities.algorithms[PkcsObjectIdentifiers.IdAlgCmsRC2Wrap.Id] = "RC2WRAP";
			WrapperUtilities.algorithms[KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap.Id] = "SEEDWRAP";
		}

		public static IWrapper GetWrapper(DerObjectIdentifier oid)
		{
			return WrapperUtilities.GetWrapper(oid.Id);
		}

		public static IWrapper GetWrapper(string algorithm)
		{
			string text = Platform.ToUpperInvariant(algorithm);
			string text2 = (string)WrapperUtilities.algorithms[text];
			if (text2 == null)
			{
				text2 = text;
			}
			try
			{
				switch ((WrapperUtilities.WrapAlgorithm)Enums.GetEnumValue(typeof(WrapperUtilities.WrapAlgorithm), text2))
				{
				case WrapperUtilities.WrapAlgorithm.AESWRAP:
				{
					IWrapper result = new AesWrapEngine();
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.CAMELLIAWRAP:
				{
					IWrapper result = new CamelliaWrapEngine();
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.DESEDEWRAP:
				{
					IWrapper result = new DesEdeWrapEngine();
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.RC2WRAP:
				{
					IWrapper result = new RC2WrapEngine();
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.SEEDWRAP:
				{
					IWrapper result = new SeedWrapEngine();
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.DESEDERFC3211WRAP:
				{
					IWrapper result = new Rfc3211WrapEngine(new DesEdeEngine());
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.AESRFC3211WRAP:
				{
					IWrapper result = new Rfc3211WrapEngine(new AesFastEngine());
					return result;
				}
				case WrapperUtilities.WrapAlgorithm.CAMELLIARFC3211WRAP:
				{
					IWrapper result = new Rfc3211WrapEngine(new CamelliaEngine());
					return result;
				}
				}
			}
			catch (ArgumentException)
			{
			}
			IBufferedCipher cipher = CipherUtilities.GetCipher(algorithm);
			if (cipher != null)
			{
				return new WrapperUtilities.BufferedCipherWrapper(cipher);
			}
			throw new SecurityUtilityException("Wrapper " + algorithm + " not recognised.");
		}

		public static string GetAlgorithmName(DerObjectIdentifier oid)
		{
			return (string)WrapperUtilities.algorithms[oid.Id];
		}
	}
}
