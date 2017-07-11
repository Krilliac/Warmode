using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredShort : IFormattable, IEquatable<ObscuredShort>
	{
		private static short cryptoKey = 214;

		private short currentCryptoKey;

		private short hiddenValue;

		private short fakeValue;

		private bool inited;

		private ObscuredShort(short value)
		{
			this.currentCryptoKey = ObscuredShort.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0;
			this.inited = true;
		}

		public static void SetNewCryptoKey(short newKey)
		{
			ObscuredShort.cryptoKey = newKey;
		}

		public static short EncryptDecrypt(short value)
		{
			return ObscuredShort.EncryptDecrypt(value, 0);
		}

		public static short EncryptDecrypt(short value, short key)
		{
			if (key == 0)
			{
				return value ^ ObscuredShort.cryptoKey;
			}
			return value ^ key;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredShort.cryptoKey)
			{
				this.hiddenValue = ObscuredShort.EncryptDecrypt(this.InternalDecrypt(), ObscuredShort.cryptoKey);
				this.currentCryptoKey = ObscuredShort.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			short value = this.InternalDecrypt();
			this.currentCryptoKey = (short)UnityEngine.Random.Range(-32768, 32767);
			this.hiddenValue = ObscuredShort.EncryptDecrypt(value, this.currentCryptoKey);
		}

		public short GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(short encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private short InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredShort.cryptoKey;
				this.hiddenValue = ObscuredShort.EncryptDecrypt(0);
				this.fakeValue = 0;
				this.inited = true;
			}
			short num = ObscuredShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0 && num != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return num;
		}

		public override bool Equals(object obj)
		{
			return obj is ObscuredShort && this.Equals((ObscuredShort)obj);
		}

		public bool Equals(ObscuredShort obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredShort.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredShort.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
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

		public static implicit operator ObscuredShort(short value)
		{
			ObscuredShort result = new ObscuredShort(ObscuredShort.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator short(ObscuredShort value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredShort operator ++(ObscuredShort input)
		{
			short value = input.InternalDecrypt() + 1;
			input.hiddenValue = ObscuredShort.EncryptDecrypt(value);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredShort operator --(ObscuredShort input)
		{
			short value = input.InternalDecrypt() - 1;
			input.hiddenValue = ObscuredShort.EncryptDecrypt(value);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
