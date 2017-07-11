using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiHeaderBuilder
	{
		private DerInteger pvno;

		private GeneralName sender;

		private GeneralName recipient;

		private DerGeneralizedTime messageTime;

		private AlgorithmIdentifier protectionAlg;

		private Asn1OctetString senderKID;

		private Asn1OctetString recipKID;

		private Asn1OctetString transactionID;

		private Asn1OctetString senderNonce;

		private Asn1OctetString recipNonce;

		private PkiFreeText freeText;

		private Asn1Sequence generalInfo;

		public PkiHeaderBuilder(int pvno, GeneralName sender, GeneralName recipient) : this(new DerInteger(pvno), sender, recipient)
		{
		}

		private PkiHeaderBuilder(DerInteger pvno, GeneralName sender, GeneralName recipient)
		{
			this.pvno = pvno;
			this.sender = sender;
			this.recipient = recipient;
		}

		public virtual PkiHeaderBuilder SetMessageTime(DerGeneralizedTime time)
		{
			this.messageTime = time;
			return this;
		}

		public virtual PkiHeaderBuilder SetProtectionAlg(AlgorithmIdentifier aid)
		{
			this.protectionAlg = aid;
			return this;
		}

		public virtual PkiHeaderBuilder SetSenderKID(byte[] kid)
		{
			return this.SetSenderKID((kid == null) ? null : new DerOctetString(kid));
		}

		public virtual PkiHeaderBuilder SetSenderKID(Asn1OctetString kid)
		{
			this.senderKID = kid;
			return this;
		}

		public virtual PkiHeaderBuilder SetRecipKID(byte[] kid)
		{
			return this.SetRecipKID((kid == null) ? null : new DerOctetString(kid));
		}

		public virtual PkiHeaderBuilder SetRecipKID(DerOctetString kid)
		{
			this.recipKID = kid;
			return this;
		}

		public virtual PkiHeaderBuilder SetTransactionID(byte[] tid)
		{
			return this.SetTransactionID((tid == null) ? null : new DerOctetString(tid));
		}

		public virtual PkiHeaderBuilder SetTransactionID(Asn1OctetString tid)
		{
			this.transactionID = tid;
			return this;
		}

		public virtual PkiHeaderBuilder SetSenderNonce(byte[] nonce)
		{
			return this.SetSenderNonce((nonce == null) ? null : new DerOctetString(nonce));
		}

		public virtual PkiHeaderBuilder SetSenderNonce(Asn1OctetString nonce)
		{
			this.senderNonce = nonce;
			return this;
		}

		public virtual PkiHeaderBuilder SetRecipNonce(byte[] nonce)
		{
			return this.SetRecipNonce((nonce == null) ? null : new DerOctetString(nonce));
		}

		public virtual PkiHeaderBuilder SetRecipNonce(Asn1OctetString nonce)
		{
			this.recipNonce = nonce;
			return this;
		}

		public virtual PkiHeaderBuilder SetFreeText(PkiFreeText text)
		{
			this.freeText = text;
			return this;
		}

		public virtual PkiHeaderBuilder SetGeneralInfo(InfoTypeAndValue genInfo)
		{
			return this.SetGeneralInfo(PkiHeaderBuilder.MakeGeneralInfoSeq(genInfo));
		}

		public virtual PkiHeaderBuilder SetGeneralInfo(InfoTypeAndValue[] genInfos)
		{
			return this.SetGeneralInfo(PkiHeaderBuilder.MakeGeneralInfoSeq(genInfos));
		}

		public virtual PkiHeaderBuilder SetGeneralInfo(Asn1Sequence seqOfInfoTypeAndValue)
		{
			this.generalInfo = seqOfInfoTypeAndValue;
			return this;
		}

		private static Asn1Sequence MakeGeneralInfoSeq(InfoTypeAndValue generalInfo)
		{
			return new DerSequence(generalInfo);
		}

		private static Asn1Sequence MakeGeneralInfoSeq(InfoTypeAndValue[] generalInfos)
		{
			Asn1Sequence result = null;
			if (generalInfos != null)
			{
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
				for (int i = 0; i < generalInfos.Length; i++)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						generalInfos[i]
					});
				}
				result = new DerSequence(asn1EncodableVector);
			}
			return result;
		}

		public virtual PkiHeader Build()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.pvno,
				this.sender,
				this.recipient
			});
			this.AddOptional(v, 0, this.messageTime);
			this.AddOptional(v, 1, this.protectionAlg);
			this.AddOptional(v, 2, this.senderKID);
			this.AddOptional(v, 3, this.recipKID);
			this.AddOptional(v, 4, this.transactionID);
			this.AddOptional(v, 5, this.senderNonce);
			this.AddOptional(v, 6, this.recipNonce);
			this.AddOptional(v, 7, this.freeText);
			this.AddOptional(v, 8, this.generalInfo);
			this.messageTime = null;
			this.protectionAlg = null;
			this.senderKID = null;
			this.recipKID = null;
			this.transactionID = null;
			this.senderNonce = null;
			this.recipNonce = null;
			this.freeText = null;
			this.generalInfo = null;
			return PkiHeader.GetInstance(new DerSequence(v));
		}

		private void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
		{
			if (obj != null)
			{
				v.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, tagNo, obj)
				});
			}
		}
	}
}
