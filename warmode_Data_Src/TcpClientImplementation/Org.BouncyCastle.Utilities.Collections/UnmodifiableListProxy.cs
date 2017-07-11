using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public class UnmodifiableListProxy : UnmodifiableList
	{
		private readonly IList l;

		public override int Count
		{
			get
			{
				return this.l.Count;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return this.l.IsFixedSize;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return this.l.IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return this.l.SyncRoot;
			}
		}

		public UnmodifiableListProxy(IList l)
		{
			this.l = l;
		}

		public override bool Contains(object o)
		{
			return this.l.Contains(o);
		}

		public override void CopyTo(Array array, int index)
		{
			this.l.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.l.GetEnumerator();
		}

		public override int IndexOf(object o)
		{
			return this.l.IndexOf(o);
		}

		protected override object GetValue(int i)
		{
			return this.l[i];
		}
	}
}
