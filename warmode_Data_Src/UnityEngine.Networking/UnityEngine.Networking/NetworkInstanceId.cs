using System;

namespace UnityEngine.Networking
{
	[Serializable]
	public struct NetworkInstanceId
	{
		[SerializeField]
		private readonly uint m_Value;

		public static NetworkInstanceId Invalid = new NetworkInstanceId(4294967295u);

		internal static NetworkInstanceId Zero = new NetworkInstanceId(0u);

		public uint Value
		{
			get
			{
				return this.m_Value;
			}
		}

		public NetworkInstanceId(uint value)
		{
			this.m_Value = value;
		}

		public bool IsEmpty()
		{
			return this.m_Value == 0u;
		}

		public override int GetHashCode()
		{
			return (int)this.m_Value;
		}

		public override bool Equals(object obj)
		{
			return obj is NetworkInstanceId && this == (NetworkInstanceId)obj;
		}

		public override string ToString()
		{
			return this.m_Value.ToString();
		}

		public static bool operator ==(NetworkInstanceId c1, NetworkInstanceId c2)
		{
			return c1.m_Value == c2.m_Value;
		}

		public static bool operator !=(NetworkInstanceId c1, NetworkInstanceId c2)
		{
			return c1.m_Value != c2.m_Value;
		}
	}
}
