using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class CompressedDataParser
	{
		private DerInteger _version;

		private AlgorithmIdentifier _compressionAlgorithm;

		private ContentInfoParser _encapContentInfo;

		public DerInteger Version
		{
			get
			{
				return this._version;
			}
		}

		public AlgorithmIdentifier CompressionAlgorithmIdentifier
		{
			get
			{
				return this._compressionAlgorithm;
			}
		}

		public CompressedDataParser(Asn1SequenceParser seq)
		{
			this._version = (DerInteger)seq.ReadObject();
			this._compressionAlgorithm = AlgorithmIdentifier.GetInstance(seq.ReadObject().ToAsn1Object());
			this._encapContentInfo = new ContentInfoParser((Asn1SequenceParser)seq.ReadObject());
		}

		public ContentInfoParser GetEncapContentInfo()
		{
			return this._encapContentInfo;
		}
	}
}
