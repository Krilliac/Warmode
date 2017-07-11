using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class ServerName
	{
		protected readonly byte mNameType;

		protected readonly object mName;

		public virtual byte NameType
		{
			get
			{
				return this.mNameType;
			}
		}

		public virtual object Name
		{
			get
			{
				return this.mName;
			}
		}

		public ServerName(byte nameType, object name)
		{
			if (!ServerName.IsCorrectType(nameType, name))
			{
				throw new ArgumentException("not an instance of the correct type", "name");
			}
			this.mNameType = nameType;
			this.mName = name;
		}

		public virtual string GetHostName()
		{
			if (!ServerName.IsCorrectType(0, this.mName))
			{
				throw new InvalidOperationException("'name' is not a HostName string");
			}
			return (string)this.mName;
		}

		public virtual void Encode(Stream output)
		{
			TlsUtilities.WriteUint8(this.mNameType, output);
			byte b = this.mNameType;
			if (b != 0)
			{
				throw new TlsFatalAlert(80);
			}
			byte[] array = Strings.ToUtf8ByteArray((string)this.mName);
			if (array.Length < 1)
			{
				throw new TlsFatalAlert(80);
			}
			TlsUtilities.WriteOpaque16(array, output);
		}

		public static ServerName Parse(Stream input)
		{
			byte b = TlsUtilities.ReadUint8(input);
			byte b2 = b;
			if (b2 != 0)
			{
				throw new TlsFatalAlert(50);
			}
			byte[] array = TlsUtilities.ReadOpaque16(input);
			if (array.Length < 1)
			{
				throw new TlsFatalAlert(50);
			}
			object name = Strings.FromUtf8ByteArray(array);
			return new ServerName(b, name);
		}

		protected static bool IsCorrectType(byte nameType, object name)
		{
			if (nameType == 0)
			{
				return name is string;
			}
			throw new ArgumentException("unsupported value", "name");
		}
	}
}
