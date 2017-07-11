using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class CertStatus : Asn1Encodable, IAsn1Choice
	{
		private readonly int tagNo;

		private readonly Asn1Encodable value;

		public int TagNo
		{
			get
			{
				return this.tagNo;
			}
		}

		public Asn1Encodable Status
		{
			get
			{
				return this.value;
			}
		}

		public CertStatus()
		{
			this.tagNo = 0;
			this.value = DerNull.Instance;
		}

		public CertStatus(RevokedInfo info)
		{
			this.tagNo = 1;
			this.value = info;
		}

		public CertStatus(int tagNo, Asn1Encodable value)
		{
			this.tagNo = tagNo;
			this.value = value;
		}

		public CertStatus(Asn1TaggedObject choice)
		{
			this.tagNo = choice.TagNo;
			switch (choice.TagNo)
			{
			case 0:
			case 2:
				this.value = DerNull.Instance;
				return;
			case 1:
				this.value = RevokedInfo.GetInstance(choice, false);
				return;
			default:
				return;
			}
		}

		public static CertStatus GetInstance(object obj)
		{
			if (obj == null || obj is CertStatus)
			{
				return (CertStatus)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new CertStatus((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerTaggedObject(false, this.tagNo, this.value);
		}
	}
}
