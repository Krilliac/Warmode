using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class PssSigner : ISigner
	{
		public const byte TrailerImplicit = 188;

		private readonly IDigest contentDigest1;

		private readonly IDigest contentDigest2;

		private readonly IDigest mgfDigest;

		private readonly IAsymmetricBlockCipher cipher;

		private SecureRandom random;

		private int hLen;

		private int mgfhLen;

		private int sLen;

		private int emBits;

		private byte[] salt;

		private byte[] mDash;

		private byte[] block;

		private byte trailer;

		public virtual string AlgorithmName
		{
			get
			{
				return this.mgfDigest.AlgorithmName + "withRSAandMGF1";
			}
		}

		public static PssSigner CreateRawSigner(IAsymmetricBlockCipher cipher, IDigest digest)
		{
			return new PssSigner(cipher, new NullDigest(), digest, digest, digest.GetDigestSize(), 188);
		}

		public static PssSigner CreateRawSigner(IAsymmetricBlockCipher cipher, IDigest contentDigest, IDigest mgfDigest, int saltLen, byte trailer)
		{
			return new PssSigner(cipher, new NullDigest(), contentDigest, mgfDigest, saltLen, trailer);
		}

		public PssSigner(IAsymmetricBlockCipher cipher, IDigest digest) : this(cipher, digest, digest.GetDigestSize())
		{
		}

		public PssSigner(IAsymmetricBlockCipher cipher, IDigest digest, int saltLen) : this(cipher, digest, saltLen, 188)
		{
		}

		public PssSigner(IAsymmetricBlockCipher cipher, IDigest contentDigest, IDigest mgfDigest, int saltLen) : this(cipher, contentDigest, mgfDigest, saltLen, 188)
		{
		}

		public PssSigner(IAsymmetricBlockCipher cipher, IDigest digest, int saltLen, byte trailer) : this(cipher, digest, digest, saltLen, 188)
		{
		}

		public PssSigner(IAsymmetricBlockCipher cipher, IDigest contentDigest, IDigest mgfDigest, int saltLen, byte trailer) : this(cipher, contentDigest, contentDigest, mgfDigest, saltLen, trailer)
		{
		}

		private PssSigner(IAsymmetricBlockCipher cipher, IDigest contentDigest1, IDigest contentDigest2, IDigest mgfDigest, int saltLen, byte trailer)
		{
			this.cipher = cipher;
			this.contentDigest1 = contentDigest1;
			this.contentDigest2 = contentDigest2;
			this.mgfDigest = mgfDigest;
			this.hLen = contentDigest2.GetDigestSize();
			this.mgfhLen = mgfDigest.GetDigestSize();
			this.sLen = saltLen;
			this.salt = new byte[saltLen];
			this.mDash = new byte[8 + saltLen + this.hLen];
			this.trailer = trailer;
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				parameters = parametersWithRandom.Parameters;
				this.random = parametersWithRandom.Random;
			}
			else if (forSigning)
			{
				this.random = new SecureRandom();
			}
			this.cipher.Init(forSigning, parameters);
			RsaKeyParameters rsaKeyParameters;
			if (parameters is RsaBlindingParameters)
			{
				rsaKeyParameters = ((RsaBlindingParameters)parameters).PublicKey;
			}
			else
			{
				rsaKeyParameters = (RsaKeyParameters)parameters;
			}
			this.emBits = rsaKeyParameters.Modulus.BitLength - 1;
			if (this.emBits < 8 * this.hLen + 8 * this.sLen + 9)
			{
				throw new ArgumentException("key too small for specified hash and salt lengths");
			}
			this.block = new byte[(this.emBits + 7) / 8];
		}

		private void ClearBlock(byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		public virtual void Update(byte input)
		{
			this.contentDigest1.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int length)
		{
			this.contentDigest1.BlockUpdate(input, inOff, length);
		}

		public virtual void Reset()
		{
			this.contentDigest1.Reset();
		}

		public virtual byte[] GenerateSignature()
		{
			this.contentDigest1.DoFinal(this.mDash, this.mDash.Length - this.hLen - this.sLen);
			if (this.sLen != 0)
			{
				this.random.NextBytes(this.salt);
				this.salt.CopyTo(this.mDash, this.mDash.Length - this.sLen);
			}
			byte[] array = new byte[this.hLen];
			this.contentDigest2.BlockUpdate(this.mDash, 0, this.mDash.Length);
			this.contentDigest2.DoFinal(array, 0);
			this.block[this.block.Length - this.sLen - 1 - this.hLen - 1] = 1;
			this.salt.CopyTo(this.block, this.block.Length - this.sLen - this.hLen - 1);
			byte[] array2 = this.MaskGeneratorFunction1(array, 0, array.Length, this.block.Length - this.hLen - 1);
			for (int num = 0; num != array2.Length; num++)
			{
				byte[] expr_10D_cp_0 = this.block;
				int expr_10D_cp_1 = num;
				expr_10D_cp_0[expr_10D_cp_1] ^= array2[num];
			}
			byte[] expr_133_cp_0 = this.block;
			int expr_133_cp_1 = 0;
			expr_133_cp_0[expr_133_cp_1] &= (byte)(255 >> this.block.Length * 8 - this.emBits);
			array.CopyTo(this.block, this.block.Length - this.hLen - 1);
			this.block[this.block.Length - 1] = this.trailer;
			byte[] result = this.cipher.ProcessBlock(this.block, 0, this.block.Length);
			this.ClearBlock(this.block);
			return result;
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			this.contentDigest1.DoFinal(this.mDash, this.mDash.Length - this.hLen - this.sLen);
			byte[] array = this.cipher.ProcessBlock(signature, 0, signature.Length);
			array.CopyTo(this.block, this.block.Length - array.Length);
			if (this.block[this.block.Length - 1] != this.trailer)
			{
				this.ClearBlock(this.block);
				return false;
			}
			byte[] array2 = this.MaskGeneratorFunction1(this.block, this.block.Length - this.hLen - 1, this.hLen, this.block.Length - this.hLen - 1);
			for (int num = 0; num != array2.Length; num++)
			{
				byte[] expr_BD_cp_0 = this.block;
				int expr_BD_cp_1 = num;
				expr_BD_cp_0[expr_BD_cp_1] ^= array2[num];
			}
			byte[] expr_E3_cp_0 = this.block;
			int expr_E3_cp_1 = 0;
			expr_E3_cp_0[expr_E3_cp_1] &= (byte)(255 >> this.block.Length * 8 - this.emBits);
			for (int num2 = 0; num2 != this.block.Length - this.hLen - this.sLen - 2; num2++)
			{
				if (this.block[num2] != 0)
				{
					this.ClearBlock(this.block);
					return false;
				}
			}
			if (this.block[this.block.Length - this.hLen - this.sLen - 2] != 1)
			{
				this.ClearBlock(this.block);
				return false;
			}
			Array.Copy(this.block, this.block.Length - this.sLen - this.hLen - 1, this.mDash, this.mDash.Length - this.sLen, this.sLen);
			this.contentDigest2.BlockUpdate(this.mDash, 0, this.mDash.Length);
			this.contentDigest2.DoFinal(this.mDash, this.mDash.Length - this.hLen);
			int num3 = this.block.Length - this.hLen - 1;
			for (int num4 = this.mDash.Length - this.hLen; num4 != this.mDash.Length; num4++)
			{
				if ((this.block[num3] ^ this.mDash[num4]) != 0)
				{
					this.ClearBlock(this.mDash);
					this.ClearBlock(this.block);
					return false;
				}
				num3++;
			}
			this.ClearBlock(this.mDash);
			this.ClearBlock(this.block);
			return true;
		}

		private void ItoOSP(int i, byte[] sp)
		{
			sp[0] = (byte)((uint)i >> 24);
			sp[1] = (byte)((uint)i >> 16);
			sp[2] = (byte)((uint)i >> 8);
			sp[3] = (byte)i;
		}

		private byte[] MaskGeneratorFunction1(byte[] Z, int zOff, int zLen, int length)
		{
			byte[] array = new byte[length];
			byte[] array2 = new byte[this.mgfhLen];
			byte[] array3 = new byte[4];
			int i = 0;
			this.mgfDigest.Reset();
			while (i < length / this.mgfhLen)
			{
				this.ItoOSP(i, array3);
				this.mgfDigest.BlockUpdate(Z, zOff, zLen);
				this.mgfDigest.BlockUpdate(array3, 0, array3.Length);
				this.mgfDigest.DoFinal(array2, 0);
				array2.CopyTo(array, i * this.mgfhLen);
				i++;
			}
			if (i * this.mgfhLen < length)
			{
				this.ItoOSP(i, array3);
				this.mgfDigest.BlockUpdate(Z, zOff, zLen);
				this.mgfDigest.BlockUpdate(array3, 0, array3.Length);
				this.mgfDigest.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, i * this.mgfhLen, array.Length - i * this.mgfhLen);
			}
			return array;
		}
	}
}
