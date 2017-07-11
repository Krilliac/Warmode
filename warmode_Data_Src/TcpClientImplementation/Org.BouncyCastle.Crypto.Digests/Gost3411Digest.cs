using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Gost3411Digest : IDigest, IMemoable
	{
		private const int DIGEST_LENGTH = 32;

		private byte[] H = new byte[32];

		private byte[] L = new byte[32];

		private byte[] M = new byte[32];

		private byte[] Sum = new byte[32];

		private byte[][] C = Gost3411Digest.MakeC();

		private byte[] xBuf = new byte[32];

		private int xBufOff;

		private ulong byteCount;

		private readonly IBlockCipher cipher = new Gost28147Engine();

		private byte[] sBox;

		private byte[] K = new byte[32];

		private byte[] a = new byte[8];

		internal short[] wS = new short[16];

		internal short[] w_S = new short[16];

		internal byte[] S = new byte[32];

		internal byte[] U = new byte[32];

		internal byte[] V = new byte[32];

		internal byte[] W = new byte[32];

		private static readonly byte[] C2 = new byte[]
		{
			0,
			255,
			0,
			255,
			0,
			255,
			0,
			255,
			255,
			0,
			255,
			0,
			255,
			0,
			255,
			0,
			0,
			255,
			255,
			0,
			255,
			0,
			0,
			255,
			255,
			0,
			0,
			0,
			255,
			255,
			0,
			255
		};

		public string AlgorithmName
		{
			get
			{
				return "Gost3411";
			}
		}

		private static byte[][] MakeC()
		{
			byte[][] array = new byte[4][];
			for (int i = 0; i < 4; i++)
			{
				array[i] = new byte[32];
			}
			return array;
		}

		public Gost3411Digest()
		{
			this.sBox = Gost28147Engine.GetSBox("D-A");
			this.cipher.Init(true, new ParametersWithSBox(null, this.sBox));
			this.Reset();
		}

		public Gost3411Digest(byte[] sBoxParam)
		{
			this.sBox = Arrays.Clone(sBoxParam);
			this.cipher.Init(true, new ParametersWithSBox(null, this.sBox));
			this.Reset();
		}

		public Gost3411Digest(Gost3411Digest t)
		{
			this.Reset(t);
		}

		public int GetDigestSize()
		{
			return 32;
		}

		public void Update(byte input)
		{
			this.xBuf[this.xBufOff++] = input;
			if (this.xBufOff == this.xBuf.Length)
			{
				this.sumByteArray(this.xBuf);
				this.processBlock(this.xBuf, 0);
				this.xBufOff = 0;
			}
			this.byteCount += 1uL;
		}

		public void BlockUpdate(byte[] input, int inOff, int length)
		{
			while (this.xBufOff != 0)
			{
				if (length <= 0)
				{
					break;
				}
				this.Update(input[inOff]);
				inOff++;
				length--;
			}
			while (length > this.xBuf.Length)
			{
				Array.Copy(input, inOff, this.xBuf, 0, this.xBuf.Length);
				this.sumByteArray(this.xBuf);
				this.processBlock(this.xBuf, 0);
				inOff += this.xBuf.Length;
				length -= this.xBuf.Length;
				this.byteCount += (ulong)this.xBuf.Length;
			}
			while (length > 0)
			{
				this.Update(input[inOff]);
				inOff++;
				length--;
			}
		}

		private byte[] P(byte[] input)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				this.K[num++] = input[i];
				this.K[num++] = input[8 + i];
				this.K[num++] = input[16 + i];
				this.K[num++] = input[24 + i];
			}
			return this.K;
		}

		private byte[] A(byte[] input)
		{
			for (int i = 0; i < 8; i++)
			{
				this.a[i] = (input[i] ^ input[i + 8]);
			}
			Array.Copy(input, 8, input, 0, 24);
			Array.Copy(this.a, 0, input, 24, 8);
			return input;
		}

		private void E(byte[] key, byte[] s, int sOff, byte[] input, int inOff)
		{
			this.cipher.Init(true, new KeyParameter(key));
			this.cipher.ProcessBlock(input, inOff, s, sOff);
		}

		private void fw(byte[] input)
		{
			Gost3411Digest.cpyBytesToShort(input, this.wS);
			this.w_S[15] = (this.wS[0] ^ this.wS[1] ^ this.wS[2] ^ this.wS[3] ^ this.wS[12] ^ this.wS[15]);
			Array.Copy(this.wS, 1, this.w_S, 0, 15);
			Gost3411Digest.cpyShortToBytes(this.w_S, input);
		}

		private void processBlock(byte[] input, int inOff)
		{
			Array.Copy(input, inOff, this.M, 0, 32);
			this.H.CopyTo(this.U, 0);
			this.M.CopyTo(this.V, 0);
			for (int i = 0; i < 32; i++)
			{
				this.W[i] = (this.U[i] ^ this.V[i]);
			}
			this.E(this.P(this.W), this.S, 0, this.H, 0);
			for (int j = 1; j < 4; j++)
			{
				byte[] array = this.A(this.U);
				for (int k = 0; k < 32; k++)
				{
					this.U[k] = (array[k] ^ this.C[j][k]);
				}
				this.V = this.A(this.A(this.V));
				for (int l = 0; l < 32; l++)
				{
					this.W[l] = (this.U[l] ^ this.V[l]);
				}
				this.E(this.P(this.W), this.S, j * 8, this.H, j * 8);
			}
			for (int m = 0; m < 12; m++)
			{
				this.fw(this.S);
			}
			for (int n = 0; n < 32; n++)
			{
				this.S[n] = (this.S[n] ^ this.M[n]);
			}
			this.fw(this.S);
			for (int num = 0; num < 32; num++)
			{
				this.S[num] = (this.H[num] ^ this.S[num]);
			}
			for (int num2 = 0; num2 < 61; num2++)
			{
				this.fw(this.S);
			}
			Array.Copy(this.S, 0, this.H, 0, this.H.Length);
		}

		private void finish()
		{
			ulong n = this.byteCount * 8uL;
			Pack.UInt64_To_LE(n, this.L);
			while (this.xBufOff != 0)
			{
				this.Update(0);
			}
			this.processBlock(this.L, 0);
			this.processBlock(this.Sum, 0);
		}

		public int DoFinal(byte[] output, int outOff)
		{
			this.finish();
			this.H.CopyTo(output, outOff);
			this.Reset();
			return 32;
		}

		public void Reset()
		{
			this.byteCount = 0uL;
			this.xBufOff = 0;
			Array.Clear(this.H, 0, this.H.Length);
			Array.Clear(this.L, 0, this.L.Length);
			Array.Clear(this.M, 0, this.M.Length);
			Array.Clear(this.C[1], 0, this.C[1].Length);
			Array.Clear(this.C[3], 0, this.C[3].Length);
			Array.Clear(this.Sum, 0, this.Sum.Length);
			Array.Clear(this.xBuf, 0, this.xBuf.Length);
			Gost3411Digest.C2.CopyTo(this.C[2], 0);
		}

		private void sumByteArray(byte[] input)
		{
			int num = 0;
			for (int num2 = 0; num2 != this.Sum.Length; num2++)
			{
				int num3 = (int)((this.Sum[num2] & 255) + (input[num2] & 255)) + num;
				this.Sum[num2] = (byte)num3;
				num = num3 >> 8;
			}
		}

		private static void cpyBytesToShort(byte[] S, short[] wS)
		{
			for (int i = 0; i < S.Length / 2; i++)
			{
				wS[i] = (short)(((int)S[i * 2 + 1] << 8 & 65280) | (int)(S[i * 2] & 255));
			}
		}

		private static void cpyShortToBytes(short[] wS, byte[] S)
		{
			for (int i = 0; i < S.Length / 2; i++)
			{
				S[i * 2 + 1] = (byte)(wS[i] >> 8);
				S[i * 2] = (byte)wS[i];
			}
		}

		public int GetByteLength()
		{
			return 32;
		}

		public IMemoable Copy()
		{
			return new Gost3411Digest(this);
		}

		public void Reset(IMemoable other)
		{
			Gost3411Digest gost3411Digest = (Gost3411Digest)other;
			this.sBox = gost3411Digest.sBox;
			this.cipher.Init(true, new ParametersWithSBox(null, this.sBox));
			this.Reset();
			Array.Copy(gost3411Digest.H, 0, this.H, 0, gost3411Digest.H.Length);
			Array.Copy(gost3411Digest.L, 0, this.L, 0, gost3411Digest.L.Length);
			Array.Copy(gost3411Digest.M, 0, this.M, 0, gost3411Digest.M.Length);
			Array.Copy(gost3411Digest.Sum, 0, this.Sum, 0, gost3411Digest.Sum.Length);
			Array.Copy(gost3411Digest.C[1], 0, this.C[1], 0, gost3411Digest.C[1].Length);
			Array.Copy(gost3411Digest.C[2], 0, this.C[2], 0, gost3411Digest.C[2].Length);
			Array.Copy(gost3411Digest.C[3], 0, this.C[3], 0, gost3411Digest.C[3].Length);
			Array.Copy(gost3411Digest.xBuf, 0, this.xBuf, 0, gost3411Digest.xBuf.Length);
			this.xBufOff = gost3411Digest.xBufOff;
			this.byteCount = gost3411Digest.byteCount;
		}
	}
}
