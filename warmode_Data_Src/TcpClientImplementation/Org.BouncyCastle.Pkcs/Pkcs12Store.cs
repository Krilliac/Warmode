using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Pkcs
{
	public class Pkcs12Store
	{
		internal class CertId
		{
			private readonly byte[] id;

			internal byte[] Id
			{
				get
				{
					return this.id;
				}
			}

			internal CertId(AsymmetricKeyParameter pubKey)
			{
				this.id = Pkcs12Store.CreateSubjectKeyID(pubKey).GetKeyIdentifier();
			}

			internal CertId(byte[] id)
			{
				this.id = id;
			}

			public override int GetHashCode()
			{
				return Arrays.GetHashCode(this.id);
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				Pkcs12Store.CertId certId = obj as Pkcs12Store.CertId;
				return certId != null && Arrays.AreEqual(this.id, certId.id);
			}
		}

		private class IgnoresCaseHashtable : IEnumerable
		{
			private readonly IDictionary orig = Platform.CreateHashtable();

			private readonly IDictionary keys = Platform.CreateHashtable();

			public ICollection Keys
			{
				get
				{
					return this.orig.Keys;
				}
			}

			public object this[string alias]
			{
				get
				{
					string key = Platform.ToLowerInvariant(alias);
					string text = (string)this.keys[key];
					if (text == null)
					{
						return null;
					}
					return this.orig[text];
				}
				set
				{
					string key = Platform.ToLowerInvariant(alias);
					string text = (string)this.keys[key];
					if (text != null)
					{
						this.orig.Remove(text);
					}
					this.keys[key] = alias;
					this.orig[alias] = value;
				}
			}

			public ICollection Values
			{
				get
				{
					return this.orig.Values;
				}
			}

			public void Clear()
			{
				this.orig.Clear();
				this.keys.Clear();
			}

			public IEnumerator GetEnumerator()
			{
				return this.orig.GetEnumerator();
			}

			public object Remove(string alias)
			{
				string key = Platform.ToLowerInvariant(alias);
				string text = (string)this.keys[key];
				if (text == null)
				{
					return null;
				}
				this.keys.Remove(key);
				object result = this.orig[text];
				this.orig.Remove(text);
				return result;
			}
		}

		private const int MinIterations = 1024;

		private const int SaltSize = 20;

		private readonly Pkcs12Store.IgnoresCaseHashtable keys = new Pkcs12Store.IgnoresCaseHashtable();

		private readonly IDictionary localIds = Platform.CreateHashtable();

		private readonly Pkcs12Store.IgnoresCaseHashtable certs = new Pkcs12Store.IgnoresCaseHashtable();

		private readonly IDictionary chainCerts = Platform.CreateHashtable();

		private readonly IDictionary keyCerts = Platform.CreateHashtable();

		private readonly DerObjectIdentifier keyAlgorithm;

		private readonly DerObjectIdentifier certAlgorithm;

		private readonly bool useDerEncoding;

		private AsymmetricKeyEntry unmarkedKeyEntry;

		public IEnumerable Aliases
		{
			get
			{
				return new EnumerableProxy(this.GetAliasesTable().Keys);
			}
		}

		public int Count
		{
			get
			{
				return this.GetAliasesTable().Count;
			}
		}

		private static SubjectKeyIdentifier CreateSubjectKeyID(AsymmetricKeyParameter pubKey)
		{
			return new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey));
		}

		internal Pkcs12Store(DerObjectIdentifier keyAlgorithm, DerObjectIdentifier certAlgorithm, bool useDerEncoding)
		{
			this.keyAlgorithm = keyAlgorithm;
			this.certAlgorithm = certAlgorithm;
			this.useDerEncoding = useDerEncoding;
		}

		public Pkcs12Store() : this(PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc, PkcsObjectIdentifiers.PbewithShaAnd40BitRC2Cbc, false)
		{
		}

		public Pkcs12Store(Stream input, char[] password) : this()
		{
			this.Load(input, password);
		}

		protected virtual void LoadKeyBag(PrivateKeyInfo privKeyInfo, Asn1Set bagAttributes)
		{
			AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(privKeyInfo);
			IDictionary dictionary = Platform.CreateHashtable();
			AsymmetricKeyEntry value = new AsymmetricKeyEntry(key, dictionary);
			string text = null;
			Asn1OctetString asn1OctetString = null;
			if (bagAttributes != null)
			{
				foreach (Asn1Sequence asn1Sequence in bagAttributes)
				{
					DerObjectIdentifier instance = DerObjectIdentifier.GetInstance(asn1Sequence[0]);
					Asn1Set instance2 = Asn1Set.GetInstance(asn1Sequence[1]);
					if (instance2.Count > 0)
					{
						Asn1Encodable asn1Encodable = instance2[0];
						if (dictionary.Contains(instance.Id))
						{
							if (!dictionary[instance.Id].Equals(asn1Encodable))
							{
								throw new IOException("attempt to add existing attribute with different value");
							}
						}
						else
						{
							dictionary.Add(instance.Id, asn1Encodable);
						}
						if (instance.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
						{
							text = ((DerBmpString)asn1Encodable).GetString();
							this.keys[text] = value;
						}
						else if (instance.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
						{
							asn1OctetString = (Asn1OctetString)asn1Encodable;
						}
					}
				}
			}
			if (asn1OctetString == null)
			{
				this.unmarkedKeyEntry = value;
				return;
			}
			string text2 = Hex.ToHexString(asn1OctetString.GetOctets());
			if (text == null)
			{
				this.keys[text2] = value;
				return;
			}
			this.localIds[text] = text2;
		}

		protected virtual void LoadPkcs8ShroudedKeyBag(EncryptedPrivateKeyInfo encPrivKeyInfo, Asn1Set bagAttributes, char[] password, bool wrongPkcs12Zero)
		{
			if (password != null)
			{
				PrivateKeyInfo privKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(password, wrongPkcs12Zero, encPrivKeyInfo);
				this.LoadKeyBag(privKeyInfo, bagAttributes);
			}
		}

		public void Load(Stream input, char[] password)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromStream(input);
			Pfx pfx = new Pfx(seq);
			ContentInfo authSafe = pfx.AuthSafe;
			bool wrongPkcs12Zero = false;
			if (password != null && pfx.MacData != null)
			{
				MacData macData = pfx.MacData;
				DigestInfo mac = macData.Mac;
				AlgorithmIdentifier algorithmID = mac.AlgorithmID;
				byte[] salt = macData.GetSalt();
				int intValue = macData.IterationCount.IntValue;
				byte[] octets = ((Asn1OctetString)authSafe.Content).GetOctets();
				byte[] a = Pkcs12Store.CalculatePbeMac(algorithmID.ObjectID, salt, intValue, password, false, octets);
				byte[] digest = mac.GetDigest();
				if (!Arrays.ConstantTimeAreEqual(a, digest))
				{
					if (password.Length > 0)
					{
						throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");
					}
					a = Pkcs12Store.CalculatePbeMac(algorithmID.ObjectID, salt, intValue, password, true, octets);
					if (!Arrays.ConstantTimeAreEqual(a, digest))
					{
						throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");
					}
					wrongPkcs12Zero = true;
				}
			}
			this.keys.Clear();
			this.localIds.Clear();
			this.unmarkedKeyEntry = null;
			IList list = Platform.CreateArrayList();
			if (authSafe.ContentType.Equals(PkcsObjectIdentifiers.Data))
			{
				byte[] octets2 = ((Asn1OctetString)authSafe.Content).GetOctets();
				AuthenticatedSafe authenticatedSafe = new AuthenticatedSafe((Asn1Sequence)Asn1Object.FromByteArray(octets2));
				ContentInfo[] contentInfo = authenticatedSafe.GetContentInfo();
				ContentInfo[] array = contentInfo;
				for (int i = 0; i < array.Length; i++)
				{
					ContentInfo contentInfo2 = array[i];
					DerObjectIdentifier contentType = contentInfo2.ContentType;
					byte[] array2 = null;
					if (contentType.Equals(PkcsObjectIdentifiers.Data))
					{
						array2 = ((Asn1OctetString)contentInfo2.Content).GetOctets();
					}
					else if (contentType.Equals(PkcsObjectIdentifiers.EncryptedData) && password != null)
					{
						EncryptedData instance = EncryptedData.GetInstance(contentInfo2.Content);
						array2 = Pkcs12Store.CryptPbeData(false, instance.EncryptionAlgorithm, password, wrongPkcs12Zero, instance.Content.GetOctets());
					}
					if (array2 != null)
					{
						Asn1Sequence asn1Sequence = (Asn1Sequence)Asn1Object.FromByteArray(array2);
						foreach (Asn1Sequence seq2 in asn1Sequence)
						{
							SafeBag safeBag = new SafeBag(seq2);
							if (safeBag.BagID.Equals(PkcsObjectIdentifiers.CertBag))
							{
								list.Add(safeBag);
							}
							else if (safeBag.BagID.Equals(PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag))
							{
								this.LoadPkcs8ShroudedKeyBag(EncryptedPrivateKeyInfo.GetInstance(safeBag.BagValue), safeBag.BagAttributes, password, wrongPkcs12Zero);
							}
							else if (safeBag.BagID.Equals(PkcsObjectIdentifiers.KeyBag))
							{
								this.LoadKeyBag(PrivateKeyInfo.GetInstance(safeBag.BagValue), safeBag.BagAttributes);
							}
						}
					}
				}
			}
			this.certs.Clear();
			this.chainCerts.Clear();
			this.keyCerts.Clear();
			foreach (SafeBag safeBag2 in list)
			{
				CertBag certBag = new CertBag((Asn1Sequence)safeBag2.BagValue);
				byte[] octets3 = ((Asn1OctetString)certBag.CertValue).GetOctets();
				X509Certificate x509Certificate = new X509CertificateParser().ReadCertificate(octets3);
				IDictionary dictionary = Platform.CreateHashtable();
				Asn1OctetString asn1OctetString = null;
				string text = null;
				if (safeBag2.BagAttributes != null)
				{
					foreach (Asn1Sequence asn1Sequence2 in safeBag2.BagAttributes)
					{
						DerObjectIdentifier instance2 = DerObjectIdentifier.GetInstance(asn1Sequence2[0]);
						Asn1Set instance3 = Asn1Set.GetInstance(asn1Sequence2[1]);
						if (instance3.Count > 0)
						{
							Asn1Encodable asn1Encodable = instance3[0];
							if (dictionary.Contains(instance2.Id))
							{
								if (!dictionary[instance2.Id].Equals(asn1Encodable))
								{
									throw new IOException("attempt to add existing attribute with different value");
								}
							}
							else
							{
								dictionary.Add(instance2.Id, asn1Encodable);
							}
							if (instance2.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName))
							{
								text = ((DerBmpString)asn1Encodable).GetString();
							}
							else if (instance2.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID))
							{
								asn1OctetString = (Asn1OctetString)asn1Encodable;
							}
						}
					}
				}
				Pkcs12Store.CertId certId = new Pkcs12Store.CertId(x509Certificate.GetPublicKey());
				X509CertificateEntry value = new X509CertificateEntry(x509Certificate, dictionary);
				this.chainCerts[certId] = value;
				if (this.unmarkedKeyEntry != null)
				{
					if (this.keyCerts.Count == 0)
					{
						string text2 = Hex.ToHexString(certId.Id);
						this.keyCerts[text2] = value;
						this.keys[text2] = this.unmarkedKeyEntry;
					}
				}
				else
				{
					if (asn1OctetString != null)
					{
						string key = Hex.ToHexString(asn1OctetString.GetOctets());
						this.keyCerts[key] = value;
					}
					if (text != null)
					{
						this.certs[text] = value;
					}
				}
			}
		}

		public AsymmetricKeyEntry GetKey(string alias)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			return (AsymmetricKeyEntry)this.keys[alias];
		}

		public bool IsCertificateEntry(string alias)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			return this.certs[alias] != null && this.keys[alias] == null;
		}

		public bool IsKeyEntry(string alias)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			return this.keys[alias] != null;
		}

		private IDictionary GetAliasesTable()
		{
			IDictionary dictionary = Platform.CreateHashtable();
			foreach (string key in this.certs.Keys)
			{
				dictionary[key] = "cert";
			}
			foreach (string key2 in this.keys.Keys)
			{
				if (dictionary[key2] == null)
				{
					dictionary[key2] = "key";
				}
			}
			return dictionary;
		}

		public bool ContainsAlias(string alias)
		{
			return this.certs[alias] != null || this.keys[alias] != null;
		}

		public X509CertificateEntry GetCertificate(string alias)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			X509CertificateEntry x509CertificateEntry = (X509CertificateEntry)this.certs[alias];
			if (x509CertificateEntry == null)
			{
				string text = (string)this.localIds[alias];
				if (text != null)
				{
					x509CertificateEntry = (X509CertificateEntry)this.keyCerts[text];
				}
				else
				{
					x509CertificateEntry = (X509CertificateEntry)this.keyCerts[alias];
				}
			}
			return x509CertificateEntry;
		}

		public string GetCertificateAlias(X509Certificate cert)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			foreach (DictionaryEntry dictionaryEntry in this.certs)
			{
				X509CertificateEntry x509CertificateEntry = (X509CertificateEntry)dictionaryEntry.Value;
				if (x509CertificateEntry.Certificate.Equals(cert))
				{
					string result = (string)dictionaryEntry.Key;
					return result;
				}
			}
			foreach (DictionaryEntry dictionaryEntry2 in this.keyCerts)
			{
				X509CertificateEntry x509CertificateEntry2 = (X509CertificateEntry)dictionaryEntry2.Value;
				if (x509CertificateEntry2.Certificate.Equals(cert))
				{
					string result = (string)dictionaryEntry2.Key;
					return result;
				}
			}
			return null;
		}

		public X509CertificateEntry[] GetCertificateChain(string alias)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			if (!this.IsKeyEntry(alias))
			{
				return null;
			}
			X509CertificateEntry x509CertificateEntry = this.GetCertificate(alias);
			if (x509CertificateEntry != null)
			{
				IList list = Platform.CreateArrayList();
				while (x509CertificateEntry != null)
				{
					X509Certificate certificate = x509CertificateEntry.Certificate;
					X509CertificateEntry x509CertificateEntry2 = null;
					Asn1OctetString extensionValue = certificate.GetExtensionValue(X509Extensions.AuthorityKeyIdentifier);
					if (extensionValue != null)
					{
						AuthorityKeyIdentifier instance = AuthorityKeyIdentifier.GetInstance(Asn1Object.FromByteArray(extensionValue.GetOctets()));
						if (instance.GetKeyIdentifier() != null)
						{
							x509CertificateEntry2 = (X509CertificateEntry)this.chainCerts[new Pkcs12Store.CertId(instance.GetKeyIdentifier())];
						}
					}
					if (x509CertificateEntry2 == null)
					{
						X509Name issuerDN = certificate.IssuerDN;
						X509Name subjectDN = certificate.SubjectDN;
						if (!issuerDN.Equivalent(subjectDN))
						{
							foreach (Pkcs12Store.CertId key in this.chainCerts.Keys)
							{
								X509CertificateEntry x509CertificateEntry3 = (X509CertificateEntry)this.chainCerts[key];
								X509Certificate certificate2 = x509CertificateEntry3.Certificate;
								X509Name subjectDN2 = certificate2.SubjectDN;
								if (subjectDN2.Equivalent(issuerDN))
								{
									try
									{
										certificate.Verify(certificate2.GetPublicKey());
										x509CertificateEntry2 = x509CertificateEntry3;
										break;
									}
									catch (InvalidKeyException)
									{
									}
								}
							}
						}
					}
					list.Add(x509CertificateEntry);
					if (x509CertificateEntry2 != x509CertificateEntry)
					{
						x509CertificateEntry = x509CertificateEntry2;
					}
					else
					{
						x509CertificateEntry = null;
					}
				}
				X509CertificateEntry[] array = new X509CertificateEntry[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					array[i] = (X509CertificateEntry)list[i];
				}
				return array;
			}
			return null;
		}

		public void SetCertificateEntry(string alias, X509CertificateEntry certEntry)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			if (certEntry == null)
			{
				throw new ArgumentNullException("certEntry");
			}
			if (this.keys[alias] != null)
			{
				throw new ArgumentException("There is a key entry with the name " + alias + ".");
			}
			this.certs[alias] = certEntry;
			this.chainCerts[new Pkcs12Store.CertId(certEntry.Certificate.GetPublicKey())] = certEntry;
		}

		public void SetKeyEntry(string alias, AsymmetricKeyEntry keyEntry, X509CertificateEntry[] chain)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			if (keyEntry == null)
			{
				throw new ArgumentNullException("keyEntry");
			}
			if (keyEntry.Key.IsPrivate && chain == null)
			{
				throw new ArgumentException("No certificate chain for private key");
			}
			if (this.keys[alias] != null)
			{
				this.DeleteEntry(alias);
			}
			this.keys[alias] = keyEntry;
			this.certs[alias] = chain[0];
			for (int num = 0; num != chain.Length; num++)
			{
				this.chainCerts[new Pkcs12Store.CertId(chain[num].Certificate.GetPublicKey())] = chain[num];
			}
		}

		public void DeleteEntry(string alias)
		{
			if (alias == null)
			{
				throw new ArgumentNullException("alias");
			}
			AsymmetricKeyEntry asymmetricKeyEntry = (AsymmetricKeyEntry)this.keys[alias];
			if (asymmetricKeyEntry != null)
			{
				this.keys.Remove(alias);
			}
			X509CertificateEntry x509CertificateEntry = (X509CertificateEntry)this.certs[alias];
			if (x509CertificateEntry != null)
			{
				this.certs.Remove(alias);
				this.chainCerts.Remove(new Pkcs12Store.CertId(x509CertificateEntry.Certificate.GetPublicKey()));
			}
			if (asymmetricKeyEntry != null)
			{
				string text = (string)this.localIds[alias];
				if (text != null)
				{
					this.localIds.Remove(alias);
					x509CertificateEntry = (X509CertificateEntry)this.keyCerts[text];
				}
				if (x509CertificateEntry != null)
				{
					this.keyCerts.Remove(text);
					this.chainCerts.Remove(new Pkcs12Store.CertId(x509CertificateEntry.Certificate.GetPublicKey()));
				}
			}
			if (x509CertificateEntry == null && asymmetricKeyEntry == null)
			{
				throw new ArgumentException("no such entry as " + alias);
			}
		}

		public bool IsEntryOfType(string alias, Type entryType)
		{
			if (entryType == typeof(X509CertificateEntry))
			{
				return this.IsCertificateEntry(alias);
			}
			return entryType == typeof(AsymmetricKeyEntry) && this.IsKeyEntry(alias) && this.GetCertificate(alias) != null;
		}

		[Obsolete("Use 'Count' property instead")]
		public int Size()
		{
			return this.Count;
		}

		public void Save(Stream stream, char[] password, SecureRandom random)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (random == null)
			{
				throw new ArgumentNullException("random");
			}
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (string text in this.keys.Keys)
			{
				byte[] array = new byte[20];
				random.NextBytes(array);
				AsymmetricKeyEntry asymmetricKeyEntry = (AsymmetricKeyEntry)this.keys[text];
				DerObjectIdentifier oid;
				Asn1Encodable asn1Encodable;
				if (password == null)
				{
					oid = PkcsObjectIdentifiers.KeyBag;
					asn1Encodable = PrivateKeyInfoFactory.CreatePrivateKeyInfo(asymmetricKeyEntry.Key);
				}
				else
				{
					oid = PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag;
					asn1Encodable = EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(this.keyAlgorithm, password, array, 1024, asymmetricKeyEntry.Key);
				}
				Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
				foreach (string text2 in asymmetricKeyEntry.BagAttributeKeys)
				{
					Asn1Encodable obj = asymmetricKeyEntry[text2];
					if (!text2.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
					{
						asn1EncodableVector2.Add(new Asn1Encodable[]
						{
							new DerSequence(new Asn1Encodable[]
							{
								new DerObjectIdentifier(text2),
								new DerSet(obj)
							})
						});
					}
				}
				asn1EncodableVector2.Add(new Asn1Encodable[]
				{
					new DerSequence(new Asn1Encodable[]
					{
						PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
						new DerSet(new DerBmpString(text))
					})
				});
				if (asymmetricKeyEntry[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
				{
					X509CertificateEntry certificate = this.GetCertificate(text);
					AsymmetricKeyParameter publicKey = certificate.Certificate.GetPublicKey();
					SubjectKeyIdentifier obj2 = Pkcs12Store.CreateSubjectKeyID(publicKey);
					asn1EncodableVector2.Add(new Asn1Encodable[]
					{
						new DerSequence(new Asn1Encodable[]
						{
							PkcsObjectIdentifiers.Pkcs9AtLocalKeyID,
							new DerSet(obj2)
						})
					});
				}
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new SafeBag(oid, asn1Encodable.ToAsn1Object(), new DerSet(asn1EncodableVector2))
				});
			}
			byte[] derEncoded = new DerSequence(asn1EncodableVector).GetDerEncoded();
			ContentInfo contentInfo = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(derEncoded));
			byte[] array2 = new byte[20];
			random.NextBytes(array2);
			Asn1EncodableVector asn1EncodableVector3 = new Asn1EncodableVector(new Asn1Encodable[0]);
			Pkcs12PbeParams pkcs12PbeParams = new Pkcs12PbeParams(array2, 1024);
			AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(this.certAlgorithm, pkcs12PbeParams.ToAsn1Object());
			ISet set = new HashSet();
			foreach (string text3 in this.keys.Keys)
			{
				X509CertificateEntry certificate2 = this.GetCertificate(text3);
				CertBag certBag = new CertBag(PkcsObjectIdentifiers.X509Certificate, new DerOctetString(certificate2.Certificate.GetEncoded()));
				Asn1EncodableVector asn1EncodableVector4 = new Asn1EncodableVector(new Asn1Encodable[0]);
				foreach (string text4 in certificate2.BagAttributeKeys)
				{
					Asn1Encodable obj3 = certificate2[text4];
					if (!text4.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
					{
						asn1EncodableVector4.Add(new Asn1Encodable[]
						{
							new DerSequence(new Asn1Encodable[]
							{
								new DerObjectIdentifier(text4),
								new DerSet(obj3)
							})
						});
					}
				}
				asn1EncodableVector4.Add(new Asn1Encodable[]
				{
					new DerSequence(new Asn1Encodable[]
					{
						PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
						new DerSet(new DerBmpString(text3))
					})
				});
				if (certificate2[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
				{
					AsymmetricKeyParameter publicKey2 = certificate2.Certificate.GetPublicKey();
					SubjectKeyIdentifier obj4 = Pkcs12Store.CreateSubjectKeyID(publicKey2);
					asn1EncodableVector4.Add(new Asn1Encodable[]
					{
						new DerSequence(new Asn1Encodable[]
						{
							PkcsObjectIdentifiers.Pkcs9AtLocalKeyID,
							new DerSet(obj4)
						})
					});
				}
				asn1EncodableVector3.Add(new Asn1Encodable[]
				{
					new SafeBag(PkcsObjectIdentifiers.CertBag, certBag.ToAsn1Object(), new DerSet(asn1EncodableVector4))
				});
				set.Add(certificate2.Certificate);
			}
			foreach (string text5 in this.certs.Keys)
			{
				X509CertificateEntry x509CertificateEntry = (X509CertificateEntry)this.certs[text5];
				if (this.keys[text5] == null)
				{
					CertBag certBag2 = new CertBag(PkcsObjectIdentifiers.X509Certificate, new DerOctetString(x509CertificateEntry.Certificate.GetEncoded()));
					Asn1EncodableVector asn1EncodableVector5 = new Asn1EncodableVector(new Asn1Encodable[0]);
					foreach (string text6 in x509CertificateEntry.BagAttributeKeys)
					{
						if (!text6.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id))
						{
							Asn1Encodable obj5 = x509CertificateEntry[text6];
							if (!text6.Equals(PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id))
							{
								asn1EncodableVector5.Add(new Asn1Encodable[]
								{
									new DerSequence(new Asn1Encodable[]
									{
										new DerObjectIdentifier(text6),
										new DerSet(obj5)
									})
								});
							}
						}
					}
					asn1EncodableVector5.Add(new Asn1Encodable[]
					{
						new DerSequence(new Asn1Encodable[]
						{
							PkcsObjectIdentifiers.Pkcs9AtFriendlyName,
							new DerSet(new DerBmpString(text5))
						})
					});
					asn1EncodableVector3.Add(new Asn1Encodable[]
					{
						new SafeBag(PkcsObjectIdentifiers.CertBag, certBag2.ToAsn1Object(), new DerSet(asn1EncodableVector5))
					});
					set.Add(x509CertificateEntry.Certificate);
				}
			}
			foreach (Pkcs12Store.CertId key in this.chainCerts.Keys)
			{
				X509CertificateEntry x509CertificateEntry2 = (X509CertificateEntry)this.chainCerts[key];
				if (!set.Contains(x509CertificateEntry2.Certificate))
				{
					CertBag certBag3 = new CertBag(PkcsObjectIdentifiers.X509Certificate, new DerOctetString(x509CertificateEntry2.Certificate.GetEncoded()));
					Asn1EncodableVector asn1EncodableVector6 = new Asn1EncodableVector(new Asn1Encodable[0]);
					foreach (string text7 in x509CertificateEntry2.BagAttributeKeys)
					{
						if (!text7.Equals(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Id))
						{
							asn1EncodableVector6.Add(new Asn1Encodable[]
							{
								new DerSequence(new Asn1Encodable[]
								{
									new DerObjectIdentifier(text7),
									new DerSet(x509CertificateEntry2[text7])
								})
							});
						}
					}
					asn1EncodableVector3.Add(new Asn1Encodable[]
					{
						new SafeBag(PkcsObjectIdentifiers.CertBag, certBag3.ToAsn1Object(), new DerSet(asn1EncodableVector6))
					});
				}
			}
			byte[] derEncoded2 = new DerSequence(asn1EncodableVector3).GetDerEncoded();
			ContentInfo contentInfo2;
			if (password == null)
			{
				contentInfo2 = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(derEncoded2));
			}
			else
			{
				byte[] str = Pkcs12Store.CryptPbeData(true, algorithmIdentifier, password, false, derEncoded2);
				EncryptedData encryptedData = new EncryptedData(PkcsObjectIdentifiers.Data, algorithmIdentifier, new BerOctetString(str));
				contentInfo2 = new ContentInfo(PkcsObjectIdentifiers.EncryptedData, encryptedData.ToAsn1Object());
			}
			ContentInfo[] info = new ContentInfo[]
			{
				contentInfo,
				contentInfo2
			};
			byte[] encoded = new AuthenticatedSafe(info).GetEncoded(this.useDerEncoding ? "DER" : "BER");
			ContentInfo contentInfo3 = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(encoded));
			MacData macData = null;
			if (password != null)
			{
				byte[] array3 = new byte[20];
				random.NextBytes(array3);
				byte[] digest = Pkcs12Store.CalculatePbeMac(OiwObjectIdentifiers.IdSha1, array3, 1024, password, false, encoded);
				AlgorithmIdentifier algID = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);
				DigestInfo digInfo = new DigestInfo(algID, digest);
				macData = new MacData(digInfo, array3, 1024);
			}
			Pfx obj6 = new Pfx(contentInfo3, macData);
			DerOutputStream derOutputStream;
			if (this.useDerEncoding)
			{
				derOutputStream = new DerOutputStream(stream);
			}
			else
			{
				derOutputStream = new BerOutputStream(stream);
			}
			derOutputStream.WriteObject(obj6);
		}

		internal static byte[] CalculatePbeMac(DerObjectIdentifier oid, byte[] salt, int itCount, char[] password, bool wrongPkcs12Zero, byte[] data)
		{
			Asn1Encodable pbeParameters = PbeUtilities.GenerateAlgorithmParameters(oid, salt, itCount);
			ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(oid, password, wrongPkcs12Zero, pbeParameters);
			IMac mac = (IMac)PbeUtilities.CreateEngine(oid);
			mac.Init(parameters);
			return MacUtilities.DoFinal(mac, data);
		}

		private static byte[] CryptPbeData(bool forEncryption, AlgorithmIdentifier algId, char[] password, bool wrongPkcs12Zero, byte[] data)
		{
			IBufferedCipher bufferedCipher = PbeUtilities.CreateEngine(algId.ObjectID) as IBufferedCipher;
			if (bufferedCipher == null)
			{
				throw new Exception("Unknown encryption algorithm: " + algId.ObjectID);
			}
			Pkcs12PbeParams instance = Pkcs12PbeParams.GetInstance(algId.Parameters);
			ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(algId.ObjectID, password, wrongPkcs12Zero, instance);
			bufferedCipher.Init(forEncryption, parameters);
			return bufferedCipher.DoFinal(data);
		}
	}
}
