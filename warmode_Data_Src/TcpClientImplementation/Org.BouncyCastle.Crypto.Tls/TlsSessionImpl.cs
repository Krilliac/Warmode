using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class TlsSessionImpl : TlsSession
	{
		internal readonly byte[] mSessionID;

		internal SessionParameters mSessionParameters;

		public virtual byte[] SessionID
		{
			get
			{
				byte[] result;
				lock (this)
				{
					result = this.mSessionID;
				}
				return result;
			}
		}

		public virtual bool IsResumable
		{
			get
			{
				bool result;
				lock (this)
				{
					result = (this.mSessionParameters != null);
				}
				return result;
			}
		}

		internal TlsSessionImpl(byte[] sessionID, SessionParameters sessionParameters)
		{
			if (sessionID == null)
			{
				throw new ArgumentNullException("sessionID");
			}
			if (sessionID.Length < 1 || sessionID.Length > 32)
			{
				throw new ArgumentException("must have length between 1 and 32 bytes, inclusive", "sessionID");
			}
			this.mSessionID = Arrays.Clone(sessionID);
			this.mSessionParameters = sessionParameters;
		}

		public virtual SessionParameters ExportSessionParameters()
		{
			SessionParameters result;
			lock (this)
			{
				result = ((this.mSessionParameters == null) ? null : this.mSessionParameters.Copy());
			}
			return result;
		}

		public virtual void Invalidate()
		{
			lock (this)
			{
				if (this.mSessionParameters != null)
				{
					this.mSessionParameters.Clear();
					this.mSessionParameters = null;
				}
			}
		}
	}
}
