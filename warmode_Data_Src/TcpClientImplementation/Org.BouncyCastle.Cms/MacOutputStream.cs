using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.IO;
using System;

namespace Org.BouncyCastle.Cms
{
	internal class MacOutputStream : BaseOutputStream
	{
		private readonly IMac mac;

		internal MacOutputStream(IMac mac)
		{
			this.mac = mac;
		}

		public override void Write(byte[] b, int off, int len)
		{
			this.mac.BlockUpdate(b, off, len);
		}

		public override void WriteByte(byte b)
		{
			this.mac.Update(b);
		}
	}
}
