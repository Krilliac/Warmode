using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.Field;
using System;

namespace Org.BouncyCastle.Asn1.X9
{
	public class X9ECParameters : Asn1Encodable
	{
		private X9FieldID fieldID;

		private ECCurve curve;

		private ECPoint g;

		private BigInteger n;

		private BigInteger h;

		private byte[] seed;

		public ECCurve Curve
		{
			get
			{
				return this.curve;
			}
		}

		public ECPoint G
		{
			get
			{
				return this.g;
			}
		}

		public BigInteger N
		{
			get
			{
				return this.n;
			}
		}

		public BigInteger H
		{
			get
			{
				if (this.h == null)
				{
					return BigInteger.One;
				}
				return this.h;
			}
		}

		public X9ECParameters(Asn1Sequence seq)
		{
			if (!(seq[0] is DerInteger) || !((DerInteger)seq[0]).Value.Equals(BigInteger.One))
			{
				throw new ArgumentException("bad version in X9ECParameters");
			}
			X9Curve x9Curve;
			if (seq[2] is X9Curve)
			{
				x9Curve = (X9Curve)seq[2];
			}
			else
			{
				x9Curve = new X9Curve(new X9FieldID((Asn1Sequence)seq[1]), (Asn1Sequence)seq[2]);
			}
			this.curve = x9Curve.Curve;
			if (seq[3] is X9ECPoint)
			{
				this.g = ((X9ECPoint)seq[3]).Point;
			}
			else
			{
				this.g = new X9ECPoint(this.curve, (Asn1OctetString)seq[3]).Point;
			}
			this.n = ((DerInteger)seq[4]).Value;
			this.seed = x9Curve.GetSeed();
			if (seq.Count == 6)
			{
				this.h = ((DerInteger)seq[5]).Value;
			}
		}

		public X9ECParameters(ECCurve curve, ECPoint g, BigInteger n) : this(curve, g, n, BigInteger.One, null)
		{
		}

		public X9ECParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h) : this(curve, g, n, h, null)
		{
		}

		public X9ECParameters(ECCurve curve, ECPoint g, BigInteger n, BigInteger h, byte[] seed)
		{
			this.curve = curve;
			this.g = g.Normalize();
			this.n = n;
			this.h = h;
			this.seed = seed;
			if (ECAlgorithms.IsFpCurve(curve))
			{
				this.fieldID = new X9FieldID(curve.Field.Characteristic);
				return;
			}
			if (!ECAlgorithms.IsF2mCurve(curve))
			{
				throw new ArgumentException("'curve' is of an unsupported type");
			}
			IPolynomialExtensionField polynomialExtensionField = (IPolynomialExtensionField)curve.Field;
			int[] exponentsPresent = polynomialExtensionField.MinimalPolynomial.GetExponentsPresent();
			if (exponentsPresent.Length == 3)
			{
				this.fieldID = new X9FieldID(exponentsPresent[2], exponentsPresent[1]);
				return;
			}
			if (exponentsPresent.Length == 5)
			{
				this.fieldID = new X9FieldID(exponentsPresent[4], exponentsPresent[1], exponentsPresent[2], exponentsPresent[3]);
				return;
			}
			throw new ArgumentException("Only trinomial and pentomial curves are supported");
		}

		public byte[] GetSeed()
		{
			return this.seed;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				new DerInteger(1),
				this.fieldID,
				new X9Curve(this.curve, this.seed),
				new X9ECPoint(this.g),
				new DerInteger(this.n)
			});
			if (this.h != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerInteger(this.h)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
