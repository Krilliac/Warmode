using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class ThreefishEngine : IBlockCipher
	{
		private abstract class ThreefishCipher
		{
			protected readonly ulong[] t;

			protected readonly ulong[] kw;

			protected ThreefishCipher(ulong[] kw, ulong[] t)
			{
				this.kw = kw;
				this.t = t;
			}

			internal abstract void EncryptBlock(ulong[] block, ulong[] outWords);

			internal abstract void DecryptBlock(ulong[] block, ulong[] outWords);
		}

		private sealed class Threefish256Cipher : ThreefishEngine.ThreefishCipher
		{
			private const int ROTATION_0_0 = 14;

			private const int ROTATION_0_1 = 16;

			private const int ROTATION_1_0 = 52;

			private const int ROTATION_1_1 = 57;

			private const int ROTATION_2_0 = 23;

			private const int ROTATION_2_1 = 40;

			private const int ROTATION_3_0 = 5;

			private const int ROTATION_3_1 = 37;

			private const int ROTATION_4_0 = 25;

			private const int ROTATION_4_1 = 33;

			private const int ROTATION_5_0 = 46;

			private const int ROTATION_5_1 = 12;

			private const int ROTATION_6_0 = 58;

			private const int ROTATION_6_1 = 22;

			private const int ROTATION_7_0 = 32;

			private const int ROTATION_7_1 = 32;

			public Threefish256Cipher(ulong[] kw, ulong[] t) : base(kw, t)
			{
			}

			internal override void EncryptBlock(ulong[] block, ulong[] outWords)
			{
				ulong[] kw = this.kw;
				ulong[] t = this.t;
				int[] mOD = ThreefishEngine.MOD5;
				int[] mOD2 = ThreefishEngine.MOD3;
				if (kw.Length != 9)
				{
					throw new ArgumentException();
				}
				if (t.Length != 5)
				{
					throw new ArgumentException();
				}
				ulong num = block[0];
				ulong num2 = block[1];
				ulong num3 = block[2];
				ulong num4 = block[3];
				num += kw[0];
				num2 += kw[1] + t[0];
				num3 += kw[2] + t[1];
				num4 += kw[3];
				for (int i = 1; i < 18; i += 2)
				{
					int num5 = mOD[i];
					int num6 = mOD2[i];
					num2 = ThreefishEngine.RotlXor(num2, 14, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 16, num3 += num4);
					num4 = ThreefishEngine.RotlXor(num4, 52, num += num4);
					num2 = ThreefishEngine.RotlXor(num2, 57, num3 += num2);
					num2 = ThreefishEngine.RotlXor(num2, 23, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 40, num3 += num4);
					num4 = ThreefishEngine.RotlXor(num4, 5, num += num4);
					num2 = ThreefishEngine.RotlXor(num2, 37, num3 += num2);
					num += kw[num5];
					num2 += kw[num5 + 1] + t[num6];
					num3 += kw[num5 + 2] + t[num6 + 1];
					num4 += kw[num5 + 3] + (ulong)i;
					num2 = ThreefishEngine.RotlXor(num2, 25, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 33, num3 += num4);
					num4 = ThreefishEngine.RotlXor(num4, 46, num += num4);
					num2 = ThreefishEngine.RotlXor(num2, 12, num3 += num2);
					num2 = ThreefishEngine.RotlXor(num2, 58, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 22, num3 += num4);
					num4 = ThreefishEngine.RotlXor(num4, 32, num += num4);
					num2 = ThreefishEngine.RotlXor(num2, 32, num3 += num2);
					num += kw[num5 + 1];
					num2 += kw[num5 + 2] + t[num6 + 1];
					num3 += kw[num5 + 3] + t[num6 + 2];
					num4 += kw[num5 + 4] + (ulong)i + 1uL;
				}
				outWords[0] = num;
				outWords[1] = num2;
				outWords[2] = num3;
				outWords[3] = num4;
			}

			internal override void DecryptBlock(ulong[] block, ulong[] state)
			{
				ulong[] kw = this.kw;
				ulong[] t = this.t;
				int[] mOD = ThreefishEngine.MOD5;
				int[] mOD2 = ThreefishEngine.MOD3;
				if (kw.Length != 9)
				{
					throw new ArgumentException();
				}
				if (t.Length != 5)
				{
					throw new ArgumentException();
				}
				ulong num = block[0];
				ulong num2 = block[1];
				ulong num3 = block[2];
				ulong num4 = block[3];
				for (int i = 17; i >= 1; i -= 2)
				{
					int num5 = mOD[i];
					int num6 = mOD2[i];
					num -= kw[num5 + 1];
					num2 -= kw[num5 + 2] + t[num6 + 1];
					num3 -= kw[num5 + 3] + t[num6 + 2];
					num4 -= kw[num5 + 4] + (ulong)i + 1uL;
					num4 = ThreefishEngine.XorRotr(num4, 32, num);
					num -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 32, num3);
					num3 -= num2;
					num2 = ThreefishEngine.XorRotr(num2, 58, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 22, num3);
					num3 -= num4;
					num4 = ThreefishEngine.XorRotr(num4, 46, num);
					num -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 12, num3);
					num3 -= num2;
					num2 = ThreefishEngine.XorRotr(num2, 25, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 33, num3);
					num3 -= num4;
					num -= kw[num5];
					num2 -= kw[num5 + 1] + t[num6];
					num3 -= kw[num5 + 2] + t[num6 + 1];
					num4 -= kw[num5 + 3] + (ulong)i;
					num4 = ThreefishEngine.XorRotr(num4, 5, num);
					num -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 37, num3);
					num3 -= num2;
					num2 = ThreefishEngine.XorRotr(num2, 23, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 40, num3);
					num3 -= num4;
					num4 = ThreefishEngine.XorRotr(num4, 52, num);
					num -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 57, num3);
					num3 -= num2;
					num2 = ThreefishEngine.XorRotr(num2, 14, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 16, num3);
					num3 -= num4;
				}
				num -= kw[0];
				num2 -= kw[1] + t[0];
				num3 -= kw[2] + t[1];
				num4 -= kw[3];
				state[0] = num;
				state[1] = num2;
				state[2] = num3;
				state[3] = num4;
			}
		}

		private sealed class Threefish512Cipher : ThreefishEngine.ThreefishCipher
		{
			private const int ROTATION_0_0 = 46;

			private const int ROTATION_0_1 = 36;

			private const int ROTATION_0_2 = 19;

			private const int ROTATION_0_3 = 37;

			private const int ROTATION_1_0 = 33;

			private const int ROTATION_1_1 = 27;

			private const int ROTATION_1_2 = 14;

			private const int ROTATION_1_3 = 42;

			private const int ROTATION_2_0 = 17;

			private const int ROTATION_2_1 = 49;

			private const int ROTATION_2_2 = 36;

			private const int ROTATION_2_3 = 39;

			private const int ROTATION_3_0 = 44;

			private const int ROTATION_3_1 = 9;

			private const int ROTATION_3_2 = 54;

			private const int ROTATION_3_3 = 56;

			private const int ROTATION_4_0 = 39;

			private const int ROTATION_4_1 = 30;

			private const int ROTATION_4_2 = 34;

			private const int ROTATION_4_3 = 24;

			private const int ROTATION_5_0 = 13;

			private const int ROTATION_5_1 = 50;

			private const int ROTATION_5_2 = 10;

			private const int ROTATION_5_3 = 17;

			private const int ROTATION_6_0 = 25;

			private const int ROTATION_6_1 = 29;

			private const int ROTATION_6_2 = 39;

			private const int ROTATION_6_3 = 43;

			private const int ROTATION_7_0 = 8;

			private const int ROTATION_7_1 = 35;

			private const int ROTATION_7_2 = 56;

			private const int ROTATION_7_3 = 22;

			internal Threefish512Cipher(ulong[] kw, ulong[] t) : base(kw, t)
			{
			}

			internal override void EncryptBlock(ulong[] block, ulong[] outWords)
			{
				ulong[] kw = this.kw;
				ulong[] t = this.t;
				int[] mOD = ThreefishEngine.MOD9;
				int[] mOD2 = ThreefishEngine.MOD3;
				if (kw.Length != 17)
				{
					throw new ArgumentException();
				}
				if (t.Length != 5)
				{
					throw new ArgumentException();
				}
				ulong num = block[0];
				ulong num2 = block[1];
				ulong num3 = block[2];
				ulong num4 = block[3];
				ulong num5 = block[4];
				ulong num6 = block[5];
				ulong num7 = block[6];
				ulong num8 = block[7];
				num += kw[0];
				num2 += kw[1];
				num3 += kw[2];
				num4 += kw[3];
				num5 += kw[4];
				num6 += kw[5] + t[0];
				num7 += kw[6] + t[1];
				num8 += kw[7];
				for (int i = 1; i < 18; i += 2)
				{
					int num9 = mOD[i];
					int num10 = mOD2[i];
					num2 = ThreefishEngine.RotlXor(num2, 46, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 36, num3 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 19, num5 += num6);
					num8 = ThreefishEngine.RotlXor(num8, 37, num7 += num8);
					num2 = ThreefishEngine.RotlXor(num2, 33, num3 += num2);
					num8 = ThreefishEngine.RotlXor(num8, 27, num5 += num8);
					num6 = ThreefishEngine.RotlXor(num6, 14, num7 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 42, num += num4);
					num2 = ThreefishEngine.RotlXor(num2, 17, num5 += num2);
					num4 = ThreefishEngine.RotlXor(num4, 49, num7 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 36, num += num6);
					num8 = ThreefishEngine.RotlXor(num8, 39, num3 += num8);
					num2 = ThreefishEngine.RotlXor(num2, 44, num7 += num2);
					num8 = ThreefishEngine.RotlXor(num8, 9, num += num8);
					num6 = ThreefishEngine.RotlXor(num6, 54, num3 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 56, num5 += num4);
					num += kw[num9];
					num2 += kw[num9 + 1];
					num3 += kw[num9 + 2];
					num4 += kw[num9 + 3];
					num5 += kw[num9 + 4];
					num6 += kw[num9 + 5] + t[num10];
					num7 += kw[num9 + 6] + t[num10 + 1];
					num8 += kw[num9 + 7] + (ulong)i;
					num2 = ThreefishEngine.RotlXor(num2, 39, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 30, num3 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 34, num5 += num6);
					num8 = ThreefishEngine.RotlXor(num8, 24, num7 += num8);
					num2 = ThreefishEngine.RotlXor(num2, 13, num3 += num2);
					num8 = ThreefishEngine.RotlXor(num8, 50, num5 += num8);
					num6 = ThreefishEngine.RotlXor(num6, 10, num7 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 17, num += num4);
					num2 = ThreefishEngine.RotlXor(num2, 25, num5 += num2);
					num4 = ThreefishEngine.RotlXor(num4, 29, num7 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 39, num += num6);
					num8 = ThreefishEngine.RotlXor(num8, 43, num3 += num8);
					num2 = ThreefishEngine.RotlXor(num2, 8, num7 += num2);
					num8 = ThreefishEngine.RotlXor(num8, 35, num += num8);
					num6 = ThreefishEngine.RotlXor(num6, 56, num3 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 22, num5 += num4);
					num += kw[num9 + 1];
					num2 += kw[num9 + 2];
					num3 += kw[num9 + 3];
					num4 += kw[num9 + 4];
					num5 += kw[num9 + 5];
					num6 += kw[num9 + 6] + t[num10 + 1];
					num7 += kw[num9 + 7] + t[num10 + 2];
					num8 += kw[num9 + 8] + (ulong)i + 1uL;
				}
				outWords[0] = num;
				outWords[1] = num2;
				outWords[2] = num3;
				outWords[3] = num4;
				outWords[4] = num5;
				outWords[5] = num6;
				outWords[6] = num7;
				outWords[7] = num8;
			}

			internal override void DecryptBlock(ulong[] block, ulong[] state)
			{
				ulong[] kw = this.kw;
				ulong[] t = this.t;
				int[] mOD = ThreefishEngine.MOD9;
				int[] mOD2 = ThreefishEngine.MOD3;
				if (kw.Length != 17)
				{
					throw new ArgumentException();
				}
				if (t.Length != 5)
				{
					throw new ArgumentException();
				}
				ulong num = block[0];
				ulong num2 = block[1];
				ulong num3 = block[2];
				ulong num4 = block[3];
				ulong num5 = block[4];
				ulong num6 = block[5];
				ulong num7 = block[6];
				ulong num8 = block[7];
				for (int i = 17; i >= 1; i -= 2)
				{
					int num9 = mOD[i];
					int num10 = mOD2[i];
					num -= kw[num9 + 1];
					num2 -= kw[num9 + 2];
					num3 -= kw[num9 + 3];
					num4 -= kw[num9 + 4];
					num5 -= kw[num9 + 5];
					num6 -= kw[num9 + 6] + t[num10 + 1];
					num7 -= kw[num9 + 7] + t[num10 + 2];
					num8 -= kw[num9 + 8] + (ulong)i + 1uL;
					num2 = ThreefishEngine.XorRotr(num2, 8, num7);
					num7 -= num2;
					num8 = ThreefishEngine.XorRotr(num8, 35, num);
					num -= num8;
					num6 = ThreefishEngine.XorRotr(num6, 56, num3);
					num3 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 22, num5);
					num5 -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 25, num5);
					num5 -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 29, num7);
					num7 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 39, num);
					num -= num6;
					num8 = ThreefishEngine.XorRotr(num8, 43, num3);
					num3 -= num8;
					num2 = ThreefishEngine.XorRotr(num2, 13, num3);
					num3 -= num2;
					num8 = ThreefishEngine.XorRotr(num8, 50, num5);
					num5 -= num8;
					num6 = ThreefishEngine.XorRotr(num6, 10, num7);
					num7 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 17, num);
					num -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 39, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 30, num3);
					num3 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 34, num5);
					num5 -= num6;
					num8 = ThreefishEngine.XorRotr(num8, 24, num7);
					num7 -= num8;
					num -= kw[num9];
					num2 -= kw[num9 + 1];
					num3 -= kw[num9 + 2];
					num4 -= kw[num9 + 3];
					num5 -= kw[num9 + 4];
					num6 -= kw[num9 + 5] + t[num10];
					num7 -= kw[num9 + 6] + t[num10 + 1];
					num8 -= kw[num9 + 7] + (ulong)i;
					num2 = ThreefishEngine.XorRotr(num2, 44, num7);
					num7 -= num2;
					num8 = ThreefishEngine.XorRotr(num8, 9, num);
					num -= num8;
					num6 = ThreefishEngine.XorRotr(num6, 54, num3);
					num3 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 56, num5);
					num5 -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 17, num5);
					num5 -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 49, num7);
					num7 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 36, num);
					num -= num6;
					num8 = ThreefishEngine.XorRotr(num8, 39, num3);
					num3 -= num8;
					num2 = ThreefishEngine.XorRotr(num2, 33, num3);
					num3 -= num2;
					num8 = ThreefishEngine.XorRotr(num8, 27, num5);
					num5 -= num8;
					num6 = ThreefishEngine.XorRotr(num6, 14, num7);
					num7 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 42, num);
					num -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 46, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 36, num3);
					num3 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 19, num5);
					num5 -= num6;
					num8 = ThreefishEngine.XorRotr(num8, 37, num7);
					num7 -= num8;
				}
				num -= kw[0];
				num2 -= kw[1];
				num3 -= kw[2];
				num4 -= kw[3];
				num5 -= kw[4];
				num6 -= kw[5] + t[0];
				num7 -= kw[6] + t[1];
				num8 -= kw[7];
				state[0] = num;
				state[1] = num2;
				state[2] = num3;
				state[3] = num4;
				state[4] = num5;
				state[5] = num6;
				state[6] = num7;
				state[7] = num8;
			}
		}

		private sealed class Threefish1024Cipher : ThreefishEngine.ThreefishCipher
		{
			private const int ROTATION_0_0 = 24;

			private const int ROTATION_0_1 = 13;

			private const int ROTATION_0_2 = 8;

			private const int ROTATION_0_3 = 47;

			private const int ROTATION_0_4 = 8;

			private const int ROTATION_0_5 = 17;

			private const int ROTATION_0_6 = 22;

			private const int ROTATION_0_7 = 37;

			private const int ROTATION_1_0 = 38;

			private const int ROTATION_1_1 = 19;

			private const int ROTATION_1_2 = 10;

			private const int ROTATION_1_3 = 55;

			private const int ROTATION_1_4 = 49;

			private const int ROTATION_1_5 = 18;

			private const int ROTATION_1_6 = 23;

			private const int ROTATION_1_7 = 52;

			private const int ROTATION_2_0 = 33;

			private const int ROTATION_2_1 = 4;

			private const int ROTATION_2_2 = 51;

			private const int ROTATION_2_3 = 13;

			private const int ROTATION_2_4 = 34;

			private const int ROTATION_2_5 = 41;

			private const int ROTATION_2_6 = 59;

			private const int ROTATION_2_7 = 17;

			private const int ROTATION_3_0 = 5;

			private const int ROTATION_3_1 = 20;

			private const int ROTATION_3_2 = 48;

			private const int ROTATION_3_3 = 41;

			private const int ROTATION_3_4 = 47;

			private const int ROTATION_3_5 = 28;

			private const int ROTATION_3_6 = 16;

			private const int ROTATION_3_7 = 25;

			private const int ROTATION_4_0 = 41;

			private const int ROTATION_4_1 = 9;

			private const int ROTATION_4_2 = 37;

			private const int ROTATION_4_3 = 31;

			private const int ROTATION_4_4 = 12;

			private const int ROTATION_4_5 = 47;

			private const int ROTATION_4_6 = 44;

			private const int ROTATION_4_7 = 30;

			private const int ROTATION_5_0 = 16;

			private const int ROTATION_5_1 = 34;

			private const int ROTATION_5_2 = 56;

			private const int ROTATION_5_3 = 51;

			private const int ROTATION_5_4 = 4;

			private const int ROTATION_5_5 = 53;

			private const int ROTATION_5_6 = 42;

			private const int ROTATION_5_7 = 41;

			private const int ROTATION_6_0 = 31;

			private const int ROTATION_6_1 = 44;

			private const int ROTATION_6_2 = 47;

			private const int ROTATION_6_3 = 46;

			private const int ROTATION_6_4 = 19;

			private const int ROTATION_6_5 = 42;

			private const int ROTATION_6_6 = 44;

			private const int ROTATION_6_7 = 25;

			private const int ROTATION_7_0 = 9;

			private const int ROTATION_7_1 = 48;

			private const int ROTATION_7_2 = 35;

			private const int ROTATION_7_3 = 52;

			private const int ROTATION_7_4 = 23;

			private const int ROTATION_7_5 = 31;

			private const int ROTATION_7_6 = 37;

			private const int ROTATION_7_7 = 20;

			public Threefish1024Cipher(ulong[] kw, ulong[] t) : base(kw, t)
			{
			}

			internal override void EncryptBlock(ulong[] block, ulong[] outWords)
			{
				ulong[] kw = this.kw;
				ulong[] t = this.t;
				int[] mOD = ThreefishEngine.MOD17;
				int[] mOD2 = ThreefishEngine.MOD3;
				if (kw.Length != 33)
				{
					throw new ArgumentException();
				}
				if (t.Length != 5)
				{
					throw new ArgumentException();
				}
				ulong num = block[0];
				ulong num2 = block[1];
				ulong num3 = block[2];
				ulong num4 = block[3];
				ulong num5 = block[4];
				ulong num6 = block[5];
				ulong num7 = block[6];
				ulong num8 = block[7];
				ulong num9 = block[8];
				ulong num10 = block[9];
				ulong num11 = block[10];
				ulong num12 = block[11];
				ulong num13 = block[12];
				ulong num14 = block[13];
				ulong num15 = block[14];
				ulong num16 = block[15];
				num += kw[0];
				num2 += kw[1];
				num3 += kw[2];
				num4 += kw[3];
				num5 += kw[4];
				num6 += kw[5];
				num7 += kw[6];
				num8 += kw[7];
				num9 += kw[8];
				num10 += kw[9];
				num11 += kw[10];
				num12 += kw[11];
				num13 += kw[12];
				num14 += kw[13] + t[0];
				num15 += kw[14] + t[1];
				num16 += kw[15];
				for (int i = 1; i < 20; i += 2)
				{
					int num17 = mOD[i];
					int num18 = mOD2[i];
					num2 = ThreefishEngine.RotlXor(num2, 24, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 13, num3 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 8, num5 += num6);
					num8 = ThreefishEngine.RotlXor(num8, 47, num7 += num8);
					num10 = ThreefishEngine.RotlXor(num10, 8, num9 += num10);
					num12 = ThreefishEngine.RotlXor(num12, 17, num11 += num12);
					num14 = ThreefishEngine.RotlXor(num14, 22, num13 += num14);
					num16 = ThreefishEngine.RotlXor(num16, 37, num15 += num16);
					num10 = ThreefishEngine.RotlXor(num10, 38, num += num10);
					num14 = ThreefishEngine.RotlXor(num14, 19, num3 += num14);
					num12 = ThreefishEngine.RotlXor(num12, 10, num7 += num12);
					num16 = ThreefishEngine.RotlXor(num16, 55, num5 += num16);
					num8 = ThreefishEngine.RotlXor(num8, 49, num11 += num8);
					num4 = ThreefishEngine.RotlXor(num4, 18, num13 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 23, num15 += num6);
					num2 = ThreefishEngine.RotlXor(num2, 52, num9 += num2);
					num8 = ThreefishEngine.RotlXor(num8, 33, num += num8);
					num6 = ThreefishEngine.RotlXor(num6, 4, num3 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 51, num5 += num4);
					num2 = ThreefishEngine.RotlXor(num2, 13, num7 += num2);
					num16 = ThreefishEngine.RotlXor(num16, 34, num13 += num16);
					num14 = ThreefishEngine.RotlXor(num14, 41, num15 += num14);
					num12 = ThreefishEngine.RotlXor(num12, 59, num9 += num12);
					num10 = ThreefishEngine.RotlXor(num10, 17, num11 += num10);
					num16 = ThreefishEngine.RotlXor(num16, 5, num += num16);
					num12 = ThreefishEngine.RotlXor(num12, 20, num3 += num12);
					num14 = ThreefishEngine.RotlXor(num14, 48, num7 += num14);
					num10 = ThreefishEngine.RotlXor(num10, 41, num5 += num10);
					num2 = ThreefishEngine.RotlXor(num2, 47, num15 += num2);
					num6 = ThreefishEngine.RotlXor(num6, 28, num9 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 16, num11 += num4);
					num8 = ThreefishEngine.RotlXor(num8, 25, num13 += num8);
					num += kw[num17];
					num2 += kw[num17 + 1];
					num3 += kw[num17 + 2];
					num4 += kw[num17 + 3];
					num5 += kw[num17 + 4];
					num6 += kw[num17 + 5];
					num7 += kw[num17 + 6];
					num8 += kw[num17 + 7];
					num9 += kw[num17 + 8];
					num10 += kw[num17 + 9];
					num11 += kw[num17 + 10];
					num12 += kw[num17 + 11];
					num13 += kw[num17 + 12];
					num14 += kw[num17 + 13] + t[num18];
					num15 += kw[num17 + 14] + t[num18 + 1];
					num16 += kw[num17 + 15] + (ulong)i;
					num2 = ThreefishEngine.RotlXor(num2, 41, num += num2);
					num4 = ThreefishEngine.RotlXor(num4, 9, num3 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 37, num5 += num6);
					num8 = ThreefishEngine.RotlXor(num8, 31, num7 += num8);
					num10 = ThreefishEngine.RotlXor(num10, 12, num9 += num10);
					num12 = ThreefishEngine.RotlXor(num12, 47, num11 += num12);
					num14 = ThreefishEngine.RotlXor(num14, 44, num13 += num14);
					num16 = ThreefishEngine.RotlXor(num16, 30, num15 += num16);
					num10 = ThreefishEngine.RotlXor(num10, 16, num += num10);
					num14 = ThreefishEngine.RotlXor(num14, 34, num3 += num14);
					num12 = ThreefishEngine.RotlXor(num12, 56, num7 += num12);
					num16 = ThreefishEngine.RotlXor(num16, 51, num5 += num16);
					num8 = ThreefishEngine.RotlXor(num8, 4, num11 += num8);
					num4 = ThreefishEngine.RotlXor(num4, 53, num13 += num4);
					num6 = ThreefishEngine.RotlXor(num6, 42, num15 += num6);
					num2 = ThreefishEngine.RotlXor(num2, 41, num9 += num2);
					num8 = ThreefishEngine.RotlXor(num8, 31, num += num8);
					num6 = ThreefishEngine.RotlXor(num6, 44, num3 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 47, num5 += num4);
					num2 = ThreefishEngine.RotlXor(num2, 46, num7 += num2);
					num16 = ThreefishEngine.RotlXor(num16, 19, num13 += num16);
					num14 = ThreefishEngine.RotlXor(num14, 42, num15 += num14);
					num12 = ThreefishEngine.RotlXor(num12, 44, num9 += num12);
					num10 = ThreefishEngine.RotlXor(num10, 25, num11 += num10);
					num16 = ThreefishEngine.RotlXor(num16, 9, num += num16);
					num12 = ThreefishEngine.RotlXor(num12, 48, num3 += num12);
					num14 = ThreefishEngine.RotlXor(num14, 35, num7 += num14);
					num10 = ThreefishEngine.RotlXor(num10, 52, num5 += num10);
					num2 = ThreefishEngine.RotlXor(num2, 23, num15 += num2);
					num6 = ThreefishEngine.RotlXor(num6, 31, num9 += num6);
					num4 = ThreefishEngine.RotlXor(num4, 37, num11 += num4);
					num8 = ThreefishEngine.RotlXor(num8, 20, num13 += num8);
					num += kw[num17 + 1];
					num2 += kw[num17 + 2];
					num3 += kw[num17 + 3];
					num4 += kw[num17 + 4];
					num5 += kw[num17 + 5];
					num6 += kw[num17 + 6];
					num7 += kw[num17 + 7];
					num8 += kw[num17 + 8];
					num9 += kw[num17 + 9];
					num10 += kw[num17 + 10];
					num11 += kw[num17 + 11];
					num12 += kw[num17 + 12];
					num13 += kw[num17 + 13];
					num14 += kw[num17 + 14] + t[num18 + 1];
					num15 += kw[num17 + 15] + t[num18 + 2];
					num16 += kw[num17 + 16] + (ulong)i + 1uL;
				}
				outWords[0] = num;
				outWords[1] = num2;
				outWords[2] = num3;
				outWords[3] = num4;
				outWords[4] = num5;
				outWords[5] = num6;
				outWords[6] = num7;
				outWords[7] = num8;
				outWords[8] = num9;
				outWords[9] = num10;
				outWords[10] = num11;
				outWords[11] = num12;
				outWords[12] = num13;
				outWords[13] = num14;
				outWords[14] = num15;
				outWords[15] = num16;
			}

			internal override void DecryptBlock(ulong[] block, ulong[] state)
			{
				ulong[] kw = this.kw;
				ulong[] t = this.t;
				int[] mOD = ThreefishEngine.MOD17;
				int[] mOD2 = ThreefishEngine.MOD3;
				if (kw.Length != 33)
				{
					throw new ArgumentException();
				}
				if (t.Length != 5)
				{
					throw new ArgumentException();
				}
				ulong num = block[0];
				ulong num2 = block[1];
				ulong num3 = block[2];
				ulong num4 = block[3];
				ulong num5 = block[4];
				ulong num6 = block[5];
				ulong num7 = block[6];
				ulong num8 = block[7];
				ulong num9 = block[8];
				ulong num10 = block[9];
				ulong num11 = block[10];
				ulong num12 = block[11];
				ulong num13 = block[12];
				ulong num14 = block[13];
				ulong num15 = block[14];
				ulong num16 = block[15];
				for (int i = 19; i >= 1; i -= 2)
				{
					int num17 = mOD[i];
					int num18 = mOD2[i];
					num -= kw[num17 + 1];
					num2 -= kw[num17 + 2];
					num3 -= kw[num17 + 3];
					num4 -= kw[num17 + 4];
					num5 -= kw[num17 + 5];
					num6 -= kw[num17 + 6];
					num7 -= kw[num17 + 7];
					num8 -= kw[num17 + 8];
					num9 -= kw[num17 + 9];
					num10 -= kw[num17 + 10];
					num11 -= kw[num17 + 11];
					num12 -= kw[num17 + 12];
					num13 -= kw[num17 + 13];
					num14 -= kw[num17 + 14] + t[num18 + 1];
					num15 -= kw[num17 + 15] + t[num18 + 2];
					num16 -= kw[num17 + 16] + (ulong)i + 1uL;
					num16 = ThreefishEngine.XorRotr(num16, 9, num);
					num -= num16;
					num12 = ThreefishEngine.XorRotr(num12, 48, num3);
					num3 -= num12;
					num14 = ThreefishEngine.XorRotr(num14, 35, num7);
					num7 -= num14;
					num10 = ThreefishEngine.XorRotr(num10, 52, num5);
					num5 -= num10;
					num2 = ThreefishEngine.XorRotr(num2, 23, num15);
					num15 -= num2;
					num6 = ThreefishEngine.XorRotr(num6, 31, num9);
					num9 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 37, num11);
					num11 -= num4;
					num8 = ThreefishEngine.XorRotr(num8, 20, num13);
					num13 -= num8;
					num8 = ThreefishEngine.XorRotr(num8, 31, num);
					num -= num8;
					num6 = ThreefishEngine.XorRotr(num6, 44, num3);
					num3 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 47, num5);
					num5 -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 46, num7);
					num7 -= num2;
					num16 = ThreefishEngine.XorRotr(num16, 19, num13);
					num13 -= num16;
					num14 = ThreefishEngine.XorRotr(num14, 42, num15);
					num15 -= num14;
					num12 = ThreefishEngine.XorRotr(num12, 44, num9);
					num9 -= num12;
					num10 = ThreefishEngine.XorRotr(num10, 25, num11);
					num11 -= num10;
					num10 = ThreefishEngine.XorRotr(num10, 16, num);
					num -= num10;
					num14 = ThreefishEngine.XorRotr(num14, 34, num3);
					num3 -= num14;
					num12 = ThreefishEngine.XorRotr(num12, 56, num7);
					num7 -= num12;
					num16 = ThreefishEngine.XorRotr(num16, 51, num5);
					num5 -= num16;
					num8 = ThreefishEngine.XorRotr(num8, 4, num11);
					num11 -= num8;
					num4 = ThreefishEngine.XorRotr(num4, 53, num13);
					num13 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 42, num15);
					num15 -= num6;
					num2 = ThreefishEngine.XorRotr(num2, 41, num9);
					num9 -= num2;
					num2 = ThreefishEngine.XorRotr(num2, 41, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 9, num3);
					num3 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 37, num5);
					num5 -= num6;
					num8 = ThreefishEngine.XorRotr(num8, 31, num7);
					num7 -= num8;
					num10 = ThreefishEngine.XorRotr(num10, 12, num9);
					num9 -= num10;
					num12 = ThreefishEngine.XorRotr(num12, 47, num11);
					num11 -= num12;
					num14 = ThreefishEngine.XorRotr(num14, 44, num13);
					num13 -= num14;
					num16 = ThreefishEngine.XorRotr(num16, 30, num15);
					num15 -= num16;
					num -= kw[num17];
					num2 -= kw[num17 + 1];
					num3 -= kw[num17 + 2];
					num4 -= kw[num17 + 3];
					num5 -= kw[num17 + 4];
					num6 -= kw[num17 + 5];
					num7 -= kw[num17 + 6];
					num8 -= kw[num17 + 7];
					num9 -= kw[num17 + 8];
					num10 -= kw[num17 + 9];
					num11 -= kw[num17 + 10];
					num12 -= kw[num17 + 11];
					num13 -= kw[num17 + 12];
					num14 -= kw[num17 + 13] + t[num18];
					num15 -= kw[num17 + 14] + t[num18 + 1];
					num16 -= kw[num17 + 15] + (ulong)i;
					num16 = ThreefishEngine.XorRotr(num16, 5, num);
					num -= num16;
					num12 = ThreefishEngine.XorRotr(num12, 20, num3);
					num3 -= num12;
					num14 = ThreefishEngine.XorRotr(num14, 48, num7);
					num7 -= num14;
					num10 = ThreefishEngine.XorRotr(num10, 41, num5);
					num5 -= num10;
					num2 = ThreefishEngine.XorRotr(num2, 47, num15);
					num15 -= num2;
					num6 = ThreefishEngine.XorRotr(num6, 28, num9);
					num9 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 16, num11);
					num11 -= num4;
					num8 = ThreefishEngine.XorRotr(num8, 25, num13);
					num13 -= num8;
					num8 = ThreefishEngine.XorRotr(num8, 33, num);
					num -= num8;
					num6 = ThreefishEngine.XorRotr(num6, 4, num3);
					num3 -= num6;
					num4 = ThreefishEngine.XorRotr(num4, 51, num5);
					num5 -= num4;
					num2 = ThreefishEngine.XorRotr(num2, 13, num7);
					num7 -= num2;
					num16 = ThreefishEngine.XorRotr(num16, 34, num13);
					num13 -= num16;
					num14 = ThreefishEngine.XorRotr(num14, 41, num15);
					num15 -= num14;
					num12 = ThreefishEngine.XorRotr(num12, 59, num9);
					num9 -= num12;
					num10 = ThreefishEngine.XorRotr(num10, 17, num11);
					num11 -= num10;
					num10 = ThreefishEngine.XorRotr(num10, 38, num);
					num -= num10;
					num14 = ThreefishEngine.XorRotr(num14, 19, num3);
					num3 -= num14;
					num12 = ThreefishEngine.XorRotr(num12, 10, num7);
					num7 -= num12;
					num16 = ThreefishEngine.XorRotr(num16, 55, num5);
					num5 -= num16;
					num8 = ThreefishEngine.XorRotr(num8, 49, num11);
					num11 -= num8;
					num4 = ThreefishEngine.XorRotr(num4, 18, num13);
					num13 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 23, num15);
					num15 -= num6;
					num2 = ThreefishEngine.XorRotr(num2, 52, num9);
					num9 -= num2;
					num2 = ThreefishEngine.XorRotr(num2, 24, num);
					num -= num2;
					num4 = ThreefishEngine.XorRotr(num4, 13, num3);
					num3 -= num4;
					num6 = ThreefishEngine.XorRotr(num6, 8, num5);
					num5 -= num6;
					num8 = ThreefishEngine.XorRotr(num8, 47, num7);
					num7 -= num8;
					num10 = ThreefishEngine.XorRotr(num10, 8, num9);
					num9 -= num10;
					num12 = ThreefishEngine.XorRotr(num12, 17, num11);
					num11 -= num12;
					num14 = ThreefishEngine.XorRotr(num14, 22, num13);
					num13 -= num14;
					num16 = ThreefishEngine.XorRotr(num16, 37, num15);
					num15 -= num16;
				}
				num -= kw[0];
				num2 -= kw[1];
				num3 -= kw[2];
				num4 -= kw[3];
				num5 -= kw[4];
				num6 -= kw[5];
				num7 -= kw[6];
				num8 -= kw[7];
				num9 -= kw[8];
				num10 -= kw[9];
				num11 -= kw[10];
				num12 -= kw[11];
				num13 -= kw[12];
				num14 -= kw[13] + t[0];
				num15 -= kw[14] + t[1];
				num16 -= kw[15];
				state[0] = num;
				state[1] = num2;
				state[2] = num3;
				state[3] = num4;
				state[4] = num5;
				state[5] = num6;
				state[6] = num7;
				state[7] = num8;
				state[8] = num9;
				state[9] = num10;
				state[10] = num11;
				state[11] = num12;
				state[12] = num13;
				state[13] = num14;
				state[14] = num15;
				state[15] = num16;
			}
		}

		public const int BLOCKSIZE_256 = 256;

		public const int BLOCKSIZE_512 = 512;

		public const int BLOCKSIZE_1024 = 1024;

		private const int TWEAK_SIZE_BYTES = 16;

		private const int TWEAK_SIZE_WORDS = 2;

		private const int ROUNDS_256 = 72;

		private const int ROUNDS_512 = 72;

		private const int ROUNDS_1024 = 80;

		private const int MAX_ROUNDS = 80;

		private const ulong C_240 = 2004413935125273122uL;

		private static readonly int[] MOD9;

		private static readonly int[] MOD17;

		private static readonly int[] MOD5;

		private static readonly int[] MOD3;

		private readonly int blocksizeBytes;

		private readonly int blocksizeWords;

		private readonly ulong[] currentBlock;

		private readonly ulong[] t = new ulong[5];

		private readonly ulong[] kw;

		private readonly ThreefishEngine.ThreefishCipher cipher;

		private bool forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "Threefish-" + this.blocksizeBytes * 8;
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		static ThreefishEngine()
		{
			ThreefishEngine.MOD9 = new int[80];
			ThreefishEngine.MOD17 = new int[ThreefishEngine.MOD9.Length];
			ThreefishEngine.MOD5 = new int[ThreefishEngine.MOD9.Length];
			ThreefishEngine.MOD3 = new int[ThreefishEngine.MOD9.Length];
			for (int i = 0; i < ThreefishEngine.MOD9.Length; i++)
			{
				ThreefishEngine.MOD17[i] = i % 17;
				ThreefishEngine.MOD9[i] = i % 9;
				ThreefishEngine.MOD5[i] = i % 5;
				ThreefishEngine.MOD3[i] = i % 3;
			}
		}

		public ThreefishEngine(int blocksizeBits)
		{
			this.blocksizeBytes = blocksizeBits / 8;
			this.blocksizeWords = this.blocksizeBytes / 8;
			this.currentBlock = new ulong[this.blocksizeWords];
			this.kw = new ulong[2 * this.blocksizeWords + 1];
			if (blocksizeBits == 256)
			{
				this.cipher = new ThreefishEngine.Threefish256Cipher(this.kw, this.t);
				return;
			}
			if (blocksizeBits == 512)
			{
				this.cipher = new ThreefishEngine.Threefish512Cipher(this.kw, this.t);
				return;
			}
			if (blocksizeBits != 1024)
			{
				throw new ArgumentException("Invalid blocksize - Threefish is defined with block size of 256, 512, or 1024 bits");
			}
			this.cipher = new ThreefishEngine.Threefish1024Cipher(this.kw, this.t);
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			byte[] key;
			byte[] array;
			if (parameters is TweakableBlockCipherParameters)
			{
				TweakableBlockCipherParameters tweakableBlockCipherParameters = (TweakableBlockCipherParameters)parameters;
				key = tweakableBlockCipherParameters.Key.GetKey();
				array = tweakableBlockCipherParameters.Tweak;
			}
			else
			{
				if (!(parameters is KeyParameter))
				{
					throw new ArgumentException("Invalid parameter passed to Threefish init - " + parameters.GetType().Name);
				}
				key = ((KeyParameter)parameters).GetKey();
				array = null;
			}
			ulong[] array2 = null;
			ulong[] tweak = null;
			if (key != null)
			{
				if (key.Length != this.blocksizeBytes)
				{
					throw new ArgumentException("Threefish key must be same size as block (" + this.blocksizeBytes + " bytes)");
				}
				array2 = new ulong[this.blocksizeWords];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = ThreefishEngine.BytesToWord(key, i * 8);
				}
			}
			if (array != null)
			{
				if (array.Length != 16)
				{
					throw new ArgumentException("Threefish tweak must be " + 16 + " bytes");
				}
				tweak = new ulong[]
				{
					ThreefishEngine.BytesToWord(array, 0),
					ThreefishEngine.BytesToWord(array, 8)
				};
			}
			this.Init(forEncryption, array2, tweak);
		}

		internal void Init(bool forEncryption, ulong[] key, ulong[] tweak)
		{
			this.forEncryption = forEncryption;
			if (key != null)
			{
				this.SetKey(key);
			}
			if (tweak != null)
			{
				this.SetTweak(tweak);
			}
		}

		private void SetKey(ulong[] key)
		{
			if (key.Length != this.blocksizeWords)
			{
				throw new ArgumentException("Threefish key must be same size as block (" + this.blocksizeWords + " words)");
			}
			ulong num = 2004413935125273122uL;
			for (int i = 0; i < this.blocksizeWords; i++)
			{
				this.kw[i] = key[i];
				num ^= this.kw[i];
			}
			this.kw[this.blocksizeWords] = num;
			Array.Copy(this.kw, 0, this.kw, this.blocksizeWords + 1, this.blocksizeWords);
		}

		private void SetTweak(ulong[] tweak)
		{
			if (tweak.Length != 2)
			{
				throw new ArgumentException("Tweak must be " + 2 + " words.");
			}
			this.t[0] = tweak[0];
			this.t[1] = tweak[1];
			this.t[2] = (this.t[0] ^ this.t[1]);
			this.t[3] = this.t[0];
			this.t[4] = this.t[1];
		}

		public virtual int GetBlockSize()
		{
			return this.blocksizeBytes;
		}

		public virtual void Reset()
		{
		}

		public virtual int ProcessBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
		{
			if (outOff + this.blocksizeBytes > outBytes.Length)
			{
				throw new DataLengthException("Output buffer too short");
			}
			if (inOff + this.blocksizeBytes > inBytes.Length)
			{
				throw new DataLengthException("Input buffer too short");
			}
			for (int i = 0; i < this.blocksizeBytes; i += 8)
			{
				this.currentBlock[i >> 3] = ThreefishEngine.BytesToWord(inBytes, inOff + i);
			}
			this.ProcessBlock(this.currentBlock, this.currentBlock);
			for (int j = 0; j < this.blocksizeBytes; j += 8)
			{
				ThreefishEngine.WordToBytes(this.currentBlock[j >> 3], outBytes, outOff + j);
			}
			return this.blocksizeBytes;
		}

		internal int ProcessBlock(ulong[] inWords, ulong[] outWords)
		{
			if (this.kw[this.blocksizeWords] == 0uL)
			{
				throw new InvalidOperationException("Threefish engine not initialised");
			}
			if (inWords.Length != this.blocksizeWords)
			{
				throw new DataLengthException("Input buffer too short");
			}
			if (outWords.Length != this.blocksizeWords)
			{
				throw new DataLengthException("Output buffer too short");
			}
			if (this.forEncryption)
			{
				this.cipher.EncryptBlock(inWords, outWords);
			}
			else
			{
				this.cipher.DecryptBlock(inWords, outWords);
			}
			return this.blocksizeWords;
		}

		internal static ulong BytesToWord(byte[] bytes, int off)
		{
			if (off + 8 > bytes.Length)
			{
				throw new ArgumentException();
			}
			int num = off + 1;
			ulong num2 = (ulong)bytes[off] & 255uL;
			num2 |= ((ulong)bytes[num++] & 255uL) << 8;
			num2 |= ((ulong)bytes[num++] & 255uL) << 16;
			num2 |= ((ulong)bytes[num++] & 255uL) << 24;
			num2 |= ((ulong)bytes[num++] & 255uL) << 32;
			num2 |= ((ulong)bytes[num++] & 255uL) << 40;
			num2 |= ((ulong)bytes[num++] & 255uL) << 48;
			return num2 | ((ulong)bytes[num++] & 255uL) << 56;
		}

		internal static void WordToBytes(ulong word, byte[] bytes, int off)
		{
			if (off + 8 > bytes.Length)
			{
				throw new ArgumentException();
			}
			int num = off + 1;
			bytes[off] = (byte)word;
			bytes[num++] = (byte)(word >> 8);
			bytes[num++] = (byte)(word >> 16);
			bytes[num++] = (byte)(word >> 24);
			bytes[num++] = (byte)(word >> 32);
			bytes[num++] = (byte)(word >> 40);
			bytes[num++] = (byte)(word >> 48);
			bytes[num++] = (byte)(word >> 56);
		}

		private static ulong RotlXor(ulong x, int n, ulong xor)
		{
			return (x << n | x >> 64 - n) ^ xor;
		}

		private static ulong XorRotr(ulong x, int n, ulong xor)
		{
			ulong num = x ^ xor;
			return num >> n | num << 64 - n;
		}
	}
}
