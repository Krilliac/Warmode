using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509.Qualified
{
	public class SemanticsInformation : Asn1Encodable
	{
		private readonly DerObjectIdentifier semanticsIdentifier;

		private readonly GeneralName[] nameRegistrationAuthorities;

		public DerObjectIdentifier SemanticsIdentifier
		{
			get
			{
				return this.semanticsIdentifier;
			}
		}

		public static SemanticsInformation GetInstance(object obj)
		{
			if (obj == null || obj is SemanticsInformation)
			{
				return (SemanticsInformation)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SemanticsInformation(Asn1Sequence.GetInstance(obj));
			}
			throw new ArgumentException("unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		public SemanticsInformation(Asn1Sequence seq)
		{
			if (seq.Count < 1)
			{
				throw new ArgumentException("no objects in SemanticsInformation");
			}
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			object obj = enumerator.Current;
			if (obj is DerObjectIdentifier)
			{
				this.semanticsIdentifier = DerObjectIdentifier.GetInstance(obj);
				if (enumerator.MoveNext())
				{
					obj = enumerator.Current;
				}
				else
				{
					obj = null;
				}
			}
			if (obj != null)
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(obj);
				this.nameRegistrationAuthorities = new GeneralName[instance.Count];
				for (int i = 0; i < instance.Count; i++)
				{
					this.nameRegistrationAuthorities[i] = GeneralName.GetInstance(instance[i]);
				}
			}
		}

		public SemanticsInformation(DerObjectIdentifier semanticsIdentifier, GeneralName[] generalNames)
		{
			this.semanticsIdentifier = semanticsIdentifier;
			this.nameRegistrationAuthorities = generalNames;
		}

		public SemanticsInformation(DerObjectIdentifier semanticsIdentifier)
		{
			this.semanticsIdentifier = semanticsIdentifier;
		}

		public SemanticsInformation(GeneralName[] generalNames)
		{
			this.nameRegistrationAuthorities = generalNames;
		}

		public GeneralName[] GetNameRegistrationAuthorities()
		{
			return this.nameRegistrationAuthorities;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.semanticsIdentifier != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.semanticsIdentifier
				});
			}
			if (this.nameRegistrationAuthorities != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerSequence(this.nameRegistrationAuthorities)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
