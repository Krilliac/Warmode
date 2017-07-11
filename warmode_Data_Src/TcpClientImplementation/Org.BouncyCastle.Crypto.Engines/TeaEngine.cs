using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class TeaEngine : IBlockCipher
	{
		private const int rounds = 32;

		private const int block_size = 8;

		private const uint delta = 2654435769u;

		private const uint d_sum = 3337565984u;

		private uint _a;

		private uint _b;

		private uint _c;

		private uint _d;

		private bool _initialised;

		private bool _forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "TEA";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public TeaEngine()
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
			this._a = Pack.BE_To_UInt32(key, 0);
			this._b = Pack.BE_To_UInt32(key, 4);
			this._c = Pack.BE_To_UInt32(key, 8);
			this._d = Pack.BE_To_UInt32(key, 12);
		}

		private int encryptBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
		{
			uint num = Pack.BE_To_UInt32(inBytes, inOff);
			uint num2 = Pack.BE_To_UInt32(inBytes, inOff + 4);
			uint num3 = 0u;
			for (int num4 = 0; num4 != 32; num4++)
			{
				num3 += 2654435769u;
				num += ((num2 << 4) + this._a ^ num2 + num3 ^ (num2 >> 5) + this._b);
				num2 += ((num << 4) + this._c ^ num + num3 ^ (num >> 5) + this._d);
			}
			Pack.UInt32_To_BE(num, outBytes, outOff);
			Pack.UInt32_To_BE(num2, outBytes, outOff + 4);
			return 8;
		}

		private int decryptBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
		{
			uint num = Pack.BE_To_UInt32(inBytes, inOff);
			uint num2 = Pack.BE_To_UInt32(inBytes, inOff + 4);
			uint num3 = 3337565984u;
			for (int num4 = 0; num4 != 32; num4++)
			{
				num2 -= ((num << 4) + this._c ^ num + num3 ^ (num >> 5) + this._d);
				num -= ((num2 << 4) + this._a ^ num2 + num3 ^ (num2 >> 5) + this._b);
				num3 -= 2654435769u;
			}
			Pack.UInt32_To_BE(num, outBytes, outOff);
			Pack.UInt32_To_BE(num2, outBytes, outOff + 4);
			return 8;
		}
	}
}
