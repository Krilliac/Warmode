using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	public class X509CertStoreSelector : IX509Selector, ICloneable
	{
		private byte[] authorityKeyIdentifier;

		private int basicConstraints = -1;

		private X509Certificate certificate;

		private DateTimeObject certificateValid;

		private ISet extendedKeyUsage;

		private X509Name issuer;

		private bool[] keyUsage;

		private ISet policy;

		private DateTimeObject privateKeyValid;

		private BigInteger serialNumber;

		private X509Name subject;

		private byte[] subjectKeyIdentifier;

		private SubjectPublicKeyInfo subjectPublicKey;

		private DerObjectIdentifier subjectPublicKeyAlgID;

		public byte[] AuthorityKeyIdentifier
		{
			get
			{
				return Arrays.Clone(this.authorityKeyIdentifier);
			}
			set
			{
				this.authorityKeyIdentifier = Arrays.Clone(value);
			}
		}

		public int BasicConstraints
		{
			get
			{
				return this.basicConstraints;
			}
			set
			{
				if (value < -2)
				{
					throw new ArgumentException("value can't be less than -2", "value");
				}
				this.basicConstraints = value;
			}
		}

		public X509Certificate Certificate
		{
			get
			{
				return this.certificate;
			}
			set
			{
				this.certificate = value;
			}
		}

		public DateTimeObject CertificateValid
		{
			get
			{
				return this.certificateValid;
			}
			set
			{
				this.certificateValid = value;
			}
		}

		public ISet ExtendedKeyUsage
		{
			get
			{
				return X509CertStoreSelector.CopySet(this.extendedKeyUsage);
			}
			set
			{
				this.extendedKeyUsage = X509CertStoreSelector.CopySet(value);
			}
		}

		public X509Name Issuer
		{
			get
			{
				return this.issuer;
			}
			set
			{
				this.issuer = value;
			}
		}

		[Obsolete("Avoid working with X509Name objects in string form")]
		public string IssuerAsString
		{
			get
			{
				if (this.issuer == null)
				{
					return null;
				}
				return this.issuer.ToString();
			}
		}

		public bool[] KeyUsage
		{
			get
			{
				return X509CertStoreSelector.CopyBoolArray(this.keyUsage);
			}
			set
			{
				this.keyUsage = X509CertStoreSelector.CopyBoolArray(value);
			}
		}

		public ISet Policy
		{
			get
			{
				return X509CertStoreSelector.CopySet(this.policy);
			}
			set
			{
				this.policy = X509CertStoreSelector.CopySet(value);
			}
		}

		public DateTimeObject PrivateKeyValid
		{
			get
			{
				return this.privateKeyValid;
			}
			set
			{
				this.privateKeyValid = value;
			}
		}

		public BigInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
			set
			{
				this.serialNumber = value;
			}
		}

		public X509Name Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		public string SubjectAsString
		{
			get
			{
				if (this.subject == null)
				{
					return null;
				}
				return this.subject.ToString();
			}
		}

		public byte[] SubjectKeyIdentifier
		{
			get
			{
				return Arrays.Clone(this.subjectKeyIdentifier);
			}
			set
			{
				this.subjectKeyIdentifier = Arrays.Clone(value);
			}
		}

		public SubjectPublicKeyInfo SubjectPublicKey
		{
			get
			{
				return this.subjectPublicKey;
			}
			set
			{
				this.subjectPublicKey = value;
			}
		}

		public DerObjectIdentifier SubjectPublicKeyAlgID
		{
			get
			{
				return this.subjectPublicKeyAlgID;
			}
			set
			{
				this.subjectPublicKeyAlgID = value;
			}
		}

		public X509CertStoreSelector()
		{
		}

		public X509CertStoreSelector(X509CertStoreSelector o)
		{
			this.authorityKeyIdentifier = o.AuthorityKeyIdentifier;
			this.basicConstraints = o.BasicConstraints;
			this.certificate = o.Certificate;
			this.certificateValid = o.CertificateValid;
			this.extendedKeyUsage = o.ExtendedKeyUsage;
			this.issuer = o.Issuer;
			this.keyUsage = o.KeyUsage;
			this.policy = o.Policy;
			this.privateKeyValid = o.PrivateKeyValid;
			this.serialNumber = o.SerialNumber;
			this.subject = o.Subject;
			this.subjectKeyIdentifier = o.SubjectKeyIdentifier;
			this.subjectPublicKey = o.SubjectPublicKey;
			this.subjectPublicKeyAlgID = o.SubjectPublicKeyAlgID;
		}

		public virtual object Clone()
		{
			return new X509CertStoreSelector(this);
		}

		public virtual bool Match(object obj)
		{
			X509Certificate x509Certificate = obj as X509Certificate;
			if (x509Certificate == null)
			{
				return false;
			}
			if (!X509CertStoreSelector.MatchExtension(this.authorityKeyIdentifier, x509Certificate, X509Extensions.AuthorityKeyIdentifier))
			{
				return false;
			}
			if (this.basicConstraints != -1)
			{
				int num = x509Certificate.GetBasicConstraints();
				if (this.basicConstraints == -2)
				{
					if (num != -1)
					{
						return false;
					}
				}
				else if (num < this.basicConstraints)
				{
					return false;
				}
			}
			if (this.certificate != null && !this.certificate.Equals(x509Certificate))
			{
				return false;
			}
			if (this.certificateValid != null && !x509Certificate.IsValid(this.certificateValid.Value))
			{
				return false;
			}
			if (this.extendedKeyUsage != null)
			{
				IList list = x509Certificate.GetExtendedKeyUsage();
				if (list != null)
				{
					foreach (DerObjectIdentifier derObjectIdentifier in this.extendedKeyUsage)
					{
						if (!list.Contains(derObjectIdentifier.Id))
						{
							return false;
						}
					}
				}
			}
			if (this.issuer != null && !this.issuer.Equivalent(x509Certificate.IssuerDN, true))
			{
				return false;
			}
			if (this.keyUsage != null)
			{
				bool[] array = x509Certificate.GetKeyUsage();
				if (array != null)
				{
					for (int i = 0; i < 9; i++)
					{
						if (this.keyUsage[i] && !array[i])
						{
							return false;
						}
					}
				}
			}
			if (this.policy != null)
			{
				Asn1OctetString extensionValue = x509Certificate.GetExtensionValue(X509Extensions.CertificatePolicies);
				if (extensionValue == null)
				{
					return false;
				}
				Asn1Sequence instance = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
				if (this.policy.Count < 1 && instance.Count < 1)
				{
					return false;
				}
				bool flag = false;
				foreach (PolicyInformation policyInformation in instance)
				{
					if (this.policy.Contains(policyInformation.PolicyIdentifier))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			if (this.privateKeyValid != null)
			{
				Asn1OctetString extensionValue2 = x509Certificate.GetExtensionValue(X509Extensions.PrivateKeyUsagePeriod);
				if (extensionValue2 == null)
				{
					return false;
				}
				PrivateKeyUsagePeriod instance2 = PrivateKeyUsagePeriod.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
				DateTime value = this.privateKeyValid.Value;
				DateTime value2 = instance2.NotAfter.ToDateTime();
				DateTime value3 = instance2.NotBefore.ToDateTime();
				if (value.CompareTo(value2) > 0 || value.CompareTo(value3) < 0)
				{
					return false;
				}
			}
			return (this.serialNumber == null || this.serialNumber.Equals(x509Certificate.SerialNumber)) && (this.subject == null || this.subject.Equivalent(x509Certificate.SubjectDN, true)) && X509CertStoreSelector.MatchExtension(this.subjectKeyIdentifier, x509Certificate, X509Extensions.SubjectKeyIdentifier) && (this.subjectPublicKey == null || this.subjectPublicKey.Equals(X509CertStoreSelector.GetSubjectPublicKey(x509Certificate))) && (this.subjectPublicKeyAlgID == null || this.subjectPublicKeyAlgID.Equals(X509CertStoreSelector.GetSubjectPublicKey(x509Certificate).AlgorithmID));
		}

		internal static bool IssuersMatch(X509Name a, X509Name b)
		{
			if (a != null)
			{
				return a.Equivalent(b, true);
			}
			return b == null;
		}

		private static bool[] CopyBoolArray(bool[] b)
		{
			if (b != null)
			{
				return (bool[])b.Clone();
			}
			return null;
		}

		private static ISet CopySet(ISet s)
		{
			if (s != null)
			{
				return new HashSet(s);
			}
			return null;
		}

		private static SubjectPublicKeyInfo GetSubjectPublicKey(X509Certificate c)
		{
			return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(c.GetPublicKey());
		}

		private static bool MatchExtension(byte[] b, X509Certificate c, DerObjectIdentifier oid)
		{
			if (b == null)
			{
				return true;
			}
			Asn1OctetString extensionValue = c.GetExtensionValue(oid);
			return extensionValue != null && Arrays.AreEqual(b, extensionValue.GetOctets());
		}
	}
}
