using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Date;
using Org.BouncyCastle.X509;
using System;

namespace Org.BouncyCastle.Ocsp
{
	public class SingleResp : X509ExtensionBase
	{
		internal readonly SingleResponse resp;

		public DateTime ThisUpdate
		{
			get
			{
				return this.resp.ThisUpdate.ToDateTime();
			}
		}

		public DateTimeObject NextUpdate
		{
			get
			{
				if (this.resp.NextUpdate != null)
				{
					return new DateTimeObject(this.resp.NextUpdate.ToDateTime());
				}
				return null;
			}
		}

		public X509Extensions SingleExtensions
		{
			get
			{
				return this.resp.SingleExtensions;
			}
		}

		public SingleResp(SingleResponse resp)
		{
			this.resp = resp;
		}

		public CertificateID GetCertID()
		{
			return new CertificateID(this.resp.CertId);
		}

		public object GetCertStatus()
		{
			CertStatus certStatus = this.resp.CertStatus;
			if (certStatus.TagNo == 0)
			{
				return null;
			}
			if (certStatus.TagNo == 1)
			{
				return new RevokedStatus(RevokedInfo.GetInstance(certStatus.Status));
			}
			return new UnknownStatus();
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.SingleExtensions;
		}
	}
}
