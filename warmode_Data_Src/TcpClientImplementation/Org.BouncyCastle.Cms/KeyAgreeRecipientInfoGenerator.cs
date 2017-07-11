using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Cms.Ecc;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	internal class KeyAgreeRecipientInfoGenerator : RecipientInfoGenerator
	{
		private static readonly CmsEnvelopedHelper Helper = CmsEnvelopedHelper.Instance;

		private DerObjectIdentifier keyAgreementOID;

		private DerObjectIdentifier keyEncryptionOID;

		private IList recipientCerts;

		private AsymmetricCipherKeyPair senderKeyPair;

		internal DerObjectIdentifier KeyAgreementOID
		{
			set
			{
				this.keyAgreementOID = value;
			}
		}

		internal DerObjectIdentifier KeyEncryptionOID
		{
			set
			{
				this.keyEncryptionOID = value;
			}
		}

		internal ICollection RecipientCerts
		{
			set
			{
				this.recipientCerts = Platform.CreateArrayList(value);
			}
		}

		internal AsymmetricCipherKeyPair SenderKeyPair
		{
			set
			{
				this.senderKeyPair = value;
			}
		}

		internal KeyAgreeRecipientInfoGenerator()
		{
		}

		public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
		{
			byte[] key = contentEncryptionKey.GetKey();
			AsymmetricKeyParameter @public = this.senderKeyPair.Public;
			ICipherParameters cipherParameters = this.senderKeyPair.Private;
			OriginatorIdentifierOrKey originator;
			try
			{
				originator = new OriginatorIdentifierOrKey(KeyAgreeRecipientInfoGenerator.CreateOriginatorPublicKey(@public));
			}
			catch (IOException arg)
			{
				throw new InvalidKeyException("cannot extract originator public key: " + arg);
			}
			Asn1OctetString ukm = null;
			if (this.keyAgreementOID.Id.Equals(CmsEnvelopedGenerator.ECMqvSha1Kdf))
			{
				try
				{
					IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(this.keyAgreementOID);
					keyPairGenerator.Init(((ECPublicKeyParameters)@public).CreateKeyGenerationParameters(random));
					AsymmetricCipherKeyPair asymmetricCipherKeyPair = keyPairGenerator.GenerateKeyPair();
					ukm = new DerOctetString(new MQVuserKeyingMaterial(KeyAgreeRecipientInfoGenerator.CreateOriginatorPublicKey(asymmetricCipherKeyPair.Public), null));
					cipherParameters = new MqvPrivateParameters((ECPrivateKeyParameters)cipherParameters, (ECPrivateKeyParameters)asymmetricCipherKeyPair.Private, (ECPublicKeyParameters)asymmetricCipherKeyPair.Public);
				}
				catch (IOException arg2)
				{
					throw new InvalidKeyException("cannot extract MQV ephemeral public key: " + arg2);
				}
				catch (SecurityUtilityException arg3)
				{
					throw new InvalidKeyException("cannot determine MQV ephemeral key pair parameters from public key: " + arg3);
				}
			}
			DerSequence parameters = new DerSequence(new Asn1Encodable[]
			{
				this.keyEncryptionOID,
				DerNull.Instance
			});
			AlgorithmIdentifier keyEncryptionAlgorithm = new AlgorithmIdentifier(this.keyAgreementOID, parameters);
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (X509Certificate x509Certificate in this.recipientCerts)
			{
				TbsCertificateStructure instance;
				try
				{
					instance = TbsCertificateStructure.GetInstance(Asn1Object.FromByteArray(x509Certificate.GetTbsCertificate()));
				}
				catch (Exception)
				{
					throw new ArgumentException("can't extract TBS structure from certificate");
				}
				IssuerAndSerialNumber issuerSerial = new IssuerAndSerialNumber(instance.Issuer, instance.SerialNumber.Value);
				KeyAgreeRecipientIdentifier id = new KeyAgreeRecipientIdentifier(issuerSerial);
				ICipherParameters cipherParameters2 = x509Certificate.GetPublicKey();
				if (this.keyAgreementOID.Id.Equals(CmsEnvelopedGenerator.ECMqvSha1Kdf))
				{
					cipherParameters2 = new MqvPublicParameters((ECPublicKeyParameters)cipherParameters2, (ECPublicKeyParameters)cipherParameters2);
				}
				IBasicAgreement basicAgreementWithKdf = AgreementUtilities.GetBasicAgreementWithKdf(this.keyAgreementOID, this.keyEncryptionOID.Id);
				basicAgreementWithKdf.Init(new ParametersWithRandom(cipherParameters, random));
				BigInteger s = basicAgreementWithKdf.CalculateAgreement(cipherParameters2);
				int qLength = GeneratorUtilities.GetDefaultKeySize(this.keyEncryptionOID) / 8;
				byte[] keyBytes = X9IntegerConverter.IntegerToBytes(s, qLength);
				KeyParameter parameters2 = ParameterUtilities.CreateKeyParameter(this.keyEncryptionOID, keyBytes);
				IWrapper wrapper = KeyAgreeRecipientInfoGenerator.Helper.CreateWrapper(this.keyEncryptionOID.Id);
				wrapper.Init(true, new ParametersWithRandom(parameters2, random));
				byte[] str = wrapper.Wrap(key, 0, key.Length);
				Asn1OctetString encryptedKey = new DerOctetString(str);
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new RecipientEncryptedKey(id, encryptedKey)
				});
			}
			return new RecipientInfo(new KeyAgreeRecipientInfo(originator, ukm, keyEncryptionAlgorithm, new DerSequence(asn1EncodableVector)));
		}

		private static OriginatorPublicKey CreateOriginatorPublicKey(AsymmetricKeyParameter publicKey)
		{
			SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
			return new OriginatorPublicKey(new AlgorithmIdentifier(subjectPublicKeyInfo.AlgorithmID.ObjectID, DerNull.Instance), subjectPublicKeyInfo.PublicKeyData.GetBytes());
		}
	}
}
