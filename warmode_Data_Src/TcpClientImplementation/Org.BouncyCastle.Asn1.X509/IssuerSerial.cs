using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class IssuerSerial : Asn1Encodable
	{
		internal readonly GeneralNames issuer;

		internal readonly DerInteger serial;

		internal readonly DerBitString issuerUid;

		public GeneralNames Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public DerInteger Serial
		{
			get
			{
				return this.serial;
			}
		}

		public DerBitString IssuerUid
		{
			get
			{
				return this.issuerUid;
			}
		}

		public static IssuerSerial GetInstance(object obj)
		{
			if (obj == null || obj is IssuerSerial)
			{
				return (IssuerSerial)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new IssuerSerial((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static IssuerSerial GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return IssuerSerial.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		private IssuerSerial(Asn1Sequence seq)
		{
			if (seq.Count != 2 && seq.Count != 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.issuer = GeneralNames.GetInstance(seq[0]);
			this.serial = DerInteger.GetInstance(seq[1]);
			if (seq.Count == 3)
			{
				this.issuerUid = DerBitString.GetInstance(seq[2]);
			}
		}

		public IssuerSerial(GeneralNames issuer, DerInteger serial)
		{
			this.issuer = issuer;
			this.serial = serial;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.issuer,
				this.serial
			});
			if (this.issuerUid != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.issuerUid
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
