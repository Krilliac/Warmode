using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class KeyRecRepContent : Asn1Encodable
	{
		private readonly PkiStatusInfo status;

		private readonly CmpCertificate newSigCert;

		private readonly Asn1Sequence caCerts;

		private readonly Asn1Sequence keyPairHist;

		public virtual PkiStatusInfo Status
		{
			get
			{
				return this.status;
			}
		}

		public virtual CmpCertificate NewSigCert
		{
			get
			{
				return this.newSigCert;
			}
		}

		private KeyRecRepContent(Asn1Sequence seq)
		{
			this.status = PkiStatusInfo.GetInstance(seq[0]);
			for (int i = 1; i < seq.Count; i++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
				switch (instance.TagNo)
				{
				case 0:
					this.newSigCert = CmpCertificate.GetInstance(instance.GetObject());
					break;
				case 1:
					this.caCerts = Asn1Sequence.GetInstance(instance.GetObject());
					break;
				case 2:
					this.keyPairHist = Asn1Sequence.GetInstance(instance.GetObject());
					break;
				default:
					throw new ArgumentException("unknown tag number: " + instance.TagNo, "seq");
				}
			}
		}

		public static KeyRecRepContent GetInstance(object obj)
		{
			if (obj is KeyRecRepContent)
			{
				return (KeyRecRepContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new KeyRecRepContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual CmpCertificate[] GetCACerts()
		{
			if (this.caCerts == null)
			{
				return null;
			}
			CmpCertificate[] array = new CmpCertificate[this.caCerts.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CmpCertificate.GetInstance(this.caCerts[num]);
			}
			return array;
		}

		public virtual CertifiedKeyPair[] GetKeyPairHist()
		{
			if (this.keyPairHist == null)
			{
				return null;
			}
			CertifiedKeyPair[] array = new CertifiedKeyPair[this.keyPairHist.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertifiedKeyPair.GetInstance(this.keyPairHist[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.status
			});
			this.AddOptional(v, 0, this.newSigCert);
			this.AddOptional(v, 1, this.caCerts);
			this.AddOptional(v, 2, this.keyPairHist);
			return new DerSequence(v);
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
