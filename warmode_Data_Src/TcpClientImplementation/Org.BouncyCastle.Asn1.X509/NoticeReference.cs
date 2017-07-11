using Org.BouncyCastle.Math;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class NoticeReference : Asn1Encodable
	{
		private readonly DisplayText organization;

		private readonly Asn1Sequence noticeNumbers;

		public virtual DisplayText Organization
		{
			get
			{
				return this.organization;
			}
		}

		private static Asn1EncodableVector ConvertVector(IList numbers)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (object current in numbers)
			{
				DerInteger derInteger;
				if (current is BigInteger)
				{
					derInteger = new DerInteger((BigInteger)current);
				}
				else
				{
					if (!(current is int))
					{
						throw new ArgumentException();
					}
					derInteger = new DerInteger((int)current);
				}
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					derInteger
				});
			}
			return asn1EncodableVector;
		}

		public NoticeReference(string organization, IList numbers) : this(organization, NoticeReference.ConvertVector(numbers))
		{
		}

		public NoticeReference(string organization, Asn1EncodableVector noticeNumbers) : this(new DisplayText(organization), noticeNumbers)
		{
		}

		public NoticeReference(DisplayText organization, Asn1EncodableVector noticeNumbers)
		{
			this.organization = organization;
			this.noticeNumbers = new DerSequence(noticeNumbers);
		}

		private NoticeReference(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.organization = DisplayText.GetInstance(seq[0]);
			this.noticeNumbers = Asn1Sequence.GetInstance(seq[1]);
		}

		public static NoticeReference GetInstance(object obj)
		{
			if (obj is NoticeReference)
			{
				return (NoticeReference)obj;
			}
			if (obj == null)
			{
				return null;
			}
			return new NoticeReference(Asn1Sequence.GetInstance(obj));
		}

		public virtual DerInteger[] GetNoticeNumbers()
		{
			DerInteger[] array = new DerInteger[this.noticeNumbers.Count];
			for (int num = 0; num != this.noticeNumbers.Count; num++)
			{
				array[num] = DerInteger.GetInstance(this.noticeNumbers[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.organization,
				this.noticeNumbers
			});
		}
	}
}
