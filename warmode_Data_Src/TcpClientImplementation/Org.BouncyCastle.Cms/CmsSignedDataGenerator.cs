using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsSignedDataGenerator : CmsSignedGenerator
	{
		private class SignerInf
		{
			private readonly CmsSignedGenerator outer;

			private readonly AsymmetricKeyParameter key;

			private readonly SignerIdentifier signerIdentifier;

			private readonly string digestOID;

			private readonly string encOID;

			private readonly CmsAttributeTableGenerator sAttr;

			private readonly CmsAttributeTableGenerator unsAttr;

			private readonly Org.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable;

			internal AlgorithmIdentifier DigestAlgorithmID
			{
				get
				{
					return new AlgorithmIdentifier(new DerObjectIdentifier(this.digestOID), DerNull.Instance);
				}
			}

			internal CmsAttributeTableGenerator SignedAttributes
			{
				get
				{
					return this.sAttr;
				}
			}

			internal CmsAttributeTableGenerator UnsignedAttributes
			{
				get
				{
					return this.unsAttr;
				}
			}

			internal SignerInf(CmsSignedGenerator outer, AsymmetricKeyParameter key, SignerIdentifier signerIdentifier, string digestOID, string encOID, CmsAttributeTableGenerator sAttr, CmsAttributeTableGenerator unsAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable)
			{
				this.outer = outer;
				this.key = key;
				this.signerIdentifier = signerIdentifier;
				this.digestOID = digestOID;
				this.encOID = encOID;
				this.sAttr = sAttr;
				this.unsAttr = unsAttr;
				this.baseSignedTable = baseSignedTable;
			}

			internal SignerInfo ToSignerInfo(DerObjectIdentifier contentType, CmsProcessable content, SecureRandom random)
			{
				AlgorithmIdentifier digestAlgorithmID = this.DigestAlgorithmID;
				string digestAlgName = CmsSignedDataGenerator.Helper.GetDigestAlgName(this.digestOID);
				IDigest digestInstance = CmsSignedDataGenerator.Helper.GetDigestInstance(digestAlgName);
				string algorithm = digestAlgName + "with" + CmsSignedDataGenerator.Helper.GetEncryptionAlgName(this.encOID);
				ISigner signatureInstance = CmsSignedDataGenerator.Helper.GetSignatureInstance(algorithm);
				if (content != null)
				{
					content.Write(new DigOutputStream(digestInstance));
				}
				byte[] array = DigestUtilities.DoFinal(digestInstance);
				this.outer._digests.Add(this.digestOID, array.Clone());
				signatureInstance.Init(true, new ParametersWithRandom(this.key, random));
				Stream stream = new BufferedStream(new SigOutputStream(signatureInstance));
				Asn1Set asn1Set = null;
				if (this.sAttr != null)
				{
					IDictionary baseParameters = this.outer.GetBaseParameters(contentType, digestAlgorithmID, array);
					Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable = this.sAttr.GetAttributes(baseParameters);
					if (contentType == null && attributeTable != null && attributeTable[CmsAttributes.ContentType] != null)
					{
						IDictionary dictionary = attributeTable.ToDictionary();
						dictionary.Remove(CmsAttributes.ContentType);
						attributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(dictionary);
					}
					asn1Set = this.outer.GetAttributeSet(attributeTable);
					new DerOutputStream(stream).WriteObject(asn1Set);
				}
				else if (content != null)
				{
					content.Write(stream);
				}
				stream.Close();
				byte[] array2 = signatureInstance.GenerateSignature();
				Asn1Set unauthenticatedAttributes = null;
				if (this.unsAttr != null)
				{
					IDictionary baseParameters2 = this.outer.GetBaseParameters(contentType, digestAlgorithmID, array);
					baseParameters2[CmsAttributeTableParameter.Signature] = array2.Clone();
					Org.BouncyCastle.Asn1.Cms.AttributeTable attributes = this.unsAttr.GetAttributes(baseParameters2);
					unauthenticatedAttributes = this.outer.GetAttributeSet(attributes);
				}
				Asn1Encodable defaultX509Parameters = SignerUtilities.GetDefaultX509Parameters(algorithm);
				AlgorithmIdentifier encAlgorithmIdentifier = CmsSignedGenerator.GetEncAlgorithmIdentifier(new DerObjectIdentifier(this.encOID), defaultX509Parameters);
				return new SignerInfo(this.signerIdentifier, digestAlgorithmID, asn1Set, encAlgorithmIdentifier, new DerOctetString(array2), unauthenticatedAttributes);
			}
		}

		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private readonly IList signerInfs = Platform.CreateArrayList();

		public CmsSignedDataGenerator()
		{
		}

		public CmsSignedDataGenerator(SecureRandom rand) : base(rand)
		{
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOID)
		{
			this.AddSigner(privateKey, cert, base.GetEncOid(privateKey, digestOID), digestOID);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOID, string digestOID)
		{
			this.doAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), encryptionOID, digestOID, new DefaultSignedAttributeTableGenerator(), null, null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOID)
		{
			this.AddSigner(privateKey, subjectKeyID, base.GetEncOid(privateKey, digestOID), digestOID);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOID, string digestOID)
		{
			this.doAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), encryptionOID, digestOID, new DefaultSignedAttributeTableGenerator(), null, null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOID, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.AddSigner(privateKey, cert, base.GetEncOid(privateKey, digestOID), digestOID, signedAttr, unsignedAttr);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOID, string digestOID, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.doAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), encryptionOID, digestOID, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), signedAttr);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOID, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.AddSigner(privateKey, subjectKeyID, base.GetEncOid(privateKey, digestOID), digestOID, signedAttr, unsignedAttr);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOID, string digestOID, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.doAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), encryptionOID, digestOID, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), signedAttr);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
		{
			this.AddSigner(privateKey, cert, base.GetEncOid(privateKey, digestOID), digestOID, signedAttrGen, unsignedAttrGen);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
		{
			this.doAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), encryptionOID, digestOID, signedAttrGen, unsignedAttrGen, null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
		{
			this.AddSigner(privateKey, subjectKeyID, base.GetEncOid(privateKey, digestOID), digestOID, signedAttrGen, unsignedAttrGen);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
		{
			this.doAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), encryptionOID, digestOID, signedAttrGen, unsignedAttrGen, null);
		}

		private void doAddSigner(AsymmetricKeyParameter privateKey, SignerIdentifier signerIdentifier, string encryptionOID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen, Org.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable)
		{
			this.signerInfs.Add(new CmsSignedDataGenerator.SignerInf(this, privateKey, signerIdentifier, digestOID, encryptionOID, signedAttrGen, unsignedAttrGen, baseSignedTable));
		}

		public CmsSignedData Generate(CmsProcessable content)
		{
			return this.Generate(content, false);
		}

		public CmsSignedData Generate(string signedContentType, CmsProcessable content, bool encapsulate)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
			this._digests.Clear();
			foreach (SignerInformation signerInformation in this._signers)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					CmsSignedDataGenerator.Helper.FixAlgID(signerInformation.DigestAlgorithmID)
				});
				asn1EncodableVector2.Add(new Asn1Encodable[]
				{
					signerInformation.ToSignerInfo()
				});
			}
			DerObjectIdentifier contentType = (signedContentType == null) ? null : new DerObjectIdentifier(signedContentType);
			foreach (CmsSignedDataGenerator.SignerInf signerInf in this.signerInfs)
			{
				try
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						signerInf.DigestAlgorithmID
					});
					asn1EncodableVector2.Add(new Asn1Encodable[]
					{
						signerInf.ToSignerInfo(contentType, content, this.rand)
					});
				}
				catch (IOException e)
				{
					throw new CmsException("encoding error.", e);
				}
				catch (InvalidKeyException e2)
				{
					throw new CmsException("key inappropriate for signature.", e2);
				}
				catch (SignatureException e3)
				{
					throw new CmsException("error creating signature.", e3);
				}
				catch (CertificateEncodingException e4)
				{
					throw new CmsException("error creating sid.", e4);
				}
			}
			Asn1Set certificates = null;
			if (this._certs.Count != 0)
			{
				certificates = CmsUtilities.CreateBerSetFromList(this._certs);
			}
			Asn1Set crls = null;
			if (this._crls.Count != 0)
			{
				crls = CmsUtilities.CreateBerSetFromList(this._crls);
			}
			Asn1OctetString content2 = null;
			if (encapsulate)
			{
				MemoryStream memoryStream = new MemoryStream();
				if (content != null)
				{
					try
					{
						content.Write(memoryStream);
					}
					catch (IOException e5)
					{
						throw new CmsException("encapsulation error.", e5);
					}
				}
				content2 = new BerOctetString(memoryStream.ToArray());
			}
			ContentInfo contentInfo = new ContentInfo(contentType, content2);
			SignedData content3 = new SignedData(new DerSet(asn1EncodableVector), contentInfo, certificates, crls, new DerSet(asn1EncodableVector2));
			ContentInfo sigData = new ContentInfo(CmsObjectIdentifiers.SignedData, content3);
			return new CmsSignedData(content, sigData);
		}

		public CmsSignedData Generate(CmsProcessable content, bool encapsulate)
		{
			return this.Generate(CmsSignedGenerator.Data, content, encapsulate);
		}

		public SignerInformationStore GenerateCounterSigners(SignerInformation signer)
		{
			return this.Generate(null, new CmsProcessableByteArray(signer.GetSignature()), false).GetSignerInfos();
		}
	}
}
