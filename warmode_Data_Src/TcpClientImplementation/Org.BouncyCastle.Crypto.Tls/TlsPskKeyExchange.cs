using Org.BouncyCastle.Asn1.X509;
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
	public class TlsPskKeyExchange : AbstractTlsKeyExchange
	{
		protected TlsPskIdentity mPskIdentity;

		protected TlsPskIdentityManager mPskIdentityManager;

		protected DHParameters mDHParameters;

		protected int[] mNamedCurves;

		protected byte[] mClientECPointFormats;

		protected byte[] mServerECPointFormats;

		protected byte[] mPskIdentityHint;

		protected byte[] mPsk;

		protected DHPrivateKeyParameters mDHAgreePrivateKey;

		protected DHPublicKeyParameters mDHAgreePublicKey;

		protected ECPrivateKeyParameters mECAgreePrivateKey;

		protected ECPublicKeyParameters mECAgreePublicKey;

		protected AsymmetricKeyParameter mServerPublicKey;

		protected RsaKeyParameters mRsaServerPublicKey;

		protected TlsEncryptionCredentials mServerCredentials;

		protected byte[] mPremasterSecret;

		public override bool RequiresServerKeyExchange
		{
			get
			{
				int mKeyExchange = this.mKeyExchange;
				return mKeyExchange == 14 || mKeyExchange == 24;
			}
		}

		public TlsPskKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, TlsPskIdentity pskIdentity, TlsPskIdentityManager pskIdentityManager, DHParameters dhParameters, int[] namedCurves, byte[] clientECPointFormats, byte[] serverECPointFormats) : base(keyExchange, supportedSignatureAlgorithms)
		{
			switch (keyExchange)
			{
			case 13:
			case 14:
			case 15:
				break;
			default:
				if (keyExchange != 24)
				{
					throw new InvalidOperationException("unsupported key exchange algorithm");
				}
				break;
			}
			this.mPskIdentity = pskIdentity;
			this.mPskIdentityManager = pskIdentityManager;
			this.mDHParameters = dhParameters;
			this.mNamedCurves = namedCurves;
			this.mClientECPointFormats = clientECPointFormats;
			this.mServerECPointFormats = serverECPointFormats;
		}

		public override void SkipServerCredentials()
		{
			if (this.mKeyExchange == 15)
			{
				throw new TlsFatalAlert(10);
			}
		}

		public override void ProcessServerCredentials(TlsCredentials serverCredentials)
		{
			if (!(serverCredentials is TlsEncryptionCredentials))
			{
				throw new TlsFatalAlert(80);
			}
			this.ProcessServerCertificate(serverCredentials.Certificate);
			this.mServerCredentials = (TlsEncryptionCredentials)serverCredentials;
		}

		public override byte[] GenerateServerKeyExchange()
		{
			this.mPskIdentityHint = this.mPskIdentityManager.GetHint();
			if (this.mPskIdentityHint == null && !this.RequiresServerKeyExchange)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			if (this.mPskIdentityHint == null)
			{
				TlsUtilities.WriteOpaque16(TlsUtilities.EmptyBytes, memoryStream);
			}
			else
			{
				TlsUtilities.WriteOpaque16(this.mPskIdentityHint, memoryStream);
			}
			if (this.mKeyExchange == 14)
			{
				if (this.mDHParameters == null)
				{
					throw new TlsFatalAlert(80);
				}
				this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralServerKeyExchange(this.mContext.SecureRandom, this.mDHParameters, memoryStream);
			}
			else if (this.mKeyExchange == 24)
			{
				this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralServerKeyExchange(this.mContext.SecureRandom, this.mNamedCurves, this.mClientECPointFormats, memoryStream);
			}
			return memoryStream.ToArray();
		}

		public override void ProcessServerCertificate(Certificate serverCertificate)
		{
			if (this.mKeyExchange != 15)
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
			if (this.mServerPublicKey.IsPrivate)
			{
				throw new TlsFatalAlert(80);
			}
			this.mRsaServerPublicKey = this.ValidateRsaPublicKey((RsaKeyParameters)this.mServerPublicKey);
			TlsUtilities.ValidateKeyUsage(certificateAt, 32);
			base.ProcessServerCertificate(serverCertificate);
		}

		public override void ProcessServerKeyExchange(Stream input)
		{
			this.mPskIdentityHint = TlsUtilities.ReadOpaque16(input);
			if (this.mKeyExchange == 14)
			{
				ServerDHParams serverDHParams = ServerDHParams.Parse(input);
				this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(serverDHParams.PublicKey);
				this.mDHParameters = this.mDHAgreePublicKey.Parameters;
				return;
			}
			if (this.mKeyExchange == 24)
			{
				ECDomainParameters curve_params = TlsEccUtilities.ReadECParameters(this.mNamedCurves, this.mClientECPointFormats, input);
				byte[] encoding = TlsUtilities.ReadOpaque8(input);
				this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(this.mClientECPointFormats, curve_params, encoding));
			}
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
			if (this.mPskIdentityHint == null)
			{
				this.mPskIdentity.SkipIdentityHint();
			}
			else
			{
				this.mPskIdentity.NotifyIdentityHint(this.mPskIdentityHint);
			}
			byte[] pskIdentity = this.mPskIdentity.GetPskIdentity();
			if (pskIdentity == null)
			{
				throw new TlsFatalAlert(80);
			}
			this.mPsk = this.mPskIdentity.GetPsk();
			if (this.mPsk == null)
			{
				throw new TlsFatalAlert(80);
			}
			TlsUtilities.WriteOpaque16(pskIdentity, output);
			this.mContext.SecurityParameters.pskIdentity = pskIdentity;
			if (this.mKeyExchange == 14)
			{
				this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralClientKeyExchange(this.mContext.SecureRandom, this.mDHParameters, output);
				return;
			}
			if (this.mKeyExchange == 24)
			{
				this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralClientKeyExchange(this.mContext.SecureRandom, this.mServerECPointFormats, this.mECAgreePublicKey.Parameters, output);
				return;
			}
			if (this.mKeyExchange == 15)
			{
				this.mPremasterSecret = TlsRsaUtilities.GenerateEncryptedPreMasterSecret(this.mContext, this.mRsaServerPublicKey, output);
			}
		}

		public override void ProcessClientKeyExchange(Stream input)
		{
			byte[] array = TlsUtilities.ReadOpaque16(input);
			this.mPsk = this.mPskIdentityManager.GetPsk(array);
			if (this.mPsk == null)
			{
				throw new TlsFatalAlert(115);
			}
			this.mContext.SecurityParameters.pskIdentity = array;
			if (this.mKeyExchange == 14)
			{
				BigInteger y = TlsDHUtilities.ReadDHParameter(input);
				this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(new DHPublicKeyParameters(y, this.mDHParameters));
				return;
			}
			if (this.mKeyExchange == 24)
			{
				byte[] encoding = TlsUtilities.ReadOpaque8(input);
				ECDomainParameters parameters = this.mECAgreePrivateKey.Parameters;
				this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(this.mServerECPointFormats, parameters, encoding));
				return;
			}
			if (this.mKeyExchange == 15)
			{
				byte[] encryptedPreMasterSecret;
				if (TlsUtilities.IsSsl(this.mContext))
				{
					encryptedPreMasterSecret = Streams.ReadAll(input);
				}
				else
				{
					encryptedPreMasterSecret = TlsUtilities.ReadOpaque16(input);
				}
				this.mPremasterSecret = this.mServerCredentials.DecryptPreMasterSecret(encryptedPreMasterSecret);
			}
		}

		public override byte[] GeneratePremasterSecret()
		{
			byte[] array = this.GenerateOtherSecret(this.mPsk.Length);
			MemoryStream memoryStream = new MemoryStream(4 + array.Length + this.mPsk.Length);
			TlsUtilities.WriteOpaque16(array, memoryStream);
			TlsUtilities.WriteOpaque16(this.mPsk, memoryStream);
			Arrays.Fill(this.mPsk, 0);
			this.mPsk = null;
			return memoryStream.ToArray();
		}

		protected virtual byte[] GenerateOtherSecret(int pskLength)
		{
			if (this.mKeyExchange == 14)
			{
				if (this.mDHAgreePrivateKey != null)
				{
					return TlsDHUtilities.CalculateDHBasicAgreement(this.mDHAgreePublicKey, this.mDHAgreePrivateKey);
				}
				throw new TlsFatalAlert(80);
			}
			else if (this.mKeyExchange == 24)
			{
				if (this.mECAgreePrivateKey != null)
				{
					return TlsEccUtilities.CalculateECDHBasicAgreement(this.mECAgreePublicKey, this.mECAgreePrivateKey);
				}
				throw new TlsFatalAlert(80);
			}
			else
			{
				if (this.mKeyExchange == 15)
				{
					return this.mPremasterSecret;
				}
				return new byte[pskLength];
			}
		}

		protected virtual RsaKeyParameters ValidateRsaPublicKey(RsaKeyParameters key)
		{
			if (!key.Exponent.IsProbablePrime(2))
			{
				throw new TlsFatalAlert(47);
			}
			return key;
		}
	}
}
