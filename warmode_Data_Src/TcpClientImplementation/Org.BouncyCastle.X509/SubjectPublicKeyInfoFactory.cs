using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.X509
{
	public sealed class SubjectPublicKeyInfoFactory
	{
		private SubjectPublicKeyInfoFactory()
		{
		}

		public static SubjectPublicKeyInfo CreateSubjectPublicKeyInfo(AsymmetricKeyParameter key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (key.IsPrivate)
			{
				throw new ArgumentException("Private key passed - public key expected.", "key");
			}
			if (key is ElGamalPublicKeyParameters)
			{
				ElGamalPublicKeyParameters elGamalPublicKeyParameters = (ElGamalPublicKeyParameters)key;
				ElGamalParameters parameters = elGamalPublicKeyParameters.Parameters;
				return new SubjectPublicKeyInfo(new AlgorithmIdentifier(OiwObjectIdentifiers.ElGamalAlgorithm, new ElGamalParameter(parameters.P, parameters.G).ToAsn1Object()), new DerInteger(elGamalPublicKeyParameters.Y));
			}
			if (key is DsaPublicKeyParameters)
			{
				DsaPublicKeyParameters dsaPublicKeyParameters = (DsaPublicKeyParameters)key;
				DsaParameters parameters2 = dsaPublicKeyParameters.Parameters;
				Asn1Encodable parameters3 = (parameters2 == null) ? null : new DsaParameter(parameters2.P, parameters2.Q, parameters2.G).ToAsn1Object();
				return new SubjectPublicKeyInfo(new AlgorithmIdentifier(X9ObjectIdentifiers.IdDsa, parameters3), new DerInteger(dsaPublicKeyParameters.Y));
			}
			if (key is DHPublicKeyParameters)
			{
				DHPublicKeyParameters dHPublicKeyParameters = (DHPublicKeyParameters)key;
				DHParameters parameters4 = dHPublicKeyParameters.Parameters;
				return new SubjectPublicKeyInfo(new AlgorithmIdentifier(dHPublicKeyParameters.AlgorithmOid, new DHParameter(parameters4.P, parameters4.G, parameters4.L).ToAsn1Object()), new DerInteger(dHPublicKeyParameters.Y));
			}
			if (key is RsaKeyParameters)
			{
				RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)key;
				return new SubjectPublicKeyInfo(new AlgorithmIdentifier(PkcsObjectIdentifiers.RsaEncryption, DerNull.Instance), new RsaPublicKeyStructure(rsaKeyParameters.Modulus, rsaKeyParameters.Exponent).ToAsn1Object());
			}
			if (key is ECPublicKeyParameters)
			{
				ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)key;
				if (!(eCPublicKeyParameters.AlgorithmName == "ECGOST3410"))
				{
					X962Parameters x962Parameters;
					if (eCPublicKeyParameters.PublicKeyParamSet == null)
					{
						ECDomainParameters parameters5 = eCPublicKeyParameters.Parameters;
						X9ECParameters ecParameters = new X9ECParameters(parameters5.Curve, parameters5.G, parameters5.N, parameters5.H, parameters5.GetSeed());
						x962Parameters = new X962Parameters(ecParameters);
					}
					else
					{
						x962Parameters = new X962Parameters(eCPublicKeyParameters.PublicKeyParamSet);
					}
					Asn1OctetString asn1OctetString = (Asn1OctetString)new X9ECPoint(eCPublicKeyParameters.Q).ToAsn1Object();
					AlgorithmIdentifier algID = new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, x962Parameters.ToAsn1Object());
					return new SubjectPublicKeyInfo(algID, asn1OctetString.GetOctets());
				}
				if (eCPublicKeyParameters.PublicKeyParamSet == null)
				{
					throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");
				}
				ECPoint eCPoint = eCPublicKeyParameters.Q.Normalize();
				BigInteger bI = eCPoint.AffineXCoord.ToBigInteger();
				BigInteger bI2 = eCPoint.AffineYCoord.ToBigInteger();
				byte[] array = new byte[64];
				SubjectPublicKeyInfoFactory.ExtractBytes(array, 0, bI);
				SubjectPublicKeyInfoFactory.ExtractBytes(array, 32, bI2);
				Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters = new Gost3410PublicKeyAlgParameters(eCPublicKeyParameters.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet);
				AlgorithmIdentifier algID2 = new AlgorithmIdentifier(CryptoProObjectIdentifiers.GostR3410x2001, gost3410PublicKeyAlgParameters.ToAsn1Object());
				return new SubjectPublicKeyInfo(algID2, new DerOctetString(array));
			}
			else
			{
				if (!(key is Gost3410PublicKeyParameters))
				{
					throw new ArgumentException("Class provided no convertible: " + key.GetType().FullName);
				}
				Gost3410PublicKeyParameters gost3410PublicKeyParameters = (Gost3410PublicKeyParameters)key;
				if (gost3410PublicKeyParameters.PublicKeyParamSet == null)
				{
					throw Platform.CreateNotImplementedException("Not a CryptoPro parameter set");
				}
				byte[] array2 = gost3410PublicKeyParameters.Y.ToByteArrayUnsigned();
				byte[] array3 = new byte[array2.Length];
				for (int num = 0; num != array3.Length; num++)
				{
					array3[num] = array2[array2.Length - 1 - num];
				}
				Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters2 = new Gost3410PublicKeyAlgParameters(gost3410PublicKeyParameters.PublicKeyParamSet, CryptoProObjectIdentifiers.GostR3411x94CryptoProParamSet);
				AlgorithmIdentifier algID3 = new AlgorithmIdentifier(CryptoProObjectIdentifiers.GostR3410x94, gost3410PublicKeyAlgParameters2.ToAsn1Object());
				return new SubjectPublicKeyInfo(algID3, new DerOctetString(array3));
			}
		}

		private static void ExtractBytes(byte[] encKey, int offset, BigInteger bI)
		{
			byte[] array = bI.ToByteArray();
			int num = (bI.BitLength + 7) / 8;
			for (int i = 0; i < num; i++)
			{
				encKey[offset + i] = array[array.Length - 1 - i];
			}
		}
	}
}
