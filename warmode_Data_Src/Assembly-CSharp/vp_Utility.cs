using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class vp_Utility
{
	private static readonly Dictionary<Type, string> m_TypeAliases = new Dictionary<Type, string>
	{
		{
			typeof(void),
			"void"
		},
		{
			typeof(byte),
			"byte"
		},
		{
			typeof(sbyte),
			"sbyte"
		},
		{
			typeof(short),
			"short"
		},
		{
			typeof(ushort),
			"ushort"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(uint),
			"uint"
		},
		{
			typeof(long),
			"long"
		},
		{
			typeof(ulong),
			"ulong"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(object),
			"object"
		},
		{
			typeof(bool),
			"bool"
		},
		{
			typeof(char),
			"char"
		},
		{
			typeof(string),
			"string"
		},
		{
			typeof(Vector2),
			"Vector2"
		},
		{
			typeof(Vector3),
			"Vector3"
		},
		{
			typeof(Vector4),
			"Vector4"
		}
	};

	public static float NaNSafeFloat(float value, float prevValue = 0f)
	{
		value = ((!double.IsNaN((double)value)) ? value : prevValue);
		return value;
	}

	public static Vector2 NaNSafeVector2(Vector2 vector, Vector2 prevVector = default(Vector2))
	{
		vector.x = ((!double.IsNaN((double)vector.x)) ? vector.x : prevVector.x);
		vector.y = ((!double.IsNaN((double)vector.y)) ? vector.y : prevVector.y);
		return vector;
	}

	public static Vector3 NaNSafeVector3(Vector3 vector, Vector3 prevVector = default(Vector3))
	{
		vector.x = ((!double.IsNaN((double)vector.x)) ? vector.x : prevVector.x);
		vector.y = ((!double.IsNaN((double)vector.y)) ? vector.y : prevVector.y);
		vector.z = ((!double.IsNaN((double)vector.z)) ? vector.z : prevVector.z);
		return vector;
	}

	public static Quaternion NaNSafeQuaternion(Quaternion quaternion, Quaternion prevQuaternion = default(Quaternion))
	{
		quaternion.x = ((!double.IsNaN((double)quaternion.x)) ? quaternion.x : prevQuaternion.x);
		quaternion.y = ((!double.IsNaN((double)quaternion.y)) ? quaternion.y : prevQuaternion.y);
		quaternion.z = ((!double.IsNaN((double)quaternion.z)) ? quaternion.z : prevQuaternion.z);
		quaternion.w = ((!double.IsNaN((double)quaternion.w)) ? quaternion.w : prevQuaternion.w);
		return quaternion;
	}

	public static Vector3 SnapToZero(Vector3 value, float epsilon = 0.0001f)
	{
		value.x = ((Mathf.Abs(value.x) >= epsilon) ? value.x : 0f);
		value.y = ((Mathf.Abs(value.y) >= epsilon) ? value.y : 0f);
		value.z = ((Mathf.Abs(value.z) >= epsilon) ? value.z : 0f);
		return value;
	}

	public static Vector3 HorizontalVector(Vector3 value)
	{
		value.y = 0f;
		return value;
	}

	public static string GetErrorLocation(int level = 1)
	{
		StackTrace stackTrace = new StackTrace();
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = stackTrace.FrameCount - 1; i > level; i--)
		{
			if (i < stackTrace.FrameCount - 1)
			{
				text += " --> ";
			}
			StackFrame frame = stackTrace.GetFrame(i);
			if (frame.GetMethod().DeclaringType.ToString() == text2)
			{
				text = string.Empty;
			}
			text2 = frame.GetMethod().DeclaringType.ToString();
			text = text + text2 + ":" + frame.GetMethod().Name;
		}
		return text;
	}

	public static string GetTypeAlias(Type type)
	{
		string empty = string.Empty;
		if (!vp_Utility.m_TypeAliases.TryGetValue(type, out empty))
		{
			return type.ToString();
		}
		return empty;
	}

	public static void Activate(GameObject obj, bool activate = true)
	{
		obj.SetActiveRecursively(activate);
	}

	public static bool IsActive(GameObject obj)
	{
		return obj.active;
	}

	public static void HideWeapon(GameObject obj, bool val)
	{
		Component[] componentsInChildren = obj.GetComponentsInChildren(typeof(Renderer));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = (Renderer)componentsInChildren[i];
			if (val)
			{
				renderer.gameObject.layer = 31;
			}
			else
			{
				renderer.gameObject.layer = 8;
			}
		}
	}
}
