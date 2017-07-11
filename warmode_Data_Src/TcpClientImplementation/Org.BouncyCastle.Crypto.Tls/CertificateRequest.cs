using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class CertificateRequest
	{
		protected readonly byte[] mCertificateTypes;

		protected readonly IList mSupportedSignatureAlgorithms;

		protected readonly IList mCertificateAuthorities;

		public virtual byte[] CertificateTypes
		{
			get
			{
				return this.mCertificateTypes;
			}
		}

		public virtual IList SupportedSignatureAlgorithms
		{
			get
			{
				return this.mSupportedSignatureAlgorithms;
			}
		}

		public virtual IList CertificateAuthorities
		{
			get
			{
				return this.mCertificateAuthorities;
			}
		}

		public CertificateRequest(byte[] certificateTypes, IList supportedSignatureAlgorithms, IList certificateAuthorities)
		{
			this.mCertificateTypes = certificateTypes;
			this.mSupportedSignatureAlgorithms = supportedSignatureAlgorithms;
			this.mCertificateAuthorities = certificateAuthorities;
		}

		public virtual void Encode(Stream output)
		{
			if (this.mCertificateTypes == null || this.mCertificateTypes.Length == 0)
			{
				TlsUtilities.WriteUint8(0, output);
			}
			else
			{
				TlsUtilities.WriteUint8ArrayWithUint8Length(this.mCertificateTypes, output);
			}
			if (this.mSupportedSignatureAlgorithms != null)
			{
				TlsUtilities.EncodeSupportedSignatureAlgorithms(this.mSupportedSignatureAlgorithms, false, output);
			}
			if (this.mCertificateAuthorities == null || this.mCertificateAuthorities.Count < 1)
			{
				TlsUtilities.WriteUint16(0, output);
				return;
			}
			IList list = Platform.CreateArrayList(this.mCertificateAuthorities.Count);
			int num = 0;
			foreach (Asn1Encodable asn1Encodable in this.mCertificateAuthorities)
			{
				byte[] encoded = asn1Encodable.GetEncoded("DER");
				list.Add(encoded);
				num += encoded.Length + 2;
			}
			TlsUtilities.CheckUint16(num);
			TlsUtilities.WriteUint16(num, output);
			foreach (byte[] buf in list)
			{
				TlsUtilities.WriteOpaque16(buf, output);
			}
		}

		public static CertificateRequest Parse(TlsContext context, Stream input)
		{
			int num = (int)TlsUtilities.ReadUint8(input);
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = TlsUtilities.ReadUint8(input);
			}
			IList supportedSignatureAlgorithms = null;
			if (TlsUtilities.IsTlsV12(context))
			{
				supportedSignatureAlgorithms = TlsUtilities.ParseSupportedSignatureAlgorithms(false, input);
			}
			IList list = Platform.CreateArrayList();
			byte[] buffer = TlsUtilities.ReadOpaque16(input);
			MemoryStream memoryStream = new MemoryStream(buffer, false);
			while (memoryStream.Position < memoryStream.Length)
			{
				byte[] encoding = TlsUtilities.ReadOpaque16(memoryStream);
				Asn1Object obj = TlsUtilities.ReadDerObject(encoding);
				list.Add(X509Name.GetInstance(obj));
			}
			return new CertificateRequest(array, supportedSignatureAlgorithms, list);
		}
	}
}
