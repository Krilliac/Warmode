using System;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class Poly1305KeyGenerator : CipherKeyGenerator
	{
		private const byte R_MASK_LOW_2 = 252;

		private const byte R_MASK_HIGH_4 = 15;

		protected override void engineInit(KeyGenerationParameters param)
		{
			this.random = param.Random;
			this.strength = 32;
		}

		protected override byte[] engineGenerateKey()
		{
			byte[] array = base.engineGenerateKey();
			Poly1305KeyGenerator.Clamp(array);
			return array;
		}

		public static void Clamp(byte[] key)
		{
			if (key.Length != 32)
			{
				throw new ArgumentException("Poly1305 key must be 256 bits.");
			}
			int expr_1A_cp_1 = 19;
			key[expr_1A_cp_1] &= 15;
			int expr_31_cp_1 = 23;
			key[expr_31_cp_1] &= 15;
			int expr_48_cp_1 = 27;
			key[expr_48_cp_1] &= 15;
			int expr_5F_cp_1 = 31;
			key[expr_5F_cp_1] &= 15;
			int expr_76_cp_1 = 20;
			key[expr_76_cp_1] &= 252;
			int expr_90_cp_1 = 24;
			key[expr_90_cp_1] &= 252;
			int expr_AA_cp_1 = 28;
			key[expr_AA_cp_1] &= 252;
		}

		public static void CheckKey(byte[] key)
		{
			if (key.Length != 32)
			{
				throw new ArgumentException("Poly1305 key must be 256 bits.");
			}
			Poly1305KeyGenerator.checkMask(key[19], 15);
			Poly1305KeyGenerator.checkMask(key[23], 15);
			Poly1305KeyGenerator.checkMask(key[27], 15);
			Poly1305KeyGenerator.checkMask(key[31], 15);
			Poly1305KeyGenerator.checkMask(key[20], 252);
			Poly1305KeyGenerator.checkMask(key[24], 252);
			Poly1305KeyGenerator.checkMask(key[28], 252);
		}

		private static void checkMask(byte b, byte mask)
		{
			if ((b & ~(mask != 0)) != 0)
			{
				throw new ArgumentException("Invalid format for r portion of Poly1305 key.");
			}
		}
	}
}
