using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class AeadParameters : ICipherParameters
	{
		private readonly byte[] associatedText;

		private readonly byte[] nonce;

		private readonly KeyParameter key;

		private readonly int macSize;

		public virtual KeyParameter Key
		{
			get
			{
				return this.key;
			}
		}

		public virtual int MacSize
		{
			get
			{
				return this.macSize;
			}
		}

		public AeadParameters(KeyParameter key, int macSize, byte[] nonce) : this(key, macSize, nonce, null)
		{
		}

		public AeadParameters(KeyParameter key, int macSize, byte[] nonce, byte[] associatedText)
		{
			this.key = key;
			this.nonce = nonce;
			this.macSize = macSize;
			this.associatedText = associatedText;
		}

		public virtual byte[] GetAssociatedText()
		{
			return this.associatedText;
		}

		public virtual byte[] GetNonce()
		{
			return this.nonce;
		}
	}
}
