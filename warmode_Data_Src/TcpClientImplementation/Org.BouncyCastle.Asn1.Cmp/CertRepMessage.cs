using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class CertRepMessage : Asn1Encodable
	{
		private readonly Asn1Sequence caPubs;

		private readonly Asn1Sequence response;

		private CertRepMessage(Asn1Sequence seq)
		{
			int index = 0;
			if (seq.Count > 1)
			{
				this.caPubs = Asn1Sequence.GetInstance((Asn1TaggedObject)seq[index++], true);
			}
			this.response = Asn1Sequence.GetInstance(seq[index]);
		}

		public static CertRepMessage GetInstance(object obj)
		{
			if (obj is CertRepMessage)
			{
				return (CertRepMessage)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertRepMessage((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public CertRepMessage(CmpCertificate[] caPubs, CertResponse[] response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (caPubs != null)
			{
				this.caPubs = new DerSequence(caPubs);
			}
			this.response = new DerSequence(response);
		}

		public virtual CmpCertificate[] GetCAPubs()
		{
			if (this.caPubs == null)
			{
				return null;
			}
			CmpCertificate[] array = new CmpCertificate[this.caPubs.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CmpCertificate.GetInstance(this.caPubs[num]);
			}
			return array;
		}

		public virtual CertResponse[] GetResponse()
		{
			CertResponse[] array = new CertResponse[this.response.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = CertResponse.GetInstance(this.response[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.caPubs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.caPubs)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.response
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
