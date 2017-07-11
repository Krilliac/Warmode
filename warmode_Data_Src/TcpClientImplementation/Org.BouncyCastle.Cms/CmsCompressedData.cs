using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsCompressedData
	{
		internal ContentInfo contentInfo;

		public ContentInfo ContentInfo
		{
			get
			{
				return this.contentInfo;
			}
		}

		public CmsCompressedData(byte[] compressedData) : this(CmsUtilities.ReadContentInfo(compressedData))
		{
		}

		public CmsCompressedData(Stream compressedDataStream) : this(CmsUtilities.ReadContentInfo(compressedDataStream))
		{
		}

		public CmsCompressedData(ContentInfo contentInfo)
		{
			this.contentInfo = contentInfo;
		}

		public byte[] GetContent()
		{
			CompressedData instance = CompressedData.GetInstance(this.contentInfo.Content);
			ContentInfo encapContentInfo = instance.EncapContentInfo;
			Asn1OctetString asn1OctetString = (Asn1OctetString)encapContentInfo.Content;
			ZInputStream zInputStream = new ZInputStream(asn1OctetString.GetOctetStream());
			byte[] result;
			try
			{
				result = CmsUtilities.StreamToByteArray(zInputStream);
			}
			catch (IOException e)
			{
				throw new CmsException("exception reading compressed stream.", e);
			}
			finally
			{
				zInputStream.Close();
			}
			return result;
		}

		public byte[] GetContent(int limit)
		{
			CompressedData instance = CompressedData.GetInstance(this.contentInfo.Content);
			ContentInfo encapContentInfo = instance.EncapContentInfo;
			Asn1OctetString asn1OctetString = (Asn1OctetString)encapContentInfo.Content;
			ZInputStream inStream = new ZInputStream(new MemoryStream(asn1OctetString.GetOctets(), false));
			byte[] result;
			try
			{
				result = CmsUtilities.StreamToByteArray(inStream, limit);
			}
			catch (IOException e)
			{
				throw new CmsException("exception reading compressed stream.", e);
			}
			return result;
		}

		public byte[] GetEncoded()
		{
			return this.contentInfo.GetEncoded();
		}
	}
}
