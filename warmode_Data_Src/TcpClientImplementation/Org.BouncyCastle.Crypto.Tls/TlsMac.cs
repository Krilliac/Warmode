using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class TlsMac
	{
		protected readonly TlsContext context;

		protected readonly byte[] secret;

		protected readonly IMac mac;

		protected readonly int digestBlockSize;

		protected readonly int digestOverhead;

		protected readonly int macLength;

		public virtual byte[] MacSecret
		{
			get
			{
				return this.secret;
			}
		}

		public virtual int Size
		{
			get
			{
				return this.macLength;
			}
		}

		public TlsMac(TlsContext context, IDigest digest, byte[] key, int keyOff, int keyLen)
		{
			this.context = context;
			KeyParameter keyParameter = new KeyParameter(key, keyOff, keyLen);
			this.secret = Arrays.Clone(keyParameter.GetKey());
			if (digest is LongDigest)
			{
				this.digestBlockSize = 128;
				this.digestOverhead = 16;
			}
			else
			{
				this.digestBlockSize = 64;
				this.digestOverhead = 8;
			}
			if (TlsUtilities.IsSsl(context))
			{
				this.mac = new Ssl3Mac(digest);
				if (digest.GetDigestSize() == 20)
				{
					this.digestOverhead = 4;
				}
			}
			else
			{
				this.mac = new HMac(digest);
			}
			this.mac.Init(keyParameter);
			this.macLength = this.mac.GetMacSize();
			if (context.SecurityParameters.truncatedHMac)
			{
				this.macLength = Math.Min(this.macLength, 10);
			}
		}

		public virtual byte[] CalculateMac(long seqNo, byte type, byte[] message, int offset, int length)
		{
			ProtocolVersion serverVersion = this.context.ServerVersion;
			bool isSsl = serverVersion.IsSsl;
			byte[] array = new byte[isSsl ? 11 : 13];
			TlsUtilities.WriteUint64(seqNo, array, 0);
			TlsUtilities.WriteUint8(type, array, 8);
			if (!isSsl)
			{
				TlsUtilities.WriteVersion(serverVersion, array, 9);
			}
			TlsUtilities.WriteUint16(length, array, array.Length - 2);
			this.mac.BlockUpdate(array, 0, array.Length);
			this.mac.BlockUpdate(message, offset, length);
			return this.Truncate(MacUtilities.DoFinal(this.mac));
		}

		public virtual byte[] CalculateMacConstantTime(long seqNo, byte type, byte[] message, int offset, int length, int fullLength, byte[] dummyData)
		{
			byte[] result = this.CalculateMac(seqNo, type, message, offset, length);
			int num = TlsUtilities.IsSsl(this.context) ? 11 : 13;
			int num2 = this.GetDigestBlockCount(num + fullLength) - this.GetDigestBlockCount(num + length);
			while (--num2 >= 0)
			{
				this.mac.BlockUpdate(dummyData, 0, this.digestBlockSize);
			}
			this.mac.Update(dummyData[0]);
			this.mac.Reset();
			return result;
		}

		protected virtual int GetDigestBlockCount(int inputLength)
		{
			return (inputLength + this.digestOverhead) / this.digestBlockSize;
		}

		protected virtual byte[] Truncate(byte[] bs)
		{
			if (bs.Length <= this.macLength)
			{
				return bs;
			}
			return Arrays.CopyOf(bs, this.macLength);
		}
	}
}
