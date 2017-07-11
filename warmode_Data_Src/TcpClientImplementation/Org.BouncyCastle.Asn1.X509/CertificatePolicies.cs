using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CertificatePolicies : Asn1Encodable
	{
		private readonly PolicyInformation[] policyInformation;

		public static CertificatePolicies GetInstance(object obj)
		{
			if (obj == null || obj is CertificatePolicies)
			{
				return (CertificatePolicies)obj;
			}
			return new CertificatePolicies(Asn1Sequence.GetInstance(obj));
		}

		public static CertificatePolicies GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return CertificatePolicies.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public CertificatePolicies(PolicyInformation name)
		{
			this.policyInformation = new PolicyInformation[]
			{
				name
			};
		}

		public CertificatePolicies(PolicyInformation[] policyInformation)
		{
			this.policyInformation = policyInformation;
		}

		private CertificatePolicies(Asn1Sequence seq)
		{
			this.policyInformation = new PolicyInformation[seq.Count];
			for (int i = 0; i < seq.Count; i++)
			{
				this.policyInformation[i] = PolicyInformation.GetInstance(seq[i]);
			}
		}

		public virtual PolicyInformation[] GetPolicyInformation()
		{
			return (PolicyInformation[])this.policyInformation.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(this.policyInformation);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("CertificatePolicies:");
			if (this.policyInformation != null && this.policyInformation.Length > 0)
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(this.policyInformation[0]);
				for (int i = 1; i < this.policyInformation.Length; i++)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(this.policyInformation[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
