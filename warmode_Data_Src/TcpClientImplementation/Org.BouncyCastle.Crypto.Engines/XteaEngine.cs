using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class XteaEngine : IBlockCipher
	{
		private const int rounds = 32;

		private const int block_size = 8;

		private const int delta = -1640531527;

		private uint[] _S = new uint[4];

		private uint[] _sum0 = new uint[32];

		private uint[] _sum1 = new uint[32];

		private bool _initialised;

		private bool _forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "XTEA";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public XteaEngine()
		{
			this._initialised = false;
		}

		public virtual int GetBlockSize()
		{
			return 8;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to TEA init - " + parameters.GetType().FullName);
			}
			this._forEncryption = forEncryption;
			this._initialised = true;
			KeyParameter keyParameter = (KeyParameter)parameters;
			this.setKey(keyParameter.GetKey());
		}

		public virtual int ProcessBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
		{
			if (!this._initialised)
			{
				throw new InvalidOperationException(this.AlgorithmName + " not initialised");
			}
			Check.DataLength(inBytes, inOff, 8, "input buffer too short");
			Check.OutputLength(outBytes, outOff, 8, "output buffer too short");
			if (!this._forEncryption)
			{
				return this.decryptBlock(inBytes, inOff, outBytes, outOff);
			}
			return this.encryptBlock(inBytes, inOff, outBytes, outOff);
		}

		public virtual void Reset()
		{
		}

		private void setKey(byte[] key)
		{
			int i;
			int num = i = 0;
			while (i < 4)
			{
				this._S[i] = Pack.BE_To_UInt32(key, num);
				i++;
				num += 4;
			}
			num = (i = 0);
			while (i < 32)
			{
				this._sum0[i] = (uint)(num + (int)this._S[num & 3]);
				num += -1640531527;
				this._sum1[i] = (uint)(num + (int)this._S[num >> 11 & 3]);
				i++;
			}
		}

		private int encryptBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
		{
			uint num = Pack.BE_To_UInt32(inBytes, inOff);
			uint num2 = Pack.BE_To_UInt32(inBytes, inOff + 4);
			for (int i = 0; i < 32; i++)
			{
				num += ((num2 << 4 ^ num2 >> 5) + num2 ^ this._sum0[i]);
				num2 += ((num << 4 ^ num >> 5) + num ^ this._sum1[i]);
			}
			Pack.UInt32_To_BE(num, outBytes, outOff);
			Pack.UInt32_To_BE(num2, outBytes, outOff + 4);
			return 8;
		}

		private int decryptBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
		{
			uint num = Pack.BE_To_UInt32(inBytes, inOff);
			uint num2 = Pack.BE_To_UInt32(inBytes, inOff + 4);
			for (int i = 31; i >= 0; i--)
			{
				num2 -= ((num << 4 ^ num >> 5) + num ^ this._sum1[i]);
				num -= ((num2 << 4 ^ num2 >> 5) + num2 ^ this._sum0[i]);
			}
			Pack.UInt32_To_BE(num, outBytes, outOff);
			Pack.UInt32_To_BE(num2, outBytes, outOff + 4);
			return 8;
		}
	}
}
