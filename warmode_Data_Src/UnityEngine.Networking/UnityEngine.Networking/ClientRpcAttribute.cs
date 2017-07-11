using System;

namespace UnityEngine.Networking
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ClientRpcAttribute : Attribute
	{
		public int channel;
	}
}
