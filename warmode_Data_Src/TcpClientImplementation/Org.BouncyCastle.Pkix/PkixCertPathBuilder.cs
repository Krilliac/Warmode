using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixCertPathBuilder
	{
		private Exception certPathException;

		public virtual PkixCertPathBuilderResult Build(PkixBuilderParameters pkixParams)
		{
			IX509Selector targetCertConstraints = pkixParams.GetTargetCertConstraints();
			if (!(targetCertConstraints is X509CertStoreSelector))
			{
				throw new PkixCertPathBuilderException(string.Concat(new object[]
				{
					"TargetConstraints must be an instance of ",
					typeof(X509CertStoreSelector).FullName,
					" for ",
					base.GetType(),
					" class."
				}));
			}
			ISet set = new HashSet();
			try
			{
				set.AddAll(PkixCertPathValidatorUtilities.FindCertificates((X509CertStoreSelector)targetCertConstraints, pkixParams.GetStores()));
			}
			catch (Exception exception)
			{
				throw new PkixCertPathBuilderException("Error finding target certificate.", exception);
			}
			if (set.IsEmpty)
			{
				throw new PkixCertPathBuilderException("No certificate found matching targetContraints.");
			}
			PkixCertPathBuilderResult pkixCertPathBuilderResult = null;
			IList tbvPath = Platform.CreateArrayList();
			foreach (X509Certificate tbvCert in set)
			{
				pkixCertPathBuilderResult = this.Build(tbvCert, pkixParams, tbvPath);
				if (pkixCertPathBuilderResult != null)
				{
					break;
				}
			}
			if (pkixCertPathBuilderResult == null && this.certPathException != null)
			{
				throw new PkixCertPathBuilderException(this.certPathException.Message, this.certPathException.InnerException);
			}
			if (pkixCertPathBuilderResult == null && this.certPathException == null)
			{
				throw new PkixCertPathBuilderException("Unable to find certificate chain.");
			}
			return pkixCertPathBuilderResult;
		}

		protected virtual PkixCertPathBuilderResult Build(X509Certificate tbvCert, PkixBuilderParameters pkixParams, IList tbvPath)
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
			PkixCertPathValidator pkixCertPathValidator = new PkixCertPathValidator();
			try
			{
				if (PkixCertPathValidatorUtilities.FindTrustAnchor(tbvCert, pkixParams.GetTrustAnchors()) != null)
				{
					PkixCertPath certPath = null;
					try
					{
						certPath = new PkixCertPath(tbvPath);
					}
					catch (Exception innerException)
					{
						throw new Exception("Certification path could not be constructed from certificate list.", innerException);
					}
					PkixCertPathValidatorResult pkixCertPathValidatorResult = null;
					try
					{
						pkixCertPathValidatorResult = pkixCertPathValidator.Validate(certPath, pkixParams);
					}
					catch (Exception innerException2)
					{
						throw new Exception("Certification path could not be validated.", innerException2);
					}
					return new PkixCertPathBuilderResult(certPath, pkixCertPathValidatorResult.TrustAnchor, pkixCertPathValidatorResult.PolicyTree, pkixCertPathValidatorResult.SubjectPublicKey);
				}
				try
				{
					PkixCertPathValidatorUtilities.AddAdditionalStoresFromAltNames(tbvCert, pkixParams);
				}
				catch (CertificateParsingException innerException3)
				{
					throw new Exception("No additiontal X.509 stores can be added from certificate locations.", innerException3);
				}
				HashSet hashSet = new HashSet();
				try
				{
					hashSet.AddAll(PkixCertPathValidatorUtilities.FindIssuerCerts(tbvCert, pkixParams));
				}
				catch (Exception innerException4)
				{
					throw new Exception("Cannot find issuer certificate for certificate in certification path.", innerException4);
				}
				if (hashSet.IsEmpty)
				{
					throw new Exception("No issuer certificate for certificate in certification path found.");
				}
				foreach (X509Certificate tbvCert2 in hashSet)
				{
					pkixCertPathBuilderResult = this.Build(tbvCert2, pkixParams, tbvPath);
					if (pkixCertPathBuilderResult != null)
					{
						break;
					}
				}
			}
			catch (Exception ex)
			{
				this.certPathException = ex;
			}
			if (pkixCertPathBuilderResult == null)
			{
				tbvPath.Remove(tbvCert);
			}
			return pkixCertPathBuilderResult;
		}
	}
}
