using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class RsaPrivateKeyStructure : Asn1Encodable
	{
		private readonly BigInteger modulus;

		private readonly BigInteger publicExponent;

		private readonly BigInteger privateExponent;

		private readonly BigInteger prime1;

		private readonly BigInteger prime2;

		private readonly BigInteger exponent1;

		private readonly BigInteger exponent2;

		private readonly BigInteger coefficient;

		public BigInteger Modulus
		{
			get
			{
				return this.modulus;
			}
		}

		public BigInteger PublicExponent
		{
			get
			{
				return this.publicExponent;
			}
		}

		public BigInteger PrivateExponent
		{
			get
			{
				return this.privateExponent;
			}
		}

		public BigInteger Prime1
		{
			get
			{
				return this.prime1;
			}
		}

		public BigInteger Prime2
		{
			get
			{
				return this.prime2;
			}
		}

		public BigInteger Exponent1
		{
			get
			{
				return this.exponent1;
			}
		}

		public BigInteger Exponent2
		{
			get
			{
				return this.exponent2;
			}
		}

		public BigInteger Coefficient
		{
			get
			{
				return this.coefficient;
			}
		}

		public static RsaPrivateKeyStructure GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return RsaPrivateKeyStructure.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static RsaPrivateKeyStructure GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is RsaPrivateKeyStructure)
			{
				return (RsaPrivateKeyStructure)obj;
			}
			return new RsaPrivateKeyStructure(Asn1Sequence.GetInstance(obj));
		}

		public RsaPrivateKeyStructure(BigInteger modulus, BigInteger publicExponent, BigInteger privateExponent, BigInteger prime1, BigInteger prime2, BigInteger exponent1, BigInteger exponent2, BigInteger coefficient)
		{
			this.modulus = modulus;
			this.publicExponent = publicExponent;
			this.privateExponent = privateExponent;
			this.prime1 = prime1;
			this.prime2 = prime2;
			this.exponent1 = exponent1;
			this.exponent2 = exponent2;
			this.coefficient = coefficient;
		}

		[Obsolete("Use 'GetInstance' method(s) instead")]
		public RsaPrivateKeyStructure(Asn1Sequence seq)
		{
			BigInteger value = ((DerInteger)seq[0]).Value;
			if (value.IntValue != 0)
			{
				throw new ArgumentException("wrong version for RSA private key");
			}
			this.modulus = ((DerInteger)seq[1]).Value;
			this.publicExponent = ((DerInteger)seq[2]).Value;
			this.privateExponent = ((DerInteger)seq[3]).Value;
			this.prime1 = ((DerInteger)seq[4]).Value;
			this.prime2 = ((DerInteger)seq[5]).Value;
			this.exponent1 = ((DerInteger)seq[6]).Value;
			this.exponent2 = ((DerInteger)seq[7]).Value;
			this.coefficient = ((DerInteger)seq[8]).Value;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				new DerInteger(0),
				new DerInteger(this.Modulus),
				new DerInteger(this.PublicExponent),
				new DerInteger(this.PrivateExponent),
				new DerInteger(this.Prime1),
				new DerInteger(this.Prime2),
				new DerInteger(this.Exponent1),
				new DerInteger(this.Exponent2),
				new DerInteger(this.Coefficient)
			});
		}
	}
}
