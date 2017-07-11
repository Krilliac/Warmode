using System;
using System.Collections.Generic;
using System.Reflection;

public class vp_Message : vp_Event
{
	public delegate void Sender();

	public vp_Message.Sender Send;

	public vp_Message(string name) : base(name)
	{
		this.InitFields();
	}

	protected static void Empty()
	{
	}

	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetType().GetMethod("Empty")
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message.Sender)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message.Sender(vp_Message.Empty);
	}

	public override void Register(object t, string m, int v)
	{
		this.Send = (vp_Message.Sender)Delegate.Combine(this.Send, (vp_Message.Sender)Delegate.CreateDelegate(this.m_DelegateTypes[v], t, m));
		base.Refresh();
	}

	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.Refresh();
	}
}
public class vp_Message<V> : vp_Message
{
	public delegate void Sender<T>(T value);

	public new vp_Message<V>.Sender<V> Send;

	public vp_Message(string name) : base(name)
	{
	}

	protected static void Empty<T>(T value)
	{
	}

	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetStaticGenericMethod(base.GetType(), "Empty", this.m_ArgumentType, typeof(void))
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message<>.Sender<>)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message<V>.Sender<V>(vp_Message<V>.Empty<V>);
		base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
	}

	public override void Register(object t, string m, int v)
	{
		base.AddExternalMethodToField(t, this.m_Fields[v], m, base.MakeGenericType(this.m_DelegateTypes[v]));
		base.Refresh();
	}

	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.Refresh();
	}
}
public class vp_Message<V, VResult> : vp_Message
{
	public delegate TResult Sender<T, TResult>(T value);

	public new vp_Message<V, VResult>.Sender<V, VResult> Send;

	public vp_Message(string name) : base(name)
	{
	}

	protected static TResult Empty<T, TResult>(T value)
	{
		return default(TResult);
	}

	protected override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetStaticGenericMethod(base.GetType(), "Empty", this.m_ArgumentType, this.m_ReturnType)
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message<, >.Sender<, >)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
	}

	public override void Register(object t, string m, int v)
	{
		base.AddExternalMethodToField(t, this.m_Fields[0], m, base.MakeGenericType(this.m_DelegateTypes[0]));
		base.Refresh();
	}

	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.Refresh();
	}
}
