using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class SignaturePolicyIdentifier : Asn1Encodable, IAsn1Choice
	{
		private readonly SignaturePolicyId sigPolicy;

		public SignaturePolicyId SignaturePolicyId
		{
			get
			{
				return this.sigPolicy;
			}
		}

		public static SignaturePolicyIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is SignaturePolicyIdentifier)
			{
				return (SignaturePolicyIdentifier)obj;
			}
			if (obj is SignaturePolicyId)
			{
				return new SignaturePolicyIdentifier((SignaturePolicyId)obj);
			}
			if (obj is Asn1Null)
			{
				return new SignaturePolicyIdentifier();
			}
			throw new ArgumentException("Unknown object in 'SignaturePolicyIdentifier' factory: " + obj.GetType().Name, "obj");
		}

		public SignaturePolicyIdentifier()
		{
			this.sigPolicy = null;
		}

		public SignaturePolicyIdentifier(SignaturePolicyId signaturePolicyId)
		{
			if (signaturePolicyId == null)
			{
				throw new ArgumentNullException("signaturePolicyId");
			}
			this.sigPolicy = signaturePolicyId;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.sigPolicy != null)
			{
				return this.sigPolicy.ToAsn1Object();
			}
			return DerNull.Instance;
		}
	}
}
