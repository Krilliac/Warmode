using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class NameConstraints : Asn1Encodable
	{
		private Asn1Sequence permitted;

		private Asn1Sequence excluded;

		public Asn1Sequence PermittedSubtrees
		{
			get
			{
				return this.permitted;
			}
		}

		public Asn1Sequence ExcludedSubtrees
		{
			get
			{
				return this.excluded;
			}
		}

		public static NameConstraints GetInstance(object obj)
		{
			if (obj == null || obj is NameConstraints)
			{
				return (NameConstraints)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new NameConstraints((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public NameConstraints(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.permitted = Asn1Sequence.GetInstance(asn1TaggedObject, false);
					break;
				case 1:
					this.excluded = Asn1Sequence.GetInstance(asn1TaggedObject, false);
					break;
				}
			}
		}

		public NameConstraints(ArrayList permitted, ArrayList excluded) : this(permitted, excluded)
		{
		}

		public NameConstraints(IList permitted, IList excluded)
		{
			if (permitted != null)
			{
				this.permitted = this.CreateSequence(permitted);
			}
			if (excluded != null)
			{
				this.excluded = this.CreateSequence(excluded);
			}
		}

		private DerSequence CreateSequence(IList subtrees)
		{
			GeneralSubtree[] array = new GeneralSubtree[subtrees.Count];
			for (int i = 0; i < subtrees.Count; i++)
			{
				array[i] = (GeneralSubtree)subtrees[i];
			}
			return new DerSequence(array);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.permitted != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.permitted)
				});
			}
			if (this.excluded != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.excluded)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
