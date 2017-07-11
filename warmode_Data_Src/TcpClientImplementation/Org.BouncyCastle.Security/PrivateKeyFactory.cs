using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Security
{
	public sealed class PrivateKeyFactory
	{
		private PrivateKeyFactory()
		{
		}

		public static AsymmetricKeyParameter CreateKey(byte[] privateKeyInfoData)
		{
			return PrivateKeyFactory.CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(privateKeyInfoData)));
		}

		public static AsymmetricKeyParameter CreateKey(Stream inStr)
		{
			return PrivateKeyFactory.CreateKey(PrivateKeyInfo.GetInstance(Asn1Object.FromStream(inStr)));
		}

		public static AsymmetricKeyParameter CreateKey(PrivateKeyInfo keyInfo)
		{
			AlgorithmIdentifier privateKeyAlgorithm = keyInfo.PrivateKeyAlgorithm;
			DerObjectIdentifier objectID = privateKeyAlgorithm.ObjectID;
			if (objectID.Equals(PkcsObjectIdentifiers.RsaEncryption) || objectID.Equals(X509ObjectIdentifiers.IdEARsa) || objectID.Equals(PkcsObjectIdentifiers.IdRsassaPss) || objectID.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
			{
				RsaPrivateKeyStructure instance = RsaPrivateKeyStructure.GetInstance(keyInfo.ParsePrivateKey());
				return new RsaPrivateCrtKeyParameters(instance.Modulus, instance.PublicExponent, instance.PrivateExponent, instance.Prime1, instance.Prime2, instance.Exponent1, instance.Exponent2, instance.Coefficient);
			}
			if (objectID.Equals(PkcsObjectIdentifiers.DhKeyAgreement))
			{
				DHParameter dHParameter = new DHParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				DerInteger derInteger = (DerInteger)keyInfo.ParsePrivateKey();
				BigInteger l = dHParameter.L;
				int l2 = (l == null) ? 0 : l.IntValue;
				DHParameters parameters = new DHParameters(dHParameter.P, dHParameter.G, null, l2);
				return new DHPrivateKeyParameters(derInteger.Value, parameters, objectID);
			}
			if (objectID.Equals(OiwObjectIdentifiers.ElGamalAlgorithm))
			{
				ElGamalParameter elGamalParameter = new ElGamalParameter(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				DerInteger derInteger2 = (DerInteger)keyInfo.ParsePrivateKey();
				return new ElGamalPrivateKeyParameters(derInteger2.Value, new ElGamalParameters(elGamalParameter.P, elGamalParameter.G));
			}
			if (objectID.Equals(X9ObjectIdentifiers.IdDsa))
			{
				DerInteger derInteger3 = (DerInteger)keyInfo.ParsePrivateKey();
				Asn1Encodable parameters2 = privateKeyAlgorithm.Parameters;
				DsaParameters parameters3 = null;
				if (parameters2 != null)
				{
					DsaParameter instance2 = DsaParameter.GetInstance(parameters2.ToAsn1Object());
					parameters3 = new DsaParameters(instance2.P, instance2.Q, instance2.G);
				}
				return new DsaPrivateKeyParameters(derInteger3.Value, parameters3);
			}
			if (objectID.Equals(X9ObjectIdentifiers.IdECPublicKey))
			{
				X962Parameters x962Parameters = new X962Parameters(privateKeyAlgorithm.Parameters.ToAsn1Object());
				X9ECParameters x9ECParameters;
				if (x962Parameters.IsNamedCurve)
				{
					x9ECParameters = ECKeyPairGenerator.FindECCurveByOid((DerObjectIdentifier)x962Parameters.Parameters);
				}
				else
				{
					x9ECParameters = new X9ECParameters((Asn1Sequence)x962Parameters.Parameters);
				}
				ECPrivateKeyStructure eCPrivateKeyStructure = new ECPrivateKeyStructure(Asn1Sequence.GetInstance(keyInfo.ParsePrivateKey()));
				BigInteger key = eCPrivateKeyStructure.GetKey();
				if (x962Parameters.IsNamedCurve)
				{
					return new ECPrivateKeyParameters("EC", key, (DerObjectIdentifier)x962Parameters.Parameters);
				}
				ECDomainParameters parameters4 = new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
				return new ECPrivateKeyParameters(key, parameters4);
			}
			else if (objectID.Equals(CryptoProObjectIdentifiers.GostR3410x2001))
			{
				Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters = new Gost3410PublicKeyAlgParameters(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
				Asn1Object asn1Object = keyInfo.ParsePrivateKey();
				ECPrivateKeyStructure eCPrivateKeyStructure2;
				if (asn1Object is DerInteger)
				{
					eCPrivateKeyStructure2 = new ECPrivateKeyStructure(((DerInteger)asn1Object).Value);
				}
				else
				{
					eCPrivateKeyStructure2 = ECPrivateKeyStructure.GetInstance(asn1Object);
				}
				if (ECGost3410NamedCurves.GetByOid(gost3410PublicKeyAlgParameters.PublicKeyParamSet) == null)
				{
					throw new ArgumentException("Unrecognized curve OID for GostR3410x2001 private key");
				}
				return new ECPrivateKeyParameters("ECGOST3410", eCPrivateKeyStructure2.GetKey(), gost3410PublicKeyAlgParameters.PublicKeyParamSet);
			}
			else
			{
				if (objectID.Equals(CryptoProObjectIdentifiers.GostR3410x94))
				{
					Gost3410PublicKeyAlgParameters gost3410PublicKeyAlgParameters2 = new Gost3410PublicKeyAlgParameters(Asn1Sequence.GetInstance(privateKeyAlgorithm.Parameters.ToAsn1Object()));
					DerOctetString derOctetString = (DerOctetString)keyInfo.ParsePrivateKey();
					BigInteger x = new BigInteger(1, Arrays.Reverse(derOctetString.GetOctets()));
					return new Gost3410PrivateKeyParameters(x, gost3410PublicKeyAlgParameters2.PublicKeyParamSet);
				}
				throw new SecurityUtilityException("algorithm identifier in key not recognised");
			}
		}

		public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, EncryptedPrivateKeyInfo encInfo)
		{
			return PrivateKeyFactory.CreateKey(PrivateKeyInfoFactory.CreatePrivateKeyInfo(passPhrase, encInfo));
		}

		public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, byte[] encryptedPrivateKeyInfoData)
		{
			return PrivateKeyFactory.DecryptKey(passPhrase, Asn1Object.FromByteArray(encryptedPrivateKeyInfoData));
		}

		public static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Stream encryptedPrivateKeyInfoStream)
		{
			return PrivateKeyFactory.DecryptKey(passPhrase, Asn1Object.FromStream(encryptedPrivateKeyInfoStream));
		}

		private static AsymmetricKeyParameter DecryptKey(char[] passPhrase, Asn1Object asn1Object)
		{
			return PrivateKeyFactory.DecryptKey(passPhrase, EncryptedPrivateKeyInfo.GetInstance(asn1Object));
		}

		public static byte[] EncryptKey(DerObjectIdentifier algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
		{
			return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
		}

		public static byte[] EncryptKey(string algorithm, char[] passPhrase, byte[] salt, int iterationCount, AsymmetricKeyParameter key)
		{
			return EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(algorithm, passPhrase, salt, iterationCount, key).GetEncoded();
		}
	}
}
