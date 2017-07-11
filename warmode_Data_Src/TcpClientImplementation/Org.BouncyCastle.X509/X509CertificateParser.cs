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
	public class X509CertificateParser
	{
		private static readonly PemParser PemCertParser = new PemParser("CERTIFICATE");

		private Asn1Set sData;

		private int sDataObjectCount;

		private Stream currentStream;

		private X509Certificate ReadDerCertificate(Asn1InputStream dIn)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
			if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
			{
				this.sData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], true)).Certificates;
				return this.GetCertificate();
			}
			return this.CreateX509Certificate(X509CertificateStructure.GetInstance(asn1Sequence));
		}

		private X509Certificate GetCertificate()
		{
			if (this.sData != null)
			{
				while (this.sDataObjectCount < this.sData.Count)
				{
					object obj = this.sData[this.sDataObjectCount++];
					if (obj is Asn1Sequence)
					{
						return this.CreateX509Certificate(X509CertificateStructure.GetInstance(obj));
					}
				}
			}
			return null;
		}

		private X509Certificate ReadPemCertificate(Stream inStream)
		{
			Asn1Sequence asn1Sequence = X509CertificateParser.PemCertParser.ReadPemObject(inStream);
			if (asn1Sequence != null)
			{
				return this.CreateX509Certificate(X509CertificateStructure.GetInstance(asn1Sequence));
			}
			return null;
		}

		protected virtual X509Certificate CreateX509Certificate(X509CertificateStructure c)
		{
			return new X509Certificate(c);
		}

		public X509Certificate ReadCertificate(byte[] input)
		{
			return this.ReadCertificate(new MemoryStream(input, false));
		}

		public ICollection ReadCertificates(byte[] input)
		{
			return this.ReadCertificates(new MemoryStream(input, false));
		}

		public X509Certificate ReadCertificate(Stream inStream)
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
			X509Certificate result;
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
			catch (Exception exception)
			{
				throw new CertificateException("Failed to read certificate", exception);
			}
			return result;
		}

		public ICollection ReadCertificates(Stream inStream)
		{
			IList list = Platform.CreateArrayList();
			X509Certificate value;
			while ((value = this.ReadCertificate(inStream)) != null)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
