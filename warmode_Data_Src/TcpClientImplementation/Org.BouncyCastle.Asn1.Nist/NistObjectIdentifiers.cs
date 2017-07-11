using System;

namespace Org.BouncyCastle.Asn1.Nist
{
	public sealed class NistObjectIdentifiers
	{
		public static readonly DerObjectIdentifier NistAlgorithm = new DerObjectIdentifier("2.16.840.1.101.3.4");

		public static readonly DerObjectIdentifier HashAlgs = NistObjectIdentifiers.NistAlgorithm.Branch("2");

		public static readonly DerObjectIdentifier IdSha256 = NistObjectIdentifiers.HashAlgs.Branch("1");

		public static readonly DerObjectIdentifier IdSha384 = NistObjectIdentifiers.HashAlgs.Branch("2");

		public static readonly DerObjectIdentifier IdSha512 = NistObjectIdentifiers.HashAlgs.Branch("3");

		public static readonly DerObjectIdentifier IdSha224 = NistObjectIdentifiers.HashAlgs.Branch("4");

		public static readonly DerObjectIdentifier IdSha512_224 = NistObjectIdentifiers.HashAlgs.Branch("5");

		public static readonly DerObjectIdentifier IdSha512_256 = NistObjectIdentifiers.HashAlgs.Branch("6");

		public static readonly DerObjectIdentifier Aes = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".1");

		public static readonly DerObjectIdentifier IdAes128Ecb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".1");

		public static readonly DerObjectIdentifier IdAes128Cbc = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".2");

		public static readonly DerObjectIdentifier IdAes128Ofb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".3");

		public static readonly DerObjectIdentifier IdAes128Cfb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".4");

		public static readonly DerObjectIdentifier IdAes128Wrap = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".5");

		public static readonly DerObjectIdentifier IdAes128Gcm = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".6");

		public static readonly DerObjectIdentifier IdAes128Ccm = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".7");

		public static readonly DerObjectIdentifier IdAes192Ecb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".21");

		public static readonly DerObjectIdentifier IdAes192Cbc = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".22");

		public static readonly DerObjectIdentifier IdAes192Ofb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".23");

		public static readonly DerObjectIdentifier IdAes192Cfb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".24");

		public static readonly DerObjectIdentifier IdAes192Wrap = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".25");

		public static readonly DerObjectIdentifier IdAes192Gcm = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".26");

		public static readonly DerObjectIdentifier IdAes192Ccm = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".27");

		public static readonly DerObjectIdentifier IdAes256Ecb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".41");

		public static readonly DerObjectIdentifier IdAes256Cbc = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".42");

		public static readonly DerObjectIdentifier IdAes256Ofb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".43");

		public static readonly DerObjectIdentifier IdAes256Cfb = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".44");

		public static readonly DerObjectIdentifier IdAes256Wrap = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".45");

		public static readonly DerObjectIdentifier IdAes256Gcm = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".46");

		public static readonly DerObjectIdentifier IdAes256Ccm = new DerObjectIdentifier(NistObjectIdentifiers.Aes + ".47");

		public static readonly DerObjectIdentifier IdDsaWithSha2 = new DerObjectIdentifier(NistObjectIdentifiers.NistAlgorithm + ".3");

		public static readonly DerObjectIdentifier DsaWithSha224 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".1");

		public static readonly DerObjectIdentifier DsaWithSha256 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".2");

		public static readonly DerObjectIdentifier DsaWithSha384 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".3");

		public static readonly DerObjectIdentifier DsaWithSha512 = new DerObjectIdentifier(NistObjectIdentifiers.IdDsaWithSha2 + ".4");

		private NistObjectIdentifiers()
		{
		}
	}
}
