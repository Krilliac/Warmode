using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class SubjectDirectoryAttributes : Asn1Encodable
	{
		private readonly IList attributes;

		public IEnumerable Attributes
		{
			get
			{
				return new EnumerableProxy(this.attributes);
			}
		}

		public static SubjectDirectoryAttributes GetInstance(object obj)
		{
			if (obj == null || obj is SubjectDirectoryAttributes)
			{
				return (SubjectDirectoryAttributes)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SubjectDirectoryAttributes((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private SubjectDirectoryAttributes(Asn1Sequence seq)
		{
			this.attributes = Platform.CreateArrayList();
			foreach (object current in seq)
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(current);
				this.attributes.Add(AttributeX509.GetInstance(instance));
			}
		}

		[Obsolete]
		public SubjectDirectoryAttributes(ArrayList attributes) : this(attributes)
		{
		}

		public SubjectDirectoryAttributes(IList attributes)
		{
			this.attributes = Platform.CreateArrayList(attributes);
		}

		public override Asn1Object ToAsn1Object()
		{
			AttributeX509[] array = new AttributeX509[this.attributes.Count];
			for (int i = 0; i < this.attributes.Count; i++)
			{
				array[i] = (AttributeX509)this.attributes[i];
			}
			return new DerSequence(array);
		}
	}
}
