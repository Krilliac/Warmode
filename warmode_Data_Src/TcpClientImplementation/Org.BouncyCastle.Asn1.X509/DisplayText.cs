using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class DisplayText : Asn1Encodable, IAsn1Choice
	{
		public const int ContentTypeIA5String = 0;

		public const int ContentTypeBmpString = 1;

		public const int ContentTypeUtf8String = 2;

		public const int ContentTypeVisibleString = 3;

		public const int DisplayTextMaximumSize = 200;

		internal readonly int contentType;

		internal readonly IAsn1String contents;

		public DisplayText(int type, string text)
		{
			if (text.Length > 200)
			{
				text = text.Substring(0, 200);
			}
			this.contentType = type;
			switch (type)
			{
			case 0:
				this.contents = new DerIA5String(text);
				return;
			case 1:
				this.contents = new DerBmpString(text);
				return;
			case 2:
				this.contents = new DerUtf8String(text);
				return;
			case 3:
				this.contents = new DerVisibleString(text);
				return;
			default:
				this.contents = new DerUtf8String(text);
				return;
			}
		}

		public DisplayText(string text)
		{
			if (text.Length > 200)
			{
				text = text.Substring(0, 200);
			}
			this.contentType = 2;
			this.contents = new DerUtf8String(text);
		}

		public DisplayText(IAsn1String contents)
		{
			this.contents = contents;
		}

		public static DisplayText GetInstance(object obj)
		{
			if (obj is IAsn1String)
			{
				return new DisplayText((IAsn1String)obj);
			}
			if (obj is DisplayText)
			{
				return (DisplayText)obj;
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			return (Asn1Object)this.contents;
		}

		public string GetString()
		{
			return this.contents.GetString();
		}
	}
}
