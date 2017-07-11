using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1
{
	public class DerEnumerated : Asn1Object
	{
		private readonly byte[] bytes;

		private static readonly DerEnumerated[] cache = new DerEnumerated[12];

		public BigInteger Value
		{
			get
			{
				return new BigInteger(this.bytes);
			}
		}

		public static DerEnumerated GetInstance(object obj)
		{
			if (obj == null || obj is DerEnumerated)
			{
				return (DerEnumerated)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerEnumerated GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerEnumerated)
			{
				return DerEnumerated.GetInstance(@object);
			}
			return DerEnumerated.FromOctetString(((Asn1OctetString)@object).GetOctets());
		}

		public DerEnumerated(int val)
		{
			this.bytes = BigInteger.ValueOf((long)val).ToByteArray();
		}

		public DerEnumerated(BigInteger val)
		{
			this.bytes = val.ToByteArray();
		}

		public DerEnumerated(byte[] bytes)
		{
			this.bytes = bytes;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(10, this.bytes);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerEnumerated derEnumerated = asn1Object as DerEnumerated;
			return derEnumerated != null && Arrays.AreEqual(this.bytes, derEnumerated.bytes);
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(this.bytes);
		}

		internal static DerEnumerated FromOctetString(byte[] enc)
		{
			if (enc.Length == 0)
			{
				throw new ArgumentException("ENUMERATED has zero length", "enc");
			}
			if (enc.Length == 1)
			{
				int num = (int)enc[0];
				if (num < DerEnumerated.cache.Length)
				{
					DerEnumerated derEnumerated = DerEnumerated.cache[num];
					if (derEnumerated != null)
					{
						return derEnumerated;
					}
					return DerEnumerated.cache[num] = new DerEnumerated(Arrays.Clone(enc));
				}
			}
			return new DerEnumerated(Arrays.Clone(enc));
		}
	}
}
