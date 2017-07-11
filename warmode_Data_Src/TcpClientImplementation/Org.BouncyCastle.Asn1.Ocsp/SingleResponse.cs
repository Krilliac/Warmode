using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class SingleResponse : Asn1Encodable
	{
		private readonly CertID certID;

		private readonly CertStatus certStatus;

		private readonly DerGeneralizedTime thisUpdate;

		private readonly DerGeneralizedTime nextUpdate;

		private readonly X509Extensions singleExtensions;

		public CertID CertId
		{
			get
			{
				return this.certID;
			}
		}

		public CertStatus CertStatus
		{
			get
			{
				return this.certStatus;
			}
		}

		public DerGeneralizedTime ThisUpdate
		{
			get
			{
				return this.thisUpdate;
			}
		}

		public DerGeneralizedTime NextUpdate
		{
			get
			{
				return this.nextUpdate;
			}
		}

		public X509Extensions SingleExtensions
		{
			get
			{
				return this.singleExtensions;
			}
		}

		public SingleResponse(CertID certID, CertStatus certStatus, DerGeneralizedTime thisUpdate, DerGeneralizedTime nextUpdate, X509Extensions singleExtensions)
		{
			this.certID = certID;
			this.certStatus = certStatus;
			this.thisUpdate = thisUpdate;
			this.nextUpdate = nextUpdate;
			this.singleExtensions = singleExtensions;
		}

		public SingleResponse(Asn1Sequence seq)
		{
			this.certID = CertID.GetInstance(seq[0]);
			this.certStatus = CertStatus.GetInstance(seq[1]);
			this.thisUpdate = (DerGeneralizedTime)seq[2];
			if (seq.Count > 4)
			{
				this.nextUpdate = DerGeneralizedTime.GetInstance((Asn1TaggedObject)seq[3], true);
				this.singleExtensions = X509Extensions.GetInstance((Asn1TaggedObject)seq[4], true);
				return;
			}
			if (seq.Count > 3)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[3];
				if (asn1TaggedObject.TagNo == 0)
				{
					this.nextUpdate = DerGeneralizedTime.GetInstance(asn1TaggedObject, true);
					return;
				}
				this.singleExtensions = X509Extensions.GetInstance(asn1TaggedObject, true);
			}
		}

		public static SingleResponse GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return SingleResponse.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static SingleResponse GetInstance(object obj)
		{
			if (obj == null || obj is SingleResponse)
			{
				return (SingleResponse)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SingleResponse((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certID,
				this.certStatus,
				this.thisUpdate
			});
			if (this.nextUpdate != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.nextUpdate)
				});
			}
			if (this.singleExtensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.singleExtensions)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
