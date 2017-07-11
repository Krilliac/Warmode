using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Agreement.Kdf
{
	public class DHKekGenerator : IDerivationFunction
	{
		private readonly IDigest digest;

		private DerObjectIdentifier algorithm;

		private int keySize;

		private byte[] z;

		private byte[] partyAInfo;

		public virtual IDigest Digest
		{
			get
			{
				return this.digest;
			}
		}

		public DHKekGenerator(IDigest digest)
		{
			this.digest = digest;
		}

		public virtual void Init(IDerivationParameters param)
		{
			DHKdfParameters dHKdfParameters = (DHKdfParameters)param;
			this.algorithm = dHKdfParameters.Algorithm;
			this.keySize = dHKdfParameters.KeySize;
			this.z = dHKdfParameters.GetZ();
			this.partyAInfo = dHKdfParameters.GetExtraInfo();
		}

		public virtual int GenerateBytes(byte[] outBytes, int outOff, int len)
		{
			if (outBytes.Length - len < outOff)
			{
				throw new DataLengthException("output buffer too small");
			}
			long num = (long)len;
			int digestSize = this.digest.GetDigestSize();
			if (num > 8589934591L)
			{
				throw new ArgumentException("Output length too large");
			}
			int num2 = (int)((num + (long)digestSize - 1L) / (long)digestSize);
			byte[] array = new byte[this.digest.GetDigestSize()];
			uint num3 = 1u;
			for (int i = 0; i < num2; i++)
			{
				this.digest.BlockUpdate(this.z, 0, this.z.Length);
				DerSequence derSequence = new DerSequence(new Asn1Encodable[]
				{
					this.algorithm,
					new DerOctetString(Pack.UInt32_To_BE(num3))
				});
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
				{
					derSequence
				});
				if (this.partyAInfo != null)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new DerTaggedObject(true, 0, new DerOctetString(this.partyAInfo))
					});
				}
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, new DerOctetString(Pack.UInt32_To_BE((uint)this.keySize)))
				});
				byte[] derEncoded = new DerSequence(asn1EncodableVector).GetDerEncoded();
				this.digest.BlockUpdate(derEncoded, 0, derEncoded.Length);
				this.digest.DoFinal(array, 0);
				if (len > digestSize)
				{
					Array.Copy(array, 0, outBytes, outOff, digestSize);
					outOff += digestSize;
					len -= digestSize;
				}
				else
				{
					Array.Copy(array, 0, outBytes, outOff, len);
				}
				num3 += 1u;
			}
			this.digest.Reset();
			return (int)num;
		}
	}
}
