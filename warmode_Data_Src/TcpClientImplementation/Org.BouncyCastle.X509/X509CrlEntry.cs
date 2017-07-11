using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.X509
{
	public class X509CrlEntry : X509ExtensionBase
	{
		private CrlEntry c;

		private bool isIndirect;

		private X509Name previousCertificateIssuer;

		private X509Name certificateIssuer;

		public BigInteger SerialNumber
		{
			get
			{
				return this.c.UserCertificate.Value;
			}
		}

		public DateTime RevocationDate
		{
			get
			{
				return this.c.RevocationDate.ToDateTime();
			}
		}

		public bool HasExtensions
		{
			get
			{
				return this.c.Extensions != null;
			}
		}

		public X509CrlEntry(CrlEntry c)
		{
			this.c = c;
			this.certificateIssuer = this.loadCertificateIssuer();
		}

		public X509CrlEntry(CrlEntry c, bool isIndirect, X509Name previousCertificateIssuer)
		{
			this.c = c;
			this.isIndirect = isIndirect;
			this.previousCertificateIssuer = previousCertificateIssuer;
			this.certificateIssuer = this.loadCertificateIssuer();
		}

		private X509Name loadCertificateIssuer()
		{
			if (!this.isIndirect)
			{
				return null;
			}
			Asn1OctetString extensionValue = this.GetExtensionValue(X509Extensions.CertificateIssuer);
			if (extensionValue == null)
			{
				return this.previousCertificateIssuer;
			}
			try
			{
				GeneralName[] names = GeneralNames.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).GetNames();
				for (int i = 0; i < names.Length; i++)
				{
					if (names[i].TagNo == 4)
					{
						return X509Name.GetInstance(names[i].Name);
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		public X509Name GetCertificateIssuer()
		{
			return this.certificateIssuer;
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.c.Extensions;
		}

		public byte[] GetEncoded()
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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("        userCertificate: ").Append(this.SerialNumber).Append(newLine);
			stringBuilder.Append("         revocationDate: ").Append(this.RevocationDate).Append(newLine);
			stringBuilder.Append("      certificateIssuer: ").Append(this.GetCertificateIssuer()).Append(newLine);
			X509Extensions extensions = this.c.Extensions;
			if (extensions != null)
			{
				IEnumerator enumerator = extensions.ExtensionOids.GetEnumerator();
				if (enumerator.MoveNext())
				{
					stringBuilder.Append("   crlEntryExtensions:").Append(newLine);
					while (true)
					{
						DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)enumerator.Current;
						X509Extension extension = extensions.GetExtension(derObjectIdentifier);
						if (extension.Value != null)
						{
							Asn1Object asn1Object = Asn1Object.FromByteArray(extension.Value.GetOctets());
							stringBuilder.Append("                       critical(").Append(extension.IsCritical).Append(") ");
							try
							{
								if (derObjectIdentifier.Equals(X509Extensions.ReasonCode))
								{
									stringBuilder.Append(new CrlReason(DerEnumerated.GetInstance(asn1Object)));
								}
								else if (derObjectIdentifier.Equals(X509Extensions.CertificateIssuer))
								{
									stringBuilder.Append("Certificate issuer: ").Append(GeneralNames.GetInstance((Asn1Sequence)asn1Object));
								}
								else
								{
									stringBuilder.Append(derObjectIdentifier.Id);
									stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object));
								}
								stringBuilder.Append(newLine);
								goto IL_1B0;
							}
							catch (Exception)
							{
								stringBuilder.Append(derObjectIdentifier.Id);
								stringBuilder.Append(" value = ").Append("*****").Append(newLine);
								goto IL_1B0;
							}
							goto IL_1A8;
						}
						goto IL_1A8;
						IL_1B0:
						if (!enumerator.MoveNext())
						{
							break;
						}
						continue;
						IL_1A8:
						stringBuilder.Append(newLine);
						goto IL_1B0;
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
