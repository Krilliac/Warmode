using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class RecordStream
	{
		private const int DEFAULT_PLAINTEXT_LIMIT = 16384;

		private TlsProtocol mHandler;

		private Stream mInput;

		private Stream mOutput;

		private TlsCompression mPendingCompression;

		private TlsCompression mReadCompression;

		private TlsCompression mWriteCompression;

		private TlsCipher mPendingCipher;

		private TlsCipher mReadCipher;

		private TlsCipher mWriteCipher;

		private long mReadSeqNo;

		private long mWriteSeqNo;

		private MemoryStream mBuffer = new MemoryStream();

		private TlsHandshakeHash mHandshakeHash;

		private ProtocolVersion mReadVersion;

		private ProtocolVersion mWriteVersion;

		private bool mRestrictReadVersion = true;

		private int mPlaintextLimit;

		private int mCompressedLimit;

		private int mCiphertextLimit;

		internal virtual ProtocolVersion ReadVersion
		{
			get
			{
				return this.mReadVersion;
			}
			set
			{
				this.mReadVersion = value;
			}
		}

		internal virtual TlsHandshakeHash HandshakeHash
		{
			get
			{
				return this.mHandshakeHash;
			}
		}

		internal RecordStream(TlsProtocol handler, Stream input, Stream output)
		{
			this.mHandler = handler;
			this.mInput = input;
			this.mOutput = output;
			this.mReadCompression = new TlsNullCompression();
			this.mWriteCompression = this.mReadCompression;
		}

		internal virtual void Init(TlsContext context)
		{
			this.mReadCipher = new TlsNullCipher(context);
			this.mWriteCipher = this.mReadCipher;
			this.mHandshakeHash = new DeferredHash();
			this.mHandshakeHash.Init(context);
			this.SetPlaintextLimit(16384);
		}

		internal virtual int GetPlaintextLimit()
		{
			return this.mPlaintextLimit;
		}

		internal virtual void SetPlaintextLimit(int plaintextLimit)
		{
			this.mPlaintextLimit = plaintextLimit;
			this.mCompressedLimit = this.mPlaintextLimit + 1024;
			this.mCiphertextLimit = this.mCompressedLimit + 1024;
		}

		internal virtual void SetWriteVersion(ProtocolVersion writeVersion)
		{
			this.mWriteVersion = writeVersion;
		}

		internal virtual void SetRestrictReadVersion(bool enabled)
		{
			this.mRestrictReadVersion = enabled;
		}

		internal virtual void SetPendingConnectionState(TlsCompression tlsCompression, TlsCipher tlsCipher)
		{
			this.mPendingCompression = tlsCompression;
			this.mPendingCipher = tlsCipher;
		}

		internal virtual void SentWriteCipherSpec()
		{
			if (this.mPendingCompression == null || this.mPendingCipher == null)
			{
				throw new TlsFatalAlert(40);
			}
			this.mWriteCompression = this.mPendingCompression;
			this.mWriteCipher = this.mPendingCipher;
			this.mWriteSeqNo = 0L;
		}

		internal virtual void ReceivedReadCipherSpec()
		{
			if (this.mPendingCompression == null || this.mPendingCipher == null)
			{
				throw new TlsFatalAlert(40);
			}
			this.mReadCompression = this.mPendingCompression;
			this.mReadCipher = this.mPendingCipher;
			this.mReadSeqNo = 0L;
		}

		internal virtual void FinaliseHandshake()
		{
			if (this.mReadCompression != this.mPendingCompression || this.mWriteCompression != this.mPendingCompression || this.mReadCipher != this.mPendingCipher || this.mWriteCipher != this.mPendingCipher)
			{
				throw new TlsFatalAlert(40);
			}
			this.mPendingCompression = null;
			this.mPendingCipher = null;
		}

		internal virtual bool ReadRecord()
		{
			byte[] array = TlsUtilities.ReadAllOrNothing(5, this.mInput);
			if (array == null)
			{
				return false;
			}
			byte b = TlsUtilities.ReadUint8(array, 0);
			RecordStream.CheckType(b, 10);
			if (!this.mRestrictReadVersion)
			{
				int num = TlsUtilities.ReadVersionRaw(array, 1);
				if (((long)num & (long)((ulong)-256)) != 768L)
				{
					throw new TlsFatalAlert(47);
				}
			}
			else
			{
				ProtocolVersion protocolVersion = TlsUtilities.ReadVersion(array, 1);
				if (this.mReadVersion == null)
				{
					this.mReadVersion = protocolVersion;
				}
				else if (!protocolVersion.Equals(this.mReadVersion))
				{
					throw new TlsFatalAlert(47);
				}
			}
			int len = TlsUtilities.ReadUint16(array, 3);
			byte[] array2 = this.DecodeAndVerify(b, this.mInput, len);
			this.mHandler.ProcessRecord(b, array2, 0, array2.Length);
			return true;
		}

		internal virtual byte[] DecodeAndVerify(byte type, Stream input, int len)
		{
			RecordStream.CheckLength(len, this.mCiphertextLimit, 22);
			byte[] array = TlsUtilities.ReadFully(len, input);
			TlsCipher arg_34_0 = this.mReadCipher;
			long seqNo;
			this.mReadSeqNo = (seqNo = this.mReadSeqNo) + 1L;
			byte[] array2 = arg_34_0.DecodeCiphertext(seqNo, type, array, 0, array.Length);
			RecordStream.CheckLength(array2.Length, this.mCompressedLimit, 22);
			Stream stream = this.mReadCompression.Decompress(this.mBuffer);
			if (stream != this.mBuffer)
			{
				stream.Write(array2, 0, array2.Length);
				stream.Flush();
				array2 = this.GetBufferContents();
			}
			RecordStream.CheckLength(array2.Length, this.mPlaintextLimit, 30);
			if (array2.Length < 1 && type != 23)
			{
				throw new TlsFatalAlert(47);
			}
			return array2;
		}

		internal virtual void WriteRecord(byte type, byte[] plaintext, int plaintextOffset, int plaintextLength)
		{
			if (this.mWriteVersion == null)
			{
				return;
			}
			RecordStream.CheckType(type, 80);
			RecordStream.CheckLength(plaintextLength, this.mPlaintextLimit, 80);
			if (plaintextLength < 1 && type != 23)
			{
				throw new TlsFatalAlert(80);
			}
			if (type == 22)
			{
				this.UpdateHandshakeData(plaintext, plaintextOffset, plaintextLength);
			}
			Stream stream = this.mWriteCompression.Compress(this.mBuffer);
			byte[] array;
			if (stream == this.mBuffer)
			{
				TlsCipher arg_7B_0 = this.mWriteCipher;
				long seqNo;
				this.mWriteSeqNo = (seqNo = this.mWriteSeqNo) + 1L;
				array = arg_7B_0.EncodePlaintext(seqNo, type, plaintext, plaintextOffset, plaintextLength);
			}
			else
			{
				stream.Write(plaintext, plaintextOffset, plaintextLength);
				stream.Flush();
				byte[] bufferContents = this.GetBufferContents();
				RecordStream.CheckLength(bufferContents.Length, plaintextLength + 1024, 80);
				TlsCipher arg_CC_0 = this.mWriteCipher;
				long seqNo2;
				this.mWriteSeqNo = (seqNo2 = this.mWriteSeqNo) + 1L;
				array = arg_CC_0.EncodePlaintext(seqNo2, type, bufferContents, 0, bufferContents.Length);
			}
			RecordStream.CheckLength(array.Length, this.mCiphertextLimit, 80);
			byte[] array2 = new byte[array.Length + 5];
			TlsUtilities.WriteUint8(type, array2, 0);
			TlsUtilities.WriteVersion(this.mWriteVersion, array2, 1);
			TlsUtilities.WriteUint16(array.Length, array2, 3);
			Array.Copy(array, 0, array2, 5, array.Length);
			this.mOutput.Write(array2, 0, array2.Length);
			this.mOutput.Flush();
		}

		internal virtual void NotifyHelloComplete()
		{
			this.mHandshakeHash = this.mHandshakeHash.NotifyPrfDetermined();
		}

		internal virtual TlsHandshakeHash PrepareToFinish()
		{
			TlsHandshakeHash result = this.mHandshakeHash;
			this.mHandshakeHash = this.mHandshakeHash.StopTracking();
			return result;
		}

		internal virtual void UpdateHandshakeData(byte[] message, int offset, int len)
		{
			this.mHandshakeHash.BlockUpdate(message, offset, len);
		}

		internal virtual void SafeClose()
		{
			try
			{
				this.mInput.Close();
			}
			catch (IOException)
			{
			}
			try
			{
				this.mOutput.Close();
			}
			catch (IOException)
			{
			}
		}

		internal virtual void Flush()
		{
			this.mOutput.Flush();
		}

		private byte[] GetBufferContents()
		{
			byte[] result = this.mBuffer.ToArray();
			this.mBuffer.SetLength(0L);
			return result;
		}

		private static void CheckType(byte type, byte alertDescription)
		{
			switch (type)
			{
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
				return;
			default:
				throw new TlsFatalAlert(alertDescription);
			}
		}

		private static void CheckLength(int length, int limit, byte alertDescription)
		{
			if (length > limit)
			{
				throw new TlsFatalAlert(alertDescription);
			}
		}
	}
}
