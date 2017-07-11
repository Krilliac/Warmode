using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class CrlListID : Asn1Encodable
	{
		private readonly Asn1Sequence crls;

		public static CrlListID GetInstance(object obj)
		{
			if (obj == null || obj is CrlListID)
			{
				return (CrlListID)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CrlListID((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'CrlListID' factory: " + obj.GetType().Name, "obj");
		}

		private CrlListID(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count != 1)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.crls = (Asn1Sequence)seq[0].ToAsn1Object();
			foreach (Asn1Encodable asn1Encodable in this.crls)
			{
				CrlValidatedID.GetInstance(asn1Encodable.ToAsn1Object());
			}
		}

		public CrlListID(params CrlValidatedID[] crls)
		{
			if (crls == null)
			{
				throw new ArgumentNullException("crls");
			}
			this.crls = new DerSequence(crls);
		}

		public CrlListID(IEnumerable crls)
		{
			if (crls == null)
			{
				throw new ArgumentNullException("crls");
			}
			if (!CollectionUtilities.CheckElementsAreOfType(crls, typeof(CrlValidatedID)))
			{
				throw new ArgumentException("Must contain only 'CrlValidatedID' objects", "crls");
			}
			this.crls = new DerSequence(Asn1EncodableVector.FromEnumerable(crls));
		}

		public CrlValidatedID[] GetCrls()
		{
			CrlValidatedID[] array = new CrlValidatedID[this.crls.Count];
			for (int i = 0; i < this.crls.Count; i++)
			{
				array[i] = CrlValidatedID.GetInstance(this.crls[i].ToAsn1Object());
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(this.crls);
		}
	}
}
