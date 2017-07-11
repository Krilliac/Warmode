using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsDheKeyExchange : TlsDHKeyExchange
	{
		protected TlsSignerCredentials mServerCredentials;

		public TlsDheKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, DHParameters dhParameters) : base(keyExchange, supportedSignatureAlgorithms, dhParameters)
		{
		}

		public override void ProcessServerCredentials(TlsCredentials serverCredentials)
		{
			if (!(serverCredentials is TlsSignerCredentials))
			{
				throw new TlsFatalAlert(80);
			}
			this.ProcessServerCertificate(serverCredentials.Certificate);
			this.mServerCredentials = (TlsSignerCredentials)serverCredentials;
		}

		public override byte[] GenerateServerKeyExchange()
		{
			if (this.mDHParameters == null)
			{
				throw new TlsFatalAlert(80);
			}
			DigestInputBuffer digestInputBuffer = new DigestInputBuffer();
			this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralServerKeyExchange(this.mContext.SecureRandom, this.mDHParameters, digestInputBuffer);
			SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(this.mContext, this.mServerCredentials);
			IDigest digest = TlsUtilities.CreateHash(signatureAndHashAlgorithm);
			SecurityParameters securityParameters = this.mContext.SecurityParameters;
			digest.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
			digest.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
			digestInputBuffer.UpdateDigest(digest);
			byte[] hash = DigestUtilities.DoFinal(digest);
			byte[] signature = this.mServerCredentials.GenerateCertificateSignature(hash);
			DigitallySigned digitallySigned = new DigitallySigned(signatureAndHashAlgorithm, signature);
			digitallySigned.Encode(digestInputBuffer);
			return digestInputBuffer.ToArray();
		}

		public override void ProcessServerKeyExchange(Stream input)
		{
			SecurityParameters securityParameters = this.mContext.SecurityParameters;
			SignerInputBuffer signerInputBuffer = new SignerInputBuffer();
			Stream input2 = new TeeInputStream(input, signerInputBuffer);
			ServerDHParams serverDHParams = ServerDHParams.Parse(input2);
			DigitallySigned digitallySigned = DigitallySigned.Parse(this.mContext, input);
			ISigner signer = this.InitVerifyer(this.mTlsSigner, digitallySigned.Algorithm, securityParameters);
			signerInputBuffer.UpdateSigner(signer);
			if (!signer.VerifySignature(digitallySigned.Signature))
			{
				throw new TlsFatalAlert(51);
			}
			this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(serverDHParams.PublicKey);
			this.mDHParameters = this.mDHAgreePublicKey.Parameters;
		}

		protected virtual ISigner InitVerifyer(TlsSigner tlsSigner, SignatureAndHashAlgorithm algorithm, SecurityParameters securityParameters)
		{
			ISigner signer = tlsSigner.CreateVerifyer(algorithm, this.mServerPublicKey);
			signer.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
			signer.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
			return signer;
		}
	}
}
