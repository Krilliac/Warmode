using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class PrivateKeyUsagePeriod : Asn1Encodable
	{
		private DerGeneralizedTime _notBefore;

		private DerGeneralizedTime _notAfter;

		public DerGeneralizedTime NotBefore
		{
			get
			{
				return this._notBefore;
			}
		}

		public DerGeneralizedTime NotAfter
		{
			get
			{
				return this._notAfter;
			}
		}

		public static PrivateKeyUsagePeriod GetInstance(object obj)
		{
			if (obj is PrivateKeyUsagePeriod)
			{
				return (PrivateKeyUsagePeriod)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PrivateKeyUsagePeriod((Asn1Sequence)obj);
			}
			if (obj is X509Extension)
			{
				return PrivateKeyUsagePeriod.GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		private PrivateKeyUsagePeriod(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				if (asn1TaggedObject.TagNo == 0)
				{
					this._notBefore = DerGeneralizedTime.GetInstance(asn1TaggedObject, false);
				}
				else if (asn1TaggedObject.TagNo == 1)
				{
					this._notAfter = DerGeneralizedTime.GetInstance(asn1TaggedObject, false);
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this._notBefore != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this._notBefore)
				});
			}
			if (this._notAfter != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this._notAfter)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
