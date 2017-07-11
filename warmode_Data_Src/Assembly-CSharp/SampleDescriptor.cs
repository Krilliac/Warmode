using System;
using UnityEngine;

public sealed class SampleDescriptor
{
	public Type Type
	{
		get;
		set;
	}

	public string DisplayName
	{
		get;
		set;
	}

	public string Description
	{
		get;
		set;
	}

	public string CodeBlock
	{
		get;
		set;
	}

	public bool IsSelected
	{
		get;
		set;
	}

	public GameObject UnityObject
	{
		get;
		set;
	}

	public bool IsRunning
	{
		get
		{
			return this.UnityObject != null;
		}
	}

	public SampleDescriptor(Type type, string displayName, string description, string codeBlock)
	{
		this.Type = type;
		this.DisplayName = displayName;
		this.Description = description;
		this.CodeBlock = codeBlock;
	}

	public void CreateUnityObject()
	{
		if (this.UnityObject != null)
		{
			return;
		}
		this.UnityObject = new GameObject(this.DisplayName);
		this.UnityObject.AddComponent(this.Type);
	}

	public void DestroyUnityObject()
	{
		if (this.UnityObject != null)
		{
			UnityEngine.Object.Destroy(this.UnityObject);
			this.UnityObject = null;
		}
	}
}
