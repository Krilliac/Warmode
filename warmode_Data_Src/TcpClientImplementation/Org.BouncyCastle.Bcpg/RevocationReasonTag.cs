using System;

namespace Org.BouncyCastle.Bcpg
{
	public enum RevocationReasonTag : byte
	{
		NoReason,
		KeySuperseded,
		KeyCompromised,
		KeyRetired,
		UserNoLongerValid = 32
	}
}
