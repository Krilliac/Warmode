using System;

namespace Org.BouncyCastle.Crypto
{
	public abstract class AsymmetricKeyParameter : ICipherParameters
	{
		private readonly bool privateKey;

		public bool IsPrivate
		{
			get
			{
				return this.privateKey;
			}
		}

		protected AsymmetricKeyParameter(bool privateKey)
		{
			this.privateKey = privateKey;
		}

		public override bool Equals(object obj)
		{
			AsymmetricKeyParameter asymmetricKeyParameter = obj as AsymmetricKeyParameter;
			return asymmetricKeyParameter != null && this.Equals(asymmetricKeyParameter);
		}

		protected bool Equals(AsymmetricKeyParameter other)
		{
			return this.privateKey == other.privateKey;
		}

		public override int GetHashCode()
		{
			return this.privateKey.GetHashCode();
		}
	}
}
