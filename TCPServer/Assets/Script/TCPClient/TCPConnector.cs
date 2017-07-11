using System;
using System.Net.Sockets;
using System.Net;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// TCPConnector
    /// TCP를 연결 시켜주는 컴포넌트 클래스
    /// 기존 세션을 넘겨서 재연결을 처리 할 수 있음.
    /// </summary>
    public class TCPConnector
    {
        public delegate void ConnectHandler(TCPSession tcpSession);
        public delegate void FailHandler();

        private Socket mSocket;
        private IPEndPoint mEndPoint;
        private ConnectHandler mConnectedCallback;
        private FailHandler mConnectedFailCallback;

        public ConnectHandler connectHandler
        {
            set { mConnectedCallback = value; }
        }

        public FailHandler failHandler
        {
            set { mConnectedFailCallback = value; }
        }

        /// <summary>
        /// 서버 연결
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="tcpSession">The TCP session.</param>
        /// <returns></returns>
        public bool Connect(String host, int port, IProtocolResolver protocol_resolver, TCPSession tcpSession = null)
        {
            this.mEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            this.mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (tcpSession == null)
            {
                tcpSession = new TCPSession();
            }

            tcpSession.SetProtocolResolver(protocol_resolver);

            SocketAsyncEventArgs event_arg = new SocketAsyncEventArgs();
            event_arg.Completed += OnConnect;
            event_arg.RemoteEndPoint = mEndPoint;
            event_arg.UserToken = tcpSession;
            bool pending = mSocket.ConnectAsync(event_arg);
            if (false == pending)
            {
                OnConnect(null, event_arg);
            }

            return true;
        }

        /// <summary>
        /// Called when [connect].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SocketAsyncEventArgs"/> instance containing the event data.</param>
        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                TCPSession tcpSession = e.UserToken as TCPSession;
                tcpSession.OnConnectCompleted(mSocket);

                if (this.mConnectedCallback != null)
                {
                    this.mConnectedCallback(tcpSession);
                }

                tcpSession.OnConnect();
                tcpSession.ReceiveRequest();
            }
            else
            {
                // 연결 끊김
                if (this.mConnectedFailCallback != null)
                {
                    this.mConnectedFailCallback();
                }

            }
        }
    }
}

