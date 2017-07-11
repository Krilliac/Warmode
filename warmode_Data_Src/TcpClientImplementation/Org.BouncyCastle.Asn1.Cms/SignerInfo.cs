using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class SignerInfo : Asn1Encodable
	{
		private DerInteger version;

		private SignerIdentifier sid;

		private AlgorithmIdentifier digAlgorithm;

		private Asn1Set authenticatedAttributes;

		private AlgorithmIdentifier digEncryptionAlgorithm;

		private Asn1OctetString encryptedDigest;

		private Asn1Set unauthenticatedAttributes;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public SignerIdentifier SignerID
		{
			get
			{
				return this.sid;
			}
		}

		public Asn1Set AuthenticatedAttributes
		{
			get
			{
				return this.authenticatedAttributes;
			}
		}

		public AlgorithmIdentifier DigestAlgorithm
		{
			get
			{
				return this.digAlgorithm;
			}
		}

		public Asn1OctetString EncryptedDigest
		{
			get
			{
				return this.encryptedDigest;
			}
		}

		public AlgorithmIdentifier DigestEncryptionAlgorithm
		{
			get
			{
				return this.digEncryptionAlgorithm;
			}
		}

		public Asn1Set UnauthenticatedAttributes
		{
			get
			{
				return this.unauthenticatedAttributes;
			}
		}

		public static SignerInfo GetInstance(object obj)
		{
			if (obj == null || obj is SignerInfo)
			{
				return (SignerInfo)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SignerInfo((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		public SignerInfo(SignerIdentifier sid, AlgorithmIdentifier digAlgorithm, Asn1Set authenticatedAttributes, AlgorithmIdentifier digEncryptionAlgorithm, Asn1OctetString encryptedDigest, Asn1Set unauthenticatedAttributes)
		{
			this.version = new DerInteger(sid.IsTagged ? 3 : 1);
			this.sid = sid;
			this.digAlgorithm = digAlgorithm;
			this.authenticatedAttributes = authenticatedAttributes;
			this.digEncryptionAlgorithm = digEncryptionAlgorithm;
			this.encryptedDigest = encryptedDigest;
			this.unauthenticatedAttributes = unauthenticatedAttributes;
		}

		public SignerInfo(SignerIdentifier sid, AlgorithmIdentifier digAlgorithm, Attributes authenticatedAttributes, AlgorithmIdentifier digEncryptionAlgorithm, Asn1OctetString encryptedDigest, Attributes unauthenticatedAttributes)
		{
			this.version = new DerInteger(sid.IsTagged ? 3 : 1);
			this.sid = sid;
			this.digAlgorithm = digAlgorithm;
			this.authenticatedAttributes = Asn1Set.GetInstance(authenticatedAttributes);
			this.digEncryptionAlgorithm = digEncryptionAlgorithm;
			this.encryptedDigest = encryptedDigest;
			this.unauthenticatedAttributes = Asn1Set.GetInstance(unauthenticatedAttributes);
		}

		[Obsolete("Use 'GetInstance' instead")]
		public SignerInfo(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			this.version = (DerInteger)enumerator.Current;
			enumerator.MoveNext();
			this.sid = SignerIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.digAlgorithm = AlgorithmIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			object current = enumerator.Current;
			if (current is Asn1TaggedObject)
			{
				this.authenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject)current, false);
				enumerator.MoveNext();
				this.digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(enumerator.Current);
			}
			else
			{
				this.authenticatedAttributes = null;
				this.digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(current);
			}
			enumerator.MoveNext();
			this.encryptedDigest = Asn1OctetString.GetInstance(enumerator.Current);
			if (enumerator.MoveNext())
			{
				this.unauthenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject)enumerator.Current, false);
				return;
			}
			this.unauthenticatedAttributes = null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.sid,
				this.digAlgorithm
			});
			if (this.authenticatedAttributes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.authenticatedAttributes)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.digEncryptionAlgorithm,
				this.encryptedDigest
			});
			if (this.unauthenticatedAttributes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.unauthenticatedAttributes)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
