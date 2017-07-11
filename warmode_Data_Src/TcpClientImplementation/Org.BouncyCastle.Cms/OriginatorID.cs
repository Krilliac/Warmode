using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Store;
using System;

namespace Org.BouncyCastle.Cms
{
	public class OriginatorID : X509CertStoreSelector
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
			OriginatorID originatorID = obj as OriginatorID;
			return originatorID != null && (Arrays.AreEqual(base.SubjectKeyIdentifier, originatorID.SubjectKeyIdentifier) && object.Equals(base.SerialNumber, originatorID.SerialNumber)) && X509CertStoreSelector.IssuersMatch(base.Issuer, originatorID.Issuer);
		}
	}
}
