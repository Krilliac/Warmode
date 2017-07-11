using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public abstract class Asn1Encodable : IAsn1Convertible
	{
		public const string Der = "DER";

		public const string Ber = "BER";

		public byte[] GetEncoded()
		{
			MemoryStream memoryStream = new MemoryStream();
			Asn1OutputStream asn1OutputStream = new Asn1OutputStream(memoryStream);
			asn1OutputStream.WriteObject(this);
			return memoryStream.ToArray();
		}

		public byte[] GetEncoded(string encoding)
		{
			if (encoding.Equals("DER"))
			{
				MemoryStream memoryStream = new MemoryStream();
				DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
				derOutputStream.WriteObject(this);
				return memoryStream.ToArray();
			}
			return this.GetEncoded();
		}

		public byte[] GetDerEncoded()
		{
			byte[] result;
			try
			{
				result = this.GetEncoded("DER");
			}
			catch (IOException)
			{
				result = null;
			}
			return result;
		}

		public sealed override int GetHashCode()
		{
			return this.ToAsn1Object().CallAsn1GetHashCode();
		}

		public sealed override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			IAsn1Convertible asn1Convertible = obj as IAsn1Convertible;
			if (asn1Convertible == null)
			{
				return false;
			}
			Asn1Object asn1Object = this.ToAsn1Object();
			Asn1Object asn1Object2 = asn1Convertible.ToAsn1Object();
			return asn1Object == asn1Object2 || asn1Object.CallAsn1Equals(asn1Object2);
		}

		public abstract Asn1Object ToAsn1Object();
	}
}
