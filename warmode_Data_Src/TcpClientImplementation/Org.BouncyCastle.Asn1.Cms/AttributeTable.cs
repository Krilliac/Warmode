using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class AttributeTable
	{
		private readonly IDictionary attributes;

		public Attribute this[DerObjectIdentifier oid]
		{
			get
			{
				object obj = this.attributes[oid];
				if (obj is IList)
				{
					return (Attribute)((IList)obj)[0];
				}
				return (Attribute)obj;
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				foreach (object current in this.attributes.Values)
				{
					if (current is IList)
					{
						num += ((IList)current).Count;
					}
					else
					{
						num++;
					}
				}
				return num;
			}
		}

		[Obsolete]
		public AttributeTable(Hashtable attrs)
		{
			this.attributes = Platform.CreateHashtable(attrs);
		}

		public AttributeTable(IDictionary attrs)
		{
			this.attributes = Platform.CreateHashtable(attrs);
		}

		public AttributeTable(Asn1EncodableVector v)
		{
			this.attributes = Platform.CreateHashtable(v.Count);
			foreach (Asn1Encodable obj in v)
			{
				Attribute instance = Attribute.GetInstance(obj);
				this.AddAttribute(instance);
			}
		}

		public AttributeTable(Asn1Set s)
		{
			this.attributes = Platform.CreateHashtable(s.Count);
			for (int num = 0; num != s.Count; num++)
			{
				Attribute instance = Attribute.GetInstance(s[num]);
				this.AddAttribute(instance);
			}
		}

		public AttributeTable(Attributes attrs) : this(Asn1Set.GetInstance(attrs.ToAsn1Object()))
		{
		}

		private void AddAttribute(Attribute a)
		{
			DerObjectIdentifier attrType = a.AttrType;
			object obj = this.attributes[attrType];
			if (obj == null)
			{
				this.attributes[attrType] = a;
				return;
			}
			IList list;
			if (obj is Attribute)
			{
				list = Platform.CreateArrayList();
				list.Add(obj);
				list.Add(a);
			}
			else
			{
				list = (IList)obj;
				list.Add(a);
			}
			this.attributes[attrType] = list;
		}

		[Obsolete("Use 'object[oid]' syntax instead")]
		public Attribute Get(DerObjectIdentifier oid)
		{
			return this[oid];
		}

		public Asn1EncodableVector GetAll(DerObjectIdentifier oid)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			object obj = this.attributes[oid];
			if (obj is IList)
			{
				using (IEnumerator enumerator = ((IList)obj).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Attribute attribute = (Attribute)enumerator.Current;
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							attribute
						});
					}
					return asn1EncodableVector;
				}
			}
			if (obj != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					(Attribute)obj
				});
			}
			return asn1EncodableVector;
		}

		public IDictionary ToDictionary()
		{
			return Platform.CreateHashtable(this.attributes);
		}

		[Obsolete("Use 'ToDictionary' instead")]
		public Hashtable ToHashtable()
		{
			return new Hashtable(this.attributes);
		}

		public Asn1EncodableVector ToAsn1EncodableVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (object current in this.attributes.Values)
			{
				if (current is IList)
				{
					using (IEnumerator enumerator2 = ((IList)current).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object current2 = enumerator2.Current;
							asn1EncodableVector.Add(new Asn1Encodable[]
							{
								Attribute.GetInstance(current2)
							});
						}
						continue;
					}
				}
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					Attribute.GetInstance(current)
				});
			}
			return asn1EncodableVector;
		}

		public Attributes ToAttributes()
		{
			return new Attributes(this.ToAsn1EncodableVector());
		}

		public AttributeTable Add(DerObjectIdentifier attrType, Asn1Encodable attrValue)
		{
			AttributeTable attributeTable = new AttributeTable(this.attributes);
			attributeTable.AddAttribute(new Attribute(attrType, new DerSet(attrValue)));
			return attributeTable;
		}

		public AttributeTable Remove(DerObjectIdentifier attrType)
		{
			AttributeTable attributeTable = new AttributeTable(this.attributes);
			attributeTable.attributes.Remove(attrType);
			return attributeTable;
		}
	}
}
