using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class TbsRequest : Asn1Encodable
	{
		private static readonly DerInteger V1 = new DerInteger(0);

		private readonly DerInteger version;

		private readonly GeneralName requestorName;

		private readonly Asn1Sequence requestList;

		private readonly X509Extensions requestExtensions;

		private bool versionSet;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public GeneralName RequestorName
		{
			get
			{
				return this.requestorName;
			}
		}

		public Asn1Sequence RequestList
		{
			get
			{
				return this.requestList;
			}
		}

		public X509Extensions RequestExtensions
		{
			get
			{
				return this.requestExtensions;
			}
		}

		public static TbsRequest GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return TbsRequest.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static TbsRequest GetInstance(object obj)
		{
			if (obj == null || obj is TbsRequest)
			{
				return (TbsRequest)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new TbsRequest((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public TbsRequest(GeneralName requestorName, Asn1Sequence requestList, X509Extensions requestExtensions)
		{
			this.version = TbsRequest.V1;
			this.requestorName = requestorName;
			this.requestList = requestList;
			this.requestExtensions = requestExtensions;
		}

		private TbsRequest(Asn1Sequence seq)
		{
			int num = 0;
			Asn1Encodable asn1Encodable = seq[0];
			if (asn1Encodable is Asn1TaggedObject)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Encodable;
				if (asn1TaggedObject.TagNo == 0)
				{
					this.versionSet = true;
					this.version = DerInteger.GetInstance(asn1TaggedObject, true);
					num++;
				}
				else
				{
					this.version = TbsRequest.V1;
				}
			}
			else
			{
				this.version = TbsRequest.V1;
			}
			if (seq[num] is Asn1TaggedObject)
			{
				this.requestorName = GeneralName.GetInstance((Asn1TaggedObject)seq[num++], true);
			}
			this.requestList = (Asn1Sequence)seq[num++];
			if (seq.Count == num + 1)
			{
				this.requestExtensions = X509Extensions.GetInstance((Asn1TaggedObject)seq[num], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (!this.version.Equals(TbsRequest.V1) || this.versionSet)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.version)
				});
			}
			if (this.requestorName != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.requestorName)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.requestList
			});
			if (this.requestExtensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.requestExtensions)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
