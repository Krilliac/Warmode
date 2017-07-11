using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredByte : IEquatable<ObscuredByte>, IFormattable
	{
		private static byte cryptoKey = 244;

		private byte currentCryptoKey;

		private byte hiddenValue;

		private byte fakeValue;

		private bool inited;

		private ObscuredByte(byte value)
		{
			this.currentCryptoKey = ObscuredByte.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(byte newKey)
		{
			ObscuredByte.cryptoKey = newKey;
		}

		public static byte EncryptDecrypt(byte value)
		{
			return ObscuredByte.EncryptDecrypt(value, 0);
		}

		public static byte EncryptDecrypt(byte value, byte key)
		{
			if (key == 0)
			{
				return value ^ ObscuredByte.cryptoKey;
			}
			return value ^ key;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredByte.cryptoKey)
			{
				this.hiddenValue = ObscuredByte.EncryptDecrypt(this.InternalDecrypt(), ObscuredByte.cryptoKey);
				this.currentCryptoKey = ObscuredByte.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			byte value = this.InternalDecrypt();
			this.currentCryptoKey = (byte)UnityEngine.Random.Range(0, 255);
			this.hiddenValue = ObscuredByte.EncryptDecrypt(value, this.currentCryptoKey);
		}

		public byte GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(byte encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private byte InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredByte.cryptoKey;
				this.hiddenValue = ObscuredByte.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			byte b = ObscuredByte.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && b != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return b;
		}

		public override bool Equals(object obj)
		{
			return obj is ObscuredByte && this.Equals((ObscuredByte)obj);
		}

		public bool Equals(ObscuredByte obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredByte.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredByte.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredByte(byte value)
		{
			ObscuredByte result = new ObscuredByte(ObscuredByte.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator byte(ObscuredByte value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredByte operator ++(ObscuredByte input)
		{
			byte value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredByte operator --(ObscuredByte input)
		{
			byte value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredByte.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
