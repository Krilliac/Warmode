using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredVector3
	{
		[Serializable]
		public struct RawEncryptedVector3
		{
			public int x;

			public int y;

			public int z;
		}

		private static int cryptoKey = 120207;

		private static readonly Vector3 initialFakeValue = Vector3.zero;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private ObscuredVector3.RawEncryptedVector3 hiddenValue;

		[SerializeField]
		private Vector3 fakeValue;

		[SerializeField]
		private bool inited;

		public float x
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.x);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector3.initialFakeValue) && Math.Abs(num - this.fakeValue.x) > ObscuredCheatingDetector.Instance.vector3Epsilon)
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
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector3.initialFakeValue) && Math.Abs(num - this.fakeValue.y) > ObscuredCheatingDetector.Instance.vector3Epsilon)
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

		public float z
		{
			get
			{
				float num = this.InternalDecryptField(this.hiddenValue.z);
				if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredVector3.initialFakeValue) && Math.Abs(num - this.fakeValue.z) > ObscuredCheatingDetector.Instance.vector3Epsilon)
				{
					ObscuredCheatingDetector.Instance.OnCheatingDetected();
				}
				return num;
			}
			set
			{
				this.hiddenValue.z = this.InternalEncryptField(value);
				if (ObscuredCheatingDetector.IsRunning)
				{
					this.fakeValue.z = value;
				}
			}
		}

		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
				}
			}
		}

		private ObscuredVector3(ObscuredVector3.RawEncryptedVector3 encrypted)
		{
			this.currentCryptoKey = ObscuredVector3.cryptoKey;
			this.hiddenValue = encrypted;
			this.fakeValue = ObscuredVector3.initialFakeValue;
			this.inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredVector3.cryptoKey = newKey;
		}

		public static ObscuredVector3.RawEncryptedVector3 Encrypt(Vector3 value)
		{
			return ObscuredVector3.Encrypt(value, 0);
		}

		public static ObscuredVector3.RawEncryptedVector3 Encrypt(Vector3 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector3.cryptoKey;
			}
			ObscuredVector3.RawEncryptedVector3 result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			result.z = ObscuredFloat.Encrypt(value.z, key);
			return result;
		}

		public static Vector3 Decrypt(ObscuredVector3.RawEncryptedVector3 value)
		{
			return ObscuredVector3.Decrypt(value, 0);
		}

		public static Vector3 Decrypt(ObscuredVector3.RawEncryptedVector3 value, int key)
		{
			if (key == 0)
			{
				key = ObscuredVector3.cryptoKey;
			}
			Vector3 result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			return result;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredVector3.cryptoKey)
			{
				this.hiddenValue = ObscuredVector3.Encrypt(this.InternalDecrypt(), ObscuredVector3.cryptoKey);
				this.currentCryptoKey = ObscuredVector3.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			Vector3 value = this.InternalDecrypt();
			this.currentCryptoKey = UnityEngine.Random.Range(-2147483648, 2147483647);
			this.hiddenValue = ObscuredVector3.Encrypt(value, this.currentCryptoKey);
		}

		public ObscuredVector3.RawEncryptedVector3 GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ObscuredVector3.RawEncryptedVector3 encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private Vector3 InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredVector3.cryptoKey;
				this.hiddenValue = ObscuredVector3.Encrypt(ObscuredVector3.initialFakeValue, ObscuredVector3.cryptoKey);
				this.fakeValue = ObscuredVector3.initialFakeValue;
				this.inited = true;
			}
			Vector3 vector;
			vector.x = ObscuredFloat.Decrypt(this.hiddenValue.x, this.currentCryptoKey);
			vector.y = ObscuredFloat.Decrypt(this.hiddenValue.y, this.currentCryptoKey);
			vector.z = ObscuredFloat.Decrypt(this.hiddenValue.z, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(Vector3.zero) && !this.CompareVectorsWithTolerance(vector, this.fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return vector;
		}

		private bool CompareVectorsWithTolerance(Vector3 vector1, Vector3 vector2)
		{
			float vector3Epsilon = ObscuredCheatingDetector.Instance.vector3Epsilon;
			return Math.Abs(vector1.x - vector2.x) < vector3Epsilon && Math.Abs(vector1.y - vector2.y) < vector3Epsilon && Math.Abs(vector1.z - vector2.z) < vector3Epsilon;
		}

		private float InternalDecryptField(int encrypted)
		{
			int key = ObscuredVector3.cryptoKey;
			if (this.currentCryptoKey != ObscuredVector3.cryptoKey)
			{
				key = this.currentCryptoKey;
			}
			return ObscuredFloat.Decrypt(encrypted, key);
		}

		private int InternalEncryptField(float encrypted)
		{
			return ObscuredFloat.Encrypt(encrypted, ObscuredVector3.cryptoKey);
		}

		public override bool Equals(object other)
		{
			return this.InternalDecrypt().Equals(other);
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

		public static implicit operator ObscuredVector3(Vector3 value)
		{
			ObscuredVector3 result = new ObscuredVector3(ObscuredVector3.Encrypt(value, ObscuredVector3.cryptoKey));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Vector3(ObscuredVector3 value)
		{
			return value.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() + b.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(Vector3 a, ObscuredVector3 b)
		{
			return a + b.InternalDecrypt();
		}

		public static ObscuredVector3 operator +(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() + b;
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a, ObscuredVector3 b)
		{
			return a.InternalDecrypt() - b.InternalDecrypt();
		}

		public static ObscuredVector3 operator -(Vector3 a, ObscuredVector3 b)
		{
			return a - b.InternalDecrypt();
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a, Vector3 b)
		{
			return a.InternalDecrypt() - b;
		}

		public static ObscuredVector3 operator -(ObscuredVector3 a)
		{
			return -a.InternalDecrypt();
		}

		public static ObscuredVector3 operator *(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() * d;
		}

		public static ObscuredVector3 operator *(float d, ObscuredVector3 a)
		{
			return d * a.InternalDecrypt();
		}

		public static ObscuredVector3 operator /(ObscuredVector3 a, float d)
		{
			return a.InternalDecrypt() / d;
		}

		public static bool operator ==(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs.InternalDecrypt();
		}

		public static bool operator ==(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs == rhs.InternalDecrypt();
		}

		public static bool operator ==(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() == rhs;
		}

		public static bool operator !=(ObscuredVector3 lhs, ObscuredVector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs.InternalDecrypt();
		}

		public static bool operator !=(Vector3 lhs, ObscuredVector3 rhs)
		{
			return lhs != rhs.InternalDecrypt();
		}

		public static bool operator !=(ObscuredVector3 lhs, Vector3 rhs)
		{
			return lhs.InternalDecrypt() != rhs;
		}
	}
}
