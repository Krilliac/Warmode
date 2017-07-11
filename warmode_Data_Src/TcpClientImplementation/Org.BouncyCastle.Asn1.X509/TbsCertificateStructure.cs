using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class TbsCertificateStructure : Asn1Encodable
	{
		internal Asn1Sequence seq;

		internal DerInteger version;

		internal DerInteger serialNumber;

		internal AlgorithmIdentifier signature;

		internal X509Name issuer;

		internal Time startDate;

		internal Time endDate;

		internal X509Name subject;

		internal SubjectPublicKeyInfo subjectPublicKeyInfo;

		internal DerBitString issuerUniqueID;

		internal DerBitString subjectUniqueID;

		internal X509Extensions extensions;

		public int Version
		{
			get
			{
				return this.version.Value.IntValue + 1;
			}
		}

		public DerInteger VersionNumber
		{
			get
			{
				return this.version;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public AlgorithmIdentifier Signature
		{
			get
			{
				return this.signature;
			}
		}

		public X509Name Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public Time StartDate
		{
			get
			{
				return this.startDate;
			}
		}

		public Time EndDate
		{
			get
			{
				return this.endDate;
			}
		}

		public X509Name Subject
		{
			get
			{
				return this.subject;
			}
		}

		public SubjectPublicKeyInfo SubjectPublicKeyInfo
		{
			get
			{
				return this.subjectPublicKeyInfo;
			}
		}

		public DerBitString IssuerUniqueID
		{
			get
			{
				return this.issuerUniqueID;
			}
		}

		public DerBitString SubjectUniqueID
		{
			get
			{
				return this.subjectUniqueID;
			}
		}

		public X509Extensions Extensions
		{
			get
			{
				return this.extensions;
			}
		}

		public static TbsCertificateStructure GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return TbsCertificateStructure.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static TbsCertificateStructure GetInstance(object obj)
		{
			if (obj is TbsCertificateStructure)
			{
				return (TbsCertificateStructure)obj;
			}
			if (obj != null)
			{
				return new TbsCertificateStructure(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		internal TbsCertificateStructure(Asn1Sequence seq)
		{
			int num = 0;
			this.seq = seq;
			if (seq[0] is DerTaggedObject)
			{
				this.version = DerInteger.GetInstance((Asn1TaggedObject)seq[0], true);
			}
			else
			{
				num = -1;
				this.version = new DerInteger(0);
			}
			this.serialNumber = DerInteger.GetInstance(seq[num + 1]);
			this.signature = AlgorithmIdentifier.GetInstance(seq[num + 2]);
			this.issuer = X509Name.GetInstance(seq[num + 3]);
			Asn1Sequence asn1Sequence = (Asn1Sequence)seq[num + 4];
			this.startDate = Time.GetInstance(asn1Sequence[0]);
			this.endDate = Time.GetInstance(asn1Sequence[1]);
			this.subject = X509Name.GetInstance(seq[num + 5]);
			this.subjectPublicKeyInfo = SubjectPublicKeyInfo.GetInstance(seq[num + 6]);
			for (int i = seq.Count - (num + 6) - 1; i > 0; i--)
			{
				DerTaggedObject derTaggedObject = (DerTaggedObject)seq[num + 6 + i];
				switch (derTaggedObject.TagNo)
				{
				case 1:
					this.issuerUniqueID = DerBitString.GetInstance(derTaggedObject, false);
					break;
				case 2:
					this.subjectUniqueID = DerBitString.GetInstance(derTaggedObject, false);
					break;
				case 3:
					this.extensions = X509Extensions.GetInstance(derTaggedObject);
					break;
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}
	}
}
