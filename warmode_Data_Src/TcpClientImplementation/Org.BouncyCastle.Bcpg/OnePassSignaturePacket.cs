using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class OnePassSignaturePacket : ContainedPacket
	{
		private int version;

		private int sigType;

		private HashAlgorithmTag hashAlgorithm;

		private PublicKeyAlgorithmTag keyAlgorithm;

		private long keyId;

		private int nested;

		public int SignatureType
		{
			get
			{
				return this.sigType;
			}
		}

		public PublicKeyAlgorithmTag KeyAlgorithm
		{
			get
			{
				return this.keyAlgorithm;
			}
		}

		public HashAlgorithmTag HashAlgorithm
		{
			get
			{
				return this.hashAlgorithm;
			}
		}

		public long KeyId
		{
			get
			{
				return this.keyId;
			}
		}

		internal OnePassSignaturePacket(BcpgInputStream bcpgIn)
		{
			this.version = bcpgIn.ReadByte();
			this.sigType = bcpgIn.ReadByte();
			this.hashAlgorithm = (HashAlgorithmTag)bcpgIn.ReadByte();
			this.keyAlgorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
			this.keyId |= (long)bcpgIn.ReadByte() << 56;
			this.keyId |= (long)bcpgIn.ReadByte() << 48;
			this.keyId |= (long)bcpgIn.ReadByte() << 40;
			this.keyId |= (long)bcpgIn.ReadByte() << 32;
			this.keyId |= (long)bcpgIn.ReadByte() << 24;
			this.keyId |= (long)bcpgIn.ReadByte() << 16;
			this.keyId |= (long)bcpgIn.ReadByte() << 8;
			this.keyId |= (long)((ulong)bcpgIn.ReadByte());
			this.nested = bcpgIn.ReadByte();
		}

		public OnePassSignaturePacket(int sigType, HashAlgorithmTag hashAlgorithm, PublicKeyAlgorithmTag keyAlgorithm, long keyId, bool isNested)
		{
			this.version = 3;
			this.sigType = sigType;
			this.hashAlgorithm = hashAlgorithm;
			this.keyAlgorithm = keyAlgorithm;
			this.keyId = keyId;
			this.nested = (isNested ? 0 : 1);
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.Write(new byte[]
			{
				(byte)this.version,
				(byte)this.sigType,
				(byte)this.hashAlgorithm,
				(byte)this.keyAlgorithm
			});
			bcpgOutputStream.WriteLong(this.keyId);
			bcpgOutputStream.WriteByte((byte)this.nested);
			bcpgOut.WritePacket(PacketTag.OnePassSignature, memoryStream.ToArray(), true);
		}
	}
}
