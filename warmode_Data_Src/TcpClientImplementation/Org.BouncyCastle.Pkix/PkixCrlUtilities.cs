using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;

namespace Org.BouncyCastle.Pkix
{
	public class PkixCrlUtilities
	{
		public virtual ISet FindCrls(X509CrlStoreSelector crlselect, PkixParameters paramsPkix, DateTime currentDate)
		{
			ISet set = new HashSet();
			try
			{
				set.AddAll(this.FindCrls(crlselect, paramsPkix.GetAdditionalStores()));
				set.AddAll(this.FindCrls(crlselect, paramsPkix.GetStores()));
			}
			catch (Exception innerException)
			{
				throw new Exception("Exception obtaining complete CRLs.", innerException);
			}
			ISet set2 = new HashSet();
			DateTime value = currentDate;
			if (paramsPkix.Date != null)
			{
				value = paramsPkix.Date.Value;
			}
			foreach (X509Crl x509Crl in set)
			{
				if (x509Crl.NextUpdate.Value.CompareTo(value) > 0)
				{
					X509Certificate certificateChecking = crlselect.CertificateChecking;
					if (certificateChecking != null)
					{
						if (x509Crl.ThisUpdate.CompareTo(certificateChecking.NotAfter) < 0)
						{
							set2.Add(x509Crl);
						}
					}
					else
					{
						set2.Add(x509Crl);
					}
				}
			}
			return set2;
		}

		public virtual ISet FindCrls(X509CrlStoreSelector crlselect, PkixParameters paramsPkix)
		{
			ISet set = new HashSet();
			try
			{
				set.AddAll(this.FindCrls(crlselect, paramsPkix.GetStores()));
			}
			catch (Exception innerException)
			{
				throw new Exception("Exception obtaining complete CRLs.", innerException);
			}
			return set;
		}

		private ICollection FindCrls(X509CrlStoreSelector crlSelect, IList crlStores)
		{
			ISet set = new HashSet();
			Exception ex = null;
			bool flag = false;
			foreach (IX509Store iX509Store in crlStores)
			{
				try
				{
					set.AddAll(iX509Store.GetMatches(crlSelect));
					flag = true;
				}
				catch (X509StoreException innerException)
				{
					ex = new Exception("Exception searching in X.509 CRL store.", innerException);
				}
			}
			if (!flag && ex != null)
			{
				throw ex;
			}
			return set;
		}
	}
}
