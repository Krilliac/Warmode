using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class Ssl3Mac : IMac
	{
		private const byte IPAD_BYTE = 54;

		private const byte OPAD_BYTE = 92;

		internal static readonly byte[] IPAD = Ssl3Mac.GenPad(54, 48);

		internal static readonly byte[] OPAD = Ssl3Mac.GenPad(92, 48);

		private readonly IDigest digest;

		private readonly int padLength;

		private byte[] secret;

		public virtual string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "/SSL3MAC";
			}
		}

		public Ssl3Mac(IDigest digest)
		{
			this.digest = digest;
			if (digest.GetDigestSize() == 20)
			{
				this.padLength = 40;
				return;
			}
			this.padLength = 48;
		}

		public virtual void Init(ICipherParameters parameters)
		{
			this.secret = Arrays.Clone(((KeyParameter)parameters).GetKey());
			this.Reset();
		}

		public virtual int GetMacSize()
		{
			return this.digest.GetDigestSize();
		}

		public virtual void Update(byte input)
		{
			this.digest.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int len)
		{
			this.digest.BlockUpdate(input, inOff, len);
		}

		public virtual int DoFinal(byte[] output, int outOff)
		{
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			this.digest.BlockUpdate(this.secret, 0, this.secret.Length);
			this.digest.BlockUpdate(Ssl3Mac.OPAD, 0, this.padLength);
			this.digest.BlockUpdate(array, 0, array.Length);
			int result = this.digest.DoFinal(output, outOff);
			this.Reset();
			return result;
		}

		public virtual void Reset()
		{
			this.digest.Reset();
			this.digest.BlockUpdate(this.secret, 0, this.secret.Length);
			this.digest.BlockUpdate(Ssl3Mac.IPAD, 0, this.padLength);
		}

		private static byte[] GenPad(byte b, int count)
		{
			byte[] array = new byte[count];
			Arrays.Fill(array, b);
			return array;
		}
	}
}
