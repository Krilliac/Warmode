using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Crmf
{
	public class PopoSigningKeyInput : Asn1Encodable
	{
		private readonly GeneralName sender;

		private readonly PKMacValue publicKeyMac;

		private readonly SubjectPublicKeyInfo publicKey;

		public virtual GeneralName Sender
		{
			get
			{
				return this.sender;
			}
		}

		public virtual PKMacValue PublicKeyMac
		{
			get
			{
				return this.publicKeyMac;
			}
		}

		public virtual SubjectPublicKeyInfo PublicKey
		{
			get
			{
				return this.publicKey;
			}
		}

		private PopoSigningKeyInput(Asn1Sequence seq)
		{
			Asn1Encodable asn1Encodable = seq[0];
			if (asn1Encodable is Asn1TaggedObject)
			{
				Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)asn1Encodable;
				if (asn1TaggedObject.TagNo != 0)
				{
					throw new ArgumentException("Unknown authInfo tag: " + asn1TaggedObject.TagNo, "seq");
				}
				this.sender = GeneralName.GetInstance(asn1TaggedObject.GetObject());
			}
			else
			{
				this.publicKeyMac = PKMacValue.GetInstance(asn1Encodable);
			}
			this.publicKey = SubjectPublicKeyInfo.GetInstance(seq[1]);
		}

		public static PopoSigningKeyInput GetInstance(object obj)
		{
			if (obj is PopoSigningKeyInput)
			{
				return (PopoSigningKeyInput)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new PopoSigningKeyInput((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public PopoSigningKeyInput(GeneralName sender, SubjectPublicKeyInfo spki)
		{
			this.sender = sender;
			this.publicKey = spki;
		}

		public PopoSigningKeyInput(PKMacValue pkmac, SubjectPublicKeyInfo spki)
		{
			this.publicKeyMac = pkmac;
			this.publicKey = spki;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.sender != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.sender)
				});
			}
			else
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.publicKeyMac
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.publicKey
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
