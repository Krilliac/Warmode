using System;

namespace Org.BouncyCastle.Bcpg
{
	public enum PacketTag
	{
		Reserved,
		PublicKeyEncryptedSession,
		Signature,
		SymmetricKeyEncryptedSessionKey,
		OnePassSignature,
		SecretKey,
		PublicKey,
		SecretSubkey,
		CompressedData,
		SymmetricKeyEncrypted,
		Marker,
		LiteralData,
		Trust,
		UserId,
		PublicSubkey,
		UserAttribute = 17,
		SymmetricEncryptedIntegrityProtected,
		ModificationDetectionCode,
		Experimental1 = 60,
		Experimental2,
		Experimental3,
		Experimental4
	}
}
