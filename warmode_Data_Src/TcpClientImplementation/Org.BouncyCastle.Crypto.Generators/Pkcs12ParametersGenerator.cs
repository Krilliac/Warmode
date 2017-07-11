using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Pkcs12ParametersGenerator : PbeParametersGenerator
	{
		public const int KeyMaterial = 1;

		public const int IVMaterial = 2;

		public const int MacMaterial = 3;

		private readonly IDigest digest;

		private readonly int u;

		private readonly int v;

		public Pkcs12ParametersGenerator(IDigest digest)
		{
			this.digest = digest;
			this.u = digest.GetDigestSize();
			this.v = digest.GetByteLength();
		}

		private void Adjust(byte[] a, int aOff, byte[] b)
		{
			int num = (int)((b[b.Length - 1] & 255) + (a[aOff + b.Length - 1] & 255) + 1);
			a[aOff + b.Length - 1] = (byte)num;
			num = (int)((uint)num >> 8);
			for (int i = b.Length - 2; i >= 0; i--)
			{
				num += (int)((b[i] & 255) + (a[aOff + i] & 255));
				a[aOff + i] = (byte)num;
				num = (int)((uint)num >> 8);
			}
		}

		private byte[] GenerateDerivedKey(int idByte, int n)
		{
			byte[] array = new byte[this.v];
			byte[] array2 = new byte[n];
			for (int num = 0; num != array.Length; num++)
			{
				array[num] = (byte)idByte;
			}
			byte[] array3;
			if (this.mSalt != null && this.mSalt.Length != 0)
			{
				array3 = new byte[this.v * ((this.mSalt.Length + this.v - 1) / this.v)];
				for (int num2 = 0; num2 != array3.Length; num2++)
				{
					array3[num2] = this.mSalt[num2 % this.mSalt.Length];
				}
			}
			else
			{
				array3 = new byte[0];
			}
			byte[] array4;
			if (this.mPassword != null && this.mPassword.Length != 0)
			{
				array4 = new byte[this.v * ((this.mPassword.Length + this.v - 1) / this.v)];
				for (int num3 = 0; num3 != array4.Length; num3++)
				{
					array4[num3] = this.mPassword[num3 % this.mPassword.Length];
				}
			}
			else
			{
				array4 = new byte[0];
			}
			byte[] array5 = new byte[array3.Length + array4.Length];
			Array.Copy(array3, 0, array5, 0, array3.Length);
			Array.Copy(array4, 0, array5, array3.Length, array4.Length);
			byte[] array6 = new byte[this.v];
			int num4 = (n + this.u - 1) / this.u;
			byte[] array7 = new byte[this.u];
			for (int i = 1; i <= num4; i++)
			{
				this.digest.BlockUpdate(array, 0, array.Length);
				this.digest.BlockUpdate(array5, 0, array5.Length);
				this.digest.DoFinal(array7, 0);
				for (int num5 = 1; num5 != this.mIterationCount; num5++)
				{
					this.digest.BlockUpdate(array7, 0, array7.Length);
					this.digest.DoFinal(array7, 0);
				}
				for (int num6 = 0; num6 != array6.Length; num6++)
				{
					array6[num6] = array7[num6 % array7.Length];
				}
				for (int num7 = 0; num7 != array5.Length / this.v; num7++)
				{
					this.Adjust(array5, num7 * this.v, array6);
				}
				if (i == num4)
				{
					Array.Copy(array7, 0, array2, (i - 1) * this.u, array2.Length - (i - 1) * this.u);
				}
				else
				{
					Array.Copy(array7, 0, array2, (i - 1) * this.u, array7.Length);
				}
			}
			return array2;
		}

		[Obsolete("Use version with 'algorithm' parameter")]
		public override ICipherParameters GenerateDerivedParameters(int keySize)
		{
			keySize /= 8;
			byte[] key = this.GenerateDerivedKey(1, keySize);
			return new KeyParameter(key, 0, keySize);
		}

		public override ICipherParameters GenerateDerivedParameters(string algorithm, int keySize)
		{
			keySize /= 8;
			byte[] keyBytes = this.GenerateDerivedKey(1, keySize);
			return ParameterUtilities.CreateKeyParameter(algorithm, keyBytes, 0, keySize);
		}

		[Obsolete("Use version with 'algorithm' parameter")]
		public override ICipherParameters GenerateDerivedParameters(int keySize, int ivSize)
		{
			keySize /= 8;
			ivSize /= 8;
			byte[] key = this.GenerateDerivedKey(1, keySize);
			byte[] iv = this.GenerateDerivedKey(2, ivSize);
			return new ParametersWithIV(new KeyParameter(key, 0, keySize), iv, 0, ivSize);
		}

		public override ICipherParameters GenerateDerivedParameters(string algorithm, int keySize, int ivSize)
		{
			keySize /= 8;
			ivSize /= 8;
			byte[] keyBytes = this.GenerateDerivedKey(1, keySize);
			KeyParameter parameters = ParameterUtilities.CreateKeyParameter(algorithm, keyBytes, 0, keySize);
			byte[] iv = this.GenerateDerivedKey(2, ivSize);
			return new ParametersWithIV(parameters, iv, 0, ivSize);
		}

		public override ICipherParameters GenerateDerivedMacParameters(int keySize)
		{
			keySize /= 8;
			byte[] key = this.GenerateDerivedKey(3, keySize);
			return new KeyParameter(key, 0, keySize);
		}
	}
}
