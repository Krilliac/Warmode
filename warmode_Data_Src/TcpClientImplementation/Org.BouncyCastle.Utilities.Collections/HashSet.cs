using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public class HashSet : ISet, ICollection, IEnumerable
	{
		private readonly IDictionary impl = Platform.CreateHashtable();

		public virtual int Count
		{
			get
			{
				return this.impl.Count;
			}
		}

		public virtual bool IsEmpty
		{
			get
			{
				return this.impl.Count == 0;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return this.impl.IsFixedSize;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return this.impl.IsReadOnly;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return this.impl.IsSynchronized;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this.impl.SyncRoot;
			}
		}

		public HashSet()
		{
		}

		public HashSet(IEnumerable s)
		{
			foreach (object current in s)
			{
				this.Add(current);
			}
		}

		public virtual void Add(object o)
		{
			this.impl[o] = null;
		}

		public virtual void AddAll(IEnumerable e)
		{
			foreach (object current in e)
			{
				this.Add(current);
			}
		}

		public virtual void Clear()
		{
			this.impl.Clear();
		}

		public virtual bool Contains(object o)
		{
			return this.impl.Contains(o);
		}

		public virtual void CopyTo(Array array, int index)
		{
			this.impl.Keys.CopyTo(array, index);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this.impl.Keys.GetEnumerator();
		}

		public virtual void Remove(object o)
		{
			this.impl.Remove(o);
		}

		public virtual void RemoveAll(IEnumerable e)
		{
			foreach (object current in e)
			{
				this.Remove(current);
			}
		}
	}
}
