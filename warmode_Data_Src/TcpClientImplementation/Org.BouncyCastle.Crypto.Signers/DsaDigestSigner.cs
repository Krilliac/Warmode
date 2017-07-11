using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class DsaDigestSigner : ISigner
	{
		private readonly IDigest digest;

		private readonly IDsa dsaSigner;

		private bool forSigning;

		public virtual string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "with" + this.dsaSigner.AlgorithmName;
			}
		}

		public DsaDigestSigner(IDsa signer, IDigest digest)
		{
			this.digest = digest;
			this.dsaSigner = signer;
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			this.forSigning = forSigning;
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				asymmetricKeyParameter = (AsymmetricKeyParameter)((ParametersWithRandom)parameters).Parameters;
			}
			else
			{
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			if (forSigning && !asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Signing Requires Private Key.");
			}
			if (!forSigning && asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Verification Requires Public Key.");
			}
			this.Reset();
			this.dsaSigner.Init(forSigning, parameters);
		}

		public virtual void Update(byte input)
		{
			this.digest.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int length)
		{
			this.digest.BlockUpdate(input, inOff, length);
		}

		public virtual byte[] GenerateSignature()
		{
			if (!this.forSigning)
			{
				throw new InvalidOperationException("DSADigestSigner not initialised for signature generation.");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			BigInteger[] array2 = this.dsaSigner.GenerateSignature(array);
			return this.DerEncode(array2[0], array2[1]);
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			if (this.forSigning)
			{
				throw new InvalidOperationException("DSADigestSigner not initialised for verification");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			bool result;
			try
			{
				BigInteger[] array2 = this.DerDecode(signature);
				result = this.dsaSigner.VerifySignature(array, array2[0], array2[1]);
			}
			catch (IOException)
			{
				result = false;
			}
			return result;
		}

		public virtual void Reset()
		{
			this.digest.Reset();
		}

		private byte[] DerEncode(BigInteger r, BigInteger s)
		{
			return new DerSequence(new Asn1Encodable[]
			{
				new DerInteger(r),
				new DerInteger(s)
			}).GetDerEncoded();
		}

		private BigInteger[] DerDecode(byte[] encoding)
		{
			Asn1Sequence asn1Sequence = (Asn1Sequence)Asn1Object.FromByteArray(encoding);
			return new BigInteger[]
			{
				((DerInteger)asn1Sequence[0]).Value,
				((DerInteger)asn1Sequence[1]).Value
			};
		}
	}
}
