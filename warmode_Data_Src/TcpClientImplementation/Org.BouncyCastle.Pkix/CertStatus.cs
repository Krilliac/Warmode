using Org.BouncyCastle.Utilities.Date;
using System;

namespace Org.BouncyCastle.Pkix
{
	public class CertStatus
	{
		public const int Unrevoked = 11;

		public const int Undetermined = 12;

		private int status = 11;

		private DateTimeObject revocationDate;

		public DateTimeObject RevocationDate
		{
			get
			{
				return this.revocationDate;
			}
			set
			{
				this.revocationDate = value;
			}
		}

		public int Status
		{
			get
			{
				return this.status;
			}
			set
			{
				this.status = value;
			}
		}
	}
}
