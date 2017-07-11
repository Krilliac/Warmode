using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnityEngine.Networking
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class SyncList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public enum Operation
		{
			OP_ADD,
			OP_CLEAR,
			OP_INSERT,
			OP_REMOVE,
			OP_REMOVEAT,
			OP_SET,
			OP_DIRTY
		}

		public delegate void SyncListChanged(SyncList<T>.Operation op, int itemIndex);

		private List<T> m_Objects = new List<T>();

		private NetworkBehaviour m_Behaviour;

		private int m_CmdHash;

		private SyncList<T>.SyncListChanged m_Callback;

		public int Count
		{
			get
			{
				return this.m_Objects.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public SyncList<T>.SyncListChanged Callback
		{
			get
			{
				return this.m_Callback;
			}
			set
			{
				this.m_Callback = value;
			}
		}

		public T this[int i]
		{
			get
			{
				return this.m_Objects[i];
			}
			set
			{
				this.m_Objects[i] = value;
				this.SendMsg(SyncList<T>.Operation.OP_SET, i, value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected abstract void SerializeItem(NetworkWriter writer, T item);

		protected abstract T DeserializeItem(NetworkReader reader);

		public void InitializeBehaviour(NetworkBehaviour beh, int cmdHash)
		{
			this.m_Behaviour = beh;
			this.m_CmdHash = cmdHash;
		}

		private void SendMsg(SyncList<T>.Operation op, int itemIndex, T item)
		{
			if (this.m_Behaviour == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("SyncList not initialized");
				}
				return;
			}
			NetworkIdentity component = this.m_Behaviour.GetComponent<NetworkIdentity>();
			if (component == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("SyncList no NetworkIdentity");
				}
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(9);
			networkWriter.Write(component.netId);
			networkWriter.WritePackedUInt32((uint)this.m_CmdHash);
			networkWriter.Write((byte)op);
			networkWriter.WritePackedUInt32((uint)itemIndex);
			this.SerializeItem(networkWriter, item);
			networkWriter.FinishMessage();
			NetworkServer.SendWriterToReady(component.gameObject, networkWriter, 0);
			if (this.m_Behaviour.isServer && this.m_Behaviour.isClient && this.m_Callback != null)
			{
				this.m_Callback(op, itemIndex);
			}
		}

		private void SendMsg(SyncList<T>.Operation op, int itemIndex)
		{
			this.SendMsg(op, itemIndex, default(T));
		}

		public void HandleMsg(NetworkReader reader)
		{
			byte op = reader.ReadByte();
			int num = (int)reader.ReadPackedUInt32();
			T t = this.DeserializeItem(reader);
			switch (op)
			{
			case 0:
				this.m_Objects.Add(t);
				break;
			case 1:
				this.m_Objects.Clear();
				break;
			case 2:
				this.m_Objects.Insert(num, t);
				break;
			case 3:
				this.m_Objects.Remove(t);
				break;
			case 4:
				this.m_Objects.RemoveAt(num);
				break;
			case 5:
			case 6:
				this.m_Objects[num] = t;
				break;
			}
			if (this.m_Callback != null)
			{
				this.m_Callback((SyncList<T>.Operation)op, num);
			}
		}

		internal void AddInternal(T item)
		{
			this.m_Objects.Add(item);
		}

		public void Add(T item)
		{
			this.m_Objects.Add(item);
			this.SendMsg(SyncList<T>.Operation.OP_ADD, this.m_Objects.Count - 1, item);
		}

		public void Clear()
		{
			this.m_Objects.Clear();
			this.SendMsg(SyncList<T>.Operation.OP_CLEAR, 0);
		}

		public bool Contains(T item)
		{
			return this.m_Objects.Contains(item);
		}

		public void CopyTo(T[] array, int index)
		{
			this.m_Objects.CopyTo(array, index);
		}

		public int IndexOf(T item)
		{
			return this.m_Objects.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.m_Objects.Insert(index, item);
			this.SendMsg(SyncList<T>.Operation.OP_INSERT, index, item);
		}

		public bool Remove(T item)
		{
			bool flag = this.m_Objects.Remove(item);
			if (flag)
			{
				this.SendMsg(SyncList<T>.Operation.OP_REMOVE, 0, item);
			}
			return flag;
		}

		public void RemoveAt(int index)
		{
			this.m_Objects.RemoveAt(index);
			this.SendMsg(SyncList<T>.Operation.OP_REMOVEAT, index);
		}

		public void Dirty(int index)
		{
			this.SendMsg(SyncList<T>.Operation.OP_DIRTY, index, this.m_Objects[index]);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.m_Objects.GetEnumerator();
		}
	}
}
