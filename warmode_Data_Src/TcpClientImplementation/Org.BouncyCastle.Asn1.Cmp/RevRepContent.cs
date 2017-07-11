using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class RevRepContent : Asn1Encodable
	{
		private readonly Asn1Sequence status;

		private readonly Asn1Sequence revCerts;

		private readonly Asn1Sequence crls;

		private RevRepContent(Asn1Sequence seq)
		{
			this.status = Asn1Sequence.GetInstance(seq[0]);
			for (int i = 1; i < seq.Count; i++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
				if (instance.TagNo == 0)
				{
					this.revCerts = Asn1Sequence.GetInstance(instance, true);
				}
				else
				{
					this.crls = Asn1Sequence.GetInstance(instance, true);
				}
			}
		}

		public static RevRepContent GetInstance(object obj)
		{
			if (obj is RevRepContent)
			{
				return (RevRepContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RevRepContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual PkiStatusInfo[] GetStatus()
		{
			PkiStatusInfo[] array = new PkiStatusInfo[this.status.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = PkiStatusInfo.GetInstance(this.status[num]);
			}
			return array;
		}

		public virtual CertId[] GetRevCerts()
		{
			if (this.revCerts == null)
			{
				return null;
			}
			CertId[] array = new CertId[this.revCerts.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertId.GetInstance(this.revCerts[num]);
			}
			return array;
		}

		public virtual CertificateList[] GetCrls()
		{
			if (this.crls == null)
			{
				return null;
			}
			CertificateList[] array = new CertificateList[this.crls.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertificateList.GetInstance(this.crls[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector v = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.status
			});
			this.AddOptional(v, 0, this.revCerts);
			this.AddOptional(v, 1, this.crls);
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
