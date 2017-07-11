using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class IssuerAndSerialNumber : Asn1Encodable
	{
		private X509Name name;

		private DerInteger serialNumber;

		public X509Name Name
		{
			get
			{
				return this.name;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public static IssuerAndSerialNumber GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			IssuerAndSerialNumber issuerAndSerialNumber = obj as IssuerAndSerialNumber;
			if (issuerAndSerialNumber != null)
			{
				return issuerAndSerialNumber;
			}
			return new IssuerAndSerialNumber(Asn1Sequence.GetInstance(obj));
		}

		[Obsolete("Use GetInstance() instead")]
		public IssuerAndSerialNumber(Asn1Sequence seq)
		{
			this.name = X509Name.GetInstance(seq[0]);
			this.serialNumber = (DerInteger)seq[1];
		}

		public IssuerAndSerialNumber(X509Name name, BigInteger serialNumber)
		{
			this.name = name;
			this.serialNumber = new DerInteger(serialNumber);
		}

		public IssuerAndSerialNumber(X509Name name, DerInteger serialNumber)
		{
			this.name = name;
			this.serialNumber = serialNumber;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.name,
				this.serialNumber
			});
		}
	}
}
