using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class PolicyMappings : Asn1Encodable
	{
		private readonly Asn1Sequence seq;

		public PolicyMappings(Asn1Sequence seq)
		{
			this.seq = seq;
		}

		public PolicyMappings(Hashtable mappings) : this(mappings)
		{
		}

		public PolicyMappings(IDictionary mappings)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (string text in mappings.Keys)
			{
				string identifier = (string)mappings[text];
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerSequence(new Asn1Encodable[]
					{
						new DerObjectIdentifier(text),
						new DerObjectIdentifier(identifier)
					})
				});
			}
			this.seq = new DerSequence(asn1EncodableVector);
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}
	}
}
