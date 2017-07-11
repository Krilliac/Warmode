using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1
{
	public class DerUnknownTag : Asn1Object
	{
		private readonly bool isConstructed;

		private readonly int tag;

		private readonly byte[] data;

		public bool IsConstructed
		{
			get
			{
				return this.isConstructed;
			}
		}

		public int Tag
		{
			get
			{
				return this.tag;
			}
		}

		public DerUnknownTag(int tag, byte[] data) : this(false, tag, data)
		{
		}

		public DerUnknownTag(bool isConstructed, int tag, byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.isConstructed = isConstructed;
			this.tag = tag;
			this.data = data;
		}

		public byte[] GetData()
		{
			return this.data;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(this.isConstructed ? 32 : 0, this.tag, this.data);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerUnknownTag derUnknownTag = asn1Object as DerUnknownTag;
			return derUnknownTag != null && (this.isConstructed == derUnknownTag.isConstructed && this.tag == derUnknownTag.tag) && Arrays.AreEqual(this.data, derUnknownTag.data);
		}

		protected override int Asn1GetHashCode()
		{
			return this.isConstructed.GetHashCode() ^ this.tag.GetHashCode() ^ Arrays.GetHashCode(this.data);
		}
	}
}
