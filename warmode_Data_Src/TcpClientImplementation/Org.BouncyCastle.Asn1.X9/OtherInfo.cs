using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X9
{
	public class OtherInfo : Asn1Encodable
	{
		private KeySpecificInfo keyInfo;

		private Asn1OctetString partyAInfo;

		private Asn1OctetString suppPubInfo;

		public KeySpecificInfo KeyInfo
		{
			get
			{
				return this.keyInfo;
			}
		}

		public Asn1OctetString PartyAInfo
		{
			get
			{
				return this.partyAInfo;
			}
		}

		public Asn1OctetString SuppPubInfo
		{
			get
			{
				return this.suppPubInfo;
			}
		}

		public OtherInfo(KeySpecificInfo keyInfo, Asn1OctetString partyAInfo, Asn1OctetString suppPubInfo)
		{
			this.keyInfo = keyInfo;
			this.partyAInfo = partyAInfo;
			this.suppPubInfo = suppPubInfo;
		}

		public OtherInfo(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			this.keyInfo = new KeySpecificInfo((Asn1Sequence)enumerator.Current);
			while (enumerator.MoveNext())
			{
				DerTaggedObject derTaggedObject = (DerTaggedObject)enumerator.Current;
				if (derTaggedObject.TagNo == 0)
				{
					this.partyAInfo = (Asn1OctetString)derTaggedObject.GetObject();
				}
				else if (derTaggedObject.TagNo == 2)
				{
					this.suppPubInfo = (Asn1OctetString)derTaggedObject.GetObject();
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.keyInfo
			});
			if (this.partyAInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(0, this.partyAInfo)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				new DerTaggedObject(2, this.suppPubInfo)
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
