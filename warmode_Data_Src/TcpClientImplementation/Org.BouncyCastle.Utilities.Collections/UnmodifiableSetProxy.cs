using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	public class UnmodifiableSetProxy : UnmodifiableSet
	{
		private readonly ISet s;

		public override int Count
		{
			get
			{
				return this.s.Count;
			}
		}

		public override bool IsEmpty
		{
			get
			{
				return this.s.IsEmpty;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return this.s.IsFixedSize;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return this.s.IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return this.s.SyncRoot;
			}
		}

		public UnmodifiableSetProxy(ISet s)
		{
			this.s = s;
		}

		public override bool Contains(object o)
		{
			return this.s.Contains(o);
		}

		public override void CopyTo(Array array, int index)
		{
			this.s.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return this.s.GetEnumerator();
		}
	}
}
