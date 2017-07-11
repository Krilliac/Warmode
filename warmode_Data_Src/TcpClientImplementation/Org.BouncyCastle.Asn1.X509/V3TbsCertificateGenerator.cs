using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class V3TbsCertificateGenerator
	{
		internal DerTaggedObject version = new DerTaggedObject(0, new DerInteger(2));

		internal DerInteger serialNumber;

		internal AlgorithmIdentifier signature;

		internal X509Name issuer;

		internal Time startDate;

		internal Time endDate;

		internal X509Name subject;

		internal SubjectPublicKeyInfo subjectPublicKeyInfo;

		internal X509Extensions extensions;

		private bool altNamePresentAndCritical;

		private DerBitString issuerUniqueID;

		private DerBitString subjectUniqueID;

		public void SetSerialNumber(DerInteger serialNumber)
		{
			this.serialNumber = serialNumber;
		}

		public void SetSignature(AlgorithmIdentifier signature)
		{
			this.signature = signature;
		}

		public void SetIssuer(X509Name issuer)
		{
			this.issuer = issuer;
		}

		public void SetStartDate(DerUtcTime startDate)
		{
			this.startDate = new Time(startDate);
		}

		public void SetStartDate(Time startDate)
		{
			this.startDate = startDate;
		}

		public void SetEndDate(DerUtcTime endDate)
		{
			this.endDate = new Time(endDate);
		}

		public void SetEndDate(Time endDate)
		{
			this.endDate = endDate;
		}

		public void SetSubject(X509Name subject)
		{
			this.subject = subject;
		}

		public void SetIssuerUniqueID(DerBitString uniqueID)
		{
			this.issuerUniqueID = uniqueID;
		}

		public void SetSubjectUniqueID(DerBitString uniqueID)
		{
			this.subjectUniqueID = uniqueID;
		}

		public void SetSubjectPublicKeyInfo(SubjectPublicKeyInfo pubKeyInfo)
		{
			this.subjectPublicKeyInfo = pubKeyInfo;
		}

		public void SetExtensions(X509Extensions extensions)
		{
			this.extensions = extensions;
			if (extensions != null)
			{
				X509Extension extension = extensions.GetExtension(X509Extensions.SubjectAlternativeName);
				if (extension != null && extension.IsCritical)
				{
					this.altNamePresentAndCritical = true;
				}
			}
		}

		public TbsCertificateStructure GenerateTbsCertificate()
		{
			if (this.serialNumber == null || this.signature == null || this.issuer == null || this.startDate == null || this.endDate == null || (this.subject == null && !this.altNamePresentAndCritical) || this.subjectPublicKeyInfo == null)
			{
				throw new InvalidOperationException("not all mandatory fields set in V3 TBScertificate generator");
			}
			DerSequence derSequence = new DerSequence(new Asn1Encodable[]
			{
				this.startDate,
				this.endDate
			});
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.serialNumber,
				this.signature,
				this.issuer,
				derSequence
			});
			if (this.subject != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.subject
				});
			}
			else
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					DerSequence.Empty
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.subjectPublicKeyInfo
			});
			if (this.issuerUniqueID != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.issuerUniqueID)
				});
			}
			if (this.subjectUniqueID != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 2, this.subjectUniqueID)
				});
			}
			if (this.extensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(3, this.extensions)
				});
			}
			return new TbsCertificateStructure(new DerSequence(asn1EncodableVector));
		}
	}
}
