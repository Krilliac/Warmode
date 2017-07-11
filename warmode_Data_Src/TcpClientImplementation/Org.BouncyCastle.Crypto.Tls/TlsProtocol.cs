using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class TlsProtocol
	{
		internal class HandshakeMessage : MemoryStream
		{
			internal HandshakeMessage(byte handshakeType) : this(handshakeType, 60)
			{
			}

			internal HandshakeMessage(byte handshakeType, int length) : base(length + 4)
			{
				TlsUtilities.WriteUint8(handshakeType, this);
				TlsUtilities.WriteUint24(0, this);
			}

			internal void Write(byte[] data)
			{
				this.Write(data, 0, data.Length);
			}

			internal void WriteToRecordStream(TlsProtocol protocol)
			{
				long num = this.Length - 4L;
				TlsUtilities.CheckUint24(num);
				this.Position = 1L;
				TlsUtilities.WriteUint24((int)num, this);
				protocol.WriteHandshakeMessage(this.GetBuffer(), 0, (int)this.Length);
				this.Close();
			}
		}

		protected const short CS_START = 0;

		protected const short CS_CLIENT_HELLO = 1;

		protected const short CS_SERVER_HELLO = 2;

		protected const short CS_SERVER_SUPPLEMENTAL_DATA = 3;

		protected const short CS_SERVER_CERTIFICATE = 4;

		protected const short CS_CERTIFICATE_STATUS = 5;

		protected const short CS_SERVER_KEY_EXCHANGE = 6;

		protected const short CS_CERTIFICATE_REQUEST = 7;

		protected const short CS_SERVER_HELLO_DONE = 8;

		protected const short CS_CLIENT_SUPPLEMENTAL_DATA = 9;

		protected const short CS_CLIENT_CERTIFICATE = 10;

		protected const short CS_CLIENT_KEY_EXCHANGE = 11;

		protected const short CS_CERTIFICATE_VERIFY = 12;

		protected const short CS_CLIENT_FINISHED = 13;

		protected const short CS_SERVER_SESSION_TICKET = 14;

		protected const short CS_SERVER_FINISHED = 15;

		protected const short CS_END = 16;

		private static readonly string TLS_ERROR_MESSAGE = "Internal TLS error, this could be an attack";

		private ByteQueue mApplicationDataQueue = new ByteQueue();

		private ByteQueue mAlertQueue = new ByteQueue(2);

		private ByteQueue mHandshakeQueue = new ByteQueue();

		internal RecordStream mRecordStream;

		protected SecureRandom mSecureRandom;

		private TlsStream mTlsStream;

		private volatile bool mClosed;

		private volatile bool mFailedWithError;

		private volatile bool mAppDataReady;

		private volatile bool mSplitApplicationDataRecords = true;

		private byte[] mExpectedVerifyData;

		protected TlsSession mTlsSession;

		protected SessionParameters mSessionParameters;

		protected SecurityParameters mSecurityParameters;

		protected Certificate mPeerCertificate;

		protected int[] mOfferedCipherSuites;

		protected byte[] mOfferedCompressionMethods;

		protected IDictionary mClientExtensions;

		protected IDictionary mServerExtensions;

		protected short mConnectionState;

		protected bool mResumedSession;

		protected bool mReceivedChangeCipherSpec;

		protected bool mSecureRenegotiation;

		protected bool mAllowCertificateStatus;

		protected bool mExpectSessionTicket;

		protected abstract TlsContext Context
		{
			get;
		}

		internal abstract AbstractTlsContext ContextAdmin
		{
			get;
		}

		protected abstract TlsPeer Peer
		{
			get;
		}

		public virtual Stream Stream
		{
			get
			{
				return this.mTlsStream;
			}
		}

		protected internal virtual bool IsClosed
		{
			get
			{
				return this.mClosed;
			}
		}

		public TlsProtocol(Stream stream, SecureRandom secureRandom) : this(stream, stream, secureRandom)
		{
		}

		public TlsProtocol(Stream input, Stream output, SecureRandom secureRandom)
		{
			this.mRecordStream = new RecordStream(this, input, output);
			this.mSecureRandom = secureRandom;
		}

		protected virtual void HandleChangeCipherSpecMessage()
		{
		}

		protected abstract void HandleHandshakeMessage(byte type, byte[] buf);

		protected virtual void HandleWarningMessage(byte description)
		{
		}

		protected virtual void ApplyMaxFragmentLengthExtension()
		{
			if (this.mSecurityParameters.maxFragmentLength >= 0)
			{
				if (!MaxFragmentLength.IsValid((byte)this.mSecurityParameters.maxFragmentLength))
				{
					throw new TlsFatalAlert(80);
				}
				int plaintextLimit = 1 << (int)(8 + this.mSecurityParameters.maxFragmentLength);
				this.mRecordStream.SetPlaintextLimit(plaintextLimit);
			}
		}

		protected virtual void CheckReceivedChangeCipherSpec(bool expected)
		{
			if (expected != this.mReceivedChangeCipherSpec)
			{
				throw new TlsFatalAlert(10);
			}
		}

		protected virtual void CleanupHandshake()
		{
			if (this.mExpectedVerifyData != null)
			{
				Arrays.Fill(this.mExpectedVerifyData, 0);
				this.mExpectedVerifyData = null;
			}
			this.mSecurityParameters.Clear();
			this.mPeerCertificate = null;
			this.mOfferedCipherSuites = null;
			this.mOfferedCompressionMethods = null;
			this.mClientExtensions = null;
			this.mServerExtensions = null;
			this.mResumedSession = false;
			this.mReceivedChangeCipherSpec = false;
			this.mSecureRenegotiation = false;
			this.mAllowCertificateStatus = false;
			this.mExpectSessionTicket = false;
		}

		protected virtual void CompleteHandshake()
		{
			try
			{
				while (this.mConnectionState != 16)
				{
					bool arg_0A_0 = this.mClosed;
					this.SafeReadRecord();
				}
				this.mRecordStream.FinaliseHandshake();
				this.mSplitApplicationDataRecords = !TlsUtilities.IsTlsV11(this.Context);
				if (!this.mAppDataReady)
				{
					this.mAppDataReady = true;
					this.mTlsStream = new TlsStream(this);
				}
				if (this.mTlsSession != null)
				{
					if (this.mSessionParameters == null)
					{
						this.mSessionParameters = new SessionParameters.Builder().SetCipherSuite(this.mSecurityParameters.CipherSuite).SetCompressionAlgorithm(this.mSecurityParameters.CompressionAlgorithm).SetMasterSecret(this.mSecurityParameters.MasterSecret).SetPeerCertificate(this.mPeerCertificate).SetPskIdentity(this.mSecurityParameters.PskIdentity).SetSrpIdentity(this.mSecurityParameters.SrpIdentity).SetServerExtensions(this.mServerExtensions).Build();
						this.mTlsSession = new TlsSessionImpl(this.mTlsSession.SessionID, this.mSessionParameters);
					}
					this.ContextAdmin.SetResumableSession(this.mTlsSession);
				}
				this.Peer.NotifyHandshakeComplete();
			}
			finally
			{
				this.CleanupHandshake();
			}
		}

		protected internal void ProcessRecord(byte protocol, byte[] buf, int offset, int len)
		{
			switch (protocol)
			{
			case 20:
				this.ProcessChangeCipherSpec(buf, offset, len);
				return;
			case 21:
				this.mAlertQueue.AddData(buf, offset, len);
				this.ProcessAlert();
				return;
			case 22:
				this.mHandshakeQueue.AddData(buf, offset, len);
				this.ProcessHandshake();
				return;
			case 23:
				if (!this.mAppDataReady)
				{
					throw new TlsFatalAlert(10);
				}
				this.mApplicationDataQueue.AddData(buf, offset, len);
				this.ProcessApplicationData();
				return;
			case 24:
				if (!this.mAppDataReady)
				{
					throw new TlsFatalAlert(10);
				}
				return;
			default:
				return;
			}
		}

		private void ProcessHandshake()
		{
			bool flag;
			do
			{
				flag = false;
				if (this.mHandshakeQueue.Available >= 4)
				{
					byte[] array = new byte[4];
					this.mHandshakeQueue.Read(array, 0, 4, 0);
					byte b = TlsUtilities.ReadUint8(array, 0);
					int num = TlsUtilities.ReadUint24(array, 1);
					if (this.mHandshakeQueue.Available >= num + 4)
					{
						byte[] array2 = this.mHandshakeQueue.RemoveData(num, 4);
						this.CheckReceivedChangeCipherSpec(this.mConnectionState == 16 || b == 20);
						byte b2 = b;
						if (b2 != 0)
						{
							TlsContext context = this.Context;
							if (b == 20 && this.mExpectedVerifyData == null && context.SecurityParameters.MasterSecret != null)
							{
								this.mExpectedVerifyData = this.CreateVerifyData(!context.IsServer);
							}
							this.mRecordStream.UpdateHandshakeData(array, 0, 4);
							this.mRecordStream.UpdateHandshakeData(array2, 0, num);
						}
						this.HandleHandshakeMessage(b, array2);
						flag = true;
					}
				}
			}
			while (flag);
		}

		private void ProcessApplicationData()
		{
		}

		private void ProcessAlert()
		{
			while (this.mAlertQueue.Available >= 2)
			{
				byte[] array = this.mAlertQueue.RemoveData(2, 0);
				byte b = array[0];
				byte b2 = array[1];
				this.Peer.NotifyAlertReceived(b, b2);
				if (b == 2)
				{
					this.InvalidateSession();
					this.mFailedWithError = true;
					this.mClosed = true;
					this.mRecordStream.SafeClose();
					throw new IOException(TlsProtocol.TLS_ERROR_MESSAGE);
				}
				if (b2 == 0)
				{
					this.HandleClose(false);
				}
				this.HandleWarningMessage(b2);
			}
		}

		private void ProcessChangeCipherSpec(byte[] buf, int off, int len)
		{
			for (int i = 0; i < len; i++)
			{
				byte b = TlsUtilities.ReadUint8(buf, off + i);
				if (b != 1)
				{
					throw new TlsFatalAlert(50);
				}
				if (this.mReceivedChangeCipherSpec || this.mAlertQueue.Available > 0 || this.mHandshakeQueue.Available > 0)
				{
					throw new TlsFatalAlert(10);
				}
				this.mRecordStream.ReceivedReadCipherSpec();
				this.mReceivedChangeCipherSpec = true;
				this.HandleChangeCipherSpecMessage();
			}
		}

		protected internal virtual int ApplicationDataAvailable()
		{
			return this.mApplicationDataQueue.Available;
		}

		protected internal virtual int ReadApplicationData(byte[] buf, int offset, int len)
		{
			if (len < 1)
			{
				return 0;
			}
			while (this.mApplicationDataQueue.Available == 0)
			{
				if (this.mClosed)
				{
					if (this.mFailedWithError)
					{
						throw new IOException(TlsProtocol.TLS_ERROR_MESSAGE);
					}
					return 0;
				}
				else
				{
					this.SafeReadRecord();
				}
			}
			len = Math.Min(len, this.mApplicationDataQueue.Available);
			this.mApplicationDataQueue.RemoveData(buf, offset, len, 0);
			return len;
		}

		protected virtual void SafeReadRecord()
		{
			try
			{
				if (!this.mRecordStream.ReadRecord())
				{
					throw new EndOfStreamException();
				}
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				if (!this.mClosed)
				{
					this.FailWithError(2, tlsFatalAlert.AlertDescription, "Failed to read record", tlsFatalAlert);
				}
				throw tlsFatalAlert;
			}
			catch (Exception ex)
			{
				if (!this.mClosed)
				{
					this.FailWithError(2, 80, "Failed to read record", ex);
				}
				throw ex;
			}
		}

		protected virtual void SafeWriteRecord(byte type, byte[] buf, int offset, int len)
		{
			try
			{
				this.mRecordStream.WriteRecord(type, buf, offset, len);
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				if (!this.mClosed)
				{
					this.FailWithError(2, tlsFatalAlert.AlertDescription, "Failed to write record", tlsFatalAlert);
				}
				throw tlsFatalAlert;
			}
			catch (Exception ex)
			{
				if (!this.mClosed)
				{
					this.FailWithError(2, 80, "Failed to write record", ex);
				}
				throw ex;
			}
		}

		protected internal virtual void WriteData(byte[] buf, int offset, int len)
		{
			if (!this.mClosed)
			{
				while (len > 0)
				{
					if (this.mSplitApplicationDataRecords)
					{
						this.SafeWriteRecord(23, buf, offset, 1);
						offset++;
						len--;
					}
					if (len > 0)
					{
						int num = Math.Min(len, this.mRecordStream.GetPlaintextLimit());
						this.SafeWriteRecord(23, buf, offset, num);
						offset += num;
						len -= num;
					}
				}
				return;
			}
			if (this.mFailedWithError)
			{
				throw new IOException(TlsProtocol.TLS_ERROR_MESSAGE);
			}
			throw new IOException("Sorry, connection has been closed, you cannot write more data");
		}

		protected virtual void WriteHandshakeMessage(byte[] buf, int off, int len)
		{
			while (len > 0)
			{
				int num = Math.Min(len, this.mRecordStream.GetPlaintextLimit());
				this.SafeWriteRecord(22, buf, off, num);
				off += num;
				len -= num;
			}
		}

		protected virtual void FailWithError(byte alertLevel, byte alertDescription, string message, Exception cause)
		{
			if (!this.mClosed)
			{
				this.mClosed = true;
				if (alertLevel == 2)
				{
					this.InvalidateSession();
					this.mFailedWithError = true;
				}
				this.RaiseAlert(alertLevel, alertDescription, message, cause);
				this.mRecordStream.SafeClose();
				if (alertLevel != 2)
				{
					return;
				}
			}
			throw new IOException(TlsProtocol.TLS_ERROR_MESSAGE);
		}

		protected virtual void InvalidateSession()
		{
			if (this.mSessionParameters != null)
			{
				this.mSessionParameters.Clear();
				this.mSessionParameters = null;
			}
			if (this.mTlsSession != null)
			{
				this.mTlsSession.Invalidate();
				this.mTlsSession = null;
			}
		}

		protected virtual void ProcessFinishedMessage(MemoryStream buf)
		{
			if (this.mExpectedVerifyData == null)
			{
				throw new TlsFatalAlert(80);
			}
			byte[] b = TlsUtilities.ReadFully(this.mExpectedVerifyData.Length, buf);
			TlsProtocol.AssertEmpty(buf);
			if (!Arrays.ConstantTimeAreEqual(this.mExpectedVerifyData, b))
			{
				throw new TlsFatalAlert(51);
			}
		}

		protected virtual void RaiseAlert(byte alertLevel, byte alertDescription, string message, Exception cause)
		{
			this.Peer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
			byte[] buf = new byte[]
			{
				alertLevel,
				alertDescription
			};
			this.SafeWriteRecord(21, buf, 0, 2);
		}

		protected virtual void RaiseWarning(byte alertDescription, string message)
		{
			this.RaiseAlert(1, alertDescription, message, null);
		}

		protected virtual void SendCertificateMessage(Certificate certificate)
		{
			if (certificate == null)
			{
				certificate = Certificate.EmptyChain;
			}
			if (certificate.IsEmpty)
			{
				TlsContext context = this.Context;
				if (!context.IsServer)
				{
					ProtocolVersion serverVersion = this.Context.ServerVersion;
					if (serverVersion.IsSsl)
					{
						string message = serverVersion.ToString() + " client didn't provide credentials";
						this.RaiseWarning(41, message);
						return;
					}
				}
			}
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(11);
			certificate.Encode(handshakeMessage);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendChangeCipherSpecMessage()
		{
			byte[] array = new byte[]
			{
				1
			};
			this.SafeWriteRecord(20, array, 0, array.Length);
			this.mRecordStream.SentWriteCipherSpec();
		}

		protected virtual void SendFinishedMessage()
		{
			byte[] array = this.CreateVerifyData(this.Context.IsServer);
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(20, array.Length);
			handshakeMessage.Write(array, 0, array.Length);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual void SendSupplementalDataMessage(IList supplementalData)
		{
			TlsProtocol.HandshakeMessage handshakeMessage = new TlsProtocol.HandshakeMessage(23);
			TlsProtocol.WriteSupplementalData(handshakeMessage, supplementalData);
			handshakeMessage.WriteToRecordStream(this);
		}

		protected virtual byte[] CreateVerifyData(bool isServer)
		{
			TlsContext context = this.Context;
			string asciiLabel = isServer ? "server finished" : "client finished";
			byte[] sslSender = isServer ? TlsUtilities.SSL_SERVER : TlsUtilities.SSL_CLIENT;
			byte[] currentPrfHash = TlsProtocol.GetCurrentPrfHash(context, this.mRecordStream.HandshakeHash, sslSender);
			return TlsUtilities.CalculateVerifyData(context, asciiLabel, currentPrfHash);
		}

		public virtual void Close()
		{
			this.HandleClose(true);
		}

		protected virtual void HandleClose(bool user_canceled)
		{
			if (!this.mClosed)
			{
				if (user_canceled && !this.mAppDataReady)
				{
					this.RaiseWarning(90, "User canceled handshake");
				}
				this.FailWithError(1, 0, "Connection closed", null);
			}
		}

		protected internal virtual void Flush()
		{
			this.mRecordStream.Flush();
		}

		protected virtual short ProcessMaxFragmentLengthExtension(IDictionary clientExtensions, IDictionary serverExtensions, byte alertDescription)
		{
			short maxFragmentLengthExtension = TlsExtensionsUtilities.GetMaxFragmentLengthExtension(serverExtensions);
			if (maxFragmentLengthExtension >= 0 && (!MaxFragmentLength.IsValid((byte)maxFragmentLengthExtension) || (!this.mResumedSession && maxFragmentLengthExtension != TlsExtensionsUtilities.GetMaxFragmentLengthExtension(clientExtensions))))
			{
				throw new TlsFatalAlert(alertDescription);
			}
			return maxFragmentLengthExtension;
		}

		protected virtual void RefuseRenegotiation()
		{
			if (TlsUtilities.IsSsl(this.Context))
			{
				throw new TlsFatalAlert(40);
			}
			this.RaiseWarning(100, "Renegotiation not supported");
		}

		protected internal static void AssertEmpty(MemoryStream buf)
		{
			if (buf.Position < buf.Length)
			{
				throw new TlsFatalAlert(50);
			}
		}

		protected internal static byte[] CreateRandomBlock(bool useGmtUnixTime, IRandomGenerator randomGenerator)
		{
			byte[] array = new byte[32];
			randomGenerator.NextBytes(array);
			if (useGmtUnixTime)
			{
				TlsUtilities.WriteGmtUnixTime(array, 0);
			}
			return array;
		}

		protected internal static byte[] CreateRenegotiationInfo(byte[] renegotiated_connection)
		{
			return TlsUtilities.EncodeOpaque8(renegotiated_connection);
		}

		protected internal static void EstablishMasterSecret(TlsContext context, TlsKeyExchange keyExchange)
		{
			byte[] array = keyExchange.GeneratePremasterSecret();
			try
			{
				context.SecurityParameters.masterSecret = TlsUtilities.CalculateMasterSecret(context, array);
			}
			finally
			{
				if (array != null)
				{
					Arrays.Fill(array, 0);
				}
			}
		}

		protected internal static byte[] GetCurrentPrfHash(TlsContext context, TlsHandshakeHash handshakeHash, byte[] sslSender)
		{
			IDigest digest = handshakeHash.ForkPrfHash();
			if (sslSender != null && TlsUtilities.IsSsl(context))
			{
				digest.BlockUpdate(sslSender, 0, sslSender.Length);
			}
			return DigestUtilities.DoFinal(digest);
		}

		protected internal static IDictionary ReadExtensions(MemoryStream input)
		{
			if (input.Position >= input.Length)
			{
				return null;
			}
			byte[] buffer = TlsUtilities.ReadOpaque16(input);
			TlsProtocol.AssertEmpty(input);
			MemoryStream memoryStream = new MemoryStream(buffer, false);
			IDictionary dictionary = Platform.CreateHashtable();
			while (memoryStream.Position < memoryStream.Length)
			{
				int num = TlsUtilities.ReadUint16(memoryStream);
				byte[] value = TlsUtilities.ReadOpaque16(memoryStream);
				if (dictionary.Contains(num))
				{
					throw new TlsFatalAlert(47);
				}
				dictionary.Add(num, value);
			}
			return dictionary;
		}

		protected internal static IList ReadSupplementalDataMessage(MemoryStream input)
		{
			byte[] buffer = TlsUtilities.ReadOpaque24(input);
			TlsProtocol.AssertEmpty(input);
			MemoryStream memoryStream = new MemoryStream(buffer, false);
			IList list = Platform.CreateArrayList();
			while (memoryStream.Position < memoryStream.Length)
			{
				int dataType = TlsUtilities.ReadUint16(memoryStream);
				byte[] data = TlsUtilities.ReadOpaque16(memoryStream);
				list.Add(new SupplementalDataEntry(dataType, data));
			}
			return list;
		}

		protected internal static void WriteExtensions(Stream output, IDictionary extensions)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (int num in extensions.Keys)
			{
				byte[] buf = (byte[])extensions[num];
				TlsUtilities.CheckUint16(num);
				TlsUtilities.WriteUint16(num, memoryStream);
				TlsUtilities.WriteOpaque16(buf, memoryStream);
			}
			byte[] buf2 = memoryStream.ToArray();
			TlsUtilities.WriteOpaque16(buf2, output);
		}

		protected internal static void WriteSupplementalData(Stream output, IList supplementalData)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (SupplementalDataEntry supplementalDataEntry in supplementalData)
			{
				int dataType = supplementalDataEntry.DataType;
				TlsUtilities.CheckUint16(dataType);
				TlsUtilities.WriteUint16(dataType, memoryStream);
				TlsUtilities.WriteOpaque16(supplementalDataEntry.Data, memoryStream);
			}
			byte[] buf = memoryStream.ToArray();
			TlsUtilities.WriteOpaque24(buf, output);
		}

		protected internal static int GetPrfAlgorithm(TlsContext context, int ciphersuite)
		{
			bool flag = TlsUtilities.IsTlsV12(context);
			if (ciphersuite <= 107)
			{
				switch (ciphersuite)
				{
				case 59:
				case 60:
				case 61:
				case 62:
				case 63:
				case 64:
					break;
				default:
					switch (ciphersuite)
					{
					case 103:
					case 104:
					case 105:
					case 106:
					case 107:
						break;
					default:
						goto IL_380;
					}
					break;
				}
			}
			else
			{
				switch (ciphersuite)
				{
				case 156:
				case 158:
				case 160:
				case 162:
				case 164:
				case 168:
				case 170:
				case 172:
				case 186:
				case 187:
				case 188:
				case 189:
				case 190:
				case 191:
				case 192:
				case 193:
				case 194:
				case 195:
				case 196:
				case 197:
					goto IL_35F;
				case 157:
				case 159:
				case 161:
				case 163:
				case 165:
				case 169:
				case 171:
				case 173:
					break;
				case 166:
				case 167:
				case 174:
				case 176:
				case 178:
				case 180:
				case 182:
				case 184:
					goto IL_380;
				case 175:
				case 177:
				case 179:
				case 181:
				case 183:
				case 185:
					goto IL_379;
				default:
					switch (ciphersuite)
					{
					case 49187:
					case 49189:
					case 49191:
					case 49193:
					case 49195:
					case 49197:
					case 49199:
					case 49201:
					case 49266:
					case 49268:
					case 49270:
					case 49272:
					case 49274:
					case 49276:
					case 49278:
					case 49280:
					case 49282:
					case 49284:
					case 49286:
					case 49288:
					case 49290:
					case 49292:
					case 49294:
					case 49296:
					case 49298:
					case 49308:
					case 49309:
					case 49310:
					case 49311:
					case 49312:
					case 49313:
					case 49314:
					case 49315:
					case 49316:
					case 49317:
					case 49318:
					case 49319:
					case 49320:
					case 49321:
					case 49322:
					case 49323:
					case 49324:
					case 49325:
					case 49326:
					case 49327:
						goto IL_35F;
					case 49188:
					case 49190:
					case 49192:
					case 49194:
					case 49196:
					case 49198:
					case 49200:
					case 49202:
					case 49267:
					case 49269:
					case 49271:
					case 49273:
					case 49275:
					case 49277:
					case 49279:
					case 49281:
					case 49283:
					case 49285:
					case 49287:
					case 49289:
					case 49291:
					case 49293:
					case 49295:
					case 49297:
					case 49299:
						break;
					case 49203:
					case 49204:
					case 49205:
					case 49206:
					case 49207:
					case 49209:
					case 49210:
					case 49212:
					case 49213:
					case 49214:
					case 49215:
					case 49216:
					case 49217:
					case 49218:
					case 49219:
					case 49220:
					case 49221:
					case 49222:
					case 49223:
					case 49224:
					case 49225:
					case 49226:
					case 49227:
					case 49228:
					case 49229:
					case 49230:
					case 49231:
					case 49232:
					case 49233:
					case 49234:
					case 49235:
					case 49236:
					case 49237:
					case 49238:
					case 49239:
					case 49240:
					case 49241:
					case 49242:
					case 49243:
					case 49244:
					case 49245:
					case 49246:
					case 49247:
					case 49248:
					case 49249:
					case 49250:
					case 49251:
					case 49252:
					case 49253:
					case 49254:
					case 49255:
					case 49256:
					case 49257:
					case 49258:
					case 49259:
					case 49260:
					case 49261:
					case 49262:
					case 49263:
					case 49264:
					case 49265:
					case 49300:
					case 49302:
					case 49304:
					case 49306:
						goto IL_380;
					case 49208:
					case 49211:
					case 49301:
					case 49303:
					case 49305:
					case 49307:
						goto IL_379;
					default:
						switch (ciphersuite)
						{
						case 52243:
						case 52244:
						case 52245:
							goto IL_35F;
						default:
							goto IL_380;
						}
						break;
					}
					break;
				}
				if (flag)
				{
					return 2;
				}
				throw new TlsFatalAlert(47);
				IL_379:
				if (flag)
				{
					return 2;
				}
				return 0;
			}
			IL_35F:
			if (flag)
			{
				return 1;
			}
			throw new TlsFatalAlert(47);
			IL_380:
			if (flag)
			{
				return 1;
			}
			return 0;
		}
	}
}
