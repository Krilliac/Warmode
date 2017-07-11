using System;
using System.Collections.Generic;

namespace UnityEngine.Networking
{
	internal class ChannelBuffer : IDisposable
	{
		private const int k_MaxFreePacketCount = 512;

		private const int k_MaxPendingPacketCount = 16;

		private const int k_PacketHeaderReserveSize = 100;

		private NetworkConnection m_Connection;

		private ChannelPacket m_CurrentPacket;

		private float m_LastFlushTime;

		private byte m_ChannelId;

		private int m_MaxPacketSize;

		private bool m_IsReliable;

		private bool m_IsBroken;

		private int m_MaxPendingPacketCount;

		private List<ChannelPacket> m_PendingPackets;

		private static List<ChannelPacket> s_FreePackets;

		internal static int pendingPacketCount;

		public float maxDelay = 0.01f;

		private float m_LastBufferedMessageCountTimer = Time.time;

		private static NetworkWriter s_SendWriter = new NetworkWriter();

		private bool m_Disposed;

		public int numMsgsOut
		{
			get;
			private set;
		}

		public int numBufferedMsgsOut
		{
			get;
			private set;
		}

		public int numBytesOut
		{
			get;
			private set;
		}

		public int numMsgsIn
		{
			get;
			private set;
		}

		public int numBytesIn
		{
			get;
			private set;
		}

		public int numBufferedPerSecond
		{
			get;
			private set;
		}

		public int lastBufferedPerSecond
		{
			get;
			private set;
		}

