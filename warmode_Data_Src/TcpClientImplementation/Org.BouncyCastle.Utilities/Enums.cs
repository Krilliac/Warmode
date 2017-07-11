using Org.BouncyCastle.Utilities.Date;
using System;

namespace Org.BouncyCastle.Utilities
{
	internal abstract class Enums
	{
		internal static Enum GetEnumValue(Type enumType, string s)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Not an enumeration type", "enumType");
			}
			if (s.Length > 0 && char.IsLetter(s[0]) && s.IndexOf(',') < 0)
			{
				s = s.Replace('-', '_');
				s = s.Replace('/', '_');
				return (Enum)Enum.Parse(enumType, s, false);
			}
			throw new ArgumentException();
		}

		internal static Array GetEnumValues(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Not an enumeration type", "enumType");
			}
			return Enum.GetValues(enumType);
		}

		internal static Enum GetArbitraryValue(Type enumType)
		{
			Array enumValues = Enums.GetEnumValues(enumType);
			int index = (int)(DateTimeUtilities.CurrentUnixMs() & 2147483647L) % enumValues.Length;
			return (Enum)enumValues.GetValue(index);
		}
	}
}
