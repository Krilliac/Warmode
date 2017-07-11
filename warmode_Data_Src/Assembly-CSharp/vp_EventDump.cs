using System;
using System.Collections.Generic;
using System.Reflection;

public class vp_EventDump
{
	public static string Dump(vp_EventHandler handler, string[] eventTypes)
	{
		string text = string.Empty;
		for (int i = 0; i < eventTypes.Length; i++)
		{
			string text2 = eventTypes[i];
			string text3 = text2;
			switch (text3)
			{
			case "vp_Message":
				text += vp_EventDump.DumpEventsOfType("vp_Message", (eventTypes.Length <= 1) ? string.Empty : "MESSAGES:\n\n", handler);
				break;
			case "vp_Attempt":
				text += vp_EventDump.DumpEventsOfType("vp_Attempt", (eventTypes.Length <= 1) ? string.Empty : "ATTEMPTS:\n\n", handler);
				break;
			case "vp_Value":
				text += vp_EventDump.DumpEventsOfType("vp_Value", (eventTypes.Length <= 1) ? string.Empty : "VALUES:\n\n", handler);
				break;
			case "vp_Activity":
				text += vp_EventDump.DumpEventsOfType("vp_Activity", (eventTypes.Length <= 1) ? string.Empty : "ACTIVITIES:\n\n", handler);
				break;
			}
		}
		return text;
	}

	private static string DumpEventsOfType(string type, string caption, vp_EventHandler handler)
	{
		string text = caption.ToUpper();
		FieldInfo[] fields = handler.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			string text2 = null;
			switch (type)
			{
			case "vp_Message":
				if (fieldInfo.FieldType.ToString().Contains("vp_Message"))
				{
					vp_Message e = (vp_Message)fieldInfo.GetValue(handler);
					text2 = vp_EventDump.DumpEventListeners(e, new string[]
					{
						"Send"
					});
				}
				break;
			case "vp_Attempt":
				if (fieldInfo.FieldType.ToString().Contains("vp_Attempt"))
				{
					vp_Event e2 = (vp_Event)fieldInfo.GetValue(handler);
					text2 = vp_EventDump.DumpEventListeners(e2, new string[]
					{
						"Try"
					});
				}
				break;
			case "vp_Value":
				if (fieldInfo.FieldType.ToString().Contains("vp_Value"))
				{
					vp_Event e3 = (vp_Event)fieldInfo.GetValue(handler);
					text2 = vp_EventDump.DumpEventListeners(e3, new string[]
					{
						"Get",
						"Set"
					});
				}
				break;
			case "vp_Activity":
				if (fieldInfo.FieldType.ToString().Contains("vp_Activity"))
				{
					vp_Event e4 = (vp_Event)fieldInfo.GetValue(handler);
					text2 = vp_EventDump.DumpEventListeners(e4, new string[]
					{
						"StartConditions",
						"StopConditions",
						"StartCallbacks",
						"StopCallbacks"
					});
				}
				break;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				string text3 = text;
				text = string.Concat(new string[]
				{
					text3,
					"\t\t",
					fieldInfo.Name,
					"\n",
					text2,
					"\n"
				});
			}
		}
		return text;
	}

	private static string DumpEventListeners(object e, string[] invokers)
	{
		Type type = e.GetType();
		string text = string.Empty;
		for (int i = 0; i < invokers.Length; i++)
		{
			string text2 = invokers[i];
			FieldInfo field = type.GetField(text2);
			if (field == null)
			{
				return string.Empty;
			}
			Delegate @delegate = (Delegate)field.GetValue(e);
			string[] methodNames = vp_EventDump.GetMethodNames(@delegate.GetInvocationList());
			text += "\t\t\t\t";
			if (type.ToString().Contains("vp_Value"))
			{
				string text3 = text2;
				if (text3 == null)
				{
					goto IL_F6;
				}
				if (vp_EventDump.<>f__switch$map8 == null)
				{
					vp_EventDump.<>f__switch$map8 = new Dictionary<string, int>(2)
					{
						{
							"Get",
							0
						},
						{
							"Set",
							1
						}
					};
				}
				int num;
				if (!vp_EventDump.<>f__switch$map8.TryGetValue(text3, out num))
				{
					goto IL_F6;
				}
				if (num != 0)
				{
					if (num != 1)
					{
						goto IL_F6;
					}
					text += "Set";
				}
				else
				{
					text += "Get";
				}
				goto IL_259;
				IL_F6:
				text += "Unsupported listener: ";
			}
			else if (type.ToString().Contains("vp_Attempt"))
			{
				text += "Try";
			}
			else if (type.ToString().Contains("vp_Message"))
			{
				text += "Send";
			}
			else if (type.ToString().Contains("vp_Activity"))
			{
				string text3 = text2;
				if (text3 == null)
				{
					goto IL_237;
				}
				if (vp_EventDump.<>f__switch$map9 == null)
				{
					vp_EventDump.<>f__switch$map9 = new Dictionary<string, int>(4)
					{
						{
							"StartConditions",
							0
						},
						{
							"StopConditions",
							1
						},
						{
							"StartCallbacks",
							2
						},
						{
							"StopCallbacks",
							3
						}
					};
				}
				int num;
				if (!vp_EventDump.<>f__switch$map9.TryGetValue(text3, out num))
				{
					goto IL_237;
				}
				switch (num)
				{
				case 0:
					text += "TryStart";
					break;
				case 1:
					text += "TryStop";
					break;
				case 2:
					text += "Start";
					break;
				case 3:
					text += "Stop";
					break;
				default:
					goto IL_237;
				}
				goto IL_259;
				IL_237:
				text += "Unsupported listener: ";
			}
			else
			{
				text += "Unsupported listener";
			}
			IL_259:
			if (methodNames.Length > 2)
			{
				text += ":\n";
			}
			else
			{
				text += ": ";
			}
			text += vp_EventDump.DumpDelegateNames(methodNames);
		}
		return text;
	}

	private static string[] GetMethodNames(Delegate[] list)
	{
		list = vp_EventDump.RemoveDelegatesFromList(list);
		string[] array = new string[list.Length];
		if (list.Length == 1)
		{
			array[0] = ((list[0].Target != null) ? ("(" + list[0].Target + ") ") : string.Empty) + list[0].Method.Name;
		}
		else
		{
			for (int i = 1; i < list.Length; i++)
			{
				array[i] = ((list[i].Target != null) ? ("(" + list[i].Target + ") ") : string.Empty) + list[i].Method.Name;
			}
		}
		return array;
	}

	private static Delegate[] RemoveDelegatesFromList(Delegate[] list)
	{
		List<Delegate> list2 = new List<Delegate>(list);
		for (int i = list2.Count - 1; i > -1; i--)
		{
			if (list2[i] != null)
			{
				if (list2[i].Method.Name.Contains("m_"))
				{
					list2.RemoveAt(i);
				}
			}
		}
		return list2.ToArray();
	}

	private static string DumpDelegateNames(string[] array)
	{
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + ((array.Length <= 2) ? string.Empty : "\t\t\t\t\t\t\t") + text2 + "\n";
			}
		}
		return text;
	}
}
