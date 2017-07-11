using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public abstract class Asn1OctetString : Asn1Object, Asn1OctetStringParser, IAsn1Convertible
	{
		internal byte[] str;

		public Asn1OctetStringParser Parser
		{
			get
			{
				return this;
			}
		}

		public static Asn1OctetString GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			Asn1Object @object = obj.GetObject();
			if (isExplicit || @object is Asn1OctetString)
			{
				return Asn1OctetString.GetInstance(@object);
			}
			return BerOctetString.FromSequence(Asn1Sequence.GetInstance(@object));
		}

		public static Asn1OctetString GetInstance(object obj)
		{
			if (obj == null || obj is Asn1OctetString)
			{
				return (Asn1OctetString)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return Asn1OctetString.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
		}

		internal Asn1OctetString(byte[] str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			this.str = str;
		}

		internal Asn1OctetString(Asn1Encodable obj)
		{
			try
			{
				this.str = obj.GetEncoded("DER");
			}
			catch (IOException ex)
			{
				throw new ArgumentException("Error processing object : " + ex.ToString());
			}
		}

		public Stream GetOctetStream()
		{
			return new MemoryStream(this.str, false);
		}

		public virtual byte[] GetOctets()
		{
			return this.str;
		}

		protected override int Asn1GetHashCode()
		{
			return Arrays.GetHashCode(this.GetOctets());
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerOctetString derOctetString = asn1Object as DerOctetString;
			return derOctetString != null && Arrays.AreEqual(this.GetOctets(), derOctetString.GetOctets());
		}

		public override string ToString()
		{
			return "#" + Hex.ToHexString(this.str);
		}
	}
}
