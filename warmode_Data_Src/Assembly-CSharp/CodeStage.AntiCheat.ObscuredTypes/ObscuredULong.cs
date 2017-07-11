using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredULong : IFormattable, IEquatable<ObscuredULong>
	{
		private static ulong cryptoKey = 444443uL;

		private ulong currentCryptoKey;

		private ulong hiddenValue;

		private ulong fakeValue;

		private bool inited;

		private ObscuredULong(ulong value)
		{
			this.currentCryptoKey = ObscuredULong.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0uL;
			this.inited = true;
		}

		public static void SetNewCryptoKey(ulong newKey)
		{
			ObscuredULong.cryptoKey = newKey;
		}

		public static ulong Encrypt(ulong value)
		{
			return ObscuredULong.Encrypt(value, 0uL);
		}

		public static ulong Decrypt(ulong value)
		{
			return ObscuredULong.Decrypt(value, 0uL);
		}

		public static ulong Encrypt(ulong value, ulong key)
		{
			if (key == 0uL)
			{
				return value ^ ObscuredULong.cryptoKey;
			}
			return value ^ key;
		}

		public static ulong Decrypt(ulong value, ulong key)
		{
			if (key == 0uL)
			{
				return value ^ ObscuredULong.cryptoKey;
			}
			return value ^ key;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredULong.cryptoKey)
			{
				this.hiddenValue = ObscuredULong.Encrypt(this.InternalDecrypt(), ObscuredULong.cryptoKey);
				this.currentCryptoKey = ObscuredULong.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			ulong value = this.InternalDecrypt();
			this.currentCryptoKey = (ulong)((long)UnityEngine.Random.Range(0, 2147483647));
			this.hiddenValue = ObscuredULong.Encrypt(value, this.currentCryptoKey);
		}

		public ulong GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ulong encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private ulong InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredULong.cryptoKey;
				this.hiddenValue = ObscuredULong.Encrypt(0uL);
				this.fakeValue = 0uL;
				this.inited = true;
			}
			ulong num = ObscuredULong.Decrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0uL && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			return obj is ObscuredULong && this.Equals((ObscuredULong)obj);
		}

		public bool Equals(ObscuredULong obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredULong.Decrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredULong.Decrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		public string ToString(string format)
		{
			return this.InternalDecrypt().ToString(format);
		}

		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(format, provider);
		}

		public static implicit operator ObscuredULong(ulong value)
		{
			ObscuredULong result = new ObscuredULong(ObscuredULong.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator ulong(ObscuredULong value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredULong operator ++(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() + 1uL;
			input.hiddenValue = ObscuredULong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredULong operator --(ObscuredULong input)
		{
			ulong value = input.InternalDecrypt() - 1uL;
			input.hiddenValue = ObscuredULong.Encrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
