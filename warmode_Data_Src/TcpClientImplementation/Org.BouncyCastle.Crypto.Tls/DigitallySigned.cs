using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class DigitallySigned
	{
		protected readonly SignatureAndHashAlgorithm mAlgorithm;

		protected readonly byte[] mSignature;

		public virtual SignatureAndHashAlgorithm Algorithm
		{
			get
			{
				return this.mAlgorithm;
			}
		}

		public virtual byte[] Signature
		{
			get
			{
				return this.mSignature;
			}
		}

		public DigitallySigned(SignatureAndHashAlgorithm algorithm, byte[] signature)
		{
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			this.mAlgorithm = algorithm;
			this.mSignature = signature;
		}

		public virtual void Encode(Stream output)
		{
			if (this.mAlgorithm != null)
			{
				this.mAlgorithm.Encode(output);
			}
			TlsUtilities.WriteOpaque16(this.mSignature, output);
		}

		public static DigitallySigned Parse(TlsContext context, Stream input)
		{
			SignatureAndHashAlgorithm algorithm = null;
			if (TlsUtilities.IsTlsV12(context))
			{
				algorithm = SignatureAndHashAlgorithm.Parse(input);
			}
			byte[] signature = TlsUtilities.ReadOpaque16(input);
			return new DigitallySigned(algorithm, signature);
		}
	}
}
