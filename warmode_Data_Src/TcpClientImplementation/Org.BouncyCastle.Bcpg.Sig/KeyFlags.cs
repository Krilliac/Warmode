using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class KeyFlags : SignatureSubpacket
	{
		public const int CertifyOther = 1;

		public const int SignData = 2;

		public const int EncryptComms = 4;

		public const int EncryptStorage = 8;

		public const int Split = 16;

		public const int Authentication = 32;

		public const int Shared = 128;

		public int Flags
		{
			get
			{
				int num = 0;
				for (int num2 = 0; num2 != this.data.Length; num2++)
				{
					num |= (int)(this.data[num2] & 255) << num2 * 8;
				}
				return num;
			}
		}

		private static byte[] IntToByteArray(int v)
		{
			byte[] array = new byte[4];
			int num = 0;
			for (int num2 = 0; num2 != 4; num2++)
			{
				array[num2] = (byte)(v >> num2 * 8);
				if (array[num2] != 0)
				{
					num = num2;
				}
			}
			byte[] array2 = new byte[num + 1];
			Array.Copy(array, 0, array2, 0, array2.Length);
			return array2;
		}

		public KeyFlags(bool critical, byte[] data) : base(SignatureSubpacketTag.KeyFlags, critical, data)
		{
		}

		public KeyFlags(bool critical, int flags) : base(SignatureSubpacketTag.KeyFlags, critical, KeyFlags.IntToByteArray(flags))
		{
		}
	}
}
