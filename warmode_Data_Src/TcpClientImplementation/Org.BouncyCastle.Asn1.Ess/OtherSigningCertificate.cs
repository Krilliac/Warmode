using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ess
{
	[Obsolete("Use version in Asn1.Esf instead")]
	public class OtherSigningCertificate : Asn1Encodable
	{
		private Asn1Sequence certs;

		private Asn1Sequence policies;

		public static OtherSigningCertificate GetInstance(object o)
		{
			if (o == null || o is OtherSigningCertificate)
			{
				return (OtherSigningCertificate)o;
			}
			if (o is Asn1Sequence)
			{
				return new OtherSigningCertificate((Asn1Sequence)o);
			}
			throw new ArgumentException("unknown object in 'OtherSigningCertificate' factory : " + o.GetType().Name + ".");
		}

		public OtherSigningCertificate(Asn1Sequence seq)
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

		public OtherSigningCertificate(OtherCertID otherCertID)
		{
			this.certs = new DerSequence(otherCertID);
		}

		public OtherCertID[] GetCerts()
		{
			OtherCertID[] array = new OtherCertID[this.certs.Count];
			for (int num = 0; num != this.certs.Count; num++)
			{
				array[num] = OtherCertID.GetInstance(this.certs[num]);
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
