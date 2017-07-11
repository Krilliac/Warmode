using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class PbeParameter : Asn1Encodable
	{
		private readonly Asn1OctetString salt;

		private readonly DerInteger iterationCount;

		public BigInteger IterationCount
		{
			get
			{
				return this.iterationCount.Value;
			}
		}

		public static PbeParameter GetInstance(object obj)
		{
			if (obj is PbeParameter || obj == null)
			{
				return (PbeParameter)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PbeParameter((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		private PbeParameter(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			this.salt = Asn1OctetString.GetInstance(seq[0]);
			this.iterationCount = DerInteger.GetInstance(seq[1]);
		}

		public PbeParameter(byte[] salt, int iterationCount)
		{
			this.salt = new DerOctetString(salt);
			this.iterationCount = new DerInteger(iterationCount);
		}

		public byte[] GetSalt()
		{
			return this.salt.GetOctets();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.salt,
				this.iterationCount
			});
		}
	}
}
