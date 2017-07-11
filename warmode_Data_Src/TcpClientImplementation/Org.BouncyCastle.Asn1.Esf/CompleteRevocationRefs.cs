using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class CompleteRevocationRefs : Asn1Encodable
	{
		private readonly Asn1Sequence crlOcspRefs;

		public static CompleteRevocationRefs GetInstance(object obj)
		{
			if (obj == null || obj is CompleteRevocationRefs)
			{
				return (CompleteRevocationRefs)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CompleteRevocationRefs((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'CompleteRevocationRefs' factory: " + obj.GetType().Name, "obj");
		}

		private CompleteRevocationRefs(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			foreach (Asn1Encodable asn1Encodable in seq)
			{
				CrlOcspRef.GetInstance(asn1Encodable.ToAsn1Object());
			}
			this.crlOcspRefs = seq;
		}

		public CompleteRevocationRefs(params CrlOcspRef[] crlOcspRefs)
		{
			if (crlOcspRefs == null)
			{
				throw new ArgumentNullException("crlOcspRefs");
			}
			this.crlOcspRefs = new DerSequence(crlOcspRefs);
		}

		public CompleteRevocationRefs(IEnumerable crlOcspRefs)
		{
			if (crlOcspRefs == null)
			{
				throw new ArgumentNullException("crlOcspRefs");
			}
			if (!CollectionUtilities.CheckElementsAreOfType(crlOcspRefs, typeof(CrlOcspRef)))
			{
				throw new ArgumentException("Must contain only 'CrlOcspRef' objects", "crlOcspRefs");
			}
			this.crlOcspRefs = new DerSequence(Asn1EncodableVector.FromEnumerable(crlOcspRefs));
		}

		public CrlOcspRef[] GetCrlOcspRefs()
		{
			CrlOcspRef[] array = new CrlOcspRef[this.crlOcspRefs.Count];
			for (int i = 0; i < this.crlOcspRefs.Count; i++)
			{
				array[i] = CrlOcspRef.GetInstance(this.crlOcspRefs[i].ToAsn1Object());
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.crlOcspRefs;
		}
	}
}
