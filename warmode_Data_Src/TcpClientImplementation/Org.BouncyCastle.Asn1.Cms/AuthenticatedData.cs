using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class AuthenticatedData : Asn1Encodable
	{
		private DerInteger version;

		private OriginatorInfo originatorInfo;

		private Asn1Set recipientInfos;

		private AlgorithmIdentifier macAlgorithm;

		private AlgorithmIdentifier digestAlgorithm;

		private ContentInfo encapsulatedContentInfo;

		private Asn1Set authAttrs;

		private Asn1OctetString mac;

		private Asn1Set unauthAttrs;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public OriginatorInfo OriginatorInfo
		{
			get
			{
				return this.originatorInfo;
			}
		}

		public Asn1Set RecipientInfos
		{
			get
			{
				return this.recipientInfos;
			}
		}

		public AlgorithmIdentifier MacAlgorithm
		{
			get
			{
				return this.macAlgorithm;
			}
		}

		public AlgorithmIdentifier DigestAlgorithm
		{
			get
			{
				return this.digestAlgorithm;
			}
		}

		public ContentInfo EncapsulatedContentInfo
		{
			get
			{
				return this.encapsulatedContentInfo;
			}
		}

		public Asn1Set AuthAttrs
		{
			get
			{
				return this.authAttrs;
			}
		}

		public Asn1OctetString Mac
		{
			get
			{
				return this.mac;
			}
		}

		public Asn1Set UnauthAttrs
		{
			get
			{
				return this.unauthAttrs;
			}
		}

		public AuthenticatedData(OriginatorInfo originatorInfo, Asn1Set recipientInfos, AlgorithmIdentifier macAlgorithm, AlgorithmIdentifier digestAlgorithm, ContentInfo encapsulatedContent, Asn1Set authAttrs, Asn1OctetString mac, Asn1Set unauthAttrs)
		{
			if ((digestAlgorithm != null || authAttrs != null) && (digestAlgorithm == null || authAttrs == null))
			{
				throw new ArgumentException("digestAlgorithm and authAttrs must be set together");
			}
			this.version = new DerInteger(AuthenticatedData.CalculateVersion(originatorInfo));
			this.originatorInfo = originatorInfo;
			this.macAlgorithm = macAlgorithm;
			this.digestAlgorithm = digestAlgorithm;
			this.recipientInfos = recipientInfos;
			this.encapsulatedContentInfo = encapsulatedContent;
			this.authAttrs = authAttrs;
			this.mac = mac;
			this.unauthAttrs = unauthAttrs;
		}

		private AuthenticatedData(Asn1Sequence seq)
		{
			int num = 0;
			this.version = (DerInteger)seq[num++];
			Asn1Encodable asn1Encodable = seq[num++];
			if (asn1Encodable is Asn1TaggedObject)
			{
				this.originatorInfo = OriginatorInfo.GetInstance((Asn1TaggedObject)asn1Encodable, false);
				asn1Encodable = seq[num++];
			}
			this.recipientInfos = Asn1Set.GetInstance(asn1Encodable);
			this.macAlgorithm = AlgorithmIdentifier.GetInstance(seq[num++]);
			asn1Encodable = seq[num++];
			if (asn1Encodable is Asn1TaggedObject)
			{
				this.digestAlgorithm = AlgorithmIdentifier.GetInstance((Asn1TaggedObject)asn1Encodable, false);
				asn1Encodable = seq[num++];
			}
			this.encapsulatedContentInfo = ContentInfo.GetInstance(asn1Encodable);
			asn1Encodable = seq[num++];
			if (asn1Encodable is Asn1TaggedObject)
			{
				this.authAttrs = Asn1Set.GetInstance((Asn1TaggedObject)asn1Encodable, false);
				asn1Encodable = seq[num++];
			}
			this.mac = Asn1OctetString.GetInstance(asn1Encodable);
			if (seq.Count > num)
			{
				this.unauthAttrs = Asn1Set.GetInstance((Asn1TaggedObject)seq[num], false);
			}
		}

		public static AuthenticatedData GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return AuthenticatedData.GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static AuthenticatedData GetInstance(object obj)
		{
			if (obj == null || obj is AuthenticatedData)
			{
				return (AuthenticatedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AuthenticatedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid AuthenticatedData: " + obj.GetType().Name);
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version
			});
			if (this.originatorInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.originatorInfo)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.recipientInfos,
				this.macAlgorithm
			});
			if (this.digestAlgorithm != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.digestAlgorithm)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.encapsulatedContentInfo
			});
			if (this.authAttrs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 2, this.authAttrs)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.mac
			});
			if (this.unauthAttrs != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 3, this.unauthAttrs)
				});
			}
			return new BerSequence(asn1EncodableVector);
		}

		public static int CalculateVersion(OriginatorInfo origInfo)
		{
			if (origInfo == null)
			{
				return 0;
			}
			int result = 0;
			foreach (object current in origInfo.Certificates)
			{
				if (current is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)current;
					if (asn1TaggedObject.TagNo == 2)
					{
						result = 1;
					}
					else if (asn1TaggedObject.TagNo == 3)
					{
						result = 3;
						break;
					}
				}
			}
			foreach (object current2 in origInfo.Crls)
			{
				if (current2 is Asn1TaggedObject)
				{
					Asn1TaggedObject asn1TaggedObject2 = (Asn1TaggedObject)current2;
					if (asn1TaggedObject2.TagNo == 1)
					{
						result = 3;
						break;
					}
				}
			}
			return result;
		}
	}
}
