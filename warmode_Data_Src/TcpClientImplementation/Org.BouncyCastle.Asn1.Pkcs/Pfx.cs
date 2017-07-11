using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class Pfx : Asn1Encodable
	{
		private ContentInfo contentInfo;

		private MacData macData;

		public ContentInfo AuthSafe
		{
			get
			{
				return this.contentInfo;
			}
		}

		public MacData MacData
		{
			get
			{
				return this.macData;
			}
		}

		public Pfx(Asn1Sequence seq)
		{
			BigInteger value = ((DerInteger)seq[0]).Value;
			if (value.IntValue != 3)
			{
				throw new ArgumentException("wrong version for PFX PDU");
			}
			this.contentInfo = ContentInfo.GetInstance(seq[1]);
			if (seq.Count == 3)
			{
				this.macData = MacData.GetInstance(seq[2]);
			}
		}

		public Pfx(ContentInfo contentInfo, MacData macData)
		{
			this.contentInfo = contentInfo;
			this.macData = macData;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				new DerInteger(3),
				this.contentInfo
			});
			if (this.macData != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.macData
				});
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
