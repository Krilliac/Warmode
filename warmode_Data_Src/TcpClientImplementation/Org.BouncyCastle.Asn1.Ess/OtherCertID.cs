using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ess
{
	[Obsolete("Use version in Asn1.Esf instead")]
	public class OtherCertID : Asn1Encodable
	{
		private Asn1Encodable otherCertHash;

		private IssuerSerial issuerSerial;

		public AlgorithmIdentifier AlgorithmHash
		{
			get
			{
				if (this.otherCertHash.ToAsn1Object() is Asn1OctetString)
				{
					return new AlgorithmIdentifier("1.3.14.3.2.26");
				}
				return DigestInfo.GetInstance(this.otherCertHash).AlgorithmID;
			}
		}

		public IssuerSerial IssuerSerial
		{
			get
			{
				return this.issuerSerial;
			}
		}

		public static OtherCertID GetInstance(object o)
		{
			if (o == null || o is OtherCertID)
			{
				return (OtherCertID)o;
			}
			if (o is Asn1Sequence)
			{
				return new OtherCertID((Asn1Sequence)o);
			}
			throw new ArgumentException("unknown object in 'OtherCertID' factory : " + o.GetType().Name + ".");
		}

		public OtherCertID(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			if (seq[0].ToAsn1Object() is Asn1OctetString)
			{
				this.otherCertHash = Asn1OctetString.GetInstance(seq[0]);
			}
			else
			{
				this.otherCertHash = DigestInfo.GetInstance(seq[0]);
			}
			if (seq.Count > 1)
			{
				this.issuerSerial = IssuerSerial.GetInstance(Asn1Sequence.GetInstance(seq[1]));
			}
		}

		public OtherCertID(AlgorithmIdentifier algId, byte[] digest)
		{
			this.otherCertHash = new DigestInfo(algId, digest);
		}

		public OtherCertID(AlgorithmIdentifier algId, byte[] digest, IssuerSerial issuerSerial)
		{
			this.otherCertHash = new DigestInfo(algId, digest);
			this.issuerSerial = issuerSerial;
		}

		public byte[] GetCertHash()
		{
			if (this.otherCertHash.ToAsn1Object() is Asn1OctetString)
			{
				return ((Asn1OctetString)this.otherCertHash.ToAsn1Object()).GetOctets();
			}
			return DigestInfo.GetInstance(this.otherCertHash).GetDigest();
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.otherCertHash
			});
			if (this.issuerSerial != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.issuerSerial
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
