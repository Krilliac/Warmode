using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class CertificateValues : Asn1Encodable
	{
		private readonly Asn1Sequence certificates;

		public static CertificateValues GetInstance(object obj)
		{
			if (obj == null || obj is CertificateValues)
			{
				return (CertificateValues)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CertificateValues((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'CertificateValues' factory: " + obj.GetType().Name, "obj");
		}

		private CertificateValues(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			foreach (Asn1Encodable asn1Encodable in seq)
			{
				X509CertificateStructure.GetInstance(asn1Encodable.ToAsn1Object());
			}
			this.certificates = seq;
		}

		public CertificateValues(params X509CertificateStructure[] certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			this.certificates = new DerSequence(certificates);
		}

		public CertificateValues(IEnumerable certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			if (!CollectionUtilities.CheckElementsAreOfType(certificates, typeof(X509CertificateStructure)))
			{
				throw new ArgumentException("Must contain only 'X509CertificateStructure' objects", "certificates");
			}
			this.certificates = new DerSequence(Asn1EncodableVector.FromEnumerable(certificates));
		}

		public X509CertificateStructure[] GetCertificates()
		{
			X509CertificateStructure[] array = new X509CertificateStructure[this.certificates.Count];
			for (int i = 0; i < this.certificates.Count; i++)
			{
				array[i] = X509CertificateStructure.GetInstance(this.certificates[i]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.certificates;
		}
	}
}
