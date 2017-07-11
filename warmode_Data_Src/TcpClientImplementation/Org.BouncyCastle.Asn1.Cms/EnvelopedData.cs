using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EnvelopedData : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorInfo originatorInfo;

		private Asn1Set recipientInfos;

		private EncryptedContentInfo encryptedContentInfo;

		private Asn1Set unprotectedAttrs;

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

		public EncryptedContentInfo EncryptedContentInfo
		{
			get
			{
				return this.encryptedContentInfo;
			}
		}

		public Asn1Set UnprotectedAttrs
		{
			get
			{
				return this.unprotectedAttrs;
			}
		}

		public EnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo encryptedContentInfo, Asn1Set unprotectedAttrs)
		{
			this.version = new DerInteger(EnvelopedData.CalculateVersion(originatorInfo, recipientInfos, unprotectedAttrs));
			this.originatorInfo = originatorInfo;
			this.recipientInfos = recipientInfos;
			this.encryptedContentInfo = encryptedContentInfo;
			this.unprotectedAttrs = unprotectedAttrs;
		}

		public EnvelopedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, EncryptedContentInfo encryptedContentInfo, Attributes unprotectedAttrs)
		{
			this.version = new DerInteger(EnvelopedData.CalculateVersion(originatorInfo, recipientInfos, Asn1Set.GetInstance(unprotectedAttrs)));
			this.originatorInfo = originatorInfo;
			this.recipientInfos = recipientInfos;
			this.encryptedContentInfo = encryptedContentInfo;
			this.unprotectedAttrs = Asn1Set.GetInstance(unprotectedAttrs);
		}

		[Obsolete("Use 'GetInstance' instead")]
		public EnvelopedData(Asn1Sequence seq)
		{
			int num = 0;
			this.version = (DerInteger)seq[num++];
			object obj = seq[num++];
			if (obj is Asn1TaggedObject)
			{
				this.originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject)obj, false);
				obj = seq[num++];
			}
			this.recipientInfos = Asn1Set.GetInstance(obj);
			this.encryptedContentInfo = EncryptedContentInfo.GetInstance(seq[num++]);
			if (seq.Count > num)
			{
				this.unprotectedAttrs = Asn1Set.GetInstance((Asn1TaggedObject)seq[num], false);
			}
		}

		public static EnvelopedData GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return EnvelopedData.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static EnvelopedData GetInstance(object obj)
		{
			if (obj is EnvelopedData)
			{
				return (EnvelopedData)obj;
			}
			if (obj == null)
			{
				return null;
			}
			return new EnvelopedData(Asn1Sequence.GetInstance(obj));
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
				this.encryptedContentInfo
			});
			if (this.unprotectedAttrs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.unprotectedAttrs)
				});
			}
			return new BerSequence(asn1EncodableVector);
		}

		public static int CalculateVersion(OriginatorInfo originatorInfo, Asn1Set recipientInfos, Asn1Set unprotectedAttrs)
		{
			if (originatorInfo != null || unprotectedAttrs != null)
			{
				return 2;
			}
			foreach (object current in recipientInfos)
			{
				RecipientInfo instance = RecipientInfo.GetInstance(current);
				if (instance.Version.Value.IntValue != 0)
				{
					return 2;
				}
			}
			return 0;
		}
	}
}
