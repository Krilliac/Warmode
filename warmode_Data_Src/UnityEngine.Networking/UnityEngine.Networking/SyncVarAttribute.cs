using System;

namespace UnityEngine.Networking
{
	[AttributeUsage(AttributeTargets.Field)]
	public class SyncVarAttribute : Attribute
	{
		public string hook;
	}
}
