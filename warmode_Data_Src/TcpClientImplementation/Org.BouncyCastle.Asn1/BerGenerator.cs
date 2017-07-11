using Org.BouncyCastle.Utilities.IO;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class BerGenerator : Asn1Generator
	{
		private bool _tagged;

		private bool _isExplicit;

		private int _tagNo;

		protected BerGenerator(Stream outStream) : base(outStream)
		{
		}

		public BerGenerator(Stream outStream, int tagNo, bool isExplicit) : base(outStream)
		{
			this._tagged = true;
			this._isExplicit = isExplicit;
			this._tagNo = tagNo;
		}

		public override void AddObject(Asn1Encodable obj)
		{
			new BerOutputStream(base.Out).WriteObject(obj);
		}

		public override Stream GetRawOutputStream()
		{
			return base.Out;
		}

		public override void Close()
		{
			this.WriteBerEnd();
		}

		private void WriteHdr(int tag)
		{
			base.Out.WriteByte((byte)tag);
			base.Out.WriteByte(128);
		}

		protected void WriteBerHeader(int tag)
		{
			if (!this._tagged)
			{
				this.WriteHdr(tag);
				return;
			}
			int num = this._tagNo | 128;
			if (this._isExplicit)
			{
				this.WriteHdr(num | 32);
				this.WriteHdr(tag);
				return;
			}
			if ((tag & 32) != 0)
			{
				this.WriteHdr(num | 32);
				return;
			}
			this.WriteHdr(num);
		}

		protected void WriteBerBody(Stream contentStream)
		{
			Streams.PipeAll(contentStream, base.Out);
		}

		protected void WriteBerEnd()
		{
			base.Out.WriteByte(0);
			base.Out.WriteByte(0);
			if (this._tagged && this._isExplicit)
			{
				base.Out.WriteByte(0);
				base.Out.WriteByte(0);
			}
		}
	}
}
