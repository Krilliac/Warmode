using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Cms
{
	public class CmsSignedDataStreamGenerator : CmsSignedGenerator
	{
		private class DigestAndSignerInfoGeneratorHolder
		{
			internal readonly SignerInfoGenerator signerInf;

			internal readonly string digestOID;

			internal AlgorithmIdentifier DigestAlgorithm
			{
				get
				{
					return new AlgorithmIdentifier(new DerObjectIdentifier(this.digestOID), DerNull.Instance);
				}
			}

			internal DigestAndSignerInfoGeneratorHolder(SignerInfoGenerator signerInf, string digestOID)
			{
				this.signerInf = signerInf;
				this.digestOID = digestOID;
			}
		}

		private class SignerInfoGeneratorImpl : SignerInfoGenerator
		{
			private readonly CmsSignedDataStreamGenerator outer;

			private readonly SignerIdentifier _signerIdentifier;

			private readonly string _digestOID;

			private readonly string _encOID;

			private readonly CmsAttributeTableGenerator _sAttr;

			private readonly CmsAttributeTableGenerator _unsAttr;

			private readonly string _encName;

			private readonly ISigner _sig;

			internal SignerInfoGeneratorImpl(CmsSignedDataStreamGenerator outer, AsymmetricKeyParameter key, SignerIdentifier signerIdentifier, string digestOID, string encOID, CmsAttributeTableGenerator sAttr, CmsAttributeTableGenerator unsAttr)
			{
				this.outer = outer;
				this._signerIdentifier = signerIdentifier;
				this._digestOID = digestOID;
				this._encOID = encOID;
				this._sAttr = sAttr;
				this._unsAttr = unsAttr;
				this._encName = CmsSignedDataStreamGenerator.Helper.GetEncryptionAlgName(this._encOID);
				string digestAlgName = CmsSignedDataStreamGenerator.Helper.GetDigestAlgName(this._digestOID);
				string algorithm = digestAlgName + "with" + this._encName;
				if (this._sAttr != null)
				{
					this._sig = CmsSignedDataStreamGenerator.Helper.GetSignatureInstance(algorithm);
				}
				else if (this._encName.Equals("RSA"))
				{
					this._sig = CmsSignedDataStreamGenerator.Helper.GetSignatureInstance("RSA");
				}
				else
				{
					if (!this._encName.Equals("DSA"))
					{
						throw new SignatureException("algorithm: " + this._encName + " not supported in base signatures.");
					}
					this._sig = CmsSignedDataStreamGenerator.Helper.GetSignatureInstance("NONEwithDSA");
				}
				this._sig.Init(true, new ParametersWithRandom(key, outer.rand));
			}

			public SignerInfo Generate(DerObjectIdentifier contentType, AlgorithmIdentifier digestAlgorithm, byte[] calculatedDigest)
			{
				SignerInfo result;
				try
				{
					string digestAlgName = CmsSignedDataStreamGenerator.Helper.GetDigestAlgName(this._digestOID);
					string algorithm = digestAlgName + "with" + this._encName;
					byte[] array = calculatedDigest;
					Asn1Set asn1Set = null;
					if (this._sAttr != null)
					{
						IDictionary baseParameters = this.outer.GetBaseParameters(contentType, digestAlgorithm, calculatedDigest);
						Org.BouncyCastle.Asn1.Cms.AttributeTable attributeTable = this._sAttr.GetAttributes(baseParameters);
						if (contentType == null && attributeTable != null && attributeTable[CmsAttributes.ContentType] != null)
						{
							IDictionary dictionary = attributeTable.ToDictionary();
							dictionary.Remove(CmsAttributes.ContentType);
							attributeTable = new Org.BouncyCastle.Asn1.Cms.AttributeTable(dictionary);
						}
						asn1Set = this.outer.GetAttributeSet(attributeTable);
						array = asn1Set.GetEncoded("DER");
					}
					else if (this._encName.Equals("RSA"))
					{
						DigestInfo digestInfo = new DigestInfo(digestAlgorithm, calculatedDigest);
						array = digestInfo.GetEncoded("DER");
					}
					this._sig.BlockUpdate(array, 0, array.Length);
					byte[] array2 = this._sig.GenerateSignature();
					Asn1Set unauthenticatedAttributes = null;
					if (this._unsAttr != null)
					{
						IDictionary baseParameters2 = this.outer.GetBaseParameters(contentType, digestAlgorithm, calculatedDigest);
						baseParameters2[CmsAttributeTableParameter.Signature] = array2.Clone();
						Org.BouncyCastle.Asn1.Cms.AttributeTable attributes = this._unsAttr.GetAttributes(baseParameters2);
						unauthenticatedAttributes = this.outer.GetAttributeSet(attributes);
					}
					Asn1Encodable defaultX509Parameters = SignerUtilities.GetDefaultX509Parameters(algorithm);
					AlgorithmIdentifier encAlgorithmIdentifier = CmsSignedGenerator.GetEncAlgorithmIdentifier(new DerObjectIdentifier(this._encOID), defaultX509Parameters);
					result = new SignerInfo(this._signerIdentifier, digestAlgorithm, asn1Set, encAlgorithmIdentifier, new DerOctetString(array2), unauthenticatedAttributes);
				}
				catch (IOException e)
				{
					throw new CmsStreamException("encoding error.", e);
				}
				catch (SignatureException e2)
				{
					throw new CmsStreamException("error creating signature.", e2);
				}
				return result;
			}
		}

		private class CmsSignedDataOutputStream : BaseOutputStream
		{
			private readonly CmsSignedDataStreamGenerator outer;

			private Stream _out;

			private DerObjectIdentifier _contentOID;

			private BerSequenceGenerator _sGen;

			private BerSequenceGenerator _sigGen;

			private BerSequenceGenerator _eiGen;

			public CmsSignedDataOutputStream(CmsSignedDataStreamGenerator outer, Stream outStream, string contentOID, BerSequenceGenerator sGen, BerSequenceGenerator sigGen, BerSequenceGenerator eiGen)
			{
				this.outer = outer;
				this._out = outStream;
				this._contentOID = new DerObjectIdentifier(contentOID);
				this._sGen = sGen;
				this._sigGen = sigGen;
				this._eiGen = eiGen;
			}

			public override void WriteByte(byte b)
			{
				this._out.WriteByte(b);
			}

			public override void Write(byte[] bytes, int off, int len)
			{
				this._out.Write(bytes, off, len);
			}

			public override void Close()
			{
				this._out.Close();
				this._eiGen.Close();
				this.outer._digests.Clear();
				if (this.outer._certs.Count > 0)
				{
					Asn1Set obj = CmsUtilities.CreateBerSetFromList(this.outer._certs);
					CmsSignedDataStreamGenerator.CmsSignedDataOutputStream.WriteToGenerator(this._sigGen, new BerTaggedObject(false, 0, obj));
				}
				if (this.outer._crls.Count > 0)
				{
					Asn1Set obj2 = CmsUtilities.CreateBerSetFromList(this.outer._crls);
					CmsSignedDataStreamGenerator.CmsSignedDataOutputStream.WriteToGenerator(this._sigGen, new BerTaggedObject(false, 1, obj2));
				}
				foreach (DictionaryEntry dictionaryEntry in this.outer._messageDigests)
				{
					this.outer._messageHashes.Add(dictionaryEntry.Key, DigestUtilities.DoFinal((IDigest)dictionaryEntry.Value));
				}
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
				foreach (CmsSignedDataStreamGenerator.DigestAndSignerInfoGeneratorHolder digestAndSignerInfoGeneratorHolder in this.outer._signerInfs)
				{
					AlgorithmIdentifier digestAlgorithm = digestAndSignerInfoGeneratorHolder.DigestAlgorithm;
					byte[] array = (byte[])this.outer._messageHashes[CmsSignedDataStreamGenerator.Helper.GetDigestAlgName(digestAndSignerInfoGeneratorHolder.digestOID)];
					this.outer._digests[digestAndSignerInfoGeneratorHolder.digestOID] = array.Clone();
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						digestAndSignerInfoGeneratorHolder.signerInf.Generate(this._contentOID, digestAlgorithm, array)
					});
				}
				foreach (SignerInformation signerInformation in this.outer._signers)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						signerInformation.ToSignerInfo()
					});
				}
				CmsSignedDataStreamGenerator.CmsSignedDataOutputStream.WriteToGenerator(this._sigGen, new DerSet(asn1EncodableVector));
				this._sigGen.Close();
				this._sGen.Close();
				base.Close();
			}

			private static void WriteToGenerator(Asn1Generator ag, Asn1Encodable ae)
			{
				byte[] encoded = ae.GetEncoded();
				ag.GetRawOutputStream().Write(encoded, 0, encoded.Length);
			}
		}

		private static readonly CmsSignedHelper Helper = CmsSignedHelper.Instance;

		private readonly IList _signerInfs = Platform.CreateArrayList();

		private readonly ISet _messageDigestOids = new HashSet();

		private readonly IDictionary _messageDigests = Platform.CreateHashtable();

		private readonly IDictionary _messageHashes = Platform.CreateHashtable();

		private bool _messageDigestsLocked;

		private int _bufferSize;

		public CmsSignedDataStreamGenerator()
		{
		}

		public CmsSignedDataStreamGenerator(SecureRandom rand) : base(rand)
		{
		}

		public void SetBufferSize(int bufferSize)
		{
			this._bufferSize = bufferSize;
		}

		public void AddDigests(params string[] digestOids)
		{
			this.AddDigests(digestOids);
		}

		public void AddDigests(IEnumerable digestOids)
		{
			foreach (string digestOid in digestOids)
			{
				this.ConfigureDigest(digestOid);
			}
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOid)
		{
			this.AddSigner(privateKey, cert, digestOid, new DefaultSignedAttributeTableGenerator(), null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOid, string digestOid)
		{
			this.AddSigner(privateKey, cert, encryptionOid, digestOid, new DefaultSignedAttributeTableGenerator(), null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOid, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.AddSigner(privateKey, cert, digestOid, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr));
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOid, string digestOid, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.AddSigner(privateKey, cert, encryptionOid, digestOid, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr));
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
		{
			this.AddSigner(privateKey, cert, base.GetEncOid(privateKey, digestOid), digestOid, signedAttrGenerator, unsignedAttrGenerator);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOid, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
		{
			this.DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), encryptionOid, digestOid, signedAttrGenerator, unsignedAttrGenerator);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOid)
		{
			this.AddSigner(privateKey, subjectKeyID, digestOid, new DefaultSignedAttributeTableGenerator(), null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOid, string digestOid)
		{
			this.AddSigner(privateKey, subjectKeyID, encryptionOid, digestOid, new DefaultSignedAttributeTableGenerator(), null);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOid, Org.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Org.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
		{
			this.AddSigner(privateKey, subjectKeyID, digestOid, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr));
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
		{
			this.AddSigner(privateKey, subjectKeyID, base.GetEncOid(privateKey, digestOid), digestOid, signedAttrGenerator, unsignedAttrGenerator);
		}

		public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOid, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
		{
			this.DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), encryptionOid, digestOid, signedAttrGenerator, unsignedAttrGenerator);
		}

		private void DoAddSigner(AsymmetricKeyParameter privateKey, SignerIdentifier signerIdentifier, string encryptionOid, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
		{
			this.ConfigureDigest(digestOid);
			CmsSignedDataStreamGenerator.SignerInfoGeneratorImpl signerInf = new CmsSignedDataStreamGenerator.SignerInfoGeneratorImpl(this, privateKey, signerIdentifier, digestOid, encryptionOid, signedAttrGenerator, unsignedAttrGenerator);
			this._signerInfs.Add(new CmsSignedDataStreamGenerator.DigestAndSignerInfoGeneratorHolder(signerInf, digestOid));
		}

		internal override void AddSignerCallback(SignerInformation si)
		{
			this.RegisterDigestOid(si.DigestAlgorithmID.ObjectID.Id);
		}

		public Stream Open(Stream outStream)
		{
			return this.Open(outStream, false);
		}

		public Stream Open(Stream outStream, bool encapsulate)
		{
			return this.Open(outStream, CmsSignedGenerator.Data, encapsulate);
		}

		public Stream Open(Stream outStream, bool encapsulate, Stream dataOutputStream)
		{
			return this.Open(outStream, CmsSignedGenerator.Data, encapsulate, dataOutputStream);
		}

		public Stream Open(Stream outStream, string signedContentType, bool encapsulate)
		{
			return this.Open(outStream, signedContentType, encapsulate, null);
		}

		public Stream Open(Stream outStream, string signedContentType, bool encapsulate, Stream dataOutputStream)
		{
			if (outStream == null)
			{
				throw new ArgumentNullException("outStream");
			}
			if (!outStream.CanWrite)
			{
				throw new ArgumentException("Expected writeable stream", "outStream");
			}
			if (dataOutputStream != null && !dataOutputStream.CanWrite)
			{
				throw new ArgumentException("Expected writeable stream", "dataOutputStream");
			}
			this._messageDigestsLocked = true;
			BerSequenceGenerator berSequenceGenerator = new BerSequenceGenerator(outStream);
			berSequenceGenerator.AddObject(CmsObjectIdentifiers.SignedData);
			BerSequenceGenerator berSequenceGenerator2 = new BerSequenceGenerator(berSequenceGenerator.GetRawOutputStream(), 0, true);
			DerObjectIdentifier derObjectIdentifier = (signedContentType == null) ? null : new DerObjectIdentifier(signedContentType);
			berSequenceGenerator2.AddObject(this.CalculateVersion(derObjectIdentifier));
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (string identifier in this._messageDigestOids)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new AlgorithmIdentifier(new DerObjectIdentifier(identifier), DerNull.Instance)
				});
			}
			byte[] encoded = new DerSet(asn1EncodableVector).GetEncoded();
			berSequenceGenerator2.GetRawOutputStream().Write(encoded, 0, encoded.Length);
			BerSequenceGenerator berSequenceGenerator3 = new BerSequenceGenerator(berSequenceGenerator2.GetRawOutputStream());
			berSequenceGenerator3.AddObject(derObjectIdentifier);
			Stream s = encapsulate ? CmsUtilities.CreateBerOctetOutputStream(berSequenceGenerator3.GetRawOutputStream(), 0, true, this._bufferSize) : null;
			Stream safeTeeOutputStream = CmsSignedDataStreamGenerator.GetSafeTeeOutputStream(dataOutputStream, s);
			Stream outStream2 = CmsSignedDataStreamGenerator.AttachDigestsToOutputStream(this._messageDigests.Values, safeTeeOutputStream);
			return new CmsSignedDataStreamGenerator.CmsSignedDataOutputStream(this, outStream2, signedContentType, berSequenceGenerator, berSequenceGenerator2, berSequenceGenerator3);
		}

		private void RegisterDigestOid(string digestOid)
		{
			if (this._messageDigestsLocked)
			{
				if (!this._messageDigestOids.Contains(digestOid))
				{
					throw new InvalidOperationException("Cannot register new digest OIDs after the data stream is opened");
				}
			}
			else
			{
				this._messageDigestOids.Add(digestOid);
			}
		}

		private void ConfigureDigest(string digestOid)
		{
			this.RegisterDigestOid(digestOid);
			string digestAlgName = CmsSignedDataStreamGenerator.Helper.GetDigestAlgName(digestOid);
			if ((IDigest)this._messageDigests[digestAlgName] == null)
			{
				if (this._messageDigestsLocked)
				{
					throw new InvalidOperationException("Cannot configure new digests after the data stream is opened");
				}
				IDigest digestInstance = CmsSignedDataStreamGenerator.Helper.GetDigestInstance(digestAlgName);
				this._messageDigests[digestAlgName] = digestInstance;
			}
		}

		internal void Generate(Stream outStream, string eContentType, bool encapsulate, Stream dataOutputStream, CmsProcessable content)
		{
			Stream stream = this.Open(outStream, eContentType, encapsulate, dataOutputStream);
			if (content != null)
			{
				content.Write(stream);
			}
			stream.Close();
		}

		private DerInteger CalculateVersion(DerObjectIdentifier contentOid)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (this._certs != null)
			{
				foreach (object current in this._certs)
				{
					if (current is Asn1TaggedObject)
					{
						Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)current;
						if (asn1TaggedObject.TagNo == 1)
						{
							flag3 = true;
						}
						else if (asn1TaggedObject.TagNo == 2)
						{
							flag4 = true;
						}
						else if (asn1TaggedObject.TagNo == 3)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				return new DerInteger(5);
			}
			if (this._crls != null)
			{
				foreach (object current2 in this._crls)
				{
					if (current2 is Asn1TaggedObject)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				return new DerInteger(5);
			}
			if (flag4)
			{
				return new DerInteger(4);
			}
			if (flag3 || !CmsObjectIdentifiers.Data.Equals(contentOid) || this.CheckForVersion3(this._signers))
			{
				return new DerInteger(3);
			}
			return new DerInteger(1);
		}

		private bool CheckForVersion3(IList signerInfos)
		{
			foreach (SignerInformation signerInformation in signerInfos)
			{
				SignerInfo instance = SignerInfo.GetInstance(signerInformation.ToSignerInfo());
				if (instance.Version.Value.IntValue == 3)
				{
					return true;
				}
			}
			return false;
		}

		private static Stream AttachDigestsToOutputStream(ICollection digests, Stream s)
		{
			Stream stream = s;
			foreach (IDigest dig in digests)
			{
				stream = CmsSignedDataStreamGenerator.GetSafeTeeOutputStream(stream, new DigOutputStream(dig));
			}
			return stream;
		}

		private static Stream GetSafeOutputStream(Stream s)
		{
			if (s == null)
			{
				return new NullOutputStream();
			}
			return s;
		}

		private static Stream GetSafeTeeOutputStream(Stream s1, Stream s2)
		{
			if (s1 == null)
			{
				return CmsSignedDataStreamGenerator.GetSafeOutputStream(s2);
			}
			if (s2 == null)
			{
				return CmsSignedDataStreamGenerator.GetSafeOutputStream(s1);
			}
			return new TeeOutputStream(s1, s2);
		}
	}
}
