using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ess
{
	public class SigningCertificate : Asn1Encodable
	{
		private Asn1Sequence certs;

		private Asn1Sequence policies;

		public static SigningCertificate GetInstance(object o)
		{
			if (o == null || o is SigningCertificate)
			{
				return (SigningCertificate)o;
			}
			if (o is Asn1Sequence)
			{
				return new SigningCertificate((Asn1Sequence)o);
			}
			throw new ArgumentException("unknown object in 'SigningCertificate' factory : " + o.GetType().Name + ".");
		}

		public SigningCertificate(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.certs = Asn1Sequence.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.policies = Asn1Sequence.GetInstance(seq[1]);
			}
		}

		public SigningCertificate(EssCertID essCertID)
		{
			this.certs = new DerSequence(essCertID);
		}

		public EssCertID[] GetCerts()
		{
			EssCertID[] array = new EssCertID[this.certs.Count];
			for (int num = 0; num != this.certs.Count; num++)
			{
				array[num] = EssCertID.GetInstance(this.certs[num]);
			}
			return array;
		}

		public PolicyInformation[] GetPolicies()
		{
			if (this.policies == null)
			{
				return null;
			}
			PolicyInformation[] array = new PolicyInformation[this.policies.Count];
			for (int num = 0; num != this.policies.Count; num++)
			{
				array[num] = PolicyInformation.GetInstance(this.policies[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certs
			});
			if (this.policies != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.policies
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
