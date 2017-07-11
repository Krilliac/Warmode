using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class CertificateStatusRequest
	{
		protected readonly byte mStatusType;

		protected readonly object mRequest;

		public virtual byte StatusType
		{
			get
			{
				return this.mStatusType;
			}
		}

		public virtual object Request
		{
			get
			{
				return this.mRequest;
			}
		}

		public CertificateStatusRequest(byte statusType, object request)
		{
			if (!CertificateStatusRequest.IsCorrectType(statusType, request))
			{
				throw new ArgumentException("not an instance of the correct type", "request");
			}
			this.mStatusType = statusType;
			this.mRequest = request;
		}

		public virtual OcspStatusRequest GetOcspStatusRequest()
		{
			if (!CertificateStatusRequest.IsCorrectType(1, this.mRequest))
			{
				throw new InvalidOperationException("'request' is not an OCSPStatusRequest");
			}
			return (OcspStatusRequest)this.mRequest;
		}

		public virtual void Encode(Stream output)
		{
			TlsUtilities.WriteUint8(this.mStatusType, output);
			byte b = this.mStatusType;
			if (b == 1)
			{
				((OcspStatusRequest)this.mRequest).Encode(output);
				return;
			}
			throw new TlsFatalAlert(80);
		}

		public static CertificateStatusRequest Parse(Stream input)
		{
			byte b = TlsUtilities.ReadUint8(input);
			byte b2 = b;
			if (b2 == 1)
			{
				object request = OcspStatusRequest.Parse(input);
				return new CertificateStatusRequest(b, request);
			}
			throw new TlsFatalAlert(50);
		}

		protected static bool IsCorrectType(byte statusType, object request)
		{
			if (statusType == 1)
			{
				return request is OcspStatusRequest;
			}
			throw new ArgumentException("unsupported value", "statusType");
		}
	}
}
