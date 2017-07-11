using System;

namespace Org.BouncyCastle.Asn1.CryptoPro
{
	public class Gost3410PublicKeyAlgParameters : Asn1Encodable
	{
		private DerObjectIdentifier publicKeyParamSet;

		private DerObjectIdentifier digestParamSet;

		private DerObjectIdentifier encryptionParamSet;

		public DerObjectIdentifier PublicKeyParamSet
		{
			get
			{
				return this.publicKeyParamSet;
			}
		}

		public DerObjectIdentifier DigestParamSet
		{
			get
			{
				return this.digestParamSet;
			}
		}

		public DerObjectIdentifier EncryptionParamSet
		{
			get
			{
				return this.encryptionParamSet;
			}
		}

		public static Gost3410PublicKeyAlgParameters GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return Gost3410PublicKeyAlgParameters.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static Gost3410PublicKeyAlgParameters GetInstance(object obj)
		{
			if (obj == null || obj is Gost3410PublicKeyAlgParameters)
			{
				return (Gost3410PublicKeyAlgParameters)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Gost3410PublicKeyAlgParameters((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid GOST3410Parameter: " + obj.GetType().Name);
		}

		public Gost3410PublicKeyAlgParameters(DerObjectIdentifier publicKeyParamSet, DerObjectIdentifier digestParamSet) : this(publicKeyParamSet, digestParamSet, null)
		{
		}

		public Gost3410PublicKeyAlgParameters(DerObjectIdentifier publicKeyParamSet, DerObjectIdentifier digestParamSet, DerObjectIdentifier encryptionParamSet)
		{
			if (publicKeyParamSet == null)
			{
				throw new ArgumentNullException("publicKeyParamSet");
			}
			if (digestParamSet == null)
			{
				throw new ArgumentNullException("digestParamSet");
			}
			this.publicKeyParamSet = publicKeyParamSet;
			this.digestParamSet = digestParamSet;
			this.encryptionParamSet = encryptionParamSet;
		}

		public Gost3410PublicKeyAlgParameters(Asn1Sequence seq)
		{
			this.publicKeyParamSet = (DerObjectIdentifier)seq[0];
			this.digestParamSet = (DerObjectIdentifier)seq[1];
			if (seq.Count > 2)
			{
				this.encryptionParamSet = (DerObjectIdentifier)seq[2];
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.publicKeyParamSet,
				this.digestParamSet
			});
			if (this.encryptionParamSet != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.encryptionParamSet
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
