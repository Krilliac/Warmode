using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class CertId : Asn1Encodable
	{
		private readonly GeneralName issuer;

		private readonly DerInteger serialNumber;

		public virtual GeneralName Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public virtual DerInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		private CertId(Asn1Sequence seq)
		{
			this.issuer = GeneralName.GetInstance(seq[0]);
			this.serialNumber = DerInteger.GetInstance(seq[1]);
		}

		public static CertId GetInstance(object obj)
		{
			if (obj is CertId)
			{
				return (CertId)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertId((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public static CertId GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return CertId.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.issuer,
				this.serialNumber
			});
		}
	}
}
