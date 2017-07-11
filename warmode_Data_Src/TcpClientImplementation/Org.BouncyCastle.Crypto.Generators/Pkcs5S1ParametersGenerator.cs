using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Pkcs5S1ParametersGenerator : PbeParametersGenerator
	{
		private readonly IDigest digest;

		public Pkcs5S1ParametersGenerator(IDigest digest)
		{
			this.digest = digest;
		}

		private byte[] GenerateDerivedKey()
		{
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.BlockUpdate(this.mPassword, 0, this.mPassword.Length);
			this.digest.BlockUpdate(this.mSalt, 0, this.mSalt.Length);
			this.digest.DoFinal(array, 0);
			for (int i = 1; i < this.mIterationCount; i++)
			{
				this.digest.BlockUpdate(array, 0, array.Length);
				this.digest.DoFinal(array, 0);
			}
			return array;
		}

		[Obsolete("Use version with 'algorithm' parameter")]
		public override ICipherParameters GenerateDerivedParameters(int keySize)
		{
			return this.GenerateDerivedMacParameters(keySize);
		}

		public override ICipherParameters GenerateDerivedParameters(string algorithm, int keySize)
		{
			keySize /= 8;
			if (keySize > this.digest.GetDigestSize())
			{
				throw new ArgumentException("Can't Generate a derived key " + keySize + " bytes long.");
			}
			byte[] keyBytes = this.GenerateDerivedKey();
			return ParameterUtilities.CreateKeyParameter(algorithm, keyBytes, 0, keySize);
		}

		[Obsolete("Use version with 'algorithm' parameter")]
		public override ICipherParameters GenerateDerivedParameters(int keySize, int ivSize)
		{
			keySize /= 8;
			ivSize /= 8;
			if (keySize + ivSize > this.digest.GetDigestSize())
			{
				throw new ArgumentException("Can't Generate a derived key " + (keySize + ivSize) + " bytes long.");
			}
			byte[] array = this.GenerateDerivedKey();
			return new ParametersWithIV(new KeyParameter(array, 0, keySize), array, keySize, ivSize);
		}

		public override ICipherParameters GenerateDerivedParameters(string algorithm, int keySize, int ivSize)
		{
			keySize /= 8;
			ivSize /= 8;
			if (keySize + ivSize > this.digest.GetDigestSize())
			{
				throw new ArgumentException("Can't Generate a derived key " + (keySize + ivSize) + " bytes long.");
			}
			byte[] array = this.GenerateDerivedKey();
			KeyParameter parameters = ParameterUtilities.CreateKeyParameter(algorithm, array, 0, keySize);
			return new ParametersWithIV(parameters, array, keySize, ivSize);
		}

		public override ICipherParameters GenerateDerivedMacParameters(int keySize)
		{
			keySize /= 8;
			if (keySize > this.digest.GetDigestSize())
			{
				throw new ArgumentException("Can't Generate a derived key " + keySize + " bytes long.");
			}
			byte[] key = this.GenerateDerivedKey();
			return new KeyParameter(key, 0, keySize);
		}
	}
}
