using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class KeyTransRecipientInformation : RecipientInformation
	{
		private KeyTransRecipientInfo info;

		internal KeyTransRecipientInformation(KeyTransRecipientInfo info, CmsSecureReadable secureReadable) : base(info.KeyEncryptionAlgorithm, secureReadable)
		{
			this.info = info;
			this.rid = new RecipientID();
			RecipientIdentifier recipientIdentifier = info.RecipientIdentifier;
			try
			{
				if (recipientIdentifier.IsTagged)
				{
					Asn1OctetString instance = Asn1OctetString.GetInstance(recipientIdentifier.ID);
					this.rid.SubjectKeyIdentifier = instance.GetOctets();
				}
				else
				{
					Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber instance2 = Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber.GetInstance(recipientIdentifier.ID);
					this.rid.Issuer = instance2.Name;
					this.rid.SerialNumber = instance2.SerialNumber.Value;
				}
			}
			catch (IOException)
			{
				throw new ArgumentException("invalid rid in KeyTransRecipientInformation");
			}
		}

		private string GetExchangeEncryptionAlgorithmName(DerObjectIdentifier oid)
		{
			if (PkcsObjectIdentifiers.RsaEncryption.Equals(oid))
			{
				return "RSA//PKCS1Padding";
			}
			return oid.Id;
		}

		internal KeyParameter UnwrapKey(ICipherParameters key)
		{
			byte[] octets = this.info.EncryptedKey.GetOctets();
			string exchangeEncryptionAlgorithmName = this.GetExchangeEncryptionAlgorithmName(this.keyEncAlg.ObjectID);
			KeyParameter result;
			try
			{
				IWrapper wrapper = WrapperUtilities.GetWrapper(exchangeEncryptionAlgorithmName);
				wrapper.Init(false, key);
				result = ParameterUtilities.CreateKeyParameter(base.GetContentAlgorithmName(), wrapper.Unwrap(octets, 0, octets.Length));
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e2)
			{
				throw new CmsException("key invalid in message.", e2);
			}
			catch (DataLengthException e3)
			{
				throw new CmsException("illegal blocksize in message.", e3);
			}
			catch (InvalidCipherTextException e4)
			{
				throw new CmsException("bad padding in message.", e4);
			}
			return result;
		}

		public override CmsTypedStream GetContentStream(ICipherParameters key)
		{
			KeyParameter sKey = this.UnwrapKey(key);
			return base.GetContentFromSessionKey(sKey);
		}
	}
}
