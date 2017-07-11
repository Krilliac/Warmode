using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class MacData : Asn1Encodable
	{
		internal DigestInfo digInfo;

		internal byte[] salt;

		internal BigInteger iterationCount;

		public DigestInfo Mac
		{
			get
			{
				return this.digInfo;
			}
		}

		public BigInteger IterationCount
		{
			get
			{
				return this.iterationCount;
			}
		}

		public static MacData GetInstance(object obj)
		{
			if (obj is MacData)
			{
				return (MacData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new MacData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		private MacData(Asn1Sequence seq)
		{
			this.digInfo = DigestInfo.GetInstance(seq[0]);
			this.salt = ((Asn1OctetString)seq[1]).GetOctets();
			if (seq.Count == 3)
			{
				this.iterationCount = ((DerInteger)seq[2]).Value;
				return;
			}
			this.iterationCount = BigInteger.One;
		}

		public MacData(DigestInfo digInfo, byte[] salt, int iterationCount)
		{
			this.digInfo = digInfo;
			this.salt = (byte[])salt.Clone();
			this.iterationCount = BigInteger.ValueOf((long)iterationCount);
		}

		public byte[] GetSalt()
		{
			return (byte[])this.salt.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.digInfo,
				new DerOctetString(this.salt)
			});
			if (!this.iterationCount.Equals(BigInteger.One))
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerInteger(this.iterationCount)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
