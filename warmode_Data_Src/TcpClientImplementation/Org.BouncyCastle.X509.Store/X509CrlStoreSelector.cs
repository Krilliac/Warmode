using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	public class X509CrlStoreSelector : IX509Selector, ICloneable
	{
		private X509Certificate certificateChecking;

		private DateTimeObject dateAndTime;

		private ICollection issuers;

		private BigInteger maxCrlNumber;

		private BigInteger minCrlNumber;

		private IX509AttributeCertificate attrCertChecking;

		private bool completeCrlEnabled;

		private bool deltaCrlIndicatorEnabled;

		private byte[] issuingDistributionPoint;

		private bool issuingDistributionPointEnabled;

		private BigInteger maxBaseCrlNumber;

		public X509Certificate CertificateChecking
		{
			get
			{
				return this.certificateChecking;
			}
			set
			{
				this.certificateChecking = value;
			}
		}

		public DateTimeObject DateAndTime
		{
			get
			{
				return this.dateAndTime;
			}
			set
			{
				this.dateAndTime = value;
			}
		}

		public ICollection Issuers
		{
			get
			{
				return Platform.CreateArrayList(this.issuers);
			}
			set
			{
				this.issuers = Platform.CreateArrayList(value);
			}
		}

		public BigInteger MaxCrlNumber
		{
			get
			{
				return this.maxCrlNumber;
			}
			set
			{
				this.maxCrlNumber = value;
			}
		}

		public BigInteger MinCrlNumber
		{
			get
			{
				return this.minCrlNumber;
			}
			set
			{
				this.minCrlNumber = value;
			}
		}

		public IX509AttributeCertificate AttrCertChecking
		{
			get
			{
				return this.attrCertChecking;
			}
			set
			{
				this.attrCertChecking = value;
			}
		}

		public bool CompleteCrlEnabled
		{
			get
			{
				return this.completeCrlEnabled;
			}
			set
			{
				this.completeCrlEnabled = value;
			}
		}

		public bool DeltaCrlIndicatorEnabled
		{
			get
			{
				return this.deltaCrlIndicatorEnabled;
			}
			set
			{
				this.deltaCrlIndicatorEnabled = value;
			}
		}

		public byte[] IssuingDistributionPoint
		{
			get
			{
				return Arrays.Clone(this.issuingDistributionPoint);
			}
			set
			{
				this.issuingDistributionPoint = Arrays.Clone(value);
			}
		}

		public bool IssuingDistributionPointEnabled
		{
			get
			{
				return this.issuingDistributionPointEnabled;
			}
			set
			{
				this.issuingDistributionPointEnabled = value;
			}
		}

		public BigInteger MaxBaseCrlNumber
		{
			get
			{
				return this.maxBaseCrlNumber;
			}
			set
			{
				this.maxBaseCrlNumber = value;
			}
		}

		public X509CrlStoreSelector()
		{
		}

		public X509CrlStoreSelector(X509CrlStoreSelector o)
		{
			this.certificateChecking = o.CertificateChecking;
			this.dateAndTime = o.DateAndTime;
			this.issuers = o.Issuers;
			this.maxCrlNumber = o.MaxCrlNumber;
			this.minCrlNumber = o.MinCrlNumber;
			this.deltaCrlIndicatorEnabled = o.DeltaCrlIndicatorEnabled;
			this.completeCrlEnabled = o.CompleteCrlEnabled;
			this.maxBaseCrlNumber = o.MaxBaseCrlNumber;
			this.attrCertChecking = o.AttrCertChecking;
			this.issuingDistributionPointEnabled = o.IssuingDistributionPointEnabled;
			this.issuingDistributionPoint = o.IssuingDistributionPoint;
		}

		public virtual object Clone()
		{
			return new X509CrlStoreSelector(this);
		}

		public virtual bool Match(object obj)
		{
			X509Crl x509Crl = obj as X509Crl;
			if (x509Crl == null)
			{
				return false;
			}
			if (this.dateAndTime != null)
			{
				DateTime value = this.dateAndTime.Value;
				DateTime thisUpdate = x509Crl.ThisUpdate;
				DateTimeObject nextUpdate = x509Crl.NextUpdate;
				if (value.CompareTo(thisUpdate) < 0 || nextUpdate == null || value.CompareTo(nextUpdate.Value) >= 0)
				{
					return false;
				}
			}
			if (this.issuers != null)
			{
				X509Name issuerDN = x509Crl.IssuerDN;
				bool flag = false;
				foreach (X509Name x509Name in this.issuers)
				{
					if (x509Name.Equivalent(issuerDN, true))
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
			if (this.maxCrlNumber != null || this.minCrlNumber != null)
			{
				Asn1OctetString extensionValue = x509Crl.GetExtensionValue(X509Extensions.CrlNumber);
				if (extensionValue == null)
				{
					return false;
				}
				BigInteger positiveValue = DerInteger.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).PositiveValue;
				if (this.maxCrlNumber != null && positiveValue.CompareTo(this.maxCrlNumber) > 0)
				{
					return false;
				}
				if (this.minCrlNumber != null && positiveValue.CompareTo(this.minCrlNumber) < 0)
				{
					return false;
				}
			}
			DerInteger derInteger = null;
			try
			{
				Asn1OctetString extensionValue2 = x509Crl.GetExtensionValue(X509Extensions.DeltaCrlIndicator);
				if (extensionValue2 != null)
				{
					derInteger = DerInteger.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
				}
			}
			catch (Exception)
			{
				return false;
			}
			if (derInteger == null)
			{
				if (this.DeltaCrlIndicatorEnabled)
				{
					return false;
				}
			}
			else
			{
				if (this.CompleteCrlEnabled)
				{
					return false;
				}
				if (this.maxBaseCrlNumber != null && derInteger.PositiveValue.CompareTo(this.maxBaseCrlNumber) > 0)
				{
					return false;
				}
			}
			if (this.issuingDistributionPointEnabled)
			{
				Asn1OctetString extensionValue3 = x509Crl.GetExtensionValue(X509Extensions.IssuingDistributionPoint);
				if (this.issuingDistributionPoint == null)
				{
					if (extensionValue3 != null)
					{
						return false;
					}
				}
				else if (!Arrays.AreEqual(extensionValue3.GetOctets(), this.issuingDistributionPoint))
				{
					return false;
				}
			}
			return true;
		}
	}
}
