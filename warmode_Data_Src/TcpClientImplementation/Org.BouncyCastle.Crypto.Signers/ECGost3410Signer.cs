using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class ECGost3410Signer : IDsa
	{
		private ECKeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "ECGOST3410";
			}
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			if (forSigning)
			{
				if (parameters is ParametersWithRandom)
				{
					ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
					this.random = parametersWithRandom.Random;
					parameters = parametersWithRandom.Parameters;
				}
				else
				{
					this.random = new SecureRandom();
				}
				if (!(parameters is ECPrivateKeyParameters))
				{
					throw new InvalidKeyException("EC private key required for signing");
				}
				this.key = (ECPrivateKeyParameters)parameters;
				return;
			}
			else
			{
				if (!(parameters is ECPublicKeyParameters))
				{
					throw new InvalidKeyException("EC public key required for verification");
				}
				this.key = (ECPublicKeyParameters)parameters;
				return;
			}
		}

		public virtual BigInteger[] GenerateSignature(byte[] message)
		{
			byte[] array = new byte[message.Length];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = message[array.Length - 1 - num];
			}
			BigInteger val = new BigInteger(1, array);
			ECDomainParameters parameters = this.key.Parameters;
			BigInteger n = parameters.N;
			BigInteger d = ((ECPrivateKeyParameters)this.key).D;
			ECMultiplier eCMultiplier = this.CreateBasePointMultiplier();
			BigInteger bigInteger2;
			BigInteger bigInteger3;
			while (true)
			{
				BigInteger bigInteger = new BigInteger(n.BitLength, this.random);
				if (bigInteger.SignValue != 0)
				{
					ECPoint eCPoint = eCMultiplier.Multiply(parameters.G, bigInteger).Normalize();
					bigInteger2 = eCPoint.AffineXCoord.ToBigInteger().Mod(n);
					if (bigInteger2.SignValue != 0)
					{
						bigInteger3 = bigInteger.Multiply(val).Add(d.Multiply(bigInteger2)).Mod(n);
						if (bigInteger3.SignValue != 0)
						{
							break;
						}
					}
				}
			}
			return new BigInteger[]
			{
				bigInteger2,
				bigInteger3
			};
		}

		public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			byte[] array = new byte[message.Length];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = message[array.Length - 1 - num];
			}
			BigInteger bigInteger = new BigInteger(1, array);
			BigInteger n = this.key.Parameters.N;
			if (r.CompareTo(BigInteger.One) < 0 || r.CompareTo(n) >= 0)
			{
				return false;
			}
			if (s.CompareTo(BigInteger.One) < 0 || s.CompareTo(n) >= 0)
			{
				return false;
			}
			BigInteger val = bigInteger.ModInverse(n);
			BigInteger a = s.Multiply(val).Mod(n);
			BigInteger b = n.Subtract(r).Multiply(val).Mod(n);
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

		protected virtual ECMultiplier CreateBasePointMultiplier()
		{
			return new FixedPointCombMultiplier();
		}
	}
}
