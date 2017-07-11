using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class CertificationRequestInfo : Asn1Encodable
	{
		internal DerInteger version = new DerInteger(0);

		internal X509Name subject;

		internal SubjectPublicKeyInfo subjectPKInfo;

		internal Asn1Set attributes;

		public DerInteger Version
		{
			get
			{
				return this.version;
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
				return this.subjectPKInfo;
			}
		}

		public Asn1Set Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public static CertificationRequestInfo GetInstance(object obj)
		{
			if (obj is CertificationRequestInfo)
			{
				return (CertificationRequestInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertificationRequestInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public CertificationRequestInfo(X509Name subject, SubjectPublicKeyInfo pkInfo, Asn1Set attributes)
		{
			this.subject = subject;
			this.subjectPKInfo = pkInfo;
			this.attributes = attributes;
			if (subject == null || this.version == null || this.subjectPKInfo == null)
			{
				throw new ArgumentException("Not all mandatory fields set in CertificationRequestInfo generator.");
			}
		}

		private CertificationRequestInfo(Asn1Sequence seq)
		{
			this.version = (DerInteger)seq[0];
			this.subject = X509Name.GetInstance(seq[1]);
			this.subjectPKInfo = SubjectPublicKeyInfo.GetInstance(seq[2]);
			if (seq.Count > 3)
			{
				DerTaggedObject obj = (DerTaggedObject)seq[3];
				this.attributes = Asn1Set.GetInstance(obj, false);
			}
			if (this.subject == null || this.version == null || this.subjectPKInfo == null)
			{
				throw new ArgumentException("Not all mandatory fields set in CertificationRequestInfo generator.");
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.subject,
				this.subjectPKInfo
			});
			if (this.attributes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.attributes)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
