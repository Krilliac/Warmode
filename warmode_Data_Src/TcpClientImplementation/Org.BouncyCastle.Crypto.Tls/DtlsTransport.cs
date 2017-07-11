using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DtlsTransport : DatagramTransport
	{
		private readonly DtlsRecordLayer mRecordLayer;

		internal DtlsTransport(DtlsRecordLayer recordLayer)
		{
			this.mRecordLayer = recordLayer;
		}

		public virtual int GetReceiveLimit()
		{
			return this.mRecordLayer.GetReceiveLimit();
		}

		public virtual int GetSendLimit()
		{
			return this.mRecordLayer.GetSendLimit();
		}

		public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
		{
			int result;
			try
			{
				result = this.mRecordLayer.Receive(buf, off, len, waitMillis);
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				this.mRecordLayer.Fail(tlsFatalAlert.AlertDescription);
				throw tlsFatalAlert;
			}
			catch (IOException ex)
			{
				this.mRecordLayer.Fail(80);
				throw ex;
			}
			catch (Exception alertCause)
			{
				this.mRecordLayer.Fail(80);
				throw new TlsFatalAlert(80, alertCause);
			}
			return result;
		}

		public virtual void Send(byte[] buf, int off, int len)
		{
			try
			{
				this.mRecordLayer.Send(buf, off, len);
			}
			catch (TlsFatalAlert tlsFatalAlert)
			{
				this.mRecordLayer.Fail(tlsFatalAlert.AlertDescription);
				throw tlsFatalAlert;
			}
			catch (IOException ex)
			{
				this.mRecordLayer.Fail(80);
				throw ex;
			}
			catch (Exception alertCause)
			{
				this.mRecordLayer.Fail(80);
				throw new TlsFatalAlert(80, alertCause);
			}
		}

		public virtual void Close()
		{
			this.mRecordLayer.Close();
		}
	}
}
