using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class SignerUserId : SignatureSubpacket
	{
		private static byte[] UserIdToBytes(string id)
		{
			byte[] array = new byte[id.Length];
			for (int num = 0; num != id.Length; num++)
			{
				array[num] = (byte)id[num];
			}
			return array;
		}

		public SignerUserId(bool critical, byte[] data) : base(SignatureSubpacketTag.SignerUserId, critical, data)
		{
		}

		public SignerUserId(bool critical, string userId) : base(SignatureSubpacketTag.SignerUserId, critical, SignerUserId.UserIdToBytes(userId))
		{
		}

		public string GetId()
		{
			char[] array = new char[this.data.Length];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = (char)(this.data[num] & 255);
			}
			return new string(array);
		}
	}
}
