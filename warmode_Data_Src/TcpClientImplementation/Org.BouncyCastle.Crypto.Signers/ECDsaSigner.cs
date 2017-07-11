using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class ECDsaSigner : IDsa
	{
		protected readonly IDsaKCalculator kCalculator;

		protected ECKeyParameters key;

		protected SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "ECDSA";
			}
		}

		public ECDsaSigner()
		{
			this.kCalculator = new RandomDsaKCalculator();
		}

		public ECDsaSigner(IDsaKCalculator kCalculator)
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
				if (!(parameters is ECPrivateKeyParameters))
				{
					throw new InvalidKeyException("EC private key required for signing");
				}
				this.key = (ECPrivateKeyParameters)parameters;
			}
			else
			{
				if (!(parameters is ECPublicKeyParameters))
				{
					throw new InvalidKeyException("EC public key required for verification");
				}
				this.key = (ECPublicKeyParameters)parameters;
			}
			this.random = this.InitSecureRandom(forSigning && !this.kCalculator.IsDeterministic, provided);
		}

		public virtual BigInteger[] GenerateSignature(byte[] message)
		{
			ECDomainParameters parameters = this.key.Parameters;
			BigInteger n = parameters.N;
			BigInteger bigInteger = this.CalculateE(n, message);
			BigInteger d = ((ECPrivateKeyParameters)this.key).D;
			if (this.kCalculator.IsDeterministic)
			{
				this.kCalculator.Init(n, d, message);
			}
			else
			{
				this.kCalculator.Init(n, this.random);
			}
			ECMultiplier eCMultiplier = this.CreateBasePointMultiplier();
			BigInteger bigInteger3;
			BigInteger bigInteger4;
			while (true)
			{
				BigInteger bigInteger2 = this.kCalculator.NextK();
				ECPoint eCPoint = eCMultiplier.Multiply(parameters.G, bigInteger2).Normalize();
				bigInteger3 = eCPoint.AffineXCoord.ToBigInteger().Mod(n);
				if (bigInteger3.SignValue != 0)
				{
					bigInteger4 = bigInteger2.ModInverse(n).Multiply(bigInteger.Add(d.Multiply(bigInteger3))).Mod(n);
					if (bigInteger4.SignValue != 0)
					{
						break;
					}
				}
			}
			return new BigInteger[]
			{
				bigInteger3,
				bigInteger4
			};
		}

		public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			BigInteger n = this.key.Parameters.N;
			if (r.SignValue < 1 || s.SignValue < 1 || r.CompareTo(n) >= 0 || s.CompareTo(n) >= 0)
			{
				return false;
			}
			BigInteger bigInteger = this.CalculateE(n, message);
			BigInteger val = s.ModInverse(n);
			BigInteger a = bigInteger.Multiply(val).Mod(n);
			BigInteger b = r.Multiply(val).Mod(n);
			ECPoint g = this.key.Parameters.G;
			ECPoint q = ((ECPublicKeyParameters)this.key).Q;
			ECPoint eCPoint = ECAlgorithms.SumOfTwoMultiplies(g, a, q, b).Normalize();
			if (eCPoint.IsInfinity)
			{
				return false;
			}
			BigInteger bigInteger2 = eCPoint.AffineXCoord.ToBigInteger().Mod(n);
			return bigInteger2.Equals(r);
		}

		protected virtual BigInteger CalculateE(BigInteger n, byte[] message)
		{
			int num = message.Length * 8;
			BigInteger bigInteger = new BigInteger(1, message);
			if (n.BitLength < num)
			{
				bigInteger = bigInteger.ShiftRight(num - n.BitLength);
			}
			return bigInteger;
		}

		protected virtual ECMultiplier CreateBasePointMultiplier()
		{
			return new FixedPointCombMultiplier();
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
