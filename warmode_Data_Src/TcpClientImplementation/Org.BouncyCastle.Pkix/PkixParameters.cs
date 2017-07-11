using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixParameters
	{
		public const int PkixValidityModel = 0;

		public const int ChainValidityModel = 1;

		private ISet trustAnchors;

		private DateTimeObject date;

		private IList certPathCheckers;

		private bool revocationEnabled = true;

		private ISet initialPolicies;

		private bool explicitPolicyRequired;

		private bool anyPolicyInhibited;

		private bool policyMappingInhibited;

		private bool policyQualifiersRejected = true;

		private IX509Selector certSelector;

		private IList stores;

		private IX509Selector selector;

		private bool additionalLocationsEnabled;

		private IList additionalStores;

		private ISet trustedACIssuers;

		private ISet necessaryACAttributes;

		private ISet prohibitedACAttributes;

		private ISet attrCertCheckers;

		private int validityModel;

		private bool useDeltas;

		public virtual bool IsRevocationEnabled
		{
			get
			{
				return this.revocationEnabled;
			}
			set
			{
				this.revocationEnabled = value;
			}
		}

		public virtual bool IsExplicitPolicyRequired
		{
			get
			{
				return this.explicitPolicyRequired;
			}
			set
			{
				this.explicitPolicyRequired = value;
			}
		}

		public virtual bool IsAnyPolicyInhibited
		{
			get
			{
				return this.anyPolicyInhibited;
			}
			set
			{
				this.anyPolicyInhibited = value;
			}
		}

		public virtual bool IsPolicyMappingInhibited
		{
			get
			{
				return this.policyMappingInhibited;
			}
			set
			{
				this.policyMappingInhibited = value;
			}
		}

		public virtual bool IsPolicyQualifiersRejected
		{
			get
			{
				return this.policyQualifiersRejected;
			}
			set
			{
				this.policyQualifiersRejected = value;
			}
		}

		public virtual DateTimeObject Date
		{
			get
			{
				return this.date;
			}
			set
			{
				this.date = value;
			}
		}

		public virtual bool IsUseDeltasEnabled
		{
			get
			{
				return this.useDeltas;
			}
			set
			{
				this.useDeltas = value;
			}
		}

		public virtual int ValidityModel
		{
			get
			{
				return this.validityModel;
			}
			set
			{
				this.validityModel = value;
			}
		}

		public virtual bool IsAdditionalLocationsEnabled
		{
			get
			{
				return this.additionalLocationsEnabled;
			}
		}

		public PkixParameters(ISet trustAnchors)
		{
			this.SetTrustAnchors(trustAnchors);
			this.initialPolicies = new HashSet();
			this.certPathCheckers = Platform.CreateArrayList();
			this.stores = Platform.CreateArrayList();
			this.additionalStores = Platform.CreateArrayList();
			this.trustedACIssuers = new HashSet();
			this.necessaryACAttributes = new HashSet();
			this.prohibitedACAttributes = new HashSet();
			this.attrCertCheckers = new HashSet();
		}

		public virtual ISet GetTrustAnchors()
		{
			return new HashSet(this.trustAnchors);
		}

		public virtual void SetTrustAnchors(ISet tas)
		{
			if (tas == null)
			{
				throw new ArgumentNullException("value");
			}
			if (tas.IsEmpty)
			{
				throw new ArgumentException("non-empty set required", "value");
			}
			this.trustAnchors = new HashSet();
			foreach (TrustAnchor trustAnchor in tas)
			{
				if (trustAnchor != null)
				{
					this.trustAnchors.Add(trustAnchor);
				}
			}
		}

		public virtual X509CertStoreSelector GetTargetCertConstraints()
		{
			if (this.certSelector == null)
			{
				return null;
			}
			return (X509CertStoreSelector)this.certSelector.Clone();
		}

		public virtual void SetTargetCertConstraints(IX509Selector selector)
		{
			if (selector == null)
			{
				this.certSelector = null;
				return;
			}
			this.certSelector = (IX509Selector)selector.Clone();
		}

		public virtual ISet GetInitialPolicies()
		{
			ISet s = this.initialPolicies;
			if (this.initialPolicies == null)
			{
				s = new HashSet();
			}
			return new HashSet(s);
		}

		public virtual void SetInitialPolicies(ISet initialPolicies)
		{
			this.initialPolicies = new HashSet();
			if (initialPolicies != null)
			{
				foreach (string text in initialPolicies)
				{
					if (text != null)
					{
						this.initialPolicies.Add(text);
					}
				}
			}
		}

		public virtual void SetCertPathCheckers(IList checkers)
		{
			this.certPathCheckers = Platform.CreateArrayList();
			if (checkers != null)
			{
				foreach (PkixCertPathChecker pkixCertPathChecker in checkers)
				{
					this.certPathCheckers.Add(pkixCertPathChecker.Clone());
				}
			}
		}

		public virtual IList GetCertPathCheckers()
		{
			IList list = Platform.CreateArrayList();
			foreach (PkixCertPathChecker pkixCertPathChecker in this.certPathCheckers)
			{
				list.Add(pkixCertPathChecker.Clone());
			}
			return list;
		}

		public virtual void AddCertPathChecker(PkixCertPathChecker checker)
		{
			if (checker != null)
			{
				this.certPathCheckers.Add(checker.Clone());
			}
		}

		public virtual object Clone()
		{
			PkixParameters pkixParameters = new PkixParameters(this.GetTrustAnchors());
			pkixParameters.SetParams(this);
			return pkixParameters;
		}

		protected virtual void SetParams(PkixParameters parameters)
		{
			this.Date = parameters.Date;
			this.SetCertPathCheckers(parameters.GetCertPathCheckers());
			this.IsAnyPolicyInhibited = parameters.IsAnyPolicyInhibited;
			this.IsExplicitPolicyRequired = parameters.IsExplicitPolicyRequired;
			this.IsPolicyMappingInhibited = parameters.IsPolicyMappingInhibited;
			this.IsRevocationEnabled = parameters.IsRevocationEnabled;
			this.SetInitialPolicies(parameters.GetInitialPolicies());
			this.IsPolicyQualifiersRejected = parameters.IsPolicyQualifiersRejected;
			this.SetTargetCertConstraints(parameters.GetTargetCertConstraints());
			this.SetTrustAnchors(parameters.GetTrustAnchors());
			this.validityModel = parameters.validityModel;
			this.useDeltas = parameters.useDeltas;
			this.additionalLocationsEnabled = parameters.additionalLocationsEnabled;
			this.selector = ((parameters.selector == null) ? null : ((IX509Selector)parameters.selector.Clone()));
			this.stores = Platform.CreateArrayList(parameters.stores);
			this.additionalStores = Platform.CreateArrayList(parameters.additionalStores);
			this.trustedACIssuers = new HashSet(parameters.trustedACIssuers);
			this.prohibitedACAttributes = new HashSet(parameters.prohibitedACAttributes);
			this.necessaryACAttributes = new HashSet(parameters.necessaryACAttributes);
			this.attrCertCheckers = new HashSet(parameters.attrCertCheckers);
		}

		public virtual void SetStores(IList stores)
		{
			if (stores == null)
			{
				this.stores = Platform.CreateArrayList();
				return;
			}
			foreach (object current in stores)
			{
				if (!(current is IX509Store))
				{
					throw new InvalidCastException("All elements of list must be of type " + typeof(IX509Store).FullName);
				}
			}
			this.stores = Platform.CreateArrayList(stores);
		}

		public virtual void AddStore(IX509Store store)
		{
			if (store != null)
			{
				this.stores.Add(store);
			}
		}

		public virtual void AddAdditionalStore(IX509Store store)
		{
			if (store != null)
			{
				this.additionalStores.Add(store);
			}
		}

		public virtual IList GetAdditionalStores()
		{
			return Platform.CreateArrayList(this.additionalStores);
		}

		public virtual IList GetStores()
		{
			return Platform.CreateArrayList(this.stores);
		}

		public virtual void SetAdditionalLocationsEnabled(bool enabled)
		{
			this.additionalLocationsEnabled = enabled;
		}

		public virtual IX509Selector GetTargetConstraints()
		{
			if (this.selector != null)
			{
				return (IX509Selector)this.selector.Clone();
			}
			return null;
		}

		public virtual void SetTargetConstraints(IX509Selector selector)
		{
			if (selector != null)
			{
				this.selector = (IX509Selector)selector.Clone();
				return;
			}
			this.selector = null;
		}

		public virtual ISet GetTrustedACIssuers()
		{
			return new HashSet(this.trustedACIssuers);
		}

		public virtual void SetTrustedACIssuers(ISet trustedACIssuers)
		{
			if (trustedACIssuers == null)
			{
				this.trustedACIssuers = new HashSet();
				return;
			}
			foreach (object current in trustedACIssuers)
			{
				if (!(current is TrustAnchor))
				{
					throw new InvalidCastException("All elements of set must be of type " + typeof(TrustAnchor).Name + ".");
				}
			}
			this.trustedACIssuers = new HashSet(trustedACIssuers);
		}

		public virtual ISet GetNecessaryACAttributes()
		{
			return new HashSet(this.necessaryACAttributes);
		}

		public virtual void SetNecessaryACAttributes(ISet necessaryACAttributes)
		{
			if (necessaryACAttributes == null)
			{
				this.necessaryACAttributes = new HashSet();
				return;
			}
			foreach (object current in necessaryACAttributes)
			{
				if (!(current is string))
				{
					throw new InvalidCastException("All elements of set must be of type string.");
				}
			}
			this.necessaryACAttributes = new HashSet(necessaryACAttributes);
		}

		public virtual ISet GetProhibitedACAttributes()
		{
			return new HashSet(this.prohibitedACAttributes);
		}

		public virtual void SetProhibitedACAttributes(ISet prohibitedACAttributes)
		{
			if (prohibitedACAttributes == null)
			{
				this.prohibitedACAttributes = new HashSet();
				return;
			}
			foreach (object current in prohibitedACAttributes)
			{
				if (!(current is string))
				{
					throw new InvalidCastException("All elements of set must be of type string.");
				}
			}
			this.prohibitedACAttributes = new HashSet(prohibitedACAttributes);
		}

		public virtual ISet GetAttrCertCheckers()
		{
			return new HashSet(this.attrCertCheckers);
		}

		public virtual void SetAttrCertCheckers(ISet attrCertCheckers)
		{
			if (attrCertCheckers == null)
			{
				this.attrCertCheckers = new HashSet();
				return;
			}
			foreach (object current in attrCertCheckers)
			{
				if (!(current is PkixAttrCertChecker))
				{
					throw new InvalidCastException("All elements of set must be of type " + typeof(PkixAttrCertChecker).FullName + ".");
				}
			}
			this.attrCertCheckers = new HashSet(attrCertCheckers);
		}
	}
}
