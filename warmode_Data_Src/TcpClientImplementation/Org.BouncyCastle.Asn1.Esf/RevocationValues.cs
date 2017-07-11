using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Esf
{
	public class RevocationValues : Asn1Encodable
	{
		private readonly Asn1Sequence crlVals;

		private readonly Asn1Sequence ocspVals;

		private readonly OtherRevVals otherRevVals;

		public OtherRevVals OtherRevVals
		{
			get
			{
				return this.otherRevVals;
			}
		}

		public static RevocationValues GetInstance(object obj)
		{
			if (obj == null || obj is RevocationValues)
			{
				return (RevocationValues)obj;
			}
			return new RevocationValues(Asn1Sequence.GetInstance(obj));
		}

		private RevocationValues(Asn1Sequence seq)
		{
			if (seq == null)
			{
				throw new ArgumentNullException("seq");
			}
			if (seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");
			}
			foreach (Asn1TaggedObject asn1TaggedObject in seq)
			{
				Asn1Object @object = asn1TaggedObject.GetObject();
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
				{
					Asn1Sequence asn1Sequence = (Asn1Sequence)@object;
					foreach (Asn1Encodable asn1Encodable in asn1Sequence)
					{
						CertificateList.GetInstance(asn1Encodable.ToAsn1Object());
					}
					this.crlVals = asn1Sequence;
					break;
				}
				case 1:
				{
					Asn1Sequence asn1Sequence2 = (Asn1Sequence)@object;
					foreach (Asn1Encodable asn1Encodable2 in asn1Sequence2)
					{
						BasicOcspResponse.GetInstance(asn1Encodable2.ToAsn1Object());
					}
					this.ocspVals = asn1Sequence2;
					break;
				}
				case 2:
					this.otherRevVals = OtherRevVals.GetInstance(@object);
					break;
				default:
					throw new ArgumentException("Illegal tag in RevocationValues", "seq");
				}
			}
		}

		public RevocationValues(CertificateList[] crlVals, BasicOcspResponse[] ocspVals, OtherRevVals otherRevVals)
		{
			if (crlVals != null)
			{
				this.crlVals = new DerSequence(crlVals);
			}
			if (ocspVals != null)
			{
				this.ocspVals = new DerSequence(ocspVals);
			}
			this.otherRevVals = otherRevVals;
		}

		public RevocationValues(IEnumerable crlVals, IEnumerable ocspVals, OtherRevVals otherRevVals)
		{
			if (crlVals != null)
			{
				if (!CollectionUtilities.CheckElementsAreOfType(crlVals, typeof(CertificateList)))
				{
					throw new ArgumentException("Must contain only 'CertificateList' objects", "crlVals");
				}
				this.crlVals = new DerSequence(Asn1EncodableVector.FromEnumerable(crlVals));
			}
			if (ocspVals != null)
			{
				if (!CollectionUtilities.CheckElementsAreOfType(ocspVals, typeof(BasicOcspResponse)))
				{
					throw new ArgumentException("Must contain only 'BasicOcspResponse' objects", "ocspVals");
				}
				this.ocspVals = new DerSequence(Asn1EncodableVector.FromEnumerable(ocspVals));
			}
			this.otherRevVals = otherRevVals;
		}

		public CertificateList[] GetCrlVals()
		{
			CertificateList[] array = new CertificateList[this.crlVals.Count];
			for (int i = 0; i < this.crlVals.Count; i++)
			{
				array[i] = CertificateList.GetInstance(this.crlVals[i].ToAsn1Object());
			}
			return array;
		}

		public BasicOcspResponse[] GetOcspVals()
		{
			BasicOcspResponse[] array = new BasicOcspResponse[this.ocspVals.Count];
			for (int i = 0; i < this.ocspVals.Count; i++)
			{
				array[i] = BasicOcspResponse.GetInstance(this.ocspVals[i].ToAsn1Object());
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.crlVals != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.crlVals)
				});
			}
			if (this.ocspVals != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.ocspVals)
				});
			}
			if (this.otherRevVals != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.otherRevVals.ToAsn1Object())
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
