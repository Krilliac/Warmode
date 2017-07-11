using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public abstract class DsaKeyParameters : AsymmetricKeyParameter
	{
		private readonly DsaParameters parameters;

		public DsaParameters Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		protected DsaKeyParameters(bool isPrivate, DsaParameters parameters) : base(isPrivate)
		{
			this.parameters = parameters;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DsaKeyParameters dsaKeyParameters = obj as DsaKeyParameters;
			return dsaKeyParameters != null && this.Equals(dsaKeyParameters);
		}

		protected bool Equals(DsaKeyParameters other)
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
