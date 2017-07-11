using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiMessage : Asn1Encodable
	{
		private readonly PkiHeader header;

		private readonly PkiBody body;

		private readonly DerBitString protection;

		private readonly Asn1Sequence extraCerts;

		public virtual PkiHeader Header
		{
			get
			{
				return this.header;
			}
		}

		public virtual PkiBody Body
		{
			get
			{
				return this.body;
			}
		}

		public virtual DerBitString Protection
		{
			get
			{
				return this.protection;
			}
		}

		private PkiMessage(Asn1Sequence seq)
		{
			this.header = PkiHeader.GetInstance(seq[0]);
			this.body = PkiBody.GetInstance(seq[1]);
			for (int i = 2; i < seq.Count; i++)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[i].ToAsn1Object();
				if (asn1TaggedObject.TagNo == 0)
				{
					this.protection = DerBitString.GetInstance(asn1TaggedObject, true);
				}
				else
				{
					this.extraCerts = Asn1Sequence.GetInstance(asn1TaggedObject, true);
				}
			}
		}

		public static PkiMessage GetInstance(object obj)
		{
			if (obj is PkiMessage)
			{
				return (PkiMessage)obj;
			}
			if (obj != null)
			{
				return new PkiMessage(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public PkiMessage(PkiHeader header, PkiBody body, DerBitString protection, CmpCertificate[] extraCerts)
		{
			this.header = header;
			this.body = body;
			this.protection = protection;
			if (extraCerts != null)
			{
				this.extraCerts = new DerSequence(extraCerts);
			}
		}

		public PkiMessage(PkiHeader header, PkiBody body, DerBitString protection) : this(header, body, protection, null)
		{
		}

		public PkiMessage(PkiHeader header, PkiBody body) : this(header, body, null, null)
		{
		}

		public virtual CmpCertificate[] GetExtraCerts()
		{
			if (this.extraCerts == null)
			{
				return null;
			}
			CmpCertificate[] array = new CmpCertificate[this.extraCerts.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = CmpCertificate.GetInstance(this.extraCerts[i]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.header,
				this.body
			});
			PkiMessage.AddOptional(v, 0, this.protection);
			PkiMessage.AddOptional(v, 1, this.extraCerts);
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
