using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class Gost3410Signer : IDsa
	{
		private Gost3410KeyParameters key;

		private SecureRandom random;

		public virtual string AlgorithmName
		{
			get
			{
				return "GOST3410";
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
				if (!(parameters is Gost3410PrivateKeyParameters))
				{
					throw new InvalidKeyException("GOST3410 private key required for signing");
				}
				this.key = (Gost3410PrivateKeyParameters)parameters;
				return;
			}
			else
			{
				if (!(parameters is Gost3410PublicKeyParameters))
				{
					throw new InvalidKeyException("GOST3410 public key required for signing");
				}
				this.key = (Gost3410PublicKeyParameters)parameters;
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
			Gost3410Parameters parameters = this.key.Parameters;
			BigInteger bigInteger;
			do
			{
				bigInteger = new BigInteger(parameters.Q.BitLength, this.random);
			}
			while (bigInteger.CompareTo(parameters.Q) >= 0);
			BigInteger bigInteger2 = parameters.A.ModPow(bigInteger, parameters.P).Mod(parameters.Q);
			BigInteger bigInteger3 = bigInteger.Multiply(val).Add(((Gost3410PrivateKeyParameters)this.key).X.Multiply(bigInteger2)).Mod(parameters.Q);
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
			Gost3410Parameters parameters = this.key.Parameters;
			if (r.SignValue < 0 || parameters.Q.CompareTo(r) <= 0)
			{
				return false;
			}
			if (s.SignValue < 0 || parameters.Q.CompareTo(s) <= 0)
			{
				return false;
			}
			BigInteger val = bigInteger.ModPow(parameters.Q.Subtract(BigInteger.Two), parameters.Q);
			BigInteger bigInteger2 = s.Multiply(val).Mod(parameters.Q);
			BigInteger bigInteger3 = parameters.Q.Subtract(r).Multiply(val).Mod(parameters.Q);
			bigInteger2 = parameters.A.ModPow(bigInteger2, parameters.P);
			bigInteger3 = ((Gost3410PublicKeyParameters)this.key).Y.ModPow(bigInteger3, parameters.P);
			BigInteger bigInteger4 = bigInteger2.Multiply(bigInteger3).Mod(parameters.P).Mod(parameters.Q);
			return bigInteger4.Equals(r);
		}
	}
}
