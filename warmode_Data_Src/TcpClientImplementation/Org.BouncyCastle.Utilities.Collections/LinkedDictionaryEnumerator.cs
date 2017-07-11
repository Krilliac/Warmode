using System;
using System.Collections;

namespace Org.BouncyCastle.Utilities.Collections
{
	internal class LinkedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private readonly LinkedDictionary parent;

		private int pos = -1;

		public virtual object Current
		{
			get
			{
				return this.Entry;
			}
		}

		public virtual DictionaryEntry Entry
		{
			get
			{
				object currentKey = this.CurrentKey;
				return new DictionaryEntry(currentKey, this.parent.hash[currentKey]);
			}
		}

		public virtual object Key
		{
			get
			{
				return this.CurrentKey;
			}
		}

		public virtual object Value
		{
			get
			{
				return this.parent.hash[this.CurrentKey];
			}
		}

		private object CurrentKey
		{
			get
			{
				if (this.pos < 0 || this.pos >= this.parent.keys.Count)
				{
					throw new InvalidOperationException();
				}
				return this.parent.keys[this.pos];
			}
		}

		internal LinkedDictionaryEnumerator(LinkedDictionary parent)
		{
			this.parent = parent;
		}

		public virtual bool MoveNext()
		{
			return this.pos < this.parent.keys.Count && ++this.pos < this.parent.keys.Count;
		}

		public virtual void Reset()
		{
			this.pos = -1;
		}
	}
}
