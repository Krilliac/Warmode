using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class Challenge : Asn1Encodable
	{
		private readonly AlgorithmIdentifier owf;

		private readonly Asn1OctetString witness;

		private readonly Asn1OctetString challenge;

		public virtual AlgorithmIdentifier Owf
		{
			get
			{
				return this.owf;
			}
		}

		private Challenge(Asn1Sequence seq)
		{
			int index = 0;
			if (seq.Count == 3)
			{
				this.owf = AlgorithmIdentifier.GetInstance(seq[index++]);
			}
			this.witness = Asn1OctetString.GetInstance(seq[index++]);
			this.challenge = Asn1OctetString.GetInstance(seq[index]);
		}

		public static Challenge GetInstance(object obj)
		{
			if (obj is Challenge)
			{
				return (Challenge)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Challenge((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.owf
			});
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.witness
			});
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.challenge
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
