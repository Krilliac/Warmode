using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Org.BouncyCastle.Bcpg
{
	public class ArmoredOutputStream : BaseOutputStream
	{
		private static readonly byte[] encodingTable = new byte[]
		{
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			43,
			47
		};

		private readonly Stream outStream;

		private int[] buf = new int[3];

		private int bufPtr;

		private Crc24 crc = new Crc24();

		private int chunkCount;

		private int lastb;

		private bool start = true;

		private bool clearText;

		private bool newLine;

		private string type;

		private static readonly string nl = Platform.NewLine;

		private static readonly string headerStart = "-----BEGIN PGP ";

		private static readonly string headerTail = "-----";

		private static readonly string footerStart = "-----END PGP ";

		private static readonly string footerTail = "-----";

		private static readonly string version = "BCPG C# v" + Assembly.GetExecutingAssembly().GetName().Version;

		private readonly IDictionary headers;

		private static void Encode(Stream outStream, int[] data, int len)
		{
			byte[] array = new byte[4];
			int num = data[0];
			array[0] = ArmoredOutputStream.encodingTable[num >> 2 & 63];
			switch (len)
			{
			case 1:
				array[1] = ArmoredOutputStream.encodingTable[num << 4 & 63];
				array[2] = 61;
				array[3] = 61;
				break;
			case 2:
			{
				int num2 = data[1];
				array[1] = ArmoredOutputStream.encodingTable[(num << 4 | num2 >> 4) & 63];
				array[2] = ArmoredOutputStream.encodingTable[num2 << 2 & 63];
				array[3] = 61;
				break;
			}
			case 3:
			{
				int num3 = data[1];
				int num4 = data[2];
				array[1] = ArmoredOutputStream.encodingTable[(num << 4 | num3 >> 4) & 63];
				array[2] = ArmoredOutputStream.encodingTable[(num3 << 2 | num4 >> 6) & 63];
				array[3] = ArmoredOutputStream.encodingTable[num4 & 63];
				break;
			}
			}
			outStream.Write(array, 0, array.Length);
		}

		public ArmoredOutputStream(Stream outStream)
		{
			this.outStream = outStream;
			this.headers = Platform.CreateHashtable();
			this.headers["Version"] = ArmoredOutputStream.version;
		}

		public ArmoredOutputStream(Stream outStream, IDictionary headers)
		{
			this.outStream = outStream;
			this.headers = Platform.CreateHashtable(headers);
			this.headers["Version"] = ArmoredOutputStream.version;
		}

		public void SetHeader(string name, string v)
		{
			this.headers[name] = v;
		}

		public void ResetHeaders()
		{
			this.headers.Clear();
			this.headers["Version"] = ArmoredOutputStream.version;
		}

		public void BeginClearText(HashAlgorithmTag hashAlgorithm)
		{
			string str;
			switch (hashAlgorithm)
			{
			case HashAlgorithmTag.MD5:
				str = "MD5";
				goto IL_82;
			case HashAlgorithmTag.Sha1:
				str = "SHA1";
				goto IL_82;
			case HashAlgorithmTag.RipeMD160:
				str = "RIPEMD160";
				goto IL_82;
			case HashAlgorithmTag.MD2:
				str = "MD2";
				goto IL_82;
			case HashAlgorithmTag.Sha256:
				str = "SHA256";
				goto IL_82;
			case HashAlgorithmTag.Sha384:
				str = "SHA384";
				goto IL_82;
			case HashAlgorithmTag.Sha512:
				str = "SHA512";
				goto IL_82;
			}
			throw new IOException("unknown hash algorithm tag in beginClearText: " + hashAlgorithm);
			IL_82:
			this.DoWrite("-----BEGIN PGP SIGNED MESSAGE-----" + ArmoredOutputStream.nl);
			this.DoWrite("Hash: " + str + ArmoredOutputStream.nl + ArmoredOutputStream.nl);
			this.clearText = true;
			this.newLine = true;
			this.lastb = 0;
		}

		public void EndClearText()
		{
			this.clearText = false;
		}

		public override void WriteByte(byte b)
		{
			if (this.clearText)
			{
				this.outStream.WriteByte(b);
				if (this.newLine)
				{
					if (b != 10 || this.lastb != 13)
					{
						this.newLine = false;
					}
					if (b == 45)
					{
						this.outStream.WriteByte(32);
						this.outStream.WriteByte(45);
					}
				}
				if (b == 13 || (b == 10 && this.lastb != 13))
				{
					this.newLine = true;
				}
				this.lastb = (int)b;
				return;
			}
			if (this.start)
			{
				bool flag = (b & 64) != 0;
				int num;
				if (flag)
				{
					num = (int)(b & 63);
				}
				else
				{
					num = (b & 63) >> 2;
				}
				switch (num)
				{
				case 2:
					this.type = "SIGNATURE";
					goto IL_EF;
				case 5:
					this.type = "PRIVATE KEY BLOCK";
					goto IL_EF;
				case 6:
					this.type = "PUBLIC KEY BLOCK";
					goto IL_EF;
				}
				this.type = "MESSAGE";
				IL_EF:
				this.DoWrite(ArmoredOutputStream.headerStart + this.type + ArmoredOutputStream.headerTail + ArmoredOutputStream.nl);
				this.WriteHeaderEntry("Version", (string)this.headers["Version"]);
				foreach (DictionaryEntry dictionaryEntry in this.headers)
				{
					string text = (string)dictionaryEntry.Key;
					if (text != "Version")
					{
						string v = (string)dictionaryEntry.Value;
						this.WriteHeaderEntry(text, v);
					}
				}
				this.DoWrite(ArmoredOutputStream.nl);
				this.start = false;
			}
			if (this.bufPtr == 3)
			{
				ArmoredOutputStream.Encode(this.outStream, this.buf, this.bufPtr);
				this.bufPtr = 0;
				if ((++this.chunkCount & 15) == 0)
				{
					this.DoWrite(ArmoredOutputStream.nl);
				}
			}
			this.crc.Update((int)b);
			this.buf[this.bufPtr++] = (int)(b & 255);
		}

		public override void Close()
		{
			if (this.type != null)
			{
				if (this.bufPtr > 0)
				{
					ArmoredOutputStream.Encode(this.outStream, this.buf, this.bufPtr);
				}
				this.DoWrite(ArmoredOutputStream.nl + '=');
				int value = this.crc.Value;
				this.buf[0] = (value >> 16 & 255);
				this.buf[1] = (value >> 8 & 255);
				this.buf[2] = (value & 255);
				ArmoredOutputStream.Encode(this.outStream, this.buf, 3);
				this.DoWrite(ArmoredOutputStream.nl);
				this.DoWrite(ArmoredOutputStream.footerStart);
				this.DoWrite(this.type);
				this.DoWrite(ArmoredOutputStream.footerTail);
				this.DoWrite(ArmoredOutputStream.nl);
				this.outStream.Flush();
				this.type = null;
				this.start = true;
				base.Close();
			}
		}

		private void WriteHeaderEntry(string name, string v)
		{
			this.DoWrite(name + ": " + v + ArmoredOutputStream.nl);
		}

		private void DoWrite(string s)
		{
			byte[] array = Strings.ToAsciiByteArray(s);
			this.outStream.Write(array, 0, array.Length);
		}
	}
}
