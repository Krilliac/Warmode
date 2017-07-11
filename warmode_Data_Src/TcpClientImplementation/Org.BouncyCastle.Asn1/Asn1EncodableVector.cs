using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	public class Asn1EncodableVector : IEnumerable
	{
		private IList v = Platform.CreateArrayList();

		public Asn1Encodable this[int index]
		{
			get
			{
				return (Asn1Encodable)this.v[index];
			}
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get
			{
				return this.v.Count;
			}
		}

		public int Count
		{
			get
			{
				return this.v.Count;
			}
		}

		public static Asn1EncodableVector FromEnumerable(IEnumerable e)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (Asn1Encodable asn1Encodable in e)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					asn1Encodable
				});
			}
			return asn1EncodableVector;
		}

		public Asn1EncodableVector(params Asn1Encodable[] v)
		{
			this.Add(v);
		}

		public void Add(params Asn1Encodable[] objs)
		{
			for (int i = 0; i < objs.Length; i++)
			{
				Asn1Encodable value = objs[i];
				this.v.Add(value);
			}
		}

		public void AddOptional(params Asn1Encodable[] objs)
		{
			if (objs != null)
			{
				for (int i = 0; i < objs.Length; i++)
				{
					Asn1Encodable asn1Encodable = objs[i];
					if (asn1Encodable != null)
					{
						this.v.Add(asn1Encodable);
					}
				}
			}
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable Get(int index)
		{
			return this[index];
		}

		public IEnumerator GetEnumerator()
		{
			return this.v.GetEnumerator();
		}
	}
}
