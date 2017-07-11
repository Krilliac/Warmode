using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Pkcs
{
	public class EncryptedData : Asn1Encodable
	{
		private readonly Asn1Sequence data;

		public DerObjectIdentifier ContentType
		{
			get
			{
				return (DerObjectIdentifier)this.data[0];
			}
		}

		public AlgorithmIdentifier EncryptionAlgorithm
		{
			get
			{
				return AlgorithmIdentifier.GetInstance(this.data[1]);
			}
		}

		public Asn1OctetString Content
		{
			get
			{
				if (this.data.Count == 3)
				{
					DerTaggedObject obj = (DerTaggedObject)this.data[2];
					return Asn1OctetString.GetInstance(obj, false);
				}
				return null;
			}
		}

		public static EncryptedData GetInstance(object obj)
		{
			if (obj is EncryptedData)
			{
				return (EncryptedData)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new EncryptedData((Asn1Sequence)obj);
			}
			throw new ArgumentException("Unknown object in factory: " + obj.GetType().FullName, "obj");
		}

		private EncryptedData(Asn1Sequence seq)
		{
			if (seq.Count != 2)
			{
				throw new ArgumentException("Wrong number of elements in sequence", "seq");
			}
			int intValue = ((DerInteger)seq[0]).Value.IntValue;
			if (intValue != 0)
			{
				throw new ArgumentException("sequence not version 0");
			}
			this.data = (Asn1Sequence)seq[1];
		}

		public EncryptedData(DerObjectIdentifier contentType, AlgorithmIdentifier encryptionAlgorithm, Asn1Encodable content)
		{
			this.data = new BerSequence(new Asn1Encodable[]
			{
				contentType,
				encryptionAlgorithm.ToAsn1Object(),
				new BerTaggedObject(false, 0, content)
			});
		}

		public override Asn1Object ToAsn1Object()
		{
			return new BerSequence(new Asn1Encodable[]
			{
				new DerInteger(0),
				this.data
			});
		}
	}
}
