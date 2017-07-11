using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1.Tsp
{
	public class TstInfo : Asn1Encodable
	{
		private readonly DerInteger version;

		private readonly DerObjectIdentifier tsaPolicyId;

		private readonly MessageImprint messageImprint;

		private readonly DerInteger serialNumber;

		private readonly DerGeneralizedTime genTime;

		private readonly Accuracy accuracy;

		private readonly DerBoolean ordering;

		private readonly DerInteger nonce;

		private readonly GeneralName tsa;

		private readonly X509Extensions extensions;

		public DerInteger Version
		{
			get
			{
				return this.version;
			}
		}

		public MessageImprint MessageImprint
		{
			get
			{
				return this.messageImprint;
			}
		}

		public DerObjectIdentifier Policy
		{
			get
			{
				return this.tsaPolicyId;
			}
		}

		public DerInteger SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public Accuracy Accuracy
		{
			get
			{
				return this.accuracy;
			}
		}

		public DerGeneralizedTime GenTime
		{
			get
			{
				return this.genTime;
			}
		}

		public DerBoolean Ordering
		{
			get
			{
				return this.ordering;
			}
		}

		public DerInteger Nonce
		{
			get
			{
				return this.nonce;
			}
		}

		public GeneralName Tsa
		{
			get
			{
				return this.tsa;
			}
		}

		public X509Extensions Extensions
		{
			get
			{
				return this.extensions;
			}
		}

		public static TstInfo GetInstance(object o)
		{
			if (o == null || o is TstInfo)
			{
				return (TstInfo)o;
			}
			if (o is Asn1Sequence)
			{
				return new TstInfo((Asn1Sequence)o);
			}
			if (o is Asn1OctetString)
			{
				try
				{
					byte[] octets = ((Asn1OctetString)o).GetOctets();
					return TstInfo.GetInstance(Asn1Object.FromByteArray(octets));
				}
				catch (IOException)
				{
					throw new ArgumentException("Bad object format in 'TstInfo' factory.");
				}
			}
			throw new ArgumentException("Unknown object in 'TstInfo' factory: " + o.GetType().FullName);
		}

		private TstInfo(Asn1Sequence seq)
		{
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			this.version = DerInteger.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.tsaPolicyId = DerObjectIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.messageImprint = MessageImprint.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.serialNumber = DerInteger.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.genTime = DerGeneralizedTime.GetInstance(enumerator.Current);
			this.ordering = DerBoolean.False;
			while (enumerator.MoveNext())
			{
				Asn1Object asn1Object = (Asn1Object)enumerator.Current;
				if (asn1Object is Asn1TaggedObject)
				{
					DerTaggedObject derTaggedObject = (DerTaggedObject)asn1Object;
					switch (derTaggedObject.TagNo)
					{
					case 0:
						this.tsa = GeneralName.GetInstance(derTaggedObject, true);
						break;
					case 1:
						this.extensions = X509Extensions.GetInstance(derTaggedObject, false);
						break;
					default:
						throw new ArgumentException("Unknown tag value " + derTaggedObject.TagNo);
					}
				}
				if (asn1Object is DerSequence)
				{
					this.accuracy = Accuracy.GetInstance(asn1Object);
				}
				if (asn1Object is DerBoolean)
				{
					this.ordering = DerBoolean.GetInstance(asn1Object);
				}
				if (asn1Object is DerInteger)
				{
					this.nonce = DerInteger.GetInstance(asn1Object);
				}
			}
		}

		public TstInfo(DerObjectIdentifier tsaPolicyId, MessageImprint messageImprint, DerInteger serialNumber, DerGeneralizedTime genTime, Accuracy accuracy, DerBoolean ordering, DerInteger nonce, GeneralName tsa, X509Extensions extensions)
		{
			this.version = new DerInteger(1);
			this.tsaPolicyId = tsaPolicyId;
			this.messageImprint = messageImprint;
			this.serialNumber = serialNumber;
			this.genTime = genTime;
			this.accuracy = accuracy;
			this.ordering = ordering;
			this.nonce = nonce;
			this.tsa = tsa;
			this.extensions = extensions;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.tsaPolicyId,
				this.messageImprint,
				this.serialNumber,
				this.genTime
			});
			if (this.accuracy != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.accuracy
				});
			}
			if (this.ordering != null && this.ordering.IsTrue)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.ordering
				});
			}
			if (this.nonce != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.nonce
				});
			}
			if (this.tsa != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.tsa)
				});
			}
			if (this.extensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.extensions)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
