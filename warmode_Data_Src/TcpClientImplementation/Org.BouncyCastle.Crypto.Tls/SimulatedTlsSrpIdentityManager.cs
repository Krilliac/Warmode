using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SimulatedTlsSrpIdentityManager : TlsSrpIdentityManager
	{
		private static readonly byte[] PREFIX_PASSWORD = Strings.ToByteArray("password");

		private static readonly byte[] PREFIX_SALT = Strings.ToByteArray("salt");

		protected readonly Srp6GroupParameters mGroup;

		protected readonly Srp6VerifierGenerator mVerifierGenerator;

		protected readonly IMac mMac;

		public static SimulatedTlsSrpIdentityManager GetRfc5054Default(Srp6GroupParameters group, byte[] seedKey)
		{
			Srp6VerifierGenerator srp6VerifierGenerator = new Srp6VerifierGenerator();
			srp6VerifierGenerator.Init(group, TlsUtilities.CreateHash(2));
			HMac hMac = new HMac(TlsUtilities.CreateHash(2));
			hMac.Init(new KeyParameter(seedKey));
			return new SimulatedTlsSrpIdentityManager(group, srp6VerifierGenerator, hMac);
		}

		public SimulatedTlsSrpIdentityManager(Srp6GroupParameters group, Srp6VerifierGenerator verifierGenerator, IMac mac)
		{
			this.mGroup = group;
			this.mVerifierGenerator = verifierGenerator;
			this.mMac = mac;
		}

		public virtual TlsSrpLoginParameters GetLoginParameters(byte[] identity)
		{
			this.mMac.BlockUpdate(SimulatedTlsSrpIdentityManager.PREFIX_SALT, 0, SimulatedTlsSrpIdentityManager.PREFIX_SALT.Length);
			this.mMac.BlockUpdate(identity, 0, identity.Length);
			byte[] array = new byte[this.mMac.GetMacSize()];
			this.mMac.DoFinal(array, 0);
			this.mMac.BlockUpdate(SimulatedTlsSrpIdentityManager.PREFIX_PASSWORD, 0, SimulatedTlsSrpIdentityManager.PREFIX_PASSWORD.Length);
			this.mMac.BlockUpdate(identity, 0, identity.Length);
			byte[] array2 = new byte[this.mMac.GetMacSize()];
			this.mMac.DoFinal(array2, 0);
			BigInteger verifier = this.mVerifierGenerator.GenerateVerifier(array, identity, array2);
			return new TlsSrpLoginParameters(this.mGroup, verifier, array);
		}
	}
}
