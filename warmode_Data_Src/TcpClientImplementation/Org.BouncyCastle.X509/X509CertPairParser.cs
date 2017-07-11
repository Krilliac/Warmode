using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.X509
{
	public class X509CertPairParser
	{
		private Stream currentStream;

		private X509CertificatePair ReadDerCrossCertificatePair(Stream inStream)
		{
			Asn1InputStream asn1InputStream = new Asn1InputStream(inStream);
			Asn1Sequence obj = (Asn1Sequence)asn1InputStream.ReadObject();
			CertificatePair instance = CertificatePair.GetInstance(obj);
			return new X509CertificatePair(instance);
		}

		public X509CertificatePair ReadCertPair(byte[] input)
		{
			return this.ReadCertPair(new MemoryStream(input, false));
		}

		public ICollection ReadCertPairs(byte[] input)
		{
			return this.ReadCertPairs(new MemoryStream(input, false));
		}

		public X509CertificatePair ReadCertPair(Stream inStream)
		{
			if (inStream == null)
			{
				throw new ArgumentNullException("inStream");
			}
			if (!inStream.CanRead)
			{
				throw new ArgumentException("inStream must be read-able", "inStream");
			}
			if (this.currentStream == null)
			{
				this.currentStream = inStream;
			}
			else if (this.currentStream != inStream)
			{
				this.currentStream = inStream;
			}
			X509CertificatePair result;
			try
			{
				PushbackStream pushbackStream = new PushbackStream(inStream);
				int num = pushbackStream.ReadByte();
				if (num < 0)
				{
					result = null;
				}
				else
				{
					pushbackStream.Unread(num);
					result = this.ReadDerCrossCertificatePair(pushbackStream);
				}
			}
			catch (Exception ex)
			{
				throw new CertificateException(ex.ToString());
			}
			return result;
		}

		public ICollection ReadCertPairs(Stream inStream)
		{
			IList list = Platform.CreateArrayList();
			X509CertificatePair value;
			while ((value = this.ReadCertPair(inStream)) != null)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
