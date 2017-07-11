using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsECDHKeyExchange : AbstractTlsKeyExchange
	{
		protected TlsSigner mTlsSigner;

		protected int[] mNamedCurves;

		protected byte[] mClientECPointFormats;

		protected byte[] mServerECPointFormats;

		protected AsymmetricKeyParameter mServerPublicKey;

		protected TlsAgreementCredentials mAgreementCredentials;

		protected ECPrivateKeyParameters mECAgreePrivateKey;

		protected ECPublicKeyParameters mECAgreePublicKey;

		public override bool RequiresServerKeyExchange
		{
			get
			{
				switch (this.mKeyExchange)
				{
				case 17:
				case 19:
				case 20:
					return true;
				}
				return false;
			}
		}

		public TlsECDHKeyExchange(int keyExchange, IList supportedSignatureAlgorithms, int[] namedCurves, byte[] clientECPointFormats, byte[] serverECPointFormats) : base(keyExchange, supportedSignatureAlgorithms)
		{
			switch (keyExchange)
			{
			case 16:
			case 18:
				this.mTlsSigner = null;
				break;
			case 17:
				this.mTlsSigner = new TlsECDsaSigner();
				break;
			case 19:
				this.mTlsSigner = new TlsRsaSigner();
				break;
			default:
				throw new InvalidOperationException("unsupported key exchange algorithm");
			}
			this.mNamedCurves = namedCurves;
			this.mClientECPointFormats = clientECPointFormats;
			this.mServerECPointFormats = serverECPointFormats;
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
					this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey((ECPublicKeyParameters)this.mServerPublicKey);
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
					break;
				default:
					switch (b)
					{
					case 64:
					case 65:
					case 66:
						break;
					default:
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
				this.mECAgreePrivateKey = TlsEccUtilities.GenerateEphemeralClientKeyExchange(this.mContext.SecureRandom, this.mServerECPointFormats, this.mECAgreePublicKey.Parameters, output);
			}
		}

		public override void ProcessClientCertificate(Certificate clientCertificate)
		{
		}

		public override void ProcessClientKeyExchange(Stream input)
		{
			if (this.mECAgreePublicKey != null)
			{
				return;
			}
			byte[] encoding = TlsUtilities.ReadOpaque8(input);
			ECDomainParameters parameters = this.mECAgreePrivateKey.Parameters;
			this.mECAgreePublicKey = TlsEccUtilities.ValidateECPublicKey(TlsEccUtilities.DeserializeECPublicKey(this.mServerECPointFormats, parameters, encoding));
		}

		public override byte[] GeneratePremasterSecret()
		{
			if (this.mAgreementCredentials != null)
			{
				return this.mAgreementCredentials.GenerateAgreement(this.mECAgreePublicKey);
			}
			if (this.mECAgreePrivateKey != null)
			{
				return TlsEccUtilities.CalculateECDHBasicAgreement(this.mECAgreePublicKey, this.mECAgreePrivateKey);
			}
			throw new TlsFatalAlert(80);
		}
	}
}
