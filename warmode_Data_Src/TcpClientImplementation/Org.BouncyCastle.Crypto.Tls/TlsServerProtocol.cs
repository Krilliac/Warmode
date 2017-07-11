using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsServerProtocol : TlsProtocol
	{
		protected TlsServer mTlsServer;

		internal TlsServerContextImpl mTlsServerContext;

		protected TlsKeyExchange mKeyExchange;

		protected TlsCredentials mServerCredentials;

		protected CertificateRequest mCertificateRequest;

		protected short mClientCertificateType = -1;

		protected TlsHandshakeHash mPrepareFinishHash;

		protected override TlsContext Context
		{
			get
			{
				return this.mTlsServerContext;
			}
		}

		internal override AbstractTlsContext ContextAdmin
		{
			get
			{
				return this.mTlsServerContext;
			}
		}

		protected override TlsPeer Peer
		{
			get
			{
				return this.mTlsServer;
			}
		}

		public TlsServerProtocol(Stream stream, SecureRandom secureRandom) : base(stream, secureRandom)
		{
		}

		public TlsServerProtocol(Stream input, Stream output, SecureRandom secureRandom) : base(input, output, secureRandom)
		{
		}

		public virtual void Accept(TlsServer tlsServer)
		{
			if (tlsServer == null)
			{
				throw new ArgumentNullException("tlsServer");
			}
			if (this.mTlsServer != null)
			{
				throw new InvalidOperationException("'Accept' can only be called once");
			}
			this.mTlsServer = tlsServer;
			this.mSecurityParameters = new SecurityParameters();
			this.mSecurityParameters.entity = 0;
			this.mTlsServerContext = new TlsServerContextImpl(this.mSecureRandom, this.mSecurityParameters);
			this.mSecurityParameters.serverRandom = TlsProtocol.CreateRandomBlock(tlsServer.ShouldUseGmtUnixTime(), this.mTlsServerContext.NonceRandomGenerator);
			this.mTlsServer.Init(this.mTlsServerContext);
			this.mRecordStream.Init(this.mTlsServerContext);
			this.mRecordStream.SetRestrictReadVersion(false);
			this.CompleteHandshake();
		}

		protected override void CleanupHandshake()
		{
			base.CleanupHandshake();
			this.mKeyExchange = null;
			this.mServerCredentials = null;
			this.mCertificateRequest = null;
			this.mPrepareFinishHash = null;
		}

		protected override void HandleHandshakeMessage(byte type, byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream(data);
			switch (type)
			{
			case 1:
			{
				short mConnectionState = this.mConnectionState;
				if (mConnectionState == 0)
				{
					this.ReceiveClientHelloMessage(memoryStream);
					this.mConnectionState = 1;
					this.SendServerHelloMessage();
					this.mConnectionState = 2;
					this.mRecordStream.NotifyHelloComplete();
					IList serverSupplementalData = this.mTlsServer.GetServerSupplementalData();
					if (serverSupplementalData != null)
					{
						this.SendSupplementalDataMessage(serverSupplementalData);
					}
					this.mConnectionState = 3;
					this.mKeyExchange = this.mTlsServer.GetKeyExchange();
					this.mKeyExchange.Init(this.Context);
					this.mServerCredentials = this.mTlsServer.GetCredentials();
					Certificate certificate = null;
					if (this.mServerCredentials == null)
					{
						this.mKeyExchange.SkipServerCredentials();
					}
					else
					{
						this.mKeyExchange.ProcessServerCredentials(this.mServerCredentials);
						certificate = this.mServerCredentials.Certificate;
						this.SendCertificateMessage(certificate);
					}
					this.mConnectionState = 4;
					if (certificate == null || certificate.IsEmpty)
					{
						this.mAllowCertificateStatus = false;
					}
					if (this.mAllowCertificateStatus)
					{
						CertificateStatus certificateStatus = this.mTlsServer.GetCertificateStatus();
						if (certificateStatus != null)
						{
							this.SendCertificateStatusMessage(certificateStatus);
						}
					}
					this.mConnectionState = 5;
					byte[] array = this.mKeyExchange.GenerateServerKeyExchange();
					if (array != null)
					{
						this.SendServerKeyExchangeMessage(array);
					}
					this.mConnectionState = 6;
					if (this.mServerCredentials != null)
					{
						this.mCertificateRequest = this.mTlsServer.GetCertificateRequest();
						if (this.mCertificateRequest != null)
						{
							this.mKeyExchange.ValidateCertificateRequest(this.mCertificateRequest);
							this.SendCertificateRequestMessage(this.mCertificateRequest);
							TlsUtilities.TrackHashAlgorithms(this.mRecordStream.HandshakeHash, this.mCertificateRequest.SupportedSignatureAlgorithms);
						}
					}
					this.mConnectionState = 7;
					this.SendServerHelloDoneMessage();
					this.mConnectionState = 8;
					this.mRecordStream.HandshakeHash.SealHashAlgorithms();
					return;
				}
				if (mConnectionState != 16)
				{
					throw new TlsFatalAlert(10);
				}
				this.RefuseRenegotiation();
				return;
			}
			case 11:
				switch (this.mConnectionState)
				{
				case 8:
				case 9:
					if (this.mConnectionState < 9)
					{
						this.mTlsServer.ProcessClientSupplementalData(null);
					}
					if (this.mCertificateRequest == null)
					{
						throw new TlsFatalAlert(10);
					}
					this.ReceiveCertificateMessage(memoryStream);
					this.mConnectionState = 10;
					return;
				default:
					throw new TlsFatalAlert(10);
				}
				break;
			case 15:
			{
				short mConnectionState2 = this.mConnectionState;
				if (mConnectionState2 != 11)
				{
					throw new TlsFatalAlert(10);
				}
				if (!this.ExpectCertificateVerifyMessage())
				{
					throw new TlsFatalAlert(10);
				}
				this.ReceiveCertificateVerifyMessage(memoryStream);
				this.mConnectionState = 12;
				return;
			}
			case 16:
				switch (this.mConnectionState)
				{
				case 8:
				case 9:
				case 10:
					if (this.mConnectionState < 9)
					{
						this.mTlsServer.ProcessClientSupplementalData(null);
					}
					if (this.mConnectionState < 10)
					{
						if (this.mCertificateRequest == null)
						{
							this.mKeyExchange.SkipClientCredentials();
						}
						else
						{
							if (TlsUtilities.IsTlsV12(this.Context))
							{
								throw new TlsFatalAlert(10);
							}
							if (TlsUtilities.IsSsl(this.Context))
							{
								if (this.mPeerCertificate == null)
								{
									throw new TlsFatalAlert(10);
								}
							}
							else
							{
								this.NotifyClientCertificate(Certificate.EmptyChain);
							}
						}
					}
					this.ReceiveClientKeyExchangeMessage(memoryStream);
					this.mConnectionState = 11;
					return;
				default:
					throw new TlsFatalAlert(10);
				}
				break;
			case 20:
				switch (this.mConnectionState)
				{
				case 11:
				case 12:
					if (this.mConnectionState < 12 && this.ExpectCertificateVerifyMessage())
					{
						throw new TlsFatalAlert(10);
					}
					this.ProcessFinishedMessage(memoryStream);
					this.mConnectionState = 13;
					if (this.mExpectSessionTicket)
					{
						this.SendNewSessionTicketMessage(this.mTlsServer.GetNewSessionTicket());
						this.SendChangeCipherSpecMessage();
					}
					this.mConnectionState = 14;
					this.SendFinishedMessage();
					this.mConnectionState = 15;
					this.mConnectionState = 16;
					return;
				default:
					throw new TlsFatalAlert(10);
				}
				break;
			case 23:
			{
				short mConnectionState3 = this.mConnectionState;
				if (mConnectionState3 == 8)
				{
					this.mTlsServer.ProcessClientSupplementalData(TlsProtocol.ReadSupplementalDataMessage(memoryStream));
					this.mConnectionState = 9;
					return;
				}
				throw new TlsFatalAlert(10);
			}
			}
			throw new TlsFatalAlert(10);
		}

		protected override void HandleWarningMessage(byte description)
		{
			if (description == 41)
			{
				if (TlsUtilities.IsSsl(this.Context) && this.mCertificateRequest != null)
				{
					this.NotifyClientCertificate(Certificate.EmptyChain);
					return;
				}
			}
			else
			{
				base.HandleWarningMessage(description);
			}
		}

		protected virtual void NotifyClientCertificate(Certificate clientCertificate)
		{
			if (this.mCertificateRequest == null)
			{
				throw new InvalidOperationException();
			}
			if (this.mPeerCertificate != null)
			{
				throw new TlsFatalAlert(10);
			}
			this.mPeerCertificate = clientCertificate;
			if (clientCertificate.IsEmpty)
			{
				this.mKeyExchange.SkipClientCredentials();
			}
			else
			{
				this.mClientCertificateType = TlsUtilities.GetClientCertificateType(clientCertificate, this.mServerCredentials.Certificate);
				this.mKeyExchange.ProcessClientCertificate(clientCertificate);
			}
			this.mTlsServer.NotifyClientCertificate(clientCertificate);
		}

		protected virtual void ReceiveCertificateMessage(MemoryStream buf)
		{
			Certificate clientCertificate = Certificate.Parse(buf);
			TlsProtocol.AssertEmpty(buf);
			this.NotifyClientCertificate(clientCertificate);
		}

		protected virtual void ReceiveCertificateVerifyMessage(MemoryStream buf)
		{
			DigitallySigned digitallySigned = DigitallySigned.Parse(this.Context, buf);
			TlsProtocol.AssertEmpty(buf);
			try
			{
				byte[] hash;
				if (TlsUtilities.IsTlsV12(this.Context))
				{
					hash = this.mPrepareFinishHash.GetFinalHash(digitallySigned.Algorithm.Hash);
				}
				else
				{
					hash = this.mSecurityParameters.SessionHash;
				}
				X509CertificateStructure certificateAt = this.mPeerCertificate.GetCertificateAt(0);
				SubjectPublicKeyInfo subjectPublicKeyInfo = certificateAt.SubjectPublicKeyInfo;
				AsymmetricKeyParameter publicKey = PublicKeyFactory.CreateKey(subjectPublicKeyInfo);
				TlsSigner tlsSigner = TlsUtilities.CreateTlsSigner((byte)this.mClientCertificateType);
				tlsSigner.Init(this.Context);
				if (!tlsSigner.VerifyRawSignature(digitallySigned.Algorithm, digitallySigned.Signature, publicKey, hash))
				{
					throw new TlsFatalAlert(51);
				}
			}
			catch (Exception alertCause)
			{
				throw new TlsFatalAlert(51, alertCause);
			}
		}

		protected virtual void ReceiveClientHelloMessage(MemoryStream buf)
		{
			ProtocolVersion protocolVersion = TlsUtilities.ReadVersion(buf);
			this.mRecordStream.SetWriteVersion(protocolVersion);
			if (protocolVersion.IsDtls)
			{
				throw new TlsFatalAlert(47);
			}
			byte[] clientRandom = TlsUtilities.ReadFully(32, buf);
			byte[] array = TlsUtilities.ReadOpaque8(buf);
			if (array.Length > 32)
			{
				throw new TlsFatalAlert(47);
			}
			int num = TlsUtilities.ReadUint16(buf);
			if (num < 2 || (num & 1) != 0)
			{
				throw new TlsFatalAlert(50);
			}
			this.mOfferedCipherSuites = TlsUtilities.ReadUint16Array(num / 2, buf);
			int num2 = (int)TlsUtilities.ReadUint8(buf);
			if (num2 < 1)
			{
				throw new TlsFatalAlert(47);
			}
			this.mOfferedCompressionMethods = TlsUtilities.ReadUint8Array(num2, buf);
			this.mClientExtensions = TlsProtocol.ReadExtensions(buf);
			this.mSecurityParameters.extendedMasterSecret = TlsExtensionsUtilities.HasExtendedMasterSecretExtension(this.mClientExtensions);
			this.ContextAdmin.SetClientVersion(protocolVersion);
			this.mTlsServer.NotifyClientVersion(protocolVersion);
			this.mTlsServer.NotifyFallback(Arrays.Contains(this.mOfferedCipherSuites, 22016));
			this.mSecurityParameters.clientRandom = clientRandom;
			this.mTlsServer.NotifyOfferedCipherSuites(this.mOfferedCipherSuites);
			this.mTlsServer.NotifyOfferedCompressionMethods(this.mOfferedCompressionMethods);
			if (Arrays.Contains(this.mOfferedCipherSuites, 255))
			{
				this.mSecureRenegotiation = true;
			}
			byte[] extensionData = TlsUtilities.GetExtensionData(this.mClientExtensions, 65281);
			if (extensionData != null)
			{
				this.mSecureRenegotiation = true;
				if (!Arrays.ConstantTimeAreEqual(extensionData, TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes)))
				{
					throw new TlsFatalAlert(40);
				}
			}
			this.mTlsServer.NotifySecureRenegotiation(this.mSecureRenegotiation);
			if (this.mClientExtensions != null)
			{
				this.mTlsServer.ProcessClientExtensions(this.mClientExtensions);
			}
		}

		protected virtual void ReceiveClientKeyExchangeMessage(MemoryStream buf)
		{
			this.mKeyExchange.ProcessClientKeyExchange(buf);
			TlsProtocol.AssertEmpty(buf);
			this.mPrepareFinishHash = this.mRecordStream.PrepareToFinish();
			this.mSecurityParameters.sessionHash = TlsProtocol.GetCurrentPrfHash(this.Context, this.mPrepareFinishHash, null);
			TlsProtocol.EstablishMasterSecret(this.Context, this.mKeyExchange);
			this.mRecordStream.SetPendingConnectionState(this.Peer.GetCompression(), this.Peer.GetCipher());
			if (!this.mExpectSessionTicket)
			{
				this.SendChangeCipherSpecMessage();
			}
		}

		protected virtual void SendCertificateRequestMessage(CertificateRequest certificateRequest)
		{
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(13);
			certificateRequest.Encode(handshakeMessage);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendCertificateStatusMessage(CertificateStatus certificateStatus)
		{
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(22);
			certificateStatus.Encode(handshakeMessage);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendNewSessionTicketMessage(NewSessionTicket newSessionTicket)
		{
			if (newSessionTicket == null)
			{
				throw new TlsFatalAlert(80);
			}
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(4);
			newSessionTicket.Encode(handshakeMessage);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendServerHelloMessage()
		{
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(2);
			ProtocolVersion serverVersion = this.mTlsServer.GetServerVersion();
			if (!serverVersion.IsEqualOrEarlierVersionOf(this.Context.ClientVersion))
			{
				throw new TlsFatalAlert(80);
			}
			this.mRecordStream.ReadVersion = serverVersion;
			this.mRecordStream.SetWriteVersion(serverVersion);
			this.mRecordStream.SetRestrictReadVersion(true);
			this.ContextAdmin.SetServerVersion(serverVersion);
			TlsUtilities.WriteVersion(serverVersion, handshakeMessage);
			handshakeMessage.Write(this.mSecurityParameters.serverRandom);
			TlsUtilities.WriteOpaque8(TlsUtilities.EmptyBytes, handshakeMessage);
			int selectedCipherSuite = this.mTlsServer.GetSelectedCipherSuite();
			if (!Arrays.Contains(this.mOfferedCipherSuites, selectedCipherSuite) || selectedCipherSuite == 0 || CipherSuite.IsScsv(selectedCipherSuite) || !TlsUtilities.IsValidCipherSuiteForVersion(selectedCipherSuite, this.Context.ServerVersion))
			{
				throw new TlsFatalAlert(80);
			}
			this.mSecurityParameters.cipherSuite = selectedCipherSuite;
			byte selectedCompressionMethod = this.mTlsServer.GetSelectedCompressionMethod();
			if (!Arrays.Contains(this.mOfferedCompressionMethods, selectedCompressionMethod))
			{
				throw new TlsFatalAlert(80);
			}
			this.mSecurityParameters.compressionAlgorithm = selectedCompressionMethod;
			TlsUtilities.WriteUint16(selectedCipherSuite, handshakeMessage);
			TlsUtilities.WriteUint8(selectedCompressionMethod, handshakeMessage);
			this.mServerExtensions = this.mTlsServer.GetServerExtensions();
			if (this.mSecureRenegotiation)
			{
				byte[] extensionData = TlsUtilities.GetExtensionData(this.mServerExtensions, 65281);
				bool flag = null == extensionData;
				if (flag)
				{
					this.mServerExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(this.mServerExtensions);
					this.mServerExtensions[65281] = TlsProtocol.CreateRenegotiationInfo(TlsUtilities.EmptyBytes);
				}
			}
			if (this.mSecurityParameters.extendedMasterSecret)
			{
				this.mServerExtensions = TlsExtensionsUtilities.EnsureExtensionsInitialised(this.mServerExtensions);
				TlsExtensionsUtilities.AddExtendedMasterSecretExtension(this.mServerExtensions);
			}
			if (this.mServerExtensions != null)
			{
				this.mSecurityParameters.encryptThenMac = TlsExtensionsUtilities.HasEncryptThenMacExtension(this.mServerExtensions);
				this.mSecurityParameters.maxFragmentLength = this.ProcessMaxFragmentLengthExtension(this.mClientExtensions, this.mServerExtensions, 80);
				this.mSecurityParameters.truncatedHMac = TlsExtensionsUtilities.HasTruncatedHMacExtension(this.mServerExtensions);
				this.mAllowCertificateStatus = (!this.mResumedSession && TlsUtilities.HasExpectedEmptyExtensionData(this.mServerExtensions, 5, 80));
				this.mExpectSessionTicket = (!this.mResumedSession && TlsUtilities.HasExpectedEmptyExtensionData(this.mServerExtensions, 35, 80));
				TlsProtocol.WriteExtensions(handshakeMessage, this.mServerExtensions);
			}
			this.mSecurityParameters.prfAlgorithm = TlsProtocol.GetPrfAlgorithm(this.Context, this.mSecurityParameters.CipherSuite);
			this.mSecurityParameters.verifyDataLength = 12;
			this.ApplyMaxFragmentLengthExtension();
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendServerHelloDoneMessage()
		{
			byte[] array = new byte[4];
			TlsUtilities.WriteUint8(14, array, 0);
			TlsUtilities.WriteUint24(0, array, 1);
			this.WriteHandshakeMessage(array, 0, array.Length);
		}

		protected virtual void SendServerKeyExchangeMessage(byte[] serverKeyExchange)
		{
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(12, serverKeyExchange.Length);
			handshakeMessage.Write(serverKeyExchange);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual bool ExpectCertificateVerifyMessage()
		{
			return this.mClientCertificateType >= 0 && TlsUtilities.HasSigningCapability((byte)this.mClientCertificateType);
		}
	}
}
