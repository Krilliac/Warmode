using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class AuthEnvelopedData : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorInfo originatorInfo;

		private Asn1Set recipientInfos;

		private EncryptedContentInfo authEncryptedContentInfo;

		private Asn1Set authAttrs;

		private Asn1OctetString mac;

		private Asn1Set unauthAttrs;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public OriginatorInfo OriginatorInfo
		{
			get
			{
				return this.originatorInfo;
			}
		}

		public Asn1Set RecipientInfos
		{
			get
			{
				return this.recipientInfos;
			}
		}

		public EncryptedContentInfo AuthEncryptedContentInfo
		{
			get
			{
				return this.authEncryptedContentInfo;
			}
		}

		public Asn1Set AuthAttrs
		{
			get
			{
				return this.authAttrs;
			}
		}

		public Asn1OctetString Mac
		{
			get
			{
				return this.mac;
			}
		}

		public Asn1Set UnauthAttrs
		{
			get
			{
				return this.unauthAttrs;
			}
		}

		public AuthEnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo authEncryptedContentInfo, Asn1Set authAttrs, Asn1OctetString mac, Asn1Set unauthAttrs)
		{
			this.version = new DerInteger(0);
			this.originatorInfo = originatorInfo;
			this.recipientInfos = recipientInfos;
			this.authEncryptedContentInfo = authEncryptedContentInfo;
			this.authAttrs = authAttrs;
			this.mac = mac;
			this.unauthAttrs = unauthAttrs;
		}

		private AuthEnvelopedData(Asn1Sequence seq)
		{
			int num = 0;
			Asn1Object asn1Object = seq[num++].ToAsn1Object();
			this.version = (DerInteger)asn1Object;
			asn1Object = seq[num++].ToAsn1Object();
			if (asn1Object is Asn1TaggedObject)
			{
				this.originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject)asn1Object, false);
				asn1Object = seq[num++].ToAsn1Object();
			}
			this.recipientInfos = Asn1Set.GetInstance(asn1Object);
			asn1Object = seq[num++].ToAsn1Object();
			this.authEncryptedContentInfo = EncryptedContentInfo.GetInstance(asn1Object);
			asn1Object = seq[num++].ToAsn1Object();
			if (asn1Object is Asn1TaggedObject)
			{
				this.authAttrs = Asn1Set.GetInstance((Asn1TaggedObject)asn1Object, false);
				asn1Object = seq[num++].ToAsn1Object();
			}
			this.mac = Asn1OctetString.GetInstance(asn1Object);
			if (seq.Count > num)
			{
				asn1Object = seq[num++].ToAsn1Object();
				this.unauthAttrs = Asn1Set.GetInstance((Asn1TaggedObject)asn1Object, false);
			}
		}

		public static AuthEnvelopedData GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return AuthEnvelopedData.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static AuthEnvelopedData GetInstance(object obj)
		{
			if (obj == null || obj is AuthEnvelopedData)
			{
				return (AuthEnvelopedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AuthEnvelopedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid AuthEnvelopedData: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version
			});
			if (this.originatorInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.originatorInfo)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.recipientInfos,
				this.authEncryptedContentInfo
			});
			if (this.authAttrs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.authAttrs)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.mac
			});
			if (this.unauthAttrs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 2, this.unauthAttrs)
				});
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
