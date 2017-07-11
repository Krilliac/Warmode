using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketEx
{
	public class TcpClient : IDisposable
	{
		private enum Properties : uint
		{
			LingerState = 1u,
			NoDelay,
			ReceiveBufferSize = 4u,
			ReceiveTimeout = 8u,
			SendBufferSize = 16u,
			SendTimeout = 32u
		}

		private NetworkStream stream;

		private bool active;

		private Socket client;

		private bool disposed;

		private TcpClient.Properties values;

		private int recv_timeout;

		private int send_timeout;

		private int recv_buffer_size;

		private int send_buffer_size;

		private LingerOption linger_state;

		private bool no_delay;

		protected bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}

		public Socket Client
		{
			get
			{
				return this.client;
			}
			set
			{
				this.client = value;
				this.stream = null;
			}
		}

		public int Available
		{
			get
			{
				return this.client.Available;
			}
		}

		public bool Connected
		{
			get
			{
				return this.client.Connected;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return this.client.ExclusiveAddressUse;
			}
			set
			{
				this.client.ExclusiveAddressUse = value;
			}
		}

		public LingerOption LingerState
		{
			get
			{
				if ((this.values & TcpClient.Properties.LingerState) != (TcpClient.Properties)0u)
				{
					return this.linger_state;
				}
				return (LingerOption)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.linger_state = value;
					this.values |= TcpClient.Properties.LingerState;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		public bool NoDelay
		{
			get
			{
				if ((this.values & TcpClient.Properties.NoDelay) != (TcpClient.Properties)0u)
				{
					return this.no_delay;
				}
				return (bool)this.client.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.no_delay = value;
					this.values |= TcpClient.Properties.NoDelay;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				if ((this.values & TcpClient.Properties.ReceiveBufferSize) != (TcpClient.Properties)0u)
				{
					return this.recv_buffer_size;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.recv_buffer_size = value;
					this.values |= TcpClient.Properties.ReceiveBufferSize;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				if ((this.values & TcpClient.Properties.ReceiveTimeout) != (TcpClient.Properties)0u)
				{
					return this.recv_timeout;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.recv_timeout = value;
					this.values |= TcpClient.Properties.ReceiveTimeout;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		public int SendBufferSize
		{
			get
			{
				if ((this.values & TcpClient.Properties.SendBufferSize) != (TcpClient.Properties)0u)
				{
					return this.send_buffer_size;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.send_buffer_size = value;
					this.values |= TcpClient.Properties.SendBufferSize;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		public int SendTimeout
		{
			get
			{
				if ((this.values & TcpClient.Properties.SendTimeout) != (TcpClient.Properties)0u)
				{
					return this.send_timeout;
				}
				return (int)this.client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (!this.client.Connected)
				{
					this.send_timeout = value;
					this.values |= TcpClient.Properties.SendTimeout;
					return;
				}
				this.client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		public TimeSpan ConnectTimeout
		{
			get;
			set;
		}

		public TimeSpan WriteTimeout
		{
			get
			{
				return TimeSpan.FromMilliseconds((double)this.SendTimeout);
			}
			set
			{
				this.ReceiveTimeout = value.Milliseconds;
			}
		}

		public TimeSpan ReadTimeout
		{
			get
			{
				return TimeSpan.FromMilliseconds((double)this.SendTimeout);
			}
			set
			{
				this.SendTimeout = value.Milliseconds;
			}
		}

		public bool UseHTTPSProtocol
		{
			get;
			set;
		}

		public int HTTPSProtocol
		{
			get;
			set;
		}

		private void Init(AddressFamily family)
		{
			this.active = false;
			if (this.client != null)
			{
				this.client.Close();
				this.client = null;
			}
			this.client = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
		}

		public TcpClient()
		{
			this.Init(AddressFamily.InterNetwork);
			this.client.Bind(new IPEndPoint(IPAddress.Any, 0));
			this.ConnectTimeout = TimeSpan.FromSeconds(2.0);
		}

		public TcpClient(AddressFamily family)
		{
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			this.Init(family);
			IPAddress address = IPAddress.Any;
			if (family == AddressFamily.InterNetworkV6)
			{
				address = IPAddress.IPv6Any;
			}
			this.client.Bind(new IPEndPoint(address, 0));
			this.ConnectTimeout = TimeSpan.FromSeconds(2.0);
		}

		public TcpClient(IPEndPoint localEP)
		{
			this.Init(localEP.AddressFamily);
			this.client.Bind(localEP);
			this.ConnectTimeout = TimeSpan.FromSeconds(2.0);
		}

		public TcpClient(string hostname, int port)
		{
			this.ConnectTimeout = TimeSpan.FromSeconds(2.0);
			this.Connect(hostname, port);
		}

		public bool IsConnected()
		{
			bool result;
			try
			{
				result = (!this.Client.Poll(1, SelectMode.SelectRead) || this.Client.Available != 0);
			}
			catch (SocketException)
			{
				result = false;
			}
			return result;
		}

		internal void SetTcpClient(Socket s)
		{
			this.Client = s;
		}

		public void UpgradeToSSL()
		{
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		public void Connect(IPEndPoint remoteEP)
		{
			try
			{
				ManualResetEvent mre = new ManualResetEvent(false);
				IAsyncResult asyncResult = this.client.BeginConnect(remoteEP, delegate(IAsyncResult res)
				{
					mre.Set();
				}, null);
				this.active = mre.WaitOne(this.ConnectTimeout);
				if (!this.active)
				{
					try
					{
						this.client.Close();
					}
					catch
					{
					}
					throw new TimeoutException("Connection timed out!");
				}
				this.client.EndConnect(asyncResult);
			}
			finally
			{
				this.CheckDisposed();
			}
		}

		public void Connect(IPAddress address, int port)
		{
			this.Connect(new IPEndPoint(address, port));
		}

		private void SetOptions()
		{
			TcpClient.Properties properties = this.values;
			this.values = (TcpClient.Properties)0u;
			if ((properties & TcpClient.Properties.LingerState) != (TcpClient.Properties)0u)
			{
				this.LingerState = this.linger_state;
			}
			if ((properties & TcpClient.Properties.NoDelay) != (TcpClient.Properties)0u)
			{
				this.NoDelay = this.no_delay;
			}
			if ((properties & TcpClient.Properties.ReceiveBufferSize) != (TcpClient.Properties)0u)
			{
				this.ReceiveBufferSize = this.recv_buffer_size;
			}
			if ((properties & TcpClient.Properties.ReceiveTimeout) != (TcpClient.Properties)0u)
			{
				this.ReceiveTimeout = this.recv_timeout;
			}
			if ((properties & TcpClient.Properties.SendBufferSize) != (TcpClient.Properties)0u)
			{
				this.SendBufferSize = this.send_buffer_size;
			}
			if ((properties & TcpClient.Properties.SendTimeout) != (TcpClient.Properties)0u)
			{
				this.SendTimeout = this.send_timeout;
			}
		}

		public void Connect(string hostname, int port)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
			this.Connect(hostAddresses, port);
		}

		public void Connect(IPAddress[] ipAddresses, int port)
		{
			this.CheckDisposed();
			if (ipAddresses == null)
			{
				throw new ArgumentNullException("ipAddresses");
			}
			for (int i = 0; i < ipAddresses.Length; i++)
			{
				try
				{
					IPAddress iPAddress = ipAddresses[i];
					if (iPAddress.Equals(IPAddress.Any) || iPAddress.Equals(IPAddress.IPv6Any))
					{
						throw new SocketException(10049);
					}
					this.Init(iPAddress.AddressFamily);
					if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						this.client.Bind(new IPEndPoint(IPAddress.Any, 0));
					}
					else
					{
						if (iPAddress.AddressFamily != AddressFamily.InterNetworkV6)
						{
							throw new NotSupportedException("This method is only valid for sockets in the InterNetwork and InterNetworkV6 families");
						}
						this.client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
					}
					this.Connect(new IPEndPoint(iPAddress, port));
					if (this.values != (TcpClient.Properties)0u)
					{
						this.SetOptions();
					}
					break;
				}
				catch (Exception ex)
				{
					this.Init(AddressFamily.InterNetwork);
					if (i == ipAddresses.Length - 1)
					{
						throw ex;
					}
				}
			}
		}

		public void EndConnect(IAsyncResult asyncResult)
		{
			this.client.EndConnect(asyncResult);
		}

		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback requestCallback, object state)
		{
			return this.client.BeginConnect(address, port, requestCallback, state);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback requestCallback, object state)
		{
			return this.client.BeginConnect(addresses, port, requestCallback, state);
		}

		public IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state)
		{
			return this.client.BeginConnect(host, port, requestCallback, state);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (disposing)
			{
				NetworkStream networkStream = this.stream;
				this.stream = null;
				if (networkStream != null)
				{
					networkStream.Close();
					this.active = false;
					return;
				}
				if (this.client != null)
				{
					this.client.Close();
					this.client = null;
				}
			}
		}

		~TcpClient()
		{
			this.Dispose(false);
		}

		public Stream GetStream()
		{
			Stream result;
			try
			{
				if (this.stream == null)
				{
					this.stream = new NetworkStream(this.client, true);
				}
				result = this.stream;
			}
			finally
			{
				this.CheckDisposed();
			}
			return result;
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}
	}
}
