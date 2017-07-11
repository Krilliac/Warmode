using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.OpenSsl
{
	public class PemReader : Org.BouncyCastle.Utilities.IO.Pem.PemReader
	{
		private readonly IPasswordFinder pFinder;

		static PemReader()
		{
		}

		public PemReader(TextReader reader) : this(reader, null)
		{
		}

		public PemReader(TextReader reader, IPasswordFinder pFinder) : base(reader)
		{
			this.pFinder = pFinder;
		}

		public object ReadObject()
		{
			PemObject pemObject = base.ReadPemObject();
			if (pemObject == null)
			{
				return null;
			}
			if (pemObject.Type.EndsWith("PRIVATE KEY"))
			{
				return this.ReadPrivateKey(pemObject);
			}
			string type;
			switch (type = pemObject.Type)
			{
			case "PUBLIC KEY":
				return this.ReadPublicKey(pemObject);
			case "RSA PUBLIC KEY":
				return this.ReadRsaPublicKey(pemObject);
			case "CERTIFICATE REQUEST":
			case "NEW CERTIFICATE REQUEST":
				return this.ReadCertificateRequest(pemObject);
			case "CERTIFICATE":
			case "X509 CERTIFICATE":
				return this.ReadCertificate(pemObject);
			case "PKCS7":
				return this.ReadPkcs7(pemObject);
			case "X509 CRL":
				return this.ReadCrl(pemObject);
			case "ATTRIBUTE CERTIFICATE":
				return this.ReadAttributeCertificate(pemObject);
			}
			throw new IOException("unrecognised object: " + pemObject.Type);
		}

		private AsymmetricKeyParameter ReadRsaPublicKey(PemObject pemObject)
		{
			RsaPublicKeyStructure instance = RsaPublicKeyStructure.GetInstance(Asn1Object.FromByteArray(pemObject.Content));
			return new RsaKeyParameters(false, instance.Modulus, instance.PublicExponent);
		}

		private AsymmetricKeyParameter ReadPublicKey(PemObject pemObject)
		{
			return PublicKeyFactory.CreateKey(pemObject.Content);
		}

		private X509Certificate ReadCertificate(PemObject pemObject)
		{
			X509Certificate result;
			try
			{
				result = new X509CertificateParser().ReadCertificate(pemObject.Content);
			}
			catch (Exception ex)
			{
				throw new PemException("problem parsing cert: " + ex.ToString());
			}
			return result;
		}

		private X509Crl ReadCrl(PemObject pemObject)
		{
			X509Crl result;
			try
			{
				result = new X509CrlParser().ReadCrl(pemObject.Content);
			}
			catch (Exception ex)
			{
				throw new PemException("problem parsing cert: " + ex.ToString());
			}
			return result;
		}

		private Pkcs10CertificationRequest ReadCertificateRequest(PemObject pemObject)
		{
			Pkcs10CertificationRequest result;
			try
			{
				result = new Pkcs10CertificationRequest(pemObject.Content);
			}
			catch (Exception ex)
			{
				throw new PemException("problem parsing cert: " + ex.ToString());
			}
			return result;
		}

		private IX509AttributeCertificate ReadAttributeCertificate(PemObject pemObject)
		{
			return new X509V2AttributeCertificate(pemObject.Content);
		}

		private Org.BouncyCastle.Asn1.Cms.ContentInfo ReadPkcs7(PemObject pemObject)
		{
			Org.BouncyCastle.Asn1.Cms.ContentInfo instance;
			try
			{
				instance = Org.BouncyCastle.Asn1.Cms.ContentInfo.GetInstance(Asn1Object.FromByteArray(pemObject.Content));
			}
			catch (Exception ex)
			{
				throw new PemException("problem parsing PKCS7 object: " + ex.ToString());
			}
			return instance;
		}

		private object ReadPrivateKey(PemObject pemObject)
		{
			string text = pemObject.Type.Substring(0, pemObject.Type.Length - "PRIVATE KEY".Length).Trim();
			byte[] array = pemObject.Content;
			IDictionary dictionary = Platform.CreateHashtable();
			foreach (PemHeader pemHeader in pemObject.Headers)
			{
				dictionary[pemHeader.Name] = pemHeader.Value;
			}
			string a = (string)dictionary["Proc-Type"];
			if (a == "4,ENCRYPTED")
			{
				if (this.pFinder == null)
				{
					throw new PasswordException("No password finder specified, but a password is required");
				}
				char[] password = this.pFinder.GetPassword();
				if (password == null)
				{
					throw new PasswordException("Password is null, but a password is required");
				}
				string text2 = (string)dictionary["DEK-Info"];
				string[] array2 = text2.Split(new char[]
				{
					','
				});
				string dekAlgName = array2[0].Trim();
				byte[] iv = Hex.Decode(array2[1].Trim());
				array = PemUtilities.Crypt(false, array, password, dekAlgName, iv);
			}
			object result;
			try
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(array);
				string a2;
				if ((a2 = text) != null)
				{
					AsymmetricKeyParameter asymmetricKeyParameter;
					AsymmetricKeyParameter publicParameter;
					if (!(a2 == "RSA"))
					{
						if (!(a2 == "DSA"))
						{
							if (!(a2 == "EC"))
							{
								if (!(a2 == "ENCRYPTED"))
								{
									if (!(a2 == ""))
									{
										goto IL_356;
									}
									result = PrivateKeyFactory.CreateKey(PrivateKeyInfo.GetInstance(instance));
									return result;
								}
								else
								{
									char[] password2 = this.pFinder.GetPassword();
									if (password2 == null)
									{
										throw new PasswordException("Password is null, but a password is required");
									}
									result = PrivateKeyFactory.DecryptKey(password2, EncryptedPrivateKeyInfo.GetInstance(instance));
									return result;
								}
							}
							else
							{
								ECPrivateKeyStructure eCPrivateKeyStructure = new ECPrivateKeyStructure(instance);
								AlgorithmIdentifier algID = new AlgorithmIdentifier(X9ObjectIdentifiers.IdECPublicKey, eCPrivateKeyStructure.GetParameters());
								PrivateKeyInfo keyInfo = new PrivateKeyInfo(algID, eCPrivateKeyStructure.ToAsn1Object());
								asymmetricKeyParameter = PrivateKeyFactory.CreateKey(keyInfo);
								DerBitString publicKey = eCPrivateKeyStructure.GetPublicKey();
								if (publicKey != null)
								{
									SubjectPublicKeyInfo keyInfo2 = new SubjectPublicKeyInfo(algID, publicKey.GetBytes());
									publicParameter = PublicKeyFactory.CreateKey(keyInfo2);
								}
								else
								{
									publicParameter = ECKeyPairGenerator.GetCorrespondingPublicKey((ECPrivateKeyParameters)asymmetricKeyParameter);
								}
							}
						}
						else
						{
							if (instance.Count != 6)
							{
								throw new PemException("malformed sequence in DSA private key");
							}
							DerInteger derInteger = (DerInteger)instance[1];
							DerInteger derInteger2 = (DerInteger)instance[2];
							DerInteger derInteger3 = (DerInteger)instance[3];
							DerInteger derInteger4 = (DerInteger)instance[4];
							DerInteger derInteger5 = (DerInteger)instance[5];
							DsaParameters parameters = new DsaParameters(derInteger.Value, derInteger2.Value, derInteger3.Value);
							asymmetricKeyParameter = new DsaPrivateKeyParameters(derInteger5.Value, parameters);
							publicParameter = new DsaPublicKeyParameters(derInteger4.Value, parameters);
						}
					}
					else
					{
						if (instance.Count != 9)
						{
							throw new PemException("malformed sequence in RSA private key");
						}
						RsaPrivateKeyStructure instance2 = RsaPrivateKeyStructure.GetInstance(instance);
						publicParameter = new RsaKeyParameters(false, instance2.Modulus, instance2.PublicExponent);
						asymmetricKeyParameter = new RsaPrivateCrtKeyParameters(instance2.Modulus, instance2.PublicExponent, instance2.PrivateExponent, instance2.Prime1, instance2.Prime2, instance2.Exponent1, instance2.Exponent2, instance2.Coefficient);
					}
					result = new AsymmetricCipherKeyPair(publicParameter, asymmetricKeyParameter);
					return result;
				}
				IL_356:
				throw new ArgumentException("Unknown key type: " + text, "type");
			}
			catch (IOException ex)
			{
				throw ex;
			}
			catch (Exception ex2)
			{
				throw new PemException("problem creating " + text + " private key: " + ex2.ToString());
			}
			return result;
		}

		private static X9ECParameters GetCurveParameters(string name)
		{
			X9ECParameters byName = CustomNamedCurves.GetByName(name);
			if (byName == null)
			{
				byName = ECNamedCurveTable.GetByName(name);
			}
			if (byName == null)
			{
				throw new Exception("unknown curve name: " + name);
			}
			return byName;
		}
	}
}
