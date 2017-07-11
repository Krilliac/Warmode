using System;

namespace UnityEngine.Networking
{
	internal class FloatConversion
	{
		public static float ToSingle(uint value)
		{
			UIntFloat uIntFloat = default(UIntFloat);
			uIntFloat.intValue = value;
			return uIntFloat.floatValue;
		}

		public static double ToDouble(ulong value)
		{
			UIntFloat uIntFloat = default(UIntFloat);
			uIntFloat.longValue = value;
			return uIntFloat.doubleValue;
		}
	}
}
