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
	public class X509CrlParser
	{
		private static readonly PemParser PemCrlParser = new PemParser("CRL");

		private readonly bool lazyAsn1;

		private Asn1Set sCrlData;

		private int sCrlDataObjectCount;

		private Stream currentCrlStream;

		public X509CrlParser() : this(false)
		{
		}

		public X509CrlParser(bool lazyAsn1)
		{
			this.lazyAsn1 = lazyAsn1;
		}

		private X509Crl ReadPemCrl(Stream inStream)
		{
			Asn1Sequence asn1Sequence = X509CrlParser.PemCrlParser.ReadPemObject(inStream);
			if (asn1Sequence != null)
			{
				return this.CreateX509Crl(CertificateList.GetInstance(asn1Sequence));
			}
			return null;
		}

		private X509Crl ReadDerCrl(Asn1InputStream dIn)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)dIn.ReadObject();
			if (asn1Sequence.Count > 1 && asn1Sequence[0] is DerObjectIdentifier && asn1Sequence[0].Equals(PkcsObjectIdentifiers.SignedData))
			{
				this.sCrlData = SignedData.GetInstance(Asn1Sequence.GetInstance((Asn1TaggedObject)asn1Sequence[1], true)).Crls;
				return this.GetCrl();
			}
			return this.CreateX509Crl(CertificateList.GetInstance(asn1Sequence));
		}

		private X509Crl GetCrl()
		{
			if (this.sCrlData == null || this.sCrlDataObjectCount >= this.sCrlData.Count)
			{
				return null;
			}
			return this.CreateX509Crl(CertificateList.GetInstance(this.sCrlData[this.sCrlDataObjectCount++]));
		}

		protected virtual X509Crl CreateX509Crl(CertificateList c)
		{
			return new X509Crl(c);
		}

		public X509Crl ReadCrl(byte[] input)
		{
			return this.ReadCrl(new MemoryStream(input, false));
		}

		public ICollection ReadCrls(byte[] input)
		{
			return this.ReadCrls(new MemoryStream(input, false));
		}

		public X509Crl ReadCrl(Stream inStream)
		{
			if (inStream == null)
			{
				throw new ArgumentNullException("inStream");
			}
			if (!inStream.CanRead)
			{
				throw new ArgumentException("inStream must be read-able", "inStream");
			}
			if (this.currentCrlStream == null)
			{
				this.currentCrlStream = inStream;
				this.sCrlData = null;
				this.sCrlDataObjectCount = 0;
			}
			else if (this.currentCrlStream != inStream)
			{
				this.currentCrlStream = inStream;
				this.sCrlData = null;
				this.sCrlDataObjectCount = 0;
			}
			X509Crl result;
			try
			{
				if (this.sCrlData != null)
				{
					if (this.sCrlDataObjectCount != this.sCrlData.Count)
					{
						result = this.GetCrl();
					}
					else
					{
						this.sCrlData = null;
						this.sCrlDataObjectCount = 0;
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
							result = this.ReadPemCrl(pushbackStream);
						}
						else
						{
							Asn1InputStream dIn = this.lazyAsn1 ? new LazyAsn1InputStream(pushbackStream) : new Asn1InputStream(pushbackStream);
							result = this.ReadDerCrl(dIn);
						}
					}
				}
			}
			catch (CrlException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new CrlException(ex2.ToString());
			}
			return result;
		}

		public ICollection ReadCrls(Stream inStream)
		{
			IList list = Platform.CreateArrayList();
			X509Crl value;
			while ((value = this.ReadCrl(inStream)) != null)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
