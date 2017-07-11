using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class OriginatorPublicKey : Asn1Encodable
	{
		private AlgorithmIdentifier algorithm;

		private DerBitString publicKey;

		public AlgorithmIdentifier Algorithm
		{
			get
			{
				return this.algorithm;
			}
		}

		public DerBitString PublicKey
		{
			get
			{
				return this.publicKey;
			}
		}

		public OriginatorPublicKey(AlgorithmIdentifier algorithm, byte[] publicKey)
		{
			this.algorithm = algorithm;
			this.publicKey = new DerBitString(publicKey);
		}

		public OriginatorPublicKey(Asn1Sequence seq)
		{
			this.algorithm = AlgorithmIdentifier.GetInstance(seq[0]);
			this.publicKey = (DerBitString)seq[1];
		}

		public static OriginatorPublicKey GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return OriginatorPublicKey.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static OriginatorPublicKey GetInstance(object obj)
		{
			if (obj == null || obj is OriginatorPublicKey)
			{
				return (OriginatorPublicKey)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new OriginatorPublicKey((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid OriginatorPublicKey: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.algorithm,
				this.publicKey
			});
		}
	}
}
