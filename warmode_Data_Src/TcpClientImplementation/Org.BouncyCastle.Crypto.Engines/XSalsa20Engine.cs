using Org.BouncyCastle.Crypto.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Engines
{
	public class XSalsa20Engine : Salsa20Engine
	{
		public override string AlgorithmName
		{
			get
			{
				return "XSalsa20";
			}
		}

		protected override int NonceSize
		{
			get
			{
				return 24;
			}
		}

		protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
		{
			if (keyBytes.Length != 32)
			{
				throw new ArgumentException(this.AlgorithmName + " requires a 256 bit key");
			}
			base.SetKey(keyBytes, ivBytes);
			this.engineState[8] = Pack.LE_To_UInt32(ivBytes, 8);
			this.engineState[9] = Pack.LE_To_UInt32(ivBytes, 12);
			uint[] array = new uint[this.engineState.Length];
			Salsa20Engine.SalsaCore(20, this.engineState, array);
			this.engineState[1] = array[0] - this.engineState[0];
			this.engineState[2] = array[5] - this.engineState[5];
			this.engineState[3] = array[10] - this.engineState[10];
			this.engineState[4] = array[15] - this.engineState[15];
			this.engineState[11] = array[6] - this.engineState[6];
			this.engineState[12] = array[7] - this.engineState[7];
			this.engineState[13] = array[8] - this.engineState[8];
			this.engineState[14] = array[9] - this.engineState[9];
			this.engineState[6] = Pack.LE_To_UInt32(ivBytes, 16);
			this.engineState[7] = Pack.LE_To_UInt32(ivBytes, 20);
			this.ResetCounter();
		}
	}
}
