using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Cms
{
	public class KekRecipientInformation : RecipientInformation
	{
		private KekRecipientInfo info;

		internal KekRecipientInformation(KekRecipientInfo info, CmsSecureReadable secureReadable) : base(info.KeyEncryptionAlgorithm, secureReadable)
		{
			this.info = info;
			this.rid = new RecipientID();
			KekIdentifier kekID = info.KekID;
			this.rid.KeyIdentifier = kekID.KeyIdentifier.GetOctets();
		}

		public override CmsTypedStream GetContentStream(ICipherParameters key)
		{
			CmsTypedStream contentFromSessionKey;
			try
			{
				byte[] octets = this.info.EncryptedKey.GetOctets();
				IWrapper wrapper = WrapperUtilities.GetWrapper(this.keyEncAlg.ObjectID.Id);
				wrapper.Init(false, key);
				KeyParameter sKey = ParameterUtilities.CreateKeyParameter(base.GetContentAlgorithmName(), wrapper.Unwrap(octets, 0, octets.Length));
				contentFromSessionKey = base.GetContentFromSessionKey(sKey);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e2)
			{
				throw new CmsException("key invalid in message.", e2);
			}
			return contentFromSessionKey;
		}
	}
}
