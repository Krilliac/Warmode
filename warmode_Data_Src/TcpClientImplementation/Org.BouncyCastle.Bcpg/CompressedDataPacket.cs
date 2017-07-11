using System;

namespace Org.BouncyCastle.Bcpg
{
	public class CompressedDataPacket : InputStreamPacket
	{
		private readonly CompressionAlgorithmTag algorithm;

		public CompressionAlgorithmTag Algorithm
		{
			get
			{
				return this.algorithm;
			}
		}

		internal CompressedDataPacket(BcpgInputStream bcpgIn) : base(bcpgIn)
		{
			this.algorithm = (CompressionAlgorithmTag)bcpgIn.ReadByte();
		}
	}
}
