using System;

namespace UnityEngine.Networking
{
	public class SyncListStruct<T> : SyncList<T> where T : struct
	{
		public new ushort Count
		{
			get
			{
				return (ushort)base.Count;
			}
		}

		public new void AddInternal(T item)
		{
			base.AddInternal(item);
		}

		protected override void SerializeItem(NetworkWriter writer, T item)
		{
		}

		protected override T DeserializeItem(NetworkReader reader)
		{
			return default(T);
		}

		public T GetItem(int i)
		{
			return base[i];
		}
	}
}
