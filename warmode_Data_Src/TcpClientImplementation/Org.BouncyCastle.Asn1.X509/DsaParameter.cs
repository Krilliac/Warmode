using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class DsaParameter : Asn1Encodable
	{
		internal readonly DerInteger p;

		internal readonly DerInteger q;

		internal readonly DerInteger g;

		public BigInteger P
		{
			get
			{
				return this.p.PositiveValue;
			}
		}

		public BigInteger Q
		{
			get
			{
				return this.q.PositiveValue;
			}
		}

		public BigInteger G
		{
			get
			{
				return this.g.PositiveValue;
			}
		}

		public static DsaParameter GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DsaParameter.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static DsaParameter GetInstance(object obj)
		{
			if (obj == null || obj is DsaParameter)
			{
				return (DsaParameter)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new DsaParameter((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid DsaParameter: " + obj.GetType().Name);
		}

		public DsaParameter(BigInteger p, BigInteger q, BigInteger g)
		{
			this.p = new DerInteger(p);
			this.q = new DerInteger(q);
			this.g = new DerInteger(g);
		}

		private DsaParameter(Asn1Sequence seq)
		{
			if (seq.Count != 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.p = DerInteger.GetInstance(seq[0]);
			this.q = DerInteger.GetInstance(seq[1]);
			this.g = DerInteger.GetInstance(seq[2]);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.p,
				this.q,
				this.g
			});
		}
	}
}
