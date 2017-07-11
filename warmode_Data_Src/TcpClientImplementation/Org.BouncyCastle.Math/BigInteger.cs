using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Org.BouncyCastle.Math
{
	[Serializable]
	public class BigInteger
	{
		private const long IMASK = 4294967295L;

		private const ulong UIMASK = 4294967295uL;

		private const int chunk2 = 1;

		private const int chunk8 = 1;

		private const int chunk10 = 19;

		private const int chunk16 = 16;

		private const int BitsPerByte = 8;

		private const int BitsPerInt = 32;

		private const int BytesPerInt = 4;

		internal static readonly int[][] primeLists;

		internal static readonly int[] primeProducts;

		private static readonly int[] ZeroMagnitude;

		private static readonly byte[] ZeroEncoding;

		private static readonly BigInteger[] SMALL_CONSTANTS;

		public static readonly BigInteger Zero;

		public static readonly BigInteger One;

		public static readonly BigInteger Two;

		public static readonly BigInteger Three;

		public static readonly BigInteger Ten;

		private static readonly byte[] BitLengthTable;

		private static readonly BigInteger radix2;

		private static readonly BigInteger radix2E;

		private static readonly BigInteger radix8;

		private static readonly BigInteger radix8E;

		private static readonly BigInteger radix10;

		private static readonly BigInteger radix10E;

		private static readonly BigInteger radix16;

		private static readonly BigInteger radix16E;

		private static readonly Random RandomSource;

		private static readonly int[] ExpWindowThresholds;

		private int[] magnitude;

		private int sign;

		private int nBits = -1;

		private int nBitLength = -1;

		private int mQuote;

		public int BitCount
		{
			get
			{
				if (this.nBits == -1)
				{
					if (this.sign < 0)
					{
						this.nBits = this.Not().BitCount;
					}
					else
					{
						int num = 0;
						for (int i = 0; i < this.magnitude.Length; i++)
						{
							num += BigInteger.BitCnt(this.magnitude[i]);
						}
						this.nBits = num;
					}
				}
				return this.nBits;
			}
		}

		public int BitLength
		{
			get
			{
				if (this.nBitLength == -1)
				{
					this.nBitLength = ((this.sign == 0) ? 0 : BigInteger.CalcBitLength(this.sign, 0, this.magnitude));
				}
				return this.nBitLength;
			}
		}

		public int IntValue
		{
			get
			{
				if (this.sign == 0)
				{
					return 0;
				}
				int num = this.magnitude.Length;
				int num2 = this.magnitude[num - 1];
				if (this.sign >= 0)
				{
					return num2;
				}
				return -num2;
			}
		}

		public long LongValue
		{
			get
			{
				if (this.sign == 0)
				{
					return 0L;
				}
				int num = this.magnitude.Length;
				long num2 = (long)this.magnitude[num - 1] & (long)((ulong)-1);
				if (num > 1)
				{
					num2 |= ((long)this.magnitude[num - 2] & (long)((ulong)-1)) << 32;
				}
				if (this.sign >= 0)
				{
					return num2;
				}
				return -num2;
			}
		}

		public int SignValue
		{
			get
			{
				return this.sign;
			}
		}

		static BigInteger()
		{
			BigInteger.primeLists = new int[][]
			{
				new int[]
				{
					3,
					5,
					7,
					11,
					13,
					17,
					19,
					23
				},
				new int[]
				{
					29,
					31,
					37,
					41,
					43
				},
				new int[]
				{
					47,
					53,
					59,
					61,
					67
				},
				new int[]
				{
					71,
					73,
					79,
					83
				},
				new int[]
				{
					89,
					97,
					101,
					103
				},
				new int[]
				{
					107,
					109,
					113,
					127
				},
				new int[]
				{
					131,
					137,
					139,
					149
				},
				new int[]
				{
					151,
					157,
					163,
					167
				},
				new int[]
				{
					173,
					179,
					181,
					191
				},
				new int[]
				{
					193,
					197,
					199,
					211
				},
				new int[]
				{
					223,
					227,
					229
				},
				new int[]
				{
					233,
					239,
					241
				},
				new int[]
				{
					251,
					257,
					263
				},
				new int[]
				{
					269,
					271,
					277
				},
				new int[]
				{
					281,
					283,
					293
				},
				new int[]
				{
					307,
					311,
					313
				},
				new int[]
				{
					317,
					331,
					337
				},
				new int[]
				{
					347,
					349,
					353
				},
				new int[]
				{
					359,
					367,
					373
				},
				new int[]
				{
					379,
					383,
					389
				},
				new int[]
				{
					397,
					401,
					409
				},
				new int[]
				{
					419,
					421,
					431
				},
				new int[]
				{
					433,
					439,
					443
				},
				new int[]
				{
					449,
					457,
					461
				},
				new int[]
				{
					463,
					467,
					479
				},
				new int[]
				{
					487,
					491,
					499
				},
				new int[]
				{
					503,
					509,
					521
				},
				new int[]
				{
					523,
					541,
					547
				},
				new int[]
				{
					557,
					563,
					569
				},
				new int[]
				{
					571,
					577,
					587
				},
				new int[]
				{
					593,
					599,
					601
				},
				new int[]
				{
					607,
					613,
					617
				},
				new int[]
				{
					619,
					631,
					641
				},
				new int[]
				{
					643,
					647,
					653
				},
				new int[]
				{
					659,
					661,
					673
				},
				new int[]
				{
					677,
					683,
					691
				},
				new int[]
				{
					701,
					709,
					719
				},
				new int[]
				{
					727,
					733,
					739
				},
				new int[]
				{
					743,
					751,
					757
				},
				new int[]
				{
					761,
					769,
					773
				},
				new int[]
				{
					787,
					797,
					809
				},
				new int[]
				{
					811,
					821,
					823
				},
				new int[]
				{
					827,
					829,
					839
				},
				new int[]
				{
					853,
					857,
					859
				},
				new int[]
				{
					863,
					877,
					881
				},
				new int[]
				{
					883,
					887,
					907
				},
				new int[]
				{
					911,
					919,
					929
				},
				new int[]
				{
					937,
					941,
					947
				},
				new int[]
				{
					953,
					967,
					971
				},
				new int[]
				{
					977,
					983,
					991
				},
				new int[]
				{
					997,
					1009,
					1013
				},
				new int[]
				{
					1019,
					1021,
					1031
				},
				new int[]
				{
					1033,
					1039,
					1049
				},
				new int[]
				{
					1051,
					1061,
					1063
				},
				new int[]
				{
					1069,
					1087,
					1091
				},
				new int[]
				{
					1093,
					1097,
					1103
				},
				new int[]
				{
					1109,
					1117,
					1123
				},
				new int[]
				{
					1129,
					1151,
					1153
				},
				new int[]
				{
					1163,
					1171,
					1181
				},
				new int[]
				{
					1187,
					1193,
					1201
				},
				new int[]
				{
					1213,
					1217,
					1223
				},
				new int[]
				{
					1229,
					1231,
					1237
				},
				new int[]
				{
					1249,
					1259,
					1277
				},
				new int[]
				{
					1279,
					1283,
					1289
				}
			};
			BigInteger.ZeroMagnitude = new int[0];
			BigInteger.ZeroEncoding = new byte[0];
			BigInteger.SMALL_CONSTANTS = new BigInteger[17];
			BigInteger.BitLengthTable = new byte[]
			{
				0,
				1,
				2,
				2,
				3,
				3,
				3,
				3,
				4,
				4,
				4,
				4,
				4,
				4,
				4,
				4,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				5,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				6,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				7,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8,
				8
			};
			BigInteger.RandomSource = new Random();
			BigInteger.ExpWindowThresholds = new int[]
			{
				7,
				25,
				81,
				241,
				673,
				1793,
				4609,
				2147483647
			};
			BigInteger.Zero = new BigInteger(0, BigInteger.ZeroMagnitude, false);
			BigInteger.Zero.nBits = 0;
			BigInteger.Zero.nBitLength = 0;
			BigInteger.SMALL_CONSTANTS[0] = BigInteger.Zero;
			uint num = 1u;
			while ((ulong)num < (ulong)((long)BigInteger.SMALL_CONSTANTS.Length))
			{
				BigInteger.SMALL_CONSTANTS[(int)((UIntPtr)num)] = BigInteger.CreateUValueOf((ulong)num);
				num += 1u;
			}
			BigInteger.One = BigInteger.SMALL_CONSTANTS[1];
			BigInteger.Two = BigInteger.SMALL_CONSTANTS[2];
			BigInteger.Three = BigInteger.SMALL_CONSTANTS[3];
			BigInteger.Ten = BigInteger.SMALL_CONSTANTS[10];
			BigInteger.radix2 = BigInteger.ValueOf(2L);
			BigInteger.radix2E = BigInteger.radix2.Pow(1);
			BigInteger.radix8 = BigInteger.ValueOf(8L);
			BigInteger.radix8E = BigInteger.radix8.Pow(1);
			BigInteger.radix10 = BigInteger.ValueOf(10L);
			BigInteger.radix10E = BigInteger.radix10.Pow(19);
			BigInteger.radix16 = BigInteger.ValueOf(16L);
			BigInteger.radix16E = BigInteger.radix16.Pow(16);
			BigInteger.primeProducts = new int[BigInteger.primeLists.Length];
			for (int i = 0; i < BigInteger.primeLists.Length; i++)
			{
				int[] array = BigInteger.primeLists[i];
				int num2 = array[0];
				for (int j = 1; j < array.Length; j++)
				{
					num2 *= array[j];
				}
				BigInteger.primeProducts[i] = num2;
			}
		}

		private static int GetByteLength(int nBits)
		{
			return (nBits + 8 - 1) / 8;
		}

		private BigInteger(int signum, int[] mag, bool checkMag)
		{
			if (!checkMag)
			{
				this.sign = signum;
				this.magnitude = mag;
				return;
			}
			int num = 0;
			while (num < mag.Length && mag[num] == 0)
			{
				num++;
			}
			if (num == mag.Length)
			{
				this.sign = 0;
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			this.sign = signum;
			if (num == 0)
			{
				this.magnitude = mag;
				return;
			}
			this.magnitude = new int[mag.Length - num];
			Array.Copy(mag, num, this.magnitude, 0, this.magnitude.Length);
		}

		public BigInteger(string value) : this(value, 10)
		{
		}

		public BigInteger(string str, int radix)
		{
			if (str.Length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			NumberStyles style;
			int num;
			BigInteger bigInteger;
			BigInteger val;
			if (radix != 2)
			{
				switch (radix)
				{
				case 8:
					style = NumberStyles.Integer;
					num = 1;
					bigInteger = BigInteger.radix8;
					val = BigInteger.radix8E;
					goto IL_A5;
				case 9:
					break;
				case 10:
					style = NumberStyles.Integer;
					num = 19;
					bigInteger = BigInteger.radix10;
					val = BigInteger.radix10E;
					goto IL_A5;
				default:
					if (radix == 16)
					{
						style = NumberStyles.AllowHexSpecifier;
						num = 16;
						bigInteger = BigInteger.radix16;
						val = BigInteger.radix16E;
						goto IL_A5;
					}
					break;
				}
				throw new FormatException("Only bases 2, 8, 10, or 16 allowed");
			}
			style = NumberStyles.Integer;
			num = 1;
			bigInteger = BigInteger.radix2;
			val = BigInteger.radix2E;
			IL_A5:
			int num2 = 0;
			this.sign = 1;
			if (str[0] == '-')
			{
				if (str.Length == 1)
				{
					throw new FormatException("Zero length BigInteger");
				}
				this.sign = -1;
				num2 = 1;
			}
			while (num2 < str.Length && int.Parse(str[num2].ToString(), style) == 0)
			{
				num2++;
			}
			if (num2 >= str.Length)
			{
				this.sign = 0;
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			BigInteger bigInteger2 = BigInteger.Zero;
			int num3 = num2 + num;
			if (num3 <= str.Length)
			{
				string text;
				while (true)
				{
					text = str.Substring(num2, num);
					ulong num4 = ulong.Parse(text, style);
					BigInteger value = BigInteger.CreateUValueOf(num4);
					if (radix != 2)
					{
						if (radix != 8)
						{
							if (radix != 16)
							{
								bigInteger2 = bigInteger2.Multiply(val);
							}
							else
							{
								bigInteger2 = bigInteger2.ShiftLeft(64);
							}
						}
						else
						{
							if (num4 >= 8uL)
							{
								goto Block_13;
							}
							bigInteger2 = bigInteger2.ShiftLeft(3);
						}
					}
					else
					{
						if (num4 >= 2uL)
						{
							break;
						}
						bigInteger2 = bigInteger2.ShiftLeft(1);
					}
					bigInteger2 = bigInteger2.Add(value);
					num2 = num3;
					num3 += num;
					if (num3 > str.Length)
					{
						goto IL_1EE;
					}
				}
				throw new FormatException("Bad character in radix 2 string: " + text);
				Block_13:
				throw new FormatException("Bad character in radix 8 string: " + text);
			}
			IL_1EE:
			if (num2 < str.Length)
			{
				string text2 = str.Substring(num2);
				ulong value2 = ulong.Parse(text2, style);
				BigInteger bigInteger3 = BigInteger.CreateUValueOf(value2);
				if (bigInteger2.sign > 0)
				{
					if (radix != 2 && radix != 8)
					{
						if (radix == 16)
						{
							bigInteger2 = bigInteger2.ShiftLeft(text2.Length << 2);
						}
						else
						{
							bigInteger2 = bigInteger2.Multiply(bigInteger.Pow(text2.Length));
						}
					}
					bigInteger2 = bigInteger2.Add(bigInteger3);
				}
				else
				{
					bigInteger2 = bigInteger3;
				}
			}
			this.magnitude = bigInteger2.magnitude;
		}

		public BigInteger(byte[] bytes) : this(bytes, 0, bytes.Length)
		{
		}

		public BigInteger(byte[] bytes, int offset, int length)
		{
			if (length == 0)
			{
				throw new FormatException("Zero length BigInteger");
			}
			if ((sbyte)bytes[offset] >= 0)
			{
				this.magnitude = BigInteger.MakeMagnitude(bytes, offset, length);
				this.sign = ((this.magnitude.Length > 0) ? 1 : 0);
				return;
			}
			this.sign = -1;
			int num = offset + length;
			int num2 = offset;
			while (num2 < num && (sbyte)bytes[num2] == -1)
			{
				num2++;
			}
			if (num2 >= num)
			{
				this.magnitude = BigInteger.One.magnitude;
				return;
			}
			int num3 = num - num2;
			byte[] array = new byte[num3];
			int i = 0;
			while (i < num3)
			{
				array[i++] = ~bytes[num2++];
			}
			while (array[--i] == 255)
			{
				array[i] = 0;
			}
			byte[] expr_A5_cp_0 = array;
			int expr_A5_cp_1 = i;
			expr_A5_cp_0[expr_A5_cp_1] += 1;
			this.magnitude = BigInteger.MakeMagnitude(array, 0, array.Length);
		}

		private static int[] MakeMagnitude(byte[] bytes, int offset, int length)
		{
			int num = offset + length;
			int num2 = offset;
			while (num2 < num && bytes[num2] == 0)
			{
				num2++;
			}
			if (num2 >= num)
			{
				return BigInteger.ZeroMagnitude;
			}
			int num3 = (num - num2 + 3) / 4;
			int num4 = (num - num2) % 4;
			if (num4 == 0)
			{
				num4 = 4;
			}
			if (num3 < 1)
			{
				return BigInteger.ZeroMagnitude;
			}
			int[] array = new int[num3];
			int num5 = 0;
			int num6 = 0;
			for (int i = num2; i < num; i++)
			{
				num5 <<= 8;
				num5 |= (int)(bytes[i] & 255);
				num4--;
				if (num4 <= 0)
				{
					array[num6] = num5;
					num6++;
					num4 = 4;
					num5 = 0;
				}
			}
			if (num6 < array.Length)
			{
				array[num6] = num5;
			}
			return array;
		}

		public BigInteger(int sign, byte[] bytes) : this(sign, bytes, 0, bytes.Length)
		{
		}

		public BigInteger(int sign, byte[] bytes, int offset, int length)
		{
			if (sign < -1 || sign > 1)
			{
				throw new FormatException("Invalid sign value");
			}
			if (sign == 0)
			{
				this.sign = 0;
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			this.magnitude = BigInteger.MakeMagnitude(bytes, offset, length);
			this.sign = ((this.magnitude.Length < 1) ? 0 : sign);
		}

		public BigInteger(int sizeInBits, Random random)
		{
			if (sizeInBits < 0)
			{
				throw new ArgumentException("sizeInBits must be non-negative");
			}
			this.nBits = -1;
			this.nBitLength = -1;
			if (sizeInBits == 0)
			{
				this.sign = 0;
				this.magnitude = BigInteger.ZeroMagnitude;
				return;
			}
			int byteLength = BigInteger.GetByteLength(sizeInBits);
			byte[] array = new byte[byteLength];
			random.NextBytes(array);
			int num = 8 * byteLength - sizeInBits;
			byte[] expr_69_cp_0 = array;
			int expr_69_cp_1 = 0;
			expr_69_cp_0[expr_69_cp_1] &= (byte)(255u >> num);
			this.magnitude = BigInteger.MakeMagnitude(array, 0, array.Length);
			this.sign = ((this.magnitude.Length < 1) ? 0 : 1);
		}

		public BigInteger(int bitLength, int certainty, Random random)
		{
			if (bitLength < 2)
			{
				throw new ArithmeticException("bitLength < 2");
			}
			this.sign = 1;
			this.nBitLength = bitLength;
			if (bitLength == 2)
			{
				this.magnitude = ((random.Next(2) == 0) ? BigInteger.Two.magnitude : BigInteger.Three.magnitude);
				return;
			}
			int byteLength = BigInteger.GetByteLength(bitLength);
			byte[] array = new byte[byteLength];
			int num = 8 * byteLength - bitLength;
			byte b = (byte)(255u >> num);
			while (true)
			{
				random.NextBytes(array);
				byte[] expr_89_cp_0 = array;
				int expr_89_cp_1 = 0;
				expr_89_cp_0[expr_89_cp_1] &= b;
				byte[] expr_9E_cp_0 = array;
				int expr_9E_cp_1 = 0;
				expr_9E_cp_0[expr_9E_cp_1] |= (byte)(1 << 7 - num);
				byte[] expr_BD_cp_0 = array;
				int expr_BD_cp_1 = byteLength - 1;
				expr_BD_cp_0[expr_BD_cp_1] |= 1;
				this.magnitude = BigInteger.MakeMagnitude(array, 0, array.Length);
				this.nBits = -1;
				this.mQuote = 0;
				if (certainty < 1)
				{
					break;
				}
				if (this.CheckProbablePrime(certainty, random))
				{
					return;
				}
				if (bitLength > 32)
				{
					for (int i = 0; i < 10000; i++)
					{
						int num2 = 33 + random.Next(bitLength - 2);
						this.magnitude[this.magnitude.Length - (num2 >> 5)] ^= 1 << num2;
						this.magnitude[this.magnitude.Length - 1] ^= random.Next() + 1 << 1;
						this.mQuote = 0;
						if (this.CheckProbablePrime(certainty, random))
						{
							return;
						}
					}
				}
			}
		}

		public BigInteger Abs()
		{
			if (this.sign < 0)
			{
				return this.Negate();
			}
			return this;
		}

		private static int[] AddMagnitudes(int[] a, int[] b)
		{
			int num = a.Length - 1;
			int i = b.Length - 1;
			long num2 = 0L;
			while (i >= 0)
			{
				num2 += (long)((ulong)a[num] + (ulong)b[i--]);
				a[num--] = (int)num2;
				num2 = (long)((ulong)num2 >> 32);
			}
			if (num2 != 0L)
			{
				while (num >= 0 && ++a[num--] == 0)
				{
				}
			}
			return a;
		}

		public BigInteger Add(BigInteger value)
		{
			if (this.sign == 0)
			{
				return value;
			}
			if (this.sign == value.sign)
			{
				return this.AddToMagnitude(value.magnitude);
			}
			if (value.sign == 0)
			{
				return this;
			}
			if (value.sign < 0)
			{
				return this.Subtract(value.Negate());
			}
			return value.Subtract(this.Negate());
		}

		private BigInteger AddToMagnitude(int[] magToAdd)
		{
			int[] array;
			int[] array2;
			if (this.magnitude.Length < magToAdd.Length)
			{
				array = magToAdd;
				array2 = this.magnitude;
			}
			else
			{
				array = this.magnitude;
				array2 = magToAdd;
			}
			uint num = 4294967295u;
			if (array.Length == array2.Length)
			{
				num -= (uint)array2[0];
			}
			bool flag = array[0] >= (int)num;
			int[] array3;
			if (flag)
			{
				array3 = new int[array.Length + 1];
				array.CopyTo(array3, 1);
			}
			else
			{
				array3 = (int[])array.Clone();
			}
			array3 = BigInteger.AddMagnitudes(array3, array2);
			return new BigInteger(this.sign, array3, flag);
		}

		public BigInteger And(BigInteger value)
		{
			if (this.sign == 0 || value.sign == 0)
			{
				return BigInteger.Zero;
			}
			int[] array = (this.sign > 0) ? this.magnitude : this.Add(BigInteger.One).magnitude;
			int[] array2 = (value.sign > 0) ? value.magnitude : value.Add(BigInteger.One).magnitude;
			bool flag = this.sign < 0 && value.sign < 0;
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 & num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger AndNot(BigInteger val)
		{
			return this.And(val.Not());
		}

		public static int BitCnt(int i)
		{
			uint num = (uint)(i - (int)((uint)i >> 1 & 1431655765u));
			num = (num & 858993459u) + (num >> 2 & 858993459u);
			num = (num + (num >> 4) & 252645135u);
			num += num >> 8;
			num += num >> 16;
			return (int)(num & 63u);
		}

		private static int CalcBitLength(int sign, int indx, int[] mag)
		{
			while (indx < mag.Length)
			{
				if (mag[indx] != 0)
				{
					int num = 32 * (mag.Length - indx - 1);
					int num2 = mag[indx];
					num += BigInteger.BitLen(num2);
					if (sign < 0 && (num2 & -num2) == num2)
					{
						while (++indx < mag.Length)
						{
							if (mag[indx] != 0)
							{
								return num;
							}
						}
						num--;
					}
					return num;
				}
				indx++;
			}
			return 0;
		}

		private static int BitLen(int w)
		{
			uint num = (uint)w >> 24;
			if (num != 0u)
			{
				return (int)(24 + BigInteger.BitLengthTable[(int)((UIntPtr)num)]);
			}
			num = (uint)w >> 16;
			if (num != 0u)
			{
				return (int)(16 + BigInteger.BitLengthTable[(int)((UIntPtr)num)]);
			}
			num = (uint)w >> 8;
			if (num != 0u)
			{
				return (int)(8 + BigInteger.BitLengthTable[(int)((UIntPtr)num)]);
			}
			return (int)BigInteger.BitLengthTable[(int)((UIntPtr)w)];
		}

		private bool QuickPow2Check()
		{
			return this.sign > 0 && this.nBits == 1;
		}

		public int CompareTo(object obj)
		{
			return this.CompareTo((BigInteger)obj);
		}

		private static int CompareTo(int xIndx, int[] x, int yIndx, int[] y)
		{
			while (xIndx != x.Length)
			{
				if (x[xIndx] != 0)
				{
					break;
				}
				xIndx++;
			}
			while (yIndx != y.Length && y[yIndx] == 0)
			{
				yIndx++;
			}
			return BigInteger.CompareNoLeadingZeroes(xIndx, x, yIndx, y);
		}

		private static int CompareNoLeadingZeroes(int xIndx, int[] x, int yIndx, int[] y)
		{
			int num = x.Length - y.Length - (xIndx - yIndx);
			if (num == 0)
			{
				while (xIndx < x.Length)
				{
					uint num2 = (uint)x[xIndx++];
					uint num3 = (uint)y[yIndx++];
					if (num2 != num3)
					{
						if (num2 >= num3)
						{
							return 1;
						}
						return -1;
					}
				}
				return 0;
			}
			if (num >= 0)
			{
				return 1;
			}
			return -1;
		}

		public int CompareTo(BigInteger value)
		{
			if (this.sign < value.sign)
			{
				return -1;
			}
			if (this.sign > value.sign)
			{
				return 1;
			}
			if (this.sign != 0)
			{
				return this.sign * BigInteger.CompareNoLeadingZeroes(0, this.magnitude, 0, value.magnitude);
			}
			return 0;
		}

		private int[] Divide(int[] x, int[] y)
		{
			int num = 0;
			while (num < x.Length && x[num] == 0)
			{
				num++;
			}
			int num2 = 0;
			while (num2 < y.Length && y[num2] == 0)
			{
				num2++;
			}
			int num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
			int[] array3;
			if (num3 > 0)
			{
				int num4 = BigInteger.CalcBitLength(1, num2, y);
				int num5 = BigInteger.CalcBitLength(1, num, x);
				int num6 = num5 - num4;
				int num7 = 0;
				int num8 = 0;
				int num9 = num4;
				int[] array;
				int[] array2;
				if (num6 > 0)
				{
					array = new int[(num6 >> 5) + 1];
					array[0] = 1 << num6 % 32;
					array2 = BigInteger.ShiftLeft(y, num6);
					num9 += num6;
				}
				else
				{
					array = new int[]
					{
						1
					};
					int num10 = y.Length - num2;
					array2 = new int[num10];
					Array.Copy(y, num2, array2, 0, num10);
				}
				array3 = new int[array.Length];
				while (true)
				{
					if (num9 < num5 || BigInteger.CompareNoLeadingZeroes(num, x, num8, array2) >= 0)
					{
						BigInteger.Subtract(num, x, num8, array2);
						BigInteger.AddMagnitudes(array3, array);
						while (x[num] == 0)
						{
							if (++num == x.Length)
							{
								return array3;
							}
						}
						num5 = 32 * (x.Length - num - 1) + BigInteger.BitLen(x[num]);
						if (num5 <= num4)
						{
							if (num5 < num4)
							{
								return array3;
							}
							num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
							if (num3 <= 0)
							{
								goto IL_1C2;
							}
						}
					}
					num6 = num9 - num5;
					if (num6 == 1)
					{
						uint num11 = (uint)array2[num8] >> 1;
						uint num12 = (uint)x[num];
						if (num11 > num12)
						{
							num6++;
						}
					}
					if (num6 < 2)
					{
						BigInteger.ShiftRightOneInPlace(num8, array2);
						num9--;
						BigInteger.ShiftRightOneInPlace(num7, array);
					}
					else
					{
						BigInteger.ShiftRightInPlace(num8, array2, num6);
						num9 -= num6;
						BigInteger.ShiftRightInPlace(num7, array, num6);
					}
					while (array2[num8] == 0)
					{
						num8++;
					}
					while (array[num7] == 0)
					{
						num7++;
					}
				}
				return array3;
			}
			array3 = new int[1];
			IL_1C2:
			if (num3 == 0)
			{
				BigInteger.AddMagnitudes(array3, BigInteger.One.magnitude);
				Array.Clear(x, num, x.Length - num);
			}
			return array3;
		}

		public BigInteger Divide(BigInteger val)
		{
			if (val.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			if (!val.QuickPow2Check())
			{
				int[] x = (int[])this.magnitude.Clone();
				return new BigInteger(this.sign * val.sign, this.Divide(x, val.magnitude), true);
			}
			BigInteger bigInteger = this.Abs().ShiftRight(val.Abs().BitLength - 1);
			if (val.sign != this.sign)
			{
				return bigInteger.Negate();
			}
			return bigInteger;
		}

		public BigInteger[] DivideAndRemainder(BigInteger val)
		{
			if (val.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			BigInteger[] array = new BigInteger[2];
			if (this.sign == 0)
			{
				array[0] = BigInteger.Zero;
				array[1] = BigInteger.Zero;
			}
			else if (val.QuickPow2Check())
			{
				int n = val.Abs().BitLength - 1;
				BigInteger bigInteger = this.Abs().ShiftRight(n);
				int[] mag = this.LastNBits(n);
				array[0] = ((val.sign == this.sign) ? bigInteger : bigInteger.Negate());
				array[1] = new BigInteger(this.sign, mag, true);
			}
			else
			{
				int[] array2 = (int[])this.magnitude.Clone();
				int[] mag2 = this.Divide(array2, val.magnitude);
				array[0] = new BigInteger(this.sign * val.sign, mag2, true);
				array[1] = new BigInteger(this.sign, array2, true);
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			BigInteger bigInteger = obj as BigInteger;
			return bigInteger != null && this.sign == bigInteger.sign && this.IsEqualMagnitude(bigInteger);
		}

		private bool IsEqualMagnitude(BigInteger x)
		{
			int[] arg_06_0 = x.magnitude;
			if (this.magnitude.Length != x.magnitude.Length)
			{
				return false;
			}
			for (int i = 0; i < this.magnitude.Length; i++)
			{
				if (this.magnitude[i] != x.magnitude[i])
				{
					return false;
				}
			}
			return true;
		}

		public BigInteger Gcd(BigInteger value)
		{
			if (value.sign == 0)
			{
				return this.Abs();
			}
			if (this.sign == 0)
			{
				return value.Abs();
			}
			BigInteger bigInteger = this;
			BigInteger bigInteger2 = value;
			while (bigInteger2.sign != 0)
			{
				BigInteger bigInteger3 = bigInteger.Mod(bigInteger2);
				bigInteger = bigInteger2;
				bigInteger2 = bigInteger3;
			}
			return bigInteger;
		}

		public override int GetHashCode()
		{
			int num = this.magnitude.Length;
			if (this.magnitude.Length > 0)
			{
				num ^= this.magnitude[0];
				if (this.magnitude.Length > 1)
				{
					num ^= this.magnitude[this.magnitude.Length - 1];
				}
			}
			if (this.sign >= 0)
			{
				return num;
			}
			return ~num;
		}

		private BigInteger Inc()
		{
			if (this.sign == 0)
			{
				return BigInteger.One;
			}
			if (this.sign < 0)
			{
				return new BigInteger(-1, BigInteger.doSubBigLil(this.magnitude, BigInteger.One.magnitude), true);
			}
			return this.AddToMagnitude(BigInteger.One.magnitude);
		}

		public bool IsProbablePrime(int certainty)
		{
			if (certainty <= 0)
			{
				return true;
			}
			BigInteger bigInteger = this.Abs();
			if (!bigInteger.TestBit(0))
			{
				return bigInteger.Equals(BigInteger.Two);
			}
			return !bigInteger.Equals(BigInteger.One) && bigInteger.CheckProbablePrime(certainty, BigInteger.RandomSource);
		}

		private bool CheckProbablePrime(int certainty, Random random)
		{
			int num = Math.Min(this.BitLength - 1, BigInteger.primeLists.Length);
			for (int i = 0; i < num; i++)
			{
				int num2 = this.Remainder(BigInteger.primeProducts[i]);
				int[] array = BigInteger.primeLists[i];
				for (int j = 0; j < array.Length; j++)
				{
					int num3 = array[j];
					if (num2 % num3 == 0)
					{
						return this.BitLength < 16 && this.IntValue == num3;
					}
				}
			}
			return this.RabinMillerTest(certainty, random);
		}

		public bool RabinMillerTest(int certainty, Random random)
		{
			int lowestSetBitMaskFirst = this.GetLowestSetBitMaskFirst(-2);
			BigInteger e = this.ShiftRight(lowestSetBitMaskFirst);
			BigInteger bigInteger = BigInteger.One.ShiftLeft(32 * this.magnitude.Length).Remainder(this);
			BigInteger bigInteger2 = this.Subtract(bigInteger);
			while (true)
			{
				BigInteger bigInteger3 = new BigInteger(this.BitLength, random);
				if (bigInteger3.sign != 0 && bigInteger3.CompareTo(this) < 0 && !bigInteger3.IsEqualMagnitude(bigInteger) && !bigInteger3.IsEqualMagnitude(bigInteger2))
				{
					BigInteger bigInteger4 = BigInteger.ModPowMonty(bigInteger3, e, this, false);
					if (!bigInteger4.Equals(bigInteger))
					{
						int num = 0;
						while (!bigInteger4.Equals(bigInteger2))
						{
							if (++num == lowestSetBitMaskFirst)
							{
								return false;
							}
							bigInteger4 = BigInteger.ModPowMonty(bigInteger4, BigInteger.Two, this, false);
							if (bigInteger4.Equals(bigInteger))
							{
								return false;
							}
						}
					}
					certainty -= 2;
					if (certainty <= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public BigInteger Max(BigInteger value)
		{
			if (this.CompareTo(value) <= 0)
			{
				return value;
			}
			return this;
		}

		public BigInteger Min(BigInteger value)
		{
			if (this.CompareTo(value) >= 0)
			{
				return value;
			}
			return this;
		}

		public BigInteger Mod(BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			BigInteger bigInteger = this.Remainder(m);
			if (bigInteger.sign < 0)
			{
				return bigInteger.Add(m);
			}
			return bigInteger;
		}

		public BigInteger ModInverse(BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			if (m.QuickPow2Check())
			{
				return this.ModInversePow2(m);
			}
			BigInteger a = this.Remainder(m);
			BigInteger bigInteger2;
			BigInteger bigInteger = BigInteger.ExtEuclid(a, m, out bigInteger2);
			if (!bigInteger.Equals(BigInteger.One))
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			if (bigInteger2.sign < 0)
			{
				bigInteger2 = bigInteger2.Add(m);
			}
			return bigInteger2;
		}

		private BigInteger ModInversePow2(BigInteger m)
		{
			if (!this.TestBit(0))
			{
				throw new ArithmeticException("Numbers not relatively prime.");
			}
			int num = m.BitLength - 1;
			long num2 = BigInteger.ModInverse64(this.LongValue);
			if (num < 64)
			{
				num2 &= (1L << num) - 1L;
			}
			BigInteger bigInteger = BigInteger.ValueOf(num2);
			if (num > 64)
			{
				BigInteger val = this.Remainder(m);
				int num3 = 64;
				do
				{
					BigInteger n = bigInteger.Multiply(val).Remainder(m);
					bigInteger = bigInteger.Multiply(BigInteger.Two.Subtract(n)).Remainder(m);
					num3 <<= 1;
				}
				while (num3 < num);
			}
			if (bigInteger.sign < 0)
			{
				bigInteger = bigInteger.Add(m);
			}
			return bigInteger;
		}

		private static int ModInverse32(int d)
		{
			int num = d + ((d + 1 & 4) << 1);
			num *= 2 - d * num;
			num *= 2 - d * num;
			return num * (2 - d * num);
		}

		private static long ModInverse64(long d)
		{
			long num = d + ((d + 1L & 4L) << 1);
			num *= 2L - d * num;
			num *= 2L - d * num;
			num *= 2L - d * num;
			return num * (2L - d * num);
		}

		private static BigInteger ExtEuclid(BigInteger a, BigInteger b, out BigInteger u1Out)
		{
			BigInteger bigInteger = BigInteger.One;
			BigInteger bigInteger2 = BigInteger.Zero;
			BigInteger bigInteger3 = a;
			BigInteger bigInteger4 = b;
			if (bigInteger4.sign > 0)
			{
				while (true)
				{
					BigInteger[] array = bigInteger3.DivideAndRemainder(bigInteger4);
					bigInteger3 = bigInteger4;
					bigInteger4 = array[1];
					BigInteger bigInteger5 = bigInteger;
					bigInteger = bigInteger2;
					if (bigInteger4.sign <= 0)
					{
						break;
					}
					bigInteger2 = bigInteger5.Subtract(bigInteger2.Multiply(array[0]));
				}
			}
			u1Out = bigInteger;
			return bigInteger3;
		}

		private static void ZeroOut(int[] x)
		{
			Array.Clear(x, 0, x.Length);
		}

		public BigInteger ModPow(BigInteger e, BigInteger m)
		{
			if (m.sign < 1)
			{
				throw new ArithmeticException("Modulus must be positive");
			}
			if (m.Equals(BigInteger.One))
			{
				return BigInteger.Zero;
			}
			if (e.sign == 0)
			{
				return BigInteger.One;
			}
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			bool flag = e.sign < 0;
			if (flag)
			{
				e = e.Negate();
			}
			BigInteger bigInteger = this.Mod(m);
			if (!e.Equals(BigInteger.One))
			{
				if ((m.magnitude[m.magnitude.Length - 1] & 1) == 0)
				{
					bigInteger = BigInteger.ModPowBarrett(bigInteger, e, m);
				}
				else
				{
					bigInteger = BigInteger.ModPowMonty(bigInteger, e, m, true);
				}
			}
			if (flag)
			{
				bigInteger = bigInteger.ModInverse(m);
			}
			return bigInteger;
		}

		private static BigInteger ModPowBarrett(BigInteger b, BigInteger e, BigInteger m)
		{
			int num = m.magnitude.Length;
			BigInteger mr = BigInteger.One.ShiftLeft(num + 1 << 5);
			BigInteger yu = BigInteger.One.ShiftLeft(num << 6).Divide(m);
			int num2 = 0;
			int i = e.BitLength;
			while (i > BigInteger.ExpWindowThresholds[num2])
			{
				num2++;
			}
			int num3 = 1 << num2;
			BigInteger[] array = new BigInteger[num3];
			array[0] = b;
			BigInteger bigInteger = BigInteger.ReduceBarrett(b.Square(), m, mr, yu);
			for (int j = 1; j < num3; j++)
			{
				array[j] = BigInteger.ReduceBarrett(array[j - 1].Multiply(bigInteger), m, mr, yu);
			}
			int[] windowList = BigInteger.GetWindowList(e.magnitude, num2);
			int num4 = windowList[0];
			int num5 = num4 & 255;
			int num6 = num4 >> 8;
			BigInteger bigInteger2;
			if (num5 == 1)
			{
				bigInteger2 = bigInteger;
				num6--;
			}
			else
			{
				bigInteger2 = array[num5 >> 1];
			}
			int num7 = 1;
			while ((num4 = windowList[num7++]) != -1)
			{
				num5 = (num4 & 255);
				int num8 = num6 + (int)BigInteger.BitLengthTable[num5];
				for (int k = 0; k < num8; k++)
				{
					bigInteger2 = BigInteger.ReduceBarrett(bigInteger2.Square(), m, mr, yu);
				}
				bigInteger2 = BigInteger.ReduceBarrett(bigInteger2.Multiply(array[num5 >> 1]), m, mr, yu);
				num6 = num4 >> 8;
			}
			for (int l = 0; l < num6; l++)
			{
				bigInteger2 = BigInteger.ReduceBarrett(bigInteger2.Square(), m, mr, yu);
			}
			return bigInteger2;
		}

		private static BigInteger ReduceBarrett(BigInteger x, BigInteger m, BigInteger mr, BigInteger yu)
		{
			int bitLength = x.BitLength;
			int bitLength2 = m.BitLength;
			if (bitLength < bitLength2)
			{
				return x;
			}
			if (bitLength - bitLength2 > 1)
			{
				int num = m.magnitude.Length;
				BigInteger bigInteger = x.DivideWords(num - 1);
				BigInteger bigInteger2 = bigInteger.Multiply(yu);
				BigInteger bigInteger3 = bigInteger2.DivideWords(num + 1);
				BigInteger bigInteger4 = x.RemainderWords(num + 1);
				BigInteger bigInteger5 = bigInteger3.Multiply(m);
				BigInteger n = bigInteger5.RemainderWords(num + 1);
				x = bigInteger4.Subtract(n);
				if (x.sign < 0)
				{
					x = x.Add(mr);
				}
			}
			while (x.CompareTo(m) >= 0)
			{
				x = x.Subtract(m);
			}
			return x;
		}

		private static BigInteger ModPowMonty(BigInteger b, BigInteger e, BigInteger m, bool convert)
		{
			int num = m.magnitude.Length;
			int num2 = 32 * num;
			bool flag = m.BitLength + 2 <= num2;
			uint mDash = (uint)m.GetMQuote();
			if (convert)
			{
				b = b.ShiftLeft(num2).Remainder(m);
			}
			int[] a = new int[num + 1];
			int[] array = b.magnitude;
			if (array.Length < num)
			{
				int[] array2 = new int[num];
				array.CopyTo(array2, num - array.Length);
				array = array2;
			}
			int num3 = 0;
			if (e.magnitude.Length > 1 || e.BitCount > 2)
			{
				int i = e.BitLength;
				while (i > BigInteger.ExpWindowThresholds[num3])
				{
					num3++;
				}
			}
			int num4 = 1 << num3;
			int[][] array3 = new int[num4][];
			array3[0] = array;
			int[] array4 = Arrays.Clone(array);
			BigInteger.SquareMonty(a, array4, m.magnitude, mDash, flag);
			for (int j = 1; j < num4; j++)
			{
				array3[j] = Arrays.Clone(array3[j - 1]);
				BigInteger.MultiplyMonty(a, array3[j], array4, m.magnitude, mDash, flag);
			}
			int[] windowList = BigInteger.GetWindowList(e.magnitude, num3);
			int num5 = windowList[0];
			int num6 = num5 & 255;
			int num7 = num5 >> 8;
			int[] array5;
			if (num6 == 1)
			{
				array5 = array4;
				num7--;
			}
			else
			{
				array5 = Arrays.Clone(array3[num6 >> 1]);
			}
			int num8 = 1;
			while ((num5 = windowList[num8++]) != -1)
			{
				num6 = (num5 & 255);
				int num9 = num7 + (int)BigInteger.BitLengthTable[num6];
				for (int k = 0; k < num9; k++)
				{
					BigInteger.SquareMonty(a, array5, m.magnitude, mDash, flag);
				}
				BigInteger.MultiplyMonty(a, array5, array3[num6 >> 1], m.magnitude, mDash, flag);
				num7 = num5 >> 8;
			}
			for (int l = 0; l < num7; l++)
			{
				BigInteger.SquareMonty(a, array5, m.magnitude, mDash, flag);
			}
			if (convert)
			{
				BigInteger.MontgomeryReduce(array5, m.magnitude, mDash);
			}
			else if (flag && BigInteger.CompareTo(0, array5, 0, m.magnitude) >= 0)
			{
				BigInteger.Subtract(0, array5, 0, m.magnitude);
			}
			return new BigInteger(1, array5, true);
		}

		private static int[] GetWindowList(int[] mag, int extraBits)
		{
			int num = mag[0];
			int num2 = BigInteger.BitLen(num);
			int num3 = ((mag.Length - 1 << 5) + num2) / (1 + extraBits) + 2;
			int[] array = new int[num3];
			int num4 = 0;
			int num5 = 33 - num2;
			num <<= num5;
			int num6 = 1;
			int num7 = 1 << extraBits;
			int num8 = 0;
			int num9 = 0;
			while (true)
			{
				if (num5 >= 32)
				{
					if (++num9 == mag.Length)
					{
						break;
					}
					num = mag[num9];
					num5 = 0;
				}
				else
				{
					if (num6 < num7)
					{
						num6 = (num6 << 1 | (int)((uint)num >> 31));
					}
					else if (num < 0)
					{
						array[num4++] = BigInteger.CreateWindowEntry(num6, num8);
						num6 = 1;
						num8 = 0;
					}
					else
					{
						num8++;
					}
					num <<= 1;
					num5++;
				}
			}
			array[num4++] = BigInteger.CreateWindowEntry(num6, num8);
			array[num4] = -1;
			return array;
		}

		private static int CreateWindowEntry(int mult, int zeroes)
		{
			while ((mult & 1) == 0)
			{
				mult >>= 1;
				zeroes++;
			}
			return mult | zeroes << 8;
		}

		private static int[] Square(int[] w, int[] x)
		{
			int num = w.Length - 1;
			ulong num3;
			for (int i = x.Length - 1; i > 0; i--)
			{
				ulong num2 = (ulong)x[i];
				num3 = num2 * num2 + (ulong)w[num];
				w[num] = (int)num3;
				num3 >>= 32;
				for (int j = i - 1; j >= 0; j--)
				{
					ulong num4 = num2 * (ulong)x[j];
					num3 += ((ulong)w[--num] & (ulong)-1) + (ulong)((ulong)((uint)num4) << 1);
					w[num] = (int)num3;
					num3 = (num3 >> 32) + (num4 >> 31);
				}
				num3 += (ulong)w[--num];
				w[num] = (int)num3;
				if (--num >= 0)
				{
					w[num] = (int)(num3 >> 32);
				}
				num += i;
			}
			num3 = (ulong)x[0];
			num3 = num3 * num3 + (ulong)w[num];
			w[num] = (int)num3;
			if (--num >= 0)
			{
				w[num] += (int)(num3 >> 32);
			}
			return w;
		}

		private static int[] Multiply(int[] x, int[] y, int[] z)
		{
			int num = z.Length;
			if (num < 1)
			{
				return x;
			}
			int num2 = x.Length - y.Length;
			do
			{
				long num3 = (long)z[--num] & (long)((ulong)-1);
				long num4 = 0L;
				if (num3 != 0L)
				{
					for (int i = y.Length - 1; i >= 0; i--)
					{
						num4 += num3 * ((long)y[i] & (long)((ulong)-1)) + ((long)x[num2 + i] & (long)((ulong)-1));
						x[num2 + i] = (int)num4;
						num4 = (long)((ulong)num4 >> 32);
					}
				}
				num2--;
				if (num2 >= 0)
				{
					x[num2] = (int)num4;
				}
			}
			while (num > 0);
			return x;
		}

		private int GetMQuote()
		{
			if (this.mQuote != 0)
			{
				return this.mQuote;
			}
			int d = -this.magnitude[this.magnitude.Length - 1];
			return this.mQuote = BigInteger.ModInverse32(d);
		}

		private static void MontgomeryReduce(int[] x, int[] m, uint mDash)
		{
			int num = m.Length;
			for (int i = num - 1; i >= 0; i--)
			{
				uint num2 = (uint)x[num - 1];
				ulong num3 = (ulong)(num2 * mDash);
				ulong num4 = num3 * (ulong)m[num - 1] + (ulong)num2;
				num4 >>= 32;
				for (int j = num - 2; j >= 0; j--)
				{
					num4 += num3 * (ulong)m[j] + (ulong)x[j];
					x[j + 1] = (int)num4;
					num4 >>= 32;
				}
				x[0] = (int)num4;
			}
			if (BigInteger.CompareTo(0, x, 0, m) >= 0)
			{
				BigInteger.Subtract(0, x, 0, m);
			}
		}

		private static void MultiplyMonty(int[] a, int[] x, int[] y, int[] m, uint mDash, bool smallMontyModulus)
		{
			int num = m.Length;
			if (num == 1)
			{
				x[0] = (int)BigInteger.MultiplyMontyNIsOne((uint)x[0], (uint)y[0], (uint)m[0], mDash);
				return;
			}
			uint num2 = (uint)y[num - 1];
			ulong num3 = (ulong)x[num - 1];
			ulong num4 = num3 * (ulong)num2;
			ulong num5 = (ulong)((uint)num4 * mDash);
			ulong num6 = num5 * (ulong)m[num - 1];
			num4 += (ulong)((uint)num6);
			num4 = (num4 >> 32) + (num6 >> 32);
			for (int i = num - 2; i >= 0; i--)
			{
				ulong num7 = num3 * (ulong)y[i];
				num6 = num5 * (ulong)m[i];
				num4 += (num7 & (ulong)-1) + (ulong)((uint)num6);
				a[i + 2] = (int)num4;
				num4 = (num4 >> 32) + (num7 >> 32) + (num6 >> 32);
			}
			a[1] = (int)num4;
			int num8 = (int)(num4 >> 32);
			for (int j = num - 2; j >= 0; j--)
			{
				uint num9 = (uint)a[num];
				ulong num10 = (ulong)x[j];
				ulong num11 = num10 * (ulong)num2;
				ulong num12 = (num11 & (ulong)-1) + (ulong)num9;
				ulong num13 = (ulong)((uint)num12 * mDash);
				ulong num14 = num13 * (ulong)m[num - 1];
				num12 += (ulong)((uint)num14);
				num12 = (num12 >> 32) + (num11 >> 32) + (num14 >> 32);
				for (int k = num - 2; k >= 0; k--)
				{
					num11 = num10 * (ulong)y[k];
					num14 = num13 * (ulong)m[k];
					num12 += (num11 & (ulong)-1) + (ulong)((uint)num14) + (ulong)a[k + 1];
					a[k + 2] = (int)num12;
					num12 = (num12 >> 32) + (num11 >> 32) + (num14 >> 32);
				}
				num12 += (ulong)num8;
				a[1] = (int)num12;
				num8 = (int)(num12 >> 32);
			}
			a[0] = num8;
			if (!smallMontyModulus && BigInteger.CompareTo(0, a, 0, m) >= 0)
			{
				BigInteger.Subtract(0, a, 0, m);
			}
			Array.Copy(a, 1, x, 0, num);
		}

		private static void SquareMonty(int[] a, int[] x, int[] m, uint mDash, bool smallMontyModulus)
		{
			int num = m.Length;
			if (num == 1)
			{
				uint num2 = (uint)x[0];
				x[0] = (int)BigInteger.MultiplyMontyNIsOne(num2, num2, (uint)m[0], mDash);
				return;
			}
			ulong num3 = (ulong)x[num - 1];
			ulong num4 = num3 * num3;
			ulong num5 = (ulong)((uint)num4 * mDash);
			ulong num6 = num5 * (ulong)m[num - 1];
			num4 += (ulong)((uint)num6);
			num4 = (num4 >> 32) + (num6 >> 32);
			for (int i = num - 2; i >= 0; i--)
			{
				ulong num7 = num3 * (ulong)x[i];
				num6 = num5 * (ulong)m[i];
				num4 += (num6 & (ulong)-1) + (ulong)((ulong)((uint)num7) << 1);
				a[i + 2] = (int)num4;
				num4 = (num4 >> 32) + (num7 >> 31) + (num6 >> 32);
			}
			a[1] = (int)num4;
			int num8 = (int)(num4 >> 32);
			for (int j = num - 2; j >= 0; j--)
			{
				uint num9 = (uint)a[num];
				ulong num10 = (ulong)(num9 * mDash);
				ulong num11 = num10 * (ulong)m[num - 1] + (ulong)num9;
				num11 >>= 32;
				for (int k = num - 2; k > j; k--)
				{
					num11 += num10 * (ulong)m[k] + (ulong)a[k + 1];
					a[k + 2] = (int)num11;
					num11 >>= 32;
				}
				ulong num12 = (ulong)x[j];
				ulong num13 = num12 * num12;
				ulong num14 = num10 * (ulong)m[j];
				num11 += (num13 & (ulong)-1) + (ulong)((uint)num14) + (ulong)a[j + 1];
				a[j + 2] = (int)num11;
				num11 = (num11 >> 32) + (num13 >> 32) + (num14 >> 32);
				for (int l = j - 1; l >= 0; l--)
				{
					ulong num15 = num12 * (ulong)x[l];
					ulong num16 = num10 * (ulong)m[l];
					num11 += (num16 & (ulong)-1) + (ulong)((ulong)((uint)num15) << 1) + (ulong)a[l + 1];
					a[l + 2] = (int)num11;
					num11 = (num11 >> 32) + (num15 >> 31) + (num16 >> 32);
				}
				num11 += (ulong)num8;
				a[1] = (int)num11;
				num8 = (int)(num11 >> 32);
			}
			a[0] = num8;
			if (!smallMontyModulus && BigInteger.CompareTo(0, a, 0, m) >= 0)
			{
				BigInteger.Subtract(0, a, 0, m);
			}
			Array.Copy(a, 1, x, 0, num);
		}

		private static uint MultiplyMontyNIsOne(uint x, uint y, uint m, uint mDash)
		{
			ulong num = (ulong)x * (ulong)y;
			uint num2 = (uint)num * mDash;
			ulong num3 = (ulong)m;
			ulong num4 = num3 * (ulong)num2;
			num += (ulong)((uint)num4);
			num = (num >> 32) + (num4 >> 32);
			if (num > num3)
			{
				num -= num3;
			}
			return (uint)num;
		}

		public BigInteger Multiply(BigInteger val)
		{
			if (val == this)
			{
				return this.Square();
			}
			if ((this.sign & val.sign) == 0)
			{
				return BigInteger.Zero;
			}
			if (val.QuickPow2Check())
			{
				BigInteger bigInteger = this.ShiftLeft(val.Abs().BitLength - 1);
				if (val.sign <= 0)
				{
					return bigInteger.Negate();
				}
				return bigInteger;
			}
			else
			{
				if (!this.QuickPow2Check())
				{
					int num = this.magnitude.Length + val.magnitude.Length;
					int[] array = new int[num];
					BigInteger.Multiply(array, this.magnitude, val.magnitude);
					int signum = this.sign ^ val.sign ^ 1;
					return new BigInteger(signum, array, true);
				}
				BigInteger bigInteger2 = val.ShiftLeft(this.Abs().BitLength - 1);
				if (this.sign <= 0)
				{
					return bigInteger2.Negate();
				}
				return bigInteger2;
			}
		}

		public BigInteger Square()
		{
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			if (this.QuickPow2Check())
			{
				return this.ShiftLeft(this.Abs().BitLength - 1);
			}
			int num = this.magnitude.Length << 1;
			if ((uint)this.magnitude[0] >> 16 == 0u)
			{
				num--;
			}
			int[] array = new int[num];
			BigInteger.Square(array, this.magnitude);
			return new BigInteger(1, array, false);
		}

		public BigInteger Negate()
		{
			if (this.sign == 0)
			{
				return this;
			}
			return new BigInteger(-this.sign, this.magnitude, false);
		}

		public BigInteger NextProbablePrime()
		{
			if (this.sign < 0)
			{
				throw new ArithmeticException("Cannot be called on value < 0");
			}
			if (this.CompareTo(BigInteger.Two) < 0)
			{
				return BigInteger.Two;
			}
			BigInteger bigInteger = this.Inc().SetBit(0);
			while (!bigInteger.CheckProbablePrime(100, BigInteger.RandomSource))
			{
				bigInteger = bigInteger.Add(BigInteger.Two);
			}
			return bigInteger;
		}

		public BigInteger Not()
		{
			return this.Inc().Negate();
		}

		public BigInteger Pow(int exp)
		{
			if (exp <= 0)
			{
				if (exp < 0)
				{
					throw new ArithmeticException("Negative exponent");
				}
				return BigInteger.One;
			}
			else
			{
				if (this.sign == 0)
				{
					return this;
				}
				if (!this.QuickPow2Check())
				{
					BigInteger bigInteger = BigInteger.One;
					BigInteger bigInteger2 = this;
					while (true)
					{
						if ((exp & 1) == 1)
						{
							bigInteger = bigInteger.Multiply(bigInteger2);
						}
						exp >>= 1;
						if (exp == 0)
						{
							break;
						}
						bigInteger2 = bigInteger2.Multiply(bigInteger2);
					}
					return bigInteger;
				}
				long num = (long)exp * (long)(this.BitLength - 1);
				if (num > 2147483647L)
				{
					throw new ArithmeticException("Result too large");
				}
				return BigInteger.One.ShiftLeft((int)num);
			}
		}

		public static BigInteger ProbablePrime(int bitLength, Random random)
		{
			return new BigInteger(bitLength, 100, random);
		}

		private int Remainder(int m)
		{
			long num = 0L;
			for (int i = 0; i < this.magnitude.Length; i++)
			{
				long num2 = (long)((ulong)this.magnitude[i]);
				num = (num << 32 | num2) % (long)m;
			}
			return (int)num;
		}

		private static int[] Remainder(int[] x, int[] y)
		{
			int num = 0;
			while (num < x.Length && x[num] == 0)
			{
				num++;
			}
			int num2 = 0;
			while (num2 < y.Length && y[num2] == 0)
			{
				num2++;
			}
			int num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
			if (num3 > 0)
			{
				int num4 = BigInteger.CalcBitLength(1, num2, y);
				int num5 = BigInteger.CalcBitLength(1, num, x);
				int num6 = num5 - num4;
				int num7 = 0;
				int num8 = num4;
				int[] array;
				if (num6 > 0)
				{
					array = BigInteger.ShiftLeft(y, num6);
					num8 += num6;
				}
				else
				{
					int num9 = y.Length - num2;
					array = new int[num9];
					Array.Copy(y, num2, array, 0, num9);
				}
				while (true)
				{
					if (num8 < num5 || BigInteger.CompareNoLeadingZeroes(num, x, num7, array) >= 0)
					{
						BigInteger.Subtract(num, x, num7, array);
						while (x[num] == 0)
						{
							if (++num == x.Length)
							{
								return x;
							}
						}
						num5 = 32 * (x.Length - num - 1) + BigInteger.BitLen(x[num]);
						if (num5 <= num4)
						{
							if (num5 < num4)
							{
								return x;
							}
							num3 = BigInteger.CompareNoLeadingZeroes(num, x, num2, y);
							if (num3 <= 0)
							{
								goto IL_14E;
							}
						}
					}
					num6 = num8 - num5;
					if (num6 == 1)
					{
						uint num10 = (uint)array[num7] >> 1;
						uint num11 = (uint)x[num];
						if (num10 > num11)
						{
							num6++;
						}
					}
					if (num6 < 2)
					{
						BigInteger.ShiftRightOneInPlace(num7, array);
						num8--;
					}
					else
					{
						BigInteger.ShiftRightInPlace(num7, array, num6);
						num8 -= num6;
					}
					while (array[num7] == 0)
					{
						num7++;
					}
				}
				return x;
			}
			IL_14E:
			if (num3 == 0)
			{
				Array.Clear(x, num, x.Length - num);
			}
			return x;
		}

		public BigInteger Remainder(BigInteger n)
		{
			if (n.sign == 0)
			{
				throw new ArithmeticException("Division by zero error");
			}
			if (this.sign == 0)
			{
				return BigInteger.Zero;
			}
			if (n.magnitude.Length == 1)
			{
				int num = n.magnitude[0];
				if (num > 0)
				{
					if (num == 1)
					{
						return BigInteger.Zero;
					}
					int num2 = this.Remainder(num);
					if (num2 != 0)
					{
						return new BigInteger(this.sign, new int[]
						{
							num2
						}, false);
					}
					return BigInteger.Zero;
				}
			}
			if (BigInteger.CompareNoLeadingZeroes(0, this.magnitude, 0, n.magnitude) < 0)
			{
				return this;
			}
			int[] array;
			if (n.QuickPow2Check())
			{
				array = this.LastNBits(n.Abs().BitLength - 1);
			}
			else
			{
				array = (int[])this.magnitude.Clone();
				array = BigInteger.Remainder(array, n.magnitude);
			}
			return new BigInteger(this.sign, array, true);
		}

		private int[] LastNBits(int n)
		{
			if (n < 1)
			{
				return BigInteger.ZeroMagnitude;
			}
			int num = (n + 32 - 1) / 32;
			num = Math.Min(num, this.magnitude.Length);
			int[] array = new int[num];
			Array.Copy(this.magnitude, this.magnitude.Length - num, array, 0, num);
			int num2 = (num << 5) - n;
			if (num2 > 0)
			{
				array[0] &= (int)(4294967295u >> num2);
			}
			return array;
		}

		private BigInteger DivideWords(int w)
		{
			int num = this.magnitude.Length;
			if (w >= num)
			{
				return BigInteger.Zero;
			}
			int[] array = new int[num - w];
			Array.Copy(this.magnitude, 0, array, 0, num - w);
			return new BigInteger(this.sign, array, false);
		}

		private BigInteger RemainderWords(int w)
		{
			int num = this.magnitude.Length;
			if (w >= num)
			{
				return this;
			}
			int[] array = new int[w];
			Array.Copy(this.magnitude, num - w, array, 0, w);
			return new BigInteger(this.sign, array, false);
		}

		private static int[] ShiftLeft(int[] mag, int n)
		{
			int num = (int)((uint)n >> 5);
			int num2 = n & 31;
			int num3 = mag.Length;
			int[] array;
			if (num2 == 0)
			{
				array = new int[num3 + num];
				mag.CopyTo(array, 0);
			}
			else
			{
				int num4 = 0;
				int num5 = 32 - num2;
				int num6 = (int)((uint)mag[0] >> num5);
				if (num6 != 0)
				{
					array = new int[num3 + num + 1];
					array[num4++] = num6;
				}
				else
				{
					array = new int[num3 + num];
				}
				int num7 = mag[0];
				for (int i = 0; i < num3 - 1; i++)
				{
					int num8 = mag[i + 1];
					array[num4++] = (num7 << num2 | (int)((uint)num8 >> num5));
					num7 = num8;
				}
				array[num4] = mag[num3 - 1] << num2;
			}
			return array;
		}

		private static int ShiftLeftOneInPlace(int[] x, int carry)
		{
			int num = x.Length;
			while (--num >= 0)
			{
				uint num2 = (uint)x[num];
				x[num] = (int)(num2 << 1 | (uint)carry);
				carry = (int)(num2 >> 31);
			}
			return carry;
		}

		public BigInteger ShiftLeft(int n)
		{
			if (this.sign == 0 || this.magnitude.Length == 0)
			{
				return BigInteger.Zero;
			}
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return this.ShiftRight(-n);
			}
			BigInteger bigInteger = new BigInteger(this.sign, BigInteger.ShiftLeft(this.magnitude, n), true);
			if (this.nBits != -1)
			{
				bigInteger.nBits = ((this.sign > 0) ? this.nBits : (this.nBits + n));
			}
			if (this.nBitLength != -1)
			{
				bigInteger.nBitLength = this.nBitLength + n;
			}
			return bigInteger;
		}

		private static void ShiftRightInPlace(int start, int[] mag, int n)
		{
			int num = (int)(((uint)n >> 5) + (uint)start);
			int num2 = n & 31;
			int num3 = mag.Length - 1;
			if (num != start)
			{
				int num4 = num - start;
				for (int i = num3; i >= num; i--)
				{
					mag[i] = mag[i - num4];
				}
				for (int j = num - 1; j >= start; j--)
				{
					mag[j] = 0;
				}
			}
			if (num2 != 0)
			{
				int num5 = 32 - num2;
				int num6 = mag[num3];
				for (int k = num3; k > num; k--)
				{
					int num7 = mag[k - 1];
					mag[k] = (int)((uint)num6 >> num2 | (uint)((uint)num7 << num5));
					num6 = num7;
				}
				mag[num] = (int)((uint)mag[num] >> num2);
			}
		}

		private static void ShiftRightOneInPlace(int start, int[] mag)
		{
			int num = mag.Length;
			int num2 = mag[num - 1];
			while (--num > start)
			{
				int num3 = mag[num - 1];
				mag[num] = (int)((uint)num2 >> 1 | (uint)((uint)num3 << 31));
				num2 = num3;
			}
			mag[start] = (int)((uint)mag[start] >> 1);
		}

		public BigInteger ShiftRight(int n)
		{
			if (n == 0)
			{
				return this;
			}
			if (n < 0)
			{
				return this.ShiftLeft(-n);
			}
			if (n < this.BitLength)
			{
				int num = this.BitLength - n + 31 >> 5;
				int[] array = new int[num];
				int num2 = n >> 5;
				int num3 = n & 31;
				if (num3 == 0)
				{
					Array.Copy(this.magnitude, 0, array, 0, array.Length);
				}
				else
				{
					int num4 = 32 - num3;
					int num5 = this.magnitude.Length - 1 - num2;
					for (int i = num - 1; i >= 0; i--)
					{
						array[i] = (int)((uint)this.magnitude[num5--] >> (num3 & 31));
						if (num5 >= 0)
						{
							array[i] |= this.magnitude[num5] << num4;
						}
					}
				}
				return new BigInteger(this.sign, array, false);
			}
			if (this.sign >= 0)
			{
				return BigInteger.Zero;
			}
			return BigInteger.One.Negate();
		}

		private static int[] Subtract(int xStart, int[] x, int yStart, int[] y)
		{
			int num = x.Length;
			int num2 = y.Length;
			int num3 = 0;
			do
			{
				long num4 = ((long)x[--num] & (long)((ulong)-1)) - ((long)y[--num2] & (long)((ulong)-1)) + (long)num3;
				x[num] = (int)num4;
				num3 = (int)(num4 >> 63);
			}
			while (num2 > yStart);
			if (num3 != 0)
			{
				while (--x[--num] == -1)
				{
				}
			}
			return x;
		}

		public BigInteger Subtract(BigInteger n)
		{
			if (n.sign == 0)
			{
				return this;
			}
			if (this.sign == 0)
			{
				return n.Negate();
			}
			if (this.sign != n.sign)
			{
				return this.Add(n.Negate());
			}
			int num = BigInteger.CompareNoLeadingZeroes(0, this.magnitude, 0, n.magnitude);
			if (num == 0)
			{
				return BigInteger.Zero;
			}
			BigInteger bigInteger;
			BigInteger bigInteger2;
			if (num < 0)
			{
				bigInteger = n;
				bigInteger2 = this;
			}
			else
			{
				bigInteger = this;
				bigInteger2 = n;
			}
			return new BigInteger(this.sign * num, BigInteger.doSubBigLil(bigInteger.magnitude, bigInteger2.magnitude), true);
		}

		private static int[] doSubBigLil(int[] bigMag, int[] lilMag)
		{
			int[] x = (int[])bigMag.Clone();
			return BigInteger.Subtract(0, x, 0, lilMag);
		}

		public byte[] ToByteArray()
		{
			return this.ToByteArray(false);
		}

		public byte[] ToByteArrayUnsigned()
		{
			return this.ToByteArray(true);
		}

		private byte[] ToByteArray(bool unsigned)
		{
			if (this.sign != 0)
			{
				int num = (unsigned && this.sign > 0) ? this.BitLength : (this.BitLength + 1);
				int byteLength = BigInteger.GetByteLength(num);
				byte[] array = new byte[byteLength];
				int i = this.magnitude.Length;
				int num2 = array.Length;
				if (this.sign > 0)
				{
					while (i > 1)
					{
						uint num3 = (uint)this.magnitude[--i];
						array[--num2] = (byte)num3;
						array[--num2] = (byte)(num3 >> 8);
						array[--num2] = (byte)(num3 >> 16);
						array[--num2] = (byte)(num3 >> 24);
					}
					uint num4;
					for (num4 = (uint)this.magnitude[0]; num4 > 255u; num4 >>= 8)
					{
						array[--num2] = (byte)num4;
					}
					array[num2 - 1] = (byte)num4;
				}
				else
				{
					bool flag = true;
					while (i > 1)
					{
						uint num5 = (uint)(~(uint)this.magnitude[--i]);
						if (flag)
						{
							flag = ((num5 += 1u) == 0u);
						}
						array[--num2] = (byte)num5;
						array[--num2] = (byte)(num5 >> 8);
						array[--num2] = (byte)(num5 >> 16);
						array[--num2] = (byte)(num5 >> 24);
					}
					uint num6 = (uint)this.magnitude[0];
					if (flag)
					{
						num6 -= 1u;
					}
					while (num6 > 255u)
					{
						array[--num2] = (byte)(~(byte)num6);
						num6 >>= 8;
					}
					array[--num2] = (byte)(~(byte)num6);
					if (num2 > 0)
					{
						array[num2 - 1] = 255;
					}
				}
				return array;
			}
			if (!unsigned)
			{
				return new byte[1];
			}
			return BigInteger.ZeroEncoding;
		}

		public override string ToString()
		{
			return this.ToString(10);
		}

		public string ToString(int radix)
		{
			if (radix != 2)
			{
				switch (radix)
				{
				case 8:
				case 10:
					goto IL_2E;
				case 9:
					break;
				default:
					if (radix == 16)
					{
						goto IL_2E;
					}
					break;
				}
				throw new FormatException("Only bases 2, 8, 10, 16 are allowed");
			}
			IL_2E:
			if (this.magnitude == null)
			{
				return "null";
			}
			if (this.sign == 0)
			{
				return "0";
			}
			int num = 0;
			while (num < this.magnitude.Length && this.magnitude[num] == 0)
			{
				num++;
			}
			if (num == this.magnitude.Length)
			{
				return "0";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (this.sign == -1)
			{
				stringBuilder.Append('-');
			}
			if (radix != 2)
			{
				switch (radix)
				{
				case 8:
				{
					int num2 = 1073741823;
					BigInteger bigInteger = this.Abs();
					int i = bigInteger.BitLength;
					IList list = Platform.CreateArrayList();
					while (i > 30)
					{
						list.Add(Convert.ToString(bigInteger.IntValue & num2, 8));
						bigInteger = bigInteger.ShiftRight(30);
						i -= 30;
					}
					stringBuilder.Append(Convert.ToString(bigInteger.IntValue, 8));
					for (int j = list.Count - 1; j >= 0; j--)
					{
						BigInteger.AppendZeroExtendedString(stringBuilder, (string)list[j], 10);
					}
					break;
				}
				case 9:
					break;
				case 10:
				{
					BigInteger bigInteger2 = this.Abs();
					if (bigInteger2.BitLength < 64)
					{
						stringBuilder.Append(Convert.ToString(bigInteger2.LongValue, radix));
					}
					else
					{
						long num3 = 9223372036854775807L / (long)radix;
						long num4 = (long)radix;
						int num5 = 1;
						while (num4 <= num3)
						{
							num4 *= (long)radix;
							num5++;
						}
						BigInteger bigInteger3 = BigInteger.ValueOf(num4);
						IList list2 = Platform.CreateArrayList();
						while (bigInteger2.CompareTo(bigInteger3) >= 0)
						{
							BigInteger[] array = bigInteger2.DivideAndRemainder(bigInteger3);
							list2.Add(Convert.ToString(array[1].LongValue, radix));
							bigInteger2 = array[0];
						}
						stringBuilder.Append(Convert.ToString(bigInteger2.LongValue, radix));
						for (int k = list2.Count - 1; k >= 0; k--)
						{
							BigInteger.AppendZeroExtendedString(stringBuilder, (string)list2[k], num5);
						}
					}
					break;
				}
				default:
					if (radix == 16)
					{
						int num6 = num;
						stringBuilder.Append(Convert.ToString(this.magnitude[num6], 16));
						while (++num6 < this.magnitude.Length)
						{
							BigInteger.AppendZeroExtendedString(stringBuilder, Convert.ToString(this.magnitude[num6], 16), 8);
						}
					}
					break;
				}
			}
			else
			{
				int num7 = num;
				stringBuilder.Append(Convert.ToString(this.magnitude[num7], 2));
				while (++num7 < this.magnitude.Length)
				{
					BigInteger.AppendZeroExtendedString(stringBuilder, Convert.ToString(this.magnitude[num7], 2), 32);
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendZeroExtendedString(StringBuilder sb, string s, int minLength)
		{
			for (int i = s.Length; i < minLength; i++)
			{
				sb.Append('0');
			}
			sb.Append(s);
		}

		private static BigInteger CreateUValueOf(ulong value)
		{
			int num = (int)(value >> 32);
			int num2 = (int)value;
			if (num != 0)
			{
				return new BigInteger(1, new int[]
				{
					num,
					num2
				}, false);
			}
			if (num2 != 0)
			{
				BigInteger bigInteger = new BigInteger(1, new int[]
				{
					num2
				}, false);
				if ((num2 & -num2) == num2)
				{
					bigInteger.nBits = 1;
				}
				return bigInteger;
			}
			return BigInteger.Zero;
		}

		private static BigInteger CreateValueOf(long value)
		{
			if (value >= 0L)
			{
				return BigInteger.CreateUValueOf((ulong)value);
			}
			if (value == -9223372036854775808L)
			{
				return BigInteger.CreateValueOf(~value).Not();
			}
			return BigInteger.CreateValueOf(-value).Negate();
		}

		public static BigInteger ValueOf(long value)
		{
			if (value >= 0L && value < (long)BigInteger.SMALL_CONSTANTS.Length)
			{
				return BigInteger.SMALL_CONSTANTS[(int)(checked((IntPtr)value))];
			}
			return BigInteger.CreateValueOf(value);
		}

		public int GetLowestSetBit()
		{
			if (this.sign == 0)
			{
				return -1;
			}
			return this.GetLowestSetBitMaskFirst(-1);
		}

		private int GetLowestSetBitMaskFirst(int firstWordMask)
		{
			int num = this.magnitude.Length;
			int num2 = 0;
			uint num3 = (uint)(this.magnitude[--num] & firstWordMask);
			while (num3 == 0u)
			{
				num3 = (uint)this.magnitude[--num];
				num2 += 32;
			}
			while ((num3 & 255u) == 0u)
			{
				num3 >>= 8;
				num2 += 8;
			}
			while ((num3 & 1u) == 0u)
			{
				num3 >>= 1;
				num2++;
			}
			return num2;
		}

		public bool TestBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit position must not be negative");
			}
			if (this.sign < 0)
			{
				return !this.Not().TestBit(n);
			}
			int num = n / 32;
			if (num >= this.magnitude.Length)
			{
				return false;
			}
			int num2 = this.magnitude[this.magnitude.Length - 1 - num];
			return (num2 >> n % 32 & 1) > 0;
		}

		public BigInteger Or(BigInteger value)
		{
			if (this.sign == 0)
			{
				return value;
			}
			if (value.sign == 0)
			{
				return this;
			}
			int[] array = (this.sign > 0) ? this.magnitude : this.Add(BigInteger.One).magnitude;
			int[] array2 = (value.sign > 0) ? value.magnitude : value.Add(BigInteger.One).magnitude;
			bool flag = this.sign < 0 || value.sign < 0;
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 | num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger Xor(BigInteger value)
		{
			if (this.sign == 0)
			{
				return value;
			}
			if (value.sign == 0)
			{
				return this;
			}
			int[] array = (this.sign > 0) ? this.magnitude : this.Add(BigInteger.One).magnitude;
			int[] array2 = (value.sign > 0) ? value.magnitude : value.Add(BigInteger.One).magnitude;
			bool flag = (this.sign < 0 && value.sign >= 0) || (this.sign >= 0 && value.sign < 0);
			int num = Math.Max(array.Length, array2.Length);
			int[] array3 = new int[num];
			int num2 = array3.Length - array.Length;
			int num3 = array3.Length - array2.Length;
			for (int i = 0; i < array3.Length; i++)
			{
				int num4 = (i >= num2) ? array[i - num2] : 0;
				int num5 = (i >= num3) ? array2[i - num3] : 0;
				if (this.sign < 0)
				{
					num4 = ~num4;
				}
				if (value.sign < 0)
				{
					num5 = ~num5;
				}
				array3[i] = (num4 ^ num5);
				if (flag)
				{
					array3[i] = ~array3[i];
				}
			}
			BigInteger bigInteger = new BigInteger(1, array3, true);
			if (flag)
			{
				bigInteger = bigInteger.Not();
			}
			return bigInteger;
		}

		public BigInteger SetBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (this.TestBit(n))
			{
				return this;
			}
			if (this.sign > 0 && n < this.BitLength - 1)
			{
				return this.FlipExistingBit(n);
			}
			return this.Or(BigInteger.One.ShiftLeft(n));
		}

		public BigInteger ClearBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (!this.TestBit(n))
			{
				return this;
			}
			if (this.sign > 0 && n < this.BitLength - 1)
			{
				return this.FlipExistingBit(n);
			}
			return this.AndNot(BigInteger.One.ShiftLeft(n));
		}

		public BigInteger FlipBit(int n)
		{
			if (n < 0)
			{
				throw new ArithmeticException("Bit address less than zero");
			}
			if (this.sign > 0 && n < this.BitLength - 1)
			{
				return this.FlipExistingBit(n);
			}
			return this.Xor(BigInteger.One.ShiftLeft(n));
		}

		private BigInteger FlipExistingBit(int n)
		{
			int[] array = (int[])this.magnitude.Clone();
			array[array.Length - 1 - (n >> 5)] ^= 1 << n;
			return new BigInteger(this.sign, array, false);
		}
	}
}
