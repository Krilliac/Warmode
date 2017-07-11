using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class vp_Event
{
	protected string m_Name;

	protected Type m_ArgumentType;

	protected Type m_ReturnType;

	protected FieldInfo[] m_Fields;

	protected Type[] m_DelegateTypes;

	protected MethodInfo[] m_DefaultMethods;

	public string[] InvokerFieldNames;

	public Dictionary<string, int> Prefixes;

	public string EventName
	{
		get
		{
			return this.m_Name;
		}
	}

	public Type ArgumentType
	{
		get
		{
			return this.m_ArgumentType;
		}
	}

	public Type ReturnType
	{
		get
		{
			return this.m_ReturnType;
		}
	}

	private Type GetArgumentType
	{
		get
		{
			if (!base.GetType().IsGenericType)
			{
				return typeof(void);
			}
			return base.GetType().GetGenericArguments()[0];
		}
	}

	private Type GetGenericReturnType
	{
		get
		{
			if (!base.GetType().IsGenericType)
			{
				return typeof(void);
			}
			if (base.GetType().GetGenericArguments().Length != 2)
			{
				return typeof(void);
			}
			return base.GetType().GetGenericArguments()[1];
		}
	}

	public vp_Event(string name = "")
	{
		this.m_ArgumentType = this.GetArgumentType;
		this.m_ReturnType = this.GetGenericReturnType;
		this.m_Name = name;
	}

	public abstract void Register(object target, string method, int variant);

	public abstract void Unregister(object target);

	protected abstract void InitFields();

	protected void StoreInvokerFieldNames()
	{
		this.InvokerFieldNames = new string[this.m_Fields.Length];
		for (int i = 0; i < this.m_Fields.Length; i++)
		{
			this.InvokerFieldNames[i] = this.m_Fields[i].Name;
		}
	}

	protected Type MakeGenericType(Type type)
	{
		if (this.m_ReturnType == typeof(void))
		{
			return type.MakeGenericType(new Type[]
			{
				this.m_ArgumentType,
				this.m_ArgumentType
			});
		}
		return type.MakeGenericType(new Type[]
		{
			this.m_ArgumentType,
			this.m_ReturnType,
			this.m_ArgumentType,
			this.m_ReturnType
		});
	}

	protected void SetFieldToExternalMethod(object target, FieldInfo field, string method, Type type)
	{
		Delegate @delegate = Delegate.CreateDelegate(type, target, method, false, false);
		if (@delegate == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error (",
				this,
				") Failed to bind: ",
				target,
				" -> ",
				method,
				"."
			}));
			return;
		}
		field.SetValue(this, @delegate);
	}

	protected void AddExternalMethodToField(object target, FieldInfo field, string method, Type type)
	{
		Delegate @delegate = Delegate.Combine((Delegate)field.GetValue(this), Delegate.CreateDelegate(type, target, method, false, false));
		if (@delegate == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error (",
				this,
				") Failed to bind: ",
				target,
				" -> ",
				method,
				"."
			}));
			return;
		}
		field.SetValue(this, @delegate);
	}

	protected void SetFieldToLocalMethod(FieldInfo field, MethodInfo method, Type type)
	{
		Delegate @delegate = Delegate.CreateDelegate(type, method);
		if (@delegate == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error (",
				this,
				") Failed to bind: ",
				method,
				"."
			}));
			return;
		}
		field.SetValue(this, @delegate);
	}

	protected void RemoveExternalMethodFromField(object target, FieldInfo field)
	{
		List<Delegate> list = new List<Delegate>(((Delegate)field.GetValue(this)).GetInvocationList());
		if (list == null)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error (",
				this,
				") Failed to remove: ",
				target,
				" -> ",
				field.Name,
				"."
			}));
			return;
		}
		for (int i = list.Count - 1; i > -1; i--)
		{
			if (list[i].Target == target)
			{
				list.Remove(list[i]);
			}
		}
		if (list != null)
		{
			field.SetValue(this, Delegate.Combine(list.ToArray()));
		}
	}

	protected MethodInfo GetStaticGenericMethod(Type e, string name, Type parameterType, Type returnType)
	{
		MethodInfo[] methods = e.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		for (int i = 0; i < methods.Length; i++)
		{
			MethodInfo methodInfo = methods[i];
			if (!(methodInfo.Name != name))
			{
				MethodInfo methodInfo2;
				if (this.GetGenericReturnType == typeof(void))
				{
					methodInfo2 = methodInfo.MakeGenericMethod(new Type[]
					{
						this.m_ArgumentType
					});
				}
				else
				{
					methodInfo2 = methodInfo.MakeGenericMethod(new Type[]
					{
						this.m_ArgumentType,
						this.m_ReturnType
					});
				}
				if (methodInfo2.GetParameters().Length <= 1)
				{
					if (methodInfo2.GetParameters().Length != 1 || parameterType != typeof(void))
					{
						if (methodInfo2.GetParameters().Length != 0 || parameterType == typeof(void))
						{
							if (methodInfo2.GetParameters().Length != 1 || methodInfo2.GetParameters()[0].ParameterType == parameterType)
							{
								if (returnType == methodInfo2.ReturnType)
								{
									return methodInfo2;
								}
							}
						}
					}
				}
			}
		}
		return null;
	}

	public Type GetParameterType(int index)
	{
		if (!base.GetType().IsGenericType)
		{
			return typeof(void);
		}
		if (index > this.m_Fields.Length - 1)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				") Event '",
				this.EventName,
				"' only supports ",
				this.m_Fields.Length,
				" indices. 'GetParameterType' referenced index ",
				index,
				"."
			}));
		}
		if (this.m_DelegateTypes[index].GetMethod("Invoke").GetParameters().Length == 0)
		{
			return typeof(void);
		}
		return this.m_ArgumentType;
	}

	public Type GetReturnType(int index)
	{
		if (index > this.m_Fields.Length - 1)
		{
			Debug.LogError(string.Concat(new object[]
			{
				"Error: (",
				this,
				") Event '",
				this.EventName,
				"' only supports ",
				this.m_Fields.Length,
				" indices. 'GetReturnType' referenced index ",
				index,
				"."
			}));
			return null;
		}
		if (base.GetType().GetGenericArguments().Length > 1)
		{
			return this.GetGenericReturnType;
		}
		Type returnType = this.m_DelegateTypes[index].GetMethod("Invoke").ReturnType;
		if (returnType.IsGenericParameter)
		{
			return this.m_ArgumentType;
		}
		return returnType;
	}

	protected void Refresh()
	{
	}
}
