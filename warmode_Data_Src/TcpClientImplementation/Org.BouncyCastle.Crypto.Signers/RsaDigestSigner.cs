using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class RsaDigestSigner : ISigner
	{
		private readonly IAsymmetricBlockCipher rsaEngine = new Pkcs1Encoding(new RsaBlindedEngine());

		private readonly AlgorithmIdentifier algId;

		private readonly IDigest digest;

		private bool forSigning;

		private static readonly IDictionary oidMap;

		public virtual string AlgorithmName
		{
			get
			{
				return this.digest.AlgorithmName + "withRSA";
			}
		}

		static RsaDigestSigner()
		{
			RsaDigestSigner.oidMap = Platform.CreateHashtable();
			RsaDigestSigner.oidMap["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
			RsaDigestSigner.oidMap["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
			RsaDigestSigner.oidMap["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
			RsaDigestSigner.oidMap["SHA-1"] = X509ObjectIdentifiers.IdSha1;
			RsaDigestSigner.oidMap["SHA-224"] = NistObjectIdentifiers.IdSha224;
			RsaDigestSigner.oidMap["SHA-256"] = NistObjectIdentifiers.IdSha256;
			RsaDigestSigner.oidMap["SHA-384"] = NistObjectIdentifiers.IdSha384;
			RsaDigestSigner.oidMap["SHA-512"] = NistObjectIdentifiers.IdSha512;
			RsaDigestSigner.oidMap["MD2"] = PkcsObjectIdentifiers.MD2;
			RsaDigestSigner.oidMap["MD4"] = PkcsObjectIdentifiers.MD4;
			RsaDigestSigner.oidMap["MD5"] = PkcsObjectIdentifiers.MD5;
		}

		public RsaDigestSigner(IDigest digest) : this(digest, (DerObjectIdentifier)RsaDigestSigner.oidMap[digest.AlgorithmName])
		{
		}

		public RsaDigestSigner(IDigest digest, DerObjectIdentifier digestOid) : this(digest, new AlgorithmIdentifier(digestOid, DerNull.Instance))
		{
		}

		public RsaDigestSigner(IDigest digest, AlgorithmIdentifier algId)
		{
			this.digest = digest;
			this.algId = algId;
		}

		public virtual void Init(bool forSigning, ICipherParameters parameters)
		{
			this.forSigning = forSigning;
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				asymmetricKeyParameter = (AsymmetricKeyParameter)((ParametersWithRandom)parameters).Parameters;
			}
			else
			{
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			if (forSigning && !asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Signing requires private key.");
			}
			if (!forSigning && asymmetricKeyParameter.IsPrivate)
			{
				throw new InvalidKeyException("Verification requires public key.");
			}
			this.Reset();
			this.rsaEngine.Init(forSigning, parameters);
		}

		public virtual void Update(byte input)
		{
			this.digest.Update(input);
		}

		public virtual void BlockUpdate(byte[] input, int inOff, int length)
		{
			this.digest.BlockUpdate(input, inOff, length);
		}

		public virtual byte[] GenerateSignature()
		{
			if (!this.forSigning)
			{
				throw new InvalidOperationException("RsaDigestSigner not initialised for signature generation.");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			byte[] array2 = this.DerEncode(array);
			return this.rsaEngine.ProcessBlock(array2, 0, array2.Length);
		}

		public virtual bool VerifySignature(byte[] signature)
		{
			if (this.forSigning)
			{
				throw new InvalidOperationException("RsaDigestSigner not initialised for verification");
			}
			byte[] array = new byte[this.digest.GetDigestSize()];
			this.digest.DoFinal(array, 0);
			byte[] array2;
			byte[] array3;
			try
			{
				array2 = this.rsaEngine.ProcessBlock(signature, 0, signature.Length);
				array3 = this.DerEncode(array);
			}
			catch (Exception)
			{
				bool result = false;
				return result;
			}
			if (array2.Length == array3.Length)
			{
				return Arrays.ConstantTimeAreEqual(array2, array3);
			}
			if (array2.Length == array3.Length - 2)
			{
				int num = array2.Length - array.Length - 2;
				int num2 = array3.Length - array.Length - 2;
				byte[] expr_8F_cp_0 = array3;
				int expr_8F_cp_1 = 1;
				expr_8F_cp_0[expr_8F_cp_1] -= 2;
				byte[] expr_A4_cp_0 = array3;
				int expr_A4_cp_1 = 3;
				expr_A4_cp_0[expr_A4_cp_1] -= 2;
				int num3 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					num3 |= (int)(array2[num + i] ^ array3[num2 + i]);
				}
				for (int j = 0; j < num; j++)
				{
					num3 |= (int)(array2[j] ^ array3[j]);
				}
				return num3 == 0;
			}
			return false;
		}

		public virtual void Reset()
		{
			this.digest.Reset();
		}

		private byte[] DerEncode(byte[] hash)
		{
			if (this.algId == null)
			{
				return hash;
			}
			DigestInfo digestInfo = new DigestInfo(this.algId, hash);
			return digestInfo.GetDerEncoded();
		}
	}
}
