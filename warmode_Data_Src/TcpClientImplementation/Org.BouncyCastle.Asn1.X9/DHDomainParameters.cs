using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X9
{
	public class DHDomainParameters : Asn1Encodable
	{
		private readonly DerInteger p;

		private readonly DerInteger g;

		private readonly DerInteger q;

		private readonly DerInteger j;

		private readonly DHValidationParms validationParms;

		public DerInteger P
		{
			get
			{
				return this.p;
			}
		}

		public DerInteger G
		{
			get
			{
				return this.g;
			}
		}

		public DerInteger Q
		{
			get
			{
				return this.q;
			}
		}

		public DerInteger J
		{
			get
			{
				return this.j;
			}
		}

		public DHValidationParms ValidationParms
		{
			get
			{
				return this.validationParms;
			}
		}

		public static DHDomainParameters GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return DHDomainParameters.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static DHDomainParameters GetInstance(object obj)
		{
			if (obj == null || obj is DHDomainParameters)
			{
				return (DHDomainParameters)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new DHDomainParameters((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid DHDomainParameters: " + obj.GetType().FullName, "obj");
		}

		public DHDomainParameters(DerInteger p, DerInteger g, DerInteger q, DerInteger j, DHValidationParms validationParms)
		{
			if (p == null)
			{
				throw new ArgumentNullException("p");
			}
			if (g == null)
			{
				throw new ArgumentNullException("g");
			}
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			this.p = p;
			this.g = g;
			this.q = q;
			this.j = j;
			this.validationParms = validationParms;
		}

		private DHDomainParameters(Asn1Sequence seq)
		{
			if (seq.Count < 3 || seq.Count > 5)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			IEnumerator enumerator = seq.GetEnumerator();
			this.p = DerInteger.GetInstance(DHDomainParameters.GetNext(enumerator));
			this.g = DerInteger.GetInstance(DHDomainParameters.GetNext(enumerator));
			this.q = DerInteger.GetInstance(DHDomainParameters.GetNext(enumerator));
			Asn1Encodable next = DHDomainParameters.GetNext(enumerator);
			if (next != null && next is DerInteger)
			{
				this.j = DerInteger.GetInstance(next);
				next = DHDomainParameters.GetNext(enumerator);
			}
			if (next != null)
			{
				this.validationParms = DHValidationParms.GetInstance(next.ToAsn1Object());
			}
		}

		private static Asn1Encodable GetNext(IEnumerator e)
		{
			if (!e.MoveNext())
			{
				return null;
			}
			return (Asn1Encodable)e.Current;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.p,
				this.g,
				this.q
			});
			if (this.j != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.j
				});
			}
			if (this.validationParms != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.validationParms
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
