using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class Request : Asn1Encodable
	{
		private readonly CertID reqCert;

		private readonly X509Extensions singleRequestExtensions;

		public CertID ReqCert
		{
			get
			{
				return this.reqCert;
			}
		}

		public X509Extensions SingleRequestExtensions
		{
			get
			{
				return this.singleRequestExtensions;
			}
		}

		public static Request GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return Request.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static Request GetInstance(object obj)
		{
			if (obj == null || obj is Request)
			{
				return (Request)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Request((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public Request(CertID reqCert, X509Extensions singleRequestExtensions)
		{
			if (reqCert == null)
			{
				throw new ArgumentNullException("reqCert");
			}
			this.reqCert = reqCert;
			this.singleRequestExtensions = singleRequestExtensions;
		}

		private Request(Asn1Sequence seq)
		{
			this.reqCert = CertID.GetInstance(seq[0]);
			if (seq.Count == 2)
			{
				this.singleRequestExtensions = X509Extensions.GetInstance((Asn1TaggedObject)seq[1], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.reqCert
			});
			if (this.singleRequestExtensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.singleRequestExtensions)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
