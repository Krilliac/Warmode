using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class OtherRevocationInfoFormat : Asn1Encodable
	{
		private readonly DerObjectIdentifier otherRevInfoFormat;

		private readonly Asn1Encodable otherRevInfo;

		public virtual DerObjectIdentifier InfoFormat
		{
			get
			{
				return this.otherRevInfoFormat;
			}
		}

		public virtual Asn1Encodable Info
		{
			get
			{
				return this.otherRevInfo;
			}
		}

		public OtherRevocationInfoFormat(DerObjectIdentifier otherRevInfoFormat, Asn1Encodable otherRevInfo)
		{
			this.otherRevInfoFormat = otherRevInfoFormat;
			this.otherRevInfo = otherRevInfo;
		}

		private OtherRevocationInfoFormat(Asn1Sequence seq)
		{
			this.otherRevInfoFormat = DerObjectIdentifier.GetInstance(seq[0]);
			this.otherRevInfo = seq[1];
		}

		public static OtherRevocationInfoFormat GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return OtherRevocationInfoFormat.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static OtherRevocationInfoFormat GetInstance(object obj)
		{
			if (obj is OtherRevocationInfoFormat)
			{
				return (OtherRevocationInfoFormat)obj;
			}
			if (obj != null)
			{
				return new OtherRevocationInfoFormat(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.otherRevInfoFormat,
				this.otherRevInfo
			});
		}
	}
}
