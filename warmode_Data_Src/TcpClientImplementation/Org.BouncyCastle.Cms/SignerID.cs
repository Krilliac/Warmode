using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Store;
using System;

namespace Org.BouncyCastle.Cms
{
	public class SignerID : X509CertStoreSelector
	{
		public override int GetHashCode()
		{
			int num = Arrays.GetHashCode(base.SubjectKeyIdentifier);
			BigInteger serialNumber = base.SerialNumber;
			if (serialNumber != null)
			{
				num ^= serialNumber.GetHashCode();
			}
			X509Name issuer = base.Issuer;
			if (issuer != null)
			{
				num ^= issuer.GetHashCode();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return false;
			}
			SignerID signerID = obj as SignerID;
			return signerID != null && (Arrays.AreEqual(base.SubjectKeyIdentifier, signerID.SubjectKeyIdentifier) && object.Equals(base.SerialNumber, signerID.SerialNumber)) && X509CertStoreSelector.IssuersMatch(base.Issuer, signerID.Issuer);
		}
	}
}
