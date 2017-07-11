using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class ECNRSigner : IDsa
	{
		private bool forSigning;

		private ECKeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "ECNR";
			}
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			this.forSigning = forSigning;
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
			if (!this.forSigning)
			{
				throw new InvalidOperationException("not initialised for signing");
			}
			BigInteger n = ((ECPrivateKeyParameters)this.key).Parameters.N;
			int bitLength = n.BitLength;
			BigInteger bigInteger = new BigInteger(1, message);
			int bitLength2 = bigInteger.BitLength;
			ECPrivateKeyParameters eCPrivateKeyParameters = (ECPrivateKeyParameters)this.key;
			if (bitLength2 > bitLength)
			{
				throw new DataLengthException("input too large for ECNR key.");
			}
			AsymmetricCipherKeyPair asymmetricCipherKeyPair;
			BigInteger bigInteger3;
			do
			{
				ECKeyPairGenerator eCKeyPairGenerator = new ECKeyPairGenerator();
				eCKeyPairGenerator.Init(new ECKeyGenerationParameters(eCPrivateKeyParameters.Parameters, this.random));
				asymmetricCipherKeyPair = eCKeyPairGenerator.GenerateKeyPair();
				ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)asymmetricCipherKeyPair.Public;
				BigInteger bigInteger2 = eCPublicKeyParameters.Q.AffineXCoord.ToBigInteger();
				bigInteger3 = bigInteger2.Add(bigInteger).Mod(n);
			}
			while (bigInteger3.SignValue == 0);
			BigInteger d = eCPrivateKeyParameters.D;
			BigInteger d2 = ((ECPrivateKeyParameters)asymmetricCipherKeyPair.Private).D;
			BigInteger bigInteger4 = d2.Subtract(bigInteger3.Multiply(d)).Mod(n);
			return new BigInteger[]
			{
				bigInteger3,
				bigInteger4
			};
		}

		public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
		{
			if (this.forSigning)
			{
				throw new InvalidOperationException("not initialised for verifying");
			}
			ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)this.key;
			BigInteger n = eCPublicKeyParameters.Parameters.N;
			int bitLength = n.BitLength;
			BigInteger bigInteger = new BigInteger(1, message);
			int bitLength2 = bigInteger.BitLength;
			if (bitLength2 > bitLength)
			{
				throw new DataLengthException("input too large for ECNR key.");
			}
			if (r.CompareTo(BigInteger.One) < 0 || r.CompareTo(n) >= 0)
			{
				return false;
			}
			if (s.CompareTo(BigInteger.Zero) < 0 || s.CompareTo(n) >= 0)
			{
				return false;
			}
			ECPoint g = eCPublicKeyParameters.Parameters.G;
			ECPoint q = eCPublicKeyParameters.Q;
			ECPoint eCPoint = ECAlgorithms.SumOfTwoMultiplies(g, s, q, r).Normalize();
			if (eCPoint.IsInfinity)
			{
				return false;
			}
			BigInteger n2 = eCPoint.AffineXCoord.ToBigInteger();
			BigInteger bigInteger2 = r.Subtract(n2).Mod(n);
			return bigInteger2.Equals(bigInteger);
		}
	}
}
