using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Digests
{
	public class Sha3Digest : IDigest, IMemoable
	{
		private static readonly ulong[] KeccakRoundConstants = Sha3Digest.KeccakInitializeRoundConstants();

		private static readonly int[] KeccakRhoOffsets = Sha3Digest.KeccakInitializeRhoOffsets();

		private byte[] state = new byte[200];

		private byte[] dataQueue = new byte[192];

		private int rate;

		private int bitsInQueue;

		private int fixedOutputLength;

		private bool squeezing;

		private int bitsAvailableForSqueezing;

		private byte[] chunk;

		private byte[] oneByte;

		private ulong[] C = new ulong[5];

		private ulong[] tempA = new ulong[25];

		private ulong[] chiC = new ulong[5];

		public virtual string AlgorithmName
		{
			get
			{
				return "SHA3-" + this.fixedOutputLength;
			}
		}

		private static ulong[] KeccakInitializeRoundConstants()
		{
			ulong[] array = new ulong[24];
			byte b = 1;
			for (int i = 0; i < 24; i++)
			{
				array[i] = 0uL;
				for (int j = 0; j < 7; j++)
				{
					int num = (1 << j) - 1;
					bool flag = (b & 1) != 0;
					if (flag)
					{
						array[i] ^= 1uL << num;
					}
					bool flag2 = (b & 128) != 0;
					b = (byte)(b << 1);
					if (flag2)
					{
						b ^= 113;
					}
				}
			}
			return array;
		}

		private static int[] KeccakInitializeRhoOffsets()
		{
			int[] array = new int[25];
			int num = 0;
			array[0] = num;
			int num2 = 1;
			int num3 = 0;
			for (int i = 1; i < 25; i++)
			{
				num = (num + i & 63);
				array[num2 % 5 + 5 * (num3 % 5)] = num;
				int num4 = num3 % 5;
				int num5 = (2 * num2 + 3 * num3) % 5;
				num2 = num4;
				num3 = num5;
			}
			return array;
		}

		private void ClearDataQueueSection(int off, int len)
		{
			for (int num = off; num != off + len; num++)
			{
				this.dataQueue[num] = 0;
			}
		}

		public Sha3Digest()
		{
			this.Init(0);
		}

		public Sha3Digest(int bitLength)
		{
			this.Init(bitLength);
		}

		public Sha3Digest(Sha3Digest source)
		{
			this.CopyIn(source);
		}

		private void CopyIn(Sha3Digest source)
		{
			Array.Copy(source.state, 0, this.state, 0, source.state.Length);
			Array.Copy(source.dataQueue, 0, this.dataQueue, 0, source.dataQueue.Length);
			this.rate = source.rate;
			this.bitsInQueue = source.bitsInQueue;
			this.fixedOutputLength = source.fixedOutputLength;
			this.squeezing = source.squeezing;
			this.bitsAvailableForSqueezing = source.bitsAvailableForSqueezing;
			this.chunk = Arrays.Clone(source.chunk);
			this.oneByte = Arrays.Clone(source.oneByte);
		}

		public virtual int GetDigestSize()
		{
			return this.fixedOutputLength / 8;
		}

		public virtual void Update(byte input)
		{
			this.oneByte[0] = input;
			this.DoUpdate(this.oneByte, 0, 8L);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			this.DoUpdate(input, inOff, (long)len * 8L);
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			this.Squeeze(output, outOff, (long)this.fixedOutputLength);
			this.Reset();
			return this.GetDigestSize();
		}

		public virtual void Reset()
		{
			this.Init(this.fixedOutputLength);
		}

		public virtual int GetByteLength()
		{
			return this.rate / 8;
		}

		private void Init(int bitLength)
		{
			if (bitLength <= 256)
			{
				if (bitLength != 0)
				{
					if (bitLength == 224)
					{
						this.InitSponge(1152, 448);
						return;
					}
					if (bitLength != 256)
					{
						goto IL_8F;
					}
					this.InitSponge(1088, 512);
					return;
				}
			}
			else if (bitLength != 288)
			{
				if (bitLength == 384)
				{
					this.InitSponge(832, 768);
					return;
				}
				if (bitLength != 512)
				{
					goto IL_8F;
				}
				this.InitSponge(576, 1024);
				return;
			}
			this.InitSponge(1024, 576);
			return;
			IL_8F:
			throw new ArgumentException("must be one of 224, 256, 384, or 512.", "bitLength");
		}

		private void DoUpdate(byte[] data, int off, long databitlen)
		{
			if (databitlen % 8L == 0L)
			{
				this.Absorb(data, off, databitlen);
				return;
			}
			this.Absorb(data, off, databitlen - databitlen % 8L);
			this.Absorb(new byte[]
			{
				(byte)(data[off + (int)(databitlen / 8L)] >> (int)(8L - databitlen % 8L))
			}, off, databitlen % 8L);
		}

		private void InitSponge(int rate, int capacity)
		{
			if (rate + capacity != 1600)
			{
				throw new InvalidOperationException("rate + capacity != 1600");
			}
			if (rate <= 0 || rate >= 1600 || rate % 64 != 0)
			{
				throw new InvalidOperationException("invalid rate value");
			}
			this.rate = rate;
			this.fixedOutputLength = 0;
			Arrays.Fill(this.state, 0);
			Arrays.Fill(this.dataQueue, 0);
			this.bitsInQueue = 0;
			this.squeezing = false;
			this.bitsAvailableForSqueezing = 0;
			this.fixedOutputLength = capacity / 2;
			this.chunk = new byte[rate / 8];
			this.oneByte = new byte[1];
		}

		private void AbsorbQueue()
		{
			this.KeccakAbsorb(this.state, this.dataQueue, this.rate / 8);
			this.bitsInQueue = 0;
		}

		private void Absorb(byte[] data, int off, long databitlen)
		{
			if (this.bitsInQueue % 8 != 0)
			{
				throw new InvalidOperationException("attempt to absorb with odd length queue.");
			}
			if (this.squeezing)
			{
				throw new InvalidOperationException("attempt to absorb while squeezing.");
			}
			long num = 0L;
			while (num < databitlen)
			{
				if (this.bitsInQueue == 0 && databitlen >= (long)this.rate && num <= databitlen - (long)this.rate)
				{
					long num2 = (databitlen - num) / (long)this.rate;
					for (long num3 = 0L; num3 < num2; num3 += 1L)
					{
						Array.Copy(data, (int)((long)off + num / 8L + num3 * (long)this.chunk.Length), this.chunk, 0, this.chunk.Length);
						this.KeccakAbsorb(this.state, this.chunk, this.chunk.Length);
					}
					num += num2 * (long)this.rate;
				}
				else
				{
					int num4 = (int)(databitlen - num);
					if (num4 + this.bitsInQueue > this.rate)
					{
						num4 = this.rate - this.bitsInQueue;
					}
					int num5 = num4 % 8;
					num4 -= num5;
					Array.Copy(data, off + (int)(num / 8L), this.dataQueue, this.bitsInQueue / 8, num4 / 8);
					this.bitsInQueue += num4;
					num += (long)num4;
					if (this.bitsInQueue == this.rate)
					{
						this.AbsorbQueue();
					}
					if (num5 > 0)
					{
						int num6 = (1 << num5) - 1;
						this.dataQueue[this.bitsInQueue / 8] = (byte)((int)data[off + (int)(num / 8L)] & num6);
						this.bitsInQueue += num5;
						num += (long)num5;
					}
				}
			}
		}

		private void PadAndSwitchToSqueezingPhase()
		{
			if (this.bitsInQueue + 1 == this.rate)
			{
				byte[] expr_23_cp_0 = this.dataQueue;
				int expr_23_cp_1 = this.bitsInQueue / 8;
				expr_23_cp_0[expr_23_cp_1] |= (byte)(1 << this.bitsInQueue % 8);
				this.AbsorbQueue();
				this.ClearDataQueueSection(0, this.rate / 8);
			}
			else
			{
				this.ClearDataQueueSection((this.bitsInQueue + 7) / 8, this.rate / 8 - (this.bitsInQueue + 7) / 8);
				byte[] expr_8B_cp_0 = this.dataQueue;
				int expr_8B_cp_1 = this.bitsInQueue / 8;
				expr_8B_cp_0[expr_8B_cp_1] |= (byte)(1 << this.bitsInQueue % 8);
			}
			byte[] expr_BB_cp_0 = this.dataQueue;
			int expr_BB_cp_1 = (this.rate - 1) / 8;
			expr_BB_cp_0[expr_BB_cp_1] |= (byte)(1 << (this.rate - 1) % 8);
			this.AbsorbQueue();
			if (this.rate == 1024)
			{
				this.KeccakExtract1024bits(this.state, this.dataQueue);
				this.bitsAvailableForSqueezing = 1024;
			}
			else
			{
				this.KeccakExtract(this.state, this.dataQueue, this.rate / 64);
				this.bitsAvailableForSqueezing = this.rate;
			}
			this.squeezing = true;
		}

		private void Squeeze(byte[] output, int offset, long outputLength)
		{
			if (!this.squeezing)
			{
				this.PadAndSwitchToSqueezingPhase();
			}
			if (outputLength % 8L != 0L)
			{
				throw new InvalidOperationException("outputLength not a multiple of 8");
			}
			int num2;
			for (long num = 0L; num < outputLength; num += (long)num2)
			{
				if (this.bitsAvailableForSqueezing == 0)
				{
					this.KeccakPermutation(this.state);
					if (this.rate == 1024)
					{
						this.KeccakExtract1024bits(this.state, this.dataQueue);
						this.bitsAvailableForSqueezing = 1024;
					}
					else
					{
						this.KeccakExtract(this.state, this.dataQueue, this.rate / 64);
						this.bitsAvailableForSqueezing = this.rate;
					}
				}
				num2 = this.bitsAvailableForSqueezing;
				if ((long)num2 > outputLength - num)
				{
					num2 = (int)(outputLength - num);
				}
				Array.Copy(this.dataQueue, (this.rate - this.bitsAvailableForSqueezing) / 8, output, offset + (int)(num / 8L), num2 / 8);
				this.bitsAvailableForSqueezing -= num2;
			}
		}

		private static void FromBytesToWords(ulong[] stateAsWords, byte[] state)
		{
			for (int i = 0; i < 25; i++)
			{
				stateAsWords[i] = 0uL;
				int num = i * 8;
				for (int j = 0; j < 8; j++)
				{
					stateAsWords[i] |= ((ulong)state[num + j] & 255uL) << 8 * j;
				}
			}
		}

		private static void FromWordsToBytes(byte[] state, ulong[] stateAsWords)
		{
			for (int i = 0; i < 25; i++)
			{
				int num = i * 8;
				for (int j = 0; j < 8; j++)
				{
					state[num + j] = (byte)(stateAsWords[i] >> 8 * j);
				}
			}
		}

		private void KeccakPermutation(byte[] state)
		{
			ulong[] stateAsWords = new ulong[state.Length / 8];
			Sha3Digest.FromBytesToWords(stateAsWords, state);
			this.KeccakPermutationOnWords(stateAsWords);
			Sha3Digest.FromWordsToBytes(state, stateAsWords);
		}

		private void KeccakPermutationAfterXor(byte[] state, byte[] data, int dataLengthInBytes)
		{
			for (int i = 0; i < dataLengthInBytes; i++)
			{
				int expr_0B_cp_1 = i;
				state[expr_0B_cp_1] ^= data[i];
			}
			this.KeccakPermutation(state);
		}

		private void KeccakPermutationOnWords(ulong[] state)
		{
			for (int i = 0; i < 24; i++)
			{
				this.Theta(state);
				this.Rho(state);
				this.Pi(state);
				this.Chi(state);
				Sha3Digest.Iota(state, i);
			}
		}

		private void Theta(ulong[] A)
		{
			for (int i = 0; i < 5; i++)
			{
				this.C[i] = 0uL;
				for (int j = 0; j < 5; j++)
				{
					this.C[i] ^= A[i + 5 * j];
				}
			}
			for (int k = 0; k < 5; k++)
			{
				ulong num = this.C[(k + 1) % 5] << 1 ^ this.C[(k + 1) % 5] >> 63 ^ this.C[(k + 4) % 5];
				for (int l = 0; l < 5; l++)
				{
					A[k + 5 * l] ^= num;
				}
			}
		}

		private void Rho(ulong[] A)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					int num = i + 5 * j;
					A[num] = ((Sha3Digest.KeccakRhoOffsets[num] != 0) ? (A[num] << Sha3Digest.KeccakRhoOffsets[num] ^ A[num] >> 64 - Sha3Digest.KeccakRhoOffsets[num]) : A[num]);
				}
			}
		}

		private void Pi(ulong[] A)
		{
			Array.Copy(A, 0, this.tempA, 0, this.tempA.Length);
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					A[j + 5 * ((2 * i + 3 * j) % 5)] = this.tempA[i + 5 * j];
				}
			}
		}

		private void Chi(ulong[] A)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					this.chiC[j] = (A[j + 5 * i] ^ (~A[(j + 1) % 5 + 5 * i] & A[(j + 2) % 5 + 5 * i]));
				}
				for (int k = 0; k < 5; k++)
				{
					A[k + 5 * i] = this.chiC[k];
				}
			}
		}

		private static void Iota(ulong[] A, int indexRound)
		{
			A[0] ^= Sha3Digest.KeccakRoundConstants[indexRound];
		}

		private void KeccakAbsorb(byte[] byteState, byte[] data, int dataInBytes)
		{
			this.KeccakPermutationAfterXor(byteState, data, dataInBytes);
		}

		private void KeccakExtract1024bits(byte[] byteState, byte[] data)
		{
			Array.Copy(byteState, 0, data, 0, 128);
		}

		private void KeccakExtract(byte[] byteState, byte[] data, int laneCount)
		{
			Array.Copy(byteState, 0, data, 0, laneCount * 8);
		}

		public IMemoable Copy()
		{
			return new Sha3Digest(this);
		}

		public void Reset(IMemoable other)
		{
			Sha3Digest source = (Sha3Digest)other;
			this.CopyIn(source);
		}
	}
}
