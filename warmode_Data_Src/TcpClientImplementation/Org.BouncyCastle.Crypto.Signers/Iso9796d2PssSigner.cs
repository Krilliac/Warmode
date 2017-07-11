using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class Iso9796d2PssSigner : ISignerWithRecovery, ISigner
	{
		public const int TrailerImplicit = 188;

		public const int TrailerRipeMD160 = 12748;

		public const int TrailerRipeMD128 = 13004;

		public const int TrailerSha1 = 13260;

		public const int TrailerSha256 = 13516;

		public const int TrailerSha512 = 13772;

		public const int TrailerSha384 = 14028;

		public const int TrailerWhirlpool = 14284;

		private static readonly IDictionary trailerMap;

		private IDigest digest;

		private IAsymmetricBlockCipher cipher;

		private SecureRandom random;

		private byte[] standardSalt;

		private int hLen;

		private int trailer;

		private int keyBits;

		private byte[] block;

		private byte[] mBuf;

		private int messageLength;

		private readonly int saltLength;

		private bool fullMessage;

		private byte[] recoveredMessage;

		private byte[] preSig;

		private byte[] preBlock;

		private int preMStart;

		private int preTLength;

		public virtual string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "withISO9796-2S2";
			}
		}

		public byte[] GetRecoveredMessage()
		{
			return this.recoveredMessage;
		}

		static Iso9796d2PssSigner()
		{
			Iso9796d2PssSigner.trailerMap = Platform.CreateHashtable();
			Iso9796d2PssSigner.trailerMap.Add("RIPEMD128", 13004);
			Iso9796d2PssSigner.trailerMap.Add("RIPEMD160", 12748);
			Iso9796d2PssSigner.trailerMap.Add("SHA-1", 13260);
			Iso9796d2PssSigner.trailerMap.Add("SHA-256", 13516);
			Iso9796d2PssSigner.trailerMap.Add("SHA-384", 14028);
			Iso9796d2PssSigner.trailerMap.Add("SHA-512", 13772);
			Iso9796d2PssSigner.trailerMap.Add("Whirlpool", 14284);
		}

		public Iso9796d2PssSigner(IAsymmetricBlockCipher cipher, IDigest digest, int saltLength, bool isImplicit)
		{
			this.cipher = cipher;
			this.digest = digest;
			this.hLen = digest.GetDigestSize();
			this.saltLength = saltLength;
			if (isImplicit)
			{
				this.trailer = 188;
				return;
			}
			string algorithmName = digest.AlgorithmName;
			if (!Iso9796d2PssSigner.trailerMap.Contains(algorithmName))
			{
				throw new ArgumentException("no valid trailer for digest");
			}
			this.trailer = (int)Iso9796d2PssSigner.trailerMap[algorithmName];
		}

		public Iso9796d2PssSigner(IAsymmetricBlockCipher cipher, IDigest digest, int saltLength) : this(cipher, digest, saltLength, false)
		{
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			RsaKeyParameters rsaKeyParameters;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				rsaKeyParameters = (RsaKeyParameters)parametersWithRandom.Parameters;
				if (forSigning)
				{
					this.random = parametersWithRandom.Random;
				}
			}
			else if (parameters is ParametersWithSalt)
			{
				if (!forSigning)
				{
					throw new ArgumentException("ParametersWithSalt only valid for signing", "parameters");
				}
				ParametersWithSalt parametersWithSalt = (ParametersWithSalt)parameters;
				rsaKeyParameters = (RsaKeyParameters)parametersWithSalt.Parameters;
				this.standardSalt = parametersWithSalt.GetSalt();
				if (this.standardSalt.Length != this.saltLength)
				{
					throw new ArgumentException("Fixed salt is of wrong length");
				}
			}
			else
			{
				rsaKeyParameters = (RsaKeyParameters)parameters;
				if (forSigning)
				{
					this.random = new SecureRandom();
				}
			}
			this.cipher.Init(forSigning, rsaKeyParameters);
			this.keyBits = rsaKeyParameters.Modulus.BitLength;
			this.block = new byte[(this.keyBits + 7) / 8];
			if (this.trailer == 188)
			{
				this.mBuf = new byte[this.block.Length - this.digest.GetDigestSize() - this.saltLength - 1 - 1];
			}
			else
			{
				this.mBuf = new byte[this.block.Length - this.digest.GetDigestSize() - this.saltLength - 1 - 2];
			}
			this.Reset();
		}

		private bool IsSameAs(byte[] a, byte[] b)
		{
			if (this.messageLength != b.Length)
			{
				return false;
			}
			bool result = true;
			for (int num = 0; num != b.Length; num++)
			{
				if (a[num] != b[num])
				{
					result = false;
				}
			}
			return result;
		}

		private void ClearBlock(byte[] block)
		{
			Array.Clear(block, 0, block.Length);
		}

		public virtual void UpdateWithRecoveredMessage(byte[] signature)
		{
			byte[] array = this.cipher.ProcessBlock(signature, 0, signature.Length);
			if (array.Length < (this.keyBits + 7) / 8)
			{
				byte[] array2 = new byte[(this.keyBits + 7) / 8];
				Array.Copy(array, 0, array2, array2.Length - array.Length, array.Length);
				this.ClearBlock(array);
				array = array2;
			}
			int num;
			if (((array[array.Length - 1] & 255) ^ 188) == 0)
			{
				num = 1;
			}
			else
			{
				int num2 = (int)(array[array.Length - 2] & 255) << 8 | (int)(array[array.Length - 1] & 255);
				string algorithmName = this.digest.AlgorithmName;
				if (!Iso9796d2PssSigner.trailerMap.Contains(algorithmName))
				{
					throw new ArgumentException("unrecognised hash in signature");
				}
				if (num2 != (int)Iso9796d2PssSigner.trailerMap[algorithmName])
				{
					throw new InvalidOperationException("signer initialised with wrong digest for trailer " + num2);
				}
				num = 2;
			}
			byte[] output = new byte[this.hLen];
			this.digest.DoFinal(output, 0);
			byte[] array3 = this.MaskGeneratorFunction1(array, array.Length - this.hLen - num, this.hLen, array.Length - this.hLen - num);
			for (int num3 = 0; num3 != array3.Length; num3++)
			{
				byte[] expr_124_cp_0 = array;
				int expr_124_cp_1 = num3;
				expr_124_cp_0[expr_124_cp_1] ^= array3[num3];
			}
			byte[] expr_14B_cp_0 = array;
			int expr_14B_cp_1 = 0;
			expr_14B_cp_0[expr_14B_cp_1] &= 127;
			int num4 = 0;
			while (num4 < array.Length && array[num4++] != 1)
			{
			}
			if (num4 >= array.Length)
			{
				this.ClearBlock(array);
			}
			this.fullMessage = (num4 > 1);
			this.recoveredMessage = new byte[array3.Length - num4 - this.saltLength];
			Array.Copy(array, num4, this.recoveredMessage, 0, this.recoveredMessage.Length);
			this.recoveredMessage.CopyTo(this.mBuf, 0);
			this.preSig = signature;
			this.preBlock = array;
			this.preMStart = num4;
			this.preTLength = num;
		}

		public virtual void Update(byte input)
		{
			if (this.preSig == null && this.messageLength < this.mBuf.Length)
			{
				this.mBuf[this.messageLength++] = input;
				return;
			}
			this.digest.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int length)
		{
			if (this.preSig == null)
			{
				while (length > 0 && this.messageLength < this.mBuf.Length)
				{
					this.Update(input[inOff]);
					inOff++;
					length--;
				}
			}
			if (length > 0)
			{
				this.digest.BlockUpdate(input, inOff, length);
			}
		}

		public virtual void Reset()
		{
			this.digest.Reset();
			this.messageLength = 0;
			if (this.mBuf != null)
			{
				this.ClearBlock(this.mBuf);
			}
			if (this.recoveredMessage != null)
			{
				this.ClearBlock(this.recoveredMessage);
				this.recoveredMessage = null;
			}
			this.fullMessage = false;
			if (this.preSig != null)
			{
				this.preSig = null;
				this.ClearBlock(this.preBlock);
				this.preBlock = null;
			}
		}

		public virtual byte[] GenerateSignature()
		{
			int digestSize = this.digest.GetDigestSize();
			byte[] array = new byte[digestSize];
			this.digest.DoFinal(array, 0);
			byte[] array2 = new byte[8];
			this.LtoOSP((long)(this.messageLength * 8), array2);
			this.digest.BlockUpdate(array2, 0, array2.Length);
			this.digest.BlockUpdate(this.mBuf, 0, this.messageLength);
			this.digest.BlockUpdate(array, 0, array.Length);
			byte[] array3;
			if (this.standardSalt != null)
			{
				array3 = this.standardSalt;
			}
			else
			{
				array3 = new byte[this.saltLength];
				this.random.NextBytes(array3);
			}
			this.digest.BlockUpdate(array3, 0, array3.Length);
			byte[] array4 = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array4, 0);
			int num = 2;
			if (this.trailer == 188)
			{
				num = 1;
			}
			int num2 = this.block.Length - this.messageLength - array3.Length - this.hLen - num - 1;
			this.block[num2] = 1;
			Array.Copy(this.mBuf, 0, this.block, num2 + 1, this.messageLength);
			Array.Copy(array3, 0, this.block, num2 + 1 + this.messageLength, array3.Length);
			byte[] array5 = this.MaskGeneratorFunction1(array4, 0, array4.Length, this.block.Length - this.hLen - num);
			for (int num3 = 0; num3 != array5.Length; num3++)
			{
				byte[] expr_172_cp_0 = this.block;
				int expr_172_cp_1 = num3;
				expr_172_cp_0[expr_172_cp_1] ^= array5[num3];
			}
			Array.Copy(array4, 0, this.block, this.block.Length - this.hLen - num, this.hLen);
			if (this.trailer == 188)
			{
				this.block[this.block.Length - 1] = 188;
			}
			else
			{
				this.block[this.block.Length - 2] = (byte)((uint)this.trailer >> 8);
				this.block[this.block.Length - 1] = (byte)this.trailer;
			}
			byte[] expr_21B_cp_0 = this.block;
			int expr_21B_cp_1 = 0;
			expr_21B_cp_0[expr_21B_cp_1] &= 127;
			byte[] result = this.cipher.ProcessBlock(this.block, 0, this.block.Length);
			this.ClearBlock(this.mBuf);
			this.ClearBlock(this.block);
			this.messageLength = 0;
			return result;
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			byte[] array = new byte[this.hLen];
			this.digest.DoFinal(array, 0);
			if (this.preSig == null)
			{
				try
				{
					this.UpdateWithRecoveredMessage(signature);
					goto IL_4F;
				}
				catch (Exception)
				{
					return false;
				}
			}
			if (!Arrays.AreEqual(this.preSig, signature))
			{
				throw new InvalidOperationException("UpdateWithRecoveredMessage called on different signature");
			}
			IL_4F:
			byte[] array2 = this.preBlock;
			int num = this.preMStart;
			int num2 = this.preTLength;
			this.preSig = null;
			this.preBlock = null;
			byte[] array3 = new byte[8];
			this.LtoOSP((long)(this.recoveredMessage.Length * 8), array3);
			this.digest.BlockUpdate(array3, 0, array3.Length);
			if (this.recoveredMessage.Length != 0)
			{
				this.digest.BlockUpdate(this.recoveredMessage, 0, this.recoveredMessage.Length);
			}
			this.digest.BlockUpdate(array, 0, array.Length);
			this.digest.BlockUpdate(array2, num + this.recoveredMessage.Length, this.saltLength);
			byte[] array4 = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array4, 0);
			int num3 = array2.Length - num2 - array4.Length;
			bool flag = true;
			for (int num4 = 0; num4 != array4.Length; num4++)
			{
				if (array4[num4] != array2[num3 + num4])
				{
					flag = false;
				}
			}
			this.ClearBlock(array2);
			this.ClearBlock(array4);
			if (!flag)
			{
				this.fullMessage = false;
				this.ClearBlock(this.recoveredMessage);
				return false;
			}
			if (this.messageLength != 0)
			{
				if (!this.IsSameAs(this.mBuf, this.recoveredMessage))
				{
					this.ClearBlock(this.mBuf);
					return false;
				}
				this.messageLength = 0;
			}
			this.ClearBlock(this.mBuf);
			return true;
		}

		public virtual bool HasFullMessage()
		{
			return this.fullMessage;
		}

		private void ItoOSP(int i, byte[] sp)
		{
			sp[0] = (byte)((uint)i >> 24);
			sp[1] = (byte)((uint)i >> 16);
			sp[2] = (byte)((uint)i >> 8);
			sp[3] = (byte)i;
		}

		private void LtoOSP(long l, byte[] sp)
		{
			sp[0] = (byte)((ulong)l >> 56);
			sp[1] = (byte)((ulong)l >> 48);
			sp[2] = (byte)((ulong)l >> 40);
			sp[3] = (byte)((ulong)l >> 32);
			sp[4] = (byte)((ulong)l >> 24);
			sp[5] = (byte)((ulong)l >> 16);
			sp[6] = (byte)((ulong)l >> 8);
			sp[7] = (byte)l;
		}

		private byte[] MaskGeneratorFunction1(byte[] Z, int zOff, int zLen, int length)
		{
			byte[] array = new byte[length];
			byte[] array2 = new byte[this.hLen];
			byte[] array3 = new byte[4];
			int num = 0;
			this.digest.Reset();
			do
			{
				this.ItoOSP(num, array3);
				this.digest.BlockUpdate(Z, zOff, zLen);
				this.digest.BlockUpdate(array3, 0, array3.Length);
				this.digest.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, num * this.hLen, this.hLen);
			}
			while (++num < length / this.hLen);
			if (num * this.hLen < length)
			{
				this.ItoOSP(num, array3);
				this.digest.BlockUpdate(Z, zOff, zLen);
				this.digest.BlockUpdate(array3, 0, array3.Length);
				this.digest.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, num * this.hLen, array.Length - num * this.hLen);
			}
			return array;
		}
	}
}
