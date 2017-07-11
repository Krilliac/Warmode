using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ess
{
	public class EssCertID : Asn1Encodable
	{
		private Asn1OctetString certHash;

		private IssuerSerial issuerSerial;

		public IssuerSerial IssuerSerial
		{
			get
			{
				return this.issuerSerial;
			}
		}

		public static EssCertID GetInstance(object o)
		{
			if (o == null || o is EssCertID)
			{
				return (EssCertID)o;
			}
			if (o is Asn1Sequence)
			{
				return new EssCertID((Asn1Sequence)o);
			}
			throw new ArgumentException("unknown object in 'EssCertID' factory : " + o.GetType().Name + ".");
		}

		public EssCertID(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.certHash = Asn1OctetString.GetInstance(seq[0]);
			if (seq.Count > 1)
			{
				this.issuerSerial = IssuerSerial.GetInstance(seq[1]);
			}
		}

		public EssCertID(byte[] hash)
		{
			this.certHash = new DerOctetString(hash);
		}

		public EssCertID(byte[] hash, IssuerSerial issuerSerial)
		{
			this.certHash = new DerOctetString(hash);
			this.issuerSerial = issuerSerial;
		}

		public byte[] GetCertHash()
		{
			return this.certHash.GetOctets();
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.certHash
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
