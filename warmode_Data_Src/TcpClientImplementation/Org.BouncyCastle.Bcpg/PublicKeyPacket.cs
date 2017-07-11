using Org.BouncyCastle.Utilities.Date;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class PublicKeyPacket : ContainedPacket
	{
		private int version;

		private long time;

		private int validDays;

		private PublicKeyAlgorithmTag algorithm;

		private IBcpgKey key;

		public virtual int Version
		{
			get
			{
				return this.version;
			}
		}

		public virtual PublicKeyAlgorithmTag Algorithm
		{
			get
			{
				return this.algorithm;
			}
		}

		public virtual int ValidDays
		{
			get
			{
				return this.validDays;
			}
		}

		public virtual IBcpgKey Key
		{
			get
			{
				return this.key;
			}
		}

		internal PublicKeyPacket(BcpgInputStream bcpgIn)
		{
			this.version = bcpgIn.ReadByte();
			this.time = (long)((ulong)(bcpgIn.ReadByte() << 24 | bcpgIn.ReadByte() << 16 | bcpgIn.ReadByte() << 8 | bcpgIn.ReadByte()));
			if (this.version <= 3)
			{
				this.validDays = (bcpgIn.ReadByte() << 8 | bcpgIn.ReadByte());
			}
			this.algorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
			PublicKeyAlgorithmTag publicKeyAlgorithmTag = this.algorithm;
			switch (publicKeyAlgorithmTag)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
			case PublicKeyAlgorithmTag.RsaSign:
				this.key = new RsaPublicBcpgKey(bcpgIn);
				return;
			default:
				switch (publicKeyAlgorithmTag)
				{
				case PublicKeyAlgorithmTag.ElGamalEncrypt:
				case PublicKeyAlgorithmTag.ElGamalGeneral:
					this.key = new ElGamalPublicBcpgKey(bcpgIn);
					return;
				case PublicKeyAlgorithmTag.Dsa:
					this.key = new DsaPublicBcpgKey(bcpgIn);
					return;
				}
				throw new IOException("unknown PGP public key algorithm encountered");
			}
		}

		public PublicKeyPacket(PublicKeyAlgorithmTag algorithm, DateTime time, IBcpgKey key)
		{
			this.version = 4;
			this.time = DateTimeUtilities.DateTimeToUnixMs(time) / 1000L;
			this.algorithm = algorithm;
			this.key = key;
		}

		public virtual DateTime GetTime()
		{
			return DateTimeUtilities.UnixMsToDateTime(this.time * 1000L);
		}

		public virtual byte[] GetEncodedContents()
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.WriteByte((byte)this.version);
			bcpgOutputStream.WriteInt((int)this.time);
			if (this.version <= 3)
			{
				bcpgOutputStream.WriteShort((short)this.validDays);
			}
			bcpgOutputStream.WriteByte((byte)this.algorithm);
			bcpgOutputStream.WriteObject((BcpgObject)this.key);
			return memoryStream.ToArray();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WritePacket(PacketTag.PublicKey, this.GetEncodedContents(), true);
		}
	}
}
