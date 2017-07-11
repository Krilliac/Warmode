using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class EncKeyWithID : Asn1Encodable
	{
		private readonly PrivateKeyInfo privKeyInfo;

		private readonly Asn1Encodable identifier;

		public virtual PrivateKeyInfo PrivateKey
		{
			get
			{
				return this.privKeyInfo;
			}
		}

		public virtual bool HasIdentifier
		{
			get
			{
				return this.identifier != null;
			}
		}

		public virtual bool IsIdentifierUtf8String
		{
			get
			{
				return this.identifier is DerUtf8String;
			}
		}

		public virtual Asn1Encodable Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public static EncKeyWithID GetInstance(object obj)
		{
			if (obj is EncKeyWithID)
			{
				return (EncKeyWithID)obj;
			}
			if (obj != null)
			{
				return new EncKeyWithID(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		private EncKeyWithID(Asn1Sequence seq)
		{
			this.privKeyInfo = PrivateKeyInfo.GetInstance(seq[0]);
			if (seq.Count <= 1)
			{
				this.identifier = null;
				return;
			}
			if (!(seq[1] is DerUtf8String))
			{
				this.identifier = GeneralName.GetInstance(seq[1]);
				return;
			}
			this.identifier = seq[1];
		}

		public EncKeyWithID(PrivateKeyInfo privKeyInfo)
		{
			this.privKeyInfo = privKeyInfo;
			this.identifier = null;
		}

		public EncKeyWithID(PrivateKeyInfo privKeyInfo, DerUtf8String str)
		{
			this.privKeyInfo = privKeyInfo;
			this.identifier = str;
		}

		public EncKeyWithID(PrivateKeyInfo privKeyInfo, GeneralName generalName)
		{
			this.privKeyInfo = privKeyInfo;
			this.identifier = generalName;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.privKeyInfo
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.identifier
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
