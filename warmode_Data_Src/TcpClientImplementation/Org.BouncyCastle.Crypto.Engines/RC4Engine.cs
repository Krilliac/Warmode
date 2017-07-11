using Org.BouncyCastle.Crypto.Parameters;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class RC4Engine : IStreamCipher
	{
		private static readonly int STATE_LENGTH = 256;

		private byte[] engineState;

		private int x;

		private int y;

		private byte[] workingKey;

		public virtual string AlgorithmName
		{
			get
			{
				return "RC4";
			}
		}

		public virtual void Init(bool forEncryption, ICipherParameters parameters)
		{
			if (parameters is KeyParameter)
			{
				this.workingKey = ((KeyParameter)parameters).GetKey();
				this.SetKey(this.workingKey);
				return;
			}
			throw new ArgumentException("invalid parameter passed to RC4 init - " + parameters.GetType().ToString());
		}

		public virtual byte ReturnByte(byte input)
		{
			this.x = (this.x + 1 & 255);
			this.y = ((int)this.engineState[this.x] + this.y & 255);
			byte b = this.engineState[this.x];
			this.engineState[this.x] = this.engineState[this.y];
			this.engineState[this.y] = b;
			return input ^ this.engineState[(int)(this.engineState[this.x] + this.engineState[this.y] & 255)];
		}

		public virtual void ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
		{
			Check.DataLength(input, inOff, length, "input buffer too short");
			Check.OutputLength(output, outOff, length, "output buffer too short");
			for (int i = 0; i < length; i++)
			{
				this.x = (this.x + 1 & 255);
				this.y = ((int)this.engineState[this.x] + this.y & 255);
				byte b = this.engineState[this.x];
				this.engineState[this.x] = this.engineState[this.y];
				this.engineState[this.y] = b;
				output[i + outOff] = (input[i + inOff] ^ this.engineState[(int)(this.engineState[this.x] + this.engineState[this.y] & 255)]);
			}
		}

		public virtual void Reset()
		{
			this.SetKey(this.workingKey);
		}

		private void SetKey(byte[] keyBytes)
		{
			this.workingKey = keyBytes;
			this.x = 0;
			this.y = 0;
			if (this.engineState == null)
			{
				this.engineState = new byte[RC4Engine.STATE_LENGTH];
			}
			for (int i = 0; i < RC4Engine.STATE_LENGTH; i++)
			{
				this.engineState[i] = (byte)i;
			}
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < RC4Engine.STATE_LENGTH; j++)
			{
				num2 = ((int)((keyBytes[num] & 255) + this.engineState[j]) + num2 & 255);
				byte b = this.engineState[j];
				this.engineState[j] = this.engineState[num2];
				this.engineState[num2] = b;
				num = (num + 1) % keyBytes.Length;
			}
		}
	}
}
