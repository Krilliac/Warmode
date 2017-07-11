using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class SymmetricKeyEncSessionPacket : ContainedPacket
	{
		private int version;

		private SymmetricKeyAlgorithmTag encAlgorithm;

		private S2k s2k;

		private readonly byte[] secKeyData;

		public SymmetricKeyAlgorithmTag EncAlgorithm
		{
			get
			{
				return this.encAlgorithm;
			}
		}

		public S2k S2k
		{
			get
			{
				return this.s2k;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public SymmetricKeyEncSessionPacket(BcpgInputStream bcpgIn)
		{
			this.version = bcpgIn.ReadByte();
			this.encAlgorithm = (SymmetricKeyAlgorithmTag)bcpgIn.ReadByte();
			this.s2k = new S2k(bcpgIn);
			this.secKeyData = bcpgIn.ReadAll();
		}

		public SymmetricKeyEncSessionPacket(SymmetricKeyAlgorithmTag encAlgorithm, S2k s2k, byte[] secKeyData)
		{
			this.version = 4;
			this.encAlgorithm = encAlgorithm;
			this.s2k = s2k;
			this.secKeyData = secKeyData;
		}

		public byte[] GetSecKeyData()
		{
			return this.secKeyData;
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.Write(new byte[]
			{
				(byte)this.version,
				(byte)this.encAlgorithm
			});
			bcpgOutputStream.WriteObject(this.s2k);
			if (this.secKeyData != null && this.secKeyData.Length > 0)
			{
				bcpgOutputStream.Write(this.secKeyData);
			}
			bcpgOut.WritePacket(PacketTag.SymmetricKeyEncryptedSessionKey, memoryStream.ToArray(), true);
		}
	}
}
