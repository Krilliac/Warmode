using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiHeader : Asn1Encodable
	{
		public static readonly GeneralName NULL_NAME = new GeneralName(X509Name.GetInstance(new DerSequence()));

		public static readonly int CMP_1999 = 1;

		public static readonly int CMP_2000 = 2;

		private readonly DerInteger pvno;

		private readonly GeneralName sender;

		private readonly GeneralName recipient;

		private readonly DerGeneralizedTime messageTime;

		private readonly AlgorithmIdentifier protectionAlg;

		private readonly Asn1OctetString senderKID;

		private readonly Asn1OctetString recipKID;

		private readonly Asn1OctetString transactionID;

		private readonly Asn1OctetString senderNonce;

		private readonly Asn1OctetString recipNonce;

		private readonly PkiFreeText freeText;

		private readonly Asn1Sequence generalInfo;

		public virtual DerInteger Pvno
		{
			get
			{
				return this.pvno;
			}
		}

		public virtual GeneralName Sender
		{
			get
			{
				return this.sender;
			}
		}

		public virtual GeneralName Recipient
		{
			get
			{
				return this.recipient;
			}
		}

		public virtual DerGeneralizedTime MessageTime
		{
			get
			{
				return this.messageTime;
			}
		}

		public virtual AlgorithmIdentifier ProtectionAlg
		{
			get
			{
				return this.protectionAlg;
			}
		}

		public virtual Asn1OctetString SenderKID
		{
			get
			{
				return this.senderKID;
			}
		}

		public virtual Asn1OctetString RecipKID
		{
			get
			{
				return this.recipKID;
			}
		}

		public virtual Asn1OctetString TransactionID
		{
			get
			{
				return this.transactionID;
			}
		}

		public virtual Asn1OctetString SenderNonce
		{
			get
			{
				return this.senderNonce;
			}
		}

		public virtual Asn1OctetString RecipNonce
		{
			get
			{
				return this.recipNonce;
			}
		}

		public virtual PkiFreeText FreeText
		{
			get
			{
				return this.freeText;
			}
		}

		private PkiHeader(Asn1Sequence seq)
		{
			this.pvno = DerInteger.GetInstance(seq[0]);
			this.sender = GeneralName.GetInstance(seq[1]);
			this.recipient = GeneralName.GetInstance(seq[2]);
			for (int i = 3; i < seq.Count; i++)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[i];
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.messageTime = DerGeneralizedTime.GetInstance(asn1TaggedObject, true);
					break;
				case 1:
					this.protectionAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
					break;
				case 2:
					this.senderKID = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 3:
					this.recipKID = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 4:
					this.transactionID = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 5:
					this.senderNonce = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 6:
					this.recipNonce = Asn1OctetString.GetInstance(asn1TaggedObject, true);
					break;
				case 7:
					this.freeText = PkiFreeText.GetInstance(asn1TaggedObject, true);
					break;
				case 8:
					this.generalInfo = Asn1Sequence.GetInstance(asn1TaggedObject, true);
					break;
				default:
					throw new ArgumentException("unknown tag number: " + asn1TaggedObject.TagNo, "seq");
				}
			}
		}

		public static PkiHeader GetInstance(object obj)
		{
			if (obj is PkiHeader)
			{
				return (PkiHeader)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PkiHeader((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public PkiHeader(int pvno, GeneralName sender, GeneralName recipient) : this(new DerInteger(pvno), sender, recipient)
		{
		}

		private PkiHeader(DerInteger pvno, GeneralName sender, GeneralName recipient)
		{
			this.pvno = pvno;
			this.sender = sender;
			this.recipient = recipient;
		}

		public virtual InfoTypeAndValue[] GetGeneralInfo()
		{
			if (this.generalInfo == null)
			{
				return null;
			}
			InfoTypeAndValue[] array = new InfoTypeAndValue[this.generalInfo.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = InfoTypeAndValue.GetInstance(this.generalInfo[i]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.pvno,
				this.sender,
				this.recipient
			});
			PkiHeader.AddOptional(v, 0, this.messageTime);
			PkiHeader.AddOptional(v, 1, this.protectionAlg);
			PkiHeader.AddOptional(v, 2, this.senderKID);
			PkiHeader.AddOptional(v, 3, this.recipKID);
			PkiHeader.AddOptional(v, 4, this.transactionID);
			PkiHeader.AddOptional(v, 5, this.senderNonce);
			PkiHeader.AddOptional(v, 6, this.recipNonce);
			PkiHeader.AddOptional(v, 7, this.freeText);
			PkiHeader.AddOptional(v, 8, this.generalInfo);
			return new DerSequence(v);
		}

		private static void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
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
