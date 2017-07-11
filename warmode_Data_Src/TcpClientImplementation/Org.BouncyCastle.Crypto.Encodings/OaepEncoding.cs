using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Encodings
{
	public class OaepEncoding : IAsymmetricBlockCipher
	{
		private byte[] defHash;

		private IDigest hash;

		private IDigest mgf1Hash;

		private IAsymmetricBlockCipher engine;

		private SecureRandom random;

		private bool forEncryption;

		public string AlgorithmName
		{
			get
			{
				return this.engine.AlgorithmName + "/OAEPPadding";
			}
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher) : this(cipher, new Sha1Digest(), null)
		{
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash) : this(cipher, hash, null)
		{
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash, byte[] encodingParams) : this(cipher, hash, hash, encodingParams)
		{
		}

		public OaepEncoding(IAsymmetricBlockCipher cipher, IDigest hash, IDigest mgf1Hash, byte[] encodingParams)
		{
			this.engine = cipher;
			this.hash = hash;
			this.mgf1Hash = mgf1Hash;
			this.defHash = new byte[hash.GetDigestSize()];
			if (encodingParams != null)
			{
				hash.BlockUpdate(encodingParams, 0, encodingParams.Length);
			}
			hash.DoFinal(this.defHash, 0);
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return this.engine;
		}

		public void Init(bool forEncryption, ICipherParameters param)
		{
			if (param is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)param;
				this.random = parametersWithRandom.Random;
			}
			else
			{
				this.random = new SecureRandom();
			}
			this.engine.Init(forEncryption, param);
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = this.engine.GetInputBlockSize();
			if (this.forEncryption)
			{
				return inputBlockSize - 1 - 2 * this.defHash.Length;
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
			return outputBlockSize - 1 - 2 * this.defHash.Length;
		}

		public byte[] ProcessBlock(byte[] inBytes, int inOff, int inLen)
		{
			if (this.forEncryption)
			{
				return this.EncodeBlock(inBytes, inOff, inLen);
			}
			return this.DecodeBlock(inBytes, inOff, inLen);
		}

		private byte[] EncodeBlock(byte[] inBytes, int inOff, int inLen)
		{
			byte[] array = new byte[this.GetInputBlockSize() + 1 + 2 * this.defHash.Length];
			Array.Copy(inBytes, inOff, array, array.Length - inLen, inLen);
			array[array.Length - inLen - 1] = 1;
			Array.Copy(this.defHash, 0, array, this.defHash.Length, this.defHash.Length);
			byte[] array2 = this.random.GenerateSeed(this.defHash.Length);
			byte[] array3 = this.maskGeneratorFunction1(array2, 0, array2.Length, array.Length - this.defHash.Length);
			for (int num = this.defHash.Length; num != array.Length; num++)
			{
				byte[] expr_8C_cp_0 = array;
				int expr_8C_cp_1 = num;
				expr_8C_cp_0[expr_8C_cp_1] ^= array3[num - this.defHash.Length];
			}
			Array.Copy(array2, 0, array, 0, this.defHash.Length);
			array3 = this.maskGeneratorFunction1(array, this.defHash.Length, array.Length - this.defHash.Length, this.defHash.Length);
			for (int num2 = 0; num2 != this.defHash.Length; num2++)
			{
				byte[] expr_F1_cp_0 = array;
				int expr_F1_cp_1 = num2;
				expr_F1_cp_0[expr_F1_cp_1] ^= array3[num2];
			}
			return this.engine.ProcessBlock(array, 0, array.Length);
		}

		private byte[] DecodeBlock(byte[] inBytes, int inOff, int inLen)
		{
			byte[] array = this.engine.ProcessBlock(inBytes, inOff, inLen);
			byte[] array2;
			if (array.Length < this.engine.GetOutputBlockSize())
			{
				array2 = new byte[this.engine.GetOutputBlockSize()];
				Array.Copy(array, 0, array2, array2.Length - array.Length, array.Length);
			}
			else
			{
				array2 = array;
			}
			if (array2.Length < 2 * this.defHash.Length + 1)
			{
				throw new InvalidCipherTextException("data too short");
			}
			byte[] array3 = this.maskGeneratorFunction1(array2, this.defHash.Length, array2.Length - this.defHash.Length, this.defHash.Length);
			for (int num = 0; num != this.defHash.Length; num++)
			{
				byte[] expr_91_cp_0 = array2;
				int expr_91_cp_1 = num;
				expr_91_cp_0[expr_91_cp_1] ^= array3[num];
			}
			array3 = this.maskGeneratorFunction1(array2, 0, this.defHash.Length, array2.Length - this.defHash.Length);
			for (int num2 = this.defHash.Length; num2 != array2.Length; num2++)
			{
				byte[] expr_E1_cp_0 = array2;
				int expr_E1_cp_1 = num2;
				expr_E1_cp_0[expr_E1_cp_1] ^= array3[num2 - this.defHash.Length];
			}
			int num3 = 0;
			for (int i = 0; i < this.defHash.Length; i++)
			{
				num3 |= (int)(this.defHash[i] ^ array2[this.defHash.Length + i]);
			}
			if (num3 != 0)
			{
				throw new InvalidCipherTextException("data hash wrong");
			}
			int num4 = 2 * this.defHash.Length;
			while (num4 != array2.Length && array2[num4] == 0)
			{
				num4++;
			}
			if (num4 >= array2.Length - 1 || array2[num4] != 1)
			{
				throw new InvalidCipherTextException("data start wrong " + num4);
			}
			num4++;
			byte[] array4 = new byte[array2.Length - num4];
			Array.Copy(array2, num4, array4, 0, array4.Length);
			return array4;
		}

		private void ItoOSP(int i, byte[] sp)
		{
			sp[0] = (byte)((uint)i >> 24);
			sp[1] = (byte)((uint)i >> 16);
			sp[2] = (byte)((uint)i >> 8);
			sp[3] = (byte)i;
		}

		private byte[] maskGeneratorFunction1(byte[] Z, int zOff, int zLen, int length)
		{
			byte[] array = new byte[length];
			byte[] array2 = new byte[this.mgf1Hash.GetDigestSize()];
			byte[] array3 = new byte[4];
			int num = 0;
			this.hash.Reset();
			do
			{
				this.ItoOSP(num, array3);
				this.mgf1Hash.BlockUpdate(Z, zOff, zLen);
				this.mgf1Hash.BlockUpdate(array3, 0, array3.Length);
				this.mgf1Hash.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, num * array2.Length, array2.Length);
			}
			while (++num < length / array2.Length);
			if (num * array2.Length < length)
			{
				this.ItoOSP(num, array3);
				this.mgf1Hash.BlockUpdate(Z, zOff, zLen);
				this.mgf1Hash.BlockUpdate(array3, 0, array3.Length);
				this.mgf1Hash.DoFinal(array2, 0);
				Array.Copy(array2, 0, array, num * array2.Length, array.Length - num * array2.Length);
			}
			return array;
		}
	}
}
