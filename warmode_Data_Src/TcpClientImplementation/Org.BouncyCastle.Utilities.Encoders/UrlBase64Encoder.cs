using System;

namespace Org.BouncyCastle.Utilities.Encoders
{
	public class UrlBase64Encoder : Base64Encoder
	{
		public UrlBase64Encoder()
		{
			this.encodingTable[this.encodingTable.Length - 2] = 45;
			this.encodingTable[this.encodingTable.Length - 1] = 95;
			this.padding = 46;
			base.InitialiseDecodingTable();
		}
	}
}
