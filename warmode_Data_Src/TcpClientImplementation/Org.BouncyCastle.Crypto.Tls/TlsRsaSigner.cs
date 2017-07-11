using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsRsaSigner : AbstractTlsSigner
	{
		public override byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter privateKey, byte[] hash)
		{
			ISigner signer = this.MakeSigner(algorithm, true, true, new ParametersWithRandom(privateKey, this.mContext.SecureRandom));
			signer.BlockUpdate(hash, 0, hash.Length);
			return signer.GenerateSignature();
		}

		public override bool VerifyRawSignature(SignatureAndHashAlgorithm algorithm, byte[] sigBytes, AsymmetricKeyParameter publicKey, byte[] hash)
		{
			ISigner signer = this.MakeSigner(algorithm, true, false, publicKey);
			signer.BlockUpdate(hash, 0, hash.Length);
			return signer.VerifySignature(sigBytes);
		}

		public override ISigner CreateSigner(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter privateKey)
		{
			return this.MakeSigner(algorithm, false, true, new ParametersWithRandom(privateKey, this.mContext.SecureRandom));
		}

		public override ISigner CreateVerifyer(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter publicKey)
		{
			return this.MakeSigner(algorithm, false, false, publicKey);
		}

		public override bool IsValidPublicKey(AsymmetricKeyParameter publicKey)
		{
			return publicKey is RsaKeyParameters && !publicKey.IsPrivate;
		}

		protected virtual ISigner MakeSigner(SignatureAndHashAlgorithm algorithm, bool raw, bool forSigning, ICipherParameters cp)
		{
			if (algorithm != null != TlsUtilities.IsTlsV12(this.mContext))
			{
				throw new InvalidOperationException();
			}
			if (algorithm != null && algorithm.Signature != 1)
			{
				throw new InvalidOperationException();
			}
			IDigest digest;
			if (raw)
			{
				digest = new NullDigest();
			}
			else if (algorithm == null)
			{
				digest = new CombinedHash();
			}
			else
			{
				digest = TlsUtilities.CreateHash(algorithm.Hash);
			}
			ISigner signer;
			if (algorithm != null)
			{
				signer = new RsaDigestSigner(digest, TlsUtilities.GetOidForHashAlgorithm(algorithm.Hash));
			}
			else
			{
				signer = new GenericSigner(this.CreateRsaImpl(), digest);
			}
			signer.Init(forSigning, cp);
			return signer;
		}

		protected virtual IAsymmetricBlockCipher CreateRsaImpl()
		{
			return new Pkcs1Encoding(new RsaBlindedEngine());
		}
	}
}
