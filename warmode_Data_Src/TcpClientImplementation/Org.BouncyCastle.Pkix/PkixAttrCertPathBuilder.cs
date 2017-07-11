using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixAttrCertPathBuilder
	{
		private Exception certPathException;

		public virtual PkixCertPathBuilderResult Build(PkixBuilderParameters pkixParams)
		{
			IX509Selector targetConstraints = pkixParams.GetTargetConstraints();
			if (!(targetConstraints is X509AttrCertStoreSelector))
			{
				throw new PkixCertPathBuilderException(string.Concat(new string[]
				{
					"TargetConstraints must be an instance of ",
					typeof(X509AttrCertStoreSelector).FullName,
					" for ",
					typeof(PkixAttrCertPathBuilder).FullName,
					" class."
				}));
			}
			ICollection collection;
			try
			{
				collection = PkixCertPathValidatorUtilities.FindCertificates((X509AttrCertStoreSelector)targetConstraints, pkixParams.GetStores());
			}
			catch (Exception exception)
			{
				throw new PkixCertPathBuilderException("Error finding target attribute certificate.", exception);
			}
			if (collection.Count == 0)
			{
				throw new PkixCertPathBuilderException("No attribute certificate found matching targetContraints.");
			}
			PkixCertPathBuilderResult pkixCertPathBuilderResult = null;
			foreach (IX509AttributeCertificate iX509AttributeCertificate in collection)
			{
				X509CertStoreSelector x509CertStoreSelector = new X509CertStoreSelector();
				X509Name[] principals = iX509AttributeCertificate.Issuer.GetPrincipals();
				ISet set = new HashSet();
				for (int i = 0; i < principals.Length; i++)
				{
					try
					{
						x509CertStoreSelector.Subject = principals[i];
						set.AddAll(PkixCertPathValidatorUtilities.FindCertificates(x509CertStoreSelector, pkixParams.GetStores()));
					}
					catch (Exception exception2)
					{
						throw new PkixCertPathBuilderException("Public key certificate for attribute certificate cannot be searched.", exception2);
					}
				}
				if (set.IsEmpty)
				{
					throw new PkixCertPathBuilderException("Public key certificate for attribute certificate cannot be found.");
				}
				IList tbvPath = Platform.CreateArrayList();
				foreach (X509Certificate tbvCert in set)
				{
					pkixCertPathBuilderResult = this.Build(iX509AttributeCertificate, tbvCert, pkixParams, tbvPath);
					if (pkixCertPathBuilderResult != null)
					{
						break;
					}
				}
				if (pkixCertPathBuilderResult != null)
				{
					break;
				}
			}
			if (pkixCertPathBuilderResult == null && this.certPathException != null)
			{
				throw new PkixCertPathBuilderException("Possible certificate chain could not be validated.", this.certPathException);
			}
			if (pkixCertPathBuilderResult == null && this.certPathException == null)
			{
				throw new PkixCertPathBuilderException("Unable to find certificate chain.");
			}
			return pkixCertPathBuilderResult;
		}

		private PkixCertPathBuilderResult Build(IX509AttributeCertificate attrCert, X509Certificate tbvCert, PkixBuilderParameters pkixParams, IList tbvPath)
		{
			if (tbvPath.Contains(tbvCert))
			{
				return null;
			}
			if (pkixParams.GetExcludedCerts().Contains(tbvCert))
			{
				return null;
			}
			if (pkixParams.MaxPathLength != -1 && tbvPath.Count - 1 > pkixParams.MaxPathLength)
			{
				return null;
			}
			tbvPath.Add(tbvCert);
			PkixCertPathBuilderResult pkixCertPathBuilderResult = null;
			PkixAttrCertPathValidator pkixAttrCertPathValidator = new PkixAttrCertPathValidator();
			try
			{
				if (PkixCertPathValidatorUtilities.FindTrustAnchor(tbvCert, pkixParams.GetTrustAnchors()) != null)
				{
					PkixCertPath certPath = new PkixCertPath(tbvPath);
					PkixCertPathValidatorResult pkixCertPathValidatorResult;
					try
					{
						pkixCertPathValidatorResult = pkixAttrCertPathValidator.Validate(certPath, pkixParams);
					}
					catch (Exception innerException)
					{
						throw new Exception("Certification path could not be validated.", innerException);
					}
					return new PkixCertPathBuilderResult(certPath, pkixCertPathValidatorResult.TrustAnchor, pkixCertPathValidatorResult.PolicyTree, pkixCertPathValidatorResult.SubjectPublicKey);
				}
				try
				{
					PkixCertPathValidatorUtilities.AddAdditionalStoresFromAltNames(tbvCert, pkixParams);
				}
				catch (CertificateParsingException innerException2)
				{
					throw new Exception("No additional X.509 stores can be added from certificate locations.", innerException2);
				}
				ISet set = new HashSet();
				try
				{
					set.AddAll(PkixCertPathValidatorUtilities.FindIssuerCerts(tbvCert, pkixParams));
				}
				catch (Exception innerException3)
				{
					throw new Exception("Cannot find issuer certificate for certificate in certification path.", innerException3);
				}
				if (set.IsEmpty)
				{
					throw new Exception("No issuer certificate for certificate in certification path found.");
				}
				foreach (X509Certificate x509Certificate in set)
				{
					if (!PkixCertPathValidatorUtilities.IsSelfIssued(x509Certificate))
					{
						pkixCertPathBuilderResult = this.Build(attrCert, x509Certificate, pkixParams, tbvPath);
						if (pkixCertPathBuilderResult != null)
						{
							break;
						}
					}
				}
			}
			catch (Exception innerException4)
			{
				this.certPathException = new Exception("No valid certification path could be build.", innerException4);
			}
			if (pkixCertPathBuilderResult == null)
			{
				tbvPath.Remove(tbvCert);
			}
			return pkixCertPathBuilderResult;
		}
	}
}
