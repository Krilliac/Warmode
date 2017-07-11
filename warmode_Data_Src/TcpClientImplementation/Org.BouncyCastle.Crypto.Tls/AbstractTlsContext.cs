using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Threading;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal abstract class AbstractTlsContext : TlsContext
	{
		private static long counter = Times.NanoTime();

		private readonly IRandomGenerator mNonceRandom;

		private readonly SecureRandom mSecureRandom;

		private readonly SecurityParameters mSecurityParameters;

		private ProtocolVersion mClientVersion;

		private ProtocolVersion mServerVersion;

		private TlsSession mSession;

		private object mUserObject;

		public virtual IRandomGenerator NonceRandomGenerator
		{
			get
			{
				return this.mNonceRandom;
			}
		}

		public virtual SecureRandom SecureRandom
		{
			get
			{
				return this.mSecureRandom;
			}
		}

		public virtual SecurityParameters SecurityParameters
		{
			get
			{
				return this.mSecurityParameters;
			}
		}

		public abstract bool IsServer
		{
			get;
		}

		public virtual ProtocolVersion ClientVersion
		{
			get
			{
				return this.mClientVersion;
			}
		}

		public virtual ProtocolVersion ServerVersion
		{
			get
			{
				return this.mServerVersion;
			}
		}

		public virtual TlsSession ResumableSession
		{
			get
			{
				return this.mSession;
			}
		}

		public virtual object UserObject
		{
			get
			{
				return this.mUserObject;
			}
			set
			{
				this.mUserObject = value;
			}
		}

		private static long NextCounterValue()
		{
			return Interlocked.Increment(ref AbstractTlsContext.counter);
		}

		internal AbstractTlsContext(SecureRandom secureRandom, SecurityParameters securityParameters)
		{
			IDigest digest = TlsUtilities.CreateHash(4);
			byte[] array = new byte[digest.GetDigestSize()];
			secureRandom.NextBytes(array);
			this.mNonceRandom = new DigestRandomGenerator(digest);
			this.mNonceRandom.AddSeedMaterial(AbstractTlsContext.NextCounterValue());
			this.mNonceRandom.AddSeedMaterial(Times.NanoTime());
			this.mNonceRandom.AddSeedMaterial(array);
			this.mSecureRandom = secureRandom;
			this.mSecurityParameters = securityParameters;
		}

		internal virtual void SetClientVersion(ProtocolVersion clientVersion)
		{
			this.mClientVersion = clientVersion;
		}

		internal virtual void SetServerVersion(ProtocolVersion serverVersion)
		{
			this.mServerVersion = serverVersion;
		}

		internal virtual void SetResumableSession(TlsSession session)
		{
			this.mSession = session;
		}

		public virtual byte[] ExportKeyingMaterial(string asciiLabel, byte[] context_value, int length)
		{
			if (context_value != null && !TlsUtilities.IsValidUint16(context_value.Length))
			{
				throw new ArgumentException("must have length less than 2^16 (or be null)", "context_value");
			}
			SecurityParameters securityParameters = this.SecurityParameters;
			byte[] clientRandom = securityParameters.ClientRandom;
			byte[] serverRandom = securityParameters.ServerRandom;
			int num = clientRandom.Length + serverRandom.Length;
			if (context_value != null)
			{
				num += 2 + context_value.Length;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			Array.Copy(clientRandom, 0, array, num2, clientRandom.Length);
			num2 += clientRandom.Length;
			Array.Copy(serverRandom, 0, array, num2, serverRandom.Length);
			num2 += serverRandom.Length;
			if (context_value != null)
			{
				TlsUtilities.WriteUint16(context_value.Length, array, num2);
				num2 += 2;
				Array.Copy(context_value, 0, array, num2, context_value.Length);
				num2 += context_value.Length;
			}
			if (num2 != num)
			{
				throw new InvalidOperationException("error in calculation of seed for export");
			}
			return TlsUtilities.PRF(this, securityParameters.MasterSecret, asciiLabel, array, length);
		}
	}
}
