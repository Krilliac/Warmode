using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixCertPathValidator
	{
		public virtual PkixCertPathValidatorResult Validate(PkixCertPath certPath, PkixParameters paramsPkix)
		{
			if (paramsPkix.GetTrustAnchors() == null)
			{
				throw new ArgumentException("trustAnchors is null, this is not allowed for certification path validation.", "parameters");
			}
			IList certificates = certPath.Certificates;
			int count = certificates.Count;
			if (certificates.Count == 0)
			{
				throw new PkixCertPathValidatorException("Certification path is empty.", null, certPath, 0);
			}
			ISet initialPolicies = paramsPkix.GetInitialPolicies();
			TrustAnchor trustAnchor;
			try
			{
				trustAnchor = PkixCertPathValidatorUtilities.FindTrustAnchor((X509Certificate)certificates[certificates.Count - 1], paramsPkix.GetTrustAnchors());
			}
			catch (Exception ex)
			{
				throw new PkixCertPathValidatorException(ex.Message, ex, certPath, certificates.Count - 1);
			}
			if (trustAnchor == null)
			{
				throw new PkixCertPathValidatorException("Trust anchor for certification path not found.", null, certPath, -1);
			}
			int i = 0;
			IList[] array = new IList[count + 1];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = Platform.CreateArrayList();
			}
			ISet set = new HashSet();
			set.Add(Rfc3280CertPathUtilities.ANY_POLICY);
			PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(Platform.CreateArrayList(), 0, set, null, new HashSet(), Rfc3280CertPathUtilities.ANY_POLICY, false);
			array[0].Add(pkixPolicyNode);
			PkixNameConstraintValidator nameConstraintValidator = new PkixNameConstraintValidator();
			ISet acceptablePolicies = new HashSet();
			int num;
			if (paramsPkix.IsExplicitPolicyRequired)
			{
				num = 0;
			}
			else
			{
				num = count + 1;
			}
			int inhibitAnyPolicy;
			if (paramsPkix.IsAnyPolicyInhibited)
			{
				inhibitAnyPolicy = 0;
			}
			else
			{
				inhibitAnyPolicy = count + 1;
			}
			int policyMapping;
			if (paramsPkix.IsPolicyMappingInhibited)
			{
				policyMapping = 0;
			}
			else
			{
				policyMapping = count + 1;
			}
			X509Certificate x509Certificate = trustAnchor.TrustedCert;
			X509Name workingIssuerName;
			AsymmetricKeyParameter asymmetricKeyParameter;
			try
			{
				if (x509Certificate != null)
				{
					workingIssuerName = x509Certificate.SubjectDN;
					asymmetricKeyParameter = x509Certificate.GetPublicKey();
				}
				else
				{
					workingIssuerName = new X509Name(trustAnchor.CAName);
					asymmetricKeyParameter = trustAnchor.CAPublicKey;
				}
			}
			catch (ArgumentException cause)
			{
				throw new PkixCertPathValidatorException("Subject of trust anchor could not be (re)encoded.", cause, certPath, -1);
			}
			try
			{
				PkixCertPathValidatorUtilities.GetAlgorithmIdentifier(asymmetricKeyParameter);
			}
			catch (PkixCertPathValidatorException cause2)
			{
				throw new PkixCertPathValidatorException("Algorithm identifier of public key of trust anchor could not be read.", cause2, certPath, -1);
			}
			int maxPathLength = count;
			X509CertStoreSelector targetCertConstraints = paramsPkix.GetTargetCertConstraints();
			if (targetCertConstraints != null && !targetCertConstraints.Match((X509Certificate)certificates[0]))
			{
				throw new PkixCertPathValidatorException("Target certificate in certification path does not match targetConstraints.", null, certPath, 0);
			}
			IList certPathCheckers = paramsPkix.GetCertPathCheckers();
			IEnumerator enumerator = certPathCheckers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				((PkixCertPathChecker)enumerator.Current).Init(false);
			}
			X509Certificate x509Certificate2 = null;
			for (i = certificates.Count - 1; i >= 0; i--)
			{
				int num2 = count - i;
				x509Certificate2 = (X509Certificate)certificates[i];
				Rfc3280CertPathUtilities.ProcessCertA(certPath, paramsPkix, i, asymmetricKeyParameter, workingIssuerName, x509Certificate);
				Rfc3280CertPathUtilities.ProcessCertBC(certPath, i, nameConstraintValidator);
				pkixPolicyNode = Rfc3280CertPathUtilities.ProcessCertD(certPath, i, acceptablePolicies, pkixPolicyNode, array, inhibitAnyPolicy);
				pkixPolicyNode = Rfc3280CertPathUtilities.ProcessCertE(certPath, i, pkixPolicyNode);
				Rfc3280CertPathUtilities.ProcessCertF(certPath, i, pkixPolicyNode, num);
				if (num2 != count)
				{
					if (x509Certificate2 != null && x509Certificate2.Version == 1)
					{
						throw new PkixCertPathValidatorException("Version 1 certificates can't be used as CA ones.", null, certPath, i);
					}
					Rfc3280CertPathUtilities.PrepareNextCertA(certPath, i);
					pkixPolicyNode = Rfc3280CertPathUtilities.PrepareCertB(certPath, i, array, pkixPolicyNode, policyMapping);
					Rfc3280CertPathUtilities.PrepareNextCertG(certPath, i, nameConstraintValidator);
					num = Rfc3280CertPathUtilities.PrepareNextCertH1(certPath, i, num);
					policyMapping = Rfc3280CertPathUtilities.PrepareNextCertH2(certPath, i, policyMapping);
					inhibitAnyPolicy = Rfc3280CertPathUtilities.PrepareNextCertH3(certPath, i, inhibitAnyPolicy);
					num = Rfc3280CertPathUtilities.PrepareNextCertI1(certPath, i, num);
					policyMapping = Rfc3280CertPathUtilities.PrepareNextCertI2(certPath, i, policyMapping);
					inhibitAnyPolicy = Rfc3280CertPathUtilities.PrepareNextCertJ(certPath, i, inhibitAnyPolicy);
					Rfc3280CertPathUtilities.PrepareNextCertK(certPath, i);
					maxPathLength = Rfc3280CertPathUtilities.PrepareNextCertL(certPath, i, maxPathLength);
					maxPathLength = Rfc3280CertPathUtilities.PrepareNextCertM(certPath, i, maxPathLength);
					Rfc3280CertPathUtilities.PrepareNextCertN(certPath, i);
					ISet set2 = x509Certificate2.GetCriticalExtensionOids();
					if (set2 != null)
					{
						set2 = new HashSet(set2);
						set2.Remove(X509Extensions.KeyUsage.Id);
						set2.Remove(X509Extensions.CertificatePolicies.Id);
						set2.Remove(X509Extensions.PolicyMappings.Id);
						set2.Remove(X509Extensions.InhibitAnyPolicy.Id);
						set2.Remove(X509Extensions.IssuingDistributionPoint.Id);
						set2.Remove(X509Extensions.DeltaCrlIndicator.Id);
						set2.Remove(X509Extensions.PolicyConstraints.Id);
						set2.Remove(X509Extensions.BasicConstraints.Id);
						set2.Remove(X509Extensions.SubjectAlternativeName.Id);
						set2.Remove(X509Extensions.NameConstraints.Id);
					}
					else
					{
						set2 = new HashSet();
					}
					Rfc3280CertPathUtilities.PrepareNextCertO(certPath, i, set2, certPathCheckers);
					x509Certificate = x509Certificate2;
					workingIssuerName = x509Certificate.SubjectDN;
					try
					{
						asymmetricKeyParameter = PkixCertPathValidatorUtilities.GetNextWorkingKey(certPath.Certificates, i);
					}
					catch (PkixCertPathValidatorException cause3)
					{
						throw new PkixCertPathValidatorException("Next working key could not be retrieved.", cause3, certPath, i);
					}
					PkixCertPathValidatorUtilities.GetAlgorithmIdentifier(asymmetricKeyParameter);
				}
			}
			num = Rfc3280CertPathUtilities.WrapupCertA(num, x509Certificate2);
			num = Rfc3280CertPathUtilities.WrapupCertB(certPath, i + 1, num);
			ISet set3 = x509Certificate2.GetCriticalExtensionOids();
			if (set3 != null)
			{
				set3 = new HashSet(set3);
				set3.Remove(X509Extensions.KeyUsage.Id);
				set3.Remove(X509Extensions.CertificatePolicies.Id);
				set3.Remove(X509Extensions.PolicyMappings.Id);
				set3.Remove(X509Extensions.InhibitAnyPolicy.Id);
				set3.Remove(X509Extensions.IssuingDistributionPoint.Id);
				set3.Remove(X509Extensions.DeltaCrlIndicator.Id);
				set3.Remove(X509Extensions.PolicyConstraints.Id);
				set3.Remove(X509Extensions.BasicConstraints.Id);
				set3.Remove(X509Extensions.SubjectAlternativeName.Id);
				set3.Remove(X509Extensions.NameConstraints.Id);
				set3.Remove(X509Extensions.CrlDistributionPoints.Id);
			}
			else
			{
				set3 = new HashSet();
			}
			Rfc3280CertPathUtilities.WrapupCertF(certPath, i + 1, certPathCheckers, set3);
			PkixPolicyNode pkixPolicyNode2 = Rfc3280CertPathUtilities.WrapupCertG(certPath, paramsPkix, initialPolicies, i + 1, array, pkixPolicyNode, acceptablePolicies);
			if (num > 0 || pkixPolicyNode2 != null)
			{
				return new PkixCertPathValidatorResult(trustAnchor, pkixPolicyNode2, x509Certificate2.GetPublicKey());
			}
			throw new PkixCertPathValidatorException("Path processing failed on policy.", null, certPath, i);
		}
	}
}
