using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Cms
{
	public abstract class CmsPbeKey : ICipherParameters
	{
		internal readonly char[] password;

		internal readonly byte[] salt;

		internal readonly int iterationCount;

		[Obsolete("Will be removed")]
		public string Password
		{
			get
			{
				return new string(this.password);
			}
		}

		public byte[] Salt
		{
			get
			{
				return Arrays.Clone(this.salt);
			}
		}

		public int IterationCount
		{
			get
			{
				return this.iterationCount;
			}
		}

		public string Algorithm
		{
			get
			{
				return "PKCS5S2";
			}
		}

		public string Format
		{
			get
			{
				return "RAW";
			}
		}

		[Obsolete("Use version taking 'char[]' instead")]
		public CmsPbeKey(string password, byte[] salt, int iterationCount) : this(password.ToCharArray(), salt, iterationCount)
		{
		}

		[Obsolete("Use version taking 'char[]' instead")]
		public CmsPbeKey(string password, AlgorithmIdentifier keyDerivationAlgorithm) : this(password.ToCharArray(), keyDerivationAlgorithm)
		{
		}

		public CmsPbeKey(char[] password, byte[] salt, int iterationCount)
		{
			this.password = (char[])password.Clone();
			this.salt = Arrays.Clone(salt);
			this.iterationCount = iterationCount;
		}

		public CmsPbeKey(char[] password, AlgorithmIdentifier keyDerivationAlgorithm)
		{
			if (!keyDerivationAlgorithm.ObjectID.Equals(PkcsObjectIdentifiers.IdPbkdf2))
			{
				throw new ArgumentException("Unsupported key derivation algorithm: " + keyDerivationAlgorithm.ObjectID);
			}
			Pbkdf2Params instance = Pbkdf2Params.GetInstance(keyDerivationAlgorithm.Parameters.ToAsn1Object());
			this.password = (char[])password.Clone();
			this.salt = instance.GetSalt();
			this.iterationCount = instance.IterationCount.IntValue;
		}

		~CmsPbeKey()
		{
			Array.Clear(this.password, 0, this.password.Length);
		}

		[Obsolete("Use 'Salt' property instead")]
		public byte[] GetSalt()
		{
			return this.Salt;
		}

		public byte[] GetEncoded()
		{
			return null;
		}

		internal abstract KeyParameter GetEncoded(string algorithmOid);
	}
}
