using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Oiw
{
	public class ElGamalParameter : Asn1Encodable
	{
		internal DerInteger p;

		internal DerInteger g;

		public BigInteger P
		{
			get
			{
				return this.p.PositiveValue;
			}
		}

		public BigInteger G
		{
			get
			{
				return this.g.PositiveValue;
			}
		}

		public ElGamalParameter(BigInteger p, BigInteger g)
		{
			this.p = new DerInteger(p);
			this.g = new DerInteger(g);
		}

		public ElGamalParameter(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			this.p = DerInteger.GetInstance(seq[0]);
			this.g = DerInteger.GetInstance(seq[1]);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.p,
				this.g
			});
		}
	}
}
