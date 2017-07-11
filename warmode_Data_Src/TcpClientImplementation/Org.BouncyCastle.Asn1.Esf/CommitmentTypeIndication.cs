using System;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class CommitmentTypeIndication : Asn1Encodable
	{
		private readonly DerObjectIdentifier commitmentTypeId;

		private readonly Asn1Sequence commitmentTypeQualifier;

		public DerObjectIdentifier CommitmentTypeID
		{
			get
			{
				return this.commitmentTypeId;
			}
		}

		public Asn1Sequence CommitmentTypeQualifier
		{
			get
			{
				return this.commitmentTypeQualifier;
			}
		}

		public static CommitmentTypeIndication GetInstance(object obj)
		{
			if (obj == null || obj is CommitmentTypeIndication)
			{
				return (CommitmentTypeIndication)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CommitmentTypeIndication((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'CommitmentTypeIndication' factory: " + obj.GetType().Name, "obj");
		}

		public CommitmentTypeIndication(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.commitmentTypeId = (DerObjectIdentifier)seq[0].ToAsn1Object();
			if (seq.Count > 1)
			{
				this.commitmentTypeQualifier = (Asn1Sequence)seq[1].ToAsn1Object();
			}
		}

		public CommitmentTypeIndication(DerObjectIdentifier commitmentTypeId) : this(commitmentTypeId, null)
		{
		}

		public CommitmentTypeIndication(DerObjectIdentifier commitmentTypeId, Asn1Sequence commitmentTypeQualifier)
		{
			if (commitmentTypeId == null)
			{
				throw new ArgumentNullException("commitmentTypeId");
			}
			this.commitmentTypeId = commitmentTypeId;
			if (commitmentTypeQualifier != null)
			{
				this.commitmentTypeQualifier = commitmentTypeQualifier;
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.commitmentTypeId
			});
			if (this.commitmentTypeQualifier != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.commitmentTypeQualifier
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
