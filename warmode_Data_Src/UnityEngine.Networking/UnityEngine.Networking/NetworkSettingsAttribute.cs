using System;

namespace UnityEngine.Networking
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NetworkSettingsAttribute : Attribute
	{
		public int channel;

		public float sendInterval = 0.1f;
	}
}
