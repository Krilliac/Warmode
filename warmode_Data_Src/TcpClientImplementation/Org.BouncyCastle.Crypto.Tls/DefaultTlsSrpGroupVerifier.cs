using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DefaultTlsSrpGroupVerifier : TlsSrpGroupVerifier
	{
		protected static readonly IList DefaultGroups;

		protected readonly IList mGroups;

		static DefaultTlsSrpGroupVerifier()
		{
			DefaultTlsSrpGroupVerifier.DefaultGroups = Platform.CreateArrayList();
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_1024);
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_1536);
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_2048);
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_3072);
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_4096);
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_6144);
			DefaultTlsSrpGroupVerifier.DefaultGroups.Add(Srp6StandardGroups.rfc5054_8192);
		}

		public DefaultTlsSrpGroupVerifier() : this(DefaultTlsSrpGroupVerifier.DefaultGroups)
		{
		}

		public DefaultTlsSrpGroupVerifier(IList groups)
		{
			this.mGroups = groups;
		}

		public virtual bool Accept(Srp6GroupParameters group)
		{
			foreach (Srp6GroupParameters b in this.mGroups)
			{
				if (this.AreGroupsEqual(group, b))
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool AreGroupsEqual(Srp6GroupParameters a, Srp6GroupParameters b)
		{
			return a == b || (this.AreParametersEqual(a.N, b.N) && this.AreParametersEqual(a.G, b.G));
		}

		protected virtual bool AreParametersEqual(BigInteger a, BigInteger b)
		{
			return a == b || a.Equals(b);
		}
	}
}
