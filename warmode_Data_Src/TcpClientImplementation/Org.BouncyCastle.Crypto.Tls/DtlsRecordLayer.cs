using Org.BouncyCastle.Utilities.Date;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsRecordLayer : DatagramTransport
	{
		private const int RECORD_HEADER_LENGTH = 13;

		private const int MAX_FRAGMENT_LENGTH = 16384;

		private const long TCP_MSL = 120000L;

		private const long RETRANSMIT_TIMEOUT = 240000L;

		private readonly DatagramTransport mTransport;

		private readonly TlsContext mContext;

		private readonly TlsPeer mPeer;

		private readonly ByteQueue mRecordQueue = new ByteQueue();

		private volatile bool mClosed;

		private volatile bool mFailed;

		private volatile ProtocolVersion mDiscoveredPeerVersion;

		private volatile bool mInHandshake;

		private volatile int mPlaintextLimit;

		private DtlsEpoch mCurrentEpoch;

		private DtlsEpoch mPendingEpoch;

		private DtlsEpoch mReadEpoch;

		private DtlsEpoch mWriteEpoch;

		private DtlsHandshakeRetransmit mRetransmit;

		private DtlsEpoch mRetransmitEpoch;

		private long mRetransmitExpiry;

		internal virtual ProtocolVersion DiscoveredPeerVersion
		{
			get
			{
				return this.mDiscoveredPeerVersion;
			}
		}

		internal DtlsRecordLayer(DatagramTransport transport, TlsContext context, TlsPeer peer, byte contentType)
		{
			this.mTransport = transport;
			this.mContext = context;
			this.mPeer = peer;
			this.mInHandshake = true;
			this.mCurrentEpoch = new DtlsEpoch(0, new TlsNullCipher(context));
			this.mPendingEpoch = null;
			this.mReadEpoch = this.mCurrentEpoch;
			this.mWriteEpoch = this.mCurrentEpoch;
			this.SetPlaintextLimit(16384);
		}

		internal virtual void SetPlaintextLimit(int plaintextLimit)
		{
			this.mPlaintextLimit = plaintextLimit;
		}

		internal virtual ProtocolVersion ResetDiscoveredPeerVersion()
		{
			ProtocolVersion result = this.mDiscoveredPeerVersion;
			this.mDiscoveredPeerVersion = null;
			return result;
		}

		internal virtual void InitPendingEpoch(TlsCipher pendingCipher)
		{
			if (this.mPendingEpoch != null)
			{
				throw new InvalidOperationException();
			}
			this.mPendingEpoch = new DtlsEpoch(this.mWriteEpoch.Epoch + 1, pendingCipher);
		}

		internal virtual void HandshakeSuccessful(DtlsHandshakeRetransmit retransmit)
		{
			if (this.mReadEpoch == this.mCurrentEpoch || this.mWriteEpoch == this.mCurrentEpoch)
			{
				throw new InvalidOperationException();
			}
			if (retransmit != null)
			{
				this.mRetransmit = retransmit;
				this.mRetransmitEpoch = this.mCurrentEpoch;
				this.mRetransmitExpiry = DateTimeUtilities.CurrentUnixMs() + 240000L;
			}
			this.mInHandshake = false;
			this.mCurrentEpoch = this.mPendingEpoch;
			this.mPendingEpoch = null;
		}

		internal virtual void ResetWriteEpoch()
		{
			if (this.mRetransmitEpoch != null)
			{
				this.mWriteEpoch = this.mRetransmitEpoch;
				return;
			}
			this.mWriteEpoch = this.mCurrentEpoch;
		}

		public virtual int GetReceiveLimit()
		{
			return Math.Min(this.mPlaintextLimit, this.mReadEpoch.Cipher.GetPlaintextLimit(this.mTransport.GetReceiveLimit() - 13));
		}

		public virtual int GetSendLimit()
		{
			return Math.Min(this.mPlaintextLimit, this.mWriteEpoch.Cipher.GetPlaintextLimit(this.mTransport.GetSendLimit() - 13));
		}

		public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
		{
			byte[] array = null;
			int result;
			while (true)
			{
				int num = Math.Min(len, this.GetReceiveLimit()) + 13;
				if (array == null || array.Length < num)
				{
					array = new byte[num];
				}
				try
				{
					if (this.mRetransmit != null && DateTimeUtilities.CurrentUnixMs() > this.mRetransmitExpiry)
					{
						this.mRetransmit = null;
						this.mRetransmitEpoch = null;
					}
					int num2 = this.ReceiveRecord(array, 0, num, waitMillis);
					if (num2 < 0)
					{
						result = num2;
					}
					else
					{
						if (num2 < 13)
						{
							continue;
						}
						int num3 = TlsUtilities.ReadUint16(array, 11);
						if (num2 != num3 + 13)
						{
							continue;
						}
						byte b = TlsUtilities.ReadUint8(array, 0);
						switch (b)
						{
						case 20:
						case 21:
						case 22:
						case 23:
						case 24:
						{
							int num4 = TlsUtilities.ReadUint16(array, 3);
							DtlsEpoch dtlsEpoch = null;
							if (num4 == this.mReadEpoch.Epoch)
							{
								dtlsEpoch = this.mReadEpoch;
							}
							else if (b == 22 && this.mRetransmitEpoch != null && num4 == this.mRetransmitEpoch.Epoch)
							{
								dtlsEpoch = this.mRetransmitEpoch;
							}
							if (dtlsEpoch == null)
							{
								continue;
							}
							long num5 = TlsUtilities.ReadUint48(array, 5);
							if (dtlsEpoch.ReplayWindow.ShouldDiscard(num5))
							{
								continue;
							}
							ProtocolVersion other = TlsUtilities.ReadVersion(array, 1);
							if (this.mDiscoveredPeerVersion != null && !this.mDiscoveredPeerVersion.Equals(other))
							{
								continue;
							}
							byte[] array2 = dtlsEpoch.Cipher.DecodeCiphertext(DtlsRecordLayer.GetMacSequenceNumber(dtlsEpoch.Epoch, num5), b, array, 13, num2 - 13);
							dtlsEpoch.ReplayWindow.ReportAuthenticated(num5);
							if (array2.Length > this.mPlaintextLimit)
							{
								continue;
							}
							if (this.mDiscoveredPeerVersion == null)
							{
								this.mDiscoveredPeerVersion = other;
							}
							switch (b)
							{
							case 20:
								for (int i = 0; i < array2.Length; i++)
								{
									byte b2 = TlsUtilities.ReadUint8(array2, i);
									if (b2 == 1 && this.mPendingEpoch != null)
									{
										this.mReadEpoch = this.mPendingEpoch;
									}
								}
								continue;
							case 21:
								if (array2.Length == 2)
								{
									byte b3 = array2[0];
									byte b4 = array2[1];
									this.mPeer.NotifyAlertReceived(b3, b4);
									if (b3 == 2)
									{
										this.Fail(b4);
										throw new TlsFatalAlert(b4);
									}
									if (b4 == 0)
									{
										this.CloseTransport();
									}
								}
								continue;
							case 22:
								if (!this.mInHandshake)
								{
									if (this.mRetransmit != null)
									{
										this.mRetransmit.ReceivedHandshakeRecord(num4, array2, 0, array2.Length);
									}
									continue;
								}
								break;
							case 23:
								if (this.mInHandshake)
								{
									continue;
								}
								break;
							case 24:
								continue;
							}
							if (!this.mInHandshake && this.mRetransmit != null)
							{
								this.mRetransmit = null;
								this.mRetransmitEpoch = null;
							}
							Array.Copy(array2, 0, buf, off, array2.Length);
							result = array2.Length;
							break;
						}
						default:
							continue;
						}
					}
				}
				catch (IOException ex)
				{
					throw ex;
				}
				break;
			}
			return result;
		}

		public virtual void Send(byte[] buf, int off, int len)
		{
			byte contentType = 23;
			if (this.mInHandshake || this.mWriteEpoch == this.mRetransmitEpoch)
			{
				contentType = 22;
				byte b = TlsUtilities.ReadUint8(buf, off);
				if (b == 20)
				{
					DtlsEpoch dtlsEpoch = null;
					if (this.mInHandshake)
					{
						dtlsEpoch = this.mPendingEpoch;
					}
					else if (this.mWriteEpoch == this.mRetransmitEpoch)
					{
						dtlsEpoch = this.mCurrentEpoch;
					}
					if (dtlsEpoch == null)
					{
						throw new InvalidOperationException();
					}
					byte[] array = new byte[]
					{
						1
					};
					this.SendRecord(20, array, 0, array.Length);
					this.mWriteEpoch = dtlsEpoch;
				}
			}
			this.SendRecord(contentType, buf, off, len);
		}

		public virtual void Close()
		{
			if (!this.mClosed)
			{
				if (this.mInHandshake)
				{
					this.Warn(90, "User canceled handshake");
				}
				this.CloseTransport();
			}
		}

		internal virtual void Fail(byte alertDescription)
		{
			if (!this.mClosed)
			{
				try
				{
					this.RaiseAlert(2, alertDescription, null, null);
				}
				catch (Exception)
				{
				}
				this.mFailed = true;
				this.CloseTransport();
			}
		}

		internal virtual void Warn(byte alertDescription, string message)
		{
			this.RaiseAlert(1, alertDescription, message, null);
		}

		private void CloseTransport()
		{
			if (!this.mClosed)
			{
				try
				{
					if (!this.mFailed)
					{
						this.Warn(0, null);
					}
					this.mTransport.Close();
				}
				catch (Exception)
				{
				}
				this.mClosed = true;
			}
		}

		private void RaiseAlert(byte alertLevel, byte alertDescription, string message, Exception cause)
		{
			this.mPeer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
			this.SendRecord(21, new byte[]
			{
				alertLevel,
				alertDescription
			}, 0, 2);
		}

		private int ReceiveRecord(byte[] buf, int off, int len, int waitMillis)
		{
			if (this.mRecordQueue.Available > 0)
			{
				int num = 0;
				if (this.mRecordQueue.Available >= 13)
				{
					byte[] buf2 = new byte[2];
					this.mRecordQueue.Read(buf2, 0, 2, 11);
					num = TlsUtilities.ReadUint16(buf2, 0);
				}
				int num2 = Math.Min(this.mRecordQueue.Available, 13 + num);
				this.mRecordQueue.RemoveData(buf, off, num2, 0);
				return num2;
			}
			int num3 = this.mTransport.Receive(buf, off, len, waitMillis);
			if (num3 >= 13)
			{
				int num4 = TlsUtilities.ReadUint16(buf, off + 11);
				int num5 = 13 + num4;
				if (num3 > num5)
				{
					this.mRecordQueue.AddData(buf, off + num5, num3 - num5);
					num3 = num5;
				}
			}
			return num3;
		}

		private void SendRecord(byte contentType, byte[] buf, int off, int len)
		{
			if (len > this.mPlaintextLimit)
			{
				throw new TlsFatalAlert(80);
			}
			if (len < 1 && contentType != 23)
			{
				throw new TlsFatalAlert(80);
			}
			int epoch = this.mWriteEpoch.Epoch;
			long num = this.mWriteEpoch.AllocateSequenceNumber();
			byte[] array = this.mWriteEpoch.Cipher.EncodePlaintext(DtlsRecordLayer.GetMacSequenceNumber(epoch, num), contentType, buf, off, len);
			byte[] array2 = new byte[array.Length + 13];
			TlsUtilities.WriteUint8(contentType, array2, 0);
			ProtocolVersion version = (this.mDiscoveredPeerVersion != null) ? this.mDiscoveredPeerVersion : this.mContext.ClientVersion;
			TlsUtilities.WriteVersion(version, array2, 1);
			TlsUtilities.WriteUint16(epoch, array2, 3);
			TlsUtilities.WriteUint48(num, array2, 5);
			TlsUtilities.WriteUint16(array.Length, array2, 11);
			Array.Copy(array, 0, array2, 13, array.Length);
			this.mTransport.Send(array2, 0, array2.Length);
		}

		private static long GetMacSequenceNumber(int epoch, long sequence_number)
		{
			return ((long)epoch & (long)((ulong)-1)) << 48 | sequence_number;
		}
	}
}
