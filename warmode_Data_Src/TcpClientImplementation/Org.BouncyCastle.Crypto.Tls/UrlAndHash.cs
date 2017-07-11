using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class UrlAndHash
	{
		protected readonly string mUrl;

		protected readonly byte[] mSha1Hash;

		public virtual string Url
		{
			get
			{
				return this.mUrl;
			}
		}

		public virtual byte[] Sha1Hash
		{
			get
			{
				return this.mSha1Hash;
			}
		}

		public UrlAndHash(string url, byte[] sha1Hash)
		{
			if (url == null || url.Length < 1 || url.Length >= 65536)
			{
				throw new ArgumentException("must have length from 1 to (2^16 - 1)", "url");
			}
			if (sha1Hash != null && sha1Hash.Length != 20)
			{
				throw new ArgumentException("must have length == 20, if present", "sha1Hash");
			}
			this.mUrl = url;
			this.mSha1Hash = sha1Hash;
		}

		public virtual void Encode(Stream output)
		{
			byte[] buf = Strings.ToByteArray(this.mUrl);
			TlsUtilities.WriteOpaque16(buf, output);
			if (this.mSha1Hash == null)
			{
				TlsUtilities.WriteUint8(0, output);
				return;
			}
			TlsUtilities.WriteUint8(1, output);
			output.Write(this.mSha1Hash, 0, this.mSha1Hash.Length);
		}

		public static UrlAndHash Parse(TlsContext context, Stream input)
		{
			byte[] array = TlsUtilities.ReadOpaque16(input);
			if (array.Length < 1)
			{
				throw new TlsFatalAlert(47);
			}
			string url = Strings.FromByteArray(array);
			byte[] sha1Hash = null;
			switch (TlsUtilities.ReadUint8(input))
			{
			case 0:
				if (TlsUtilities.IsTlsV12(context))
				{
					throw new TlsFatalAlert(47);
				}
				break;
			case 1:
				sha1Hash = TlsUtilities.ReadFully(20, input);
				break;
			default:
				throw new TlsFatalAlert(47);
			}
			return new UrlAndHash(url, sha1Hash);
		}
	}
}
