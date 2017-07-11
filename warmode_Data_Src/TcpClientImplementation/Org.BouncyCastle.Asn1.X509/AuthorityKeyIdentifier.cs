using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class AuthorityKeyIdentifier : Asn1Encodable
	{
		internal readonly Asn1OctetString keyidentifier;

		internal readonly GeneralNames certissuer;

		internal readonly DerInteger certserno;

		public GeneralNames AuthorityCertIssuer
		{
			get
			{
				return this.certissuer;
			}
		}

		public BigInteger AuthorityCertSerialNumber
		{
			get
			{
				if (this.certserno != null)
				{
					return this.certserno.Value;
				}
				return null;
			}
		}

		public static AuthorityKeyIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return AuthorityKeyIdentifier.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static AuthorityKeyIdentifier GetInstance(object obj)
		{
			if (obj is AuthorityKeyIdentifier)
			{
				return (AuthorityKeyIdentifier)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new AuthorityKeyIdentifier((Asn1Sequence)obj);
			}
			if (obj is X509Extension)
			{
				return AuthorityKeyIdentifier.GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		protected internal AuthorityKeyIdentifier(Asn1Sequence seq)
		{
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					this.keyidentifier = Asn1OctetString.GetInstance(asn1TaggedObject, false);
					break;
				case 1:
					this.certissuer = GeneralNames.GetInstance(asn1TaggedObject, false);
					break;
				case 2:
					this.certserno = DerInteger.GetInstance(asn1TaggedObject, false);
					break;
				default:
					throw new ArgumentException("illegal tag");
				}
			}
		}

		public AuthorityKeyIdentifier(SubjectPublicKeyInfo spki)
		{
			IDigest digest = new Sha1Digest();
			byte[] array = new byte[digest.GetDigestSize()];
			byte[] bytes = spki.PublicKeyData.GetBytes();
			digest.BlockUpdate(bytes, 0, bytes.Length);
			digest.DoFinal(array, 0);
			this.keyidentifier = new DerOctetString(array);
		}

		public AuthorityKeyIdentifier(SubjectPublicKeyInfo spki, GeneralNames name, BigInteger serialNumber)
		{
			IDigest digest = new Sha1Digest();
			byte[] array = new byte[digest.GetDigestSize()];
			byte[] bytes = spki.PublicKeyData.GetBytes();
			digest.BlockUpdate(bytes, 0, bytes.Length);
			digest.DoFinal(array, 0);
			this.keyidentifier = new DerOctetString(array);
			this.certissuer = name;
			this.certserno = new DerInteger(serialNumber);
		}

		public AuthorityKeyIdentifier(GeneralNames name, BigInteger serialNumber)
		{
			this.keyidentifier = null;
			this.certissuer = GeneralNames.GetInstance(name.ToAsn1Object());
			this.certserno = new DerInteger(serialNumber);
		}

		public AuthorityKeyIdentifier(byte[] keyIdentifier)
		{
			this.keyidentifier = new DerOctetString(keyIdentifier);
			this.certissuer = null;
			this.certserno = null;
		}

		public AuthorityKeyIdentifier(byte[] keyIdentifier, GeneralNames name, BigInteger serialNumber)
		{
			this.keyidentifier = new DerOctetString(keyIdentifier);
			this.certissuer = GeneralNames.GetInstance(name.ToAsn1Object());
			this.certserno = new DerInteger(serialNumber);
		}

		public byte[] GetKeyIdentifier()
		{
			if (this.keyidentifier != null)
			{
				return this.keyidentifier.GetOctets();
			}
			return null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.keyidentifier != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.keyidentifier)
				});
			}
			if (this.certissuer != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.certissuer)
				});
			}
			if (this.certserno != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 2, this.certserno)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}

		public override string ToString()
		{
			return "AuthorityKeyIdentifier: KeyID(" + this.keyidentifier.GetOctets() + ")";
		}
	}
}
