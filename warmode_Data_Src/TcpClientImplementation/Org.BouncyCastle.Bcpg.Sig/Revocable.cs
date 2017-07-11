using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class Revocable : SignatureSubpacket
	{
		private static byte[] BooleanToByteArray(bool value)
		{
			byte[] array = new byte[1];
			if (value)
			{
				array[0] = 1;
				return array;
			}
			return array;
		}

		public Revocable(bool critical, byte[] data) : base(SignatureSubpacketTag.Revocable, critical, data)
		{
		}

		public Revocable(bool critical, bool isRevocable) : base(SignatureSubpacketTag.Revocable, critical, Revocable.BooleanToByteArray(isRevocable))
		{
		}

		public bool IsRevocable()
		{
			return this.data[0] != 0;
		}
	}
}
