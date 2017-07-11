using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class GeneralSubtree : Asn1Encodable
	{
		private readonly GeneralName baseName;

		private readonly DerInteger minimum;

		private readonly DerInteger maximum;

		public GeneralName Base
		{
			get
			{
				return this.baseName;
			}
		}

		public BigInteger Minimum
		{
			get
			{
				if (this.minimum != null)
				{
					return this.minimum.Value;
				}
				return BigInteger.Zero;
			}
		}

		public BigInteger Maximum
		{
			get
			{
				if (this.maximum != null)
				{
					return this.maximum.Value;
				}
				return null;
			}
		}

		private GeneralSubtree(Asn1Sequence seq)
		{
			this.baseName = GeneralName.GetInstance(seq[0]);
			switch (seq.Count)
			{
			case 1:
				return;
			case 2:
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[1]);
				switch (instance.TagNo)
				{
				case 0:
					this.minimum = DerInteger.GetInstance(instance, false);
					return;
				case 1:
					this.maximum = DerInteger.GetInstance(instance, false);
					return;
				default:
					throw new ArgumentException("Bad tag number: " + instance.TagNo);
				}
				break;
			}
			case 3:
			{
				Asn1TaggedObject instance2 = Asn1TaggedObject.GetInstance(seq[1]);
				if (instance2.TagNo != 0)
				{
					throw new ArgumentException("Bad tag number for 'minimum': " + instance2.TagNo);
				}
				this.minimum = DerInteger.GetInstance(instance2, false);
				Asn1TaggedObject instance3 = Asn1TaggedObject.GetInstance(seq[2]);
				if (instance3.TagNo != 1)
				{
					throw new ArgumentException("Bad tag number for 'maximum': " + instance3.TagNo);
				}
				this.maximum = DerInteger.GetInstance(instance3, false);
				return;
			}
			default:
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
		}

		public GeneralSubtree(GeneralName baseName, BigInteger minimum, BigInteger maximum)
		{
			this.baseName = baseName;
			if (minimum != null)
			{
				this.minimum = new DerInteger(minimum);
			}
			if (maximum != null)
			{
				this.maximum = new DerInteger(maximum);
			}
		}

		public GeneralSubtree(GeneralName baseName) : this(baseName, null, null)
		{
		}

		public static GeneralSubtree GetInstance(Asn1TaggedObject o, bool isExplicit)
		{
			return new GeneralSubtree(Asn1Sequence.GetInstance(o, isExplicit));
		}

		public static GeneralSubtree GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is GeneralSubtree)
			{
				return (GeneralSubtree)obj;
			}
			return new GeneralSubtree(Asn1Sequence.GetInstance(obj));
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.baseName
			});
			if (this.minimum != null && this.minimum.Value.SignValue != 0)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.minimum)
				});
			}
			if (this.maximum != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.maximum)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
