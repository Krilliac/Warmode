using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RC2WrapEngine : IWrapper
	{
		private CbcBlockCipher engine;

		private ICipherParameters parameters;

		private ParametersWithIV paramPlusIV;

		private byte[] iv;

		private bool forWrapping;

		private SecureRandom sr;

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

		private IDigest sha1 = new Sha1Digest();

		private byte[] digest = new byte[20];

		public virtual string AlgorithmName
		{
			get
			{
				return "RC2";
			}
		}

		public virtual void Init(bool forWrapping, ICipherParameters parameters)
		{
			this.forWrapping = forWrapping;
			this.engine = new CbcBlockCipher(new RC2Engine());
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				this.sr = parametersWithRandom.Random;
				parameters = parametersWithRandom.Parameters;
			}
			else
			{
				this.sr = new SecureRandom();
			}
			if (parameters is ParametersWithIV)
			{
				if (!forWrapping)
				{
					throw new ArgumentException("You should not supply an IV for unwrapping");
				}
				this.paramPlusIV = (ParametersWithIV)parameters;
				this.iv = this.paramPlusIV.GetIV();
				this.parameters = this.paramPlusIV.Parameters;
				if (this.iv.Length != 8)
				{
					throw new ArgumentException("IV is not 8 octets");
				}
			}
			else
			{
				this.parameters = parameters;
				if (this.forWrapping)
				{
					this.iv = new byte[8];
					this.sr.NextBytes(this.iv);
					this.paramPlusIV = new ParametersWithIV(this.parameters, this.iv);
				}
			}
		}

		public virtual byte[] Wrap(byte[] input, int inOff, int length)
		{
			if (!this.forWrapping)
			{
				throw new InvalidOperationException("Not initialized for wrapping");
			}
			int num = length + 1;
			if (num % 8 != 0)
			{
				num += 8 - num % 8;
			}
			byte[] array = new byte[num];
			array[0] = (byte)length;
			Array.Copy(input, inOff, array, 1, length);
			byte[] array2 = new byte[array.Length - length - 1];
			if (array2.Length > 0)
			{
				this.sr.NextBytes(array2);
				Array.Copy(array2, 0, array, length + 1, array2.Length);
			}
			byte[] array3 = this.CalculateCmsKeyChecksum(array);
			byte[] array4 = new byte[array.Length + array3.Length];
			Array.Copy(array, 0, array4, 0, array.Length);
			Array.Copy(array3, 0, array4, array.Length, array3.Length);
			byte[] array5 = new byte[array4.Length];
			Array.Copy(array4, 0, array5, 0, array4.Length);
			int num2 = array4.Length / this.engine.GetBlockSize();
			int num3 = array4.Length % this.engine.GetBlockSize();
			if (num3 != 0)
			{
				throw new InvalidOperationException("Not multiple of block length");
			}
			this.engine.Init(true, this.paramPlusIV);
			for (int i = 0; i < num2; i++)
			{
				int num4 = i * this.engine.GetBlockSize();
				this.engine.ProcessBlock(array5, num4, array5, num4);
			}
			byte[] array6 = new byte[this.iv.Length + array5.Length];
			Array.Copy(this.iv, 0, array6, 0, this.iv.Length);
			Array.Copy(array5, 0, array6, this.iv.Length, array5.Length);
			byte[] array7 = new byte[array6.Length];
			for (int j = 0; j < array6.Length; j++)
			{
				array7[j] = array6[array6.Length - (j + 1)];
			}
			ParametersWithIV parametersWithIV = new ParametersWithIV(this.parameters, RC2WrapEngine.IV2);
			this.engine.Init(true, parametersWithIV);
			for (int k = 0; k < num2 + 1; k++)
			{
				int num5 = k * this.engine.GetBlockSize();
				this.engine.ProcessBlock(array7, num5, array7, num5);
			}
			return array7;
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
			if (length % this.engine.GetBlockSize() != 0)
			{
				throw new InvalidCipherTextException("Ciphertext not multiple of " + this.engine.GetBlockSize());
			}
			ParametersWithIV parametersWithIV = new ParametersWithIV(this.parameters, RC2WrapEngine.IV2);
			this.engine.Init(false, parametersWithIV);
			byte[] array = new byte[length];
			Array.Copy(input, inOff, array, 0, length);
			for (int i = 0; i < array.Length / this.engine.GetBlockSize(); i++)
			{
				int num = i * this.engine.GetBlockSize();
				this.engine.ProcessBlock(array, num, array, num);
			}
			byte[] array2 = new byte[array.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array2[j] = array[array.Length - (j + 1)];
			}
			this.iv = new byte[8];
			byte[] array3 = new byte[array2.Length - 8];
			Array.Copy(array2, 0, this.iv, 0, 8);
			Array.Copy(array2, 8, array3, 0, array2.Length - 8);
			this.paramPlusIV = new ParametersWithIV(this.parameters, this.iv);
			this.engine.Init(false, this.paramPlusIV);
			byte[] array4 = new byte[array3.Length];
			Array.Copy(array3, 0, array4, 0, array3.Length);
			for (int k = 0; k < array4.Length / this.engine.GetBlockSize(); k++)
			{
				int num2 = k * this.engine.GetBlockSize();
				this.engine.ProcessBlock(array4, num2, array4, num2);
			}
			byte[] array5 = new byte[array4.Length - 8];
			byte[] array6 = new byte[8];
			Array.Copy(array4, 0, array5, 0, array4.Length - 8);
			Array.Copy(array4, array4.Length - 8, array6, 0, 8);
			if (!this.CheckCmsKeyChecksum(array5, array6))
			{
				throw new InvalidCipherTextException("Checksum inside ciphertext is corrupted");
			}
			if (array5.Length - (int)((array5[0] & 255) + 1) > 7)
			{
				throw new InvalidCipherTextException("too many pad bytes (" + (array5.Length - (int)((array5[0] & 255) + 1)) + ")");
			}
			byte[] array7 = new byte[(int)array5[0]];
			Array.Copy(array5, 1, array7, 0, array7.Length);
			return array7;
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
	}
}
