using System;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class Accuracy : Asn1Encodable
	{
		protected const int MinMillis = 1;

		protected const int MaxMillis = 999;

		protected const int MinMicros = 1;

		protected const int MaxMicros = 999;

		private readonly DerInteger seconds;

		private readonly DerInteger millis;

		private readonly DerInteger micros;

		public DerInteger Seconds
		{
			get
			{
				return this.seconds;
			}
		}

		public DerInteger Millis
		{
			get
			{
				return this.millis;
			}
		}

		public DerInteger Micros
		{
			get
			{
				return this.micros;
			}
		}

		public Accuracy(DerInteger seconds, DerInteger millis, DerInteger micros)
		{
			if (millis != null && (millis.Value.IntValue < 1 || millis.Value.IntValue > 999))
			{
				throw new ArgumentException("Invalid millis field : not in (1..999)");
			}
			if (micros != null && (micros.Value.IntValue < 1 || micros.Value.IntValue > 999))
			{
				throw new ArgumentException("Invalid micros field : not in (1..999)");
			}
			this.seconds = seconds;
			this.millis = millis;
			this.micros = micros;
		}

		private Accuracy(Asn1Sequence seq)
		{
			for (int i = 0; i < seq.Count; i++)
			{
				if (seq[i] is DerInteger)
				{
					this.seconds = (DerInteger)seq[i];
				}
				else if (seq[i] is DerTaggedObject)
				{
					DerTaggedObject derTaggedObject = (DerTaggedObject)seq[i];
					switch (derTaggedObject.TagNo)
					{
					case 0:
						this.millis = DerInteger.GetInstance(derTaggedObject, false);
						if (this.millis.Value.IntValue < 1 || this.millis.Value.IntValue > 999)
						{
							throw new ArgumentException("Invalid millis field : not in (1..999).");
						}
						break;
					case 1:
						this.micros = DerInteger.GetInstance(derTaggedObject, false);
						if (this.micros.Value.IntValue < 1 || this.micros.Value.IntValue > 999)
						{
							throw new ArgumentException("Invalid micros field : not in (1..999).");
						}
						break;
					default:
						throw new ArgumentException("Invalig tag number");
					}
				}
			}
		}

		public static Accuracy GetInstance(object o)
		{
			if (o == null || o is Accuracy)
			{
				return (Accuracy)o;
			}
			if (o is Asn1Sequence)
			{
				return new Accuracy((Asn1Sequence)o);
			}
			throw new ArgumentException("Unknown object in 'Accuracy' factory: " + o.GetType().FullName);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.seconds != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.seconds
				});
			}
			if (this.millis != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.millis)
				});
			}
			if (this.micros != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.micros)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
