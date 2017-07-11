using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Misc;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509.Extension;
using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.X509
{
	public class X509Certificate : X509ExtensionBase
	{
		private readonly X509CertificateStructure c;

		private readonly BasicConstraints basicConstraints;

		private readonly bool[] keyUsage;

		private bool hashValueSet;

		private int hashValue;

		public virtual X509CertificateStructure CertificateStructure
		{
			get
			{
				return this.c;
			}
		}

		public virtual bool IsValidNow
		{
			get
			{
				return this.IsValid(DateTime.UtcNow);
			}
		}

		public virtual int Version
		{
			get
			{
				return this.c.Version;
			}
		}

		public virtual BigInteger SerialNumber
		{
			get
			{
				return this.c.SerialNumber.Value;
			}
		}

		public virtual X509Name IssuerDN
		{
			get
			{
				return this.c.Issuer;
			}
		}

		public virtual X509Name SubjectDN
		{
			get
			{
				return this.c.Subject;
			}
		}

		public virtual DateTime NotBefore
		{
			get
			{
				return this.c.StartDate.ToDateTime();
			}
		}

		public virtual DateTime NotAfter
		{
			get
			{
				return this.c.EndDate.ToDateTime();
			}
		}

		public virtual string SigAlgName
		{
			get
			{
				return SignerUtilities.GetEncodingName(this.c.SignatureAlgorithm.ObjectID);
			}
		}

		public virtual string SigAlgOid
		{
			get
			{
				return this.c.SignatureAlgorithm.ObjectID.Id;
			}
		}

		public virtual DerBitString IssuerUniqueID
		{
			get
			{
				return this.c.TbsCertificate.IssuerUniqueID;
			}
		}

		public virtual DerBitString SubjectUniqueID
		{
			get
			{
				return this.c.TbsCertificate.SubjectUniqueID;
			}
		}

		protected X509Certificate()
		{
		}

		public X509Certificate(X509CertificateStructure c)
		{
			this.c = c;
			try
			{
				Asn1OctetString extensionValue = this.GetExtensionValue(new DerObjectIdentifier("2.5.29.19"));
				if (extensionValue != null)
				{
					this.basicConstraints = BasicConstraints.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
				}
			}
			catch (Exception arg)
			{
				throw new CertificateParsingException("cannot construct BasicConstraints: " + arg);
			}
			try
			{
				Asn1OctetString extensionValue2 = this.GetExtensionValue(new DerObjectIdentifier("2.5.29.15"));
				if (extensionValue2 != null)
				{
					DerBitString instance = DerBitString.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
					byte[] bytes = instance.GetBytes();
					int num = bytes.Length * 8 - instance.PadBits;
					this.keyUsage = new bool[(num < 9) ? 9 : num];
					for (int num2 = 0; num2 != num; num2++)
					{
						this.keyUsage[num2] = (((int)bytes[num2 / 8] & 128 >> num2 % 8) != 0);
					}
				}
				else
				{
					this.keyUsage = null;
				}
			}
			catch (Exception arg2)
			{
				throw new CertificateParsingException("cannot construct KeyUsage: " + arg2);
			}
		}

		public virtual bool IsValid(DateTime time)
		{
			return time.CompareTo(this.NotBefore) >= 0 && time.CompareTo(this.NotAfter) <= 0;
		}

		public virtual void CheckValidity()
		{
			this.CheckValidity(DateTime.UtcNow);
		}

		public virtual void CheckValidity(DateTime time)
		{
			if (time.CompareTo(this.NotAfter) > 0)
			{
				throw new CertificateExpiredException("certificate expired on " + this.c.EndDate.GetTime());
			}
			if (time.CompareTo(this.NotBefore) < 0)
			{
				throw new CertificateNotYetValidException("certificate not valid until " + this.c.StartDate.GetTime());
			}
		}

		public virtual byte[] GetTbsCertificate()
		{
			return this.c.TbsCertificate.GetDerEncoded();
		}

		public virtual byte[] GetSignature()
		{
			return this.c.Signature.GetBytes();
		}

		public virtual byte[] GetSigAlgParams()
		{
			if (this.c.SignatureAlgorithm.Parameters != null)
			{
				return this.c.SignatureAlgorithm.Parameters.GetDerEncoded();
			}
			return null;
		}

		public virtual bool[] GetKeyUsage()
		{
			if (this.keyUsage != null)
			{
				return (bool[])this.keyUsage.Clone();
			}
			return null;
		}

		public virtual IList GetExtendedKeyUsage()
		{
			Asn1OctetString extensionValue = this.GetExtensionValue(new DerObjectIdentifier("2.5.29.37"));
			if (extensionValue == null)
			{
				return null;
			}
			IList result;
			try
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
				IList list = Platform.CreateArrayList();
				foreach (DerObjectIdentifier derObjectIdentifier in instance)
				{
					list.Add(derObjectIdentifier.Id);
				}
				result = list;
			}
			catch (Exception exception)
			{
				throw new CertificateParsingException("error processing extended key usage extension", exception);
			}
			return result;
		}

		public virtual int GetBasicConstraints()
		{
			if (this.basicConstraints == null || !this.basicConstraints.IsCA())
			{
				return -1;
			}
			if (this.basicConstraints.PathLenConstraint == null)
			{
				return 2147483647;
			}
			return this.basicConstraints.PathLenConstraint.IntValue;
		}

		public virtual ICollection GetSubjectAlternativeNames()
		{
			return this.GetAlternativeNames("2.5.29.17");
		}

		public virtual ICollection GetIssuerAlternativeNames()
		{
			return this.GetAlternativeNames("2.5.29.18");
		}

		protected virtual ICollection GetAlternativeNames(string oid)
		{
			Asn1OctetString extensionValue = this.GetExtensionValue(new DerObjectIdentifier(oid));
			if (extensionValue == null)
			{
				return null;
			}
			Asn1Object obj = X509ExtensionUtilities.FromExtensionValue(extensionValue);
			GeneralNames instance = GeneralNames.GetInstance(obj);
			IList list = Platform.CreateArrayList();
			GeneralName[] names = instance.GetNames();
			for (int i = 0; i < names.Length; i++)
			{
				GeneralName generalName = names[i];
				IList list2 = Platform.CreateArrayList();
				list2.Add(generalName.TagNo);
				list2.Add(generalName.Name.ToString());
				list.Add(list2);
			}
			return list;
		}

		protected override X509Extensions GetX509Extensions()
		{
			if (this.c.Version != 3)
			{
				return null;
			}
			return this.c.TbsCertificate.Extensions;
		}

		public virtual AsymmetricKeyParameter GetPublicKey()
		{
			return PublicKeyFactory.CreateKey(this.c.SubjectPublicKeyInfo);
		}

		public virtual byte[] GetEncoded()
		{
			return this.c.GetDerEncoded();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			X509Certificate x509Certificate = obj as X509Certificate;
			return x509Certificate != null && this.c.Equals(x509Certificate.c);
		}

		public override int GetHashCode()
		{
			lock (this)
			{
				if (!this.hashValueSet)
				{
					this.hashValue = this.c.GetHashCode();
					this.hashValueSet = true;
				}
			}
			return this.hashValue;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("  [0]         Version: ").Append(this.Version).Append(newLine);
			stringBuilder.Append("         SerialNumber: ").Append(this.SerialNumber).Append(newLine);
			stringBuilder.Append("             IssuerDN: ").Append(this.IssuerDN).Append(newLine);
			stringBuilder.Append("           Start Date: ").Append(this.NotBefore).Append(newLine);
			stringBuilder.Append("           Final Date: ").Append(this.NotAfter).Append(newLine);
			stringBuilder.Append("            SubjectDN: ").Append(this.SubjectDN).Append(newLine);
			stringBuilder.Append("           Public Key: ").Append(this.GetPublicKey()).Append(newLine);
			stringBuilder.Append("  Signature Algorithm: ").Append(this.SigAlgName).Append(newLine);
			byte[] signature = this.GetSignature();
			stringBuilder.Append("            Signature: ").Append(Hex.ToHexString(signature, 0, 20)).Append(newLine);
			for (int i = 20; i < signature.Length; i += 20)
			{
				int length = Math.Min(20, signature.Length - i);
				stringBuilder.Append("                       ").Append(Hex.ToHexString(signature, i, length)).Append(newLine);
			}
			X509Extensions extensions = this.c.TbsCertificate.Extensions;
			if (extensions != null)
			{
				IEnumerator enumerator = extensions.ExtensionOids.GetEnumerator();
				if (enumerator.MoveNext())
				{
					stringBuilder.Append("       Extensions: \n");
				}
				do
				{
					DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)enumerator.Current;
					X509Extension extension = extensions.GetExtension(derObjectIdentifier);
					if (extension.Value != null)
					{
						byte[] octets = extension.Value.GetOctets();
						Asn1Object asn1Object = Asn1Object.FromByteArray(octets);
						stringBuilder.Append("                       critical(").Append(extension.IsCritical).Append(") ");
						try
						{
							if (derObjectIdentifier.Equals(X509Extensions.BasicConstraints))
							{
								stringBuilder.Append(BasicConstraints.GetInstance(asn1Object));
							}
							else if (derObjectIdentifier.Equals(X509Extensions.KeyUsage))
							{
								stringBuilder.Append(KeyUsage.GetInstance(asn1Object));
							}
							else if (derObjectIdentifier.Equals(MiscObjectIdentifiers.NetscapeCertType))
							{
								stringBuilder.Append(new NetscapeCertType((DerBitString)asn1Object));
							}
							else if (derObjectIdentifier.Equals(MiscObjectIdentifiers.NetscapeRevocationUrl))
							{
								stringBuilder.Append(new NetscapeRevocationUrl((DerIA5String)asn1Object));
							}
							else if (derObjectIdentifier.Equals(MiscObjectIdentifiers.VerisignCzagExtension))
							{
								stringBuilder.Append(new VerisignCzagExtension((DerIA5String)asn1Object));
							}
							else
							{
								stringBuilder.Append(derObjectIdentifier.Id);
								stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object));
							}
						}
						catch (Exception)
						{
							stringBuilder.Append(derObjectIdentifier.Id);
							stringBuilder.Append(" value = ").Append("*****");
						}
					}
					stringBuilder.Append(newLine);
				}
				while (enumerator.MoveNext());
			}
			return stringBuilder.ToString();
		}

		public virtual void Verify(AsymmetricKeyParameter key)
		{
			string signatureName = X509SignatureUtilities.GetSignatureName(this.c.SignatureAlgorithm);
			ISigner signer = SignerUtilities.GetSigner(signatureName);
			this.CheckSignature(key, signer);
		}

		protected virtual void CheckSignature(AsymmetricKeyParameter publicKey, ISigner signature)
		{
			if (!X509Certificate.IsAlgIDEqual(this.c.SignatureAlgorithm, this.c.TbsCertificate.Signature))
			{
				throw new CertificateException("signature algorithm in TBS cert not same as outer cert");
			}
			Asn1Encodable parameters = this.c.SignatureAlgorithm.Parameters;
			X509SignatureUtilities.SetSignatureParameters(signature, parameters);
			signature.Init(false, publicKey);
			byte[] tbsCertificate = this.GetTbsCertificate();
			signature.BlockUpdate(tbsCertificate, 0, tbsCertificate.Length);
			byte[] signature2 = this.GetSignature();
			if (!signature.VerifySignature(signature2))
			{
				throw new InvalidKeyException("Public key presented not for certificate signature");
			}
		}

		private static bool IsAlgIDEqual(AlgorithmIdentifier id1, AlgorithmIdentifier id2)
		{
			if (!id1.ObjectID.Equals(id2.ObjectID))
			{
				return false;
			}
			Asn1Encodable parameters = id1.Parameters;
			Asn1Encodable parameters2 = id2.Parameters;
			if (parameters == null == (parameters2 == null))
			{
				return object.Equals(parameters, parameters2);
			}
			if (parameters != null)
			{
				return parameters.ToAsn1Object() is Asn1Null;
			}
			return parameters2.ToAsn1Object() is Asn1Null;
		}
	}
}
