using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredUShort : IFormattable, IEquatable<ObscuredUShort>
	{
		private static ushort cryptoKey = 224;

		private ushort currentCryptoKey;

		private ushort hiddenValue;

		private ushort fakeValue;

		private bool inited;

		private ObscuredUShort(ushort value)
		{
			this.currentCryptoKey = ObscuredUShort.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(ushort newKey)
		{
			ObscuredUShort.cryptoKey = newKey;
		}

		public static ushort EncryptDecrypt(ushort value)
		{
			return ObscuredUShort.EncryptDecrypt(value, 0);
		}

		public static ushort EncryptDecrypt(ushort value, ushort key)
		{
			if (key == 0)
			{
				return value ^ ObscuredUShort.cryptoKey;
			}
			return value ^ key;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredUShort.cryptoKey)
			{
				this.hiddenValue = ObscuredUShort.EncryptDecrypt(this.InternalDecrypt(), ObscuredUShort.cryptoKey);
				this.currentCryptoKey = ObscuredUShort.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			ushort value = this.InternalDecrypt();
			this.currentCryptoKey = (ushort)UnityEngine.Random.Range(0, 32767);
			this.hiddenValue = ObscuredUShort.EncryptDecrypt(value, this.currentCryptoKey);
		}

		public ushort GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ushort encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private ushort InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredUShort.cryptoKey;
				this.hiddenValue = ObscuredUShort.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			ushort num = ObscuredUShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			return obj is ObscuredUShort && this.Equals((ObscuredUShort)obj);
		}

		public bool Equals(ObscuredUShort obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredUShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredUShort.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
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

		public static implicit operator ObscuredUShort(ushort value)
		{
			ObscuredUShort result = new ObscuredUShort(ObscuredUShort.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ushort(ObscuredUShort value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredUShort operator ++(ObscuredUShort input)
		{
			ushort value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredUShort.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredUShort operator --(ObscuredUShort input)
		{
			ushort value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredUShort.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
