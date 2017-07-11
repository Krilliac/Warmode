using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class DsaParameterGenerationParameters
	{
		public const int DigitalSignatureUsage = 1;

		public const int KeyEstablishmentUsage = 2;

		private readonly int l;

		private readonly int n;

		private readonly int certainty;

		private readonly SecureRandom random;

		private readonly int usageIndex;

		public virtual int L
		{
			get
			{
				return this.l;
			}
		}

		public virtual int N
		{
			get
			{
				return this.n;
			}
		}

		public virtual int UsageIndex
		{
			get
			{
				return this.usageIndex;
			}
		}

		public virtual int Certainty
		{
			get
			{
				return this.certainty;
			}
		}

		public virtual SecureRandom Random
		{
			get
			{
				return this.random;
			}
		}

		public DsaParameterGenerationParameters(int L, int N, int certainty, SecureRandom random) : this(L, N, certainty, random, -1)
		{
		}

		public DsaParameterGenerationParameters(int L, int N, int certainty, SecureRandom random, int usageIndex)
		{
			this.l = L;
			this.n = N;
			this.certainty = certainty;
			this.random = random;
			this.usageIndex = usageIndex;
		}
	}
}
