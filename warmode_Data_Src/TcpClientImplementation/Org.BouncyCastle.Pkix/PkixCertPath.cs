using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Pkix
{
	public class PkixCertPath
	{
		internal static readonly IList certPathEncodings;

		private readonly IList certificates;

		public virtual IEnumerable Encodings
		{
			get
			{
				return new EnumerableProxy(PkixCertPath.certPathEncodings);
			}
		}

		public virtual IList Certificates
		{
			get
			{
				return CollectionUtilities.ReadOnly(this.certificates);
			}
		}

		static PkixCertPath()
		{
			IList list = Platform.CreateArrayList();
			list.Add("PkiPath");
			list.Add("PEM");
			list.Add("PKCS7");
			PkixCertPath.certPathEncodings = CollectionUtilities.ReadOnly(list);
		}

		private static IList SortCerts(IList certs)
		{
			if (certs.Count < 2)
			{
				return certs;
			}
			X509Name issuerDN = ((X509Certificate)certs[0]).IssuerDN;
			bool flag = true;
			for (int num = 1; num != certs.Count; num++)
			{
				X509Certificate x509Certificate = (X509Certificate)certs[num];
				if (!issuerDN.Equivalent(x509Certificate.SubjectDN, true))
				{
					flag = false;
					break;
				}
				issuerDN = ((X509Certificate)certs[num]).IssuerDN;
			}
			if (flag)
			{
				return certs;
			}
			IList list = Platform.CreateArrayList(certs.Count);
			IList result = Platform.CreateArrayList(certs);
			for (int i = 0; i < certs.Count; i++)
			{
				X509Certificate x509Certificate2 = (X509Certificate)certs[i];
				bool flag2 = false;
				X509Name subjectDN = x509Certificate2.SubjectDN;
				foreach (X509Certificate x509Certificate3 in certs)
				{
					if (x509Certificate3.IssuerDN.Equivalent(subjectDN, true))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					list.Add(x509Certificate2);
					certs.RemoveAt(i);
				}
			}
			if (list.Count > 1)
			{
				return result;
			}
			for (int num2 = 0; num2 != list.Count; num2++)
			{
				issuerDN = ((X509Certificate)list[num2]).IssuerDN;
				for (int j = 0; j < certs.Count; j++)
				{
					X509Certificate x509Certificate4 = (X509Certificate)certs[j];
					if (issuerDN.Equivalent(x509Certificate4.SubjectDN, true))
					{
						list.Add(x509Certificate4);
						certs.RemoveAt(j);
						break;
					}
				}
			}
			if (certs.Count > 0)
			{
				return result;
			}
			return list;
		}

		public PkixCertPath(ICollection certificates)
		{
			this.certificates = PkixCertPath.SortCerts(Platform.CreateArrayList(certificates));
		}

		public PkixCertPath(Stream inStream) : this(inStream, "PkiPath")
		{
		}

		public PkixCertPath(Stream inStream, string encoding)
		{
			string text = encoding.ToUpper();
			IList list;
			try
			{
				if (text.Equals("PkiPath".ToUpper()))
				{
					Asn1InputStream asn1InputStream = new Asn1InputStream(inStream);
					Asn1Object asn1Object = asn1InputStream.ReadObject();
					if (!(asn1Object is Asn1Sequence))
					{
						throw new CertificateException("input stream does not contain a ASN1 SEQUENCE while reading PkiPath encoded data to load CertPath");
					}
					list = Platform.CreateArrayList();
					using (IEnumerator enumerator = ((Asn1Sequence)asn1Object).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Asn1Encodable asn1Encodable = (Asn1Encodable)enumerator.Current;
							byte[] encoded = asn1Encodable.GetEncoded("DER");
							Stream inStream2 = new MemoryStream(encoded, false);
							list.Insert(0, new X509CertificateParser().ReadCertificate(inStream2));
						}
						goto IL_EF;
					}
				}
				if (!text.Equals("PKCS7") && !text.Equals("PEM"))
				{
					throw new CertificateException("unsupported encoding: " + encoding);
				}
				list = Platform.CreateArrayList(new X509CertificateParser().ReadCertificates(inStream));
				IL_EF:;
			}
			catch (IOException ex)
			{
				throw new CertificateException("IOException throw while decoding CertPath:\n" + ex.ToString());
			}
			this.certificates = PkixCertPath.SortCerts(list);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			PkixCertPath pkixCertPath = obj as PkixCertPath;
			if (pkixCertPath == null)
			{
				return false;
			}
			IList list = this.Certificates;
			IList list2 = pkixCertPath.Certificates;
			if (list.Count != list2.Count)
			{
				return false;
			}
			IEnumerator enumerator = list.GetEnumerator();
			IEnumerator enumerator2 = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator2.MoveNext();
				if (!object.Equals(enumerator.Current, enumerator2.Current))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return this.Certificates.GetHashCode();
		}

		public virtual byte[] GetEncoded()
		{
			foreach (object current in this.Encodings)
			{
				if (current is string)
				{
					return this.GetEncoded((string)current);
				}
			}
			return null;
		}

		public virtual byte[] GetEncoded(string encoding)
		{
			if (Platform.CompareIgnoreCase(encoding, "PkiPath") == 0)
			{
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
				for (int i = this.certificates.Count - 1; i >= 0; i--)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						this.ToAsn1Object((X509Certificate)this.certificates[i])
					});
				}
				return this.ToDerEncoded(new DerSequence(asn1EncodableVector));
			}
			if (Platform.CompareIgnoreCase(encoding, "PKCS7") == 0)
			{
				ContentInfo contentInfo = new ContentInfo(PkcsObjectIdentifiers.Data, null);
				Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
				for (int num = 0; num != this.certificates.Count; num++)
				{
					asn1EncodableVector2.Add(new Asn1Encodable[]
					{
						this.ToAsn1Object((X509Certificate)this.certificates[num])
					});
				}
				SignedData content = new SignedData(new DerInteger(1), new DerSet(), contentInfo, new DerSet(asn1EncodableVector2), null, new DerSet());
				return this.ToDerEncoded(new ContentInfo(PkcsObjectIdentifiers.SignedData, content));
			}
			if (Platform.CompareIgnoreCase(encoding, "PEM") == 0)
			{
				MemoryStream memoryStream = new MemoryStream();
				PemWriter pemWriter = new PemWriter(new StreamWriter(memoryStream));
				try
				{
					for (int num2 = 0; num2 != this.certificates.Count; num2++)
					{
						pemWriter.WriteObject(this.certificates[num2]);
					}
					pemWriter.Writer.Close();
				}
				catch (Exception)
				{
					throw new CertificateEncodingException("can't encode certificate for PEM encoded path");
				}
				return memoryStream.ToArray();
			}
			throw new CertificateEncodingException("unsupported encoding: " + encoding);
		}

		private Asn1Object ToAsn1Object(X509Certificate cert)
		{
			Asn1Object result;
			try
			{
				result = Asn1Object.FromByteArray(cert.GetEncoded());
			}
			catch (Exception e)
			{
				throw new CertificateEncodingException("Exception while encoding certificate", e);
			}
			return result;
		}

		private byte[] ToDerEncoded(Asn1Encodable obj)
		{
			byte[] encoded;
			try
			{
				encoded = obj.GetEncoded("DER");
			}
			catch (IOException e)
			{
				throw new CertificateEncodingException("Exception thrown", e);
			}
			return encoded;
		}
	}
}
