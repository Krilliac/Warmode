using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class ProcurationSyntax : Asn1Encodable
	{
		private readonly string country;

		private readonly DirectoryString typeOfSubstitution;

		private readonly GeneralName thirdPerson;

		private readonly IssuerSerial certRef;

		public virtual string Country
		{
			get
			{
				return this.country;
			}
		}

		public virtual DirectoryString TypeOfSubstitution
		{
			get
			{
				return this.typeOfSubstitution;
			}
		}

		public virtual GeneralName ThirdPerson
		{
			get
			{
				return this.thirdPerson;
			}
		}

		public virtual IssuerSerial CertRef
		{
			get
			{
				return this.certRef;
			}
		}

		public static ProcurationSyntax GetInstance(object obj)
		{
			if (obj == null || obj is ProcurationSyntax)
			{
				return (ProcurationSyntax)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ProcurationSyntax((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private ProcurationSyntax(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			IEnumerator enumerator = seq.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(enumerator.Current);
				switch (instance.TagNo)
				{
				case 1:
					this.country = DerPrintableString.GetInstance(instance, true).GetString();
					break;
				case 2:
					this.typeOfSubstitution = DirectoryString.GetInstance(instance, true);
					break;
				case 3:
				{
					Asn1Object @object = instance.GetObject();
					if (@object is Asn1TaggedObject)
					{
						this.thirdPerson = GeneralName.GetInstance(@object);
					}
					else
					{
						this.certRef = IssuerSerial.GetInstance(@object);
					}
					break;
				}
				default:
					throw new ArgumentException("Bad tag number: " + instance.TagNo);
				}
			}
		}

		public ProcurationSyntax(string country, DirectoryString typeOfSubstitution, IssuerSerial certRef)
		{
			this.country = country;
			this.typeOfSubstitution = typeOfSubstitution;
			this.thirdPerson = null;
			this.certRef = certRef;
		}

		public ProcurationSyntax(string country, DirectoryString typeOfSubstitution, GeneralName thirdPerson)
		{
			this.country = country;
			this.typeOfSubstitution = typeOfSubstitution;
			this.thirdPerson = thirdPerson;
			this.certRef = null;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.country != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, new DerPrintableString(this.country, true))
				});
			}
			if (this.typeOfSubstitution != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 2, this.typeOfSubstitution)
				});
			}
			if (this.thirdPerson != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 3, this.thirdPerson)
				});
			}
			else
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 3, this.certRef)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
