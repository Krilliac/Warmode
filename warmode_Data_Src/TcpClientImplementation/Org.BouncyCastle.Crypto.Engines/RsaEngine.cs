using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RsaEngine : IAsymmetricBlockCipher
	{
		private RsaCoreEngine core;

		public virtual string AlgorithmName
		{
			get
			{
				return "RSA";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (this.core == null)
			{
				this.core = new RsaCoreEngine();
			}
			this.core.Init(forEncryption, parameters);
		}

		public virtual int GetInputBlockSize()
		{
			return this.core.GetInputBlockSize();
		}

		public virtual int GetOutputBlockSize()
		{
			return this.core.GetOutputBlockSize();
		}

		public virtual byte[] ProcessBlock(byte[] inBuf, int inOff, int inLen)
		{
			if (this.core == null)
			{
				throw new InvalidOperationException("RSA engine not initialised");
			}
			return this.core.ConvertOutput(this.core.ProcessBlock(this.core.ConvertInput(inBuf, inOff, inLen)));
		}
	}
}
