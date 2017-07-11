using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class DesEdeWrapEngine : IWrapper
	{
		private CbcBlockCipher engine;

		private KeyParameter param;

		private ParametersWithIV paramPlusIV;

		private byte[] iv;

		private bool forWrapping;

		private static readonly byte[] IV2 = new byte[]
		{
			74,
			221,
			162,
			44,
			121,
			232,
			33,
			5
		};

		private readonly IDigest sha1 = new Sha1Digest();

		private readonly byte[] digest = new byte[20];

		public virtual string AlgorithmName
		{
			get
			{
				return "DESede";
			}
		}

		public virtual void Init(bool forWrapping, ICipherParameters parameters)
		{
			this.forWrapping = forWrapping;
			this.engine = new CbcBlockCipher(new DesEdeEngine());
			SecureRandom secureRandom;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				parameters = parametersWithRandom.Parameters;
				secureRandom = parametersWithRandom.Random;
			}
			else
			{
				secureRandom = new SecureRandom();
			}
			if (parameters is KeyParameter)
			{
				this.param = (KeyParameter)parameters;
				if (this.forWrapping)
				{
					this.iv = new byte[8];
					secureRandom.NextBytes(this.iv);
					this.paramPlusIV = new ParametersWithIV(this.param, this.iv);
					return;
				}
			}
			else if (parameters is ParametersWithIV)
			{
				if (!forWrapping)
				{
					throw new ArgumentException("You should not supply an IV for unwrapping");
				}
				this.paramPlusIV = (ParametersWithIV)parameters;
				this.iv = this.paramPlusIV.GetIV();
				this.param = (KeyParameter)this.paramPlusIV.Parameters;
				if (this.iv.Length != 8)
				{
					throw new ArgumentException("IV is not 8 octets", "parameters");
				}
			}
		}

		public virtual byte[] Wrap(byte[] input, int inOff, int length)
		{
			if (!this.forWrapping)
			{
				throw new InvalidOperationException("Not initialized for wrapping");
			}
			byte[] array = new byte[length];
			Array.Copy(input, inOff, array, 0, length);
			byte[] array2 = this.CalculateCmsKeyChecksum(array);
			byte[] array3 = new byte[array.Length + array2.Length];
			Array.Copy(array, 0, array3, 0, array.Length);
			Array.Copy(array2, 0, array3, array.Length, array2.Length);
			int blockSize = this.engine.GetBlockSize();
			if (array3.Length % blockSize != 0)
			{
				throw new InvalidOperationException("Not multiple of block length");
			}
			this.engine.Init(true, this.paramPlusIV);
			byte[] array4 = new byte[array3.Length];
			for (int num = 0; num != array3.Length; num += blockSize)
			{
				this.engine.ProcessBlock(array3, num, array4, num);
			}
			byte[] array5 = new byte[this.iv.Length + array4.Length];
			Array.Copy(this.iv, 0, array5, 0, this.iv.Length);
			Array.Copy(array4, 0, array5, this.iv.Length, array4.Length);
			byte[] array6 = DesEdeWrapEngine.reverse(array5);
			ParametersWithIV parameters = new ParametersWithIV(this.param, DesEdeWrapEngine.IV2);
			this.engine.Init(true, parameters);
			for (int num2 = 0; num2 != array6.Length; num2 += blockSize)
			{
				this.engine.ProcessBlock(array6, num2, array6, num2);
			}
			return array6;
		}

		public virtual byte[] Unwrap(byte[] input, int inOff, int length)
		{
			if (this.forWrapping)
			{
				throw new InvalidOperationException("Not set for unwrapping");
			}
			if (input == null)
			{
				throw new InvalidCipherTextException("Null pointer as ciphertext");
			}
			int blockSize = this.engine.GetBlockSize();
			if (length % blockSize != 0)
			{
				throw new InvalidCipherTextException("Ciphertext not multiple of " + blockSize);
			}
			ParametersWithIV parameters = new ParametersWithIV(this.param, DesEdeWrapEngine.IV2);
			this.engine.Init(false, parameters);
			byte[] array = new byte[length];
			for (int num = 0; num != array.Length; num += blockSize)
			{
				this.engine.ProcessBlock(input, inOff + num, array, num);
			}
			byte[] array2 = DesEdeWrapEngine.reverse(array);
			this.iv = new byte[8];
			byte[] array3 = new byte[array2.Length - 8];
			Array.Copy(array2, 0, this.iv, 0, 8);
			Array.Copy(array2, 8, array3, 0, array2.Length - 8);
			this.paramPlusIV = new ParametersWithIV(this.param, this.iv);
			this.engine.Init(false, this.paramPlusIV);
			byte[] array4 = new byte[array3.Length];
			for (int num2 = 0; num2 != array4.Length; num2 += blockSize)
			{
				this.engine.ProcessBlock(array3, num2, array4, num2);
			}
			byte[] array5 = new byte[array4.Length - 8];
			byte[] array6 = new byte[8];
			Array.Copy(array4, 0, array5, 0, array4.Length - 8);
			Array.Copy(array4, array4.Length - 8, array6, 0, 8);
			if (!this.CheckCmsKeyChecksum(array5, array6))
			{
				throw new InvalidCipherTextException("Checksum inside ciphertext is corrupted");
			}
			return array5;
		}

		private byte[] CalculateCmsKeyChecksum(byte[] key)
		{
			this.sha1.BlockUpdate(key, 0, key.Length);
			this.sha1.DoFinal(this.digest, 0);
			byte[] array = new byte[8];
			Array.Copy(this.digest, 0, array, 0, 8);
			return array;
		}

		private bool CheckCmsKeyChecksum(byte[] key, byte[] checksum)
		{
			return Arrays.ConstantTimeAreEqual(this.CalculateCmsKeyChecksum(key), checksum);
		}

		private static byte[] reverse(byte[] bs)
		{
			byte[] array = new byte[bs.Length];
			for (int i = 0; i < bs.Length; i++)
			{
				array[i] = bs[bs.Length - (i + 1)];
			}
			return array;
		}
	}
}
