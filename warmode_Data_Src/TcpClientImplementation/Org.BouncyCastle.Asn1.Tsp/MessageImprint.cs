using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class MessageImprint : Asn1Encodable
	{
		private readonly AlgorithmIdentifier hashAlgorithm;

		private readonly byte[] hashedMessage;

		public AlgorithmIdentifier HashAlgorithm
		{
			get
			{
				return this.hashAlgorithm;
			}
		}

		public static MessageImprint GetInstance(object o)
		{
			if (o == null || o is MessageImprint)
			{
				return (MessageImprint)o;
			}
			if (o is Asn1Sequence)
			{
				return new MessageImprint((Asn1Sequence)o);
			}
			throw new ArgumentException("Unknown object in 'MessageImprint' factory: " + o.GetType().FullName);
		}

		private MessageImprint(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			this.hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[0]);
			this.hashedMessage = Asn1OctetString.GetInstance(seq[1]).GetOctets();
		}

		public MessageImprint(AlgorithmIdentifier hashAlgorithm, byte[] hashedMessage)
		{
			this.hashAlgorithm = hashAlgorithm;
			this.hashedMessage = hashedMessage;
		}

		public byte[] GetHashedMessage()
		{
			return this.hashedMessage;
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.hashAlgorithm,
				new DerOctetString(this.hashedMessage)
			});
		}
	}
}
