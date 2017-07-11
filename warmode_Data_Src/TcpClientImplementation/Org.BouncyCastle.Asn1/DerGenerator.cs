using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public abstract class DerGenerator : Asn1Generator
	{
		private bool _tagged;

		private bool _isExplicit;

		private int _tagNo;

		protected DerGenerator(Stream outStream) : base(outStream)
		{
		}

		protected DerGenerator(Stream outStream, int tagNo, bool isExplicit) : base(outStream)
		{
			this._tagged = true;
			this._isExplicit = isExplicit;
			this._tagNo = tagNo;
		}

		private static void WriteLength(Stream outStr, int length)
		{
			if (length > 127)
			{
				int num = 1;
				int num2 = length;
				while ((num2 >>= 8) != 0)
				{
					num++;
				}
				outStr.WriteByte((byte)(num | 128));
				for (int i = (num - 1) * 8; i >= 0; i -= 8)
				{
					outStr.WriteByte((byte)(length >> i));
				}
				return;
			}
			outStr.WriteByte((byte)length);
		}

		internal static void WriteDerEncoded(Stream outStream, int tag, byte[] bytes)
		{
			outStream.WriteByte((byte)tag);
			DerGenerator.WriteLength(outStream, bytes.Length);
			outStream.Write(bytes, 0, bytes.Length);
		}

		internal void WriteDerEncoded(int tag, byte[] bytes)
		{
			if (!this._tagged)
			{
				DerGenerator.WriteDerEncoded(base.Out, tag, bytes);
				return;
			}
			int num = this._tagNo | 128;
			if (this._isExplicit)
			{
				int tag2 = this._tagNo | 32 | 128;
				MemoryStream memoryStream = new MemoryStream();
				DerGenerator.WriteDerEncoded(memoryStream, tag, bytes);
				DerGenerator.WriteDerEncoded(base.Out, tag2, memoryStream.ToArray());
				return;
			}
			if ((tag & 32) != 0)
			{
				num |= 32;
			}
			DerGenerator.WriteDerEncoded(base.Out, num, bytes);
		}

		internal static void WriteDerEncoded(Stream outStr, int tag, Stream inStr)
		{
			DerGenerator.WriteDerEncoded(outStr, tag, Streams.ReadAll(inStr));
		}
	}
}
