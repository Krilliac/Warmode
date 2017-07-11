using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class OcspRequest : Asn1Encodable
	{
		private readonly TbsRequest tbsRequest;

		private readonly Signature optionalSignature;

		public TbsRequest TbsRequest
		{
			get
			{
				return this.tbsRequest;
			}
		}

		public Signature OptionalSignature
		{
			get
			{
				return this.optionalSignature;
			}
		}

		public static OcspRequest GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return OcspRequest.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static OcspRequest GetInstance(object obj)
		{
			if (obj == null || obj is OcspRequest)
			{
				return (OcspRequest)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OcspRequest((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public OcspRequest(TbsRequest tbsRequest, Signature optionalSignature)
		{
			if (tbsRequest == null)
			{
				throw new ArgumentNullException("tbsRequest");
			}
			this.tbsRequest = tbsRequest;
			this.optionalSignature = optionalSignature;
		}

		private OcspRequest(Asn1Sequence seq)
		{
			this.tbsRequest = TbsRequest.GetInstance(seq[0]);
			if (seq.Count == 2)
			{
				this.optionalSignature = Signature.GetInstance((Asn1TaggedObject)seq[1], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.tbsRequest
			});
			if (this.optionalSignature != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.optionalSignature)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
