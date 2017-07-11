using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsReliableHandshake
	{
		internal class Message
		{
			private readonly int mMessageSeq;

			private readonly byte mMsgType;

			private readonly byte[] mBody;

			public int Seq
			{
				get
				{
					return this.mMessageSeq;
				}
			}

			public byte Type
			{
				get
				{
					return this.mMsgType;
				}
			}

			public byte[] Body
			{
				get
				{
					return this.mBody;
				}
			}

			internal Message(int message_seq, byte msg_type, byte[] body)
			{
				this.mMessageSeq = message_seq;
				this.mMsgType = msg_type;
				this.mBody = body;
			}
		}

		internal class RecordLayerBuffer : MemoryStream
		{
			internal RecordLayerBuffer(int size) : base(size)
			{
			}

			internal void SendToRecordLayer(DtlsRecordLayer recordLayer)
			{
				recordLayer.Send(this.GetBuffer(), 0, (int)this.Length);
				this.Close();
			}
		}

		internal class Retransmit : DtlsHandshakeRetransmit
		{
			private readonly DtlsReliableHandshake mOuter;

			internal Retransmit(DtlsReliableHandshake outer)
			{
				this.mOuter = outer;
			}

			public void ReceivedHandshakeRecord(int epoch, byte[] buf, int off, int len)
			{
				this.mOuter.HandleRetransmittedHandshakeRecord(epoch, buf, off, len);
			}
		}

		private const int MAX_RECEIVE_AHEAD = 10;

		private readonly DtlsRecordLayer mRecordLayer;

		private TlsHandshakeHash mHandshakeHash;

		private IDictionary mCurrentInboundFlight = Platform.CreateHashtable();

		private IDictionary mPreviousInboundFlight;

		private IList mOutboundFlight = Platform.CreateArrayList();

		private bool mSending = true;

		private int mMessageSeq;

		private int mNextReceiveSeq;

		internal TlsHandshakeHash HandshakeHash
		{
			get
			{
				return this.mHandshakeHash;
			}
		}

		internal DtlsReliableHandshake(TlsContext context, DtlsRecordLayer transport)
		{
			this.mRecordLayer = transport;
			this.mHandshakeHash = new DeferredHash();
			this.mHandshakeHash.Init(context);
		}

		internal void NotifyHelloComplete()
		{
			this.mHandshakeHash = this.mHandshakeHash.NotifyPrfDetermined();
		}

		internal TlsHandshakeHash PrepareToFinish()
		{
			TlsHandshakeHash result = this.mHandshakeHash;
			this.mHandshakeHash = this.mHandshakeHash.StopTracking();
			return result;
		}

		internal void SendMessage(byte msg_type, byte[] body)
		{
			TlsUtilities.CheckUint24(body.Length);
			if (!this.mSending)
			{
				this.CheckInboundFlight();
				this.mSending = true;
				this.mOutboundFlight.Clear();
			}
			DtlsReliableHandshake.Message message = new DtlsReliableHandshake.Message(this.mMessageSeq++, msg_type, body);
			this.mOutboundFlight.Add(message);
			this.WriteMessage(message);
			this.UpdateHandshakeMessagesDigest(message);
		}

		internal byte[] ReceiveMessageBody(byte msg_type)
		{
			DtlsReliableHandshake.Message message = this.ReceiveMessage();
			if (message.Type != msg_type)
			{
				throw new TlsFatalAlert(10);
			}
			return message.Body;
		}

		internal DtlsReliableHandshake.Message ReceiveMessage()
		{
			if (this.mSending)
			{
				this.mSending = false;
				this.PrepareInboundFlight();
			}
			DtlsReassembler dtlsReassembler = (DtlsReassembler)this.mCurrentInboundFlight[this.mNextReceiveSeq];
			if (dtlsReassembler != null)
			{
				byte[] bodyIfComplete = dtlsReassembler.GetBodyIfComplete();
				if (bodyIfComplete != null)
				{
					this.mPreviousInboundFlight = null;
					return this.UpdateHandshakeMessagesDigest(new DtlsReliableHandshake.Message(this.mNextReceiveSeq++, dtlsReassembler.MsgType, bodyIfComplete));
				}
			}
			byte[] array = null;
			int num = 1000;
			DtlsReliableHandshake.Message result;
			while (true)
			{
				int receiveLimit = this.mRecordLayer.GetReceiveLimit();
				if (array != null && array.Length >= receiveLimit)
				{
					goto IL_92;
				}
				array = new byte[receiveLimit];
				try
				{
					DtlsReassembler dtlsReassembler3;
					byte[] bodyIfComplete2;
					while (true)
					{
						IL_92:
						int num2 = this.mRecordLayer.Receive(array, 0, receiveLimit, num);
						if (num2 < 0)
						{
							goto IL_214;
						}
						if (num2 >= 12)
						{
							int num3 = TlsUtilities.ReadUint24(array, 9);
							if (num2 == num3 + 12)
							{
								int num4 = TlsUtilities.ReadUint16(array, 4);
								if (num4 <= this.mNextReceiveSeq + 10)
								{
									byte msg_type = TlsUtilities.ReadUint8(array, 0);
									int num5 = TlsUtilities.ReadUint24(array, 1);
									int num6 = TlsUtilities.ReadUint24(array, 6);
									if (num6 + num3 <= num5)
									{
										if (num4 < this.mNextReceiveSeq)
										{
											if (this.mPreviousInboundFlight != null)
											{
												DtlsReassembler dtlsReassembler2 = (DtlsReassembler)this.mPreviousInboundFlight[num4];
												if (dtlsReassembler2 != null)
												{
													dtlsReassembler2.ContributeFragment(msg_type, num5, array, 12, num6, num3);
													if (DtlsReliableHandshake.CheckAll(this.mPreviousInboundFlight))
													{
														this.ResendOutboundFlight();
														num = Math.Min(num * 2, 60000);
														DtlsReliableHandshake.ResetAll(this.mPreviousInboundFlight);
													}
												}
											}
										}
										else
										{
											dtlsReassembler3 = (DtlsReassembler)this.mCurrentInboundFlight[num4];
											if (dtlsReassembler3 == null)
											{
												dtlsReassembler3 = new DtlsReassembler(msg_type, num5);
												this.mCurrentInboundFlight[num4] = dtlsReassembler3;
											}
											dtlsReassembler3.ContributeFragment(msg_type, num5, array, 12, num6, num3);
											if (num4 == this.mNextReceiveSeq)
											{
												bodyIfComplete2 = dtlsReassembler3.GetBodyIfComplete();
												if (bodyIfComplete2 != null)
												{
													break;
												}
											}
										}
									}
								}
							}
						}
					}
					this.mPreviousInboundFlight = null;
					result = this.UpdateHandshakeMessagesDigest(new DtlsReliableHandshake.Message(this.mNextReceiveSeq++, dtlsReassembler3.MsgType, bodyIfComplete2));
					break;
					IL_214:;
				}
				catch (IOException)
				{
				}
				this.ResendOutboundFlight();
				num = Math.Min(num * 2, 60000);
			}
			return result;
		}

		internal void Finish()
		{
			DtlsHandshakeRetransmit retransmit = null;
			if (!this.mSending)
			{
				this.CheckInboundFlight();
			}
			else if (this.mCurrentInboundFlight != null)
			{
				retransmit = new DtlsReliableHandshake.Retransmit(this);
			}
			this.mRecordLayer.HandshakeSuccessful(retransmit);
		}

		internal void ResetHandshakeMessagesDigest()
		{
			this.mHandshakeHash.Reset();
		}

		private void HandleRetransmittedHandshakeRecord(int epoch, byte[] buf, int off, int len)
		{
			if (len < 12)
			{
				return;
			}
			int num = TlsUtilities.ReadUint24(buf, off + 9);
			if (len != num + 12)
			{
				return;
			}
			int num2 = TlsUtilities.ReadUint16(buf, off + 4);
			if (num2 >= this.mNextReceiveSeq)
			{
				return;
			}
			byte b = TlsUtilities.ReadUint8(buf, off);
			int num3 = (b == 20) ? 1 : 0;
			if (epoch != num3)
			{
				return;
			}
			int num4 = TlsUtilities.ReadUint24(buf, off + 1);
			int num5 = TlsUtilities.ReadUint24(buf, off + 6);
			if (num5 + num > num4)
			{
				return;
			}
			DtlsReassembler dtlsReassembler = (DtlsReassembler)this.mCurrentInboundFlight[num2];
			if (dtlsReassembler != null)
			{
				dtlsReassembler.ContributeFragment(b, num4, buf, off + 12, num5, num);
				if (DtlsReliableHandshake.CheckAll(this.mCurrentInboundFlight))
				{
					this.ResendOutboundFlight();
					DtlsReliableHandshake.ResetAll(this.mCurrentInboundFlight);
				}
			}
		}

		private void CheckInboundFlight()
		{
			foreach (int num in this.mCurrentInboundFlight.Keys)
			{
				int arg_26_0 = this.mNextReceiveSeq;
			}
		}

		private void PrepareInboundFlight()
		{
			DtlsReliableHandshake.ResetAll(this.mCurrentInboundFlight);
			this.mPreviousInboundFlight = this.mCurrentInboundFlight;
			this.mCurrentInboundFlight = Platform.CreateHashtable();
		}

		private void ResendOutboundFlight()
		{
			this.mRecordLayer.ResetWriteEpoch();
			for (int i = 0; i < this.mOutboundFlight.Count; i++)
			{
				this.WriteMessage((DtlsReliableHandshake.Message)this.mOutboundFlight[i]);
			}
		}

		private DtlsReliableHandshake.Message UpdateHandshakeMessagesDigest(DtlsReliableHandshake.Message message)
		{
			if (message.Type != 0)
			{
				byte[] body = message.Body;
				byte[] array = new byte[12];
				TlsUtilities.WriteUint8(message.Type, array, 0);
				TlsUtilities.WriteUint24(body.Length, array, 1);
				TlsUtilities.WriteUint16(message.Seq, array, 4);
				TlsUtilities.WriteUint24(0, array, 6);
				TlsUtilities.WriteUint24(body.Length, array, 9);
				this.mHandshakeHash.BlockUpdate(array, 0, array.Length);
				this.mHandshakeHash.BlockUpdate(body, 0, body.Length);
			}
			return message;
		}

		private void WriteMessage(DtlsReliableHandshake.Message message)
		{
			int sendLimit = this.mRecordLayer.GetSendLimit();
			int num = sendLimit - 12;
			if (num < 1)
			{
				throw new TlsFatalAlert(80);
			}
			int num2 = message.Body.Length;
			int num3 = 0;
			do
			{
				int num4 = Math.Min(num2 - num3, num);
				this.WriteHandshakeFragment(message, num3, num4);
				num3 += num4;
			}
			while (num3 < num2);
		}

		private void WriteHandshakeFragment(DtlsReliableHandshake.Message message, int fragment_offset, int fragment_length)
		{
			DtlsReliableHandshake.RecordLayerBuffer recordLayerBuffer = new DtlsReliableHandshake.RecordLayerBuffer(12 + fragment_length);
			TlsUtilities.WriteUint8(message.Type, recordLayerBuffer);
			TlsUtilities.WriteUint24(message.Body.Length, recordLayerBuffer);
			TlsUtilities.WriteUint16(message.Seq, recordLayerBuffer);
			TlsUtilities.WriteUint24(fragment_offset, recordLayerBuffer);
			TlsUtilities.WriteUint24(fragment_length, recordLayerBuffer);
			recordLayerBuffer.Write(message.Body, fragment_offset, fragment_length);
			recordLayerBuffer.SendToRecordLayer(this.mRecordLayer);
		}

		private static bool CheckAll(IDictionary inboundFlight)
		{
			foreach (DtlsReassembler dtlsReassembler in inboundFlight.Values)
			{
				if (dtlsReassembler.GetBodyIfComplete() == null)
				{
					return false;
				}
			}
			return true;
		}

		private static void ResetAll(IDictionary inboundFlight)
		{
			foreach (DtlsReassembler dtlsReassembler in inboundFlight.Values)
			{
				dtlsReassembler.Reset();
			}
		}
	}
}
