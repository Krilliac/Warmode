using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PollReqContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private PollReqContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static PollReqContent GetInstance(object obj)
		{
			if (obj is PollReqContent)
			{
				return (PollReqContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PollReqContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual DerInteger[][] GetCertReqIDs()
		{
			DerInteger[][] array = new DerInteger[this.content.Count][];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = PollReqContent.SequenceToDerIntegerArray((Asn1Sequence)this.content[num]);
			}
			return array;
		}

		private static DerInteger[] SequenceToDerIntegerArray(Asn1Sequence seq)
		{
			DerInteger[] array = new DerInteger[seq.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = DerInteger.GetInstance(seq[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
