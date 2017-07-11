using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1
{
	public class DerNumericString : DerStringBase
	{
		private readonly string str;

		public static DerNumericString GetInstance(object obj)
		{
			if (obj == null || obj is DerNumericString)
			{
				return (DerNumericString)obj;
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		public static DerNumericString GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is DerNumericString)
			{
				return DerNumericString.GetInstance(@object);
			}
			return new DerNumericString(Asn1OctetString.GetInstance(@object).GetOctets());
		}

		public DerNumericString(byte[] str) : this(Strings.FromAsciiByteArray(str), false)
		{
		}

		public DerNumericString(string str) : this(str, false)
		{
		}

		public DerNumericString(string str, bool validate)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (validate && !DerNumericString.IsNumericString(str))
			{
				throw new ArgumentException("string contains illegal characters", "str");
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
			derOut.WriteEncoded(18, this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerNumericString derNumericString = asn1Object as DerNumericString;
			return derNumericString != null && this.str.Equals(derNumericString.str);
		}

		public static bool IsNumericString(string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (c > '\u007f' || (c != ' ' && !char.IsDigit(c)))
				{
					return false;
				}
			}
			return true;
		}
	}
}
