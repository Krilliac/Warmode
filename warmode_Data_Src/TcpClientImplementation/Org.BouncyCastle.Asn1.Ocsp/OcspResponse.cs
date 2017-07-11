using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class OcspResponse : Asn1Encodable
	{
		private readonly OcspResponseStatus responseStatus;

		private readonly ResponseBytes responseBytes;

		public OcspResponseStatus ResponseStatus
		{
			get
			{
				return this.responseStatus;
			}
		}

		public ResponseBytes ResponseBytes
		{
			get
			{
				return this.responseBytes;
			}
		}

		public static OcspResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return OcspResponse.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static OcspResponse GetInstance(object obj)
		{
			if (obj == null || obj is OcspResponse)
			{
				return (OcspResponse)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OcspResponse((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public OcspResponse(OcspResponseStatus responseStatus, ResponseBytes responseBytes)
		{
			if (responseStatus == null)
			{
				throw new ArgumentNullException("responseStatus");
			}
			this.responseStatus = responseStatus;
			this.responseBytes = responseBytes;
		}

		private OcspResponse(Asn1Sequence seq)
		{
			this.responseStatus = new OcspResponseStatus(DerEnumerated.GetInstance(seq[0]));
			if (seq.Count == 2)
			{
				this.responseBytes = ResponseBytes.GetInstance((Asn1TaggedObject)seq[1], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.responseStatus
			});
			if (this.responseBytes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.responseBytes)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
