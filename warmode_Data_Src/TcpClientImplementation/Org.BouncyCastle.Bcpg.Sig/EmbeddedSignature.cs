using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class EmbeddedSignature : SignatureSubpacket
	{
		public EmbeddedSignature(bool critical, byte[] data) : base(SignatureSubpacketTag.EmbeddedSignature, critical, data)
		{
		}
	}
}
