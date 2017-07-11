using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1.Ess
{
	public class EssCertIDv2 : Asn1Encodable
	{
		private readonly AlgorithmIdentifier hashAlgorithm;

		private readonly byte[] certHash;

		private readonly IssuerSerial issuerSerial;

		private static readonly AlgorithmIdentifier DefaultAlgID = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256);

		public AlgorithmIdentifier HashAlgorithm
		{
			get
			{
				return this.hashAlgorithm;
			}
		}

		public IssuerSerial IssuerSerial
		{
			get
			{
				return this.issuerSerial;
			}
		}

		public static EssCertIDv2 GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			EssCertIDv2 essCertIDv = obj as EssCertIDv2;
			if (essCertIDv != null)
			{
				return essCertIDv;
			}
			return new EssCertIDv2(Asn1Sequence.GetInstance(obj));
		}

		private EssCertIDv2(Asn1Sequence seq)
		{
			if (seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			int num = 0;
			if (seq[0] is Asn1OctetString)
			{
				this.hashAlgorithm = EssCertIDv2.DefaultAlgID;
			}
			else
			{
				this.hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[num++].ToAsn1Object());
			}
			this.certHash = Asn1OctetString.GetInstance(seq[num++].ToAsn1Object()).GetOctets();
			if (seq.Count > num)
			{
				this.issuerSerial = IssuerSerial.GetInstance(Asn1Sequence.GetInstance(seq[num].ToAsn1Object()));
			}
		}

		public EssCertIDv2(byte[] certHash) : this(null, certHash, null)
		{
		}

		public EssCertIDv2(AlgorithmIdentifier algId, byte[] certHash) : this(algId, certHash, null)
		{
		}

		public EssCertIDv2(byte[] certHash, IssuerSerial issuerSerial) : this(null, certHash, issuerSerial)
		{
		}

		public EssCertIDv2(AlgorithmIdentifier algId, byte[] certHash, IssuerSerial issuerSerial)
		{
			if (algId == null)
			{
				this.hashAlgorithm = EssCertIDv2.DefaultAlgID;
			}
			else
			{
				this.hashAlgorithm = algId;
			}
			this.certHash = certHash;
			this.issuerSerial = issuerSerial;
		}

		public byte[] GetCertHash()
		{
			return Arrays.Clone(this.certHash);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (!this.hashAlgorithm.Equals(EssCertIDv2.DefaultAlgID))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.hashAlgorithm
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				new DerOctetString(this.certHash).ToAsn1Object()
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
