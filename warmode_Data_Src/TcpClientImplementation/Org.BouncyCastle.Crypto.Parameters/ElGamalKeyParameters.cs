using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ElGamalKeyParameters : AsymmetricKeyParameter
	{
		private readonly ElGamalParameters parameters;

		public ElGamalParameters Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		protected ElGamalKeyParameters(bool isPrivate, ElGamalParameters parameters) : base(isPrivate)
		{
			this.parameters = parameters;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			ElGamalKeyParameters elGamalKeyParameters = obj as ElGamalKeyParameters;
			return elGamalKeyParameters != null && this.Equals(elGamalKeyParameters);
		}

		protected bool Equals(ElGamalKeyParameters other)
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
