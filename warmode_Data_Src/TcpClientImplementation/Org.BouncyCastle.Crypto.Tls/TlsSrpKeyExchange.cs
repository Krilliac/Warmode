using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsSrpKeyExchange : AbstractTlsKeyExchange
	{
		protected TlsSigner mTlsSigner;

		protected TlsSrpGroupVerifier mGroupVerifier;

		protected byte[] mIdentity;

		protected byte[] mPassword;

		protected AsymmetricKeyParameter mServerPublicKey;

		protected Srp6GroupParameters mSrpGroup;

		protected Srp6Client mSrpClient;

		protected Srp6Server mSrpServer;

		protected BigInteger mSrpPeerCredentials;

		protected BigInteger mSrpVerifier;

		protected byte[] mSrpSalt;

		protected TlsSignerCredentials mServerCredentials;

		public override bool RequiresServerKeyExchange
		{
			get
			{
				return true;
			}
		}

		protected static TlsSigner CreateSigner(int keyExchange)
		{
			switch (keyExchange)
			{
			case 21:
				return null;
			case 22:
				return new TlsDssSigner();
			case 23:
				return new TlsRsaSigner();
			default:
				throw new ArgumentException("unsupported key exchange algorithm");
			}
		}

		[Obsolete("Use constructor taking an explicit 'groupVerifier' argument")]
		public TlsSrpKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, byte[] identity, byte[] password) : this(keyExchange, supportedSignatureAlgorithms, new DefaultTlsSrpGroupVerifier(), identity, password)
		{
		}

		public TlsSrpKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, TlsSrpGroupVerifier groupVerifier, byte[] identity, byte[] password) : base(keyExchange, supportedSignatureAlgorithms)
		{
			this.mTlsSigner = TlsSrpKeyExchange.CreateSigner(keyExchange);
			this.mGroupVerifier = groupVerifier;
			this.mIdentity = identity;
			this.mPassword = password;
			this.mSrpClient = new Srp6Client();
		}

		public TlsSrpKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, byte[] identity, TlsSrpLoginParameters loginParameters) : base(keyExchange, supportedSignatureAlgorithms)
		{
			this.mTlsSigner = TlsSrpKeyExchange.CreateSigner(keyExchange);
			this.mIdentity = identity;
			this.mSrpServer = new Srp6Server();
			this.mSrpGroup = loginParameters.Group;
			this.mSrpVerifier = loginParameters.Verifier;
			this.mSrpSalt = loginParameters.Salt;
		}

		public override void Init(TlsContext context)
		{
			base.Init(context);
			if (this.mTlsSigner != null)
			{
				this.mTlsSigner.Init(context);
			}
		}

		public override void SkipServerCredentials()
		{
			if (this.mTlsSigner != null)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public override void ProcessServerCertificate(Certificate serverCertificate)
		{
			if (this.mTlsSigner == null)
			{
				throw new TlsFatalAlert(10);
			}
			if (serverCertificate.IsEmpty)
			{
				throw new TlsFatalAlert(42);
			}
			X509CertificateStructure certificateAt = serverCertificate.GetCertificateAt(0);
			SubjectPublicKeyInfo subjectPublicKeyInfo = certificateAt.SubjectPublicKeyInfo;
			try
			{
				this.mServerPublicKey = PublicKeyFactory.CreateKey(subjectPublicKeyInfo);
			}
			catch (Exception alertCause)
			{
				throw new TlsFatalAlert(43, alertCause);
			}
			if (!this.mTlsSigner.IsValidPublicKey(this.mServerPublicKey))
			{
				throw new TlsFatalAlert(46);
			}
			TlsUtilities.ValidateKeyUsage(certificateAt, 128);
			base.ProcessServerCertificate(serverCertificate);
		}

		public override void ProcessServerCredentials(TlsCredentials serverCredentials)
		{
			if (this.mKeyExchange == 21 || !(serverCredentials is TlsSignerCredentials))
			{
				throw new TlsFatalAlert(80);
			}
			this.ProcessServerCertificate(serverCredentials.Certificate);
			this.mServerCredentials = (TlsSignerCredentials)serverCredentials;
		}

		public override byte[] GenerateServerKeyExchange()
		{
			this.mSrpServer.Init(this.mSrpGroup, this.mSrpVerifier, TlsUtilities.CreateHash(2), this.mContext.SecureRandom);
			BigInteger b = this.mSrpServer.GenerateServerCredentials();
			ServerSrpParams serverSrpParams = new ServerSrpParams(this.mSrpGroup.N, this.mSrpGroup.G, this.mSrpSalt, b);
			DigestInputBuffer digestInputBuffer = new DigestInputBuffer();
			serverSrpParams.Encode(digestInputBuffer);
			if (this.mServerCredentials != null)
			{
				SignatureAndHashAlgorithm signatureAndHashAlgorithm = TlsUtilities.GetSignatureAndHashAlgorithm(this.mContext, this.mServerCredentials);
				IDigest digest = TlsUtilities.CreateHash(signatureAndHashAlgorithm);
				SecurityParameters securityParameters = this.mContext.SecurityParameters;
				digest.BlockUpdate(securityParameters.clientRandom, 0, securityParameters.clientRandom.Length);
				digest.BlockUpdate(securityParameters.serverRandom, 0, securityParameters.serverRandom.Length);
				digestInputBuffer.UpdateDigest(digest);
				byte[] array = new byte[digest.GetDigestSize()];
				digest.DoFinal(array, 0);
				byte[] signature = this.mServerCredentials.GenerateCertificateSignature(array);
				DigitallySigned digitallySigned = new DigitallySigned(signatureAndHashAlgorithm, signature);
				digitallySigned.Encode(digestInputBuffer);
			}
			return digestInputBuffer.ToArray();
		}

		public override void ProcessServerKeyExchange(Stream input)
		{
			SecurityParameters securityParameters = this.mContext.SecurityParameters;
			SignerInputBuffer signerInputBuffer = null;
			Stream input2 = input;
			if (this.mTlsSigner != null)
			{
				signerInputBuffer = new SignerInputBuffer();
				input2 = new TeeInputStream(input, signerInputBuffer);
			}
			ServerSrpParams serverSrpParams = ServerSrpParams.Parse(input2);
			if (signerInputBuffer != null)
			{
				DigitallySigned digitallySigned = DigitallySigned.Parse(this.mContext, input);
				ISigner signer = this.InitVerifyer(this.mTlsSigner, digitallySigned.Algorithm, securityParameters);
				signerInputBuffer.UpdateSigner(signer);
				if (!signer.VerifySignature(digitallySigned.Signature))
				{
					throw new TlsFatalAlert(51);
				}
			}
			this.mSrpGroup = new Srp6GroupParameters(serverSrpParams.N, serverSrpParams.G);
			if (!this.mGroupVerifier.Accept(this.mSrpGroup))
			{
				throw new TlsFatalAlert(71);
			}
			this.mSrpSalt = serverSrpParams.S;
			try
			{
				this.mSrpPeerCredentials = Srp6Utilities.ValidatePublicValue(this.mSrpGroup.N, serverSrpParams.B);
			}
			catch (CryptoException alertCause)
			{
				throw new TlsFatalAlert(47, alertCause);
			}
			this.mSrpClient.Init(this.mSrpGroup, TlsUtilities.CreateHash(2), this.mContext.SecureRandom);
		}

		public override void ValidateCertificateRequest(CertificateRequest certificateRequest)
		{
			throw new TlsFatalAlert(10);
		}

		public override void ProcessClientCredentials(TlsCredentials clientCredentials)
		{
			throw new TlsFatalAlert(80);
		}

		public override void GenerateClientKeyExchange(Stream output)
		{
			BigInteger x = this.mSrpClient.GenerateClientCredentials(this.mSrpSalt, this.mIdentity, this.mPassword);
			TlsSrpUtilities.WriteSrpParameter(x, output);
			this.mContext.SecurityParameters.srpIdentity = Arrays.Clone(this.mIdentity);
		}

		public override void ProcessClientKeyExchange(Stream input)
		{
			try
			{
				this.mSrpPeerCredentials = Srp6Utilities.ValidatePublicValue(this.mSrpGroup.N, TlsSrpUtilities.ReadSrpParameter(input));
			}
			catch (CryptoException alertCause)
			{
				throw new TlsFatalAlert(47, alertCause);
			}
			this.mContext.SecurityParameters.srpIdentity = Arrays.Clone(this.mIdentity);
		}

		public override byte[] GeneratePremasterSecret()
		{
			byte[] result;
			try
			{
				BigInteger n = (this.mSrpServer != null) ? this.mSrpServer.CalculateSecret(this.mSrpPeerCredentials) : this.mSrpClient.CalculateSecret(this.mSrpPeerCredentials);
				result = BigIntegers.AsUnsignedByteArray(n);
			}
			catch (CryptoException alertCause)
			{
				throw new TlsFatalAlert(47, alertCause);
			}
			return result;
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
