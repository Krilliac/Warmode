using System;

namespace Org.BouncyCastle.Bcpg
{
	public enum SignatureSubpacketTag
	{
		CreationTime = 2,
		ExpireTime,
		Exportable,
		TrustSig,
		RegExp,
		Revocable,
		KeyExpireTime = 9,
		Placeholder,
		PreferredSymmetricAlgorithms,
		RevocationKey,
		IssuerKeyId = 16,
		NotationData = 20,
		PreferredHashAlgorithms,
		PreferredCompressionAlgorithms,
		KeyServerPreferences,
		PreferredKeyServer,
		PrimaryUserId,
		PolicyUrl,
		KeyFlags,
		SignerUserId,
		RevocationReason,
		Features,
		SignatureTarget,
		EmbeddedSignature
	}
}
