using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Bcpg
{
	public class SignatureSubpacketsParser
	{
		private readonly Stream input;

		public SignatureSubpacketsParser(Stream input)
		{
			this.input = input;
		}

		public SignatureSubpacket ReadPacket()
		{
			int num = this.input.ReadByte();
			if (num < 0)
			{
				return null;
			}
			int num2 = 0;
			if (num < 192)
			{
				num2 = num;
			}
			else if (num <= 223)
			{
				num2 = (num - 192 << 8) + this.input.ReadByte() + 192;
			}
			else if (num == 255)
			{
				num2 = (this.input.ReadByte() << 24 | this.input.ReadByte() << 16 | this.input.ReadByte() << 8 | this.input.ReadByte());
			}
			int num3 = this.input.ReadByte();
			if (num3 < 0)
			{
				throw new EndOfStreamException("unexpected EOF reading signature sub packet");
			}
			byte[] array = new byte[num2 - 1];
			if (Streams.ReadFully(this.input, array) < array.Length)
			{
				throw new EndOfStreamException();
			}
			bool critical = (num3 & 128) != 0;
			SignatureSubpacketTag type = (SignatureSubpacketTag)(num3 & 127);
			switch (type)
			{
			case SignatureSubpacketTag.CreationTime:
				return new SignatureCreationTime(critical, array);
			case SignatureSubpacketTag.ExpireTime:
				return new SignatureExpirationTime(critical, array);
			case SignatureSubpacketTag.Exportable:
				return new Exportable(critical, array);
			case SignatureSubpacketTag.TrustSig:
				return new TrustSignature(critical, array);
			case SignatureSubpacketTag.Revocable:
				return new Revocable(critical, array);
			case SignatureSubpacketTag.KeyExpireTime:
				return new KeyExpirationTime(critical, array);
			case SignatureSubpacketTag.PreferredSymmetricAlgorithms:
			case SignatureSubpacketTag.PreferredHashAlgorithms:
			case SignatureSubpacketTag.PreferredCompressionAlgorithms:
				return new PreferredAlgorithms(type, critical, array);
			case SignatureSubpacketTag.IssuerKeyId:
				return new IssuerKeyId(critical, array);
			case SignatureSubpacketTag.NotationData:
				return new NotationData(critical, array);
			case SignatureSubpacketTag.PrimaryUserId:
				return new PrimaryUserId(critical, array);
			case SignatureSubpacketTag.KeyFlags:
				return new KeyFlags(critical, array);
			case SignatureSubpacketTag.SignerUserId:
				return new SignerUserId(critical, array);
			}
			return new SignatureSubpacket(type, critical, array);
		}
	}
}
