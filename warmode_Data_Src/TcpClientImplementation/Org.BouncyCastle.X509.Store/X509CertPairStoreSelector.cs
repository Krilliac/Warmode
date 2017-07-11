using System;

namespace Org.BouncyCastle.X509.Store
{
	public class X509CertPairStoreSelector : IX509Selector, ICloneable
	{
		private X509CertificatePair certPair;

		private X509CertStoreSelector forwardSelector;

		private X509CertStoreSelector reverseSelector;

		public X509CertificatePair CertPair
		{
			get
			{
				return this.certPair;
			}
			set
			{
				this.certPair = value;
			}
		}

		public X509CertStoreSelector ForwardSelector
		{
			get
			{
				return X509CertPairStoreSelector.CloneSelector(this.forwardSelector);
			}
			set
			{
				this.forwardSelector = X509CertPairStoreSelector.CloneSelector(value);
			}
		}

		public X509CertStoreSelector ReverseSelector
		{
			get
			{
				return X509CertPairStoreSelector.CloneSelector(this.reverseSelector);
			}
			set
			{
				this.reverseSelector = X509CertPairStoreSelector.CloneSelector(value);
			}
		}

		private static X509CertStoreSelector CloneSelector(X509CertStoreSelector s)
		{
			if (s != null)
			{
				return (X509CertStoreSelector)s.Clone();
			}
			return null;
		}

		public X509CertPairStoreSelector()
		{
		}

		private X509CertPairStoreSelector(X509CertPairStoreSelector o)
		{
			this.certPair = o.CertPair;
			this.forwardSelector = o.ForwardSelector;
			this.reverseSelector = o.ReverseSelector;
		}

		public bool Match(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			X509CertificatePair x509CertificatePair = obj as X509CertificatePair;
			return x509CertificatePair != null && (this.certPair == null || this.certPair.Equals(x509CertificatePair)) && (this.forwardSelector == null || this.forwardSelector.Match(x509CertificatePair.Forward)) && (this.reverseSelector == null || this.reverseSelector.Match(x509CertificatePair.Reverse));
		}

		public object Clone()
		{
			return new X509CertPairStoreSelector(this);
		}
	}
}
