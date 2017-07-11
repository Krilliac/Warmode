using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1
{
	public class DerVisibleString : DerStringBase
	{
		private readonly string str;

		public static DerVisibleString GetInstance(object obj)
		{
			if (obj == null || obj is DerVisibleString)
			{
				return (DerVisibleString)obj;
			}
			if (obj is Asn1OctetString)
			{
				return new DerVisibleString(((Asn1OctetString)obj).GetOctets());
			}
			if (obj is Asn1TaggedObject)
			{
				return DerVisibleString.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerVisibleString GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerVisibleString.GetInstance(obj.GetObject());
		}

		public DerVisibleString(byte[] str) : this(Strings.FromAsciiByteArray(str))
		{
		}

		public DerVisibleString(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		public override string GetString()
		{
			return this.str;
		}

		public byte[] GetOctets()
		{
			return Strings.ToAsciiByteArray(this.str);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(26, this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerVisibleString derVisibleString = asn1Object as DerVisibleString;
			return derVisibleString != null && this.str.Equals(derVisibleString.str);
		}

		protected override int Asn1GetHashCode()
		{
			return this.str.GetHashCode();
		}
	}
}
