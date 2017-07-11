using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	public sealed class X509StoreFactory
	{
		private X509StoreFactory()
		{
		}

		public static IX509Store Create(string type, IX509StoreParameters parameters)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			string[] array = Platform.ToUpperInvariant(type).Split(new char[]
			{
				'/'
			});
			if (array.Length < 2)
			{
				throw new ArgumentException("type");
			}
			if (array[1] != "COLLECTION")
			{
				throw new NoSuchStoreException("X.509 store type '" + type + "' not available.");
			}
			X509CollectionStoreParameters x509CollectionStoreParameters = (X509CollectionStoreParameters)parameters;
			ICollection collection = x509CollectionStoreParameters.GetCollection();
			string a;
			if ((a = array[0]) != null)
			{
				if (!(a == "ATTRIBUTECERTIFICATE"))
				{
					if (!(a == "CERTIFICATE"))
					{
						if (!(a == "CERTIFICATEPAIR"))
						{
							if (!(a == "CRL"))
							{
								goto IL_F8;
							}
							X509StoreFactory.checkCorrectType(collection, typeof(X509Crl));
						}
						else
						{
							X509StoreFactory.checkCorrectType(collection, typeof(X509CertificatePair));
						}
					}
					else
					{
						X509StoreFactory.checkCorrectType(collection, typeof(X509Certificate));
					}
				}
				else
				{
					X509StoreFactory.checkCorrectType(collection, typeof(IX509AttributeCertificate));
				}
				return new X509CollectionStore(collection);
			}
			IL_F8:
			throw new NoSuchStoreException("X.509 store type '" + type + "' not available.");
		}

		private static void checkCorrectType(ICollection coll, Type t)
		{
			foreach (object current in coll)
			{
				if (!t.IsInstanceOfType(current))
				{
					throw new InvalidCastException("Can't cast object to type: " + t.FullName);
				}
			}
		}
	}
}
