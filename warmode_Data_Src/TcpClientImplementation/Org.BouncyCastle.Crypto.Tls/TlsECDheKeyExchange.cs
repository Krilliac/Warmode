using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsECDheKeyExchange : TlsECDHKeyExchange
	{
		protected TlsSignerCredentials mServerCredentials;

		public TlsECDheKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, int[] namedCurves, byte[] clientECPointFormats, byte[] serverECPointFormats) : base(keyExchange, supportedSignatureAlgorithms, namedCurves, clientECPointFormats, serverECPointFormats)
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
			DigestInputBuffer digestInputBuffer = new DigestInputBuffer();
			this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralServerKeyExchange(this.mContext.SecureRandom, this.mNamedCurves, this.mClientECPointFormats, digestInputBuffer);
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
			ECDomainParameters curve_params = TlsEccUtilities.ReadECParameters(this.mNamedCurves, this.mClientECPointFormats, input2);
			byte[] encoding = TlsUtilities.ReadOpaque8(input2);
			DigitallySigned digitallySigned = DigitallySigned.Parse(this.mContext, input);
			ISigner signer = this.InitVerifyer(this.mTlsSigner, digitallySigned.Algorithm, securityParameters);
			signerInputBuffer.UpdateSigner(signer);
			if (!signer.VerifySignature(digitallySigned.Signature))
			{
				throw new TlsFatalAlert(51);
			}
			this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(this.mClientECPointFormats, curve_params, encoding));
		}

		public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
		{
			byte[] certificateTypes = certificateRequest.CertificateTypes;
			for (int i = 0; i < certificateTypes.Length; i++)
			{
				byte b = certificateTypes[i];
				switch (b)
				{
				case 1:
				case 2:
					break;
				default:
					if (b != 64)
					{
						throw new TlsFatalAlert(47);
					}
					break;
				}
			}
		}

		public override void ProcessClientCredentials(TlsCredentials clientCredentials)
		{
			if (clientCredentials is TlsSignerCredentials)
			{
				return;
			}
			throw new TlsFatalAlert(80);
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
