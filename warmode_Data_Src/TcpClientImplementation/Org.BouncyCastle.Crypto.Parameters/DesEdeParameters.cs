using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DesEdeParameters : DesParameters
	{
		public const int DesEdeKeyLength = 24;

		private static byte[] FixKey(byte[] key, int keyOff, int keyLen)
		{
			byte[] array = new byte[24];
			if (keyLen != 16)
			{
				if (keyLen != 24)
				{
					throw new ArgumentException("Bad length for DESede key: " + keyLen, "keyLen");
				}
				Array.Copy(key, keyOff, array, 0, 24);
			}
			else
			{
				Array.Copy(key, keyOff, array, 0, 16);
				Array.Copy(key, keyOff, array, 16, 8);
			}
			if (DesEdeParameters.IsWeakKey(array))
			{
				throw new ArgumentException("attempt to create weak DESede key");
			}
			return array;
		}

		public DesEdeParameters(byte[] key) : base(DesEdeParameters.FixKey(key, 0, key.Length))
		{
		}

		public DesEdeParameters(byte[] key, int keyOff, int keyLen) : base(DesEdeParameters.FixKey(key, keyOff, keyLen))
		{
		}

		public static bool IsWeakKey(byte[] key, int offset, int length)
		{
			for (int i = offset; i < length; i += 8)
			{
				if (DesParameters.IsWeakKey(key, i))
				{
					return true;
				}
			}
			return false;
		}

		public new static bool IsWeakKey(byte[] key, int offset)
		{
			return DesEdeParameters.IsWeakKey(key, offset, key.Length - offset);
		}

		public new static bool IsWeakKey(byte[] key)
		{
			return DesEdeParameters.IsWeakKey(key, 0, key.Length);
		}
	}
}
