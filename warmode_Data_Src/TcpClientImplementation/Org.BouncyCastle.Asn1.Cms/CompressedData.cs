using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class CompressedData : Asn1Encodable
	{
		private DerInteger version;

		private AlgorithmIdentifier compressionAlgorithm;

		private ContentInfo encapContentInfo;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public AlgorithmIdentifier CompressionAlgorithmIdentifier
		{
			get
			{
				return this.compressionAlgorithm;
			}
		}

		public ContentInfo EncapContentInfo
		{
			get
			{
				return this.encapContentInfo;
			}
		}

		public CompressedData(AlgorithmIdentifier compressionAlgorithm, ContentInfo encapContentInfo)
		{
			this.version = new DerInteger(0);
			this.compressionAlgorithm = compressionAlgorithm;
			this.encapContentInfo = encapContentInfo;
		}

		public CompressedData(Asn1Sequence seq)
		{
			this.version = (DerInteger)seq[0];
			this.compressionAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
			this.encapContentInfo = ContentInfo.GetInstance(seq[2]);
		}

		public static CompressedData GetInstance(Asn1TaggedObject ato, bool explicitly)
		{
			return CompressedData.GetInstance(Asn1Sequence.GetInstance(ato, explicitly));
		}

		public static CompressedData GetInstance(object obj)
		{
			if (obj == null || obj is CompressedData)
			{
				return (CompressedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CompressedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid CompressedData: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new BerSequence(new Asn1Encodable[]
			{
				this.version,
				this.compressionAlgorithm,
				this.encapContentInfo
			});
		}
	}
}
