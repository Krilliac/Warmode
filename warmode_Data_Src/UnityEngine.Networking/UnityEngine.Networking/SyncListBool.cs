using System;

namespace UnityEngine.Networking
{
	public class SyncListBool : SyncList<bool>
	{
		protected override void SerializeItem(NetworkWriter writer, bool item)
		{
			writer.Write(item);
		}

		protected override bool DeserializeItem(NetworkReader reader)
		{
			return reader.ReadBoolean();
		}

		public static SyncListBool ReadInstance(NetworkReader reader)
		{
			ushort num = reader.ReadUInt16();
			SyncListBool syncListBool = new SyncListBool();
			for (ushort num2 = 0; num2 < num; num2 += 1)
			{
				syncListBool.AddInternal(reader.ReadBoolean());
			}
			return syncListBool;
		}

		public static void WriteInstance(NetworkWriter writer, SyncListBool items)
		{
			writer.Write((ushort)items.Count);
			foreach (bool current in items)
			{
				writer.Write(current);
			}
		}
	}
}
