using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public abstract class RecipientInformation
	{
		internal RecipientID rid = new RecipientID();

		internal AlgorithmIdentifier keyEncAlg;

		internal CmsSecureReadable secureReadable;

		private byte[] resultMac;

		public RecipientID RecipientID
		{
			get
			{
				return this.rid;
			}
		}

		public AlgorithmIdentifier KeyEncryptionAlgorithmID
		{
			get
			{
				return this.keyEncAlg;
			}
		}

		public string KeyEncryptionAlgOid
		{
			get
			{
				return this.keyEncAlg.ObjectID.Id;
			}
		}

		public Asn1Object KeyEncryptionAlgParams
		{
			get
			{
				Asn1Encodable parameters = this.keyEncAlg.Parameters;
				if (parameters != null)
				{
					return parameters.ToAsn1Object();
				}
				return null;
			}
		}

		internal RecipientInformation(AlgorithmIdentifier keyEncAlg, CmsSecureReadable secureReadable)
		{
			this.keyEncAlg = keyEncAlg;
			this.secureReadable = secureReadable;
		}

		internal string GetContentAlgorithmName()
		{
			AlgorithmIdentifier algorithm = this.secureReadable.Algorithm;
			return algorithm.ObjectID.Id;
		}

		internal CmsTypedStream GetContentFromSessionKey(KeyParameter sKey)
		{
			CmsReadable readable = this.secureReadable.GetReadable(sKey);
			CmsTypedStream result;
			try
			{
				result = new CmsTypedStream(readable.GetInputStream());
			}
			catch (IOException e)
			{
				throw new CmsException("error getting .", e);
			}
			return result;
		}

		public byte[] GetContent(ICipherParameters key)
		{
			byte[] result;
			try
			{
				result = CmsUtilities.StreamToByteArray(this.GetContentStream(key).ContentStream);
			}
			catch (IOException arg)
			{
				throw new Exception("unable to parse internal stream: " + arg);
			}
			return result;
		}

		public byte[] GetMac()
		{
			if (this.resultMac == null)
			{
				object cryptoObject = this.secureReadable.CryptoObject;
				if (cryptoObject is IMac)
				{
					this.resultMac = MacUtilities.DoFinal((IMac)cryptoObject);
				}
			}
			return Arrays.Clone(this.resultMac);
		}

		public abstract CmsTypedStream GetContentStream(ICipherParameters key);
	}
}
