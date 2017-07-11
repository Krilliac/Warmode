using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EncryptedContentInfoParser
	{
		private DerObjectIdentifier _contentType;

		private AlgorithmIdentifier _contentEncryptionAlgorithm;

		private Asn1TaggedObjectParser _encryptedContent;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return this._contentType;
			}
		}

		public AlgorithmIdentifier ContentEncryptionAlgorithm
		{
			get
			{
				return this._contentEncryptionAlgorithm;
			}
		}

		public EncryptedContentInfoParser(Asn1SequenceParser seq)
		{
			this._contentType = (DerObjectIdentifier)seq.ReadObject();
			this._contentEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq.ReadObject().ToAsn1Object());
			this._encryptedContent = (Asn1TaggedObjectParser)seq.ReadObject();
		}

		public IAsn1Convertible GetEncryptedContent(int tag)
		{
			return this._encryptedContent.GetObjectParser(tag, false);
		}
	}
}
