using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiMessages : Asn1Encodable
	{
		private Asn1Sequence content;

		private PkiMessages(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static PkiMessages GetInstance(object obj)
		{
			if (obj is PkiMessages)
			{
				return (PkiMessages)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PkiMessages((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public PkiMessages(params PkiMessage[] msgs)
		{
			this.content = new DerSequence(msgs);
		}

		public virtual PkiMessage[] ToPkiMessageArray()
		{
			PkiMessage[] array = new PkiMessage[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = PkiMessage.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
