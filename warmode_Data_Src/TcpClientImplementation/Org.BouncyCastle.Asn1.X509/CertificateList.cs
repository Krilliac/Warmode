using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CertificateList : Asn1Encodable
	{
		private readonly TbsCertificateList tbsCertList;

		private readonly AlgorithmIdentifier sigAlgID;

		private readonly DerBitString sig;

		public TbsCertificateList TbsCertList
		{
			get
			{
				return this.tbsCertList;
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

		public int Version
		{
			get
			{
				return this.tbsCertList.Version;
			}
		}

		public X509Name Issuer
		{
			get
			{
				return this.tbsCertList.Issuer;
			}
		}

		public Time ThisUpdate
		{
			get
			{
				return this.tbsCertList.ThisUpdate;
			}
		}

		public Time NextUpdate
		{
			get
			{
				return this.tbsCertList.NextUpdate;
			}
		}

		public static CertificateList GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return CertificateList.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static CertificateList GetInstance(object obj)
		{
			if (obj is CertificateList)
			{
				return (CertificateList)obj;
			}
			if (obj != null)
			{
				return new CertificateList(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		private CertificateList(Asn1Sequence seq)
		{
			if (seq.Count != 3)
			{
				throw new ArgumentException("sequence wrong size for CertificateList", "seq");
			}
			this.tbsCertList = TbsCertificateList.GetInstance(seq[0]);
			this.sigAlgID = AlgorithmIdentifier.GetInstance(seq[1]);
			this.sig = DerBitString.GetInstance(seq[2]);
		}

		public CrlEntry[] GetRevokedCertificates()
		{
			return this.tbsCertList.GetRevokedCertificates();
		}

		public IEnumerable GetRevokedCertificateEnumeration()
		{
			return this.tbsCertList.GetRevokedCertificateEnumeration();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.tbsCertList,
				this.sigAlgID,
				this.sig
			});
		}
	}
}
