using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class vp_EventHandler : MonoBehaviour
{
	protected class ScriptMethods
	{
		public List<MethodInfo> Events = new List<MethodInfo>();

		public ScriptMethods(Type type)
		{
			this.Events = vp_EventHandler.ScriptMethods.GetMethods(type);
		}

		protected static List<MethodInfo> GetMethods(Type type)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			List<string> list2 = new List<string>();
			while (type != null)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < methods.Length; i++)
				{
					MethodInfo methodInfo = methods[i];
					if (!methodInfo.Name.Contains(">m__"))
					{
						if (!list2.Contains(methodInfo.Name))
						{
							string[] supportedPrefixes = vp_EventHandler.m_SupportedPrefixes;
							for (int j = 0; j < supportedPrefixes.Length; j++)
							{
								string value = supportedPrefixes[j];
								if (methodInfo.Name.Contains(value))
								{
									list.Add(methodInfo);
									list2.Add(methodInfo.Name);
									break;
								}
							}
						}
					}
				}
				type = type.BaseType;
			}
			return list;
		}
	}

	protected bool m_Initialized;

	protected Dictionary<string, vp_Event> m_HandlerEvents = new Dictionary<string, vp_Event>();

	protected List<object> m_PendingRegistrants = new List<object>();

	protected static Dictionary<Type, vp_EventHandler.ScriptMethods> m_StoredScriptTypes = new Dictionary<Type, vp_EventHandler.ScriptMethods>();

	protected static string[] m_SupportedPrefixes = new string[]
	{
		"OnMessage_",
		"CanStart_",
		"CanStop_",
		"OnStart_",
		"OnStop_",
		"OnAttempt_",
		"get_OnValue_",
		"set_OnValue_"
	};

	protected virtual void Awake()
	{
		this.StoreHandlerEvents();
		this.m_Initialized = true;
		for (int i = this.m_PendingRegistrants.Count - 1; i > -1; i--)
		{
			this.Register(this.m_PendingRegistrants[i]);
			this.m_PendingRegistrants.Remove(this.m_PendingRegistrants[i]);
		}
	}

	protected void StoreHandlerEvents()
	{
		FieldInfo[] fields = base.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < fields.Length; i++)
		{
			FieldInfo fieldInfo = fields[i];
			object obj = Activator.CreateInstance(fieldInfo.FieldType, new object[]
			{
				fieldInfo.Name
			});
			if (obj != null)
			{
				fieldInfo.SetValue(this, obj);
				foreach (string current in ((vp_Event)obj).Prefixes.Keys)
				{
					this.m_HandlerEvents.Add(current + fieldInfo.Name, (vp_Event)obj);
				}
			}
		}
	}

	public void Register(object target)
	{
		if (target == null)
		{
			Debug.LogError("Error: (" + this + ") Target object was null.");
			return;
		}
		if (!this.m_Initialized)
		{
			this.m_PendingRegistrants.Add(target);
			return;
		}
		vp_EventHandler.ScriptMethods scriptMethods = this.GetScriptMethods(target);
		if (scriptMethods == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				") could not get script methods for '",
				target,
				"'."
			}));
			return;
		}
		foreach (MethodInfo current in scriptMethods.Events)
		{
			vp_Event vp_Event;
			if (!this.m_HandlerEvents.TryGetValue(current.Name, out vp_Event))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Warning: (",
					current.DeclaringType,
					") Event handler can't register method '",
					current.Name,
					"' because '",
					base.GetType(),
					"' has not (successfully) registered any event named '",
					current.Name.Substring(current.Name.Substring(0, current.Name.IndexOf('_', 4) + 1).Length)
				}));
			}
			else
			{
				int num;
				vp_Event.Prefixes.TryGetValue(current.Name.Substring(0, current.Name.IndexOf('_', 4) + 1), out num);
				if (this.CompareMethodSignatures(current, vp_Event.GetParameterType(num), vp_Event.GetReturnType(num)))
				{
					vp_Event.Register(target, current.Name, num);
				}
			}
		}
	}

	public void Unregister(object target)
	{
		if (target == null)
		{
			Debug.LogError("Error: (" + this + ") Target object was null.");
			return;
		}
		foreach (vp_Event current in this.m_HandlerEvents.Values)
		{
			if (current != null)
			{
				string[] invokerFieldNames = current.InvokerFieldNames;
				for (int i = 0; i < invokerFieldNames.Length; i++)
				{
					string name = invokerFieldNames[i];
					Type type = current.GetType();
					FieldInfo field = type.GetField(name);
					if (field != null)
					{
						object value = field.GetValue(current);
						if (value != null)
						{
							Delegate @delegate = (Delegate)value;
							if (@delegate != null)
							{
								Delegate[] invocationList = @delegate.GetInvocationList();
								for (int j = 0; j < invocationList.Length; j++)
								{
									Delegate delegate2 = invocationList[j];
									if (delegate2.Target == target)
									{
										current.Unregister(target);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	protected bool CompareMethodSignatures(MethodInfo scriptMethod, Type handlerParameterType, Type handlerReturnType)
	{
		if (scriptMethod.ReturnType != handlerReturnType)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				scriptMethod.DeclaringType,
				") Return type (",
				vp_Utility.GetTypeAlias(scriptMethod.ReturnType),
				") is not valid for '",
				scriptMethod.Name,
				"'. Return type declared in event handler was: (",
				vp_Utility.GetTypeAlias(handlerReturnType),
				")."
			}));
			return false;
		}
		if (scriptMethod.GetParameters().Length == 1)
		{
			if (((ParameterInfo)scriptMethod.GetParameters().GetValue(0)).ParameterType != handlerParameterType)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Error: (",
					scriptMethod.DeclaringType,
					") Parameter type (",
					vp_Utility.GetTypeAlias(((ParameterInfo)scriptMethod.GetParameters().GetValue(0)).ParameterType),
					") is not valid for '",
					scriptMethod.Name,
					"'. Parameter type declared in event handler was: (",
					vp_Utility.GetTypeAlias(handlerParameterType),
					")."
				}));
				return false;
			}
		}
		else if (scriptMethod.GetParameters().Length == 0)
		{
			if (handlerParameterType != typeof(void))
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Error: (",
					scriptMethod.DeclaringType,
					") Can't register method '",
					scriptMethod.Name,
					"' with 0 parameters. Expected: 1 parameter of type (",
					vp_Utility.GetTypeAlias(handlerParameterType),
					")."
				}));
				return false;
			}
		}
		else if (scriptMethod.GetParameters().Length > 1)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				scriptMethod.DeclaringType,
				") Can't register method '",
				scriptMethod.Name,
				"' with ",
				scriptMethod.GetParameters().Length,
				" parameters. Max parameter count: 1 of type (",
				vp_Utility.GetTypeAlias(handlerParameterType),
				")."
			}));
			return false;
		}
		return true;
	}

	protected vp_EventHandler.ScriptMethods GetScriptMethods(object target)
	{
		vp_EventHandler.ScriptMethods scriptMethods;
		if (!vp_EventHandler.m_StoredScriptTypes.TryGetValue(target.GetType(), out scriptMethods))
		{
			scriptMethods = new vp_EventHandler.ScriptMethods(target.GetType());
			vp_EventHandler.m_StoredScriptTypes.Add(target.GetType(), scriptMethods);
		}
		return scriptMethods;
	}
}
