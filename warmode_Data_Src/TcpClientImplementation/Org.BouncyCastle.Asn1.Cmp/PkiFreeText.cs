using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiFreeText : Asn1Encodable
	{
		internal Asn1Sequence strings;

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return this.strings.Count;
			}
		}

		public int Count
		{
			get
			{
				return this.strings.Count;
			}
		}

		public DerUtf8String this[int index]
		{
			get
			{
				return (DerUtf8String)this.strings[index];
			}
		}

		public static PkiFreeText GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return PkiFreeText.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static PkiFreeText GetInstance(object obj)
		{
			if (obj is PkiFreeText)
			{
				return (PkiFreeText)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PkiFreeText((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public PkiFreeText(Asn1Sequence seq)
		{
			foreach (object current in seq)
			{
				if (!(current is DerUtf8String))
				{
					throw new ArgumentException("attempt to insert non UTF8 STRING into PkiFreeText");
				}
			}
			this.strings = seq;
		}

		public PkiFreeText(DerUtf8String p)
		{
			this.strings = new DerSequence(p);
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public DerUtf8String GetStringAt(int index)
		{
			return this[index];
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.strings;
		}
	}
}
