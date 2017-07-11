using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class SignerInformation
	{
		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private SignerID sid;

		private Org.BouncyCastle.Asn1.Cms.SignerInfo info;

		private AlgorithmIdentifier digestAlgorithm;

		private AlgorithmIdentifier encryptionAlgorithm;

		private readonly Asn1Set signedAttributeSet;

		private readonly Asn1Set unsignedAttributeSet;

		private CmsProcessable content;

		private byte[] signature;

		private DerObjectIdentifier contentType;

		private IDigestCalculator digestCalculator;

		private byte[] resultDigest;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttributeTable;

		private Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributeTable;

		private readonly bool isCounterSignature;

		public bool IsCounterSignature
		{
			get
			{
				return this.isCounterSignature;
			}
		}

		public DerObjectIdentifier ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public SignerID SignerID
		{
			get
			{
				return this.sid;
			}
		}

		public int Version
		{
			get
			{
				return this.info.Version.Value.IntValue;
			}
		}

		public AlgorithmIdentifier DigestAlgorithmID
		{
			get
			{
				return this.digestAlgorithm;
			}
		}

		public string DigestAlgOid
		{
			get
			{
				return this.digestAlgorithm.ObjectID.Id;
			}
		}

		public Asn1Object DigestAlgParams
		{
			get
			{
				Asn1Encodable parameters = this.digestAlgorithm.Parameters;
				if (parameters != null)
				{
					return parameters.ToAsn1Object();
				}
				return null;
			}
		}

		public AlgorithmIdentifier EncryptionAlgorithmID
		{
			get
			{
				return this.encryptionAlgorithm;
			}
		}

		public string EncryptionAlgOid
		{
			get
			{
				return this.encryptionAlgorithm.ObjectID.Id;
			}
		}

		public Asn1Object EncryptionAlgParams
		{
			get
			{
				Asn1Encodable parameters = this.encryptionAlgorithm.Parameters;
				if (parameters != null)
				{
					return parameters.ToAsn1Object();
				}
				return null;
			}
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable SignedAttributes
		{
			get
			{
				if (this.signedAttributeSet != null && this.signedAttributeTable == null)
				{
					this.signedAttributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(this.signedAttributeSet);
				}
				return this.signedAttributeTable;
			}
		}

		public Org.BouncyCastle.Asn1.Cms.AttributeTable UnsignedAttributes
		{
			get
			{
				if (this.unsignedAttributeSet != null && this.unsignedAttributeTable == null)
				{
					this.unsignedAttributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(this.unsignedAttributeSet);
				}
				return this.unsignedAttributeTable;
			}
		}

		internal SignerInformation(Org.BouncyCastle.Asn1.Cms.SignerInfo info, DerObjectIdentifier contentType, CmsProcessable content, IDigestCalculator digestCalculator)
		{
			this.info = info;
			this.sid = new SignerID();
			this.contentType = contentType;
			this.isCounterSignature = (contentType == null);
			try
			{
				SignerIdentifier signerID = info.SignerID;
				if (signerID.IsTagged)
				{
					Asn1OctetString instance = Asn1OctetString.GetInstance(signerID.ID);
					this.sid.SubjectKeyIdentifier = instance.GetEncoded();
				}
				else
				{
					Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber instance2 = Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber.GetInstance(signerID.ID);
					this.sid.Issuer = instance2.Name;
					this.sid.SerialNumber = instance2.SerialNumber.Value;
				}
			}
			catch (IOException)
			{
				throw new ArgumentException("invalid sid in SignerInfo");
			}
			this.digestAlgorithm = info.DigestAlgorithm;
			this.signedAttributeSet = info.AuthenticatedAttributes;
			this.unsignedAttributeSet = info.UnauthenticatedAttributes;
			this.encryptionAlgorithm = info.DigestEncryptionAlgorithm;
			this.signature = info.EncryptedDigest.GetOctets();
			this.content = content;
			this.digestCalculator = digestCalculator;
		}

		public byte[] GetContentDigest()
		{
			if (this.resultDigest == null)
			{
				throw new InvalidOperationException("method can only be called after verify.");
			}
			return (byte[])this.resultDigest.Clone();
		}

		public byte[] GetSignature()
		{
			return (byte[])this.signature.Clone();
		}

		public SignerInformationStore GetCounterSignatures()
		{
			Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributes = this.UnsignedAttributes;
			if (unsignedAttributes == null)
			{
				return new SignerInformationStore(Platform.CreateArrayList(0));
			}
			IList list = Platform.CreateArrayList();
			Asn1EncodableVector all = unsignedAttributes.GetAll(CmsAttributes.CounterSignature);
			foreach (Org.BouncyCastle.Asn1.Cms.Attribute attribute in all)
			{
				Asn1Set attrValues = attribute.AttrValues;
				int arg_53_0 = attrValues.Count;
				foreach (Asn1Encodable asn1Encodable in attrValues)
				{
					Org.BouncyCastle.Asn1.Cms.SignerInfo instance = Org.BouncyCastle.Asn1.Cms.SignerInfo.GetInstance(asn1Encodable.ToAsn1Object());
					string digestAlgName = CmsSignedHelper.Instance.GetDigestAlgName(instance.DigestAlgorithm.ObjectID.Id);
					list.Add(new SignerInformation(instance, null, null, new CounterSignatureDigestCalculator(digestAlgName, this.GetSignature())));
				}
			}
			return new SignerInformationStore(list);
		}

		public byte[] GetEncodedSignedAttributes()
		{
			if (this.signedAttributeSet != null)
			{
				return this.signedAttributeSet.GetEncoded("DER");
			}
			return null;
		}

		private bool DoVerify(AsymmetricKeyParameter key)
		{
			string digestAlgName = SignerInformation.Helper.GetDigestAlgName(this.DigestAlgOid);
			IDigest digestInstance = SignerInformation.Helper.GetDigestInstance(digestAlgName);
			DerObjectIdentifier objectID = this.encryptionAlgorithm.ObjectID;
			Asn1Encodable parameters = this.encryptionAlgorithm.Parameters;
			ISigner signer;
			if (objectID.Equals(PkcsObjectIdentifiers.IdRsassaPss))
			{
				if (parameters == null)
				{
					throw new CmsException("RSASSA-PSS signature must specify algorithm parameters");
				}
				try
				{
					RsassaPssParameters instance = RsassaPssParameters.GetInstance(parameters.ToAsn1Object());
					if (!instance.HashAlgorithm.ObjectID.Equals(this.digestAlgorithm.ObjectID))
					{
						throw new CmsException("RSASSA-PSS signature parameters specified incorrect hash algorithm");
					}
					if (!instance.MaskGenAlgorithm.ObjectID.Equals(PkcsObjectIdentifiers.IdMgf1))
					{
						throw new CmsException("RSASSA-PSS signature parameters specified unknown MGF");
					}
					IDigest digest = DigestUtilities.GetDigest(instance.HashAlgorithm.ObjectID);
					int intValue = instance.SaltLength.Value.IntValue;
					byte b = (byte)instance.TrailerField.Value.IntValue;
					if (b != 1)
					{
						throw new CmsException("RSASSA-PSS signature parameters must have trailerField of 1");
					}
					signer = new PssSigner(new RsaBlindedEngine(), digest, intValue);
					goto IL_142;
				}
				catch (Exception e)
				{
					throw new CmsException("failed to set RSASSA-PSS signature parameters", e);
				}
			}
			string algorithm = digestAlgName + "with" + SignerInformation.Helper.GetEncryptionAlgName(this.EncryptionAlgOid);
			signer = SignerInformation.Helper.GetSignatureInstance(algorithm);
			try
			{
				IL_142:
				if (this.digestCalculator != null)
				{
					this.resultDigest = this.digestCalculator.GetDigest();
				}
				else
				{
					if (this.content != null)
					{
						this.content.Write(new DigOutputStream(digestInstance));
					}
					else if (this.signedAttributeSet == null)
					{
						throw new CmsException("data not encapsulated in signature - use detached constructor.");
					}
					this.resultDigest = DigestUtilities.DoFinal(digestInstance);
				}
			}
			catch (IOException e2)
			{
				throw new CmsException("can't process mime object to create signature.", e2);
			}
			Asn1Object singleValuedSignedAttribute = this.GetSingleValuedSignedAttribute(CmsAttributes.ContentType, "content-type");
			if (singleValuedSignedAttribute == null)
			{
				if (!this.isCounterSignature && this.signedAttributeSet != null)
				{
					throw new CmsException("The content-type attribute type MUST be present whenever signed attributes are present in signed-data");
				}
			}
			else
			{
				if (this.isCounterSignature)
				{
					throw new CmsException("[For counter signatures,] the signedAttributes field MUST NOT contain a content-type attribute");
				}
				if (!(singleValuedSignedAttribute is DerObjectIdentifier))
				{
					throw new CmsException("content-type attribute value not of ASN.1 type 'OBJECT IDENTIFIER'");
				}
				DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)singleValuedSignedAttribute;
				if (!derObjectIdentifier.Equals(this.contentType))
				{
					throw new CmsException("content-type attribute value does not match eContentType");
				}
			}
			Asn1Object singleValuedSignedAttribute2 = this.GetSingleValuedSignedAttribute(CmsAttributes.MessageDigest, "message-digest");
			if (singleValuedSignedAttribute2 == null)
			{
				if (this.signedAttributeSet != null)
				{
					throw new CmsException("the message-digest signed attribute type MUST be present when there are any signed attributes present");
				}
			}
			else
			{
				if (!(singleValuedSignedAttribute2 is Asn1OctetString))
				{
					throw new CmsException("message-digest attribute value not of ASN.1 type 'OCTET STRING'");
				}
				Asn1OctetString asn1OctetString = (Asn1OctetString)singleValuedSignedAttribute2;
				if (!Arrays.AreEqual(this.resultDigest, asn1OctetString.GetOctets()))
				{
					throw new CmsException("message-digest attribute value does not match calculated value");
				}
			}
			Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttributes = this.SignedAttributes;
			if (signedAttributes != null && signedAttributes.GetAll(CmsAttributes.CounterSignature).Count > 0)
			{
				throw new CmsException("A countersignature attribute MUST NOT be a signed attribute");
			}
			Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributes = this.UnsignedAttributes;
			if (unsignedAttributes != null)
			{
				foreach (Org.BouncyCastle.Asn1.Cms.Attribute attribute in unsignedAttributes.GetAll(CmsAttributes.CounterSignature))
				{
					if (attribute.AttrValues.Count < 1)
					{
						throw new CmsException("A countersignature attribute MUST contain at least one AttributeValue");
					}
				}
			}
			bool result;
			try
			{
				signer.Init(false, key);
				if (this.signedAttributeSet == null)
				{
					if (this.digestCalculator != null)
					{
						result = this.VerifyDigest(this.resultDigest, key, this.GetSignature());
						return result;
					}
					if (this.content != null)
					{
						this.content.Write(new SigOutputStream(signer));
					}
				}
				else
				{
					byte[] encodedSignedAttributes = this.GetEncodedSignedAttributes();
					signer.BlockUpdate(encodedSignedAttributes, 0, encodedSignedAttributes.Length);
				}
				result = signer.VerifySignature(this.GetSignature());
			}
			catch (InvalidKeyException e3)
			{
				throw new CmsException("key not appropriate to signature in message.", e3);
			}
			catch (IOException e4)
			{
				throw new CmsException("can't process mime object to create signature.", e4);
			}
			catch (SignatureException ex)
			{
				throw new CmsException("invalid signature format in message: " + ex.Message, ex);
			}
			return result;
		}

		private bool IsNull(Asn1Encodable o)
		{
			return o is Asn1Null || o == null;
		}

		private DigestInfo DerDecode(byte[] encoding)
		{
			if (encoding[0] != 48)
			{
				throw new IOException("not a digest info object");
			}
			DigestInfo instance = DigestInfo.GetInstance(Asn1Object.FromByteArray(encoding));
			if (instance.GetEncoded().Length != encoding.Length)
			{
				throw new CmsException("malformed RSA signature");
			}
			return instance;
		}

		private bool VerifyDigest(byte[] digest, AsymmetricKeyParameter key, byte[] signature)
		{
			string encryptionAlgName = SignerInformation.Helper.GetEncryptionAlgName(this.EncryptionAlgOid);
			bool result;
			try
			{
				if (encryptionAlgName.Equals("RSA"))
				{
					IBufferedCipher bufferedCipher = CmsEnvelopedHelper.Instance.CreateAsymmetricCipher("RSA/ECB/PKCS1Padding");
					bufferedCipher.Init(false, key);
					byte[] encoding = bufferedCipher.DoFinal(signature);
					DigestInfo digestInfo = this.DerDecode(encoding);
					if (!digestInfo.AlgorithmID.ObjectID.Equals(this.digestAlgorithm.ObjectID))
					{
						result = false;
					}
					else if (!this.IsNull(digestInfo.AlgorithmID.Parameters))
					{
						result = false;
					}
					else
					{
						byte[] digest2 = digestInfo.GetDigest();
						result = Arrays.ConstantTimeAreEqual(digest, digest2);
					}
				}
				else
				{
					if (!encryptionAlgName.Equals("DSA"))
					{
						throw new CmsException("algorithm: " + encryptionAlgName + " not supported in base signatures.");
					}
					ISigner signer = SignerUtilities.GetSigner("NONEwithDSA");
					signer.Init(false, key);
					signer.BlockUpdate(digest, 0, digest.Length);
					result = signer.VerifySignature(signature);
				}
			}
			catch (SecurityUtilityException ex)
			{
				throw ex;
			}
			catch (GeneralSecurityException ex2)
			{
				throw new CmsException("Exception processing signature: " + ex2, ex2);
			}
			catch (IOException ex3)
			{
				throw new CmsException("Exception decoding signature: " + ex3, ex3);
			}
			return result;
		}

		public bool Verify(AsymmetricKeyParameter pubKey)
		{
			if (pubKey.IsPrivate)
			{
				throw new ArgumentException("Expected public key", "pubKey");
			}
			this.GetSigningTime();
			return this.DoVerify(pubKey);
		}

		public bool Verify(X509Certificate cert)
		{
			Org.BouncyCastle.Asn1.Cms.Time signingTime = this.GetSigningTime();
			if (signingTime != null)
			{
				cert.CheckValidity(signingTime.Date);
			}
			return this.DoVerify(cert.GetPublicKey());
		}

		public Org.BouncyCastle.Asn1.Cms.SignerInfo ToSignerInfo()
		{
			return this.info;
		}

		private Asn1Object GetSingleValuedSignedAttribute(DerObjectIdentifier attrOID, string printableName)
		{
			Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributes = this.UnsignedAttributes;
			if (unsignedAttributes != null && unsignedAttributes.GetAll(attrOID).Count > 0)
			{
				throw new CmsException("The " + printableName + " attribute MUST NOT be an unsigned attribute");
			}
			Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttributes = this.SignedAttributes;
			if (signedAttributes == null)
			{
				return null;
			}
			Asn1EncodableVector all = signedAttributes.GetAll(attrOID);
			switch (all.Count)
			{
			case 0:
				return null;
			case 1:
			{
				Org.BouncyCastle.Asn1.Cms.Attribute attribute = (Org.BouncyCastle.Asn1.Cms.Attribute)all[0];
				Asn1Set attrValues = attribute.AttrValues;
				if (attrValues.Count != 1)
				{
					throw new CmsException("A " + printableName + " attribute MUST have a single attribute value");
				}
				return attrValues[0].ToAsn1Object();
			}
			default:
				throw new CmsException("The SignedAttributes in a signerInfo MUST NOT include multiple instances of the " + printableName + " attribute");
			}
		}

		private Org.BouncyCastle.Asn1.Cms.Time GetSigningTime()
		{
			Asn1Object singleValuedSignedAttribute = this.GetSingleValuedSignedAttribute(CmsAttributes.SigningTime, "signing-time");
			if (singleValuedSignedAttribute == null)
			{
				return null;
			}
			Org.BouncyCastle.Asn1.Cms.Time instance;
			try
			{
				instance = Org.BouncyCastle.Asn1.Cms.Time.GetInstance(singleValuedSignedAttribute);
			}
			catch (ArgumentException)
			{
				throw new CmsException("signing-time attribute value not a valid 'Time' structure");
			}
			return instance;
		}

		public static SignerInformation ReplaceUnsignedAttributes(SignerInformation signerInformation, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributes)
		{
			Org.BouncyCastle.Asn1.Cms.SignerInfo signerInfo = signerInformation.info;
			Asn1Set unauthenticatedAttributes = null;
			if (unsignedAttributes != null)
			{
				unauthenticatedAttributes = new DerSet(unsignedAttributes.ToAsn1EncodableVector());
			}
			return new SignerInformation(new Org.BouncyCastle.Asn1.Cms.SignerInfo(signerInfo.SignerID, signerInfo.DigestAlgorithm, signerInfo.AuthenticatedAttributes, signerInfo.DigestEncryptionAlgorithm, signerInfo.EncryptedDigest, unauthenticatedAttributes), signerInformation.contentType, signerInformation.content, null);
		}

		public static SignerInformation AddCounterSigners(SignerInformation signerInformation, SignerInformationStore counterSigners)
		{
			Org.BouncyCastle.Asn1.Cms.SignerInfo signerInfo = signerInformation.info;
			Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributes = signerInformation.UnsignedAttributes;
			Asn1EncodableVector asn1EncodableVector;
			if (unsignedAttributes != null)
			{
				asn1EncodableVector = unsignedAttributes.ToAsn1EncodableVector();
			}
			else
			{
				asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			}
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (SignerInformation signerInformation2 in counterSigners.GetSigners())
			{
				asn1EncodableVector2.Add(new Asn1Encodable[]
				{
					signerInformation2.ToSignerInfo()
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				new Org.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.CounterSignature, new DerSet(asn1EncodableVector2))
			});
			return new SignerInformation(new Org.BouncyCastle.Asn1.Cms.SignerInfo(signerInfo.SignerID, signerInfo.DigestAlgorithm, signerInfo.AuthenticatedAttributes, signerInfo.DigestEncryptionAlgorithm, signerInfo.EncryptedDigest, new DerSet(asn1EncodableVector)), signerInformation.contentType, signerInformation.content, null);
		}
	}
}
