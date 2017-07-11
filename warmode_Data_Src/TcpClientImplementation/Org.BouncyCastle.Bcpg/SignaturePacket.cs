using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Date;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class SignaturePacket : ContainedPacket
	{
		private int version;

		private int signatureType;

		private long creationTime;

		private long keyId;

		private PublicKeyAlgorithmTag keyAlgorithm;

		private HashAlgorithmTag hashAlgorithm;

		private MPInteger[] signature;

		private byte[] fingerprint;

		private SignatureSubpacket[] hashedData;

		private SignatureSubpacket[] unhashedData;

		private byte[] signatureEncoding;

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public int SignatureType
		{
			get
			{
				return this.signatureType;
			}
		}

		public long KeyId
		{
			get
			{
				return this.keyId;
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

		public long CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		internal SignaturePacket(BcpgInputStream bcpgIn)
		{
			this.version = bcpgIn.ReadByte();
			if (this.version == 3 || this.version == 2)
			{
				bcpgIn.ReadByte();
				this.signatureType = bcpgIn.ReadByte();
				this.creationTime = ((long)bcpgIn.ReadByte() << 24 | (long)bcpgIn.ReadByte() << 16 | (long)bcpgIn.ReadByte() << 8 | (long)((ulong)bcpgIn.ReadByte())) * 1000L;
				this.keyId |= (long)bcpgIn.ReadByte() << 56;
				this.keyId |= (long)bcpgIn.ReadByte() << 48;
				this.keyId |= (long)bcpgIn.ReadByte() << 40;
				this.keyId |= (long)bcpgIn.ReadByte() << 32;
				this.keyId |= (long)bcpgIn.ReadByte() << 24;
				this.keyId |= (long)bcpgIn.ReadByte() << 16;
				this.keyId |= (long)bcpgIn.ReadByte() << 8;
				this.keyId |= (long)((ulong)bcpgIn.ReadByte());
				this.keyAlgorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
				this.hashAlgorithm = (HashAlgorithmTag)bcpgIn.ReadByte();
			}
			else
			{
				if (this.version != 4)
				{
					throw new Exception("unsupported version: " + this.version);
				}
				this.signatureType = bcpgIn.ReadByte();
				this.keyAlgorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
				this.hashAlgorithm = (HashAlgorithmTag)bcpgIn.ReadByte();
				int num = bcpgIn.ReadByte() << 8 | bcpgIn.ReadByte();
				byte[] buffer = new byte[num];
				bcpgIn.ReadFully(buffer);
				SignatureSubpacketsParser signatureSubpacketsParser = new SignatureSubpacketsParser(new MemoryStream(buffer, false));
				IList list = Platform.CreateArrayList();
				SignatureSubpacket value;
				while ((value = signatureSubpacketsParser.ReadPacket()) != null)
				{
					list.Add(value);
				}
				this.hashedData = new SignatureSubpacket[list.Count];
				for (int num2 = 0; num2 != this.hashedData.Length; num2++)
				{
					SignatureSubpacket signatureSubpacket = (SignatureSubpacket)list[num2];
					if (signatureSubpacket is IssuerKeyId)
					{
						this.keyId = ((IssuerKeyId)signatureSubpacket).KeyId;
					}
					else if (signatureSubpacket is SignatureCreationTime)
					{
						this.creationTime = DateTimeUtilities.DateTimeToUnixMs(((SignatureCreationTime)signatureSubpacket).GetTime());
					}
					this.hashedData[num2] = signatureSubpacket;
				}
				int num3 = bcpgIn.ReadByte() << 8 | bcpgIn.ReadByte();
				byte[] buffer2 = new byte[num3];
				bcpgIn.ReadFully(buffer2);
				signatureSubpacketsParser = new SignatureSubpacketsParser(new MemoryStream(buffer2, false));
				list.Clear();
				while ((value = signatureSubpacketsParser.ReadPacket()) != null)
				{
					list.Add(value);
				}
				this.unhashedData = new SignatureSubpacket[list.Count];
				for (int num4 = 0; num4 != this.unhashedData.Length; num4++)
				{
					SignatureSubpacket signatureSubpacket2 = (SignatureSubpacket)list[num4];
					if (signatureSubpacket2 is IssuerKeyId)
					{
						this.keyId = ((IssuerKeyId)signatureSubpacket2).KeyId;
					}
					this.unhashedData[num4] = signatureSubpacket2;
				}
			}
			this.fingerprint = new byte[2];
			bcpgIn.ReadFully(this.fingerprint);
			PublicKeyAlgorithmTag publicKeyAlgorithmTag = this.keyAlgorithm;
			switch (publicKeyAlgorithmTag)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaSign:
			{
				MPInteger mPInteger = new MPInteger(bcpgIn);
				this.signature = new MPInteger[]
				{
					mPInteger
				};
				return;
			}
			case PublicKeyAlgorithmTag.RsaEncrypt:
				break;
			default:
				switch (publicKeyAlgorithmTag)
				{
				case PublicKeyAlgorithmTag.ElGamalEncrypt:
				case PublicKeyAlgorithmTag.ElGamalGeneral:
				{
					MPInteger mPInteger2 = new MPInteger(bcpgIn);
					MPInteger mPInteger3 = new MPInteger(bcpgIn);
					MPInteger mPInteger4 = new MPInteger(bcpgIn);
					this.signature = new MPInteger[]
					{
						mPInteger2,
						mPInteger3,
						mPInteger4
					};
					return;
				}
				case PublicKeyAlgorithmTag.Dsa:
				{
					MPInteger mPInteger5 = new MPInteger(bcpgIn);
					MPInteger mPInteger6 = new MPInteger(bcpgIn);
					this.signature = new MPInteger[]
					{
						mPInteger5,
						mPInteger6
					};
					return;
				}
				}
				break;
			}
			if (this.keyAlgorithm >= PublicKeyAlgorithmTag.Experimental_1 && this.keyAlgorithm <= PublicKeyAlgorithmTag.Experimental_11)
			{
				this.signature = null;
				MemoryStream memoryStream = new MemoryStream();
				int num5;
				while ((num5 = bcpgIn.ReadByte()) >= 0)
				{
					memoryStream.WriteByte((byte)num5);
				}
				this.signatureEncoding = memoryStream.ToArray();
				return;
			}
			throw new IOException("unknown signature key algorithm: " + this.keyAlgorithm);
		}

		public SignaturePacket(int signatureType, long keyId, PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm, SignatureSubpacket[] hashedData, SignatureSubpacket[] unhashedData, byte[] fingerprint, MPInteger[] signature) : this(4, signatureType, keyId, keyAlgorithm, hashAlgorithm, hashedData, unhashedData, fingerprint, signature)
		{
		}

		public SignaturePacket(int version, int signatureType, long keyId, PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm, long creationTime, byte[] fingerprint, MPInteger[] signature) : this(version, signatureType, keyId, keyAlgorithm, hashAlgorithm, null, null, fingerprint, signature)
		{
			this.creationTime = creationTime;
		}

		public SignaturePacket(int version, int signatureType, long keyId, PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm, SignatureSubpacket[] hashedData, SignatureSubpacket[] unhashedData, byte[] fingerprint, MPInteger[] signature)
		{
			this.version = version;
			this.signatureType = signatureType;
			this.keyId = keyId;
			this.keyAlgorithm = keyAlgorithm;
			this.hashAlgorithm = hashAlgorithm;
			this.hashedData = hashedData;
			this.unhashedData = unhashedData;
			this.fingerprint = fingerprint;
			this.signature = signature;
			if (hashedData != null)
			{
				this.setCreationTime();
			}
		}

		public byte[] GetSignatureTrailer()
		{
			byte[] array;
			if (this.version == 3)
			{
				array = new byte[5];
				long num = this.creationTime / 1000L;
				array[0] = (byte)this.signatureType;
				array[1] = (byte)(num >> 24);
				array[2] = (byte)(num >> 16);
				array[3] = (byte)(num >> 8);
				array[4] = (byte)num;
			}
			else
			{
				MemoryStream memoryStream = new MemoryStream();
				memoryStream.WriteByte((byte)this.Version);
				memoryStream.WriteByte((byte)this.SignatureType);
				memoryStream.WriteByte((byte)this.KeyAlgorithm);
				memoryStream.WriteByte((byte)this.HashAlgorithm);
				MemoryStream memoryStream2 = new MemoryStream();
				SignatureSubpacket[] hashedSubPackets = this.GetHashedSubPackets();
				for (int num2 = 0; num2 != hashedSubPackets.Length; num2++)
				{
					hashedSubPackets[num2].Encode(memoryStream2);
				}
				byte[] array2 = memoryStream2.ToArray();
				memoryStream.WriteByte((byte)(array2.Length >> 8));
				memoryStream.WriteByte((byte)array2.Length);
				memoryStream.Write(array2, 0, array2.Length);
				byte[] array3 = memoryStream.ToArray();
				memoryStream.WriteByte((byte)this.Version);
				memoryStream.WriteByte(255);
				memoryStream.WriteByte((byte)(array3.Length >> 24));
				memoryStream.WriteByte((byte)(array3.Length >> 16));
				memoryStream.WriteByte((byte)(array3.Length >> 8));
				memoryStream.WriteByte((byte)array3.Length);
				array = memoryStream.ToArray();
			}
			return array;
		}

		public MPInteger[] GetSignature()
		{
			return this.signature;
		}

		public byte[] GetSignatureBytes()
		{
			if (this.signatureEncoding != null)
			{
				return (byte[])this.signatureEncoding.Clone();
			}
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			MPInteger[] array = this.signature;
			for (int i = 0; i < array.Length; i++)
			{
				MPInteger bcpgObject = array[i];
				try
				{
					bcpgOutputStream.WriteObject(bcpgObject);
				}
				catch (IOException arg)
				{
					throw new Exception("internal error: " + arg);
				}
			}
			return memoryStream.ToArray();
		}

		public SignatureSubpacket[] GetHashedSubPackets()
		{
			return this.hashedData;
		}

		public SignatureSubpacket[] GetUnhashedSubPackets()
		{
			return this.unhashedData;
		}

		public override void Encode(BcpgOutputStream bcpgOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream);
			bcpgOutputStream.WriteByte((byte)this.version);
			if (this.version == 3 || this.version == 2)
			{
				bcpgOutputStream.Write(new byte[]
				{
					5,
					(byte)this.signatureType
				});
				bcpgOutputStream.WriteInt((int)(this.creationTime / 1000L));
				bcpgOutputStream.WriteLong(this.keyId);
				bcpgOutputStream.Write(new byte[]
				{
					(byte)this.keyAlgorithm,
					(byte)this.hashAlgorithm
				});
			}
			else
			{
				if (this.version != 4)
				{
					throw new IOException("unknown version: " + this.version);
				}
				bcpgOutputStream.Write(new byte[]
				{
					(byte)this.signatureType,
					(byte)this.keyAlgorithm,
					(byte)this.hashAlgorithm
				});
				SignaturePacket.EncodeLengthAndData(bcpgOutputStream, SignaturePacket.GetEncodedSubpackets(this.hashedData));
				SignaturePacket.EncodeLengthAndData(bcpgOutputStream, SignaturePacket.GetEncodedSubpackets(this.unhashedData));
			}
			bcpgOutputStream.Write(this.fingerprint);
			if (this.signature != null)
			{
				bcpgOutputStream.WriteObjects(this.signature);
			}
			else
			{
				bcpgOutputStream.Write(this.signatureEncoding);
			}
			bcpgOut.WritePacket(PacketTag.Signature, memoryStream.ToArray(), true);
		}

		private static void EncodeLengthAndData(BcpgOutputStream pOut, byte[] data)
		{
			pOut.WriteShort((short)data.Length);
			pOut.Write(data);
		}

		private static byte[] GetEncodedSubpackets(SignatureSubpacket[] ps)
		{
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < ps.Length; i++)
			{
				SignatureSubpacket signatureSubpacket = ps[i];
				signatureSubpacket.Encode(memoryStream);
			}
			return memoryStream.ToArray();
		}

		private void setCreationTime()
		{
			SignatureSubpacket[] array = this.hashedData;
			for (int i = 0; i < array.Length; i++)
			{
				SignatureSubpacket signatureSubpacket = array[i];
				if (signatureSubpacket is SignatureCreationTime)
				{
					this.creationTime = DateTimeUtilities.DateTimeToUnixMs(((SignatureCreationTime)signatureSubpacket).GetTime());
					return;
				}
			}
		}
	}
}
