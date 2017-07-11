using System;

namespace Org.BouncyCastle.Bcpg
{
	public enum PublicKeyAlgorithmTag
	{
		RsaGeneral = 1,
		RsaEncrypt,
		RsaSign,
		ElGamalEncrypt = 16,
		Dsa,
		EC,
		ECDH = 18,
		ECDsa,
		ElGamalGeneral,
		DiffieHellman,
		Experimental_1 = 100,
		Experimental_2,
		Experimental_3,
		Experimental_4,
		Experimental_5,
		Experimental_6,
		Experimental_7,
		Experimental_8,
		Experimental_9,
		Experimental_10,
		Experimental_11
	}
}
