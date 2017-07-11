using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsDHKeyExchange : AbstractTlsKeyExchange
	{
		protected TlsSigner mTlsSigner;

		protected DHParameters mDHParameters;

		protected AsymmetricKeyParameter mServerPublicKey;

		protected TlsAgreementCredentials mAgreementCredentials;

		protected DHPrivateKeyParameters mDHAgreePrivateKey;

		protected DHPublicKeyParameters mDHAgreePublicKey;

		public override bool RequiresServerKeyExchange
		{
			get
			{
				int mKeyExchange = this.mKeyExchange;
				switch (mKeyExchange)
				{
				case 3:
				case 5:
					break;
				case 4:
					return false;
				default:
					if (mKeyExchange != 11)
					{
						return false;
					}
					break;
				}
				return true;
			}
		}

		public TlsDHKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, DHParameters dhParameters) : base(keyExchange, supportedSignatureAlgorithms)
		{
			switch (keyExchange)
			{
			case 3:
				this.mTlsSigner = new TlsDssSigner();
				goto IL_5E;
			case 5:
				this.mTlsSigner = new TlsRsaSigner();
				goto IL_5E;
			case 7:
			case 9:
				this.mTlsSigner = null;
				goto IL_5E;
			}
			throw new InvalidOperationException("unsupported key exchange algorithm");
			IL_5E:
			this.mDHParameters = dhParameters;
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
			throw new TlsFatalAlert(10);
		}

		public override void ProcessServerCertificate(Certificate serverCertificate)
		{
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
			if (this.mTlsSigner == null)
			{
				try
				{
					this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey((DHPublicKeyParameters)this.mServerPublicKey);
				}
				catch (InvalidCastException alertCause2)
				{
					throw new TlsFatalAlert(46, alertCause2);
				}
				TlsUtilities.ValidateKeyUsage(certificateAt, 8);
			}
			else
			{
				if (!this.mTlsSigner.IsValidPublicKey(this.mServerPublicKey))
				{
					throw new TlsFatalAlert(46);
				}
				TlsUtilities.ValidateKeyUsage(certificateAt, 128);
			}
			base.ProcessServerCertificate(serverCertificate);
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
				case 3:
				case 4:
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
			if (clientCredentials is TlsAgreementCredentials)
			{
				this.mAgreementCredentials = (TlsAgreementCredentials)clientCredentials;
				return;
			}
			if (clientCredentials is TlsSignerCredentials)
			{
				return;
			}
			throw new TlsFatalAlert(80);
		}

		public override void GenerateClientKeyExchange(Stream output)
		{
			if (this.mAgreementCredentials == null)
			{
				this.mDHAgreePrivateKey = TlsDHUtilities.GenerateEphemeralClientKeyExchange(this.mContext.SecureRandom, this.mDHParameters, output);
			}
		}

		public override void ProcessClientCertificate(Certificate clientCertificate)
		{
		}

		public override void ProcessClientKeyExchange(Stream input)
		{
			if (this.mDHAgreePublicKey != null)
			{
				return;
			}
			BigInteger y = TlsDHUtilities.ReadDHParameter(input);
			this.mDHAgreePublicKey = TlsDHUtilities.ValidateDHPublicKey(new DHPublicKeyParameters(y, this.mDHParameters));
		}

		public override byte[] GeneratePremasterSecret()
		{
			if (this.mAgreementCredentials != null)
			{
				return this.mAgreementCredentials.GenerateAgreement(this.mDHAgreePublicKey);
			}
			if (this.mDHAgreePrivateKey != null)
			{
				return TlsDHUtilities.CalculateDHBasicAgreement(this.mDHAgreePublicKey, this.mDHAgreePrivateKey);
			}
			throw new TlsFatalAlert(80);
		}
	}
}
