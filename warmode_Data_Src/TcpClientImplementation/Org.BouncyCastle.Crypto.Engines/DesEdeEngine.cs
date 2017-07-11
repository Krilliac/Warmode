using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class DesEdeEngine : DesEngine
	{
		private int[] workingKey1;

		private int[] workingKey2;

		private int[] workingKey3;

		private bool forEncryption;

		public override string AlgorithmName
		{
			get
			{
				return "DESede";
			}
		}

		public override void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to DESede init - " + parameters.GetType().ToString());
			}
			byte[] key = ((KeyParameter)parameters).GetKey();
			if (key.Length != 24 && key.Length != 16)
			{
				throw new ArgumentException("key size must be 16 or 24 bytes.");
			}
			this.forEncryption = forEncryption;
			byte[] array = new byte[8];
			Array.Copy(key, 0, array, 0, array.Length);
			this.workingKey1 = DesEngine.GenerateWorkingKey(forEncryption, array);
			byte[] array2 = new byte[8];
			Array.Copy(key, 8, array2, 0, array2.Length);
			this.workingKey2 = DesEngine.GenerateWorkingKey(!forEncryption, array2);
			if (key.Length == 24)
			{
				byte[] array3 = new byte[8];
				Array.Copy(key, 16, array3, 0, array3.Length);
				this.workingKey3 = DesEngine.GenerateWorkingKey(forEncryption, array3);
				return;
			}
			this.workingKey3 = this.workingKey1;
		}

		public override int GetBlockSize()
		{
			return 8;
		}

		public override int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
		{
			if (this.workingKey1 == null)
			{
				throw new InvalidOperationException("DESede engine not initialised");
			}
			Check.DataLength(input, inOff, 8, "input buffer too short");
			Check.OutputLength(output, outOff, 8, "output buffer too short");
			byte[] array = new byte[8];
			if (this.forEncryption)
			{
				DesEngine.DesFunc(this.workingKey1, input, inOff, array, 0);
				DesEngine.DesFunc(this.workingKey2, array, 0, array, 0);
				DesEngine.DesFunc(this.workingKey3, array, 0, output, outOff);
			}
			else
			{
				DesEngine.DesFunc(this.workingKey3, input, inOff, array, 0);
				DesEngine.DesFunc(this.workingKey2, array, 0, array, 0);
				DesEngine.DesFunc(this.workingKey1, array, 0, output, outOff);
			}
			return 8;
		}

		public override void Reset()
		{
		}
	}
}
