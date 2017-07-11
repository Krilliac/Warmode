using Org.BouncyCastle.Asn1.Cms;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class PopoPrivKey : Asn1Encodable, IAsn1Choice
	{
		public const int thisMessage = 0;

		public const int subsequentMessage = 1;

		public const int dhMAC = 2;

		public const int agreeMAC = 3;

		public const int encryptedKey = 4;

		private readonly int tagNo;

		private readonly Asn1Encodable obj;

		public virtual int Type
		{
			get
			{
				return this.tagNo;
			}
		}

		public virtual Asn1Encodable Value
		{
			get
			{
				return this.obj;
			}
		}

		private PopoPrivKey(Asn1TaggedObject obj)
		{
			this.tagNo = obj.TagNo;
			switch (this.tagNo)
			{
			case 0:
				this.obj = DerBitString.GetInstance(obj, false);
				return;
			case 1:
				this.obj = SubsequentMessage.ValueOf(DerInteger.GetInstance(obj, false).Value.IntValue);
				return;
			case 2:
				this.obj = DerBitString.GetInstance(obj, false);
				return;
			case 3:
				this.obj = PKMacValue.GetInstance(obj, false);
				return;
			case 4:
				this.obj = EnvelopedData.GetInstance(obj, false);
				return;
			default:
				throw new ArgumentException("unknown tag in PopoPrivKey", "obj");
			}
		}

		public static PopoPrivKey GetInstance(Asn1TaggedObject tagged, bool isExplicit)
		{
			return new PopoPrivKey(Asn1TaggedObject.GetInstance(tagged.GetObject()));
		}

		public PopoPrivKey(SubsequentMessage msg)
		{
			this.tagNo = 1;
			this.obj = msg;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerTaggedObject(false, this.tagNo, this.obj);
		}
	}
}
