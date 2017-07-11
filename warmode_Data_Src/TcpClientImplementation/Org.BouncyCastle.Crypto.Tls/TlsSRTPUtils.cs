using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public abstract class TlsSRTPUtils
	{
		public static void AddUseSrtpExtension(IDictionary extensions, UseSrtpData useSRTPData)
		{
			extensions[14] = TlsSRTPUtils.CreateUseSrtpExtension(useSRTPData);
		}

		public static UseSrtpData GetUseSrtpExtension(IDictionary extensions)
		{
			byte[] extensionData = TlsUtilities.GetExtensionData(extensions, 14);
			if (extensionData != null)
			{
				return TlsSRTPUtils.ReadUseSrtpExtension(extensionData);
			}
			return null;
		}

		public static byte[] CreateUseSrtpExtension(UseSrtpData useSrtpData)
		{
			if (useSrtpData == null)
			{
				throw new ArgumentNullException("useSrtpData");
			}
			MemoryStream memoryStream = new MemoryStream();
			TlsUtilities.WriteUint16ArrayWithUint16Length(useSrtpData.ProtectionProfiles, memoryStream);
			TlsUtilities.WriteOpaque8(useSrtpData.Mki, memoryStream);
			return memoryStream.ToArray();
		}

		public static UseSrtpData ReadUseSrtpExtension(byte[] extensionData)
		{
			if (extensionData == null)
			{
				throw new ArgumentNullException("extensionData");
			}
			MemoryStream memoryStream = new MemoryStream(extensionData, true);
			int num = TlsUtilities.ReadUint16(memoryStream);
			if (num < 2 || (num & 1) != 0)
			{
				throw new TlsFatalAlert(50);
			}
			int[] protectionProfiles = TlsUtilities.ReadUint16Array(num / 2, memoryStream);
			byte[] mki = TlsUtilities.ReadOpaque8(memoryStream);
			TlsProtocol.AssertEmpty(memoryStream);
			return new UseSrtpData(protectionProfiles, mki);
		}
	}
}
