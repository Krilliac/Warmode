using System;

namespace Org.BouncyCastle.Tsp
{
	[Serializable]
	public class TspValidationException : TspException
	{
		private int failureCode;

		public int FailureCode
		{
			get
			{
				return this.failureCode;
			}
		}

		public TspValidationException(string message) : base(message)
		{
			this.failureCode = -1;
		}

		public TspValidationException(string message, int failureCode) : base(message)
		{
			this.failureCode = failureCode;
		}
	}
}
