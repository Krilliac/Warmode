using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemWriter
	{
		private const int LineLength = 64;

		private readonly TextWriter writer;

		private readonly int nlLength;

		private char[] buf = new char[64];

		public TextWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		public PemWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.nlLength = Platform.NewLine.Length;
		}

		public int GetOutputSize(PemObject obj)
		{
			int num = 2 * (obj.Type.Length + 10 + this.nlLength) + 6 + 4;
			if (obj.Headers.Count > 0)
			{
				foreach (PemHeader pemHeader in obj.Headers)
				{
					num += pemHeader.Name.Length + ": ".Length + pemHeader.Value.Length + this.nlLength;
				}
				num += this.nlLength;
			}
			int num2 = (obj.Content.Length + 2) / 3 * 4;
			num += num2 + (num2 + 64 - 1) / 64 * this.nlLength;
			return num;
		}

		public void WriteObject(PemObjectGenerator objGen)
		{
			PemObject pemObject = objGen.Generate();
			this.WritePreEncapsulationBoundary(pemObject.Type);
			if (pemObject.Headers.Count > 0)
			{
				foreach (PemHeader pemHeader in pemObject.Headers)
				{
					this.writer.Write(pemHeader.Name);
					this.writer.Write(": ");
					this.writer.WriteLine(pemHeader.Value);
				}
				this.writer.WriteLine();
			}
			this.WriteEncoded(pemObject.Content);
			this.WritePostEncapsulationBoundary(pemObject.Type);
		}

		private void WriteEncoded(byte[] bytes)
		{
			bytes = Base64.Encode(bytes);
			for (int i = 0; i < bytes.Length; i += this.buf.Length)
			{
				int num = 0;
				while (num != this.buf.Length && i + num < bytes.Length)
				{
					this.buf[num] = (char)bytes[i + num];
					num++;
				}
				this.writer.WriteLine(this.buf, 0, num);
			}
		}

		private void WritePreEncapsulationBoundary(string type)
		{
			this.writer.WriteLine("-----BEGIN " + type + "-----");
		}

		private void WritePostEncapsulationBoundary(string type)
		{
			this.writer.WriteLine("-----END " + type + "-----");
		}
	}
}
