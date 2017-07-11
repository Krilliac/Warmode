using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class RsassaPssParameters : Asn1Encodable
	{
		private AlgorithmIdentifier hashAlgorithm;

		private AlgorithmIdentifier maskGenAlgorithm;

		private DerInteger saltLength;

		private DerInteger trailerField;

		public static readonly AlgorithmIdentifier DefaultHashAlgorithm = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);

		public static readonly AlgorithmIdentifier DefaultMaskGenFunction = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, RsassaPssParameters.DefaultHashAlgorithm);

		public static readonly DerInteger DefaultSaltLength = new DerInteger(20);

		public static readonly DerInteger DefaultTrailerField = new DerInteger(1);

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

		public DerInteger SaltLength
		{
			get
			{
				return this.saltLength;
			}
		}

		public DerInteger TrailerField
		{
			get
			{
				return this.trailerField;
			}
		}

		public static RsassaPssParameters GetInstance(object obj)
		{
			if (obj == null || obj is RsassaPssParameters)
			{
				return (RsassaPssParameters)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RsassaPssParameters((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public RsassaPssParameters()
		{
			this.hashAlgorithm = RsassaPssParameters.DefaultHashAlgorithm;
			this.maskGenAlgorithm = RsassaPssParameters.DefaultMaskGenFunction;
			this.saltLength = RsassaPssParameters.DefaultSaltLength;
			this.trailerField = RsassaPssParameters.DefaultTrailerField;
		}

		public RsassaPssParameters(AlgorithmIdentifier hashAlgorithm, AlgorithmIdentifier maskGenAlgorithm, DerInteger saltLength, DerInteger trailerField)
		{
			this.hashAlgorithm = hashAlgorithm;
			this.maskGenAlgorithm = maskGenAlgorithm;
			this.saltLength = saltLength;
			this.trailerField = trailerField;
		}

		public RsassaPssParameters(Asn1Sequence seq)
		{
			this.hashAlgorithm = RsassaPssParameters.DefaultHashAlgorithm;
			this.maskGenAlgorithm = RsassaPssParameters.DefaultMaskGenFunction;
			this.saltLength = RsassaPssParameters.DefaultSaltLength;
			this.trailerField = RsassaPssParameters.DefaultTrailerField;
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
					this.saltLength = DerInteger.GetInstance(asn1TaggedObject, true);
					break;
				case 3:
					this.trailerField = DerInteger.GetInstance(asn1TaggedObject, true);
					break;
				default:
					throw new ArgumentException("unknown tag");
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (!this.hashAlgorithm.Equals(RsassaPssParameters.DefaultHashAlgorithm))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.hashAlgorithm)
				});
			}
			if (!this.maskGenAlgorithm.Equals(RsassaPssParameters.DefaultMaskGenFunction))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.maskGenAlgorithm)
				});
			}
			if (!this.saltLength.Equals(RsassaPssParameters.DefaultSaltLength))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.saltLength)
				});
			}
			if (!this.trailerField.Equals(RsassaPssParameters.DefaultTrailerField))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 3, this.trailerField)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
