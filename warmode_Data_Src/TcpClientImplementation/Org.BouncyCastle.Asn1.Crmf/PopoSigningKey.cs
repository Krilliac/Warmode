using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class PopoSigningKey : Asn1Encodable
	{
		private readonly PopoSigningKeyInput poposkInput;

		private readonly AlgorithmIdentifier algorithmIdentifier;

		private readonly DerBitString signature;

		public virtual PopoSigningKeyInput PoposkInput
		{
			get
			{
				return this.poposkInput;
			}
		}

		public virtual AlgorithmIdentifier AlgorithmIdentifier
		{
			get
			{
				return this.algorithmIdentifier;
			}
		}

		public virtual DerBitString Signature
		{
			get
			{
				return this.signature;
			}
		}

		private PopoSigningKey(Asn1Sequence seq)
		{
			int index = 0;
			if (seq[index] is Asn1TaggedObject)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[index++];
				if (asn1TaggedObject.TagNo != 0)
				{
					throw new ArgumentException("Unknown PopoSigningKeyInput tag: " + asn1TaggedObject.TagNo, "seq");
				}
				this.poposkInput = PopoSigningKeyInput.GetInstance(asn1TaggedObject.GetObject());
			}
			this.algorithmIdentifier = AlgorithmIdentifier.GetInstance(seq[index++]);
			this.signature = DerBitString.GetInstance(seq[index]);
		}

		public static PopoSigningKey GetInstance(object obj)
		{
			if (obj is PopoSigningKey)
			{
				return (PopoSigningKey)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PopoSigningKey((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public static PopoSigningKey GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return PopoSigningKey.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public PopoSigningKey(PopoSigningKeyInput poposkIn, AlgorithmIdentifier aid, DerBitString signature)
		{
			this.poposkInput = poposkIn;
			this.algorithmIdentifier = aid;
			this.signature = signature;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.poposkInput != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.poposkInput)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.algorithmIdentifier
			});
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.signature
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
