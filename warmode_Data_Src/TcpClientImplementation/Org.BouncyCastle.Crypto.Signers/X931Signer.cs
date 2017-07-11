using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class X931Signer : ISigner
	{
		public const int TRAILER_IMPLICIT = 188;

		public const int TRAILER_RIPEMD160 = 12748;

		public const int TRAILER_RIPEMD128 = 13004;

		public const int TRAILER_SHA1 = 13260;

		public const int TRAILER_SHA256 = 13516;

		public const int TRAILER_SHA512 = 13772;

		public const int TRAILER_SHA384 = 14028;

		public const int TRAILER_WHIRLPOOL = 14284;

		public const int TRAILER_SHA224 = 14540;

		private static readonly IDictionary trailerMap;

		private IDigest digest;

		private IAsymmetricBlockCipher cipher;

		private RsaKeyParameters kParam;

		private int trailer;

		private int keyBits;

		private byte[] block;

		public virtual string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "with" + this.cipher.AlgorithmName + "/X9.31";
			}
		}

		static X931Signer()
		{
			X931Signer.trailerMap = Platform.CreateHashtable();
			X931Signer.trailerMap.Add("RIPEMD128", 13004);
			X931Signer.trailerMap.Add("RIPEMD160", 12748);
			X931Signer.trailerMap.Add("SHA-1", 13260);
			X931Signer.trailerMap.Add("SHA-224", 14540);
			X931Signer.trailerMap.Add("SHA-256", 13516);
			X931Signer.trailerMap.Add("SHA-384", 14028);
			X931Signer.trailerMap.Add("SHA-512", 13772);
			X931Signer.trailerMap.Add("Whirlpool", 14284);
		}

		public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest, bool isImplicit)
		{
			this.cipher = cipher;
			this.digest = digest;
			if (isImplicit)
			{
				this.trailer = 188;
				return;
			}
			string algorithmName = digest.AlgorithmName;
			if (!X931Signer.trailerMap.Contains(algorithmName))
			{
				throw new ArgumentException("no valid trailer", "digest");
			}
			this.trailer = (int)X931Signer.trailerMap[algorithmName];
		}

		public X931Signer(IAsymmetricBlockCipher cipher, IDigest digest) : this(cipher, digest, false)
		{
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			this.kParam = (RsaKeyParameters)parameters;
			this.cipher.Init(forSigning, this.kParam);
			this.keyBits = this.kParam.Modulus.BitLength;
			this.block = new byte[(this.keyBits + 7) / 8];
			this.Reset();
		}

		private void ClearBlock(byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		public virtual void Update(byte b)
		{
			this.digest.Update(b);
		}

		public virtual void BlockUpdate(byte[] input, int off, int len)
		{
			this.digest.BlockUpdate(input, off, len);
		}

		public virtual void Reset()
		{
			this.digest.Reset();
		}

		public virtual byte[] GenerateSignature()
		{
			this.CreateSignatureBlock();
			BigInteger bigInteger = new BigInteger(1, this.cipher.ProcessBlock(this.block, 0, this.block.Length));
			this.ClearBlock(this.block);
			bigInteger = bigInteger.Min(this.kParam.Modulus.Subtract(bigInteger));
			return BigIntegers.AsUnsignedByteArray((this.kParam.Modulus.BitLength + 7) / 8, bigInteger);
		}

		private void CreateSignatureBlock()
		{
			int digestSize = this.digest.GetDigestSize();
			int num;
			if (this.trailer == 188)
			{
				num = this.block.Length - digestSize - 1;
				this.digest.DoFinal(this.block, num);
				this.block[this.block.Length - 1] = 188;
			}
			else
			{
				num = this.block.Length - digestSize - 2;
				this.digest.DoFinal(this.block, num);
				this.block[this.block.Length - 2] = (byte)(this.trailer >> 8);
				this.block[this.block.Length - 1] = (byte)this.trailer;
			}
			this.block[0] = 107;
			for (int num2 = num - 2; num2 != 0; num2--)
			{
				this.block[num2] = 187;
			}
			this.block[num - 1] = 186;
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			try
			{
				this.block = this.cipher.ProcessBlock(signature, 0, signature.Length);
			}
			catch (Exception)
			{
				return false;
			}
			BigInteger bigInteger = new BigInteger(this.block);
			BigInteger n;
			if ((bigInteger.IntValue & 15) == 12)
			{
				n = bigInteger;
			}
			else
			{
				bigInteger = this.kParam.Modulus.Subtract(bigInteger);
				if ((bigInteger.IntValue & 15) != 12)
				{
					return false;
				}
				n = bigInteger;
			}
			this.CreateSignatureBlock();
			byte[] b = BigIntegers.AsUnsignedByteArray(this.block.Length, n);
			bool result = Arrays.ConstantTimeAreEqual(this.block, b);
			this.ClearBlock(this.block);
			this.ClearBlock(b);
			return result;
		}
	}
}
