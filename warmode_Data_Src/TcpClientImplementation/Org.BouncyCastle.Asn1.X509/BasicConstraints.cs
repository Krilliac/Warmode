using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class BasicConstraints : Asn1Encodable
	{
		private readonly DerBoolean cA;

		private readonly DerInteger pathLenConstraint;

		public BigInteger PathLenConstraint
		{
			get
			{
				if (this.pathLenConstraint != null)
				{
					return this.pathLenConstraint.Value;
				}
				return null;
			}
		}

		public static BasicConstraints GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return BasicConstraints.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static BasicConstraints GetInstance(object obj)
		{
			if (obj == null || obj is BasicConstraints)
			{
				return (BasicConstraints)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new BasicConstraints((Asn1Sequence)obj);
			}
			if (obj is X509Extension)
			{
				return BasicConstraints.GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private BasicConstraints(Asn1Sequence seq)
		{
			if (seq.Count > 0)
			{
				if (seq[0] is DerBoolean)
				{
					this.cA = DerBoolean.GetInstance(seq[0]);
				}
				else
				{
					this.pathLenConstraint = DerInteger.GetInstance(seq[0]);
				}
				if (seq.Count > 1)
				{
					if (this.cA == null)
					{
						throw new ArgumentException("wrong sequence in constructor", "seq");
					}
					this.pathLenConstraint = DerInteger.GetInstance(seq[1]);
				}
			}
		}

		public BasicConstraints(bool cA)
		{
			if (cA)
			{
				this.cA = DerBoolean.True;
			}
		}

		public BasicConstraints(int pathLenConstraint)
		{
			this.cA = DerBoolean.True;
			this.pathLenConstraint = new DerInteger(pathLenConstraint);
		}

		public bool IsCA()
		{
			return this.cA != null && this.cA.IsTrue;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.cA != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.cA
				});
			}
			if (this.pathLenConstraint != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.pathLenConstraint
				});
			}
			return new DerSequence(asn1EncodableVector);
		}

		public override string ToString()
		{
			if (this.pathLenConstraint == null)
			{
				return "BasicConstraints: isCa(" + this.IsCA() + ")";
			}
			return string.Concat(new object[]
			{
				"BasicConstraints: isCa(",
				this.IsCA(),
				"), pathLenConstraint = ",
				this.pathLenConstraint.Value
			});
		}
	}
}
