using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class UserNotice : Asn1Encodable
	{
		private readonly NoticeReference noticeRef;

		private readonly DisplayText explicitText;

		public virtual NoticeReference NoticeRef
		{
			get
			{
				return this.noticeRef;
			}
		}

		public virtual DisplayText ExplicitText
		{
			get
			{
				return this.explicitText;
			}
		}

		public UserNotice(NoticeReference noticeRef, DisplayText explicitText)
		{
			this.noticeRef = noticeRef;
			this.explicitText = explicitText;
		}

		public UserNotice(NoticeReference noticeRef, string str) : this(noticeRef, new DisplayText(str))
		{
		}

		public UserNotice(Asn1Sequence seq)
		{
			if (seq.Count == 2)
			{
				this.noticeRef = NoticeReference.GetInstance(seq[0]);
				this.explicitText = DisplayText.GetInstance(seq[1]);
				return;
			}
			if (seq.Count != 1)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			if (seq[0].ToAsn1Object() is Asn1Sequence)
			{
				this.noticeRef = NoticeReference.GetInstance(seq[0]);
				return;
			}
			this.explicitText = DisplayText.GetInstance(seq[0]);
		}

		public static UserNotice GetInstance(object obj)
		{
			if (obj is UserNotice)
			{
				return (UserNotice)obj;
			}
			if (obj == null)
			{
				return null;
			}
			return new UserNotice(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.noticeRef != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.noticeRef
				});
			}
			if (this.explicitText != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.explicitText
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
