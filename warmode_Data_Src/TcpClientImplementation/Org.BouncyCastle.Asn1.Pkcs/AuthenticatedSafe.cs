using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class AuthenticatedSafe : Asn1Encodable
	{
		private readonly ContentInfo[] info;

		public AuthenticatedSafe(Asn1Sequence seq)
		{
			this.info = new ContentInfo[seq.Count];
			for (int num = 0; num != this.info.Length; num++)
			{
				this.info[num] = ContentInfo.GetInstance(seq[num]);
			}
		}

		public AuthenticatedSafe(ContentInfo[] info)
		{
			this.info = (ContentInfo[])info.Clone();
		}

		public ContentInfo[] GetContentInfo()
		{
			return (ContentInfo[])this.info.Clone();
		}

		public override Asn1Object ToAsn1Object()
		{
			return new BerSequence(this.info);
		}
	}
}
