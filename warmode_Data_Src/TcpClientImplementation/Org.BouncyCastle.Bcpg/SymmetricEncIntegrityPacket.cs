using System;

namespace Org.BouncyCastle.Bcpg
{
	public class SymmetricEncIntegrityPacket : InputStreamPacket
	{
		internal readonly int version;

		internal SymmetricEncIntegrityPacket(BcpgInputStream bcpgIn) : base(bcpgIn)
		{
			this.version = bcpgIn.ReadByte();
		}
	}
}
