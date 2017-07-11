using CodeStage.AntiCheat.Detectors;
using System;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredQuaternion
	{
		[Serializable]
		public struct RawEncryptedQuaternion
		{
			public int x;

			public int y;

			public int z;

			public int w;
		}

		private static int cryptoKey = 120205;

		private static readonly Quaternion initialFakeValue = Quaternion.identity;

		[SerializeField]
		private int currentCryptoKey;

		[SerializeField]
		private ObscuredQuaternion.RawEncryptedQuaternion hiddenValue;

		[SerializeField]
		private Quaternion fakeValue;

		[SerializeField]
		private bool inited;

		private ObscuredQuaternion(ObscuredQuaternion.RawEncryptedQuaternion value)
		{
			this.currentCryptoKey = ObscuredQuaternion.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = ObscuredQuaternion.initialFakeValue;
			this.inited = true;
		}

		public static void SetNewCryptoKey(int newKey)
		{
			ObscuredQuaternion.cryptoKey = newKey;
		}

		public static ObscuredQuaternion.RawEncryptedQuaternion Encrypt(Quaternion value)
		{
			return ObscuredQuaternion.Encrypt(value, 0);
		}

		public static ObscuredQuaternion.RawEncryptedQuaternion Encrypt(Quaternion value, int key)
		{
			if (key == 0)
			{
				key = ObscuredQuaternion.cryptoKey;
			}
			ObscuredQuaternion.RawEncryptedQuaternion result;
			result.x = ObscuredFloat.Encrypt(value.x, key);
			result.y = ObscuredFloat.Encrypt(value.y, key);
			result.z = ObscuredFloat.Encrypt(value.z, key);
			result.w = ObscuredFloat.Encrypt(value.w, key);
			return result;
		}

		public static Quaternion Decrypt(ObscuredQuaternion.RawEncryptedQuaternion value)
		{
			return ObscuredQuaternion.Decrypt(value, 0);
		}

		public static Quaternion Decrypt(ObscuredQuaternion.RawEncryptedQuaternion value, int key)
		{
			if (key == 0)
			{
				key = ObscuredQuaternion.cryptoKey;
			}
			Quaternion result;
			result.x = ObscuredFloat.Decrypt(value.x, key);
			result.y = ObscuredFloat.Decrypt(value.y, key);
			result.z = ObscuredFloat.Decrypt(value.z, key);
			result.w = ObscuredFloat.Decrypt(value.w, key);
			return result;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredQuaternion.cryptoKey)
			{
				this.hiddenValue = ObscuredQuaternion.Encrypt(this.InternalDecrypt(), ObscuredQuaternion.cryptoKey);
				this.currentCryptoKey = ObscuredQuaternion.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			Quaternion value = this.InternalDecrypt();
			this.currentCryptoKey = UnityEngine.Random.Range(-2147483648, 2147483647);
			this.hiddenValue = ObscuredQuaternion.Encrypt(value, this.currentCryptoKey);
		}

		public ObscuredQuaternion.RawEncryptedQuaternion GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			return this.hiddenValue;
		}

		public void SetEncrypted(ObscuredQuaternion.RawEncryptedQuaternion encrypted)
		{
			this.inited = true;
			this.hiddenValue = encrypted;
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private Quaternion InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredQuaternion.cryptoKey;
				this.hiddenValue = ObscuredQuaternion.Encrypt(ObscuredQuaternion.initialFakeValue);
				this.fakeValue = ObscuredQuaternion.initialFakeValue;
				this.inited = true;
			}
			Quaternion quaternion;
			quaternion.x = ObscuredFloat.Decrypt(this.hiddenValue.x, this.currentCryptoKey);
			quaternion.y = ObscuredFloat.Decrypt(this.hiddenValue.y, this.currentCryptoKey);
			quaternion.z = ObscuredFloat.Decrypt(this.hiddenValue.z, this.currentCryptoKey);
			quaternion.w = ObscuredFloat.Decrypt(this.hiddenValue.w, this.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning && !this.fakeValue.Equals(ObscuredQuaternion.initialFakeValue) && !this.CompareQuaternionsWithTolerance(quaternion, this.fakeValue))
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return quaternion;
		}

		private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
		{
			float quaternionEpsilon = ObscuredCheatingDetector.Instance.quaternionEpsilon;
			return Math.Abs(q1.x - q2.x) < quaternionEpsilon && Math.Abs(q1.y - q2.y) < quaternionEpsilon && Math.Abs(q1.z - q2.z) < quaternionEpsilon && Math.Abs(q1.w - q2.w) < quaternionEpsilon;
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

		public static implicit operator ObscuredQuaternion(Quaternion value)
		{
			ObscuredQuaternion result = new ObscuredQuaternion(ObscuredQuaternion.Encrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator Quaternion(ObscuredQuaternion value)
		{
			return value.InternalDecrypt();
		}
	}
}
