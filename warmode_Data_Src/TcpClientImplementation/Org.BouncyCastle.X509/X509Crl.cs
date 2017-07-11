using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.X509
{
	public class X509Crl : X509ExtensionBase
	{
		private readonly CertificateList c;

		private readonly string sigAlgName;

		private readonly byte[] sigAlgParams;

		private readonly bool isIndirect;

		public virtual int Version
		{
			get
			{
				return this.c.Version;
			}
		}

		public virtual X509Name IssuerDN
		{
			get
			{
				return this.c.Issuer;
			}
		}

		public virtual DateTime ThisUpdate
		{
			get
			{
				return this.c.ThisUpdate.ToDateTime();
			}
		}

		public virtual DateTimeObject NextUpdate
		{
			get
			{
				if (this.c.NextUpdate != null)
				{
					return new DateTimeObject(this.c.NextUpdate.ToDateTime());
				}
				return null;
			}
		}

		public virtual string SigAlgName
		{
			get
			{
				return this.sigAlgName;
			}
		}

		public virtual string SigAlgOid
		{
			get
			{
				return this.c.SignatureAlgorithm.ObjectID.Id;
			}
		}

		protected virtual bool IsIndirectCrl
		{
			get
			{
				Asn1OctetString extensionValue = this.GetExtensionValue(X509Extensions.IssuingDistributionPoint);
				bool result = false;
				try
				{
					if (extensionValue != null)
					{
						result = IssuingDistributionPoint.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).IsIndirectCrl;
					}
				}
				catch (Exception arg)
				{
					throw new CrlException("Exception reading IssuingDistributionPoint" + arg);
				}
				return result;
			}
		}

		public X509Crl(CertificateList c)
		{
			this.c = c;
			try
			{
				this.sigAlgName = X509SignatureUtilities.GetSignatureName(c.SignatureAlgorithm);
				if (c.SignatureAlgorithm.Parameters != null)
				{
					this.sigAlgParams = c.SignatureAlgorithm.Parameters.GetDerEncoded();
				}
				else
				{
					this.sigAlgParams = null;
				}
				this.isIndirect = this.IsIndirectCrl;
			}
			catch (Exception arg)
			{
				throw new CrlException("CRL contents invalid: " + arg);
			}
		}

		protected override X509Extensions GetX509Extensions()
		{
			if (this.Version != 2)
			{
				return null;
			}
			return this.c.TbsCertList.Extensions;
		}

		public virtual byte[] GetEncoded()
		{
			byte[] derEncoded;
			try
			{
				derEncoded = this.c.GetDerEncoded();
			}
			catch (Exception ex)
			{
				throw new CrlException(ex.ToString());
			}
			return derEncoded;
		}

		public virtual void Verify(AsymmetricKeyParameter publicKey)
		{
			if (!this.c.SignatureAlgorithm.Equals(this.c.TbsCertList.Signature))
			{
				throw new CrlException("Signature algorithm on CertificateList does not match TbsCertList.");
			}
			ISigner signer = SignerUtilities.GetSigner(this.SigAlgName);
			signer.Init(false, publicKey);
			byte[] tbsCertList = this.GetTbsCertList();
			signer.BlockUpdate(tbsCertList, 0, tbsCertList.Length);
			if (!signer.VerifySignature(this.GetSignature()))
			{
				throw new SignatureException("CRL does not verify with supplied public key.");
			}
		}

		private ISet LoadCrlEntries()
		{
			ISet set = new HashSet();
			IEnumerable revokedCertificateEnumeration = this.c.GetRevokedCertificateEnumeration();
			X509Name previousCertificateIssuer = this.IssuerDN;
			foreach (CrlEntry crlEntry in revokedCertificateEnumeration)
			{
				X509CrlEntry x509CrlEntry = new X509CrlEntry(crlEntry, this.isIndirect, previousCertificateIssuer);
				set.Add(x509CrlEntry);
				previousCertificateIssuer = x509CrlEntry.GetCertificateIssuer();
			}
			return set;
		}

		public virtual X509CrlEntry GetRevokedCertificate(BigInteger serialNumber)
		{
			IEnumerable revokedCertificateEnumeration = this.c.GetRevokedCertificateEnumeration();
			X509Name previousCertificateIssuer = this.IssuerDN;
			foreach (CrlEntry crlEntry in revokedCertificateEnumeration)
			{
				X509CrlEntry x509CrlEntry = new X509CrlEntry(crlEntry, this.isIndirect, previousCertificateIssuer);
				if (serialNumber.Equals(crlEntry.UserCertificate.Value))
				{
					return x509CrlEntry;
				}
				previousCertificateIssuer = x509CrlEntry.GetCertificateIssuer();
			}
			return null;
		}

		public virtual ISet GetRevokedCertificates()
		{
			ISet set = this.LoadCrlEntries();
			if (set.Count > 0)
			{
				return set;
			}
			return null;
		}

		public virtual byte[] GetTbsCertList()
		{
			byte[] derEncoded;
			try
			{
				derEncoded = this.c.TbsCertList.GetDerEncoded();
			}
			catch (Exception ex)
			{
				throw new CrlException(ex.ToString());
			}
			return derEncoded;
		}

		public virtual byte[] GetSignature()
		{
			return this.c.Signature.GetBytes();
		}

		public virtual byte[] GetSigAlgParams()
		{
			return Arrays.Clone(this.sigAlgParams);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			X509Crl x509Crl = obj as X509Crl;
			return x509Crl != null && this.c.Equals(x509Crl.c);
		}

		public override int GetHashCode()
		{
			return this.c.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("              Version: ").Append(this.Version).Append(newLine);
			stringBuilder.Append("             IssuerDN: ").Append(this.IssuerDN).Append(newLine);
			stringBuilder.Append("          This update: ").Append(this.ThisUpdate).Append(newLine);
			stringBuilder.Append("          Next update: ").Append(this.NextUpdate).Append(newLine);
			stringBuilder.Append("  Signature Algorithm: ").Append(this.SigAlgName).Append(newLine);
			byte[] signature = this.GetSignature();
			stringBuilder.Append("            Signature: ");
			stringBuilder.Append(Hex.ToHexString(signature, 0, 20)).Append(newLine);
			for (int i = 20; i < signature.Length; i += 20)
			{
				int length = Math.Min(20, signature.Length - i);
				stringBuilder.Append("                       ");
				stringBuilder.Append(Hex.ToHexString(signature, i, length)).Append(newLine);
			}
			X509Extensions extensions = this.c.TbsCertList.Extensions;
			if (extensions != null)
			{
				IEnumerator enumerator = extensions.ExtensionOids.GetEnumerator();
				if (enumerator.MoveNext())
				{
					stringBuilder.Append("           Extensions: ").Append(newLine);
				}
				while (true)
				{
					DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)enumerator.Current;
					X509Extension extension = extensions.GetExtension(derObjectIdentifier);
					if (extension.Value != null)
					{
						Asn1Object asn1Object = X509ExtensionUtilities.FromExtensionValue(extension.Value);
						stringBuilder.Append("                       critical(").Append(extension.IsCritical).Append(") ");
						try
						{
							if (derObjectIdentifier.Equals(X509Extensions.CrlNumber))
							{
								stringBuilder.Append(new CrlNumber(DerInteger.GetInstance(asn1Object).PositiveValue)).Append(newLine);
							}
							else if (derObjectIdentifier.Equals(X509Extensions.DeltaCrlIndicator))
							{
								stringBuilder.Append("Base CRL: " + new CrlNumber(DerInteger.GetInstance(asn1Object).PositiveValue)).Append(newLine);
							}
							else if (derObjectIdentifier.Equals(X509Extensions.IssuingDistributionPoint))
							{
								stringBuilder.Append(IssuingDistributionPoint.GetInstance((Asn1Sequence)asn1Object)).Append(newLine);
							}
							else if (derObjectIdentifier.Equals(X509Extensions.CrlDistributionPoints))
							{
								stringBuilder.Append(CrlDistPoint.GetInstance((Asn1Sequence)asn1Object)).Append(newLine);
							}
							else if (derObjectIdentifier.Equals(X509Extensions.FreshestCrl))
							{
								stringBuilder.Append(CrlDistPoint.GetInstance((Asn1Sequence)asn1Object)).Append(newLine);
							}
							else
							{
								stringBuilder.Append(derObjectIdentifier.Id);
								stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object)).Append(newLine);
							}
							goto IL_2EC;
						}
						catch (Exception)
						{
							stringBuilder.Append(derObjectIdentifier.Id);
							stringBuilder.Append(" value = ").Append("*****").Append(newLine);
							goto IL_2EC;
						}
						goto IL_2E4;
					}
					goto IL_2E4;
					IL_2EC:
					if (!enumerator.MoveNext())
					{
						break;
					}
					continue;
					IL_2E4:
					stringBuilder.Append(newLine);
					goto IL_2EC;
				}
			}
			ISet revokedCertificates = this.GetRevokedCertificates();
			if (revokedCertificates != null)
			{
				foreach (X509CrlEntry value in revokedCertificates)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(newLine);
				}
			}
			return stringBuilder.ToString();
		}

		public virtual bool IsRevoked(X509Certificate cert)
		{
			CrlEntry[] revokedCertificates = this.c.GetRevokedCertificates();
			if (revokedCertificates != null)
			{
				BigInteger serialNumber = cert.SerialNumber;
				for (int i = 0; i < revokedCertificates.Length; i++)
				{
					if (revokedCertificates[i].UserCertificate.Value.Equals(serialNumber))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
