using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class SkeinParameters : ICipherParameters
	{
		public class Builder
		{
			private IDictionary parameters = Platform.CreateHashtable();

			public Builder()
			{
			}

			public Builder(IDictionary paramsMap)
			{
				IEnumerator enumerator = paramsMap.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int num = (int)enumerator.Current;
					this.parameters.Add(num, paramsMap[num]);
				}
			}

			public Builder(SkeinParameters parameters)
			{
				IEnumerator enumerator = parameters.parameters.Keys.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int num = (int)enumerator.Current;
					this.parameters.Add(num, parameters.parameters[num]);
				}
			}

			public SkeinParameters.Builder Set(int type, byte[] value)
			{
				if (value == null)
				{
					throw new ArgumentException("Parameter value must not be null.");
				}
				if (type != 0 && (type <= 4 || type >= 63 || type == 48))
				{
					throw new ArgumentException("Parameter types must be in the range 0,5..47,49..62.");
				}
				if (type == 4)
				{
					throw new ArgumentException("Parameter type " + 4 + " is reserved for internal use.");
				}
				this.parameters.Add(type, value);
				return this;
			}

			public SkeinParameters.Builder SetKey(byte[] key)
			{
				return this.Set(0, key);
			}

			public SkeinParameters.Builder SetPersonalisation(byte[] personalisation)
			{
				return this.Set(8, personalisation);
			}

			public SkeinParameters.Builder SetPersonalisation(DateTime date, string emailAddress, string distinguisher)
			{
				SkeinParameters.Builder result;
				try
				{
					MemoryStream memoryStream = new MemoryStream();
					StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
					streamWriter.Write(date.ToString("YYYYMMDD"));
					streamWriter.Write(" ");
					streamWriter.Write(emailAddress);
					streamWriter.Write(" ");
					streamWriter.Write(distinguisher);
					streamWriter.Close();
					result = this.Set(8, memoryStream.ToArray());
				}
				catch (IOException innerException)
				{
					throw new InvalidOperationException("Byte I/O failed.", innerException);
				}
				return result;
			}

			public SkeinParameters.Builder SetPublicKey(byte[] publicKey)
			{
				return this.Set(12, publicKey);
			}

			public SkeinParameters.Builder SetKeyIdentifier(byte[] keyIdentifier)
			{
				return this.Set(16, keyIdentifier);
			}

			public SkeinParameters.Builder SetNonce(byte[] nonce)
			{
				return this.Set(20, nonce);
			}

			public SkeinParameters Build()
			{
				return new SkeinParameters(this.parameters);
			}
		}

		public const int PARAM_TYPE_KEY = 0;

		public const int PARAM_TYPE_CONFIG = 4;

		public const int PARAM_TYPE_PERSONALISATION = 8;

		public const int PARAM_TYPE_PUBLIC_KEY = 12;

		public const int PARAM_TYPE_KEY_IDENTIFIER = 16;

		public const int PARAM_TYPE_NONCE = 20;

		public const int PARAM_TYPE_MESSAGE = 48;

		public const int PARAM_TYPE_OUTPUT = 63;

		private IDictionary parameters;

		public SkeinParameters() : this(Platform.CreateHashtable())
		{
		}

		private SkeinParameters(IDictionary parameters)
		{
			this.parameters = parameters;
		}

		public IDictionary GetParameters()
		{
			return this.parameters;
		}

		public byte[] GetKey()
		{
			return (byte[])this.parameters[0];
		}

		public byte[] GetPersonalisation()
		{
			return (byte[])this.parameters[8];
		}

		public byte[] GetPublicKey()
		{
			return (byte[])this.parameters[12];
		}

		public byte[] GetKeyIdentifier()
		{
			return (byte[])this.parameters[16];
		}

		public byte[] GetNonce()
		{
			return (byte[])this.parameters[20];
		}
	}
}
