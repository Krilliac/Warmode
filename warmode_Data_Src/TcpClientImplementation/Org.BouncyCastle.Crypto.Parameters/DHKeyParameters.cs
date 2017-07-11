using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DHKeyParameters : AsymmetricKeyParameter
	{
		private readonly DHParameters parameters;

		private readonly DerObjectIdentifier algorithmOid;

		public DHParameters Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public DerObjectIdentifier AlgorithmOid
		{
			get
			{
				return this.algorithmOid;
			}
		}

		protected DHKeyParameters(bool isPrivate, DHParameters parameters) : this(isPrivate, parameters, PkcsObjectIdentifiers.DhKeyAgreement)
		{
		}

		protected DHKeyParameters(bool isPrivate, DHParameters parameters, DerObjectIdentifier algorithmOid) : base(isPrivate)
		{
			this.parameters = parameters;
			this.algorithmOid = algorithmOid;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DHKeyParameters dHKeyParameters = obj as DHKeyParameters;
			return dHKeyParameters != null && this.Equals(dHKeyParameters);
		}

		protected bool Equals(DHKeyParameters other)
		{
			return object.Equals(this.parameters, other.parameters) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			if (this.parameters != null)
			{
				num ^= this.parameters.GetHashCode();
			}
			return num;
		}
	}
}
