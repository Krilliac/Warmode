using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class PrivateKeyInfo : Asn1Encodable
	{
		private readonly Asn1OctetString privKey;

		private readonly AlgorithmIdentifier algID;

		private readonly Asn1Set attributes;

		public virtual AlgorithmIdentifier PrivateKeyAlgorithm
		{
			get
			{
				return this.algID;
			}
		}

		[Obsolete("Use 'PrivateKeyAlgorithm' property instead")]
		public virtual AlgorithmIdentifier AlgorithmID
		{
			get
			{
				return this.algID;
			}
		}

		[Obsolete("Use 'ParsePrivateKey' instead")]
		public virtual Asn1Object PrivateKey
		{
			get
			{
				Asn1Object result;
				try
				{
					result = this.ParsePrivateKey();
				}
				catch (IOException)
				{
					throw new InvalidOperationException("unable to parse private key");
				}
				return result;
			}
		}

		public virtual Asn1Set Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		public static PrivateKeyInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return PrivateKeyInfo.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static PrivateKeyInfo GetInstance(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is PrivateKeyInfo)
			{
				return (PrivateKeyInfo)obj;
			}
			return new PrivateKeyInfo(Asn1Sequence.GetInstance(obj));
		}

		public PrivateKeyInfo(AlgorithmIdentifier algID, Asn1Object privateKey) : this(algID, privateKey, null)
		{
		}

		public PrivateKeyInfo(AlgorithmIdentifier algID, Asn1Object privateKey, Asn1Set attributes)
		{
			this.algID = algID;
			this.privKey = new DerOctetString(privateKey.GetEncoded("DER"));
			this.attributes = attributes;
		}

		private PrivateKeyInfo(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			BigInteger value = ((DerInteger)enumerator.Current).Value;
			if (value.IntValue != 0)
			{
				throw new ArgumentException("wrong version for private key info: " + value.IntValue);
			}
			enumerator.MoveNext();
			this.algID = AlgorithmIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.privKey = Asn1OctetString.GetInstance(enumerator.Current);
			if (enumerator.MoveNext())
			{
				this.attributes = Asn1Set.GetInstance((Asn1TaggedObject)enumerator.Current, false);
			}
		}

		public virtual Asn1Object ParsePrivateKey()
		{
			return Asn1Object.FromByteArray(this.privKey.GetOctets());
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				new DerInteger(0),
				this.algID,
				this.privKey
			});
			if (this.attributes != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 0, this.attributes)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
