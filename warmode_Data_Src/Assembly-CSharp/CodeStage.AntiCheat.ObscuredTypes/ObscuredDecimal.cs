using CodeStage.AntiCheat.Detectors;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	[Serializable]
	public struct ObscuredDecimal : IEquatable<ObscuredDecimal>, IFormattable
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct DecimalLongBytesUnion
		{
			[FieldOffset(0)]
			public decimal d;

			[FieldOffset(0)]
			public long l1;

			[FieldOffset(8)]
			public long l2;

			[FieldOffset(0)]
			public byte b1;

			[FieldOffset(1)]
			public byte b2;

			[FieldOffset(2)]
			public byte b3;

			[FieldOffset(3)]
			public byte b4;

			[FieldOffset(4)]
			public byte b5;

			[FieldOffset(5)]
			public byte b6;

			[FieldOffset(6)]
			public byte b7;

			[FieldOffset(7)]
			public byte b8;

			[FieldOffset(8)]
			public byte b9;

			[FieldOffset(9)]
			public byte b10;

			[FieldOffset(10)]
			public byte b11;

			[FieldOffset(11)]
			public byte b12;

			[FieldOffset(12)]
			public byte b13;

			[FieldOffset(13)]
			public byte b14;

			[FieldOffset(14)]
			public byte b15;

			[FieldOffset(15)]
			public byte b16;
		}

		private static long cryptoKey = 209208L;

		private long currentCryptoKey;

		private byte[] hiddenValue;

		private decimal fakeValue;

		private bool inited;

		private ObscuredDecimal(byte[] value)
		{
			this.currentCryptoKey = ObscuredDecimal.cryptoKey;
			this.hiddenValue = value;
			this.fakeValue = 0m;
			this.inited = true;
		}

		public static void SetNewCryptoKey(long newKey)
		{
			ObscuredDecimal.cryptoKey = newKey;
		}

		public static decimal Encrypt(decimal value)
		{
			return ObscuredDecimal.Encrypt(value, ObscuredDecimal.cryptoKey);
		}

		public static decimal Encrypt(decimal value, long key)
		{
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = value;
			decimalLongBytesUnion.l1 ^= key;
			decimalLongBytesUnion.l2 ^= key;
			return decimalLongBytesUnion.d;
		}

		private static byte[] InternalEncrypt(decimal value)
		{
			return ObscuredDecimal.InternalEncrypt(value, 0L);
		}

		private static byte[] InternalEncrypt(decimal value, long key)
		{
			long num = key;
			if (num == 0L)
			{
				num = ObscuredDecimal.cryptoKey;
			}
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = value;
			decimalLongBytesUnion.l1 ^= num;
			decimalLongBytesUnion.l2 ^= num;
			return new byte[]
			{
				decimalLongBytesUnion.b1,
				decimalLongBytesUnion.b2,
				decimalLongBytesUnion.b3,
				decimalLongBytesUnion.b4,
				decimalLongBytesUnion.b5,
				decimalLongBytesUnion.b6,
				decimalLongBytesUnion.b7,
				decimalLongBytesUnion.b8,
				decimalLongBytesUnion.b9,
				decimalLongBytesUnion.b10,
				decimalLongBytesUnion.b11,
				decimalLongBytesUnion.b12,
				decimalLongBytesUnion.b13,
				decimalLongBytesUnion.b14,
				decimalLongBytesUnion.b15,
				decimalLongBytesUnion.b16
			};
		}

		public static decimal Decrypt(decimal value)
		{
			return ObscuredDecimal.Decrypt(value, ObscuredDecimal.cryptoKey);
		}

		public static decimal Decrypt(decimal value, long key)
		{
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = value;
			decimalLongBytesUnion.l1 ^= key;
			decimalLongBytesUnion.l2 ^= key;
			return decimalLongBytesUnion.d;
		}

		public void ApplyNewCryptoKey()
		{
			if (this.currentCryptoKey != ObscuredDecimal.cryptoKey)
			{
				this.hiddenValue = ObscuredDecimal.InternalEncrypt(this.InternalDecrypt(), ObscuredDecimal.cryptoKey);
				this.currentCryptoKey = ObscuredDecimal.cryptoKey;
			}
		}

		public void RandomizeCryptoKey()
		{
			decimal value = this.InternalDecrypt();
			this.currentCryptoKey = (long)UnityEngine.Random.Range(-2147483648, 2147483647);
			this.hiddenValue = ObscuredDecimal.InternalEncrypt(value, this.currentCryptoKey);
		}

		public decimal GetEncrypted()
		{
			this.ApplyNewCryptoKey();
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.b1 = this.hiddenValue[0];
			decimalLongBytesUnion.b2 = this.hiddenValue[1];
			decimalLongBytesUnion.b3 = this.hiddenValue[2];
			decimalLongBytesUnion.b4 = this.hiddenValue[3];
			decimalLongBytesUnion.b5 = this.hiddenValue[4];
			decimalLongBytesUnion.b6 = this.hiddenValue[5];
			decimalLongBytesUnion.b7 = this.hiddenValue[6];
			decimalLongBytesUnion.b8 = this.hiddenValue[7];
			decimalLongBytesUnion.b9 = this.hiddenValue[8];
			decimalLongBytesUnion.b10 = this.hiddenValue[9];
			decimalLongBytesUnion.b11 = this.hiddenValue[10];
			decimalLongBytesUnion.b12 = this.hiddenValue[11];
			decimalLongBytesUnion.b13 = this.hiddenValue[12];
			decimalLongBytesUnion.b14 = this.hiddenValue[13];
			decimalLongBytesUnion.b15 = this.hiddenValue[14];
			decimalLongBytesUnion.b16 = this.hiddenValue[15];
			return decimalLongBytesUnion.d;
		}

		public void SetEncrypted(decimal encrypted)
		{
			this.inited = true;
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.d = encrypted;
			this.hiddenValue = new byte[]
			{
				decimalLongBytesUnion.b1,
				decimalLongBytesUnion.b2,
				decimalLongBytesUnion.b3,
				decimalLongBytesUnion.b4,
				decimalLongBytesUnion.b5,
				decimalLongBytesUnion.b6,
				decimalLongBytesUnion.b7,
				decimalLongBytesUnion.b8,
				decimalLongBytesUnion.b9,
				decimalLongBytesUnion.b10,
				decimalLongBytesUnion.b11,
				decimalLongBytesUnion.b12,
				decimalLongBytesUnion.b13,
				decimalLongBytesUnion.b14,
				decimalLongBytesUnion.b15,
				decimalLongBytesUnion.b16
			};
			if (ObscuredCheatingDetector.IsRunning)
			{
				this.fakeValue = this.InternalDecrypt();
			}
		}

		private decimal InternalDecrypt()
		{
			if (!this.inited)
			{
				this.currentCryptoKey = ObscuredDecimal.cryptoKey;
				this.hiddenValue = ObscuredDecimal.InternalEncrypt(0m);
				this.fakeValue = 0m;
				this.inited = true;
			}
			ObscuredDecimal.DecimalLongBytesUnion decimalLongBytesUnion = default(ObscuredDecimal.DecimalLongBytesUnion);
			decimalLongBytesUnion.b1 = this.hiddenValue[0];
			decimalLongBytesUnion.b2 = this.hiddenValue[1];
			decimalLongBytesUnion.b3 = this.hiddenValue[2];
			decimalLongBytesUnion.b4 = this.hiddenValue[3];
			decimalLongBytesUnion.b5 = this.hiddenValue[4];
			decimalLongBytesUnion.b6 = this.hiddenValue[5];
			decimalLongBytesUnion.b7 = this.hiddenValue[6];
			decimalLongBytesUnion.b8 = this.hiddenValue[7];
			decimalLongBytesUnion.b9 = this.hiddenValue[8];
			decimalLongBytesUnion.b10 = this.hiddenValue[9];
			decimalLongBytesUnion.b11 = this.hiddenValue[10];
			decimalLongBytesUnion.b12 = this.hiddenValue[11];
			decimalLongBytesUnion.b13 = this.hiddenValue[12];
			decimalLongBytesUnion.b14 = this.hiddenValue[13];
			decimalLongBytesUnion.b15 = this.hiddenValue[14];
			decimalLongBytesUnion.b16 = this.hiddenValue[15];
			decimalLongBytesUnion.l1 ^= this.currentCryptoKey;
			decimalLongBytesUnion.l2 ^= this.currentCryptoKey;
			decimal d = decimalLongBytesUnion.d;
			if (ObscuredCheatingDetector.IsRunning && this.fakeValue != 0m && d != this.fakeValue)
			{
				ObscuredCheatingDetector.Instance.OnCheatingDetected();
			}
			return d;
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

		public override bool Equals(object obj)
		{
			return obj is ObscuredDecimal && this.Equals((ObscuredDecimal)obj);
		}

		public bool Equals(ObscuredDecimal obj)
		{
			return obj.InternalDecrypt().Equals(this.InternalDecrypt());
		}

		public override int GetHashCode()
		{
			return this.InternalDecrypt().GetHashCode();
		}

		public static implicit operator ObscuredDecimal(decimal value)
		{
			ObscuredDecimal result = new ObscuredDecimal(ObscuredDecimal.InternalEncrypt(value));
			if (ObscuredCheatingDetector.IsRunning)
			{
				result.fakeValue = value;
			}
			return result;
		}

		public static implicit operator decimal(ObscuredDecimal value)
		{
			return value.InternalDecrypt();
		}

		public static explicit operator ObscuredDecimal(ObscuredFloat f)
		{
			return (decimal)f;
		}

		public static ObscuredDecimal operator ++(ObscuredDecimal input)
		{
			decimal value = input.InternalDecrypt() + 1m;
			input.hiddenValue = ObscuredDecimal.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}

		public static ObscuredDecimal operator --(ObscuredDecimal input)
		{
			decimal value = input.InternalDecrypt() - 1m;
			input.hiddenValue = ObscuredDecimal.InternalEncrypt(value, input.currentCryptoKey);
			if (ObscuredCheatingDetector.IsRunning)
			{
				input.fakeValue = value;
			}
			return input;
		}
	}
}
