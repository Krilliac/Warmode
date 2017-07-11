using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class EncryptedContentInfo : Asn1Encodable
	{
		private DerObjectIdentifier contentType;

		private AlgorithmIdentifier contentEncryptionAlgorithm;

		private Asn1OctetString encryptedContent;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public AlgorithmIdentifier ContentEncryptionAlgorithm
		{
			get
			{
				return this.contentEncryptionAlgorithm;
			}
		}

		public Asn1OctetString EncryptedContent
		{
			get
			{
				return this.encryptedContent;
			}
		}

		public EncryptedContentInfo(DerObjectIdentifier contentType, AlgorithmIdentifier contentEncryptionAlgorithm, Asn1OctetString encryptedContent)
		{
			this.contentType = contentType;
			this.contentEncryptionAlgorithm = contentEncryptionAlgorithm;
			this.encryptedContent = encryptedContent;
		}

		public EncryptedContentInfo(Asn1Sequence seq)
		{
			this.contentType = (DerObjectIdentifier)seq[0];
			this.contentEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
			if (seq.Count > 2)
			{
				this.encryptedContent = Asn1OctetString.GetInstance((Asn1TaggedObject)seq[2], false);
			}
		}

		public static EncryptedContentInfo GetInstance(object obj)
		{
			if (obj == null || obj is EncryptedContentInfo)
			{
				return (EncryptedContentInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new EncryptedContentInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid EncryptedContentInfo: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.contentType,
				this.contentEncryptionAlgorithm
			});
			if (this.encryptedContent != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new BerTaggedObject(false, 0, this.encryptedContent)
				});
			}
			return new BerSequence(asn1EncodableVector);
		}
	}
}
