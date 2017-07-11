using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class SignerInputBuffer : MemoryStream
	{
		private class SigStream : BaseOutputStream
		{
			private readonly ISigner s;

			internal SigStream(ISigner s)
			{
				this.s = s;
			}

			public override void WriteByte(byte b)
			{
				this.s.Update(b);
			}

			public override void Write(byte[] buf, int off, int len)
			{
				this.s.BlockUpdate(buf, off, len);
			}
		}

		internal void UpdateSigner(ISigner s)
		{
			this.WriteTo(new SignerInputBuffer.SigStream(s));
		}
	}
}
