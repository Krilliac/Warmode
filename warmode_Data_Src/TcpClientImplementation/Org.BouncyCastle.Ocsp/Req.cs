using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using System;

namespace Org.BouncyCastle.Ocsp
{
	public class Req : X509ExtensionBase
	{
		private Request req;

		public X509Extensions SingleRequestExtensions
		{
			get
			{
				return this.req.SingleRequestExtensions;
			}
		}

		public Req(Request req)
		{
			this.req = req;
		}

		public CertificateID GetCertID()
		{
			return new CertificateID(this.req.ReqCert);
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.SingleRequestExtensions;
		}
	}
}
