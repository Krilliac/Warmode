using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class BcpgInputStream : BaseInputStream
	{
		private class PartialInputStream : BaseInputStream
		{
			private BcpgInputStream m_in;

			private bool partial;

			private int dataLength;

			internal PartialInputStream(BcpgInputStream bcpgIn, bool partial, int dataLength)
			{
				this.m_in = bcpgIn;
				this.partial = partial;
				this.dataLength = dataLength;
			}

			public override int ReadByte()
			{
				while (this.dataLength == 0)
				{
					if (!this.partial || this.ReadPartialDataLength() < 0)
					{
						return -1;
					}
				}
				int num = this.m_in.ReadByte();
				if (num < 0)
				{
					throw new EndOfStreamException("Premature end of stream in PartialInputStream");
				}
				this.dataLength--;
				return num;
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				while (this.dataLength == 0)
				{
					if (!this.partial || this.ReadPartialDataLength() < 0)
					{
						return 0;
					}
				}
				int count2 = (this.dataLength > count || this.dataLength < 0) ? count : this.dataLength;
				int num = this.m_in.Read(buffer, offset, count2);
				if (num < 1)
				{
					throw new EndOfStreamException("Premature end of stream in PartialInputStream");
				}
				this.dataLength -= num;
				return num;
			}

			private int ReadPartialDataLength()
			{
				int num = this.m_in.ReadByte();
				if (num < 0)
				{
					return -1;
				}
				this.partial = false;
				if (num < 192)
				{
					this.dataLength = num;
				}
				else if (num <= 223)
				{
					this.dataLength = (num - 192 << 8) + this.m_in.ReadByte() + 192;
				}
				else if (num == 255)
				{
					this.dataLength = (this.m_in.ReadByte() << 24 | this.m_in.ReadByte() << 16 | this.m_in.ReadByte() << 8 | this.m_in.ReadByte());
				}
				else
				{
					this.partial = true;
					this.dataLength = 1 << num;
				}
				return 0;
			}
		}

		private Stream m_in;

		private bool next;

		private int nextB;

		internal static BcpgInputStream Wrap(Stream inStr)
		{
			if (inStr is BcpgInputStream)
			{
				return (BcpgInputStream)inStr;
			}
			return new BcpgInputStream(inStr);
		}

		private BcpgInputStream(Stream inputStream)
		{
			this.m_in = inputStream;
		}

		public override int ReadByte()
		{
			if (this.next)
			{
				this.next = false;
				return this.nextB;
			}
			return this.m_in.ReadByte();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!this.next)
			{
				return this.m_in.Read(buffer, offset, count);
			}
			if (this.nextB < 0)
			{
				return 0;
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			buffer[offset] = (byte)this.nextB;
			this.next = false;
			return 1;
		}

		public byte[] ReadAll()
		{
			return Streams.ReadAll(this);
		}

		public void ReadFully(byte[] buffer, int off, int len)
		{
			if (Streams.ReadFully(this, buffer, off, len) < len)
			{
				throw new EndOfStreamException();
			}
		}

		public void ReadFully(byte[] buffer)
		{
			this.ReadFully(buffer, 0, buffer.Length);
		}

		public PacketTag NextPacketTag()
		{
			if (!this.next)
			{
				try
				{
					this.nextB = this.m_in.ReadByte();
				}
				catch (EndOfStreamException)
				{
					this.nextB = -1;
				}
				this.next = true;
			}
			if (this.nextB < 0)
			{
				return (PacketTag)this.nextB;
			}
			if ((this.nextB & 64) != 0)
			{
				return (PacketTag)(this.nextB & 63);
			}
			return (PacketTag)((this.nextB & 63) >> 2);
		}

		public Packet ReadPacket()
		{
			int num = this.ReadByte();
			if (num < 0)
			{
				return null;
			}
			if ((num & 128) == 0)
			{
				throw new IOException("invalid header encountered");
			}
			bool flag = (num & 64) != 0;
			int num2 = 0;
			bool flag2 = false;
			PacketTag packetTag;
			if (flag)
			{
				packetTag = (PacketTag)(num & 63);
				int num3 = this.ReadByte();
				if (num3 < 192)
				{
					num2 = num3;
				}
				else if (num3 <= 223)
				{
					int num4 = this.m_in.ReadByte();
					num2 = (num3 - 192 << 8) + num4 + 192;
				}
				else if (num3 == 255)
				{
					num2 = (this.m_in.ReadByte() << 24 | this.m_in.ReadByte() << 16 | this.m_in.ReadByte() << 8 | this.m_in.ReadByte());
				}
				else
				{
					flag2 = true;
					num2 = 1 << num3;
				}
			}
			else
			{
				int num5 = num & 3;
				packetTag = (PacketTag)((num & 63) >> 2);
				switch (num5)
				{
				case 0:
					num2 = this.ReadByte();
					break;
				case 1:
					num2 = (this.ReadByte() << 8 | this.ReadByte());
					break;
				case 2:
					num2 = (this.ReadByte() << 24 | this.ReadByte() << 16 | this.ReadByte() << 8 | this.ReadByte());
					break;
				case 3:
					flag2 = true;
					break;
				default:
					throw new IOException("unknown length type encountered");
				}
			}
			BcpgInputStream bcpgIn;
			if (num2 == 0 && flag2)
			{
				bcpgIn = this;
			}
			else
			{
				BcpgInputStream.PartialInputStream inputStream = new BcpgInputStream.PartialInputStream(this, flag2, num2);
				bcpgIn = new BcpgInputStream(inputStream);
			}
			PacketTag packetTag2 = packetTag;
			switch (packetTag2)
			{
			case PacketTag.Reserved:
				return new InputStreamPacket(bcpgIn);
			case PacketTag.PublicKeyEncryptedSession:
				return new PublicKeyEncSessionPacket(bcpgIn);
			case PacketTag.Signature:
				return new SignaturePacket(bcpgIn);
			case PacketTag.SymmetricKeyEncryptedSessionKey:
				return new SymmetricKeyEncSessionPacket(bcpgIn);
			case PacketTag.OnePassSignature:
				return new OnePassSignaturePacket(bcpgIn);
			case PacketTag.SecretKey:
				return new SecretKeyPacket(bcpgIn);
			case PacketTag.PublicKey:
				return new PublicKeyPacket(bcpgIn);
			case PacketTag.SecretSubkey:
				return new SecretSubkeyPacket(bcpgIn);
			case PacketTag.CompressedData:
				return new CompressedDataPacket(bcpgIn);
			case PacketTag.SymmetricKeyEncrypted:
				return new SymmetricEncDataPacket(bcpgIn);
			case PacketTag.Marker:
				return new MarkerPacket(bcpgIn);
			case PacketTag.LiteralData:
				return new LiteralDataPacket(bcpgIn);
			case PacketTag.Trust:
				return new TrustPacket(bcpgIn);
			case PacketTag.UserId:
				return new UserIdPacket(bcpgIn);
			case PacketTag.PublicSubkey:
				return new PublicSubkeyPacket(bcpgIn);
			case (PacketTag)15:
			case (PacketTag)16:
				break;
			case PacketTag.UserAttribute:
				return new UserAttributePacket(bcpgIn);
			case PacketTag.SymmetricEncryptedIntegrityProtected:
				return new SymmetricEncIntegrityPacket(bcpgIn);
			case PacketTag.ModificationDetectionCode:
				return new ModDetectionCodePacket(bcpgIn);
			default:
				switch (packetTag2)
				{
				case PacketTag.Experimental1:
				case PacketTag.Experimental2:
				case PacketTag.Experimental3:
				case PacketTag.Experimental4:
					return new ExperimentalPacket(packetTag, bcpgIn);
				}
				break;
			}
			throw new IOException("unknown packet type encountered: " + packetTag);
		}

		public override void Close()
		{
			this.m_in.Close();
			base.Close();
		}
	}
}
