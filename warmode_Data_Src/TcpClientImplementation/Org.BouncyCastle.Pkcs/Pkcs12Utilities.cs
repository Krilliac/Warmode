using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.IO;

namespace Org.BouncyCastle.Pkcs
{
	public class Pkcs12Utilities
	{
		public static byte[] ConvertToDefiniteLength(byte[] berPkcs12File)
		{
			Pfx pfx = new Pfx(Asn1Sequence.GetInstance(Asn1Object.FromByteArray(berPkcs12File)));
			return pfx.GetEncoded("DER");
		}

		public static byte[] ConvertToDefiniteLength(byte[] berPkcs12File, char[] passwd)
		{
			Pfx pfx = new Pfx(Asn1Sequence.GetInstance(Asn1Object.FromByteArray(berPkcs12File)));
			ContentInfo contentInfo = pfx.AuthSafe;
			Asn1OctetString instance = Asn1OctetString.GetInstance(contentInfo.Content);
			Asn1Object asn1Object = Asn1Object.FromByteArray(instance.GetOctets());
			contentInfo = new ContentInfo(contentInfo.ContentType, new DerOctetString(asn1Object.GetEncoded("DER")));
			MacData macData = pfx.MacData;
			try
			{
				int intValue = macData.IterationCount.IntValue;
				byte[] octets = Asn1OctetString.GetInstance(contentInfo.Content).GetOctets();
				byte[] digest = Pkcs12Store.CalculatePbeMac(macData.Mac.AlgorithmID.ObjectID, macData.GetSalt(), intValue, passwd, false, octets);
				AlgorithmIdentifier algID = new AlgorithmIdentifier(macData.Mac.AlgorithmID.ObjectID, DerNull.Instance);
				DigestInfo digInfo = new DigestInfo(algID, digest);
				macData = new MacData(digInfo, macData.GetSalt(), intValue);
			}
			catch (Exception ex)
			{
				throw new IOException("error constructing MAC: " + ex.ToString());
			}
			pfx = new Pfx(contentInfo, macData);
			return pfx.GetEncoded("DER");
		}
	}
}
