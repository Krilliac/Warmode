using System;

namespace Org.BouncyCastle.Asn1.X509.Qualified
{
	public class BiometricData : Asn1Encodable
	{
		private readonly TypeOfBiometricData typeOfBiometricData;

		private readonly AlgorithmIdentifier hashAlgorithm;

		private readonly Asn1OctetString biometricDataHash;

		private readonly DerIA5String sourceDataUri;

		public TypeOfBiometricData TypeOfBiometricData
		{
			get
			{
				return this.typeOfBiometricData;
			}
		}

		public AlgorithmIdentifier HashAlgorithm
		{
			get
			{
				return this.hashAlgorithm;
			}
		}

		public Asn1OctetString BiometricDataHash
		{
			get
			{
				return this.biometricDataHash;
			}
		}

		public DerIA5String SourceDataUri
		{
			get
			{
				return this.sourceDataUri;
			}
		}

		public static BiometricData GetInstance(object obj)
		{
			if (obj == null || obj is BiometricData)
			{
				return (BiometricData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new BiometricData(Asn1Sequence.GetInstance(obj));
			}
			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		private BiometricData(Asn1Sequence seq)
		{
			this.typeOfBiometricData = TypeOfBiometricData.GetInstance(seq[0]);
			this.hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
			this.biometricDataHash = Asn1OctetString.GetInstance(seq[2]);
			if (seq.Count > 3)
			{
				this.sourceDataUri = DerIA5String.GetInstance(seq[3]);
			}
		}

		public BiometricData(TypeOfBiometricData typeOfBiometricData, AlgorithmIdentifier hashAlgorithm, Asn1OctetString biometricDataHash, DerIA5String sourceDataUri)
		{
			this.typeOfBiometricData = typeOfBiometricData;
			this.hashAlgorithm = hashAlgorithm;
			this.biometricDataHash = biometricDataHash;
			this.sourceDataUri = sourceDataUri;
		}

		public BiometricData(TypeOfBiometricData typeOfBiometricData, AlgorithmIdentifier hashAlgorithm, Asn1OctetString biometricDataHash)
		{
			this.typeOfBiometricData = typeOfBiometricData;
			this.hashAlgorithm = hashAlgorithm;
			this.biometricDataHash = biometricDataHash;
			this.sourceDataUri = null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.typeOfBiometricData,
				this.hashAlgorithm,
				this.biometricDataHash
			});
			if (this.sourceDataUri != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.sourceDataUri
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
