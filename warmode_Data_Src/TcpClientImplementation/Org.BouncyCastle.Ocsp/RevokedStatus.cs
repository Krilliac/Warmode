using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Ocsp
{
	public class RevokedStatus : CertificateStatus
	{
		internal readonly RevokedInfo info;

		public DateTime RevocationTime
		{
			get
			{
				return this.info.RevocationTime.ToDateTime();
			}
		}

		public bool HasRevocationReason
		{
			get
			{
				return this.info.RevocationReason != null;
			}
		}

		public int RevocationReason
		{
			get
			{
				if (this.info.RevocationReason == null)
				{
					throw new InvalidOperationException("attempt to get a reason where none is available");
				}
				return this.info.RevocationReason.Value.IntValue;
			}
		}

		public RevokedStatus(RevokedInfo info)
		{
			this.info = info;
		}

		public RevokedStatus(DateTime revocationDate, int reason)
		{
			this.info = new RevokedInfo(new DerGeneralizedTime(revocationDate), new CrlReason(reason));
		}
	}
}
