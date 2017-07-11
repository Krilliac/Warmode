using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredVector2
	{
		[Serializable]
		public struct RawEncryptedVector2
		{
			public int x;

			public int y;
		}

		private static int cryptoKey = 120206;

		private static readonly Vector2 initialFakeValue = Vector2.zero;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private ObscuredVector2.RawEncryptedVector2 hiddenValue;

		[SerializeField]
		private Vector2 fakeValue;

		[SerializeField]
		private bool inited;

		public float x
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.x);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector2.initialFakeValue) && Math.Abs(num - this.fakeValue.x) > ObscuredCheatingDetector.Instance.vector2Epsilon)
				{
					ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return num;
			}
			set
			{
				this.hiddenValue.x = this.InternalEncryptField(value);
				if (ObscuredCheatingDetector.IsRunning)
				{
					this.fakeValue.x = value;
				}
			}
		}

		public float y
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.y);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector2.initialFakeValue) && Math.Abs(num - this.fakeValue.y) > ObscuredCheatingDetector.Instance.vector2Epsilon)
				{
					ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return num;
			}
			set
			{
				this.hiddenValue.y = this.InternalEncryptField(value);
				if (ObscuredCheatingDetector.IsRunning)
				{
					this.fakeValue.y = value;
				}
			}
		}

		public float this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.x;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
				}
				return this.y;
			}
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid ObscuredVector2 index!");
					}
					this.y = value;
				}
				else
				{
					this.x = value;
				}
			}
		}

		private ObscuredVector2(ObscuredVector2.RawEncryptedVector2 value)
		{
			this.currentCryptoKey = ObscuredVector2.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = ObscuredVector2.initialFakeValue;
			this.inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredVector2.cryptoKey = newKey;
		}

		public static ObscuredVector2.RawEncryptedVector2 Encrypt(Vector2 value)
		{
			return ObscuredVector2.Encrypt(value, 0);
		}

		public static ObscuredVector2.RawEncryptedVector2 Encrypt(Vector2 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector2.cryptoKey;
			}
			ObscuredVector2.RawEncryptedVector2 result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			return result;
		}

		public static Vector2 Decrypt(ObscuredVector2.RawEncryptedVector2 value)
		{
			return ObscuredVector2.Decrypt(value, 0);
		}

		public static Vector2 Decrypt(ObscuredVector2.RawEncryptedVector2 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector2.cryptoKey;
			}
			Vector2 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			return result;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredVector2.cryptoKey)
			{
				this.hiddenValue = ObscuredVector2.Encrypt(this.InternalDecrypt(), ObscuredVector2.cryptoKey);
				this.currentCryptoKey = ObscuredVector2.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			Vector2 value = this.InternalDecrypt();
			this.currentCryptoKey = UnityEngine.Random.Range(-2147483648, 2147483647);
			this.hiddenValue = ObscuredVector2.Encrypt(value, this.currentCryptoKey);
		}

		public ObscuredVector2.RawEncryptedVector2 GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ObscuredVector2.RawEncryptedVector2 encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private Vector2 InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredVector2.cryptoKey;
				this.hiddenValue = ObscuredVector2.Encrypt(ObscuredVector2.initialFakeValue);
				this.fakeValue = ObscuredVector2.initialFakeValue;
				this.inited = true;
			}
			Vector2 vector;
			vector.x = ObscuredFloat.Decrypt(this.hiddenValue.x, this.currentCryptoKey);
			vector.y = ObscuredFloat.Decrypt(this.hiddenValue.y, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector2.initialFakeValue) && !this.CompareVectorsWithTolerance(vector, this.fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return vector;
		}

		private bool CompareVectorsWithTolerance(Vector2 vector1, Vector2 vector2)
		{
			float vector2Epsilon = ObscuredCheatingDetector.Instance.vector2Epsilon;
			return Math.Abs(vector1.x - vector2.x) < vector2Epsilon && Math.Abs(vector1.y - vector2.y) < vector2Epsilon;
		}

		private float InternalDecryptField(int encrypted)
		{
			int key = ObscuredVector2.cryptoKey;
			if (this.currentCryptoKey != ObscuredVector2.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			return ObscuredFloat.Decrypt(encrypted, key);
		}

		private int InternalEncryptField(float encrypted)
		{
			return ObscuredFloat.Encrypt(encrypted, ObscuredVector2.cryptoKey);
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

		public static implicit operator ObscuredVector2(Vector2 value)
		{
			ObscuredVector2 result = new ObscuredVector2(ObscuredVector2.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Vector2(ObscuredVector2 value)
		{
			return value.InternalDecrypt();
		}

		public static implicit operator Vector3(ObscuredVector2 value)
		{
			Vector2 vector = value.InternalDecrypt();
			return new Vector3(vector.x, vector.y, 0f);
		}
	}
}
