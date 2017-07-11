using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class X509CertificateStructure : Asn1Encodable
	{
		private readonly TbsCertificateStructure tbsCert;

		private readonly AlgorithmIdentifier sigAlgID;

		private readonly DerBitString sig;

		public TbsCertificateStructure TbsCertificate
		{
			get
			{
				return this.tbsCert;
			}
		}

		public int Version
		{
			get
			{
				return this.tbsCert.Version;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return this.tbsCert.SerialNumber;
			}
		}

		public X509Name Issuer
		{
			get
			{
				return this.tbsCert.Issuer;
			}
		}

		public Time StartDate
		{
			get
			{
				return this.tbsCert.StartDate;
			}
		}

		public Time EndDate
		{
			get
			{
				return this.tbsCert.EndDate;
			}
		}

		public X509Name Subject
		{
			get
			{
				return this.tbsCert.Subject;
			}
		}

		public SubjectPublicKeyInfo SubjectPublicKeyInfo
		{
			get
			{
				return this.tbsCert.SubjectPublicKeyInfo;
			}
		}

		public AlgorithmIdentifier SignatureAlgorithm
		{
			get
			{
				return this.sigAlgID;
			}
		}

		public DerBitString Signature
		{
			get
			{
				return this.sig;
			}
		}

		public static X509CertificateStructure GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return X509CertificateStructure.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static X509CertificateStructure GetInstance(object obj)
		{
			if (obj is X509CertificateStructure)
			{
				return (X509CertificateStructure)obj;
			}
			if (obj == null)
			{
				return null;
			}
			return new X509CertificateStructure(Asn1Sequence.GetInstance(obj));
		}

		public X509CertificateStructure(TbsCertificateStructure tbsCert, AlgorithmIdentifier sigAlgID, DerBitString sig)
		{
			if (tbsCert == null)
			{
				throw new ArgumentNullException("tbsCert");
			}
			if (sigAlgID == null)
			{
				throw new ArgumentNullException("sigAlgID");
			}
			if (sig == null)
			{
				throw new ArgumentNullException("sig");
			}
			this.tbsCert = tbsCert;
			this.sigAlgID = sigAlgID;
			this.sig = sig;
		}

		private X509CertificateStructure(Asn1Sequence seq)
		{
			if (seq.Count != 3)
			{
				throw new ArgumentException("sequence wrong size for a certificate", "seq");
			}
			this.tbsCert = TbsCertificateStructure.GetInstance(seq[0]);
			this.sigAlgID = AlgorithmIdentifier.GetInstance(seq[1]);
			this.sig = DerBitString.GetInstance(seq[2]);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.tbsCert,
				this.sigAlgID,
				this.sig
			});
		}
	}
}
