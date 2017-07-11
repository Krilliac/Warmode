using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class ParametersWithRandom : ICipherParameters
	{
		private readonly ICipherParameters parameters;

		private readonly SecureRandom random;

		public SecureRandom Random
		{
			get
			{
				return this.random;
			}
		}

		public ICipherParameters Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public ParametersWithRandom(ICipherParameters parameters, SecureRandom random)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("random");
			}
			if (random == null)
			{
				throw new ArgumentNullException("random");
			}
			this.parameters = parameters;
			this.random = random;
		}

		public ParametersWithRandom(ICipherParameters parameters) : this(parameters, new SecureRandom())
		{
		}

		[Obsolete("Use Random property instead")]
		public SecureRandom GetRandom()
		{
			return this.Random;
		}
	}
}
