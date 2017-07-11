using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Agreement.Kdf
{
	public class ECDHKekGenerator : IDerivationFunction
	{
		private readonly IDerivationFunction kdf;

		private DerObjectIdentifier algorithm;

		private int keySize;

		private byte[] z;

		public virtual IDigest Digest
		{
			get
			{
				return this.kdf.Digest;
			}
		}

		public ECDHKekGenerator(IDigest digest)
		{
			this.kdf = new Kdf2BytesGenerator(digest);
		}

		public virtual void Init(IDerivationParameters param)
		{
			DHKdfParameters dHKdfParameters = (DHKdfParameters)param;
			this.algorithm = dHKdfParameters.Algorithm;
			this.keySize = dHKdfParameters.KeySize;
			this.z = dHKdfParameters.GetZ();
		}

		public virtual int GenerateBytes(byte[] outBytes, int outOff, int len)
		{
			DerSequence derSequence = new DerSequence(new Asn1Encodable[]
			{
				new AlgorithmIdentifier(this.algorithm, DerNull.Instance),
				new DerTaggedObject(true, 2, new DerOctetString(Pack.UInt32_To_BE((uint)this.keySize)))
			});
			this.kdf.Init(new KdfParameters(this.z, derSequence.GetDerEncoded()));
			return this.kdf.GenerateBytes(outBytes, outOff, len);
		}
	}
}
