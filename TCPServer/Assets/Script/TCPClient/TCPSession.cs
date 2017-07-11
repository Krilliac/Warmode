using System;
using System.Net.Sockets;


namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// TCPSession
    /// 
    /// 1-Send, 1-Recv
    /// Send의 경우, 1-Send를 위해서 Send 요청시 SendBuffer에 먼저 채워 넣는다.
    /// SendBuffer가 비어있다면 Send Flush, SendBuffer에 데이터가 있다면 Send 완료 함수에서 Send를 Flush해준다.
    /// SendBuffer는 MainThread와 IO ThreadPool의 Thread에서 접근하므로 Thread-Safe 해야한다.
    /// 
    /// Recv는 Recv 발생시 RecvBuffer에 들어간 후, IPacketResolver에서 패킷이 전부 다 왔는지 제대로 왔는지 체크한다.
    /// 그 후, RecvBuffer에서 삭제.
    /// RecvBuffer는 IO ThreadPool의 Thread에서만 접근하므로 Thread-Safe할 필요는 없다.
    /// 
    /// TCPSession은 IClientSession과 연결된다.
    /// IClientSession은 Application Level에서 구현된다.
    /// 
    /// </summary>
    public partial class TCPSession
    {
        public enum SESSION_STATE
        {
            NONE,
            CONNECTED,
            DISCONNECTED,
        }

        private Socket mSocket;
        private SocketAsyncEventArgs mReceiveEventArgs;
        private SocketAsyncEventArgs mSendEventArgs;

        private CircularBuffer mReceiveBuffer = new CircularBuffer(TCPCommon.SEND_BUFFERQUEUE_POWER);
        private CircularBuffer mSendBuffer = new CircularBuffer(TCPCommon.RECV_BUFFERQUEUE_POWER);
        private byte[] mReadQueueBuffer = new byte[TCPCommon.RECV_BUFFER_SIZE];

        private IProtocolResolver mProtocolResolver;
        private WeakReference mClientSession;

        private SESSION_STATE mSessionState;
        public SESSION_STATE sessionState
        {
            get { return mSessionState; }
        }

        private object mSendingBufferLock;

        public void SetClientSession(IClientSession client_session)
        {
            this.mClientSession = new WeakReference(client_session);
        }

        public void SetProtocolResolver(IProtocolResolver protocolResolver)
        {
            this.mProtocolResolver = protocolResolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TCPSession"/> class.
        /// </summary>
        public TCPSession()
        {
            this.mSendingBufferLock = new object();
            mSessionState = SESSION_STATE.NONE;
        }

        /// <summary>
        /// Receive 요청(PreReceive)
        /// </summary>
        public void ReceiveRequest()
        {
            if (mSessionState != SESSION_STATE.CONNECTED)
                return;

            bool pending = mSocket.ReceiveAsync(mReceiveEventArgs);
            if (false == pending)
            {
                OnReceive();
            }
        }

        /// <summary>
        /// Send 버퍼에 데이터 Write
        /// 1-Send로 처리하기 때문에 우선 패킷은 버퍼에 쌓아놓음.
        /// 버퍼에 데이터가 있다면 Send완료후, OnSend에서 SendFlush 호출.
        /// 버퍼에 데이터가 없다면 바로 SendFlush 호출.
        /// 
        /// Thread Safe
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="size">The size.</param>
        public void SendRequest(PacketStream packet)
        {
            if (mSessionState != SESSION_STATE.CONNECTED)
                return;

            if (packet == null)
                return;

            if (packet.Buffer == null || packet.Size <= 0)
                return;

            lock (mSendingBufferLock)
            {
                int stored_size = mSendBuffer.GetStoredSize();
                if (false == mSendBuffer.Write(packet.Buffer, 0, packet.Size))
                {
                    // 연결 끊김
                }

                // 안에 데이터가 없으면 바로 전송
                if (stored_size <= 0)
                {
                    SendFlush();
                }


            }
        }

        /// <summary>
        /// 버퍼에 쌓인 패킷들을 Send 처리
        /// 
        /// Thread-Safe
        /// </summary>
        private void SendFlush()
        {
            if (mSessionState != SESSION_STATE.CONNECTED)
                return;

            int storedSize = 0;
            lock (mSendingBufferLock)
            {
                storedSize = mSendBuffer.GetStoredSize();

                // 재전송을 위해서 지우지않고 Peek
                mSendBuffer.Peek(mSendEventArgs.Buffer, storedSize);
            }

            mSendEventArgs.SetBuffer(mSendEventArgs.Offset, storedSize);
            bool pending = mSocket.SendAsync(mSendEventArgs);
            if (false == pending)
            {
                OnSend();
            }
        }

        /// <summary>
        ///  연결 완료
        /// </summary>
        public void OnConnect()
        {
            mSessionState = SESSION_STATE.CONNECTED;

            if (mClientSession.IsAlive)
            {
                (mClientSession.Target as IClientSession).OnConnected();
            }
        }


        /// <summary>
        /// Receive 완료 콜백
        /// </summary>
        /// <exception cref="Exception">Close Socket</exception>
        public void OnReceive()
        {
            if (mReceiveEventArgs.BytesTransferred > 0 && mReceiveEventArgs.SocketError == SocketError.Success)
            {
                OnProcessReceive(mReceiveEventArgs.Buffer, mReceiveEventArgs.Offset, mReceiveEventArgs.BytesTransferred);

                ReceiveRequest();
            }
            else
            {
                // 연결 끊김
                OnDisconnect();
            }
        }

        /// <summary>
        /// Send 완료 콜백
        /// 
        /// Thread-Safe
        /// </summary>
        public void OnSend()
        {
            if (mSendEventArgs.BytesTransferred <= 0 || mSendEventArgs.SocketError != SocketError.Success)
            {
                return;
            }

            int remainBufferSize = 0;
            lock (mSendingBufferLock)
            {
                mSendBuffer.Remove(mSendEventArgs.BytesTransferred);
                remainBufferSize = mSendBuffer.GetStoredSize();
            }

            // 아직 버퍼에 쌓인 데이터가 있다면 재전송
            if (remainBufferSize > 0)
            {
                SendFlush();
            }

        }

        /// <summary>
        /// 종료 콜백
        /// </summary>
        public void OnDisconnect()
        {
            mSessionState = SESSION_STATE.DISCONNECTED;
            Reset();

            if (mClientSession.IsAlive)
            {
                (mClientSession.Target as IClientSession).OnDisconnected();
            }
        }

        /// <summary>
        /// Called when [receive complete].
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="transfer">The transfer.</param>
        private void OnProcessReceive(byte[] buffer, int offset, int bytes)
        {
            // 버퍼 오버플로우
            if (false == mReceiveBuffer.Write(buffer, offset, bytes))
            {
                // 연결 끊김
                Disconnect();
            }

            int storedSize = mReceiveBuffer.GetStoredSize();
            int offsetIndex = 0;

            if (false == mReceiveBuffer.Peek(mReadQueueBuffer, storedSize))
            {

            }

            PacketStream packet = null;
            int packetSize = 0;
            while ((packet = mProtocolResolver.PacketProtocolResolve(new ArraySegment<byte>(mReadQueueBuffer, offsetIndex, storedSize), out packetSize)) != null)
            {
                if (false == mClientSession.IsAlive)
                {
                    break;
                }
                (mClientSession.Target as IClientSession).OnRead(packet);

                offsetIndex += packetSize;
                storedSize -= packetSize;

                // 버퍼에서 삭제
                mReceiveBuffer.Remove(packetSize);

                if (storedSize <= 0)
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            mSendBuffer.Clear();
            mReceiveBuffer.Clear();
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            mSessionState = SESSION_STATE.DISCONNECTED;
            try
            {
                mSocket.Shutdown(SocketShutdown.Send);
            }
            catch (Exception) { }
            mSocket.Close();
            mSocket = null;
        }
    }

}

