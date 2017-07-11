using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class ProtectedPart : Asn1Encodable
	{
		private readonly PkiHeader header;

		private readonly PkiBody body;

		public virtual PkiHeader Header
		{
			get
			{
				return this.header;
			}
		}

		public virtual PkiBody Body
		{
			get
			{
				return this.body;
			}
		}

		private ProtectedPart(Asn1Sequence seq)
		{
			this.header = PkiHeader.GetInstance(seq[0]);
			this.body = PkiBody.GetInstance(seq[1]);
		}

		public static ProtectedPart GetInstance(object obj)
		{
			if (obj is ProtectedPart)
			{
				return (ProtectedPart)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ProtectedPart((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public ProtectedPart(PkiHeader header, PkiBody body)
		{
			this.header = header;
			this.body = body;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.header,
				this.body
			});
		}
	}
}
