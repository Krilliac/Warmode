using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class Gost3410DigestSigner : ISigner
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

		public Gost3410DigestSigner(IDsa signer, IDigest digest)
		{
			this.dsaSigner = signer;
			this.digest = digest;
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
				throw new InvalidOperationException("GOST3410DigestSigner not initialised for signature generation.");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			byte[] result;
			try
			{
				BigInteger[] array2 = this.dsaSigner.GenerateSignature(array);
				byte[] array3 = new byte[64];
				byte[] array4 = array2[0].ToByteArrayUnsigned();
				byte[] array5 = array2[1].ToByteArrayUnsigned();
				array5.CopyTo(array3, 32 - array5.Length);
				array4.CopyTo(array3, 64 - array4.Length);
				result = array3;
			}
			catch (Exception ex)
			{
				throw new SignatureException(ex.Message, ex);
			}
			return result;
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			if (this.forSigning)
			{
				throw new InvalidOperationException("DSADigestSigner not initialised for verification");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			BigInteger r;
			BigInteger s;
			try
			{
				r = new BigInteger(1, signature, 32, 32);
				s = new BigInteger(1, signature, 0, 32);
			}
			catch (Exception exception)
			{
				throw new SignatureException("error decoding signature bytes.", exception);
			}
			return this.dsaSigner.VerifySignature(array, r, s);
		}

		public virtual void Reset()
		{
			this.digest.Reset();
		}
	}
}
