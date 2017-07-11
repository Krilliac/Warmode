using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class PolicyInformation : Asn1Encodable
	{
		private readonly DerObjectIdentifier policyIdentifier;

		private readonly Asn1Sequence policyQualifiers;

		public DerObjectIdentifier PolicyIdentifier
		{
			get
			{
				return this.policyIdentifier;
			}
		}

		public Asn1Sequence PolicyQualifiers
		{
			get
			{
				return this.policyQualifiers;
			}
		}

		private PolicyInformation(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.policyIdentifier = DerObjectIdentifier.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.policyQualifiers = Asn1Sequence.GetInstance(seq[1]);
			}
		}

		public PolicyInformation(DerObjectIdentifier policyIdentifier)
		{
			this.policyIdentifier = policyIdentifier;
		}

		public PolicyInformation(DerObjectIdentifier policyIdentifier, Asn1Sequence policyQualifiers)
		{
			this.policyIdentifier = policyIdentifier;
			this.policyQualifiers = policyQualifiers;
		}

		public static PolicyInformation GetInstance(object obj)
		{
			if (obj == null || obj is PolicyInformation)
			{
				return (PolicyInformation)obj;
			}
			return new PolicyInformation(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.policyIdentifier
			});
			if (this.policyQualifiers != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.policyQualifiers
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
