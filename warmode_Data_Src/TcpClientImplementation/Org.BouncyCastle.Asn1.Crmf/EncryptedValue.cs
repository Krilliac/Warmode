using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class EncryptedValue : Asn1Encodable
	{
		private readonly AlgorithmIdentifier intendedAlg;

		private readonly AlgorithmIdentifier symmAlg;

		private readonly DerBitString encSymmKey;

		private readonly AlgorithmIdentifier keyAlg;

		private readonly Asn1OctetString valueHint;

		private readonly DerBitString encValue;

		public virtual AlgorithmIdentifier IntendedAlg
		{
			get
			{
				return this.intendedAlg;
			}
		}

		public virtual AlgorithmIdentifier SymmAlg
		{
			get
			{
				return this.symmAlg;
			}
		}

		public virtual DerBitString EncSymmKey
		{
			get
			{
				return this.encSymmKey;
			}
		}

		public virtual AlgorithmIdentifier KeyAlg
		{
			get
			{
				return this.keyAlg;
			}
		}

		public virtual Asn1OctetString ValueHint
		{
			get
			{
				return this.valueHint;
			}
		}

		public virtual DerBitString EncValue
		{
			get
			{
				return this.encValue;
			}
		}

		private EncryptedValue(Asn1Sequence seq)
		{
			int num = 0;
			while (seq[num] is Asn1TaggedObject)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[num];
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.intendedAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 1:
					this.symmAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 2:
					this.encSymmKey = DerBitString.GetInstance(asn1TaggedObject, false);
					break;
				case 3:
					this.keyAlg = AlgorithmIdentifier.GetInstance(asn1TaggedObject, false);
					break;
				case 4:
					this.valueHint = Asn1OctetString.GetInstance(asn1TaggedObject, false);
					break;
				}
				num++;
			}
			this.encValue = DerBitString.GetInstance(seq[num]);
		}

		public static EncryptedValue GetInstance(object obj)
		{
			if (obj is EncryptedValue)
			{
				return (EncryptedValue)obj;
			}
			if (obj != null)
			{
				return new EncryptedValue(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		public EncryptedValue(AlgorithmIdentifier intendedAlg, AlgorithmIdentifier symmAlg, DerBitString encSymmKey, AlgorithmIdentifier keyAlg, Asn1OctetString valueHint, DerBitString encValue)
		{
			if (encValue == null)
			{
				throw new ArgumentNullException("encValue");
			}
			this.intendedAlg = intendedAlg;
			this.symmAlg = symmAlg;
			this.encSymmKey = encSymmKey;
			this.keyAlg = keyAlg;
			this.valueHint = valueHint;
			this.encValue = encValue;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			this.AddOptional(asn1EncodableVector, 0, this.intendedAlg);
			this.AddOptional(asn1EncodableVector, 1, this.symmAlg);
			this.AddOptional(asn1EncodableVector, 2, this.encSymmKey);
			this.AddOptional(asn1EncodableVector, 3, this.keyAlg);
			this.AddOptional(asn1EncodableVector, 4, this.valueHint);
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.encValue
			});
			return new DerSequence(asn1EncodableVector);
		}

		private void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
		{
			if (obj != null)
			{
				v.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, tagNo, obj)
				});
			}
		}
	}
}