		public ChannelBuffer(NetworkConnection conn, int bufferSize, byte cid, bool isReliable)
		{
			this.m_Connection = conn;
			this.m_MaxPacketSize = bufferSize - 100;
			this.m_CurrentPacket = new ChannelPacket(this.m_MaxPacketSize, isReliable);
			this.m_ChannelId = cid;
			this.m_MaxPendingPacketCount = 16;
			this.m_IsReliable = isReliable;
			if (isReliable)
			{
				this.m_PendingPackets = new List<ChannelPacket>();
				if (ChannelBuffer.s_FreePackets == null)
				{
					ChannelBuffer.s_FreePackets = new List<ChannelPacket>();
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_Disposed && disposing && this.m_PendingPackets != null)
			{
				foreach (ChannelPacket current in this.m_PendingPackets)
				{
					ChannelBuffer.pendingPacketCount--;
					if (ChannelBuffer.s_FreePackets.Count < 512)
					{
						ChannelBuffer.s_FreePackets.Add(current);
					}
				}
				this.m_PendingPackets.Clear();
			}
			this.m_Disposed = true;
		}

		public bool SetOption(ChannelOption option, int value)
		{
			if (option != ChannelOption.MaxPendingBuffers)
			{
				return false;
			}
			if (!this.m_IsReliable)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("Cannot set MaxPendingBuffers on unreliable channel " + this.m_ChannelId);
				}
				return false;
			}
			if (value < 0 || value >= 512)
			{
				if (LogFilter.logError)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Invalid MaxPendingBuffers for channel ",
						this.m_ChannelId,
						". Must be greater than zero and less than ",
						512
					}));
				}
				return false;
			}
			this.m_MaxPendingPacketCount = value;
			return true;
		}

		public void CheckInternalBuffer()
		{
			if (Time.time - this.m_LastFlushTime > this.maxDelay && !this.m_CurrentPacket.IsEmpty())
			{
				this.SendInternalBuffer();
				this.m_LastFlushTime = Time.time;
			}
			if (Time.time - this.m_LastBufferedMessageCountTimer > 1f)
			{
				this.lastBufferedPerSecond = this.numBufferedPerSecond;
				this.numBufferedPerSecond = 0;
				this.m_LastBufferedMessageCountTimer = Time.time;
			}
		}

		public bool SendWriter(NetworkWriter writer)
		{
			return this.SendBytes(writer.AsArraySegment().Array, writer.AsArraySegment().Count);
		}

		public bool Send(short msgType, MessageBase msg)
		{
			ChannelBuffer.s_SendWriter.StartMessage(msgType);
			msg.Serialize(ChannelBuffer.s_SendWriter);
			ChannelBuffer.s_SendWriter.FinishMessage();
			this.numMsgsOut++;
			return this.SendWriter(ChannelBuffer.s_SendWriter);
		}

		internal bool SendBytes(byte[] bytes, int bytesToSend)
		{
			if (bytesToSend <= 0)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("ChannelBuffer:SendBytes cannot send zero bytes");
				}
				return false;
			}
			if (bytesToSend > this.m_MaxPacketSize)
			{
				if (LogFilter.logError)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Failed to send big message of ",
						bytesToSend,
						" bytes. The maximum is ",
						this.m_MaxPacketSize,
						" bytes on this channel."
					}));
				}
				return false;
			}
			if (this.m_CurrentPacket.HasSpace(bytesToSend))
			{
				this.m_CurrentPacket.Write(bytes, bytesToSend);
				return this.maxDelay != 0f || this.SendInternalBuffer();
			}
			if (this.m_IsReliable)
			{
				if (this.m_PendingPackets.Count == 0)
				{
					if (!this.m_CurrentPacket.SendToTransport(this.m_Connection, (int)this.m_ChannelId))
					{
						this.QueuePacket();
					}
					this.m_CurrentPacket.Write(bytes, bytesToSend);
					return true;
				}
				if (this.m_PendingPackets.Count >= this.m_MaxPendingPacketCount)
				{
					if (!this.m_IsBroken && LogFilter.logError)
					{
						Debug.LogError("ChannelBuffer buffer limit of " + this.m_PendingPackets.Count + " packets reached.");
					}
					this.m_IsBroken = true;
					return false;
				}
				this.QueuePacket();
				this.m_CurrentPacket.Write(bytes, bytesToSend);
				return true;
			}
			else
			{
				if (!this.m_CurrentPacket.SendToTransport(this.m_Connection, (int)this.m_ChannelId))
				{
					if (LogFilter.logError)
					{
						Debug.Log("ChannelBuffer SendBytes no space on unreliable channel " + this.m_ChannelId);
					}
					return false;
				}
				this.m_CurrentPacket.Write(bytes, bytesToSend);
				return true;
			}
		}

		private void QueuePacket()
		{
			ChannelBuffer.pendingPacketCount++;
			this.m_PendingPackets.Add(this.m_CurrentPacket);
			this.m_CurrentPacket = this.AllocPacket();
		}

		private ChannelPacket AllocPacket()
		{
			if (ChannelBuffer.s_FreePackets.Count == 0)
			{
				return new ChannelPacket(this.m_MaxPacketSize, this.m_IsReliable);
			}
			ChannelPacket result = ChannelBuffer.s_FreePackets[0];
			ChannelBuffer.s_FreePackets.RemoveAt(0);
			result.Reset();
			return result;
		}

		private static void FreePacket(ChannelPacket packet)
		{
			if (ChannelBuffer.s_FreePackets.Count >= 512)
			{
				return;
			}
			ChannelBuffer.s_FreePackets.Add(packet);
		}

		public bool SendInternalBuffer()
		{
			if (this.m_IsReliable && this.m_PendingPackets.Count > 0)
			{
				while (this.m_PendingPackets.Count > 0)
				{
					ChannelPacket packet = this.m_PendingPackets[0];
					if (!packet.SendToTransport(this.m_Connection, (int)this.m_ChannelId))
					{
						break;
					}
					ChannelBuffer.pendingPacketCount--;
					this.m_PendingPackets.RemoveAt(0);
					ChannelBuffer.FreePacket(packet);
					if (this.m_IsBroken && this.m_PendingPackets.Count < this.m_MaxPendingPacketCount / 2)
					{
						if (LogFilter.logWarn)
						{
							Debug.LogWarning("ChannelBuffer recovered from overflow but data was lost.");
						}
						this.m_IsBroken = false;
					}
				}
				return true;
			}
			return this.m_CurrentPacket.SendToTransport(this.m_Connection, (int)this.m_ChannelId);
		}
	}
}
