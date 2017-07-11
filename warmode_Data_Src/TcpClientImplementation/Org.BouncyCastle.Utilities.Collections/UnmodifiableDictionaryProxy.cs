using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public class UnmodifiableDictionaryProxy : UnmodifiableDictionary
	{
		private readonly IDictionary d;

		public override int Count
		{
			get
			{
				return this.d.Count;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return this.d.IsFixedSize;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return this.d.IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return this.d.SyncRoot;
			}
		}

		public override ICollection Keys
		{
			get
			{
				return this.d.Keys;
			}
		}

		public override ICollection Values
		{
			get
			{
				return this.d.Values;
			}
		}

		public UnmodifiableDictionaryProxy(IDictionary d)
		{
			this.d = d;
		}

		public override bool Contains(object k)
		{
			return this.d.Contains(k);
		}

		public override void CopyTo(Array array, int index)
		{
			this.d.CopyTo(array, index);
		}

		public override IDictionaryEnumerator GetEnumerator()
		{
			return this.d.GetEnumerator();
		}

		protected override object GetValue(object k)
		{
			return this.d[k];
		}
	}
}
