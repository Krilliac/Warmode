using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CAKeyUpdAnnContent : Asn1Encodable
	{
		private readonly CmpCertificate oldWithNew;

		private readonly CmpCertificate newWithOld;

		private readonly CmpCertificate newWithNew;

		public virtual CmpCertificate OldWithNew
		{
			get
			{
				return this.oldWithNew;
			}
		}

		public virtual CmpCertificate NewWithOld
		{
			get
			{
				return this.newWithOld;
			}
		}

		public virtual CmpCertificate NewWithNew
		{
			get
			{
				return this.newWithNew;
			}
		}

		private CAKeyUpdAnnContent(Asn1Sequence seq)
		{
			this.oldWithNew = CmpCertificate.GetInstance(seq[0]);
			this.newWithOld = CmpCertificate.GetInstance(seq[1]);
			this.newWithNew = CmpCertificate.GetInstance(seq[2]);
		}

		public static CAKeyUpdAnnContent GetInstance(object obj)
		{
			if (obj is CAKeyUpdAnnContent)
			{
				return (CAKeyUpdAnnContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CAKeyUpdAnnContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.oldWithNew,
				this.newWithOld,
				this.newWithNew
			});
		}
	}
}
