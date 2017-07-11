using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Macs
{
	public class GMac : IMac
	{
		private readonly GcmBlockCipher cipher;

		private readonly int macSizeBits;

		public string AlgorithmName
		{
			get
			{
				return this.cipher.GetUnderlyingCipher().AlgorithmName + "-GMAC";
			}
		}

		public GMac(GcmBlockCipher cipher) : this(cipher, 128)
		{
		}

		public GMac(GcmBlockCipher cipher, int macSizeBits)
		{
			this.cipher = cipher;
			this.macSizeBits = macSizeBits;
		}

		public void Init(ICipherParameters parameters)
		{
			if (parameters is ParametersWithIV)
			{
				ParametersWithIV parametersWithIV = (ParametersWithIV)parameters;
				byte[] iV = parametersWithIV.GetIV();
				KeyParameter key = (KeyParameter)parametersWithIV.Parameters;
				this.cipher.Init(true, new AeadParameters(key, this.macSizeBits, iV));
				return;
			}
			throw new ArgumentException("GMAC requires ParametersWithIV");
		}

		public int GetMacSize()
		{
			return this.macSizeBits / 8;
		}

		public void Update(byte input)
		{
			this.cipher.ProcessAadByte(input);
		}

		public void BlockUpdate(byte[] input, int inOff, int len)
		{
			this.cipher.ProcessAadBytes(input, inOff, len);
		}

		public int DoFinal(byte[] output, int outOff)
		{
			int result;
			try
			{
				result = this.cipher.DoFinal(output, outOff);
			}
			catch (InvalidCipherTextException ex)
			{
				throw new InvalidOperationException(ex.ToString());
			}
			return result;
		}

		public void Reset()
		{
			this.cipher.Reset();
		}
	}
}
