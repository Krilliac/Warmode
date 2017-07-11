using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1
{
	public class DerInteger : Asn1Object
	{
		private readonly byte[] bytes;

		public BigInteger Value
		{
			get
			{
				return new BigInteger(this.bytes);
			}
		}

		public BigInteger PositiveValue
		{
			get
			{
				return new BigInteger(1, this.bytes);
			}
		}

		public static DerInteger GetInstance(object obj)
		{
			if (obj == null || obj is DerInteger)
			{
				return (DerInteger)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerInteger GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerInteger)
			{
				return DerInteger.GetInstance(@object);
			}
			return new DerInteger(Asn1OctetString.GetInstance(@object).GetOctets());
		}

		public DerInteger(int value)
		{
			this.bytes = BigInteger.ValueOf((long)value).ToByteArray();
		}

		public DerInteger(BigInteger value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.bytes = value.ToByteArray();
		}

		public DerInteger(byte[] bytes)
		{
			this.bytes = bytes;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(2, this.bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(this.bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerInteger derInteger = asn1Object as DerInteger;
			return derInteger != null && Arrays.AreEqual(this.bytes, derInteger.bytes);
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}
