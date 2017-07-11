using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509.Store;
using System;

namespace Org.BouncyCastle.X509
{
	public class AttributeCertificateIssuer : IX509Selector, ICloneable
	{
		internal readonly Asn1Encodable form;

		public AttributeCertificateIssuer(AttCertIssuer issuer)
		{
			this.form = issuer.Issuer;
		}

		public AttributeCertificateIssuer(X509Name principal)
		{
			this.form = new V2Form(new GeneralNames(new GeneralName(principal)));
		}

		private object[] GetNames()
		{
			GeneralNames generalNames;
			if (this.form is V2Form)
			{
				generalNames = ((V2Form)this.form).IssuerName;
			}
			else
			{
				generalNames = (GeneralNames)this.form;
			}
			GeneralName[] names = generalNames.GetNames();
			int num = 0;
			for (int num2 = 0; num2 != names.Length; num2++)
			{
				if (names[num2].TagNo == 4)
				{
					num++;
				}
			}
			object[] array = new object[num];
			int num3 = 0;
			for (int num4 = 0; num4 != names.Length; num4++)
			{
				if (names[num4].TagNo == 4)
				{
					array[num3++] = X509Name.GetInstance(names[num4].Name);
				}
			}
			return array;
		}

		public X509Name[] GetPrincipals()
		{
			object[] names = this.GetNames();
			int num = 0;
			for (int num2 = 0; num2 != names.Length; num2++)
			{
				if (names[num2] is X509Name)
				{
					num++;
				}
			}
			X509Name[] array = new X509Name[num];
			int num3 = 0;
			for (int num4 = 0; num4 != names.Length; num4++)
			{
				if (names[num4] is X509Name)
				{
					array[num3++] = (X509Name)names[num4];
				}
			}
			return array;
		}

		private bool MatchesDN(X509Name subject, GeneralNames targets)
		{
			GeneralName[] names = targets.GetNames();
			for (int num = 0; num != names.Length; num++)
			{
				GeneralName generalName = names[num];
				if (generalName.TagNo == 4)
				{
					try
					{
						if (X509Name.GetInstance(generalName.Name).Equivalent(subject))
						{
							return true;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return false;
		}

		public object Clone()
		{
			return new AttributeCertificateIssuer(AttCertIssuer.GetInstance(this.form));
		}

		public bool Match(X509Certificate x509Cert)
		{
			if (!(this.form is V2Form))
			{
				return this.MatchesDN(x509Cert.SubjectDN, (GeneralNames)this.form);
			}
			V2Form v2Form = (V2Form)this.form;
			if (v2Form.BaseCertificateID != null)
			{
				return v2Form.BaseCertificateID.Serial.Value.Equals(x509Cert.SerialNumber) && this.MatchesDN(x509Cert.IssuerDN, v2Form.BaseCertificateID.Issuer);
			}
			return this.MatchesDN(x509Cert.SubjectDN, v2Form.IssuerName);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is AttributeCertificateIssuer))
			{
				return false;
			}
			AttributeCertificateIssuer attributeCertificateIssuer = (AttributeCertificateIssuer)obj;
			return this.form.Equals(attributeCertificateIssuer.form);
		}

		public override int GetHashCode()
		{
			return this.form.GetHashCode();
		}

		public bool Match(object obj)
		{
			return obj is X509Certificate && this.Match((X509Certificate)obj);
		}
	}
}
