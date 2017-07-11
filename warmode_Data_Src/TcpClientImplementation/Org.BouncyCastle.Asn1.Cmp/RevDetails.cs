using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class RevDetails : Asn1Encodable
	{
		private readonly CertTemplate certDetails;

		private readonly X509Extensions crlEntryDetails;

		public virtual CertTemplate CertDetails
		{
			get
			{
				return this.certDetails;
			}
		}

		public virtual X509Extensions CrlEntryDetails
		{
			get
			{
				return this.crlEntryDetails;
			}
		}

		private RevDetails(Asn1Sequence seq)
		{
			this.certDetails = CertTemplate.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.crlEntryDetails = X509Extensions.GetInstance(seq[1]);
			}
		}

		public static RevDetails GetInstance(object obj)
		{
			if (obj is RevDetails)
			{
				return (RevDetails)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RevDetails((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public RevDetails(CertTemplate certDetails)
		{
			this.certDetails = certDetails;
		}

		public RevDetails(CertTemplate certDetails, X509Extensions crlEntryDetails)
		{
			this.crlEntryDetails = crlEntryDetails;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certDetails
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.crlEntryDetails
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
