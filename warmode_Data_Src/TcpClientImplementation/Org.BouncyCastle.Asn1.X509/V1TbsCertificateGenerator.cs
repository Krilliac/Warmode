using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class V1TbsCertificateGenerator
	{
		internal DerTaggedObject version = new DerTaggedObject(0, new DerInteger(0));

		internal DerInteger serialNumber;

		internal AlgorithmIdentifier signature;

		internal X509Name issuer;

		internal Time startDate;

		internal Time endDate;

		internal X509Name subject;

		internal SubjectPublicKeyInfo subjectPublicKeyInfo;

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

		public void SetStartDate(Time startDate)
		{
			this.startDate = startDate;
		}

		public void SetStartDate(DerUtcTime startDate)
		{
			this.startDate = new Time(startDate);
		}

		public void SetEndDate(Time endDate)
		{
			this.endDate = endDate;
		}

		public void SetEndDate(DerUtcTime endDate)
		{
			this.endDate = new Time(endDate);
		}

		public void SetSubject(X509Name subject)
		{
			this.subject = subject;
		}

		public void SetSubjectPublicKeyInfo(SubjectPublicKeyInfo pubKeyInfo)
		{
			this.subjectPublicKeyInfo = pubKeyInfo;
		}

		public TbsCertificateStructure GenerateTbsCertificate()
		{
			if (this.serialNumber == null || this.signature == null || this.issuer == null || this.startDate == null || this.endDate == null || this.subject == null || this.subjectPublicKeyInfo == null)
			{
				throw new InvalidOperationException("not all mandatory fields set in V1 TBScertificate generator");
			}
			return new TbsCertificateStructure(new DerSequence(new Asn1Encodable[]
			{
				this.serialNumber,
				this.signature,
				this.issuer,
				new DerSequence(new Asn1Encodable[]
				{
					this.startDate,
					this.endDate
				}),
				this.subject,
				this.subjectPublicKeyInfo
			}));
		}
	}
}
