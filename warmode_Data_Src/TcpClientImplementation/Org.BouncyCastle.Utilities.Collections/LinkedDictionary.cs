using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public class LinkedDictionary : IDictionary, ICollection, IEnumerable
	{
		internal readonly IDictionary hash = Platform.CreateHashtable();

		internal readonly IList keys = Platform.CreateArrayList();

		public virtual int Count
		{
			get
			{
				return this.hash.Count;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return false;
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				return Platform.CreateArrayList(this.keys);
			}
		}

		public virtual ICollection Values
		{
			get
			{
				IList list = Platform.CreateArrayList(this.keys.Count);
				foreach (object current in this.keys)
				{
					list.Add(this.hash[current]);
				}
				return list;
			}
		}

		public virtual object this[object k]
		{
			get
			{
				return this.hash[k];
			}
			set
			{
				if (!this.hash.Contains(k))
				{
					this.keys.Add(k);
				}
				this.hash[k] = value;
			}
		}

		public virtual void Add(object k, object v)
		{
			this.hash.Add(k, v);
			this.keys.Add(k);
		}

		public virtual void Clear()
		{
			this.hash.Clear();
			this.keys.Clear();
		}

		public virtual bool Contains(object k)
		{
			return this.hash.Contains(k);
		}

		public virtual void CopyTo(Array array, int index)
		{
			foreach (object current in this.keys)
			{
				array.SetValue(this.hash[current], index++);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public virtual IDictionaryEnumerator GetEnumerator()
		{
			return new LinkedDictionaryEnumerator(this);
		}

		public virtual void Remove(object k)
		{
			this.hash.Remove(k);
			this.keys.Remove(k);
		}
	}
}
