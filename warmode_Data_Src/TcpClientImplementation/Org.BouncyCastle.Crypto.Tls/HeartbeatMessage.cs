using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class HeartbeatMessage
	{
		internal class PayloadBuffer : MemoryStream
		{
			internal byte[] ToTruncatedByteArray(int payloadLength)
			{
				int num = payloadLength + 16;
				if (this.Length < (long)num)
				{
					return null;
				}
				return Arrays.CopyOf(this.GetBuffer(), payloadLength);
			}
		}

		protected readonly byte mType;

		protected readonly byte[] mPayload;

		protected readonly int mPaddingLength;

		public HeartbeatMessage(byte type, byte[] payload, int paddingLength)
		{
			if (!HeartbeatMessageType.IsValid(type))
			{
				throw new ArgumentException("not a valid HeartbeatMessageType value", "type");
			}
			if (payload == null || payload.Length >= 65536)
			{
				throw new ArgumentException("must have length < 2^16", "payload");
			}
			if (paddingLength < 16)
			{
				throw new ArgumentException("must be at least 16", "paddingLength");
			}
			this.mType = type;
			this.mPayload = payload;
			this.mPaddingLength = paddingLength;
		}

		public virtual void Encode(TlsContext context, Stream output)
		{
			TlsUtilities.WriteUint8(this.mType, output);
			TlsUtilities.CheckUint16(this.mPayload.Length);
			TlsUtilities.WriteUint16(this.mPayload.Length, output);
			output.Write(this.mPayload, 0, this.mPayload.Length);
			byte[] array = new byte[this.mPaddingLength];
			context.NonceRandomGenerator.NextBytes(array);
			output.Write(array, 0, array.Length);
		}

		public static HeartbeatMessage Parse(Stream input)
		{
			byte b = TlsUtilities.ReadUint8(input);
			if (!HeartbeatMessageType.IsValid(b))
			{
				throw new TlsFatalAlert(47);
			}
			int payloadLength = TlsUtilities.ReadUint16(input);
			HeartbeatMessage.PayloadBuffer payloadBuffer = new HeartbeatMessage.PayloadBuffer();
			Streams.PipeAll(input, payloadBuffer);
			byte[] array = payloadBuffer.ToTruncatedByteArray(payloadLength);
			if (array == null)
			{
				return null;
			}
			TlsUtilities.CheckUint16(payloadBuffer.Length);
			int paddingLength = (int)payloadBuffer.Length - array.Length;
			return new HeartbeatMessage(b, array, paddingLength);
		}
	}
}
