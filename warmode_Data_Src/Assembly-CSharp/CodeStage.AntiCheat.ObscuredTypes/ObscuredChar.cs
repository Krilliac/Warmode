using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredChar : IEquatable<ObscuredChar>
	{
		private static char cryptoKey = '—';

		private char currentCryptoKey;

		private char hiddenValue;

		private char fakeValue;

		private bool inited;

		private ObscuredChar(char value)
		{
			this.currentCryptoKey = ObscuredChar.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = '\0';
			this.inited = true;
		}

		public static void SetNewCryptoKey(char newKey)
		{
			ObscuredChar.cryptoKey = newKey;
		}

		public static char EncryptDecrypt(char value)
		{
			return ObscuredChar.EncryptDecrypt(value, '\0');
		}

		public static char EncryptDecrypt(char value, char key)
		{
			if (key == '\0')
			{
				return value ^ ObscuredChar.cryptoKey;
			}
			return value ^ key;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredChar.cryptoKey)
			{
				this.hiddenValue = ObscuredChar.EncryptDecrypt(this.InternalDecrypt(), ObscuredChar.cryptoKey);
				this.currentCryptoKey = ObscuredChar.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			char value = this.InternalDecrypt();
			this.currentCryptoKey = (char)UnityEngine.Random.Range(0, 65535);
			this.hiddenValue = ObscuredChar.EncryptDecrypt(value, this.currentCryptoKey);
		}

		public char GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(char encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private char InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredChar.cryptoKey;
				this.hiddenValue = ObscuredChar.EncryptDecrypt('\0');
				this.fakeValue = '\0';
				this.inited = true;
			}
			char c = ObscuredChar.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != '\0' && c != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return c;
		}

		public override bool Equals(object obj)
		{
			return obj is ObscuredChar && this.Equals((ObscuredChar)obj);
		}

		public bool Equals(ObscuredChar obj)
		{
			if (this.currentCryptoKey == obj.currentCryptoKey)
			{
				return this.hiddenValue == obj.hiddenValue;
			}
			return ObscuredChar.EncryptDecrypt(this.hiddenValue, this.currentCryptoKey) == ObscuredChar.EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
		}

		public override string ToString()
		{
			return this.InternalDecrypt().ToString();
		}

		public string ToString(IFormatProvider provider)
		{
			return this.InternalDecrypt().ToString(provider);
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredChar(char value)
		{
			ObscuredChar result = new ObscuredChar(ObscuredChar.EncryptDecrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator char(ObscuredChar value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredChar operator ++(ObscuredChar input)
		{
			char value = input.InternalDecrypt() + '\u0001';
			input.hiddenValue = ObscuredChar.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredChar operator --(ObscuredChar input)
		{
			char value = input.InternalDecrypt() - '\u0001';
			input.hiddenValue = ObscuredChar.EncryptDecrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
