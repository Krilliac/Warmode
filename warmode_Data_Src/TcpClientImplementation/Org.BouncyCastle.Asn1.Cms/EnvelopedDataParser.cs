using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EnvelopedDataParser
	{
		private Asn1SequenceParser _seq;

		private DerInteger _version;

		private IAsn1Convertible _nextObject;

		private bool _originatorInfoCalled;

		public DerInteger Version
		{
			get
			{
				return this._version;
			}
		}

		public EnvelopedDataParser(Asn1SequenceParser seq)
		{
			this._seq = seq;
			this._version = (DerInteger)seq.ReadObject();
		}

		public OriginatorInfo GetOriginatorInfo()
		{
			this._originatorInfoCalled = true;
			if (this._nextObject == null)
			{
				this._nextObject = this._seq.ReadObject();
			}
			if (this._nextObject is Asn1TaggedObjectParser && ((Asn1TaggedObjectParser)this._nextObject).TagNo == 0)
			{
				Asn1SequenceParser asn1SequenceParser = (Asn1SequenceParser)((Asn1TaggedObjectParser)this._nextObject).GetObjectParser(16, false);
				this._nextObject = null;
				return OriginatorInfo.GetInstance(asn1SequenceParser.ToAsn1Object());
			}
			return null;
		}

		public Asn1SetParser GetRecipientInfos()
		{
			if (!this._originatorInfoCalled)
			{
				this.GetOriginatorInfo();
			}
			if (this._nextObject == null)
			{
				this._nextObject = this._seq.ReadObject();
			}
			Asn1SetParser result = (Asn1SetParser)this._nextObject;
			this._nextObject = null;
			return result;
		}

		public EncryptedContentInfoParser GetEncryptedContentInfo()
		{
			if (this._nextObject == null)
			{
				this._nextObject = this._seq.ReadObject();
			}
			if (this._nextObject != null)
			{
				Asn1SequenceParser seq = (Asn1SequenceParser)this._nextObject;
				this._nextObject = null;
				return new EncryptedContentInfoParser(seq);
			}
			return null;
		}

		public Asn1SetParser GetUnprotectedAttrs()
		{
			if (this._nextObject == null)
			{
				this._nextObject = this._seq.ReadObject();
			}
			if (this._nextObject != null)
			{
				IAsn1Convertible nextObject = this._nextObject;
				this._nextObject = null;
				return (Asn1SetParser)((Asn1TaggedObjectParser)nextObject).GetObjectParser(17, false);
			}
			return null;
		}
	}
}
