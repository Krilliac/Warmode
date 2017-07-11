using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using System;

namespace Org.BouncyCastle.X509
{
	public class X509CertificatePair
	{
		private readonly X509Certificate forward;

		private readonly X509Certificate reverse;

		public X509Certificate Forward
		{
			get
			{
				return this.forward;
			}
		}

		public X509Certificate Reverse
		{
			get
			{
				return this.reverse;
			}
		}

		public X509CertificatePair(X509Certificate forward, X509Certificate reverse)
		{
			this.forward = forward;
			this.reverse = reverse;
		}

		public X509CertificatePair(CertificatePair pair)
		{
			if (pair.Forward != null)
			{
				this.forward = new X509Certificate(pair.Forward);
			}
			if (pair.Reverse != null)
			{
				this.reverse = new X509Certificate(pair.Reverse);
			}
		}

		public byte[] GetEncoded()
		{
			byte[] derEncoded;
			try
			{
				X509CertificateStructure x509CertificateStructure = null;
				X509CertificateStructure x509CertificateStructure2 = null;
				if (this.forward != null)
				{
					x509CertificateStructure = X509CertificateStructure.GetInstance(Asn1Object.FromByteArray(this.forward.GetEncoded()));
					if (x509CertificateStructure == null)
					{
						throw new CertificateEncodingException("unable to get encoding for forward");
					}
				}
				if (this.reverse != null)
				{
					x509CertificateStructure2 = X509CertificateStructure.GetInstance(Asn1Object.FromByteArray(this.reverse.GetEncoded()));
					if (x509CertificateStructure2 == null)
					{
						throw new CertificateEncodingException("unable to get encoding for reverse");
					}
				}
				derEncoded = new CertificatePair(x509CertificateStructure, x509CertificateStructure2).GetDerEncoded();
			}
			catch (Exception ex)
			{
				throw new CertificateEncodingException(ex.Message, ex);
			}
			return derEncoded;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			X509CertificatePair x509CertificatePair = obj as X509CertificatePair;
			return x509CertificatePair != null && object.Equals(this.forward, x509CertificatePair.forward) && object.Equals(this.reverse, x509CertificatePair.reverse);
		}

		public override int GetHashCode()
		{
			int num = -1;
			if (this.forward != null)
			{
				num ^= this.forward.GetHashCode();
			}
			if (this.reverse != null)
			{
				num *= 17;
				num ^= this.reverse.GetHashCode();
			}
			return num;
		}
	}
}
