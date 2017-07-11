using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Text;

namespace Org.BouncyCastle.Pkix
{
	public class TrustAnchor
	{
		private readonly AsymmetricKeyParameter pubKey;

		private readonly string caName;

		private readonly X509Name caPrincipal;

		private readonly X509Certificate trustedCert;

		private byte[] ncBytes;

		private NameConstraints nc;

		public X509Certificate TrustedCert
		{
			get
			{
				return this.trustedCert;
			}
		}

		public X509Name CA
		{
			get
			{
				return this.caPrincipal;
			}
		}

		public string CAName
		{
			get
			{
				return this.caName;
			}
		}

		public AsymmetricKeyParameter CAPublicKey
		{
			get
			{
				return this.pubKey;
			}
		}

		public byte[] GetNameConstraints
		{
			get
			{
				return Arrays.Clone(this.ncBytes);
			}
		}

		public TrustAnchor(X509Certificate trustedCert, byte[] nameConstraints)
		{
			if (trustedCert == null)
			{
				throw new ArgumentNullException("trustedCert");
			}
			this.trustedCert = trustedCert;
			this.pubKey = null;
			this.caName = null;
			this.caPrincipal = null;
			this.setNameConstraints(nameConstraints);
		}

		public TrustAnchor(X509Name caPrincipal, AsymmetricKeyParameter pubKey, byte[] nameConstraints)
		{
			if (caPrincipal == null)
			{
				throw new ArgumentNullException("caPrincipal");
			}
			if (pubKey == null)
			{
				throw new ArgumentNullException("pubKey");
			}
			this.trustedCert = null;
			this.caPrincipal = caPrincipal;
			this.caName = caPrincipal.ToString();
			this.pubKey = pubKey;
			this.setNameConstraints(nameConstraints);
		}

		public TrustAnchor(string caName, AsymmetricKeyParameter pubKey, byte[] nameConstraints)
		{
			if (caName == null)
			{
				throw new ArgumentNullException("caName");
			}
			if (pubKey == null)
			{
				throw new ArgumentNullException("pubKey");
			}
			if (caName.Length == 0)
			{
				throw new ArgumentException("caName can not be an empty string");
			}
			this.caPrincipal = new X509Name(caName);
			this.pubKey = pubKey;
			this.caName = caName;
			this.trustedCert = null;
			this.setNameConstraints(nameConstraints);
		}

		private void setNameConstraints(byte[] bytes)
		{
			if (bytes == null)
			{
				this.ncBytes = null;
				this.nc = null;
				return;
			}
			this.ncBytes = (byte[])bytes.Clone();
			this.nc = NameConstraints.GetInstance(Asn1Object.FromByteArray(bytes));
		}

		public override string ToString()
		{
			string newLine = Platform.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			stringBuilder.Append(newLine);
			if (this.pubKey != null)
			{
				stringBuilder.Append("  Trusted CA Public Key: ").Append(this.pubKey).Append(newLine);
				stringBuilder.Append("  Trusted CA Issuer Name: ").Append(this.caName).Append(newLine);
			}
			else
			{
				stringBuilder.Append("  Trusted CA cert: ").Append(this.TrustedCert).Append(newLine);
			}
			if (this.nc != null)
			{
				stringBuilder.Append("  Name Constraints: ").Append(this.nc).Append(newLine);
			}
			return stringBuilder.ToString();
		}
	}
}
