using System;

namespace Org.BouncyCastle.Bcpg.Sig
{
	public class PreferredAlgorithms : SignatureSubpacket
	{
		private static byte[] IntToByteArray(int[] v)
		{
			byte[] array = new byte[v.Length];
			for (int num = 0; num != v.Length; num++)
			{
				array[num] = (byte)v[num];
			}
			return array;
		}

		public PreferredAlgorithms(SignatureSubpacketTag type, bool critical, byte[] data) : base(type, critical, data)
		{
		}

		public PreferredAlgorithms(SignatureSubpacketTag type, bool critical, int[] preferences) : base(type, critical, PreferredAlgorithms.IntToByteArray(preferences))
		{
		}

		public int[] GetPreferences()
		{
			int[] array = new int[this.data.Length];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = (int)(this.data[num] & 255);
			}
			return array;
		}
	}
}
