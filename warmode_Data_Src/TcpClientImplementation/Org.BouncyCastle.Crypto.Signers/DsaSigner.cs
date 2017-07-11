using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class DsaSigner : IDsa
	{
		protected readonly IDsaKCalculator kCalculator;

		protected DsaKeyParameters key;

		protected SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "DSA";
			}
		}

		public DsaSigner()
		{
			this.kCalculator = new RandomDsaKCalculator();
		}

		public DsaSigner(IDsaKCalculator kCalculator)
		{
			this.kCalculator = kCalculator;
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			SecureRandom provided = null;
			if (forSigning)
			{
				if (parameters is ParametersWithRandom)
				{
					ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
					provided = parametersWithRandom.Random;
					parameters = parametersWithRandom.Parameters;
				}
				if (!(parameters is DsaPrivateKeyParameters))
				{
					throw new InvalidKeyException("DSA private key required for signing");
				}
				this.key = (DsaPrivateKeyParameters)parameters;
			}
			else
			{
				if (!(parameters is DsaPublicKeyParameters))
				{
					throw new InvalidKeyException("DSA public key required for verification");
				}
				this.key = (DsaPublicKeyParameters)parameters;
			}
			this.random = this.InitSecureRandom(forSigning && !this.kCalculator.IsDeterministic, provided);
		}

		public virtual BigInteger[] GenerateSignature(byte[] message)
		{
			DsaParameters parameters = this.key.Parameters;
			BigInteger q = parameters.Q;
			BigInteger bigInteger = this.CalculateE(q, message);
			BigInteger x = ((DsaPrivateKeyParameters)this.key).X;
			if (this.kCalculator.IsDeterministic)
			{
				this.kCalculator.Init(q, x, message);
			}
			else
			{
				this.kCalculator.Init(q, this.random);
			}
			BigInteger bigInteger2 = this.kCalculator.NextK();
			BigInteger bigInteger3 = parameters.G.ModPow(bigInteger2, parameters.P).Mod(q);
			bigInteger2 = bigInteger2.ModInverse(q).Multiply(bigInteger.Add(x.Multiply(bigInteger3)));
			BigInteger bigInteger4 = bigInteger2.Mod(q);
			return new BigInteger[]
			{
				bigInteger3,
				bigInteger4
			};
		}

		public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			DsaParameters parameters = this.key.Parameters;
			BigInteger q = parameters.Q;
			BigInteger bigInteger = this.CalculateE(q, message);
			if (r.SignValue <= 0 || q.CompareTo(r) <= 0)
			{
				return false;
			}
			if (s.SignValue <= 0 || q.CompareTo(s) <= 0)
			{
				return false;
			}
			BigInteger val = s.ModInverse(q);
			BigInteger bigInteger2 = bigInteger.Multiply(val).Mod(q);
			BigInteger bigInteger3 = r.Multiply(val).Mod(q);
			BigInteger p = parameters.P;
			bigInteger2 = parameters.G.ModPow(bigInteger2, p);
			bigInteger3 = ((DsaPublicKeyParameters)this.key).Y.ModPow(bigInteger3, p);
			BigInteger bigInteger4 = bigInteger2.Multiply(bigInteger3).Mod(p).Mod(q);
			return bigInteger4.Equals(r);
		}

		protected virtual BigInteger CalculateE(BigInteger n, byte[] message)
		{
			int length = Math.Min(message.Length, n.BitLength / 8);
			return new BigInteger(1, message, 0, length);
		}

		protected virtual SecureRandom InitSecureRandom(bool needed, SecureRandom provided)
		{
			if (!needed)
			{
				return null;
			}
			if (provided == null)
			{
				return new SecureRandom();
			}
			return provided;
		}
	}
}
