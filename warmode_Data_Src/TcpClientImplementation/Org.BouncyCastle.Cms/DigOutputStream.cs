using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.IO;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class DigOutputStream : BaseOutputStream
	{
		private readonly IDigest dig;

		internal DigOutputStream(IDigest dig)
		{
			this.dig = dig;
		}

		public override void WriteByte(byte b)
		{
			this.dig.Update(b);
		}

		public override void Write(byte[] b, int off, int len)
		{
			this.dig.BlockUpdate(b, off, len);
		}
	}
}
