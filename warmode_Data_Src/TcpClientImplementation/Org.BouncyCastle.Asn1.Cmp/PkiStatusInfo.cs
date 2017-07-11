using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class PkiStatusInfo : Asn1Encodable
	{
		private DerInteger status;

		private PkiFreeText statusString;

		private DerBitString failInfo;

		public BigInteger Status
		{
			get
			{
				return this.status.Value;
			}
		}

		public PkiFreeText StatusString
		{
			get
			{
				return this.statusString;
			}
		}

		public DerBitString FailInfo
		{
			get
			{
				return this.failInfo;
			}
		}

		public static PkiStatusInfo GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return PkiStatusInfo.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static PkiStatusInfo GetInstance(object obj)
		{
			if (obj is PkiStatusInfo)
			{
				return (PkiStatusInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PkiStatusInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public PkiStatusInfo(Asn1Sequence seq)
		{
			this.status = DerInteger.GetInstance(seq[0]);
			this.statusString = null;
			this.failInfo = null;
			if (seq.Count > 2)
			{
				this.statusString = PkiFreeText.GetInstance(seq[1]);
				this.failInfo = DerBitString.GetInstance(seq[2]);
				return;
			}
			if (seq.Count > 1)
			{
				object obj = seq[1];
				if (obj is DerBitString)
				{
					this.failInfo = DerBitString.GetInstance(obj);
					return;
				}
				this.statusString = PkiFreeText.GetInstance(obj);
			}
		}

		public PkiStatusInfo(int status)
		{
			this.status = new DerInteger(status);
		}

		public PkiStatusInfo(int status, PkiFreeText statusString)
		{
			this.status = new DerInteger(status);
			this.statusString = statusString;
		}

		public PkiStatusInfo(int status, PkiFreeText statusString, PkiFailureInfo failInfo)
		{
			this.status = new DerInteger(status);
			this.statusString = statusString;
			this.failInfo = failInfo;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.status
			});
			if (this.statusString != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.statusString
				});
			}
			if (this.failInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.failInfo
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
