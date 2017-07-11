using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.X509
{
	public class X509AttrCertParser
	{
		private static readonly PemParser PemAttrCertParser = new PemParser("ATTRIBUTE CERTIFICATE");

		private Asn1Set sData;

		private int sDataObjectCount;

		private Stream currentStream;

		private IX509AttributeCertificate ReadDerCertificate(Asn1InputStream dIn)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
			if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
			{
				this.sData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], true)).Certificates;
				return this.GetCertificate();
			}
			return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(asn1Sequence));
		}

		private IX509AttributeCertificate GetCertificate()
		{
			if (this.sData != null)
			{
				while (this.sDataObjectCount < this.sData.Count)
				{
					object obj = this.sData[this.sDataObjectCount++];
					if (obj is Asn1TaggedObject && ((Asn1TaggedObject)obj).TagNo == 2)
					{
						return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)obj, false)));
					}
				}
			}
			return null;
		}

		private IX509AttributeCertificate ReadPemCertificate(Stream inStream)
		{
			Asn1Sequence asn1Sequence = X509AttrCertParser.PemAttrCertParser.ReadPemObject(inStream);
			if (asn1Sequence != null)
			{
				return new X509V2AttributeCertificate(AttributeCertificate.GetInstance(asn1Sequence));
			}
			return null;
		}

		public IX509AttributeCertificate ReadAttrCert(byte[] input)
		{
			return this.ReadAttrCert(new MemoryStream(input, false));
		}

		public ICollection ReadAttrCerts(byte[] input)
		{
			return this.ReadAttrCerts(new MemoryStream(input, false));
		}

		public IX509AttributeCertificate ReadAttrCert(Stream inStream)
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
				this.sData = null;
				this.sDataObjectCount = 0;
			}
			else if (this.currentStream != inStream)
			{
				this.currentStream = inStream;
				this.sData = null;
				this.sDataObjectCount = 0;
			}
			IX509AttributeCertificate result;
			try
			{
				if (this.sData != null)
				{
					if (this.sDataObjectCount != this.sData.Count)
					{
						result = this.GetCertificate();
					}
					else
					{
						this.sData = null;
						this.sDataObjectCount = 0;
						result = null;
					}
				}
				else
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
						if (num != 48)
						{
							result = this.ReadPemCertificate(pushbackStream);
						}
						else
						{
							result = this.ReadDerCertificate(new Asn1InputStream(pushbackStream));
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new CertificateException(ex.ToString());
			}
			return result;
		}

		public ICollection ReadAttrCerts(Stream inStream)
		{
			IList list = Platform.CreateArrayList();
			IX509AttributeCertificate value;
			while ((value = this.ReadAttrCert(inStream)) != null)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
