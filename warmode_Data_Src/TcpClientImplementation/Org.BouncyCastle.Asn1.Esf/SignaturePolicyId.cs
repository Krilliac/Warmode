using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class SignaturePolicyId : Asn1Encodable
	{
		private readonly DerObjectIdentifier sigPolicyIdentifier;

		private readonly OtherHashAlgAndValue sigPolicyHash;

		private readonly Asn1Sequence sigPolicyQualifiers;

		public DerObjectIdentifier SigPolicyIdentifier
		{
			get
			{
				return this.sigPolicyIdentifier;
			}
		}

		public OtherHashAlgAndValue SigPolicyHash
		{
			get
			{
				return this.sigPolicyHash;
			}
		}

		public static SignaturePolicyId GetInstance(object obj)
		{
			if (obj == null || obj is SignaturePolicyId)
			{
				return (SignaturePolicyId)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SignaturePolicyId((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in 'SignaturePolicyId' factory: " + obj.GetType().Name, "obj");
		}

		private SignaturePolicyId(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count < 2 || seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			this.sigPolicyIdentifier = (DerObjectIdentifier)seq[0].ToAsn1Object();
			this.sigPolicyHash = OtherHashAlgAndValue.GetInstance(seq[1].ToAsn1Object());
			if (seq.Count > 2)
			{
				this.sigPolicyQualifiers = (Asn1Sequence)seq[2].ToAsn1Object();
			}
		}

		public SignaturePolicyId(DerObjectIdentifier sigPolicyIdentifier, OtherHashAlgAndValue sigPolicyHash) : this(sigPolicyIdentifier, sigPolicyHash, null)
		{
		}

		public SignaturePolicyId(DerObjectIdentifier sigPolicyIdentifier, OtherHashAlgAndValue sigPolicyHash, params SigPolicyQualifierInfo[] sigPolicyQualifiers)
		{
			if (sigPolicyIdentifier == null)
			{
				throw new ArgumentNullException("sigPolicyIdentifier");
			}
			if (sigPolicyHash == null)
			{
				throw new ArgumentNullException("sigPolicyHash");
			}
			this.sigPolicyIdentifier = sigPolicyIdentifier;
			this.sigPolicyHash = sigPolicyHash;
			if (sigPolicyQualifiers != null)
			{
				this.sigPolicyQualifiers = new DerSequence(sigPolicyQualifiers);
			}
		}

		public SignaturePolicyId(DerObjectIdentifier sigPolicyIdentifier, OtherHashAlgAndValue sigPolicyHash, IEnumerable sigPolicyQualifiers)
		{
			if (sigPolicyIdentifier == null)
			{
				throw new ArgumentNullException("sigPolicyIdentifier");
			}
			if (sigPolicyHash == null)
			{
				throw new ArgumentNullException("sigPolicyHash");
			}
			this.sigPolicyIdentifier = sigPolicyIdentifier;
			this.sigPolicyHash = sigPolicyHash;
			if (sigPolicyQualifiers != null)
			{
				if (!CollectionUtilities.CheckElementsAreOfType(sigPolicyQualifiers, typeof(SigPolicyQualifierInfo)))
				{
					throw new ArgumentException("Must contain only 'SigPolicyQualifierInfo' objects", "sigPolicyQualifiers");
				}
				this.sigPolicyQualifiers = new DerSequence(Asn1EncodableVector.FromEnumerable(sigPolicyQualifiers));
			}
		}

		public SigPolicyQualifierInfo[] GetSigPolicyQualifiers()
		{
			if (this.sigPolicyQualifiers == null)
			{
				return null;
			}
			SigPolicyQualifierInfo[] array = new SigPolicyQualifierInfo[this.sigPolicyQualifiers.Count];
			for (int i = 0; i < this.sigPolicyQualifiers.Count; i++)
			{
				array[i] = SigPolicyQualifierInfo.GetInstance(this.sigPolicyQualifiers[i]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.sigPolicyIdentifier,
				this.sigPolicyHash.ToAsn1Object()
			});
			if (this.sigPolicyQualifiers != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.sigPolicyQualifiers.ToAsn1Object()
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
