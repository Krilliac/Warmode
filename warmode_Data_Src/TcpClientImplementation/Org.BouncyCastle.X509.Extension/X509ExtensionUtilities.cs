using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.X509.Extension
{
	public class X509ExtensionUtilities
	{
		public static Asn1Object FromExtensionValue(Asn1OctetString extensionValue)
		{
			return Asn1Object.FromByteArray(extensionValue.GetOctets());
		}

		public static ICollection GetIssuerAlternativeNames(X509Certificate cert)
		{
			Asn1OctetString extensionValue = cert.GetExtensionValue(X509Extensions.IssuerAlternativeName);
			return X509ExtensionUtilities.GetAlternativeName(extensionValue);
		}

		public static ICollection GetSubjectAlternativeNames(X509Certificate cert)
		{
			Asn1OctetString extensionValue = cert.GetExtensionValue(X509Extensions.SubjectAlternativeName);
			return X509ExtensionUtilities.GetAlternativeName(extensionValue);
		}

		private static ICollection GetAlternativeName(Asn1OctetString extVal)
		{
			IList list = Platform.CreateArrayList();
			if (extVal != null)
			{
				try
				{
					Asn1Sequence instance = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(extVal));
					foreach (GeneralName generalName in instance)
					{
						IList list2 = Platform.CreateArrayList();
						list2.Add(generalName.TagNo);
						switch (generalName.TagNo)
						{
						case 0:
						case 3:
						case 5:
							list2.Add(generalName.Name.ToAsn1Object());
							break;
						case 1:
						case 2:
						case 6:
							list2.Add(((IAsn1String)generalName.Name).GetString());
							break;
						case 4:
							list2.Add(X509Name.GetInstance(generalName.Name).ToString());
							break;
						case 7:
							list2.Add(Asn1OctetString.GetInstance(generalName.Name).GetOctets());
							break;
						case 8:
							list2.Add(DerObjectIdentifier.GetInstance(generalName.Name).Id);
							break;
						default:
							throw new IOException("Bad tag number: " + generalName.TagNo);
						}
						list.Add(list2);
					}
				}
				catch (Exception ex)
				{
					throw new CertificateParsingException(ex.Message);
				}
			}
			return list;
		}
	}
}
