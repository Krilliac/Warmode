using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto.Encodings
{
	public class ISO9796d1Encoding : IAsymmetricBlockCipher
	{
		private static readonly BigInteger Sixteen = BigInteger.ValueOf(16L);

		private static readonly BigInteger Six = BigInteger.ValueOf(6L);

		private static readonly byte[] shadows = new byte[]
		{
			14,
			3,
			5,
			8,
			9,
			4,
			2,
			15,
			0,
			13,
			11,
			6,
			7,
			10,
			12,
			1
		};

		private static readonly byte[] inverse = new byte[]
		{
			8,
			15,
			6,
			1,
			5,
			2,
			11,
			12,
			3,
			4,
			13,
			10,
			14,
			9,
			0,
			7
		};

		private readonly IAsymmetricBlockCipher engine;

		private bool forEncryption;

		private int bitSize;

		private int padBits;

		private BigInteger modulus;

		public string AlgorithmName
		{
			get
			{
				return this.engine.AlgorithmName + "/ISO9796-1Padding";
			}
		}

		public ISO9796d1Encoding(IAsymmetricBlockCipher cipher)
		{
			this.engine = cipher;
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return this.engine;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			RsaKeyParameters rsaKeyParameters;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				rsaKeyParameters = (RsaKeyParameters)parametersWithRandom.Parameters;
			}
			else
			{
				rsaKeyParameters = (RsaKeyParameters)parameters;
			}
			this.engine.Init(forEncryption, parameters);
			this.modulus = rsaKeyParameters.Modulus;
			this.bitSize = this.modulus.BitLength;
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = this.engine.GetInputBlockSize();
			if (this.forEncryption)
			{
				return (inputBlockSize + 1) / 2;
			}
			return inputBlockSize;
		}

		public int GetOutputBlockSize()
		{
			int outputBlockSize = this.engine.GetOutputBlockSize();
			if (this.forEncryption)
			{
				return outputBlockSize;
			}
			return (outputBlockSize + 1) / 2;
		}

		public void SetPadBits(int padBits)
		{
			if (padBits > 7)
			{
				throw new ArgumentException("padBits > 7");
			}
			this.padBits = padBits;
		}

		public int GetPadBits()
		{
			return this.padBits;
		}

		public byte[] ProcessBlock(byte[] input, int inOff, int length)
		{
			if (this.forEncryption)
			{
				return this.EncodeBlock(input, inOff, length);
			}
			return this.DecodeBlock(input, inOff, length);
		}

		private byte[] EncodeBlock(byte[] input, int inOff, int inLen)
		{
			byte[] array = new byte[(this.bitSize + 7) / 8];
			int num = this.padBits + 1;
			int num2 = (this.bitSize + 13) / 16;
			for (int i = 0; i < num2; i += inLen)
			{
				if (i > num2 - inLen)
				{
					Array.Copy(input, inOff + inLen - (num2 - i), array, array.Length - num2, num2 - i);
				}
				else
				{
					Array.Copy(input, inOff, array, array.Length - (i + inLen), inLen);
				}
			}
			for (int num3 = array.Length - 2 * num2; num3 != array.Length; num3 += 2)
			{
				byte b = array[array.Length - num2 + num3 / 2];
				array[num3] = (byte)((int)ISO9796d1Encoding.shadows[(int)((UIntPtr)((uint)(b & 255) >> 4))] << 4 | (int)ISO9796d1Encoding.shadows[(int)(b & 15)]);
				array[num3 + 1] = b;
			}
			byte[] expr_C9_cp_0 = array;
			int expr_C9_cp_1 = array.Length - 2 * inLen;
			expr_C9_cp_0[expr_C9_cp_1] ^= (byte)num;
			array[array.Length - 1] = (byte)((int)array[array.Length - 1] << 4 | 6);
			int num4 = 8 - (this.bitSize - 1) % 8;
			int num5 = 0;
			if (num4 != 8)
			{
				byte[] expr_108_cp_0 = array;
				int expr_108_cp_1 = 0;
				expr_108_cp_0[expr_108_cp_1] &= (byte)(255 >> num4);
				byte[] expr_128_cp_0 = array;
				int expr_128_cp_1 = 0;
				expr_128_cp_0[expr_128_cp_1] |= (byte)(128 >> num4);
			}
			else
			{
				array[0] = 0;
				byte[] expr_14E_cp_0 = array;
				int expr_14E_cp_1 = 1;
				expr_14E_cp_0[expr_14E_cp_1] |= 128;
				num5 = 1;
			}
			return this.engine.ProcessBlock(array, num5, array.Length - num5);
		}

		private byte[] DecodeBlock(byte[] input, int inOff, int inLen)
		{
			byte[] array = this.engine.ProcessBlock(input, inOff, inLen);
			int num = 1;
			int num2 = (this.bitSize + 13) / 16;
			BigInteger bigInteger = new BigInteger(1, array);
			BigInteger bigInteger2;
			if (bigInteger.Mod(ISO9796d1Encoding.Sixteen).Equals(ISO9796d1Encoding.Six))
			{
				bigInteger2 = bigInteger;
			}
			else
			{
				bigInteger2 = this.modulus.Subtract(bigInteger);
				if (!bigInteger2.Mod(ISO9796d1Encoding.Sixteen).Equals(ISO9796d1Encoding.Six))
				{
					throw new InvalidCipherTextException("resulting integer iS or (modulus - iS) is not congruent to 6 mod 16");
				}
			}
			array = bigInteger2.ToByteArrayUnsigned();
			if ((array[array.Length - 1] & 15) != 6)
			{
				throw new InvalidCipherTextException("invalid forcing byte in block");
			}
			array[array.Length - 1] = (byte)((ushort)(array[array.Length - 1] & 255) >> 4 | (int)ISO9796d1Encoding.inverse[(array[array.Length - 2] & 255) >> 4] << 4);
			array[0] = (byte)((int)ISO9796d1Encoding.shadows[(int)((UIntPtr)((uint)(array[1] & 255) >> 4))] << 4 | (int)ISO9796d1Encoding.shadows[(int)(array[1] & 15)]);
			bool flag = false;
			int num3 = 0;
			for (int i = array.Length - 1; i >= array.Length - 2 * num2; i -= 2)
			{
				int num4 = (int)ISO9796d1Encoding.shadows[(int)((UIntPtr)((uint)(array[i] & 255) >> 4))] << 4 | (int)ISO9796d1Encoding.shadows[(int)(array[i] & 15)];
				if ((((int)array[i - 1] ^ num4) & 255) != 0)
				{
					if (flag)
					{
						throw new InvalidCipherTextException("invalid tsums in block");
					}
					flag = true;
					num = (((int)array[i - 1] ^ num4) & 255);
					num3 = i - 1;
				}
			}
			array[num3] = 0;
			byte[] array2 = new byte[(array.Length - num3) / 2];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = array[2 * j + num3 + 1];
			}
			this.padBits = num - 1;
			return array2;
		}
	}
}
