using System;

namespace UnityEngine.Networking
{
	[AttributeUsage(AttributeTargets.Event)]
	public class SyncEventAttribute : Attribute
	{
		public int channel;
	}
}
