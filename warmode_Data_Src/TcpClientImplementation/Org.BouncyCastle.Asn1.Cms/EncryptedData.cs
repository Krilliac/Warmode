using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EncryptedData : Asn1Encodable
	{
		private readonly DerInteger version;

		private readonly EncryptedContentInfo encryptedContentInfo;

		private readonly Asn1Set unprotectedAttrs;

		public virtual DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public virtual EncryptedContentInfo EncryptedContentInfo
		{
			get
			{
				return this.encryptedContentInfo;
			}
		}

		public virtual Asn1Set UnprotectedAttrs
		{
			get
			{
				return this.unprotectedAttrs;
			}
		}

		public static EncryptedData GetInstance(object obj)
		{
			if (obj is EncryptedData)
			{
				return (EncryptedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new EncryptedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid EncryptedData: " + obj.GetType().Name);
		}

		public EncryptedData(EncryptedContentInfo encInfo) : this(encInfo, null)
		{
		}

		public EncryptedData(EncryptedContentInfo encInfo, Asn1Set unprotectedAttrs)
		{
			if (encInfo == null)
			{
				throw new ArgumentNullException("encInfo");
			}
			this.version = new DerInteger((unprotectedAttrs == null) ? 0 : 2);
			this.encryptedContentInfo = encInfo;
			this.unprotectedAttrs = unprotectedAttrs;
		}

		private EncryptedData(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count < 2 || seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.version = DerInteger.GetInstance(seq[0]);
			this.encryptedContentInfo = EncryptedContentInfo.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				this.unprotectedAttrs = Asn1Set.GetInstance((Asn1TaggedObject)seq[2], false);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.encryptedContentInfo
			});
			if (this.unprotectedAttrs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new BerTaggedObject(false, 1, this.unprotectedAttrs)
				});
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
