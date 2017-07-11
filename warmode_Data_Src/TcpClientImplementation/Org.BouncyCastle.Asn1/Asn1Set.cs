using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public abstract class Asn1Set : Asn1Object, IEnumerable
	{
		private class Asn1SetParserImpl : Asn1SetParser, IAsn1Convertible
		{
			private readonly Asn1Set outer;

			private readonly int max;

			private int index;

			public Asn1SetParserImpl(Asn1Set outer)
			{
				this.outer = outer;
				this.max = outer.Count;
			}

			public IAsn1Convertible ReadObject()
			{
				if (this.index == this.max)
				{
					return null;
				}
				Asn1Encodable asn1Encodable = this.outer[this.index++];
				if (asn1Encodable is Asn1Sequence)
				{
					return ((Asn1Sequence)asn1Encodable).Parser;
				}
				if (asn1Encodable is Asn1Set)
				{
					return ((Asn1Set)asn1Encodable).Parser;
				}
				return asn1Encodable;
			}

			public virtual Asn1Object ToAsn1Object()
			{
				return this.outer;
			}
		}

		private class DerComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				byte[] array = (byte[])x;
				byte[] array2 = (byte[])y;
				int num = Math.Min(array.Length, array2.Length);
				int num2 = 0;
				while (num2 != num)
				{
					byte b = array[num2];
					byte b2 = array2[num2];
					if (b != b2)
					{
						if (b >= b2)
						{
							return 1;
						}
						return -1;
					}
					else
					{
						num2++;
					}
				}
				if (array.Length > array2.Length)
				{
					if (!this.AllZeroesFrom(array, num))
					{
						return 1;
					}
					return 0;
				}
				else
				{
					if (array.Length >= array2.Length)
					{
						return 0;
					}
					if (!this.AllZeroesFrom(array2, num))
					{
						return -1;
					}
					return 0;
				}
			}

			private bool AllZeroesFrom(byte[] bs, int pos)
			{
				while (pos < bs.Length)
				{
					if (bs[pos++] != 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		private readonly IList _set;

		public virtual Asn1Encodable this[int index]
		{
			get
			{
				return (Asn1Encodable)this._set[index];
			}
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return this.Count;
			}
		}

		public virtual int Count
		{
			get
			{
				return this._set.Count;
			}
		}

		public Asn1SetParser Parser
		{
			get
			{
				return new Asn1Set.Asn1SetParserImpl(this);
			}
		}

		public static Asn1Set GetInstance(object obj)
		{
			if (obj == null || obj is Asn1Set)
			{
				return (Asn1Set)obj;
			}
			if (obj is Asn1SetParser)
			{
				return Asn1Set.GetInstance(((Asn1SetParser)obj).ToAsn1Object());
			}
			if (obj is byte[])
			{
				try
				{
					return Asn1Set.GetInstance(Asn1Object.FromByteArray((byte[])obj));
				}
				catch (IOException ex)
				{
					throw new ArgumentException("failed to construct set from byte[]: " + ex.Message);
				}
			}
			if (obj is Asn1Encodable)
			{
				Asn1Object asn1Object = ((Asn1Encodable)obj).ToAsn1Object();
				if (asn1Object is Asn1Set)
				{
					return (Asn1Set)asn1Object;
				}
			}
			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public static Asn1Set GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			Asn1Object @object = obj.GetObject();
			if (explicitly)
			{
				if (!obj.IsExplicit())
				{
					throw new ArgumentException("object implicit - explicit expected.");
				}
				return (Asn1Set)@object;
			}
			else
			{
				if (obj.IsExplicit())
				{
					return new DerSet(@object);
				}
				if (@object is Asn1Set)
				{
					return (Asn1Set)@object;
				}
				if (@object is Asn1Sequence)
				{
					Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
					Asn1Sequence asn1Sequence = (Asn1Sequence)@object;
					foreach (Asn1Encodable asn1Encodable in asn1Sequence)
					{
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							asn1Encodable
						});
					}
					return new DerSet(asn1EncodableVector, false);
				}
				throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
			}
		}

		protected internal Asn1Set(int capacity)
		{
			this._set = Platform.CreateArrayList(capacity);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return this._set.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return this.GetEnumerator();
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable GetObjectAt(int index)
		{
			return this[index];
		}

		public virtual Asn1Encodable[] ToArray()
		{
			Asn1Encodable[] array = new Asn1Encodable[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				array[i] = this[i];
			}
			return array;
		}

		protected override int Asn1GetHashCode()
		{
			int num = this.Count;
			foreach (object current in this)
			{
				num *= 17;
				if (current == null)
				{
					num ^= DerNull.Instance.GetHashCode();
				}
				else
				{
					num ^= current.GetHashCode();
				}
			}
			return num;
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			Asn1Set asn1Set = asn1Object as Asn1Set;
			if (asn1Set == null)
			{
				return false;
			}
			if (this.Count != asn1Set.Count)
			{
				return false;
			}
			IEnumerator enumerator = this.GetEnumerator();
			IEnumerator enumerator2 = asn1Set.GetEnumerator();
			while (enumerator.MoveNext() && enumerator2.MoveNext())
			{
				Asn1Object asn1Object2 = this.GetCurrent(enumerator).ToAsn1Object();
				Asn1Object obj = this.GetCurrent(enumerator2).ToAsn1Object();
				if (!asn1Object2.Equals(obj))
				{
					return false;
				}
			}
			return true;
		}

		private Asn1Encodable GetCurrent(IEnumerator e)
		{
			Asn1Encodable asn1Encodable = (Asn1Encodable)e.Current;
			if (asn1Encodable == null)
			{
				return DerNull.Instance;
			}
			return asn1Encodable;
		}

		protected internal void Sort()
		{
			if (this._set.Count < 2)
			{
				return;
			}
			Asn1Encodable[] array = new Asn1Encodable[this._set.Count];
			byte[][] array2 = new byte[this._set.Count][];
			for (int i = 0; i < this._set.Count; i++)
			{
				Asn1Encodable asn1Encodable = (Asn1Encodable)this._set[i];
				array[i] = asn1Encodable;
				array2[i] = asn1Encodable.GetEncoded("DER");
			}
			Array.Sort(array2, array, new Asn1Set.DerComparer());
			for (int j = 0; j < this._set.Count; j++)
			{
				this._set[j] = array[j];
			}
		}

		protected internal void AddObject(Asn1Encodable obj)
		{
			this._set.Add(obj);
		}

		public override string ToString()
		{
			return CollectionUtilities.ToString(this._set);
		}
	}
}
