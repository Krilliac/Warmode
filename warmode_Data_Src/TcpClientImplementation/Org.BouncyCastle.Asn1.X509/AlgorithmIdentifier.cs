using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AlgorithmIdentifier : Asn1Encodable
	{
		private readonly DerObjectIdentifier objectID;

		private readonly Asn1Encodable parameters;

		private readonly bool parametersDefined;

		public virtual DerObjectIdentifier ObjectID
		{
			get
			{
				return this.objectID;
			}
		}

		public Asn1Encodable Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public static AlgorithmIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return AlgorithmIdentifier.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static AlgorithmIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is AlgorithmIdentifier)
			{
				return (AlgorithmIdentifier)obj;
			}
			if (obj is DerObjectIdentifier)
			{
				return new AlgorithmIdentifier((DerObjectIdentifier)obj);
			}
			if (obj is string)
			{
				return new AlgorithmIdentifier((string)obj);
			}
			return new AlgorithmIdentifier(Asn1Sequence.GetInstance(obj));
		}

		public AlgorithmIdentifier(DerObjectIdentifier objectID)
		{
			this.objectID = objectID;
		}

		public AlgorithmIdentifier(string objectID)
		{
			this.objectID = new DerObjectIdentifier(objectID);
		}

		public AlgorithmIdentifier(DerObjectIdentifier objectID, Asn1Encodable parameters)
		{
			this.objectID = objectID;
			this.parameters = parameters;
			this.parametersDefined = true;
		}

		internal AlgorithmIdentifier(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.objectID = DerObjectIdentifier.GetInstance(seq[0]);
			this.parametersDefined = (seq.Count == 2);
			if (this.parametersDefined)
			{
				this.parameters = seq[1];
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.objectID
			});
			if (this.parametersDefined)
			{
				if (this.parameters != null)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						this.parameters
					});
				}
				else
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						DerNull.Instance
					});
				}
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
