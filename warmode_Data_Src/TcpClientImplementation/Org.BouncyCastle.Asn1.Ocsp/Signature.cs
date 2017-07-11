using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Ocsp
{
	public class Signature : Asn1Encodable
	{
		internal AlgorithmIdentifier signatureAlgorithm;

		internal DerBitString signatureValue;

		internal Asn1Sequence certs;

		public AlgorithmIdentifier SignatureAlgorithm
		{
			get
			{
				return this.signatureAlgorithm;
			}
		}

		public DerBitString SignatureValue
		{
			get
			{
				return this.signatureValue;
			}
		}

		public Asn1Sequence Certs
		{
			get
			{
				return this.certs;
			}
		}

		public static Signature GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return Signature.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static Signature GetInstance(object obj)
		{
			if (obj == null || obj is Signature)
			{
				return (Signature)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Signature((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public Signature(AlgorithmIdentifier signatureAlgorithm, DerBitString signatureValue) : this(signatureAlgorithm, signatureValue, null)
		{
		}

		public Signature(AlgorithmIdentifier signatureAlgorithm, DerBitString signatureValue, Asn1Sequence certs)
		{
			if (signatureAlgorithm == null)
			{
				throw new ArgumentException("signatureAlgorithm");
			}
			if (signatureValue == null)
			{
				throw new ArgumentException("signatureValue");
			}
			this.signatureAlgorithm = signatureAlgorithm;
			this.signatureValue = signatureValue;
			this.certs = certs;
		}

		private Signature(Asn1Sequence seq)
		{
			this.signatureAlgorithm = AlgorithmIdentifier.GetInstance(seq[0]);
			this.signatureValue = (DerBitString)seq[1];
			if (seq.Count == 3)
			{
				this.certs = Asn1Sequence.GetInstance((Asn1TaggedObject)seq[2], true);
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.signatureAlgorithm,
				this.signatureValue
			});
			if (this.certs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.certs)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
