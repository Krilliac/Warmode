using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Store;
using System;

namespace Org.BouncyCastle.X509
{
	public class AttributeCertificateHolder : IX509Selector, ICloneable
	{
		internal readonly Holder holder;

		public int DigestedObjectType
		{
			get
			{
				ObjectDigestInfo objectDigestInfo = this.holder.ObjectDigestInfo;
				if (objectDigestInfo != null)
				{
					return objectDigestInfo.DigestedObjectType.Value.IntValue;
				}
				return -1;
			}
		}

		public string DigestAlgorithm
		{
			get
			{
				ObjectDigestInfo objectDigestInfo = this.holder.ObjectDigestInfo;
				if (objectDigestInfo != null)
				{
					return objectDigestInfo.DigestAlgorithm.ObjectID.Id;
				}
				return null;
			}
		}

		public string OtherObjectTypeID
		{
			get
			{
				ObjectDigestInfo objectDigestInfo = this.holder.ObjectDigestInfo;
				if (objectDigestInfo != null)
				{
					return objectDigestInfo.OtherObjectTypeID.Id;
				}
				return null;
			}
		}

		public BigInteger SerialNumber
		{
			get
			{
				if (this.holder.BaseCertificateID != null)
				{
					return this.holder.BaseCertificateID.Serial.Value;
				}
				return null;
			}
		}

		internal AttributeCertificateHolder(Asn1Sequence seq)
		{
			this.holder = Holder.GetInstance(seq);
		}

		public AttributeCertificateHolder(X509Name issuerName, BigInteger serialNumber)
		{
			this.holder = new Holder(new IssuerSerial(this.GenerateGeneralNames(issuerName), new DerInteger(serialNumber)));
		}

		public AttributeCertificateHolder(X509Certificate cert)
		{
			X509Name issuerX509Principal;
			try
			{
				issuerX509Principal = PrincipalUtilities.GetIssuerX509Principal(cert);
			}
			catch (Exception ex)
			{
				throw new CertificateParsingException(ex.Message);
			}
			this.holder = new Holder(new IssuerSerial(this.GenerateGeneralNames(issuerX509Principal), new DerInteger(cert.SerialNumber)));
		}

		public AttributeCertificateHolder(X509Name principal)
		{
			this.holder = new Holder(this.GenerateGeneralNames(principal));
		}

		public AttributeCertificateHolder(int digestedObjectType, string digestAlgorithm, string otherObjectTypeID, byte[] objectDigest)
		{
			this.holder = new Holder(new ObjectDigestInfo(digestedObjectType, otherObjectTypeID, new AlgorithmIdentifier(digestAlgorithm), Arrays.Clone(objectDigest)));
		}

		public byte[] GetObjectDigest()
		{
			ObjectDigestInfo objectDigestInfo = this.holder.ObjectDigestInfo;
			if (objectDigestInfo != null)
			{
				return objectDigestInfo.ObjectDigest.GetBytes();
			}
			return null;
		}

		private GeneralNames GenerateGeneralNames(X509Name principal)
		{
			return new GeneralNames(new GeneralName(principal));
		}

		private bool MatchesDN(X509Name subject, GeneralNames targets)
		{
			GeneralName[] names = targets.GetNames();
			for (int num = 0; num != names.Length; num++)
			{
				GeneralName generalName = names[num];
				if (generalName.TagNo == 4)
				{
					try
					{
						if (X509Name.GetInstance(generalName.Name).Equivalent(subject))
						{
							return true;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return false;
		}

		private object[] GetNames(GeneralName[] names)
		{
			int num = 0;
			for (int num2 = 0; num2 != names.Length; num2++)
			{
				if (names[num2].TagNo == 4)
				{
					num++;
				}
			}
			object[] array = new object[num];
			int num3 = 0;
			for (int num4 = 0; num4 != names.Length; num4++)
			{
				if (names[num4].TagNo == 4)
				{
					array[num3++] = X509Name.GetInstance(names[num4].Name);
				}
			}
			return array;
		}

		private X509Name[] GetPrincipals(GeneralNames names)
		{
			object[] names2 = this.GetNames(names.GetNames());
			int num = 0;
			for (int num2 = 0; num2 != names2.Length; num2++)
			{
				if (names2[num2] is X509Name)
				{
					num++;
				}
			}
			X509Name[] array = new X509Name[num];
			int num3 = 0;
			for (int num4 = 0; num4 != names2.Length; num4++)
			{
				if (names2[num4] is X509Name)
				{
					array[num3++] = (X509Name)names2[num4];
				}
			}
			return array;
		}

		public X509Name[] GetEntityNames()
		{
			if (this.holder.EntityName != null)
			{
				return this.GetPrincipals(this.holder.EntityName);
			}
			return null;
		}

		public X509Name[] GetIssuer()
		{
			if (this.holder.BaseCertificateID != null)
			{
				return this.GetPrincipals(this.holder.BaseCertificateID.Issuer);
			}
			return null;
		}

		public object Clone()
		{
			return new AttributeCertificateHolder((Asn1Sequence)this.holder.ToAsn1Object());
		}

		public bool Match(X509Certificate x509Cert)
		{
			try
			{
				if (this.holder.BaseCertificateID != null)
				{
					bool result = this.holder.BaseCertificateID.Serial.Value.Equals(x509Cert.SerialNumber) && this.MatchesDN(PrincipalUtilities.GetIssuerX509Principal(x509Cert), this.holder.BaseCertificateID.Issuer);
					return result;
				}
				if (this.holder.EntityName != null && this.MatchesDN(PrincipalUtilities.GetSubjectX509Principal(x509Cert), this.holder.EntityName))
				{
					bool result = true;
					return result;
				}
				if (this.holder.ObjectDigestInfo != null)
				{
					IDigest digest = null;
					try
					{
						digest = DigestUtilities.GetDigest(this.DigestAlgorithm);
					}
					catch (Exception)
					{
						bool result = false;
						return result;
					}
					switch (this.DigestedObjectType)
					{
					case 0:
					{
						byte[] encoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(x509Cert.GetPublicKey()).GetEncoded();
						digest.BlockUpdate(encoded, 0, encoded.Length);
						break;
					}
					case 1:
					{
						byte[] encoded2 = x509Cert.GetEncoded();
						digest.BlockUpdate(encoded2, 0, encoded2.Length);
						break;
					}
					}
					if (!Arrays.AreEqual(DigestUtilities.DoFinal(digest), this.GetObjectDigest()))
					{
						bool result = false;
						return result;
					}
				}
			}
			catch (CertificateEncodingException)
			{
				bool result = false;
				return result;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is AttributeCertificateHolder))
			{
				return false;
			}
			AttributeCertificateHolder attributeCertificateHolder = (AttributeCertificateHolder)obj;
			return this.holder.Equals(attributeCertificateHolder.holder);
		}

		public override int GetHashCode()
		{
			return this.holder.GetHashCode();
		}

		public bool Match(object obj)
		{
			return obj is X509Certificate && this.Match((X509Certificate)obj);
		}
	}
}
