using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class PkiPublicationInfo : Asn1Encodable
	{
		private readonly DerInteger action;

		private readonly Asn1Sequence pubInfos;

		public virtual DerInteger Action
		{
			get
			{
				return this.action;
			}
		}

		private PkiPublicationInfo(Asn1Sequence seq)
		{
			this.action = DerInteger.GetInstance(seq[0]);
			this.pubInfos = Asn1Sequence.GetInstance(seq[1]);
		}

		public static PkiPublicationInfo GetInstance(object obj)
		{
			if (obj is PkiPublicationInfo)
			{
				return (PkiPublicationInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PkiPublicationInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual SinglePubInfo[] GetPubInfos()
		{
			if (this.pubInfos == null)
			{
				return null;
			}
			SinglePubInfo[] array = new SinglePubInfo[this.pubInfos.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = SinglePubInfo.GetInstance(this.pubInfos[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.action,
				this.pubInfos
			});
		}
	}
}
