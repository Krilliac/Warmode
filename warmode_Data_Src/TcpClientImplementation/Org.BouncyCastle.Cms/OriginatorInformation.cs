using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public class OriginatorInformation
	{
		private readonly OriginatorInfo originatorInfo;

		internal OriginatorInformation(OriginatorInfo originatorInfo)
		{
			this.originatorInfo = originatorInfo;
		}

		public virtual IX509Store GetCertificates()
		{
			Asn1Set certificates = this.originatorInfo.Certificates;
			if (certificates != null)
			{
				IList list = Platform.CreateArrayList(certificates.Count);
				foreach (Asn1Encodable asn1Encodable in certificates)
				{
					Asn1Object asn1Object = asn1Encodable.ToAsn1Object();
					if (asn1Object is Asn1Sequence)
					{
						list.Add(new X509Certificate(X509CertificateStructure.GetInstance(asn1Object)));
					}
				}
				return X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(list));
			}
			return X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(Platform.CreateArrayList()));
		}

		public virtual IX509Store GetCrls()
		{
			Asn1Set certificates = this.originatorInfo.Certificates;
			if (certificates != null)
			{
				IList list = Platform.CreateArrayList(certificates.Count);
				foreach (Asn1Encodable asn1Encodable in certificates)
				{
					Asn1Object asn1Object = asn1Encodable.ToAsn1Object();
					if (asn1Object is Asn1Sequence)
					{
						list.Add(new X509Crl(CertificateList.GetInstance(asn1Object)));
					}
				}
				return X509StoreFactory.Create("CRL/Collection", new X509CollectionStoreParameters(list));
			}
			return X509StoreFactory.Create("CRL/Collection", new X509CollectionStoreParameters(Platform.CreateArrayList()));
		}

		public virtual OriginatorInfo ToAsn1Structure()
		{
			return this.originatorInfo;
		}
	}
}
