using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class SignedData : Asn1Encodable
	{
		private static readonly DerInteger Version1 = new DerInteger(1);

		private static readonly DerInteger Version3 = new DerInteger(3);

		private static readonly DerInteger Version4 = new DerInteger(4);

		private static readonly DerInteger Version5 = new DerInteger(5);

		private readonly DerInteger version;

		private readonly Asn1Set digestAlgorithms;

		private readonly ContentInfo contentInfo;

		private readonly Asn1Set certificates;

		private readonly Asn1Set crls;

		private readonly Asn1Set signerInfos;

		private readonly bool certsBer;

		private readonly bool crlsBer;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public Asn1Set DigestAlgorithms
		{
			get
			{
				return this.digestAlgorithms;
			}
		}

		public ContentInfo EncapContentInfo
		{
			get
			{
				return this.contentInfo;
			}
		}

		public Asn1Set Certificates
		{
			get
			{
				return this.certificates;
			}
		}

		public Asn1Set CRLs
		{
			get
			{
				return this.crls;
			}
		}

		public Asn1Set SignerInfos
		{
			get
			{
				return this.signerInfos;
			}
		}

		public static SignedData GetInstance(object obj)
		{
			if (obj is SignedData)
			{
				return (SignedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SignedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public SignedData(Asn1Set digestAlgorithms, ContentInfo contentInfo, Asn1Set certificates, Asn1Set crls, Asn1Set signerInfos)
		{
			this.version = this.CalculateVersion(contentInfo.ContentType, certificates, crls, signerInfos);
			this.digestAlgorithms = digestAlgorithms;
			this.contentInfo = contentInfo;
			this.certificates = certificates;
			this.crls = crls;
			this.signerInfos = signerInfos;
			this.crlsBer = (crls is BerSet);
			this.certsBer = (certificates is BerSet);
		}

		private DerInteger CalculateVersion(DerObjectIdentifier contentOid, Asn1Set certs, Asn1Set crls, Asn1Set signerInfs)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (certs != null)
			{
				foreach (object current in certs)
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
				return SignedData.Version5;
			}
			if (crls != null)
			{
				foreach (object current2 in crls)
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
				return SignedData.Version5;
			}
			if (flag4)
			{
				return SignedData.Version4;
			}
			if (flag3 || !CmsObjectIdentifiers.Data.Equals(contentOid) || this.CheckForVersion3(signerInfs))
			{
				return SignedData.Version3;
			}
			return SignedData.Version1;
		}

		private bool CheckForVersion3(Asn1Set signerInfs)
		{
			foreach (object current in signerInfs)
			{
				SignerInfo instance = SignerInfo.GetInstance(current);
				if (instance.Version.Value.IntValue == 3)
				{
					return true;
				}
			}
			return false;
		}

		private SignedData(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			this.version = (DerInteger)enumerator.Current;
			enumerator.MoveNext();
			this.digestAlgorithms = (Asn1Set)enumerator.Current;
			enumerator.MoveNext();
			this.contentInfo = ContentInfo.GetInstance(enumerator.Current);
			while (enumerator.MoveNext())
			{
				Asn1Object asn1Object = (Asn1Object)enumerator.Current;
				if (asn1Object is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Object;
					switch (asn1TaggedObject.TagNo)
					{
					case 0:
						this.certsBer = (asn1TaggedObject is BerTaggedObject);
						this.certificates = Asn1Set.GetInstance(asn1TaggedObject, false);
						break;
					case 1:
						this.crlsBer = (asn1TaggedObject is BerTaggedObject);
						this.crls = Asn1Set.GetInstance(asn1TaggedObject, false);
						break;
					default:
						throw new ArgumentException("unknown tag value " + asn1TaggedObject.TagNo);
					}
				}
				else
				{
					this.signerInfos = (Asn1Set)asn1Object;
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.digestAlgorithms,
				this.contentInfo
			});
			if (this.certificates != null)
			{
				if (this.certsBer)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new BerTaggedObject(false, 0, this.certificates)
					});
				}
				else
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new DerTaggedObject(false, 0, this.certificates)
					});
				}
			}
			if (this.crls != null)
			{
				if (this.crlsBer)
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new BerTaggedObject(false, 1, this.crls)
					});
				}
				else
				{
					asn1EncodableVector.Add(new Asn1Encodable[]
					{
						new DerTaggedObject(false, 1, this.crls)
					});
				}
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.signerInfos
			});
			return new BerSequence(asn1EncodableVector);
		}
	}
}
