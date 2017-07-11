using Org.BouncyCastle.Math;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class PublicKeyEncSessionPacket : ContainedPacket
	{
		private int version;

		private long keyId;

		private PublicKeyAlgorithmTag algorithm;

		private BigInteger[] data;

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public long KeyId
		{
			get
			{
				return this.keyId;
			}
		}

		public PublicKeyAlgorithmTag Algorithm
		{
			get
			{
				return this.algorithm;
			}
		}

		internal PublicKeyEncSessionPacket(BcpgInputStream bcpgIn)
		{
			this.version = bcpgIn.ReadByte();
			this.keyId |= (long)bcpgIn.ReadByte() << 56;
			this.keyId |= (long)bcpgIn.ReadByte() << 48;
			this.keyId |= (long)bcpgIn.ReadByte() << 40;
			this.keyId |= (long)bcpgIn.ReadByte() << 32;
			this.keyId |= (long)bcpgIn.ReadByte() << 24;
			this.keyId |= (long)bcpgIn.ReadByte() << 16;
			this.keyId |= (long)bcpgIn.ReadByte() << 8;
			this.keyId |= (long)((ulong)bcpgIn.ReadByte());
			this.algorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
			PublicKeyAlgorithmTag publicKeyAlgorithmTag = this.algorithm;
			switch (publicKeyAlgorithmTag)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
				this.data = new BigInteger[]
				{
					new MPInteger(bcpgIn).Value
				};
				return;
			default:
				if (publicKeyAlgorithmTag != PublicKeyAlgorithmTag.ElGamalEncrypt && publicKeyAlgorithmTag != PublicKeyAlgorithmTag.ElGamalGeneral)
				{
					throw new IOException("unknown PGP public key algorithm encountered");
				}
				this.data = new BigInteger[]
				{
					new MPInteger(bcpgIn).Value,
					new MPInteger(bcpgIn).Value
				};
				return;
			}
		}

		public PublicKeyEncSessionPacket(long keyId, PublicKeyAlgorithmTag algorithm, BigInteger[] data)
		{
			this.version = 3;
			this.keyId = keyId;
			this.algorithm = algorithm;
			this.data = (BigInteger[])data.Clone();
		}

		public BigInteger[] GetEncSessionKey()
		{
			return (BigInteger[])this.data.Clone();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.WriteByte((byte)this.version);
			bcpgOutputStream.WriteLong(this.keyId);
			bcpgOutputStream.WriteByte((byte)this.algorithm);
			for (int num = 0; num != this.data.Length; num++)
			{
				MPInteger.Encode(bcpgOutputStream, this.data[num]);
			}
			bcpgOut.WritePacket(PacketTag.PublicKeyEncryptedSession, memoryStream.ToArray(), true);
		}
	}
}
