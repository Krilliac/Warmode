using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class AuthEnvelopedDataParser
	{
		private Asn1SequenceParser seq;

		private DerInteger version;

		private IAsn1Convertible nextObject;

		private bool originatorInfoCalled;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public AuthEnvelopedDataParser(Asn1SequenceParser seq)
		{
			this.seq = seq;
			this.version = (DerInteger)seq.ReadObject();
		}

		public OriginatorInfo GetOriginatorInfo()
		{
			this.originatorInfoCalled = true;
			if (this.nextObject == null)
			{
				this.nextObject = this.seq.ReadObject();
			}
			if (this.nextObject is Asn1TaggedObjectParser && ((Asn1TaggedObjectParser)this.nextObject).TagNo == 0)
			{
				Asn1SequenceParser asn1SequenceParser = (Asn1SequenceParser)((Asn1TaggedObjectParser)this.nextObject).GetObjectParser(16, false);
				this.nextObject = null;
				return OriginatorInfo.GetInstance(asn1SequenceParser.ToAsn1Object());
			}
			return null;
		}

		public Asn1SetParser GetRecipientInfos()
		{
			if (!this.originatorInfoCalled)
			{
				this.GetOriginatorInfo();
			}
			if (this.nextObject == null)
			{
				this.nextObject = this.seq.ReadObject();
			}
			Asn1SetParser result = (Asn1SetParser)this.nextObject;
			this.nextObject = null;
			return result;
		}

		public EncryptedContentInfoParser GetAuthEncryptedContentInfo()
		{
			if (this.nextObject == null)
			{
				this.nextObject = this.seq.ReadObject();
			}
			if (this.nextObject != null)
			{
				Asn1SequenceParser asn1SequenceParser = (Asn1SequenceParser)this.nextObject;
				this.nextObject = null;
				return new EncryptedContentInfoParser(asn1SequenceParser);
			}
			return null;
		}

		public Asn1SetParser GetAuthAttrs()
		{
			if (this.nextObject == null)
			{
				this.nextObject = this.seq.ReadObject();
			}
			if (this.nextObject is Asn1TaggedObjectParser)
			{
				IAsn1Convertible asn1Convertible = this.nextObject;
				this.nextObject = null;
				return (Asn1SetParser)((Asn1TaggedObjectParser)asn1Convertible).GetObjectParser(17, false);
			}
			return null;
		}

		public Asn1OctetString GetMac()
		{
			if (this.nextObject == null)
			{
				this.nextObject = this.seq.ReadObject();
			}
			IAsn1Convertible asn1Convertible = this.nextObject;
			this.nextObject = null;
			return Asn1OctetString.GetInstance(asn1Convertible.ToAsn1Object());
		}

		public Asn1SetParser GetUnauthAttrs()
		{
			if (this.nextObject == null)
			{
				this.nextObject = this.seq.ReadObject();
			}
			if (this.nextObject != null)
			{
				IAsn1Convertible asn1Convertible = this.nextObject;
				this.nextObject = null;
				return (Asn1SetParser)((Asn1TaggedObjectParser)asn1Convertible).GetObjectParser(17, false);
			}
			return null;
		}
	}
}
