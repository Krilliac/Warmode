using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class ResponseData : Asn1Encodable
	{
		private static readonly DerInteger V1 = new DerInteger(0);

		private readonly bool versionPresent;

		private readonly DerInteger version;

		private readonly ResponderID responderID;

		private readonly DerGeneralizedTime producedAt;

		private readonly Asn1Sequence responses;

		private readonly X509Extensions responseExtensions;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public ResponderID ResponderID
		{
			get
			{
				return this.responderID;
			}
		}

		public DerGeneralizedTime ProducedAt
		{
			get
			{
				return this.producedAt;
			}
		}

		public Asn1Sequence Responses
		{
			get
			{
				return this.responses;
			}
		}

		public X509Extensions ResponseExtensions
		{
			get
			{
				return this.responseExtensions;
			}
		}

		public static ResponseData GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return ResponseData.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static ResponseData GetInstance(object obj)
		{
			if (obj == null || obj is ResponseData)
			{
				return (ResponseData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ResponseData((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public ResponseData(DerInteger version, ResponderID responderID, DerGeneralizedTime producedAt, Asn1Sequence responses, X509Extensions responseExtensions)
		{
			this.version = version;
			this.responderID = responderID;
			this.producedAt = producedAt;
			this.responses = responses;
			this.responseExtensions = responseExtensions;
		}

		public ResponseData(ResponderID responderID, DerGeneralizedTime producedAt, Asn1Sequence responses, X509Extensions responseExtensions) : this(ResponseData.V1, responderID, producedAt, responses, responseExtensions)
		{
		}

		private ResponseData(Asn1Sequence seq)
		{
			int num = 0;
			Asn1Encodable asn1Encodable = seq[0];
			if (asn1Encodable is Asn1TaggedObject)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Encodable;
				if (asn1TaggedObject.TagNo == 0)
				{
					this.versionPresent = true;
					this.version = DerInteger.GetInstance(asn1TaggedObject, true);
					num++;
				}
				else
				{
					this.version = ResponseData.V1;
				}
			}
			else
			{
				this.version = ResponseData.V1;
			}
			this.responderID = ResponderID.GetInstance(seq[num++]);
			this.producedAt = (DerGeneralizedTime)seq[num++];
			this.responses = (Asn1Sequence)seq[num++];
			if (seq.Count > num)
			{
				this.responseExtensions = X509Extensions.GetInstance((Asn1TaggedObject)seq[num], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.versionPresent || !this.version.Equals(ResponseData.V1))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.version)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.responderID,
				this.producedAt,
				this.responses
			});
			if (this.responseExtensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.responseExtensions)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
