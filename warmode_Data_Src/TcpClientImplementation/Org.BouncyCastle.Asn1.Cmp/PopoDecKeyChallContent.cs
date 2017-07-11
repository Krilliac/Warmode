using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PopoDecKeyChallContent : Asn1Encodable
	{
		private readonly Asn1Sequence content;

		private PopoDecKeyChallContent(Asn1Sequence seq)
		{
			this.content = seq;
		}

		public static PopoDecKeyChallContent GetInstance(object obj)
		{
			if (obj is PopoDecKeyChallContent)
			{
				return (PopoDecKeyChallContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PopoDecKeyChallContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public virtual Challenge[] ToChallengeArray()
		{
			Challenge[] array = new Challenge[this.content.Count];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = Challenge.GetInstance(this.content[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.content;
		}
	}
}
