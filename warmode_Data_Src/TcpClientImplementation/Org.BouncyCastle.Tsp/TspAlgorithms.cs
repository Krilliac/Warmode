using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Tsp
{
	public abstract class TspAlgorithms
	{
		public static readonly string MD5;

		public static readonly string Sha1;

		public static readonly string Sha224;

		public static readonly string Sha256;

		public static readonly string Sha384;

		public static readonly string Sha512;

		public static readonly string RipeMD128;

		public static readonly string RipeMD160;

		public static readonly string RipeMD256;

		public static readonly string Gost3411;

		public static readonly IList Allowed;

		static TspAlgorithms()
		{
			TspAlgorithms.MD5 = PkcsObjectIdentifiers.MD5.Id;
			TspAlgorithms.Sha1 = OiwObjectIdentifiers.IdSha1.Id;
			TspAlgorithms.Sha224 = NistObjectIdentifiers.IdSha224.Id;
			TspAlgorithms.Sha256 = NistObjectIdentifiers.IdSha256.Id;
			TspAlgorithms.Sha384 = NistObjectIdentifiers.IdSha384.Id;
			TspAlgorithms.Sha512 = NistObjectIdentifiers.IdSha512.Id;
			TspAlgorithms.RipeMD128 = TeleTrusTObjectIdentifiers.RipeMD128.Id;
			TspAlgorithms.RipeMD160 = TeleTrusTObjectIdentifiers.RipeMD160.Id;
			TspAlgorithms.RipeMD256 = TeleTrusTObjectIdentifiers.RipeMD256.Id;
			TspAlgorithms.Gost3411 = CryptoProObjectIdentifiers.GostR3411.Id;
			string[] array = new string[]
			{
				TspAlgorithms.Gost3411,
				TspAlgorithms.MD5,
				TspAlgorithms.Sha1,
				TspAlgorithms.Sha224,
				TspAlgorithms.Sha256,
				TspAlgorithms.Sha384,
				TspAlgorithms.Sha512,
				TspAlgorithms.RipeMD128,
				TspAlgorithms.RipeMD160,
				TspAlgorithms.RipeMD256
			};
			TspAlgorithms.Allowed = Platform.CreateArrayList();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string value = array2[i];
				TspAlgorithms.Allowed.Add(value);
			}
		}
	}
}
