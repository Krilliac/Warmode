using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	internal class CmsUtilities
	{
		internal static int MaximumMemory
		{
			get
			{
				long num = 2147483647L;
				if (num > 2147483647L)
				{
					return 2147483647;
				}
				return (int)num;
			}
		}

		internal static ContentInfo ReadContentInfo(byte[] input)
		{
			return CmsUtilities.ReadContentInfo(new Asn1InputStream(input));
		}

		internal static ContentInfo ReadContentInfo(Stream input)
		{
			return CmsUtilities.ReadContentInfo(new Asn1InputStream(input, CmsUtilities.MaximumMemory));
		}

		private static ContentInfo ReadContentInfo(Asn1InputStream aIn)
		{
			ContentInfo instance;
			try
			{
				instance = ContentInfo.GetInstance(aIn.ReadObject());
			}
			catch (IOException e)
			{
				throw new CmsException("IOException reading content.", e);
			}
			catch (InvalidCastException e2)
			{
				throw new CmsException("Malformed content.", e2);
			}
			catch (ArgumentException e3)
			{
				throw new CmsException("Malformed content.", e3);
			}
			return instance;
		}

		public static byte[] StreamToByteArray(Stream inStream)
		{
			return Streams.ReadAll(inStream);
		}

		public static byte[] StreamToByteArray(Stream inStream, int limit)
		{
			return Streams.ReadAllLimited(inStream, limit);
		}

		public static IList GetCertificatesFromStore(IX509Store certStore)
		{
			IList result;
			try
			{
				IList list = Platform.CreateArrayList();
				if (certStore != null)
				{
					foreach (X509Certificate x509Certificate in certStore.GetMatches(null))
					{
						list.Add(X509CertificateStructure.GetInstance(Asn1Object.FromByteArray(x509Certificate.GetEncoded())));
					}
				}
				result = list;
			}
			catch (CertificateEncodingException e)
			{
				throw new CmsException("error encoding certs", e);
			}
			catch (Exception e2)
			{
				throw new CmsException("error processing certs", e2);
			}
			return result;
		}

		public static IList GetCrlsFromStore(IX509Store crlStore)
		{
			IList result;
			try
			{
				IList list = Platform.CreateArrayList();
				if (crlStore != null)
				{
					foreach (X509Crl x509Crl in crlStore.GetMatches(null))
					{
						list.Add(CertificateList.GetInstance(Asn1Object.FromByteArray(x509Crl.GetEncoded())));
					}
				}
				result = list;
			}
			catch (CrlException e)
			{
				throw new CmsException("error encoding crls", e);
			}
			catch (Exception e2)
			{
				throw new CmsException("error processing crls", e2);
			}
			return result;
		}

		public static Asn1Set CreateBerSetFromList(IList berObjects)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (Asn1Encodable asn1Encodable in berObjects)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					asn1Encodable
				});
			}
			return new BerSet(asn1EncodableVector);
		}

		public static Asn1Set CreateDerSetFromList(IList derObjects)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (Asn1Encodable asn1Encodable in derObjects)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					asn1Encodable
				});
			}
			return new DerSet(asn1EncodableVector);
		}

		internal static Stream CreateBerOctetOutputStream(Stream s, int tagNo, bool isExplicit, int bufferSize)
		{
			BerOctetStringGenerator berOctetStringGenerator = new BerOctetStringGenerator(s, tagNo, isExplicit);
			return berOctetStringGenerator.GetOctetOutputStream(bufferSize);
		}

		internal static TbsCertificateStructure GetTbsCertificateStructure(X509Certificate cert)
		{
			return TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(cert.GetTbsCertificate()));
		}

		internal static IssuerAndSerialNumber GetIssuerAndSerialNumber(X509Certificate cert)
		{
			TbsCertificateStructure tbsCertificateStructure = CmsUtilities.GetTbsCertificateStructure(cert);
			return new IssuerAndSerialNumber(tbsCertificateStructure.Issuer, tbsCertificateStructure.SerialNumber.Value);
		}
	}
}
