using System;

namespace Org.BouncyCastle.Asn1.X500
{
	public class DirectoryString : Asn1Encodable, IAsn1Choice, IAsn1String
	{
		private readonly DerStringBase str;

		public static DirectoryString GetInstance(object obj)
		{
			if (obj is DirectoryString)
			{
				return (DirectoryString)obj;
			}
			if (obj is DerStringBase && (obj is DerT61String || obj is DerPrintableString || obj is DerUniversalString || obj is DerUtf8String || obj is DerBmpString))
			{
				return new DirectoryString((DerStringBase)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static DirectoryString GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			if (!isExplicit)
			{
				throw new ArgumentException("choice item must be explicitly tagged");
			}
			return DirectoryString.GetInstance(obj.GetObject());
		}

		private DirectoryString(DerStringBase str)
		{
			this.str = str;
		}

		public DirectoryString(string str)
		{
			this.str = new DerUtf8String(str);
		}

		public string GetString()
		{
			return this.str.GetString();
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.str.ToAsn1Object();
		}
	}
}
