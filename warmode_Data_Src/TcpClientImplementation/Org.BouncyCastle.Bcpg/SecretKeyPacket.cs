using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class SecretKeyPacket : ContainedPacket
	{
		public const int UsageNone = 0;

		public const int UsageChecksum = 255;

		public const int UsageSha1 = 254;

		private PublicKeyPacket pubKeyPacket;

		private readonly byte[] secKeyData;

		private int s2kUsage;

		private SymmetricKeyAlgorithmTag encAlgorithm;

		private S2k s2k;

		private byte[] iv;

		public SymmetricKeyAlgorithmTag EncAlgorithm
		{
			get
			{
				return this.encAlgorithm;
			}
		}

		public int S2kUsage
		{
			get
			{
				return this.s2kUsage;
			}
		}

		public S2k S2k
		{
			get
			{
				return this.s2k;
			}
		}

		public PublicKeyPacket PublicKeyPacket
		{
			get
			{
				return this.pubKeyPacket;
			}
		}

		internal SecretKeyPacket(BcpgInputStream bcpgIn)
		{
			if (this is SecretSubkeyPacket)
			{
				this.pubKeyPacket = new PublicSubkeyPacket(bcpgIn);
			}
			else
			{
				this.pubKeyPacket = new PublicKeyPacket(bcpgIn);
			}
			this.s2kUsage = bcpgIn.ReadByte();
			if (this.s2kUsage == 255 || this.s2kUsage == 254)
			{
				this.encAlgorithm = (SymmetricKeyAlgorithmTag)bcpgIn.ReadByte();
				this.s2k = new S2k(bcpgIn);
			}
			else
			{
				this.encAlgorithm = (SymmetricKeyAlgorithmTag)this.s2kUsage;
			}
			if ((this.s2k == null || this.s2k.Type != 101 || this.s2k.ProtectionMode != 1) && this.s2kUsage != 0)
			{
				if (this.encAlgorithm < SymmetricKeyAlgorithmTag.Aes128)
				{
					this.iv = new byte[8];
				}
				else
				{
					this.iv = new byte[16];
				}
				bcpgIn.ReadFully(this.iv);
			}
			this.secKeyData = bcpgIn.ReadAll();
		}

		public SecretKeyPacket(PublicKeyPacket pubKeyPacket, SymmetricKeyAlgorithmTag encAlgorithm, S2k s2k, byte[] iv, byte[] secKeyData)
		{
			this.pubKeyPacket = pubKeyPacket;
			this.encAlgorithm = encAlgorithm;
			if (encAlgorithm != SymmetricKeyAlgorithmTag.Null)
			{
				this.s2kUsage = 255;
			}
			else
			{
				this.s2kUsage = 0;
			}
			this.s2k = s2k;
			this.iv = Arrays.Clone(iv);
			this.secKeyData = secKeyData;
		}

		public SecretKeyPacket(PublicKeyPacket pubKeyPacket, SymmetricKeyAlgorithmTag encAlgorithm, int s2kUsage, S2k s2k, byte[] iv, byte[] secKeyData)
		{
			this.pubKeyPacket = pubKeyPacket;
			this.encAlgorithm = encAlgorithm;
			this.s2kUsage = s2kUsage;
			this.s2k = s2k;
			this.iv = Arrays.Clone(iv);
			this.secKeyData = secKeyData;
		}

		public byte[] GetIV()
		{
			return Arrays.Clone(this.iv);
		}

		public byte[] GetSecretKeyData()
		{
			return this.secKeyData;
		}

		public byte[] GetEncodedContents()
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.Write(this.pubKeyPacket.GetEncodedContents());
			bcpgOutputStream.WriteByte((byte)this.s2kUsage);
			if (this.s2kUsage == 255 || this.s2kUsage == 254)
			{
				bcpgOutputStream.WriteByte((byte)this.encAlgorithm);
				bcpgOutputStream.WriteObject(this.s2k);
			}
			if (this.iv != null)
			{
				bcpgOutputStream.Write(this.iv);
			}
			if (this.secKeyData != null && this.secKeyData.Length > 0)
			{
				bcpgOutputStream.Write(this.secKeyData);
			}
			return memoryStream.ToArray();
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			bcpgOut.WritePacket(PacketTag.SecretKey, this.GetEncodedContents(), true);
		}
	}
}
