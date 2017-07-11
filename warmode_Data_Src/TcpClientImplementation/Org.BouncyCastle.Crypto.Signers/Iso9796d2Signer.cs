using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class Iso9796d2Signer : ISignerWithRecovery, ISigner
	{
		public const int TrailerImplicit = 188;

		public const int TrailerRipeMD160 = 12748;

		public const int TrailerRipeMD128 = 13004;

		public const int TrailerSha1 = 13260;

		public const int TrailerSha256 = 13516;

		public const int TrailerSha512 = 13772;

		public const int TrailerSha384 = 14028;

		public const int TrailerWhirlpool = 14284;

		private static IDictionary trailerMap;

		private IDigest digest;

		private IAsymmetricBlockCipher cipher;

		private int trailer;

		private int keyBits;

		private byte[] block;

		private byte[] mBuf;

		private int messageLength;

		private bool fullMessage;

		private byte[] recoveredMessage;

		private byte[] preSig;

		private byte[] preBlock;

		public virtual string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "withISO9796-2S1";
			}
		}

		public byte[] GetRecoveredMessage()
		{
			return this.recoveredMessage;
		}

		static Iso9796d2Signer()
		{
			Iso9796d2Signer.trailerMap = Platform.CreateHashtable();
			Iso9796d2Signer.trailerMap.Add("RIPEMD128", 13004);
			Iso9796d2Signer.trailerMap.Add("RIPEMD160", 12748);
			Iso9796d2Signer.trailerMap.Add("SHA-1", 13260);
			Iso9796d2Signer.trailerMap.Add("SHA-256", 13516);
			Iso9796d2Signer.trailerMap.Add("SHA-384", 14028);
			Iso9796d2Signer.trailerMap.Add("SHA-512", 13772);
			Iso9796d2Signer.trailerMap.Add("Whirlpool", 14284);
		}

		public Iso9796d2Signer(IAsymmetricBlockCipher cipher, IDigest digest, bool isImplicit)
		{
			this.cipher = cipher;
			this.digest = digest;
			if (isImplicit)
			{
				this.trailer = 188;
				return;
			}
			string algorithmName = digest.AlgorithmName;
			if (Iso9796d2Signer.trailerMap.Contains(algorithmName))
			{
				this.trailer = (int)Iso9796d2Signer.trailerMap[digest.AlgorithmName];
				return;
			}
			throw new ArgumentException("no valid trailer for digest");
		}

		public Iso9796d2Signer(IAsymmetricBlockCipher cipher, IDigest digest) : this(cipher, digest, false)
		{
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)parameters;
			this.cipher.Init(forSigning, rsaKeyParameters);
			this.keyBits = rsaKeyParameters.Modulus.BitLength;
			this.block = new byte[(this.keyBits + 7) / 8];
			if (this.trailer == 188)
			{
				this.mBuf = new byte[this.block.Length - this.digest.GetDigestSize() - 2];
			}
			else
			{
				this.mBuf = new byte[this.block.Length - this.digest.GetDigestSize() - 3];
			}
			this.Reset();
		}

		private bool IsSameAs(byte[] a, byte[] b)
		{
			int num;
			if (this.messageLength > this.mBuf.Length)
			{
				if (this.mBuf.Length > b.Length)
				{
					return false;
				}
				num = this.mBuf.Length;
			}
			else
			{
				if (this.messageLength != b.Length)
				{
					return false;
				}
				num = b.Length;
			}
			bool result = true;
			for (int num2 = 0; num2 != num; num2++)
			{
				if (a[num2] != b[num2])
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
			if (((array[0] & 192) ^ 64) != 0)
			{
				throw new InvalidCipherTextException("malformed signature");
			}
			if (((array[array.Length - 1] & 15) ^ 12) != 0)
			{
				throw new InvalidCipherTextException("malformed signature");
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
				if (!Iso9796d2Signer.trailerMap.Contains(algorithmName))
				{
					throw new ArgumentException("unrecognised hash in signature");
				}
				if (num2 != (int)Iso9796d2Signer.trailerMap[algorithmName])
				{
					throw new InvalidOperationException("signer initialised with wrong digest for trailer " + num2);
				}
				num = 2;
			}
			int num3 = 0;
			while (num3 != array.Length && ((array[num3] & 15) ^ 10) != 0)
			{
				num3++;
			}
			num3++;
			int num4 = array.Length - num - this.digest.GetDigestSize();
			if (num4 - num3 <= 0)
			{
				throw new InvalidCipherTextException("malformed block");
			}
			if ((array[0] & 32) == 0)
			{
				this.fullMessage = true;
				this.recoveredMessage = new byte[num4 - num3];
				Array.Copy(array, num3, this.recoveredMessage, 0, this.recoveredMessage.Length);
			}
			else
			{
				this.fullMessage = false;
				this.recoveredMessage = new byte[num4 - num3];
				Array.Copy(array, num3, this.recoveredMessage, 0, this.recoveredMessage.Length);
			}
			this.preSig = signature;
			this.preBlock = array;
			this.digest.BlockUpdate(this.recoveredMessage, 0, this.recoveredMessage.Length);
			this.messageLength = this.recoveredMessage.Length;
			this.recoveredMessage.CopyTo(this.mBuf, 0);
		}

		public virtual void Update(byte input)
		{
			this.digest.Update(input);
			if (this.messageLength < this.mBuf.Length)
			{
				this.mBuf[this.messageLength] = input;
			}
			this.messageLength++;
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int length)
		{
			while (length > 0 && this.messageLength < this.mBuf.Length)
			{
				this.Update(input[inOff]);
				inOff++;
				length--;
			}
			this.digest.BlockUpdate(input, inOff, length);
			this.messageLength += length;
		}

		public virtual void Reset()
		{
			this.digest.Reset();
			this.messageLength = 0;
			this.ClearBlock(this.mBuf);
			if (this.recoveredMessage != null)
			{
				this.ClearBlock(this.recoveredMessage);
			}
			this.recoveredMessage = null;
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
			int num;
			int num2;
			if (this.trailer == 188)
			{
				num = 8;
				num2 = this.block.Length - digestSize - 1;
				this.digest.DoFinal(this.block, num2);
				this.block[this.block.Length - 1] = 188;
			}
			else
			{
				num = 16;
				num2 = this.block.Length - digestSize - 2;
				this.digest.DoFinal(this.block, num2);
				this.block[this.block.Length - 2] = (byte)((uint)this.trailer >> 8);
				this.block[this.block.Length - 1] = (byte)this.trailer;
			}
			int num3 = (digestSize + this.messageLength) * 8 + num + 4 - this.keyBits;
			byte b;
			if (num3 > 0)
			{
				int num4 = this.messageLength - (num3 + 7) / 8;
				b = 96;
				num2 -= num4;
				Array.Copy(this.mBuf, 0, this.block, num2, num4);
			}
			else
			{
				b = 64;
				num2 -= this.messageLength;
				Array.Copy(this.mBuf, 0, this.block, num2, this.messageLength);
			}
			if (num2 - 1 > 0)
			{
				for (int num5 = num2 - 1; num5 != 0; num5--)
				{
					this.block[num5] = 187;
				}
				byte[] expr_150_cp_0 = this.block;
				int expr_150_cp_1 = num2 - 1;
				expr_150_cp_0[expr_150_cp_1] ^= 1;
				this.block[0] = 11;
				byte[] expr_174_cp_0 = this.block;
				int expr_174_cp_1 = 0;
				expr_174_cp_0[expr_174_cp_1] |= b;
			}
			else
			{
				this.block[0] = 10;
				byte[] expr_19A_cp_0 = this.block;
				int expr_19A_cp_1 = 0;
				expr_19A_cp_0[expr_19A_cp_1] |= b;
			}
			byte[] result = this.cipher.ProcessBlock(this.block, 0, this.block.Length);
			this.ClearBlock(this.mBuf);
			this.ClearBlock(this.block);
			return result;
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			byte[] array;
			if (this.preSig == null)
			{
				try
				{
					array = this.cipher.ProcessBlock(signature, 0, signature.Length);
					goto IL_52;
				}
				catch (Exception)
				{
					return false;
				}
			}
			if (!Arrays.AreEqual(this.preSig, signature))
			{
				throw new InvalidOperationException("updateWithRecoveredMessage called on different signature");
			}
			array = this.preBlock;
			this.preSig = null;
			this.preBlock = null;
			IL_52:
			if (((array[0] & 192) ^ 64) != 0)
			{
				return this.ReturnFalse(array);
			}
			if (((array[array.Length - 1] & 15) ^ 12) != 0)
			{
				return this.ReturnFalse(array);
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
				if (!Iso9796d2Signer.trailerMap.Contains(algorithmName))
				{
					throw new ArgumentException("unrecognised hash in signature");
				}
				if (num2 != (int)Iso9796d2Signer.trailerMap[algorithmName])
				{
					throw new InvalidOperationException("signer initialised with wrong digest for trailer " + num2);
				}
				num = 2;
			}
			int num3 = 0;
			while (num3 != array.Length && ((array[num3] & 15) ^ 10) != 0)
			{
				num3++;
			}
			num3++;
			byte[] array2 = new byte[this.digest.GetDigestSize()];
			int num4 = array.Length - num - array2.Length;
			if (num4 - num3 <= 0)
			{
				return this.ReturnFalse(array);
			}
			if ((array[0] & 32) == 0)
			{
				this.fullMessage = true;
				if (this.messageLength > num4 - num3)
				{
					return this.ReturnFalse(array);
				}
				this.digest.Reset();
				this.digest.BlockUpdate(array, num3, num4 - num3);
				this.digest.DoFinal(array2, 0);
				bool flag = true;
				for (int num5 = 0; num5 != array2.Length; num5++)
				{
					byte[] expr_1C0_cp_0 = array;
					int expr_1C0_cp_1 = num4 + num5;
					expr_1C0_cp_0[expr_1C0_cp_1] ^= array2[num5];
					if (array[num4 + num5] != 0)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					return this.ReturnFalse(array);
				}
				this.recoveredMessage = new byte[num4 - num3];
				Array.Copy(array, num3, this.recoveredMessage, 0, this.recoveredMessage.Length);
			}
			else
			{
				this.fullMessage = false;
				this.digest.DoFinal(array2, 0);
				bool flag2 = true;
				for (int num6 = 0; num6 != array2.Length; num6++)
				{
					byte[] expr_24D_cp_0 = array;
					int expr_24D_cp_1 = num4 + num6;
					expr_24D_cp_0[expr_24D_cp_1] ^= array2[num6];
					if (array[num4 + num6] != 0)
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					return this.ReturnFalse(array);
				}
				this.recoveredMessage = new byte[num4 - num3];
				Array.Copy(array, num3, this.recoveredMessage, 0, this.recoveredMessage.Length);
			}
			if (this.messageLength != 0 && !this.IsSameAs(this.mBuf, this.recoveredMessage))
			{
				return this.ReturnFalse(array);
			}
			this.ClearBlock(this.mBuf);
			this.ClearBlock(array);
			return true;
		}

		private bool ReturnFalse(byte[] block)
		{
			this.ClearBlock(this.mBuf);
			this.ClearBlock(block);
			return false;
		}

		public virtual bool HasFullMessage()
		{
			return this.fullMessage;
		}
	}
}
