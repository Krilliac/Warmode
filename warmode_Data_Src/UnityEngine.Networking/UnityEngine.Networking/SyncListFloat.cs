using System;

namespace UnityEngine.Networking
{
	public sealed class SyncListFloat : SyncList<float>
	{
		protected override void SerializeItem(NetworkWriter writer, float item)
		{
			writer.Write(item);
		}

		protected override float DeserializeItem(NetworkReader reader)
		{
			return reader.ReadSingle();
		}

		public static SyncListFloat ReadInstance(NetworkReader reader)
		{
			ushort num = reader.ReadUInt16();
			SyncListFloat syncListFloat = new SyncListFloat();
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				syncListFloat.AddInternal(reader.ReadSingle());
			}
			return syncListFloat;
		}

		public static void WriteInstance(NetworkWriter writer, SyncListFloat items)
		{
			writer.Write((ushort)items.Count);
			foreach (float value in items)
			{
				writer.Write(value);
			}
		}
	}
}
