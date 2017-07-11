using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class RsaesOaepParameters : Asn1Encodable
	{
		private AlgorithmIdentifier hashAlgorithm;

		private AlgorithmIdentifier maskGenAlgorithm;

		private AlgorithmIdentifier pSourceAlgorithm;

		public static readonly AlgorithmIdentifier DefaultHashAlgorithm = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);

		public static readonly AlgorithmIdentifier DefaultMaskGenFunction = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, RsaesOaepParameters.DefaultHashAlgorithm);

		public static readonly AlgorithmIdentifier DefaultPSourceAlgorithm = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdPSpecified, new DerOctetString(new byte[0]));

		public AlgorithmIdentifier HashAlgorithm
		{
			get
			{
				return this.hashAlgorithm;
			}
		}

		public AlgorithmIdentifier MaskGenAlgorithm
		{
			get
			{
				return this.maskGenAlgorithm;
			}
		}

		public AlgorithmIdentifier PSourceAlgorithm
		{
			get
			{
				return this.pSourceAlgorithm;
			}
		}

		public static RsaesOaepParameters GetInstance(object obj)
		{
			if (obj is RsaesOaepParameters)
			{
				return (RsaesOaepParameters)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RsaesOaepParameters((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public RsaesOaepParameters()
		{
			this.hashAlgorithm = RsaesOaepParameters.DefaultHashAlgorithm;
			this.maskGenAlgorithm = RsaesOaepParameters.DefaultMaskGenFunction;
			this.pSourceAlgorithm = RsaesOaepParameters.DefaultPSourceAlgorithm;
		}

		public RsaesOaepParameters(AlgorithmIdentifier hashAlgorithm, AlgorithmIdentifier maskGenAlgorithm, AlgorithmIdentifier pSourceAlgorithm)
		{
			this.hashAlgorithm = hashAlgorithm;
			this.maskGenAlgorithm = maskGenAlgorithm;
			this.pSourceAlgorithm = pSourceAlgorithm;
		}

		public RsaesOaepParameters(Asn1Sequence seq)
		{
			this.hashAlgorithm = RsaesOaepParameters.DefaultHashAlgorithm;
			this.maskGenAlgorithm = RsaesOaepParameters.DefaultMaskGenFunction;
			this.pSourceAlgorithm = RsaesOaepParameters.DefaultPSourceAlgorithm;
			for (int num = 0; num != seq.Count; num++)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[num];
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.hashAlgorithm = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
					break;
				case 1:
					this.maskGenAlgorithm = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
					break;
				case 2:
					this.pSourceAlgorithm = AlgorithmIdentifier.GetInstance(asn1TaggedObject, true);
					break;
				default:
					throw new ArgumentException("unknown tag");
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (!this.hashAlgorithm.Equals(RsaesOaepParameters.DefaultHashAlgorithm))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.hashAlgorithm)
				});
			}
			if (!this.maskGenAlgorithm.Equals(RsaesOaepParameters.DefaultMaskGenFunction))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.maskGenAlgorithm)
				});
			}
			if (!this.pSourceAlgorithm.Equals(RsaesOaepParameters.DefaultPSourceAlgorithm))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.pSourceAlgorithm)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
