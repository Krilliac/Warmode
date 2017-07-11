using System;

namespace Org.BouncyCastle.Asn1.X9
{
	public abstract class X9ObjectIdentifiers
	{
		internal const string AnsiX962 = "1.2.840.10045";

		[Obsolete("Use 'id_ecSigType' instead")]
		public const string IdECSigType = "1.2.840.10045.4";

		[Obsolete("Use 'id_publicKeyType' instead")]
		public const string IdPublicKeyType = "1.2.840.10045.2";

		public static readonly DerObjectIdentifier ansi_X9_62 = new DerObjectIdentifier("1.2.840.10045");

		public static readonly DerObjectIdentifier IdFieldType = X9ObjectIdentifiers.ansi_X9_62.Branch("1");

		public static readonly DerObjectIdentifier PrimeField = X9ObjectIdentifiers.IdFieldType.Branch("1");

		public static readonly DerObjectIdentifier CharacteristicTwoField = X9ObjectIdentifiers.IdFieldType.Branch("2");

		public static readonly DerObjectIdentifier GNBasis = X9ObjectIdentifiers.CharacteristicTwoField.Branch("3.1");

		public static readonly DerObjectIdentifier TPBasis = X9ObjectIdentifiers.CharacteristicTwoField.Branch("3.2");

		public static readonly DerObjectIdentifier PPBasis = X9ObjectIdentifiers.CharacteristicTwoField.Branch("3.3");

		public static readonly DerObjectIdentifier id_ecSigType = X9ObjectIdentifiers.ansi_X9_62.Branch("4");

		public static readonly DerObjectIdentifier ECDsaWithSha1 = X9ObjectIdentifiers.id_ecSigType.Branch("1");

		public static readonly DerObjectIdentifier id_publicKeyType = X9ObjectIdentifiers.ansi_X9_62.Branch("2");

		public static readonly DerObjectIdentifier IdECPublicKey = X9ObjectIdentifiers.id_publicKeyType.Branch("1");

		public static readonly DerObjectIdentifier ECDsaWithSha2 = X9ObjectIdentifiers.id_ecSigType.Branch("3");

		public static readonly DerObjectIdentifier ECDsaWithSha224 = X9ObjectIdentifiers.ECDsaWithSha2.Branch("1");

		public static readonly DerObjectIdentifier ECDsaWithSha256 = X9ObjectIdentifiers.ECDsaWithSha2.Branch("2");

		public static readonly DerObjectIdentifier ECDsaWithSha384 = X9ObjectIdentifiers.ECDsaWithSha2.Branch("3");

		public static readonly DerObjectIdentifier ECDsaWithSha512 = X9ObjectIdentifiers.ECDsaWithSha2.Branch("4");

		public static readonly DerObjectIdentifier EllipticCurve = X9ObjectIdentifiers.ansi_X9_62.Branch("3");

		public static readonly DerObjectIdentifier CTwoCurve = X9ObjectIdentifiers.EllipticCurve.Branch("0");

		public static readonly DerObjectIdentifier C2Pnb163v1 = X9ObjectIdentifiers.CTwoCurve.Branch("1");

		public static readonly DerObjectIdentifier C2Pnb163v2 = X9ObjectIdentifiers.CTwoCurve.Branch("2");

		public static readonly DerObjectIdentifier C2Pnb163v3 = X9ObjectIdentifiers.CTwoCurve.Branch("3");

		public static readonly DerObjectIdentifier C2Pnb176w1 = X9ObjectIdentifiers.CTwoCurve.Branch("4");

		public static readonly DerObjectIdentifier C2Tnb191v1 = X9ObjectIdentifiers.CTwoCurve.Branch("5");

		public static readonly DerObjectIdentifier C2Tnb191v2 = X9ObjectIdentifiers.CTwoCurve.Branch("6");

		public static readonly DerObjectIdentifier C2Tnb191v3 = X9ObjectIdentifiers.CTwoCurve.Branch("7");

		public static readonly DerObjectIdentifier C2Onb191v4 = X9ObjectIdentifiers.CTwoCurve.Branch("8");

		public static readonly DerObjectIdentifier C2Onb191v5 = X9ObjectIdentifiers.CTwoCurve.Branch("9");

		public static readonly DerObjectIdentifier C2Pnb208w1 = X9ObjectIdentifiers.CTwoCurve.Branch("10");

		public static readonly DerObjectIdentifier C2Tnb239v1 = X9ObjectIdentifiers.CTwoCurve.Branch("11");

		public static readonly DerObjectIdentifier C2Tnb239v2 = X9ObjectIdentifiers.CTwoCurve.Branch("12");

