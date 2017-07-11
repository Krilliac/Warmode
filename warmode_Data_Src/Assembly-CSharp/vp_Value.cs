using System;
using System.Collections.Generic;
using System.Reflection;

public class vp_Value<V> : vp_Event
{
	public delegate T Getter<T>();

	public delegate void Setter<T>(T o);

	public vp_Value<V>.Getter<V> Get;

	public vp_Value<V>.Setter<V> Set;

	private FieldInfo[] Fields
	{
		get
		{
			return this.m_Fields;
		}
	}

	public vp_Value(string name) : base(name)
	{
		this.InitFields();
	}

	protected static T Empty<T>()
	{
		return default(T);
	}

	protected static void Empty<T>(T value)
	{
	}

	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Get"),
			base.GetType().GetField("Set")
		};
		base.StoreInvokerFieldNames();
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Value<>.Getter<>),
			typeof(vp_Value<>.Setter<>)
		};
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetStaticGenericMethod(base.GetType(), "Empty", typeof(void), this.m_ArgumentType),
			base.GetStaticGenericMethod(base.GetType(), "Empty", this.m_ArgumentType, typeof(void))
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"get_OnValue_",
				0
			},
			{
				"set_OnValue_",
				1
			}
		};
		base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		base.SetFieldToLocalMethod(this.m_Fields[1], this.m_DefaultMethods[1], base.MakeGenericType(this.m_DelegateTypes[1]));
	}

	public override void Register(object t, string m, int v)
	{
		base.SetFieldToExternalMethod(t, this.m_Fields[v], m, base.MakeGenericType(this.m_DelegateTypes[v]));
		base.Refresh();
	}

	public override void Unregister(object t)
	{
		base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		base.SetFieldToLocalMethod(this.m_Fields[1], this.m_DefaultMethods[1], base.MakeGenericType(this.m_DelegateTypes[1]));
		base.Refresh();
	}
}
