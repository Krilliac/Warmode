using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class TimeStampedDataParser
	{
		private DerInteger version;

		private DerIA5String dataUri;

		private MetaData metaData;

		private Asn1OctetStringParser content;

		private Evidence temporalEvidence;

		private Asn1SequenceParser parser;

		public virtual DerIA5String DataUri
		{
			get
			{
				return this.dataUri;
			}
		}

		public virtual MetaData MetaData
		{
			get
			{
				return this.metaData;
			}
		}

		public virtual Asn1OctetStringParser Content
		{
			get
			{
				return this.content;
			}
		}

		private TimeStampedDataParser(Asn1SequenceParser parser)
		{
			this.parser = parser;
			this.version = DerInteger.GetInstance(parser.ReadObject());
			Asn1Object asn1Object = parser.ReadObject().ToAsn1Object();
			if (asn1Object is DerIA5String)
			{
				this.dataUri = DerIA5String.GetInstance(asn1Object);
				asn1Object = parser.ReadObject().ToAsn1Object();
			}
			if (asn1Object is Asn1SequenceParser)
			{
				this.metaData = MetaData.GetInstance(asn1Object.ToAsn1Object());
				asn1Object = parser.ReadObject().ToAsn1Object();
			}
			if (asn1Object is Asn1OctetStringParser)
			{
				this.content = (Asn1OctetStringParser)asn1Object;
			}
		}

		public static TimeStampedDataParser GetInstance(object obj)
		{
			if (obj is Asn1Sequence)
			{
				return new TimeStampedDataParser(((Asn1Sequence)obj).Parser);
			}
			if (obj is Asn1SequenceParser)
			{
				return new TimeStampedDataParser((Asn1SequenceParser)obj);
			}
			return null;
		}

		public virtual Evidence GetTemporalEvidence()
		{
			if (this.temporalEvidence == null)
			{
				this.temporalEvidence = Evidence.GetInstance(this.parser.ReadObject().ToAsn1Object());
			}
			return this.temporalEvidence;
		}
	}
}
