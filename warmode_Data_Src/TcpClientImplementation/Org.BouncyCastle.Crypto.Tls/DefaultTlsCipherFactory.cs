using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsCipherFactory : AbstractTlsCipherFactory
	{
		public override TlsCipher CreateCipher(TlsContext context, int encryptionAlgorithm, int macAlgorithm)
		{
			switch (encryptionAlgorithm)
			{
			case 0:
				return this.CreateNullCipher(context, macAlgorithm);
			case 1:
			case 3:
			case 4:
			case 5:
			case 6:
				break;
			case 2:
				return this.CreateRC4Cipher(context, 16, macAlgorithm);
			case 7:
				return this.CreateDesEdeCipher(context, macAlgorithm);
			case 8:
				return this.CreateAESCipher(context, 16, macAlgorithm);
			case 9:
				return this.CreateAESCipher(context, 32, macAlgorithm);
			case 10:
				return this.CreateCipher_Aes_Gcm(context, 16, 16);
			case 11:
				return this.CreateCipher_Aes_Gcm(context, 32, 16);
			case 12:
				return this.CreateCamelliaCipher(context, 16, macAlgorithm);
			case 13:
				return this.CreateCamelliaCipher(context, 32, macAlgorithm);
			case 14:
				return this.CreateSeedCipher(context, macAlgorithm);
			case 15:
				return this.CreateCipher_Aes_Ccm(context, 16, 16);
			case 16:
				return this.CreateCipher_Aes_Ccm(context, 16, 8);
			case 17:
				return this.CreateCipher_Aes_Ccm(context, 32, 16);
			case 18:
				return this.CreateCipher_Aes_Ccm(context, 32, 8);
			case 19:
				return this.CreateCipher_Camellia_Gcm(context, 16, 16);
			case 20:
				return this.CreateCipher_Camellia_Gcm(context, 32, 16);
			default:
				switch (encryptionAlgorithm)
				{
				case 100:
					return this.CreateSalsa20Cipher(context, 12, 32, macAlgorithm);
				case 101:
					return this.CreateSalsa20Cipher(context, 20, 32, macAlgorithm);
				case 102:
					return this.CreateChaCha20Poly1305(context);
				}
				break;
			}
			throw new TlsFatalAlert(80);
		}

		protected virtual TlsBlockCipher CreateAESCipher(TlsContext context, int cipherKeySize, int macAlgorithm)
		{
			return new TlsBlockCipher(context, this.CreateAesBlockCipher(), this.CreateAesBlockCipher(), this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm), cipherKeySize);
		}

		protected virtual TlsBlockCipher CreateCamelliaCipher(TlsContext context, int cipherKeySize, int macAlgorithm)
		{
			return new TlsBlockCipher(context, this.CreateCamelliaBlockCipher(), this.CreateCamelliaBlockCipher(), this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm), cipherKeySize);
		}

		protected virtual TlsCipher CreateChaCha20Poly1305(TlsContext context)
		{
			return new Chacha20Poly1305(context);
		}

		protected virtual TlsAeadCipher CreateCipher_Aes_Ccm(TlsContext context, int cipherKeySize, int macSize)
		{
			return new TlsAeadCipher(context, this.CreateAeadBlockCipher_Aes_Ccm(), this.CreateAeadBlockCipher_Aes_Ccm(), cipherKeySize, macSize);
		}

		protected virtual TlsAeadCipher CreateCipher_Aes_Gcm(TlsContext context, int cipherKeySize, int macSize)
		{
			return new TlsAeadCipher(context, this.CreateAeadBlockCipher_Aes_Gcm(), this.CreateAeadBlockCipher_Aes_Gcm(), cipherKeySize, macSize);
		}

		protected virtual TlsAeadCipher CreateCipher_Camellia_Gcm(TlsContext context, int cipherKeySize, int macSize)
		{
			return new TlsAeadCipher(context, this.CreateAeadBlockCipher_Camellia_Gcm(), this.CreateAeadBlockCipher_Camellia_Gcm(), cipherKeySize, macSize);
		}

		protected virtual TlsBlockCipher CreateDesEdeCipher(TlsContext context, int macAlgorithm)
		{
			return new TlsBlockCipher(context, this.CreateDesEdeBlockCipher(), this.CreateDesEdeBlockCipher(), this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm), 24);
		}

		protected virtual TlsNullCipher CreateNullCipher(TlsContext context, int macAlgorithm)
		{
			return new TlsNullCipher(context, this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm));
		}

		protected virtual TlsStreamCipher CreateRC4Cipher(TlsContext context, int cipherKeySize, int macAlgorithm)
		{
			return new TlsStreamCipher(context, this.CreateRC4StreamCipher(), this.CreateRC4StreamCipher(), this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm), cipherKeySize, false);
		}

		protected virtual TlsStreamCipher CreateSalsa20Cipher(TlsContext context, int rounds, int cipherKeySize, int macAlgorithm)
		{
			return new TlsStreamCipher(context, this.CreateSalsa20StreamCipher(rounds), this.CreateSalsa20StreamCipher(rounds), this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm), cipherKeySize, true);
		}

		protected virtual TlsBlockCipher CreateSeedCipher(TlsContext context, int macAlgorithm)
		{
			return new TlsBlockCipher(context, this.CreateSeedBlockCipher(), this.CreateSeedBlockCipher(), this.CreateHMacDigest(macAlgorithm), this.CreateHMacDigest(macAlgorithm), 16);
		}

		protected virtual IBlockCipher CreateAesEngine()
		{
			return new AesEngine();
		}

		protected virtual IBlockCipher CreateCamelliaEngine()
		{
			return new CamelliaEngine();
		}

		protected virtual IBlockCipher CreateAesBlockCipher()
		{
			return new CbcBlockCipher(this.CreateAesEngine());
		}

		protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Ccm()
		{
			return new CcmBlockCipher(this.CreateAesEngine());
		}

		protected virtual IAeadBlockCipher CreateAeadBlockCipher_Aes_Gcm()
		{
			return new GcmBlockCipher(this.CreateAesEngine());
		}

		protected virtual IAeadBlockCipher CreateAeadBlockCipher_Camellia_Gcm()
		{
			return new GcmBlockCipher(this.CreateCamelliaEngine());
		}

		protected virtual IBlockCipher CreateCamelliaBlockCipher()
		{
			return new CbcBlockCipher(this.CreateCamelliaEngine());
		}

		protected virtual IBlockCipher CreateDesEdeBlockCipher()
		{
			return new CbcBlockCipher(new DesEdeEngine());
		}

		protected virtual IStreamCipher CreateRC4StreamCipher()
		{
			return new RC4Engine();
		}

		protected virtual IStreamCipher CreateSalsa20StreamCipher(int rounds)
		{
			return new Salsa20Engine(rounds);
		}

		protected virtual IBlockCipher CreateSeedBlockCipher()
		{
			return new CbcBlockCipher(new SeedEngine());
		}

		protected virtual IDigest CreateHMacDigest(int macAlgorithm)
		{
			switch (macAlgorithm)
			{
			case 0:
				return null;
			case 1:
				return TlsUtilities.CreateHash(1);
			case 2:
				return TlsUtilities.CreateHash(2);
			case 3:
				return TlsUtilities.CreateHash(4);
			case 4:
				return TlsUtilities.CreateHash(5);
			case 5:
				return TlsUtilities.CreateHash(6);
			default:
				throw new TlsFatalAlert(80);
			}
		}
	}
}
