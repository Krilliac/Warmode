using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class MetaData : Asn1Encodable
	{
		private DerBoolean hashProtected;

		private DerUtf8String fileName;

		private DerIA5String mediaType;

		private Attributes otherMetaData;

		public virtual bool IsHashProtected
		{
			get
			{
				return this.hashProtected.IsTrue;
			}
		}

		public virtual DerUtf8String FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public virtual DerIA5String MediaType
		{
			get
			{
				return this.mediaType;
			}
		}

		public virtual Attributes OtherMetaData
		{
			get
			{
				return this.otherMetaData;
			}
		}

		public MetaData(DerBoolean hashProtected, DerUtf8String fileName, DerIA5String mediaType, Attributes otherMetaData)
		{
			this.hashProtected = hashProtected;
			this.fileName = fileName;
			this.mediaType = mediaType;
			this.otherMetaData = otherMetaData;
		}

		private MetaData(Asn1Sequence seq)
		{
			this.hashProtected = DerBoolean.GetInstance(seq[0]);
			int num = 1;
			if (num < seq.Count && seq[num] is DerUtf8String)
			{
				this.fileName = DerUtf8String.GetInstance(seq[num++]);
			}
			if (num < seq.Count && seq[num] is DerIA5String)
			{
				this.mediaType = DerIA5String.GetInstance(seq[num++]);
			}
			if (num < seq.Count)
			{
				this.otherMetaData = Attributes.GetInstance(seq[num++]);
			}
		}

		public static MetaData GetInstance(object obj)
		{
			if (obj is MetaData)
			{
				return (MetaData)obj;
			}
			if (obj != null)
			{
				return new MetaData(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.hashProtected
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.fileName,
				this.mediaType,
				this.otherMetaData
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
