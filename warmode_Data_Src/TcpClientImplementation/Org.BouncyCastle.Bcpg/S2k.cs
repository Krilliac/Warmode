using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class S2k : BcpgObject
	{
		private const int ExpBias = 6;

		public const int Simple = 0;

		public const int Salted = 1;

		public const int SaltedAndIterated = 3;

		public const int GnuDummyS2K = 101;

		public const int GnuProtectionModeNoPrivateKey = 1;

		public const int GnuProtectionModeDivertToCard = 2;

		internal int type;

		internal HashAlgorithmTag algorithm;

		internal byte[] iv;

		internal int itCount = -1;

		internal int protectionMode = -1;

		public int Type
		{
			get
			{
				return this.type;
			}
		}

		public HashAlgorithmTag HashAlgorithm
		{
			get
			{
				return this.algorithm;
			}
		}

		public long IterationCount
		{
			get
			{
				return (long)((long)(16 + (this.itCount & 15)) << (this.itCount >> 4) + 6);
			}
		}

		public int ProtectionMode
		{
			get
			{
				return this.protectionMode;
			}
		}

		internal S2k(Stream inStr)
		{
			this.type = inStr.ReadByte();
			this.algorithm = (HashAlgorithmTag)inStr.ReadByte();
			if (this.type != 101)
			{
				if (this.type != 0)
				{
					this.iv = new byte[8];
					if (Streams.ReadFully(inStr, this.iv, 0, this.iv.Length) < this.iv.Length)
					{
						throw new EndOfStreamException();
					}
					if (this.type == 3)
					{
						this.itCount = inStr.ReadByte();
						return;
					}
				}
			}
			else
			{
				inStr.ReadByte();
				inStr.ReadByte();
				inStr.ReadByte();
				this.protectionMode = inStr.ReadByte();
			}
		}

		public S2k(HashAlgorithmTag algorithm)
		{
			this.type = 0;
			this.algorithm = algorithm;
		}

		public S2k(HashAlgorithmTag algorithm, byte[] iv)
		{
			this.type = 1;
			this.algorithm = algorithm;
			this.iv = iv;
		}

		public S2k(HashAlgorithmTag algorithm, byte[] iv, int itCount)
		{
			this.type = 3;
			this.algorithm = algorithm;
			this.iv = iv;
			this.itCount = itCount;
		}

		public byte[] GetIV()
		{
			return Arrays.Clone(this.iv);
		}

		[Obsolete("Use 'IterationCount' property instead")]
		public long GetIterationCount()
		{
			return this.IterationCount;
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WriteByte((byte)this.type);
			bcpgOut.WriteByte((byte)this.algorithm);
			if (this.type != 101)
			{
				if (this.type != 0)
				{
					bcpgOut.Write(this.iv);
				}
				if (this.type == 3)
				{
					bcpgOut.WriteByte((byte)this.itCount);
					return;
				}
			}
			else
			{
				bcpgOut.WriteByte(71);
				bcpgOut.WriteByte(78);
				bcpgOut.WriteByte(85);
				bcpgOut.WriteByte((byte)this.protectionMode);
			}
		}
	}
}
