using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class NoekeonEngine : IBlockCipher
	{
		private const int GenericSize = 16;

		private static readonly uint[] nullVector;

		private static readonly uint[] roundConstants;

		private uint[] state = new uint[4];

		private uint[] subKeys = new uint[4];

		private uint[] decryptKeys = new uint[4];

		private bool _initialised;

		private bool _forEncryption;

		public virtual string AlgorithmName
		{
			get
			{
				return "Noekeon";
			}
		}

		public virtual bool IsPartialBlockOkay
		{
			get
			{
				return false;
			}
		}

		public NoekeonEngine()
		{
			this._initialised = false;
		}

		public virtual int GetBlockSize()
		{
			return 16;
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("Invalid parameters passed to Noekeon init - " + parameters.GetType().Name, "parameters");
			}
			this._forEncryption = forEncryption;
			this._initialised = true;
			KeyParameter keyParameter = (KeyParameter)parameters;
			this.setKey(keyParameter.GetKey());
		}

		public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (!this._initialised)
			{
				throw new InvalidOperationException(this.AlgorithmName + " not initialised");
			}
			Check.DataLength(input, inOff, 16, "input buffer too short");
			Check.OutputLength(output, outOff, 16, "output buffer too short");
			if (!this._forEncryption)
			{
				return this.decryptBlock(input, inOff, output, outOff);
			}
			return this.encryptBlock(input, inOff, output, outOff);
		}

		public virtual void Reset()
		{
		}

		private void setKey(byte[] key)
		{
			this.subKeys[0] = Pack.BE_To_UInt32(key, 0);
			this.subKeys[1] = Pack.BE_To_UInt32(key, 4);
			this.subKeys[2] = Pack.BE_To_UInt32(key, 8);
			this.subKeys[3] = Pack.BE_To_UInt32(key, 12);
		}

		private int encryptBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			this.state[0] = Pack.BE_To_UInt32(input, inOff);
			this.state[1] = Pack.BE_To_UInt32(input, inOff + 4);
			this.state[2] = Pack.BE_To_UInt32(input, inOff + 8);
			this.state[3] = Pack.BE_To_UInt32(input, inOff + 12);
			int i;
			for (i = 0; i < 16; i++)
			{
				this.state[0] ^= NoekeonEngine.roundConstants[i];
				this.theta(this.state, this.subKeys);
				this.pi1(this.state);
				this.gamma(this.state);
				this.pi2(this.state);
			}
			this.state[0] ^= NoekeonEngine.roundConstants[i];
			this.theta(this.state, this.subKeys);
			Pack.UInt32_To_BE(this.state[0], output, outOff);
			Pack.UInt32_To_BE(this.state[1], output, outOff + 4);
			Pack.UInt32_To_BE(this.state[2], output, outOff + 8);
			Pack.UInt32_To_BE(this.state[3], output, outOff + 12);
			return 16;
		}

		private int decryptBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			this.state[0] = Pack.BE_To_UInt32(input, inOff);
			this.state[1] = Pack.BE_To_UInt32(input, inOff + 4);
			this.state[2] = Pack.BE_To_UInt32(input, inOff + 8);
			this.state[3] = Pack.BE_To_UInt32(input, inOff + 12);
			Array.Copy(this.subKeys, 0, this.decryptKeys, 0, this.subKeys.Length);
			this.theta(this.decryptKeys, NoekeonEngine.nullVector);
			int i;
			for (i = 16; i > 0; i--)
			{
				this.theta(this.state, this.decryptKeys);
				this.state[0] ^= NoekeonEngine.roundConstants[i];
				this.pi1(this.state);
				this.gamma(this.state);
				this.pi2(this.state);
			}
			this.theta(this.state, this.decryptKeys);
			this.state[0] ^= NoekeonEngine.roundConstants[i];
			Pack.UInt32_To_BE(this.state[0], output, outOff);
			Pack.UInt32_To_BE(this.state[1], output, outOff + 4);
			Pack.UInt32_To_BE(this.state[2], output, outOff + 8);
			Pack.UInt32_To_BE(this.state[3], output, outOff + 12);
			return 16;
		}

		private void gamma(uint[] a)
		{
			a[1] ^= (~a[3] & ~a[2]);
			a[0] ^= (a[2] & a[1]);
			uint num = a[3];
			a[3] = a[0];
			a[0] = num;
			a[2] ^= (a[0] ^ a[1] ^ a[3]);
			a[1] ^= (~a[3] & ~a[2]);
			a[0] ^= (a[2] & a[1]);
		}

		private void theta(uint[] a, uint[] k)
		{
			uint num = a[0] ^ a[2];
			num ^= (this.rotl(num, 8) ^ this.rotl(num, 24));
			a[1] ^= num;
			a[3] ^= num;
			for (int i = 0; i < 4; i++)
			{
				a[i] ^= k[i];
			}
			num = (a[1] ^ a[3]);
			num ^= (this.rotl(num, 8) ^ this.rotl(num, 24));
			a[0] ^= num;
			a[2] ^= num;
		}

		private void pi1(uint[] a)
		{
			a[1] = this.rotl(a[1], 1);
			a[2] = this.rotl(a[2], 5);
			a[3] = this.rotl(a[3], 2);
		}

		private void pi2(uint[] a)
		{
			a[1] = this.rotl(a[1], 31);
			a[2] = this.rotl(a[2], 27);
			a[3] = this.rotl(a[3], 30);
		}

		private uint rotl(uint x, int y)
		{
			return x << y | x >> 32 - y;
		}

		static NoekeonEngine()
		{
			// Note: this type is marked as 'beforefieldinit'.
			uint[] array = new uint[4];
			NoekeonEngine.nullVector = array;
			NoekeonEngine.roundConstants = new uint[]
			{
				128u,
				27u,
				54u,
				108u,
				216u,
				171u,
				77u,
				154u,
				47u,
				94u,
				188u,
				99u,
				198u,
				151u,
				53u,
				106u,
				212u
			};
		}
	}
}
