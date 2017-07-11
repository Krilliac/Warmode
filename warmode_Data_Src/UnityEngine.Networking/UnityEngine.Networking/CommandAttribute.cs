using System;

namespace UnityEngine.Networking
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		public int channel;
	}
}
