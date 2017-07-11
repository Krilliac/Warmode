using CodeStage.AntiCheat.Utils;
using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AntiCheat.ObscuredTypes
{
	public static class ObscuredPrefs
	{
		internal enum DataType : byte
		{
			Unknown,
			Int = 5,
			UInt = 10,
			String = 15,
			Float = 20,
			Double = 25,
			Long = 30,
			Bool = 35,
			ByteArray = 40,
			Vector2 = 45,
			Vector3 = 50,
			Quaternion = 55,
			Color = 60,
			Rect = 65
		}

		public enum DeviceLockLevel : byte
		{
			None,
			Soft,
			Strict
		}

		private const byte VERSION = 2;

		private const string RAW_NOT_FOUND = "{not_found}";

		private const string DATA_SEPARATOR = "|";

		private const char DEPRECATED_RAW_SEPARATOR = ':';

		private static bool foreignSavesReported;

		private static string cryptoKey = "e806f6";

		private static string deviceId;

		private static uint deviceIdHash;

		public static Action onAlterationDetected;

		public static bool preservePlayerPrefs;

		public static Action onPossibleForeignSavesDetected;

		public static ObscuredPrefs.DeviceLockLevel lockToDevice;

		public static bool readForeignSaves;

		public static bool emergencyMode;

		private static string deprecatedDeviceId;

		public static string CryptoKey
		{
			get
			{
				return ObscuredPrefs.cryptoKey;
			}
			set
			{
				ObscuredPrefs.cryptoKey = value;
			}
		}

		public static string DeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(ObscuredPrefs.deviceId))
				{
					ObscuredPrefs.deviceId = ObscuredPrefs.GetDeviceId();
				}
				return ObscuredPrefs.deviceId;
			}
			set
			{
				ObscuredPrefs.deviceId = value;
			}
		}

		[Obsolete("This property is obsolete, please use DeviceId instead.")]
		internal static string DeviceID
		{
			get
			{
				return ObscuredPrefs.DeviceId;
			}
			set
			{
				ObscuredPrefs.DeviceId = value;
			}
		}

		private static uint DeviceIdHash
		{
			get
			{
				if (ObscuredPrefs.deviceIdHash == 0u)
				{
					ObscuredPrefs.deviceIdHash = ObscuredPrefs.CalculateChecksum(ObscuredPrefs.DeviceId);
				}
				return ObscuredPrefs.deviceIdHash;
			}
		}

		private static string DeprecatedDeviceId
		{
			get
			{
				if (string.IsNullOrEmpty(ObscuredPrefs.deprecatedDeviceId))
				{
					ObscuredPrefs.deprecatedDeviceId = ObscuredPrefs.DeprecatedCalculateChecksum(ObscuredPrefs.DeviceId);
				}
				return ObscuredPrefs.deprecatedDeviceId;
			}
		}

		public static void ForceLockToDeviceInit()
		{
			if (string.IsNullOrEmpty(ObscuredPrefs.deviceId))
			{
				ObscuredPrefs.deviceId = ObscuredPrefs.GetDeviceId();
				ObscuredPrefs.deviceIdHash = ObscuredPrefs.CalculateChecksum(ObscuredPrefs.deviceId);
			}
			else
			{
				Debug.LogWarning("[ACTk] ObscuredPrefs.ForceLockToDeviceInit() is called, but device ID is already obtained!");
			}
		}

		[Obsolete("This method is obsolete, use property CryptoKey instead")]
		internal static void SetNewCryptoKey(string newKey)
		{
			ObscuredPrefs.CryptoKey = newKey;
		}

		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptIntValue(key, value));
		}

		public static int GetInt(string key)
		{
			return ObscuredPrefs.GetInt(key, 0);
		}

		public static int GetInt(string key, int defaultValue)
		{
			string text = ObscuredPrefs.EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				int @int = PlayerPrefs.GetInt(key, defaultValue);
				if (!ObscuredPrefs.preservePlayerPrefs)
				{
					ObscuredPrefs.SetInt(key, @int);
					PlayerPrefs.DeleteKey(key);
				}
				return @int;
			}
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptIntValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		internal static string EncryptIntValue(string key, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Int);
		}

		internal static int DecryptIntValue(string key, string encryptedInput, int defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				int num;
				int.TryParse(text, out num);
				ObscuredPrefs.SetInt(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToInt32(array, 0);
			}
		}

		public static void SetUInt(string key, uint value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptUIntValue(key, value));
		}

		public static uint GetUInt(string key)
		{
			return ObscuredPrefs.GetUInt(key, 0u);
		}

		public static uint GetUInt(string key, uint defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptUIntValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptUIntValue(string key, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.UInt);
		}

		private static uint DecryptUIntValue(string key, string encryptedInput, uint defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				uint num;
				uint.TryParse(text, out num);
				ObscuredPrefs.SetUInt(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToUInt32(array, 0);
			}
		}

		public static void SetString(string key, string value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptStringValue(key, value));
		}

		public static string GetString(string key)
		{
			return ObscuredPrefs.GetString(key, string.Empty);
		}

		public static string GetString(string key, string defaultValue)
		{
			string text = ObscuredPrefs.EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				string @string = PlayerPrefs.GetString(key, defaultValue);
				if (!ObscuredPrefs.preservePlayerPrefs)
				{
					ObscuredPrefs.SetString(key, @string);
					PlayerPrefs.DeleteKey(key);
				}
				return @string;
			}
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptStringValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		internal static string EncryptStringValue(string key, string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.String);
		}

		internal static string DecryptStringValue(string key, string encryptedInput, string defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				ObscuredPrefs.SetString(key, text);
				return text;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return Encoding.UTF8.GetString(array, 0, array.Length);
			}
		}

		public static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptFloatValue(key, value));
		}

		public static float GetFloat(string key)
		{
			return ObscuredPrefs.GetFloat(key, 0f);
		}

		public static float GetFloat(string key, float defaultValue)
		{
			string text = ObscuredPrefs.EncryptKey(key);
			if (!PlayerPrefs.HasKey(text) && PlayerPrefs.HasKey(key))
			{
				float @float = PlayerPrefs.GetFloat(key, defaultValue);
				if (!ObscuredPrefs.preservePlayerPrefs)
				{
					ObscuredPrefs.SetFloat(key, @float);
					PlayerPrefs.DeleteKey(key);
				}
				return @float;
			}
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, text);
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptFloatValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		internal static string EncryptFloatValue(string key, float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Float);
		}

		internal static float DecryptFloatValue(string key, string encryptedInput, float defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				float num;
				float.TryParse(text, out num);
				ObscuredPrefs.SetFloat(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToSingle(array, 0);
			}
		}

		public static void SetDouble(string key, double value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptDoubleValue(key, value));
		}

		public static double GetDouble(string key)
		{
			return ObscuredPrefs.GetDouble(key, 0.0);
		}

		public static double GetDouble(string key, double defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptDoubleValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptDoubleValue(string key, double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Double);
		}

		private static double DecryptDoubleValue(string key, string encryptedInput, double defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				double num;
				double.TryParse(text, out num);
				ObscuredPrefs.SetDouble(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToDouble(array, 0);
			}
		}

		public static void SetLong(string key, long value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptLongValue(key, value));
		}

		public static long GetLong(string key)
		{
			return ObscuredPrefs.GetLong(key, 0L);
		}

		public static long GetLong(string key, long defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptLongValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptLongValue(string key, long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Long);
		}

		private static long DecryptLongValue(string key, string encryptedInput, long defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				long num;
				long.TryParse(text, out num);
				ObscuredPrefs.SetLong(key, num);
				return num;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToInt64(array, 0);
			}
		}

		public static void SetBool(string key, bool value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptBoolValue(key, value));
		}

		public static bool GetBool(string key)
		{
			return ObscuredPrefs.GetBool(key, false);
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptBoolValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptBoolValue(string key, bool value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Bool);
		}

		private static bool DecryptBoolValue(string key, string encryptedInput, bool defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				int num;
				int.TryParse(text, out num);
				ObscuredPrefs.SetBool(key, num == 1);
				return num == 1;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return defaultValue;
				}
				return BitConverter.ToBoolean(array, 0);
			}
		}

		public static void SetByteArray(string key, byte[] value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptByteArrayValue(key, value));
		}

		public static byte[] GetByteArray(string key)
		{
			return ObscuredPrefs.GetByteArray(key, 0, 0);
		}

		public static byte[] GetByteArray(string key, byte defaultValue, int defaultLength)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			if (encryptedPrefsString == "{not_found}")
			{
				return ObscuredPrefs.ConstructByteArray(defaultValue, defaultLength);
			}
			return ObscuredPrefs.DecryptByteArrayValue(key, encryptedPrefsString, defaultValue, defaultLength);
		}

		private static string EncryptByteArrayValue(string key, byte[] value)
		{
			return ObscuredPrefs.EncryptData(key, value, ObscuredPrefs.DataType.ByteArray);
		}

		private static byte[] DecryptByteArrayValue(string key, string encryptedInput, byte defaultValue, int defaultLength)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return ObscuredPrefs.ConstructByteArray(defaultValue, defaultLength);
				}
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				ObscuredPrefs.SetByteArray(key, bytes);
				return bytes;
			}
			else
			{
				byte[] array = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array == null)
				{
					return ObscuredPrefs.ConstructByteArray(defaultValue, defaultLength);
				}
				return array;
			}
		}

		private static byte[] ConstructByteArray(byte value, int length)
		{
			byte[] array = new byte[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = value;
			}
			return array;
		}

		public static void SetVector2(string key, Vector2 value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptVector2Value(key, value));
		}

		public static Vector2 GetVector2(string key)
		{
			return ObscuredPrefs.GetVector2(key, Vector2.zero);
		}

		public static Vector2 GetVector2(string key, Vector2 defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptVector2Value(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptVector2Value(string key, Vector2 value)
		{
			byte[] array = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Vector2);
		}

		private static Vector2 DecryptVector2Value(string key, string encryptedInput, Vector2 defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				Vector2 vector = new Vector2(x, y);
				ObscuredPrefs.SetVector2(key, vector);
				return vector;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				Vector2 result;
				result.x = BitConverter.ToSingle(array2, 0);
				result.y = BitConverter.ToSingle(array2, 4);
				return result;
			}
		}

		public static void SetVector3(string key, Vector3 value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptVector3Value(key, value));
		}

		public static Vector3 GetVector3(string key)
		{
			return ObscuredPrefs.GetVector3(key, Vector3.zero);
		}

		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptVector3Value(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptVector3Value(string key, Vector3 value)
		{
			byte[] array = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, array, 8, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Vector3);
		}

		private static Vector3 DecryptVector3Value(string key, string encryptedInput, Vector3 defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				float z;
				float.TryParse(array[2], out z);
				Vector3 vector = new Vector3(x, y, z);
				ObscuredPrefs.SetVector3(key, vector);
				return vector;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				Vector3 result;
				result.x = BitConverter.ToSingle(array2, 0);
				result.y = BitConverter.ToSingle(array2, 4);
				result.z = BitConverter.ToSingle(array2, 8);
				return result;
			}
		}

		public static void SetQuaternion(string key, Quaternion value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptQuaternionValue(key, value));
		}

		public static Quaternion GetQuaternion(string key)
		{
			return ObscuredPrefs.GetQuaternion(key, Quaternion.identity);
		}

		public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptQuaternionValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptQuaternionValue(string key, Quaternion value)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.w), 0, array, 12, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Quaternion);
		}

		private static Quaternion DecryptQuaternionValue(string key, string encryptedInput, Quaternion defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				float z;
				float.TryParse(array[2], out z);
				float w;
				float.TryParse(array[3], out w);
				Quaternion quaternion = new Quaternion(x, y, z, w);
				ObscuredPrefs.SetQuaternion(key, quaternion);
				return quaternion;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				Quaternion result;
				result.x = BitConverter.ToSingle(array2, 0);
				result.y = BitConverter.ToSingle(array2, 4);
				result.z = BitConverter.ToSingle(array2, 8);
				result.w = BitConverter.ToSingle(array2, 12);
				return result;
			}
		}

		public static void SetColor(string key, Color32 value)
		{
			uint value2 = (uint)((int)value.a << 24 | (int)value.r << 16 | (int)value.g << 8 | (int)value.b);
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptColorValue(key, value2));
		}

		public static Color32 GetColor(string key)
		{
			return ObscuredPrefs.GetColor(key, new Color32(0, 0, 0, 1));
		}

		public static Color32 GetColor(string key, Color32 defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			if (encryptedPrefsString == "{not_found}")
			{
				return defaultValue;
			}
			uint num = ObscuredPrefs.DecryptUIntValue(key, encryptedPrefsString, 16777216u);
			byte a = (byte)(num >> 24);
			byte r = (byte)(num >> 16);
			byte g = (byte)(num >> 8);
			byte b = (byte)num;
			return new Color32(r, g, b, a);
		}

		private static string EncryptColorValue(string key, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return ObscuredPrefs.EncryptData(key, bytes, ObscuredPrefs.DataType.Color);
		}

		public static void SetRect(string key, Rect value)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptRectValue(key, value));
		}

		public static Rect GetRect(string key)
		{
			return ObscuredPrefs.GetRect(key, new Rect(0f, 0f, 0f, 0f));
		}

		public static Rect GetRect(string key, Rect defaultValue)
		{
			string encryptedPrefsString = ObscuredPrefs.GetEncryptedPrefsString(key, ObscuredPrefs.EncryptKey(key));
			return (!(encryptedPrefsString == "{not_found}")) ? ObscuredPrefs.DecryptRectValue(key, encryptedPrefsString, defaultValue) : defaultValue;
		}

		private static string EncryptRectValue(string key, Rect value)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.width), 0, array, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(value.height), 0, array, 12, 4);
			return ObscuredPrefs.EncryptData(key, array, ObscuredPrefs.DataType.Rect);
		}

		private static Rect DecryptRectValue(string key, string encryptedInput, Rect defaultValue)
		{
			if (encryptedInput.IndexOf(':') > -1)
			{
				string text = ObscuredPrefs.DeprecatedDecryptValue(encryptedInput);
				if (text == string.Empty)
				{
					return defaultValue;
				}
				string[] array = text.Split(new char[]
				{
					"|"[0]
				});
				float x;
				float.TryParse(array[0], out x);
				float y;
				float.TryParse(array[1], out y);
				float width;
				float.TryParse(array[2], out width);
				float height;
				float.TryParse(array[3], out height);
				Rect rect = new Rect(x, y, width, height);
				ObscuredPrefs.SetRect(key, rect);
				return rect;
			}
			else
			{
				byte[] array2 = ObscuredPrefs.DecryptData(key, encryptedInput);
				if (array2 == null)
				{
					return defaultValue;
				}
				return new Rect
				{
					x = BitConverter.ToSingle(array2, 0),
					y = BitConverter.ToSingle(array2, 4),
					width = BitConverter.ToSingle(array2, 8),
					height = BitConverter.ToSingle(array2, 12)
				};
			}
		}

		public static void SetRawValue(string key, string encryptedValue)
		{
			PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), encryptedValue);
		}

		public static string GetRawValue(string key)
		{
			string key2 = ObscuredPrefs.EncryptKey(key);
			return PlayerPrefs.GetString(key2);
		}

		internal static ObscuredPrefs.DataType GetRawValueType(string value)
		{
			ObscuredPrefs.DataType dataType = ObscuredPrefs.DataType.Unknown;
			byte[] array;
			try
			{
				array = Convert.FromBase64String(value);
			}
			catch (Exception)
			{
				ObscuredPrefs.DataType result = dataType;
				return result;
			}
			if (array.Length < 7)
			{
				return dataType;
			}
			int num = array.Length;
			dataType = (ObscuredPrefs.DataType)array[num - 7];
			return dataType;
		}

		internal static string EncryptKey(string key)
		{
			key = ObscuredString.EncryptDecrypt(key, ObscuredPrefs.cryptoKey);
			key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
			return key;
		}

		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key) || PlayerPrefs.HasKey(ObscuredPrefs.EncryptKey(key));
		}

		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(ObscuredPrefs.EncryptKey(key));
			if (!ObscuredPrefs.preservePlayerPrefs)
			{
				PlayerPrefs.DeleteKey(key);
			}
		}

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}

		private static string GetEncryptedPrefsString(string key, string encryptedKey)
		{
			string @string = PlayerPrefs.GetString(encryptedKey, "{not_found}");
			if (@string == "{not_found}" && PlayerPrefs.HasKey(key))
			{
				Debug.LogWarning("[ACTk] Are you trying to read regular PlayerPrefs data using ObscuredPrefs (key = " + key + ")?");
			}
			return @string;
		}

		private static string EncryptData(string key, byte[] cleanBytes, ObscuredPrefs.DataType type)
		{
			int num = cleanBytes.Length;
			byte[] src = ObscuredPrefs.EncryptDecryptBytes(cleanBytes, num, key + ObscuredPrefs.cryptoKey);
			uint num2 = xxHash.CalculateHash(cleanBytes, num, 0u);
			byte[] src2 = new byte[]
			{
				(byte)(num2 & 255u),
				(byte)(num2 >> 8 & 255u),
				(byte)(num2 >> 16 & 255u),
				(byte)(num2 >> 24 & 255u)
			};
			byte[] array = null;
			int num3;
			if (ObscuredPrefs.lockToDevice != ObscuredPrefs.DeviceLockLevel.None)
			{
				num3 = num + 11;
				uint num4 = ObscuredPrefs.DeviceIdHash;
				array = new byte[]
				{
					(byte)(num4 & 255u),
					(byte)(num4 >> 8 & 255u),
					(byte)(num4 >> 16 & 255u),
					(byte)(num4 >> 24 & 255u)
				};
			}
			else
			{
				num3 = num + 7;
			}
			byte[] array2 = new byte[num3];
			Buffer.BlockCopy(src, 0, array2, 0, num);
			if (array != null)
			{
				Buffer.BlockCopy(array, 0, array2, num, 4);
			}
			array2[num3 - 7] = (byte)type;
			array2[num3 - 6] = 2;
			array2[num3 - 5] = (byte)ObscuredPrefs.lockToDevice;
			Buffer.BlockCopy(src2, 0, array2, num3 - 4, 4);
			return Convert.ToBase64String(array2);
		}

		internal static byte[] DecryptData(string key, string encryptedInput)
		{
			byte[] array;
			try
			{
				array = Convert.FromBase64String(encryptedInput);
			}
			catch (Exception)
			{
				ObscuredPrefs.SavesTampered();
				byte[] result = null;
				return result;
			}
			if (array.Length <= 0)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			int num = array.Length;
			byte b = array[num - 6];
			if (b != 2)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			ObscuredPrefs.DeviceLockLevel deviceLockLevel = (ObscuredPrefs.DeviceLockLevel)array[num - 5];
			byte[] array2 = new byte[4];
			Buffer.BlockCopy(array, num - 4, array2, 0, 4);
			uint num2 = (uint)((int)array2[0] | (int)array2[1] << 8 | (int)array2[2] << 16 | (int)array2[3] << 24);
			uint num3 = 0u;
			int num4;
			if (deviceLockLevel != ObscuredPrefs.DeviceLockLevel.None)
			{
				num4 = num - 11;
				if (ObscuredPrefs.lockToDevice != ObscuredPrefs.DeviceLockLevel.None)
				{
					byte[] array3 = new byte[4];
					Buffer.BlockCopy(array, num4, array3, 0, 4);
					num3 = (uint)((int)array3[0] | (int)array3[1] << 8 | (int)array3[2] << 16 | (int)array3[3] << 24);
				}
			}
			else
			{
				num4 = num - 7;
			}
			byte[] array4 = new byte[num4];
			Buffer.BlockCopy(array, 0, array4, 0, num4);
			byte[] array5 = ObscuredPrefs.EncryptDecryptBytes(array4, num4, key + ObscuredPrefs.cryptoKey);
			uint num5 = xxHash.CalculateHash(array5, num4, 0u);
			if (num5 != num2)
			{
				ObscuredPrefs.SavesTampered();
				return null;
			}
			if (ObscuredPrefs.lockToDevice == ObscuredPrefs.DeviceLockLevel.Strict && num3 == 0u && !ObscuredPrefs.emergencyMode && !ObscuredPrefs.readForeignSaves)
			{
				return null;
			}
			if (num3 != 0u && !ObscuredPrefs.emergencyMode)
			{
				uint num6 = ObscuredPrefs.DeviceIdHash;
				if (num3 != num6)
				{
					ObscuredPrefs.PossibleForeignSavesDetected();
					if (!ObscuredPrefs.readForeignSaves)
					{
						return null;
					}
				}
			}
			return array5;
		}

		private static uint CalculateChecksum(string input)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(input + ObscuredPrefs.cryptoKey);
			return xxHash.CalculateHash(bytes, bytes.Length, 0u);
		}

		private static void SavesTampered()
		{
			if (ObscuredPrefs.onAlterationDetected != null)
			{
				ObscuredPrefs.onAlterationDetected();
				ObscuredPrefs.onAlterationDetected = null;
			}
		}

		private static void PossibleForeignSavesDetected()
		{
			if (ObscuredPrefs.onPossibleForeignSavesDetected != null && !ObscuredPrefs.foreignSavesReported)
			{
				ObscuredPrefs.foreignSavesReported = true;
				ObscuredPrefs.onPossibleForeignSavesDetected();
			}
		}

		private static string GetDeviceId()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(text))
			{
				text = SystemInfo.deviceUniqueIdentifier;
			}
			return text;
		}

		private static byte[] EncryptDecryptBytes(byte[] bytes, int dataLength, string key)
		{
			int length = key.Length;
			byte[] array = new byte[dataLength];
			for (int i = 0; i < dataLength; i++)
			{
				array[i] = (byte)((char)bytes[i] ^ key[i % length]);
			}
			return array;
		}

		private static string DeprecatedDecryptValue(string value)
		{
			string[] array = value.Split(new char[]
			{
				':'
			});
			if (array.Length < 2)
			{
				ObscuredPrefs.SavesTampered();
				return string.Empty;
			}
			string text = array[0];
			string a = array[1];
			byte[] array2;
			try
			{
				array2 = Convert.FromBase64String(text);
			}
			catch
			{
				ObscuredPrefs.SavesTampered();
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(array2, 0, array2.Length);
			string result = ObscuredString.EncryptDecrypt(@string, ObscuredPrefs.cryptoKey);
			if (array.Length == 3)
			{
				if (a != ObscuredPrefs.DeprecatedCalculateChecksum(text + ObscuredPrefs.DeprecatedDeviceId))
				{
					ObscuredPrefs.SavesTampered();
				}
			}
			else if (array.Length == 2)
			{
				if (a != ObscuredPrefs.DeprecatedCalculateChecksum(text))
				{
					ObscuredPrefs.SavesTampered();
				}
			}
			else
			{
				ObscuredPrefs.SavesTampered();
			}
			if (ObscuredPrefs.lockToDevice != ObscuredPrefs.DeviceLockLevel.None && !ObscuredPrefs.emergencyMode)
			{
				if (array.Length >= 3)
				{
					string a2 = array[2];
					if (a2 != ObscuredPrefs.DeprecatedDeviceId)
					{
						if (!ObscuredPrefs.readForeignSaves)
						{
							result = string.Empty;
						}
						ObscuredPrefs.PossibleForeignSavesDetected();
					}
				}
				else if (ObscuredPrefs.lockToDevice == ObscuredPrefs.DeviceLockLevel.Strict)
				{
					if (!ObscuredPrefs.readForeignSaves)
					{
						result = string.Empty;
					}
					ObscuredPrefs.PossibleForeignSavesDetected();
				}
				else if (a != ObscuredPrefs.DeprecatedCalculateChecksum(text))
				{
					if (!ObscuredPrefs.readForeignSaves)
					{
						result = string.Empty;
					}
					ObscuredPrefs.PossibleForeignSavesDetected();
				}
			}
			return result;
		}

		private static string DeprecatedCalculateChecksum(string input)
		{
			int num = 0;
			byte[] bytes = Encoding.UTF8.GetBytes(input + ObscuredPrefs.cryptoKey);
			int num2 = bytes.Length;
			int num3 = ObscuredPrefs.cryptoKey.Length ^ 64;
			for (int i = 0; i < num2; i++)
			{
				byte b = bytes[i];
				num += (int)b + (int)b * (i + num3) % 3;
			}
			return num.ToString("X2");
		}
	}
}
