using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class Holder : Asn1Encodable
	{
		internal readonly IssuerSerial baseCertificateID;

		internal readonly GeneralNames entityName;

		internal readonly ObjectDigestInfo objectDigestInfo;

		private readonly int version;

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public IssuerSerial BaseCertificateID
		{
			get
			{
				return this.baseCertificateID;
			}
		}

		public GeneralNames EntityName
		{
			get
			{
				return this.entityName;
			}
		}

		public ObjectDigestInfo ObjectDigestInfo
		{
			get
			{
				return this.objectDigestInfo;
			}
		}

		public static Holder GetInstance(object obj)
		{
			if (obj is Holder)
			{
				return (Holder)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Holder((Asn1Sequence)obj);
			}
			if (obj is Asn1TaggedObject)
			{
				return new Holder((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public Holder(Asn1TaggedObject tagObj)
		{
			switch (tagObj.TagNo)
			{
			case 0:
				this.baseCertificateID = IssuerSerial.GetInstance(tagObj, false);
				break;
			case 1:
				this.entityName = GeneralNames.GetInstance(tagObj, false);
				break;
			default:
				throw new ArgumentException("unknown tag in Holder");
			}
			this.version = 0;
		}

		private Holder(Asn1Sequence seq)
		{
			if (seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			for (int num = 0; num != seq.Count; num++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[num]);
				switch (instance.TagNo)
				{
				case 0:
					this.baseCertificateID = IssuerSerial.GetInstance(instance, false);
					break;
				case 1:
					this.entityName = GeneralNames.GetInstance(instance, false);
					break;
				case 2:
					this.objectDigestInfo = ObjectDigestInfo.GetInstance(instance, false);
					break;
				default:
					throw new ArgumentException("unknown tag in Holder");
				}
			}
			this.version = 1;
		}

		public Holder(IssuerSerial baseCertificateID) : this(baseCertificateID, 1)
		{
		}

		public Holder(IssuerSerial baseCertificateID, int version)
		{
			this.baseCertificateID = baseCertificateID;
			this.version = version;
		}

		public Holder(GeneralNames entityName) : this(entityName, 1)
		{
		}

		public Holder(GeneralNames entityName, int version)
		{
			this.entityName = entityName;
			this.version = version;
		}

		public Holder(ObjectDigestInfo objectDigestInfo)
		{
			this.objectDigestInfo = objectDigestInfo;
			this.version = 1;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.version == 1)
			{
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
				if (this.baseCertificateID != null)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new DerTaggedObject(false, 0, this.baseCertificateID)
					});
				}
				if (this.entityName != null)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new DerTaggedObject(false, 1, this.entityName)
					});
				}
				if (this.objectDigestInfo != null)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new DerTaggedObject(false, 2, this.objectDigestInfo)
					});
				}
				return new DerSequence(asn1EncodableVector);
			}
			if (this.entityName != null)
			{
				return new DerTaggedObject(false, 1, this.entityName);
			}
			return new DerTaggedObject(false, 0, this.baseCertificateID);
		}
	}
}
