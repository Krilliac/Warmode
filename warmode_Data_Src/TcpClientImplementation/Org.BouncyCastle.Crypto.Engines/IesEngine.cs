using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class IesEngine
	{
		private readonly IBasicAgreement agree;

		private readonly IDerivationFunction kdf;

		private readonly IMac mac;

		private readonly BufferedBlockCipher cipher;

		private readonly byte[] macBuf;

		private bool forEncryption;

		private ICipherParameters privParam;

		private ICipherParameters pubParam;

		private IesParameters param;

		public IesEngine(IBasicAgreement agree, IDerivationFunction kdf, IMac mac)
		{
			this.agree = agree;
			this.kdf = kdf;
			this.mac = mac;
			this.macBuf = new byte[mac.GetMacSize()];
		}

		public IesEngine(IBasicAgreement agree, IDerivationFunction kdf, IMac mac, BufferedBlockCipher cipher)
		{
			this.agree = agree;
			this.kdf = kdf;
			this.mac = mac;
			this.macBuf = new byte[mac.GetMacSize()];
			this.cipher = cipher;
		}

		public virtual void Init(bool forEncryption, ICipherParameters privParameters, ICipherParameters pubParameters, ICipherParameters iesParameters)
		{
			this.forEncryption = forEncryption;
			this.privParam = privParameters;
			this.pubParam = pubParameters;
			this.param = (IesParameters)iesParameters;
		}

		private byte[] DecryptBlock(byte[] in_enc, int inOff, int inLen, byte[] z)
		{
			KdfParameters kdfParameters = new KdfParameters(z, this.param.GetDerivationV());
			int macKeySize = this.param.MacKeySize;
			this.kdf.Init(kdfParameters);
			inLen -= this.mac.GetMacSize();
			byte[] array2;
			KeyParameter parameters;
			if (this.cipher == null)
			{
				byte[] array = this.GenerateKdfBytes(kdfParameters, inLen + macKeySize / 8);
				array2 = new byte[inLen];
				for (int num = 0; num != inLen; num++)
				{
					array2[num] = (in_enc[inOff + num] ^ array[num]);
				}
				parameters = new KeyParameter(array, inLen, macKeySize / 8);
			}
			else
			{
				int cipherKeySize = ((IesWithCipherParameters)this.param).CipherKeySize;
				byte[] key = this.GenerateKdfBytes(kdfParameters, cipherKeySize / 8 + macKeySize / 8);
				this.cipher.Init(false, new KeyParameter(key, 0, cipherKeySize / 8));
				array2 = this.cipher.DoFinal(in_enc, inOff, inLen);
				parameters = new KeyParameter(key, cipherKeySize / 8, macKeySize / 8);
			}
			byte[] encodingV = this.param.GetEncodingV();
			this.mac.Init(parameters);
			this.mac.BlockUpdate(in_enc, inOff, inLen);
			this.mac.BlockUpdate(encodingV, 0, encodingV.Length);
			this.mac.DoFinal(this.macBuf, 0);
			inOff += inLen;
			byte[] a = Arrays.CopyOfRange(in_enc, inOff, inOff + this.macBuf.Length);
			if (!Arrays.ConstantTimeAreEqual(a, this.macBuf))
			{
				throw new InvalidCipherTextException("Invalid MAC.");
			}
			return array2;
		}

		private byte[] EncryptBlock(byte[] input, int inOff, int inLen, byte[] z)
		{
			KdfParameters kParam = new KdfParameters(z, this.param.GetDerivationV());
			int macKeySize = this.param.MacKeySize;
			byte[] array2;
			int num;
			KeyParameter parameters;
			if (this.cipher == null)
			{
				byte[] array = this.GenerateKdfBytes(kParam, inLen + macKeySize / 8);
				array2 = new byte[inLen + this.mac.GetMacSize()];
				num = inLen;
				for (int num2 = 0; num2 != inLen; num2++)
				{
					array2[num2] = (input[inOff + num2] ^ array[num2]);
				}
				parameters = new KeyParameter(array, inLen, macKeySize / 8);
			}
			else
			{
				int cipherKeySize = ((IesWithCipherParameters)this.param).CipherKeySize;
				byte[] key = this.GenerateKdfBytes(kParam, cipherKeySize / 8 + macKeySize / 8);
				this.cipher.Init(true, new KeyParameter(key, 0, cipherKeySize / 8));
				num = this.cipher.GetOutputSize(inLen);
				byte[] array3 = new byte[num];
				int num3 = this.cipher.ProcessBytes(input, inOff, inLen, array3, 0);
				num3 += this.cipher.DoFinal(array3, num3);
				array2 = new byte[num3 + this.mac.GetMacSize()];
				num = num3;
				Array.Copy(array3, 0, array2, 0, num3);
				parameters = new KeyParameter(key, cipherKeySize / 8, macKeySize / 8);
			}
			byte[] encodingV = this.param.GetEncodingV();
			this.mac.Init(parameters);
			this.mac.BlockUpdate(array2, 0, num);
			this.mac.BlockUpdate(encodingV, 0, encodingV.Length);
			this.mac.DoFinal(array2, num);
			return array2;
		}

		private byte[] GenerateKdfBytes(KdfParameters kParam, int length)
		{
			byte[] array = new byte[length];
			this.kdf.Init(kParam);
			this.kdf.GenerateBytes(array, 0, array.Length);
			return array;
		}

		public virtual byte[] ProcessBlock(byte[] input, int inOff, int inLen)
		{
			this.agree.Init(this.privParam);
			BigInteger n = this.agree.CalculateAgreement(this.pubParam);
			byte[] array = BigIntegers.AsUnsignedByteArray(this.agree.GetFieldSize(), n);
			byte[] result;
			try
			{
				result = (this.forEncryption ? this.EncryptBlock(input, inOff, inLen, array) : this.DecryptBlock(input, inOff, inLen, array));
			}
			finally
			{
				Array.Clear(array, 0, array.Length);
			}
			return result;
		}
	}
}
