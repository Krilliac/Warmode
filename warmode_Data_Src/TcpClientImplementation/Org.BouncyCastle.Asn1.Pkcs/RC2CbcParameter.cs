using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class RC2CbcParameter : Asn1Encodable
	{
		internal DerInteger version;

		internal Asn1OctetString iv;

		public BigInteger RC2ParameterVersion
		{
			get
			{
				if (this.version != null)
				{
					return this.version.Value;
				}
				return null;
			}
		}

		public static RC2CbcParameter GetInstance(object obj)
		{
			if (obj is Asn1Sequence)
			{
				return new RC2CbcParameter((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public RC2CbcParameter(byte[] iv)
		{
			this.iv = new DerOctetString(iv);
		}

		public RC2CbcParameter(int parameterVersion, byte[] iv)
		{
			this.version = new DerInteger(parameterVersion);
			this.iv = new DerOctetString(iv);
		}

		private RC2CbcParameter(Asn1Sequence seq)
		{
			if (seq.Count == 1)
			{
				this.iv = (Asn1OctetString)seq[0];
				return;
			}
			this.version = (DerInteger)seq[0];
			this.iv = (Asn1OctetString)seq[1];
		}

		public byte[] GetIV()
		{
			return Arrays.Clone(this.iv.GetOctets());
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.version != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.version
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.iv
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
