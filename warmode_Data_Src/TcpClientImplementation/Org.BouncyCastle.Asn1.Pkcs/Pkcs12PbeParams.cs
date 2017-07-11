using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class Pkcs12PbeParams : Asn1Encodable
	{
		private readonly DerInteger iterations;

		private readonly Asn1OctetString iv;

		public BigInteger Iterations
		{
			get
			{
				return this.iterations.Value;
			}
		}

		public Pkcs12PbeParams(byte[] salt, int iterations)
		{
			this.iv = new DerOctetString(salt);
			this.iterations = new DerInteger(iterations);
		}

		private Pkcs12PbeParams(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			this.iv = Asn1OctetString.GetInstance(seq[0]);
			this.iterations = DerInteger.GetInstance(seq[1]);
		}

		public static Pkcs12PbeParams GetInstance(object obj)
		{
			if (obj is Pkcs12PbeParams)
			{
				return (Pkcs12PbeParams)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Pkcs12PbeParams((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public byte[] GetIV()
		{
			return this.iv.GetOctets();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(new Asn1Encodable[]
			{
				this.iv,
				this.iterations
			});
		}
	}
}
