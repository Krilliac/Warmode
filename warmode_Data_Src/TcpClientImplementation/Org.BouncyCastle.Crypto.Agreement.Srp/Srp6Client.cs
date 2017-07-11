using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Agreement.Srp
{
	public class Srp6Client
	{
		protected BigInteger N;

		protected BigInteger g;

		protected BigInteger privA;

		protected BigInteger pubA;

		protected BigInteger B;

		protected BigInteger x;

		protected BigInteger u;

		protected BigInteger S;

		protected BigInteger M1;

		protected BigInteger M2;

		protected BigInteger Key;

		protected IDigest digest;

		protected SecureRandom random;

		public virtual void Init(BigInteger N, BigInteger g, IDigest digest, SecureRandom random)
		{
			this.N = N;
			this.g = g;
			this.digest = digest;
			this.random = random;
		}

		public virtual void Init(Srp6GroupParameters group, IDigest digest, SecureRandom random)
		{
			this.Init(group.N, group.G, digest, random);
		}

		public virtual BigInteger GenerateClientCredentials(byte[] salt, byte[] identity, byte[] password)
		{
			this.x = Srp6Utilities.CalculateX(this.digest, this.N, salt, identity, password);
			this.privA = this.SelectPrivateValue();
			this.pubA = this.g.ModPow(this.privA, this.N);
			return this.pubA;
		}

		public virtual BigInteger CalculateSecret(BigInteger serverB)
		{
			this.B = Srp6Utilities.ValidatePublicValue(this.N, serverB);
			this.u = Srp6Utilities.CalculateU(this.digest, this.N, this.pubA, this.B);
			this.S = this.CalculateS();
			return this.S;
		}

		protected virtual BigInteger SelectPrivateValue()
		{
			return Srp6Utilities.GeneratePrivateValue(this.digest, this.N, this.g, this.random);
		}

		private BigInteger CalculateS()
		{
			BigInteger val = Srp6Utilities.CalculateK(this.digest, this.N, this.g);
			BigInteger e = this.u.Multiply(this.x).Add(this.privA);
			BigInteger n = this.g.ModPow(this.x, this.N).Multiply(val).Mod(this.N);
			return this.B.Subtract(n).Mod(this.N).ModPow(e, this.N);
		}

		public virtual BigInteger CalculateClientEvidenceMessage()
		{
			if (this.pubA == null || this.B == null || this.S == null)
			{
				throw new CryptoException("Impossible to compute M1: some data are missing from the previous operations (A,B,S)");
			}
			this.M1 = Srp6Utilities.CalculateM1(this.digest, this.N, this.pubA, this.B, this.S);
			return this.M1;
		}

		public virtual bool VerifyServerEvidenceMessage(BigInteger serverM2)
		{
			if (this.pubA == null || this.M1 == null || this.S == null)
			{
				throw new CryptoException("Impossible to compute and verify M2: some data are missing from the previous operations (A,M1,S)");
			}
			BigInteger bigInteger = Srp6Utilities.CalculateM2(this.digest, this.N, this.pubA, this.M1, this.S);
			if (bigInteger.Equals(serverM2))
			{
				this.M2 = serverM2;
				return true;
			}
			return false;
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
