using System;
using System.Net.Sockets;


namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// TCPSession
    /// 
    /// </summary>
    public partial class TCPSession
    {
        /// <summary>
        /// TCP 연결 완료 콜백
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="tcpSession">The TCP session.</param>
        public void OnConnectCompleted(Socket socket)
        {
            // 재접속시에는 다시 만들지 않아도 된다.
            if (sessionState == TCPSession.SESSION_STATE.NONE)
            {
                // Receive Socket Async Event Regist
                mReceiveEventArgs = new SocketAsyncEventArgs();
                mReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                mReceiveEventArgs.UserToken = this;
                mReceiveEventArgs.SetBuffer(new byte[TCPCommon.RECV_BUFFER_SIZE], 0, TCPCommon.RECV_BUFFER_SIZE);

                // Send Socket Async Event Regist
                mSendEventArgs = new SocketAsyncEventArgs();
                mSendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                mSendEventArgs.UserToken = this;
                mSendEventArgs.SetBuffer(new byte[TCPCommon.SEND_BUFFER_SIZE], 0, TCPCommon.SEND_BUFFER_SIZE);
            }

            mSocket = socket;
        }

        /// <summary>
        /// Called when [receive completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SocketAsyncEventArgs"/> instance containing the event data.</param>
        public void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            TCPSession tcpSession = e.UserToken as TCPSession;
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                tcpSession.OnReceive();
                return;
            }
            else
            {
                tcpSession.OnDisconnect();
            }
        }

        /// <summary>
        /// Called when [send completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SocketAsyncEventArgs"/> instance containing the event data.</param>
        public void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            TCPSession tcpSession = e.UserToken as TCPSession;
            tcpSession.OnSend();
        }
    }


}