		public static readonly DerObjectIdentifier C2Tnb239v3 = X9ObjectIdentifiers.CTwoCurve.Branch("13");

		public static readonly DerObjectIdentifier C2Onb239v4 = X9ObjectIdentifiers.CTwoCurve.Branch("14");

		public static readonly DerObjectIdentifier C2Onb239v5 = X9ObjectIdentifiers.CTwoCurve.Branch("15");

		public static readonly DerObjectIdentifier C2Pnb272w1 = X9ObjectIdentifiers.CTwoCurve.Branch("16");

		public static readonly DerObjectIdentifier C2Pnb304w1 = X9ObjectIdentifiers.CTwoCurve.Branch("17");

		public static readonly DerObjectIdentifier C2Tnb359v1 = X9ObjectIdentifiers.CTwoCurve.Branch("18");

		public static readonly DerObjectIdentifier C2Pnb368w1 = X9ObjectIdentifiers.CTwoCurve.Branch("19");

		public static readonly DerObjectIdentifier C2Tnb431r1 = X9ObjectIdentifiers.CTwoCurve.Branch("20");

		public static readonly DerObjectIdentifier PrimeCurve = X9ObjectIdentifiers.EllipticCurve.Branch("1");

		public static readonly DerObjectIdentifier Prime192v1 = X9ObjectIdentifiers.PrimeCurve.Branch("1");

		public static readonly DerObjectIdentifier Prime192v2 = X9ObjectIdentifiers.PrimeCurve.Branch("2");

		public static readonly DerObjectIdentifier Prime192v3 = X9ObjectIdentifiers.PrimeCurve.Branch("3");

		public static readonly DerObjectIdentifier Prime239v1 = X9ObjectIdentifiers.PrimeCurve.Branch("4");

		public static readonly DerObjectIdentifier Prime239v2 = X9ObjectIdentifiers.PrimeCurve.Branch("5");

		public static readonly DerObjectIdentifier Prime239v3 = X9ObjectIdentifiers.PrimeCurve.Branch("6");

		public static readonly DerObjectIdentifier Prime256v1 = X9ObjectIdentifiers.PrimeCurve.Branch("7");

		public static readonly DerObjectIdentifier IdDsa = new DerObjectIdentifier("1.2.840.10040.4.1");

		public static readonly DerObjectIdentifier IdDsaWithSha1 = new DerObjectIdentifier("1.2.840.10040.4.3");

		public static readonly DerObjectIdentifier X9x63Scheme = new DerObjectIdentifier("1.3.133.16.840.63.0");

		public static readonly DerObjectIdentifier DHSinglePassStdDHSha1KdfScheme = X9ObjectIdentifiers.X9x63Scheme.Branch("2");

		public static readonly DerObjectIdentifier DHSinglePassCofactorDHSha1KdfScheme = X9ObjectIdentifiers.X9x63Scheme.Branch("3");

		public static readonly DerObjectIdentifier MqvSinglePassSha1KdfScheme = X9ObjectIdentifiers.X9x63Scheme.Branch("16");

		public static readonly DerObjectIdentifier ansi_x9_42 = new DerObjectIdentifier("1.2.840.10046");

		public static readonly DerObjectIdentifier DHPublicNumber = X9ObjectIdentifiers.ansi_x9_42.Branch("2.1");

		public static readonly DerObjectIdentifier X9x42Schemes = X9ObjectIdentifiers.ansi_x9_42.Branch("2.3");

		public static readonly DerObjectIdentifier DHStatic = X9ObjectIdentifiers.X9x42Schemes.Branch("1");

		public static readonly DerObjectIdentifier DHEphem = X9ObjectIdentifiers.X9x42Schemes.Branch("2");

		public static readonly DerObjectIdentifier DHOneFlow = X9ObjectIdentifiers.X9x42Schemes.Branch("3");

		public static readonly DerObjectIdentifier DHHybrid1 = X9ObjectIdentifiers.X9x42Schemes.Branch("4");

		public static readonly DerObjectIdentifier DHHybrid2 = X9ObjectIdentifiers.X9x42Schemes.Branch("5");

		public static readonly DerObjectIdentifier DHHybridOneFlow = X9ObjectIdentifiers.X9x42Schemes.Branch("6");

		public static readonly DerObjectIdentifier Mqv2 = X9ObjectIdentifiers.X9x42Schemes.Branch("7");

		public static readonly DerObjectIdentifier Mqv1 = X9ObjectIdentifiers.X9x42Schemes.Branch("8");
	}
}
