using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class SigOutputStream : BaseOutputStream
	{
		private readonly ISigner sig;

		internal SigOutputStream(ISigner sig)
		{
			this.sig = sig;
		}

		public override void WriteByte(byte b)
		{
			try
			{
				this.sig.Update(b);
			}
			catch (SignatureException arg)
			{
				throw new CmsStreamException("signature problem: " + arg);
			}
		}

		public override void Write(byte[] b, int off, int len)
		{
			try
			{
				this.sig.BlockUpdate(b, off, len);
			}
			catch (SignatureException arg)
			{
				throw new CmsStreamException("signature problem: " + arg);
			}
		}
	}
}
