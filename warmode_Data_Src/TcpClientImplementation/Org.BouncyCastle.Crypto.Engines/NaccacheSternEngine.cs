using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class NaccacheSternEngine : IAsymmetricBlockCipher
	{
		private bool forEncryption;

		private NaccacheSternKeyParameters key;

		private IList[] lookup;

		private bool debug;

		public string AlgorithmName
		{
			get
			{
				return "NaccacheStern";
			}
		}

		public virtual bool Debug
		{
			set
			{
				this.debug = value;
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			this.forEncryption = forEncryption;
			if (parameters is ParametersWithRandom)
			{
				parameters = ((ParametersWithRandom)parameters).Parameters;
			}
			this.key = (NaccacheSternKeyParameters)parameters;
			if (!this.forEncryption)
			{
				if (this.debug)
				{
					Console.WriteLine("Constructing lookup Array");
				}
				NaccacheSternPrivateKeyParameters naccacheSternPrivateKeyParameters = (NaccacheSternPrivateKeyParameters)this.key;
				IList smallPrimesList = naccacheSternPrivateKeyParameters.SmallPrimesList;
				this.lookup = new IList[smallPrimesList.Count];
				for (int i = 0; i < smallPrimesList.Count; i++)
				{
					BigInteger bigInteger = (BigInteger)smallPrimesList[i];
					int intValue = bigInteger.IntValue;
					this.lookup[i] = Platform.CreateArrayList(intValue);
					this.lookup[i].Add(BigInteger.One);
					if (this.debug)
					{
						Console.WriteLine("Constructing lookup ArrayList for " + intValue);
					}
					BigInteger bigInteger2 = BigInteger.Zero;
					for (int j = 1; j < intValue; j++)
					{
						bigInteger2 = bigInteger2.Add(naccacheSternPrivateKeyParameters.PhiN);
						BigInteger e = bigInteger2.Divide(bigInteger);
						this.lookup[i].Add(naccacheSternPrivateKeyParameters.G.ModPow(e, naccacheSternPrivateKeyParameters.Modulus));
					}
				}
			}
		}

		public virtual int GetInputBlockSize()
		{
			if (this.forEncryption)
			{
				return (this.key.LowerSigmaBound + 7) / 8 - 1;
			}
			return this.key.Modulus.BitLength / 8 + 1;
		}

		public virtual int GetOutputBlockSize()
		{
			if (this.forEncryption)
			{
				return this.key.Modulus.BitLength / 8 + 1;
			}
			return (this.key.LowerSigmaBound + 7) / 8 - 1;
		}

		public virtual byte[] ProcessBlock(byte[] inBytes, int inOff, int length)
		{
			if (this.key == null)
			{
				throw new InvalidOperationException("NaccacheStern engine not initialised");
			}
			if (length > this.GetInputBlockSize() + 1)
			{
				throw new DataLengthException("input too large for Naccache-Stern cipher.\n");
			}
			if (!this.forEncryption && length < this.GetInputBlockSize())
			{
				throw new InvalidCipherTextException("BlockLength does not match modulus for Naccache-Stern cipher.\n");
			}
			BigInteger bigInteger = new BigInteger(1, inBytes, inOff, length);
			if (this.debug)
			{
				Console.WriteLine("input as BigInteger: " + bigInteger);
			}
			byte[] result;
			if (this.forEncryption)
			{
				result = this.Encrypt(bigInteger);
			}
			else
			{
				IList list = Platform.CreateArrayList();
				NaccacheSternPrivateKeyParameters naccacheSternPrivateKeyParameters = (NaccacheSternPrivateKeyParameters)this.key;
				IList smallPrimesList = naccacheSternPrivateKeyParameters.SmallPrimesList;
				for (int i = 0; i < smallPrimesList.Count; i++)
				{
					BigInteger bigInteger2 = bigInteger.ModPow(naccacheSternPrivateKeyParameters.PhiN.Divide((BigInteger)smallPrimesList[i]), naccacheSternPrivateKeyParameters.Modulus);
					IList list2 = this.lookup[i];
					if (this.lookup[i].Count != ((BigInteger)smallPrimesList[i]).IntValue)
					{
						if (this.debug)
						{
							Console.WriteLine(string.Concat(new object[]
							{
								"Prime is ",
								smallPrimesList[i],
								", lookup table has size ",
								list2.Count
							}));
						}
						throw new InvalidCipherTextException(string.Concat(new object[]
						{
							"Error in lookup Array for ",
							((BigInteger)smallPrimesList[i]).IntValue,
							": Size mismatch. Expected ArrayList with length ",
							((BigInteger)smallPrimesList[i]).IntValue,
							" but found ArrayList of length ",
							this.lookup[i].Count
						}));
					}
					int num = list2.IndexOf(bigInteger2);
					if (num == -1)
					{
						if (this.debug)
						{
							Console.WriteLine("Actual prime is " + smallPrimesList[i]);
							Console.WriteLine("Decrypted value is " + bigInteger2);
							Console.WriteLine(string.Concat(new object[]
							{
								"LookupList for ",
								smallPrimesList[i],
								" with size ",
								this.lookup[i].Count,
								" is: "
							}));
							for (int j = 0; j < this.lookup[i].Count; j++)
							{
								Console.WriteLine(this.lookup[i][j]);
							}
						}
						throw new InvalidCipherTextException("Lookup failed");
					}
					list.Add(BigInteger.ValueOf((long)num));
				}
				BigInteger bigInteger3 = NaccacheSternEngine.chineseRemainder(list, smallPrimesList);
				result = bigInteger3.ToByteArray();
			}
			return result;
		}

		public virtual byte[] Encrypt(BigInteger plain)
		{
			byte[] array = new byte[this.key.Modulus.BitLength / 8 + 1];
			byte[] array2 = this.key.G.ModPow(plain, this.key.Modulus).ToByteArray();
			Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
			if (this.debug)
			{
				Console.WriteLine("Encrypted value is:  " + new BigInteger(array));
			}
			return array;
		}

		public virtual byte[] AddCryptedBlocks(byte[] block1, byte[] block2)
		{
			if (this.forEncryption)
			{
				if (block1.Length > this.GetOutputBlockSize() || block2.Length > this.GetOutputBlockSize())
				{
					throw new InvalidCipherTextException("BlockLength too large for simple addition.\n");
				}
			}
			else if (block1.Length > this.GetInputBlockSize() || block2.Length > this.GetInputBlockSize())
			{
				throw new InvalidCipherTextException("BlockLength too large for simple addition.\n");
			}
			BigInteger bigInteger = new BigInteger(1, block1);
			BigInteger bigInteger2 = new BigInteger(1, block2);
			BigInteger bigInteger3 = bigInteger.Multiply(bigInteger2);
			bigInteger3 = bigInteger3.Mod(this.key.Modulus);
			if (this.debug)
			{
				Console.WriteLine("c(m1) as BigInteger:....... " + bigInteger);
				Console.WriteLine("c(m2) as BigInteger:....... " + bigInteger2);
				Console.WriteLine("c(m1)*c(m2)%n = c(m1+m2)%n: " + bigInteger3);
			}
			byte[] array = new byte[this.key.Modulus.BitLength / 8 + 1];
			byte[] array2 = bigInteger3.ToByteArray();
			Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
			return array;
		}

		public virtual byte[] ProcessData(byte[] data)
		{
			if (this.debug)
			{
				Console.WriteLine();
			}
			if (data.Length > this.GetInputBlockSize())
			{
				int inputBlockSize = this.GetInputBlockSize();
				int outputBlockSize = this.GetOutputBlockSize();
				if (this.debug)
				{
					Console.WriteLine("Input blocksize is:  " + inputBlockSize + " bytes");
					Console.WriteLine("Output blocksize is: " + outputBlockSize + " bytes");
					Console.WriteLine("Data has length:.... " + data.Length + " bytes");
				}
				int i = 0;
				int num = 0;
				byte[] array = new byte[(data.Length / inputBlockSize + 1) * outputBlockSize];
				while (i < data.Length)
				{
					byte[] array2;
					if (i + inputBlockSize < data.Length)
					{
						array2 = this.ProcessBlock(data, i, inputBlockSize);
						i += inputBlockSize;
					}
					else
					{
						array2 = this.ProcessBlock(data, i, data.Length - i);
						i += data.Length - i;
					}
					if (this.debug)
					{
						Console.WriteLine("new datapos is " + i);
					}
					if (array2 == null)
					{
						if (this.debug)
						{
							Console.WriteLine("cipher returned null");
						}
						throw new InvalidCipherTextException("cipher returned null");
					}
					array2.CopyTo(array, num);
					num += array2.Length;
				}
				byte[] array3 = new byte[num];
				Array.Copy(array, 0, array3, 0, num);
				if (this.debug)
				{
					Console.WriteLine("returning " + array3.Length + " bytes");
				}
				return array3;
			}
			if (this.debug)
			{
				Console.WriteLine("data size is less then input block size, processing directly");
			}
			return this.ProcessBlock(data, 0, data.Length);
		}

		private static BigInteger chineseRemainder(IList congruences, IList primes)
		{
			BigInteger bigInteger = BigInteger.Zero;
			BigInteger bigInteger2 = BigInteger.One;
			for (int i = 0; i < primes.Count; i++)
			{
				bigInteger2 = bigInteger2.Multiply((BigInteger)primes[i]);
			}
			for (int j = 0; j < primes.Count; j++)
			{
				BigInteger bigInteger3 = (BigInteger)primes[j];
				BigInteger bigInteger4 = bigInteger2.Divide(bigInteger3);
				BigInteger val = bigInteger4.ModInverse(bigInteger3);
				BigInteger bigInteger5 = bigInteger4.Multiply(val);
				bigInteger5 = bigInteger5.Multiply((BigInteger)congruences[j]);
				bigInteger = bigInteger.Add(bigInteger5);
			}
			return bigInteger.Mod(bigInteger2);
		}
	}
}
