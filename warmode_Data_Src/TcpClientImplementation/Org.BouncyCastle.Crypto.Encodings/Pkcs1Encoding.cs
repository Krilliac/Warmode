using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Encodings
{
	public class Pkcs1Encoding : IAsymmetricBlockCipher
	{
		public const string StrictLengthEnabledProperty = "Org.BouncyCastle.Pkcs1.Strict";

		private const int HeaderLength = 10;

		private static readonly bool[] strictLengthEnabled;

		private SecureRandom random;

		private IAsymmetricBlockCipher engine;

		private bool forEncryption;

		private bool forPrivateKey;

		private bool useStrictLength;

		private int pLen = -1;

		private byte[] fallback;

		public static bool StrictLengthEnabled
		{
			get
			{
				return Pkcs1Encoding.strictLengthEnabled[0];
			}
			set
			{
				Pkcs1Encoding.strictLengthEnabled[0] = value;
			}
		}

		public string AlgorithmName
		{
			get
			{
				return this.engine.AlgorithmName + "/PKCS1Padding";
			}
		}

		static Pkcs1Encoding()
		{
			string environmentVariable = Platform.GetEnvironmentVariable("Org.BouncyCastle.Pkcs1.Strict");
			Pkcs1Encoding.strictLengthEnabled = new bool[]
			{
				environmentVariable == null || environmentVariable.Equals("true")
			};
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher)
		{
			this.engine = cipher;
			this.useStrictLength = Pkcs1Encoding.StrictLengthEnabled;
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher, int pLen)
		{
			this.engine = cipher;
			this.useStrictLength = Pkcs1Encoding.StrictLengthEnabled;
			this.pLen = pLen;
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher, byte[] fallback)
		{
			this.engine = cipher;
			this.useStrictLength = Pkcs1Encoding.StrictLengthEnabled;
			this.fallback = fallback;
			this.pLen = fallback.Length;
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return this.engine;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				this.random = parametersWithRandom.Random;
				asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
			}
			else
			{
				this.random = new SecureRandom();
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			this.engine.Init(forEncryption, parameters);
			this.forPrivateKey = asymmetricKeyParameter.IsPrivate;
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = this.engine.GetInputBlockSize();
			if (!this.forEncryption)
			{
				return inputBlockSize;
			}
			return inputBlockSize - 10;
		}

		public int GetOutputBlockSize()
		{
			int outputBlockSize = this.engine.GetOutputBlockSize();
			if (!this.forEncryption)
			{
				return outputBlockSize - 10;
			}
			return outputBlockSize;
		}

		public byte[] ProcessBlock(byte[] input, int inOff, int length)
		{
			if (!this.forEncryption)
			{
				return this.DecodeBlock(input, inOff, length);
			}
			return this.EncodeBlock(input, inOff, length);
		}

		private byte[] EncodeBlock(byte[] input, int inOff, int inLen)
		{
			if (inLen > this.GetInputBlockSize())
			{
				throw new ArgumentException("input data too large", "inLen");
			}
			byte[] array = new byte[this.engine.GetInputBlockSize()];
			if (this.forPrivateKey)
			{
				array[0] = 1;
				for (int num = 1; num != array.Length - inLen - 1; num++)
				{
					array[num] = 255;
				}
			}
			else
			{
				this.random.NextBytes(array);
				array[0] = 2;
				for (int num2 = 1; num2 != array.Length - inLen - 1; num2++)
				{
					while (array[num2] == 0)
					{
						array[num2] = (byte)this.random.NextInt();
					}
				}
			}
			array[array.Length - inLen - 1] = 0;
			Array.Copy(input, inOff, array, array.Length - inLen, inLen);
			return this.engine.ProcessBlock(array, 0, array.Length);
		}

		private static int CheckPkcs1Encoding(byte[] encoded, int pLen)
		{
			int num = 0;
			num |= (int)(encoded[0] ^ 2);
			int num2 = encoded.Length - (pLen + 1);
			for (int i = 1; i < num2; i++)
			{
				int num3 = (int)encoded[i];
				num3 |= num3 >> 1;
				num3 |= num3 >> 2;
				num3 |= num3 >> 4;
				num |= (num3 & 1) - 1;
			}
			num |= (int)encoded[encoded.Length - (pLen + 1)];
			num |= num >> 1;
			num |= num >> 2;
			num |= num >> 4;
			return ~((num & 1) - 1);
		}

		private byte[] DecodeBlockOrRandom(byte[] input, int inOff, int inLen)
		{
			if (!this.forPrivateKey)
			{
				throw new InvalidCipherTextException("sorry, this method is only for decryption, not for signing");
			}
			byte[] array = this.engine.ProcessBlock(input, inOff, inLen);
			byte[] array2;
			if (this.fallback == null)
			{
				array2 = new byte[this.pLen];
				this.random.NextBytes(array2);
			}
			else
			{
				array2 = this.fallback;
			}
			if (array.Length < this.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}
			if (this.useStrictLength && array.Length != this.engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			int num = Pkcs1Encoding.CheckPkcs1Encoding(array, this.pLen);
			byte[] array3 = new byte[this.pLen];
			for (int i = 0; i < this.pLen; i++)
			{
				array3[i] = (byte)(((int)array[i + (array.Length - this.pLen)] & ~num) | ((int)array2[i] & num));
			}
			return array3;
		}

		private byte[] DecodeBlock(byte[] input, int inOff, int inLen)
		{
			if (this.pLen != -1)
			{
				return this.DecodeBlockOrRandom(input, inOff, inLen);
			}
			byte[] array = this.engine.ProcessBlock(input, inOff, inLen);
			if (array.Length < this.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}
			byte b = array[0];
			if (b != 1 && b != 2)
			{
				throw new InvalidCipherTextException("unknown block type");
			}
			if (this.useStrictLength && array.Length != this.engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			int num;
			for (num = 1; num != array.Length; num++)
			{
				byte b2 = array[num];
				if (b2 == 0)
				{
					break;
				}
				if (b == 1 && b2 != 255)
				{
					throw new InvalidCipherTextException("block padding incorrect");
				}
			}
			num++;
			if (num > array.Length || num < 10)
			{
				throw new InvalidCipherTextException("no data in block");
			}
			byte[] array2 = new byte[array.Length - num];
			Array.Copy(array, num, array2, 0, array2.Length);
			return array2;
		}
	}
}
