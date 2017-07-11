using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Agreement.Srp
{
	public class Srp6Server
	{
		protected BigInteger N;

		protected BigInteger g;

		protected BigInteger v;

		protected SecureRandom random;

		protected IDigest digest;

		protected BigInteger A;

		protected BigInteger privB;

		protected BigInteger pubB;

		protected BigInteger u;

		protected BigInteger S;

		protected BigInteger M1;

		protected BigInteger M2;

		protected BigInteger Key;

		public virtual void Init(BigInteger N, BigInteger g, BigInteger v, IDigest digest, SecureRandom random)
		{
			this.N = N;
			this.g = g;
			this.v = v;
			this.random = random;
			this.digest = digest;
		}

		public virtual void Init(Srp6GroupParameters group, BigInteger v, IDigest digest, SecureRandom random)
		{
			this.Init(group.N, group.G, v, digest, random);
		}

		public virtual BigInteger GenerateServerCredentials()
		{
			BigInteger bigInteger = Srp6Utilities.CalculateK(this.digest, this.N, this.g);
			this.privB = this.SelectPrivateValue();
			this.pubB = bigInteger.Multiply(this.v).Mod(this.N).Add(this.g.ModPow(this.privB, this.N)).Mod(this.N);
			return this.pubB;
		}

		public virtual BigInteger CalculateSecret(BigInteger clientA)
		{
			this.A = Srp6Utilities.ValidatePublicValue(this.N, clientA);
			this.u = Srp6Utilities.CalculateU(this.digest, this.N, this.A, this.pubB);
			this.S = this.CalculateS();
			return this.S;
		}

		protected virtual BigInteger SelectPrivateValue()
		{
			return Srp6Utilities.GeneratePrivateValue(this.digest, this.N, this.g, this.random);
		}

		private BigInteger CalculateS()
		{
			return this.v.ModPow(this.u, this.N).Multiply(this.A).Mod(this.N).ModPow(this.privB, this.N);
		}

		public virtual bool VerifyClientEvidenceMessage(BigInteger clientM1)
		{
			if (this.A == null || this.pubB == null || this.S == null)
			{
				throw new CryptoException("Impossible to compute and verify M1: some data are missing from the previous operations (A,B,S)");
			}
			BigInteger bigInteger = Srp6Utilities.CalculateM1(this.digest, this.N, this.A, this.pubB, this.S);
			if (bigInteger.Equals(clientM1))
			{
				this.M1 = clientM1;
				return true;
			}
			return false;
		}

		public virtual BigInteger CalculateServerEvidenceMessage()
		{
			if (this.A == null || this.M1 == null || this.S == null)
			{
				throw new CryptoException("Impossible to compute M2: some data are missing from the previous operations (A,M1,S)");
			}
			this.M2 = Srp6Utilities.CalculateM2(this.digest, this.N, this.A, this.M1, this.S);
			return this.M2;
		}

		public virtual BigInteger CalculateSessionKey()
		{
			if (this.S == null || this.M1 == null || this.M2 == null)
			{
				throw new CryptoException("Impossible to compute Key: some data are missing from the previous operations (S,M1,M2)");
			}
			this.Key = Srp6Utilities.CalculateKey(this.digest, this.N, this.S);
			return this.Key;
		}
	}
}
