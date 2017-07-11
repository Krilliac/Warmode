using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public sealed class ProtocolVersion
	{
		public static readonly ProtocolVersion SSLv3 = new ProtocolVersion(768, "SSL 3.0");

		public static readonly ProtocolVersion TLSv10 = new ProtocolVersion(769, "TLS 1.0");

		public static readonly ProtocolVersion TLSv11 = new ProtocolVersion(770, "TLS 1.1");

		public static readonly ProtocolVersion TLSv12 = new ProtocolVersion(771, "TLS 1.2");

		public static readonly ProtocolVersion DTLSv10 = new ProtocolVersion(65279, "DTLS 1.0");

		public static readonly ProtocolVersion DTLSv12 = new ProtocolVersion(65277, "DTLS 1.2");

		private readonly int version;

		private readonly string name;

		public int FullVersion
		{
			get
			{
				return this.version;
			}
		}

		public int MajorVersion
		{
			get
			{
				return this.version >> 8;
			}
		}

		public int MinorVersion
		{
			get
			{
				return this.version & 255;
			}
		}

		public bool IsDtls
		{
			get
			{
				return this.MajorVersion == 254;
			}
		}

		public bool IsSsl
		{
			get
			{
				return this == ProtocolVersion.SSLv3;
			}
		}

		public bool IsTls
		{
			get
			{
				return this.MajorVersion == 3;
			}
		}

		private ProtocolVersion(int v, string name)
		{
			this.version = (v & 65535);
			this.name = name;
		}

		public ProtocolVersion GetEquivalentTLSVersion()
		{
			if (!this.IsDtls)
			{
				return this;
			}
			if (this == ProtocolVersion.DTLSv10)
			{
				return ProtocolVersion.TLSv11;
			}
			return ProtocolVersion.TLSv12;
		}

		public bool IsEqualOrEarlierVersionOf(ProtocolVersion version)
		{
			if (this.MajorVersion != version.MajorVersion)
			{
				return false;
			}
			int num = version.MinorVersion - this.MinorVersion;
			if (!this.IsDtls)
			{
				return num >= 0;
			}
			return num <= 0;
		}

		public bool IsLaterVersionOf(ProtocolVersion version)
		{
			if (this.MajorVersion != version.MajorVersion)
			{
				return false;
			}
			int num = version.MinorVersion - this.MinorVersion;
			if (!this.IsDtls)
			{
				return num < 0;
			}
			return num > 0;
		}

		public override bool Equals(object other)
		{
			return this == other || (other is ProtocolVersion && this.Equals((ProtocolVersion)other));
		}

		public bool Equals(ProtocolVersion other)
		{
			return other != null && this.version == other.version;
		}

		public override int GetHashCode()
		{
			return this.version;
		}

		public static ProtocolVersion Get(int major, int minor)
		{
			if (major != 3)
			{
				if (major != 254)
				{
					throw new TlsFatalAlert(47);
				}
				switch (minor)
				{
				case 253:
					return ProtocolVersion.DTLSv12;
				case 254:
					throw new TlsFatalAlert(47);
				case 255:
					return ProtocolVersion.DTLSv10;
				default:
					return ProtocolVersion.GetUnknownVersion(major, minor, "DTLS");
				}
			}
			else
			{
				switch (minor)
				{
				case 0:
					return ProtocolVersion.SSLv3;
				case 1:
					return ProtocolVersion.TLSv10;
				case 2:
					return ProtocolVersion.TLSv11;
				case 3:
					return ProtocolVersion.TLSv12;
				default:
					return ProtocolVersion.GetUnknownVersion(major, minor, "TLS");
				}
			}
		}

		public override string ToString()
		{
			return this.name;
		}

		private static ProtocolVersion GetUnknownVersion(int major, int minor, string prefix)
		{
			TlsUtilities.CheckUint8(major);
			TlsUtilities.CheckUint8(minor);
			int num = major << 8 | minor;
			string str = Platform.ToUpperInvariant(Convert.ToString(65536 | num, 16).Substring(1));
			return new ProtocolVersion(num, prefix + " 0x" + str);
		}
	}
}
