using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class SignedData : Asn1Encodable
	{
		private readonly DerInteger version;

		private readonly Asn1Set digestAlgorithms;

		private readonly ContentInfo contentInfo;

		private readonly Asn1Set certificates;

		private readonly Asn1Set crls;

		private readonly Asn1Set signerInfos;

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

		public ContentInfo ContentInfo
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

		public Asn1Set Crls
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
			if (obj == null)
			{
				return null;
			}
			SignedData signedData = obj as SignedData;
			if (signedData != null)
			{
				return signedData;
			}
			return new SignedData(Asn1Sequence.GetInstance(obj));
		}

		public SignedData(DerInteger _version, Asn1Set _digestAlgorithms, ContentInfo _contentInfo, Asn1Set _certificates, Asn1Set _crls, Asn1Set _signerInfos)
		{
			this.version = _version;
			this.digestAlgorithms = _digestAlgorithms;
			this.contentInfo = _contentInfo;
			this.certificates = _certificates;
			this.crls = _crls;
			this.signerInfos = _signerInfos;
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
				if (asn1Object is DerTaggedObject)
				{
					DerTaggedObject derTaggedObject = (DerTaggedObject)asn1Object;
					switch (derTaggedObject.TagNo)
					{
					case 0:
						this.certificates = Asn1Set.GetInstance(derTaggedObject, false);
						break;
					case 1:
						this.crls = Asn1Set.GetInstance(derTaggedObject, false);
						break;
					default:
						throw new ArgumentException("unknown tag value " + derTaggedObject.TagNo);
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
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.certificates)
				});
			}
			if (this.crls != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.crls)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.signerInfos
			});
			return new BerSequence(asn1EncodableVector);
		}
	}
}
