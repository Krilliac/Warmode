using Org.BouncyCastle.Math;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class NaccacheSternPrivateKeyParameters : NaccacheSternKeyParameters
	{
		private readonly BigInteger phiN;

		private readonly IList smallPrimes;

		public BigInteger PhiN
		{
			get
			{
				return this.phiN;
			}
		}

		[Obsolete("Use 'SmallPrimesList' instead")]
		public ArrayList SmallPrimes
		{
			get
			{
				return new ArrayList(this.smallPrimes);
			}
		}

		public IList SmallPrimesList
		{
			get
			{
				return this.smallPrimes;
			}
		}

		[Obsolete]
		public NaccacheSternPrivateKeyParameters(BigInteger g, BigInteger n, int lowerSigmaBound, ArrayList smallPrimes, BigInteger phiN) : base(true, g, n, lowerSigmaBound)
		{
			this.smallPrimes = smallPrimes;
			this.phiN = phiN;
		}

		public NaccacheSternPrivateKeyParameters(BigInteger g, BigInteger n, int lowerSigmaBound, IList smallPrimes, BigInteger phiN) : base(true, g, n, lowerSigmaBound)
		{
			this.smallPrimes = smallPrimes;
			this.phiN = phiN;
		}
	}
}
