using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class Poly1305 : IMac
	{
		private const int BLOCK_SIZE = 16;

		private readonly IBlockCipher cipher;

		private readonly byte[] singleByte = new byte[1];

		private uint r0;

		private uint r1;

		private uint r2;

		private uint r3;

		private uint r4;

		private uint s1;

		private uint s2;

		private uint s3;

		private uint s4;

		private uint k0;

		private uint k1;

		private uint k2;

		private uint k3;

		private byte[] currentBlock = new byte[16];

		private int currentBlockOffset;

		private uint h0;

		private uint h1;

		private uint h2;

		private uint h3;

		private uint h4;

		public string AlgorithmName
		{
			get
			{
				if (this.cipher != null)
				{
					return "Poly1305-" + this.cipher.AlgorithmName;
				}
				return "Poly1305";
			}
		}

		public Poly1305()
		{
			this.cipher = null;
		}

		public Poly1305(IBlockCipher cipher)
		{
			if (cipher.GetBlockSize() != 16)
			{
				throw new ArgumentException("Poly1305 requires a 128 bit block cipher.");
			}
			this.cipher = cipher;
		}

		public void Init(ICipherParameters parameters)
		{
			byte[] nonce = null;
			if (this.cipher != null)
			{
				if (!(parameters is ParametersWithIV))
				{
					throw new ArgumentException("Poly1305 requires an IV when used with a block cipher.", "parameters");
				}
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				nonce = parametersWithIV.GetIV();
				parameters = parametersWithIV.Parameters;
			}
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("Poly1305 requires a key.");
			}
			KeyParameter keyParameter = (KeyParameter)parameters;
			this.SetKey(keyParameter.GetKey(), nonce);
			this.Reset();
		}

		private void SetKey(byte[] key, byte[] nonce)
		{
			if (this.cipher != null && (nonce == null || nonce.Length != 16))
			{
				throw new ArgumentException("Poly1305 requires a 128 bit IV.");
			}
			Poly1305KeyGenerator.CheckKey(key);
			uint num = Pack.LE_To_UInt32(key, 16);
			uint num2 = Pack.LE_To_UInt32(key, 20);
			uint num3 = Pack.LE_To_UInt32(key, 24);
			uint num4 = Pack.LE_To_UInt32(key, 28);
			this.r0 = (num & 67108863u);
			num >>= 26;
			num |= num2 << 6;
			this.r1 = (num & 67108611u);
			num2 >>= 20;
			num2 |= num3 << 12;
			this.r2 = (num2 & 67092735u);
			num3 >>= 14;
			num3 |= num4 << 18;
			this.r3 = (num3 & 66076671u);
			num4 >>= 8;
			this.r4 = (num4 & 1048575u);
			this.s1 = this.r1 * 5u;
			this.s2 = this.r2 * 5u;
			this.s3 = this.r3 * 5u;
			this.s4 = this.r4 * 5u;
			byte[] array;
			if (this.cipher == null)
			{
				array = key;
			}
			else
			{
				array = new byte[16];
				this.cipher.Init(true, new KeyParameter(key, 0, 16));
				this.cipher.ProcessBlock(nonce, 0, array, 0);
			}
			this.k0 = Pack.LE_To_UInt32(array, 0);
			this.k1 = Pack.LE_To_UInt32(array, 4);
			this.k2 = Pack.LE_To_UInt32(array, 8);
			this.k3 = Pack.LE_To_UInt32(array, 12);
		}

		public int GetMacSize()
		{
			return 16;
		}

		public void Update(byte input)
		{
			this.singleByte[0] = input;
			this.BlockUpdate(this.singleByte, 0, 1);
		}

		public void BlockUpdate(byte[] input, int inOff, int len)
		{
			int num = 0;
			while (len > num)
			{
				if (this.currentBlockOffset == 16)
				{
					this.processBlock();
					this.currentBlockOffset = 0;
				}
				int num2 = Math.Min(len - num, 16 - this.currentBlockOffset);
				Array.Copy(input, num + inOff, this.currentBlock, this.currentBlockOffset, num2);
				num += num2;
				this.currentBlockOffset += num2;
			}
		}

		private void processBlock()
		{
			if (this.currentBlockOffset < 16)
			{
				this.currentBlock[this.currentBlockOffset] = 1;
				for (int i = this.currentBlockOffset + 1; i < 16; i++)
				{
					this.currentBlock[i] = 0;
				}
			}
			ulong num = (ulong)Pack.LE_To_UInt32(this.currentBlock, 0);
			ulong num2 = (ulong)Pack.LE_To_UInt32(this.currentBlock, 4);
			ulong num3 = (ulong)Pack.LE_To_UInt32(this.currentBlock, 8);
			ulong num4 = (ulong)Pack.LE_To_UInt32(this.currentBlock, 12);
			this.h0 += (uint)(num & 67108863uL);
			this.h1 += (uint)((num2 << 32 | num) >> 26 & 67108863uL);
			this.h2 += (uint)((num3 << 32 | num2) >> 20 & 67108863uL);
			this.h3 += (uint)((num4 << 32 | num3) >> 14 & 67108863uL);
			this.h4 += (uint)(num4 >> 8);
			if (this.currentBlockOffset == 16)
			{
				this.h4 += 16777216u;
			}
			ulong num5 = Poly1305.mul32x32_64(this.h0, this.r0) + Poly1305.mul32x32_64(this.h1, this.s4) + Poly1305.mul32x32_64(this.h2, this.s3) + Poly1305.mul32x32_64(this.h3, this.s2) + Poly1305.mul32x32_64(this.h4, this.s1);
			ulong num6 = Poly1305.mul32x32_64(this.h0, this.r1) + Poly1305.mul32x32_64(this.h1, this.r0) + Poly1305.mul32x32_64(this.h2, this.s4) + Poly1305.mul32x32_64(this.h3, this.s3) + Poly1305.mul32x32_64(this.h4, this.s2);
			ulong num7 = Poly1305.mul32x32_64(this.h0, this.r2) + Poly1305.mul32x32_64(this.h1, this.r1) + Poly1305.mul32x32_64(this.h2, this.r0) + Poly1305.mul32x32_64(this.h3, this.s4) + Poly1305.mul32x32_64(this.h4, this.s3);
			ulong num8 = Poly1305.mul32x32_64(this.h0, this.r3) + Poly1305.mul32x32_64(this.h1, this.r2) + Poly1305.mul32x32_64(this.h2, this.r1) + Poly1305.mul32x32_64(this.h3, this.r0) + Poly1305.mul32x32_64(this.h4, this.s4);
			ulong num9 = Poly1305.mul32x32_64(this.h0, this.r4) + Poly1305.mul32x32_64(this.h1, this.r3) + Poly1305.mul32x32_64(this.h2, this.r2) + Poly1305.mul32x32_64(this.h3, this.r1) + Poly1305.mul32x32_64(this.h4, this.r0);
			this.h0 = ((uint)num5 & 67108863u);
			ulong num10 = num5 >> 26;
			num6 += num10;
			this.h1 = ((uint)num6 & 67108863u);
			num10 = num6 >> 26;
			num7 += num10;
			this.h2 = ((uint)num7 & 67108863u);
			num10 = num7 >> 26;
			num8 += num10;
			this.h3 = ((uint)num8 & 67108863u);
			num10 = num8 >> 26;
			num9 += num10;
			this.h4 = ((uint)num9 & 67108863u);
			num10 = num9 >> 26;
			this.h0 += (uint)(num10 * 5uL);
		}

		public int DoFinal(byte[] output, int outOff)
		{
			if (outOff + 16 > output.Length)
			{
				throw new DataLengthException("Output buffer is too short.");
			}
			if (this.currentBlockOffset > 0)
			{
				this.processBlock();
			}
			uint num = this.h0 >> 26;
			this.h0 &= 67108863u;
			this.h1 += num;
			num = this.h1 >> 26;
			this.h1 &= 67108863u;
			this.h2 += num;
			num = this.h2 >> 26;
			this.h2 &= 67108863u;
			this.h3 += num;
			num = this.h3 >> 26;
			this.h3 &= 67108863u;
			this.h4 += num;
			num = this.h4 >> 26;
			this.h4 &= 67108863u;
			this.h0 += num * 5u;
			uint num2 = this.h0 + 5u;
			num = num2 >> 26;
			num2 &= 67108863u;
			uint num3 = this.h1 + num;
			num = num3 >> 26;
			num3 &= 67108863u;
			uint num4 = this.h2 + num;
			num = num4 >> 26;
			num4 &= 67108863u;
			uint num5 = this.h3 + num;
			num = num5 >> 26;
			num5 &= 67108863u;
			uint num6 = this.h4 + num - 67108864u;
			num = (num6 >> 31) - 1u;
			uint num7 = ~num;
			this.h0 = ((this.h0 & num7) | (num2 & num));
			this.h1 = ((this.h1 & num7) | (num3 & num));
			this.h2 = ((this.h2 & num7) | (num4 & num));
			this.h3 = ((this.h3 & num7) | (num5 & num));
			this.h4 = ((this.h4 & num7) | (num6 & num));
			ulong num8 = (ulong)(this.h0 | this.h1 << 26) + (ulong)this.k0;
			ulong num9 = (ulong)(this.h1 >> 6 | this.h2 << 20) + (ulong)this.k1;
			ulong num10 = (ulong)(this.h2 >> 12 | this.h3 << 14) + (ulong)this.k2;
			ulong num11 = (ulong)(this.h3 >> 18 | this.h4 << 8) + (ulong)this.k3;
			Pack.UInt32_To_LE((uint)num8, output, outOff);
			num9 += num8 >> 32;
			Pack.UInt32_To_LE((uint)num9, output, outOff + 4);
			num10 += num9 >> 32;
			Pack.UInt32_To_LE((uint)num10, output, outOff + 8);
			num11 += num10 >> 32;
			Pack.UInt32_To_LE((uint)num11, output, outOff + 12);
			this.Reset();
			return 16;
		}

		public void Reset()
		{
			this.currentBlockOffset = 0;
			this.h0 = (this.h1 = (this.h2 = (this.h3 = (this.h4 = 0u))));
		}

		private static ulong mul32x32_64(uint i1, uint i2)
		{
			return (ulong)i1 * (ulong)i2;
		}
	}
}
