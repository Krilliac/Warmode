using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class Exportable : SignatureSubpacket
	{
		private static byte[] BooleanToByteArray(bool val)
		{
			byte[] array = new byte[1];
			if (val)
			{
				array[0] = 1;
				return array;
			}
			return array;
		}

		public Exportable(bool critical, byte[] data) : base(SignatureSubpacketTag.Exportable, critical, data)
		{
		}

		public Exportable(bool critical, bool isExportable) : base(SignatureSubpacketTag.Exportable, critical, Exportable.BooleanToByteArray(isExportable))
		{
		}

		public bool IsExportable()
		{
			return this.data[0] != 0;
		}
	}
}
