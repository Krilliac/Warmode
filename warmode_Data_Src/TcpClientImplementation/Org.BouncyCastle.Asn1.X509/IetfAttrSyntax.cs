using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class IetfAttrSyntax : Asn1Encodable
	{
		public const int ValueOctets = 1;

		public const int ValueOid = 2;

		public const int ValueUtf8 = 3;

		internal readonly GeneralNames policyAuthority;

		internal readonly Asn1EncodableVector values = new Asn1EncodableVector(new Asn1Encodable[0]);

		internal int valueChoice = -1;

		public GeneralNames PolicyAuthority
		{
			get
			{
				return this.policyAuthority;
			}
		}

		public int ValueType
		{
			get
			{
				return this.valueChoice;
			}
		}

		public IetfAttrSyntax(Asn1Sequence seq)
		{
			int num = 0;
			if (seq[0] is Asn1TaggedObject)
			{
				this.policyAuthority = GeneralNames.GetInstance((Asn1TaggedObject)seq[0], false);
				num++;
			}
			else if (seq.Count == 2)
			{
				this.policyAuthority = GeneralNames.GetInstance(seq[0]);
				num++;
			}
			if (!(seq[num] is Asn1Sequence))
			{
				throw new ArgumentException("Non-IetfAttrSyntax encoding");
			}
			seq = (Asn1Sequence)seq[num];
			foreach (Asn1Object asn1Object in seq)
			{
				int num2;
				if (asn1Object is DerObjectIdentifier)
				{
					num2 = 2;
				}
				else if (asn1Object is DerUtf8String)
				{
					num2 = 3;
				}
				else
				{
					if (!(asn1Object is DerOctetString))
					{
						throw new ArgumentException("Bad value type encoding IetfAttrSyntax");
					}
					num2 = 1;
				}
				if (this.valueChoice < 0)
				{
					this.valueChoice = num2;
				}
				if (num2 != this.valueChoice)
				{
					throw new ArgumentException("Mix of value types in IetfAttrSyntax");
				}
				this.values.Add(new Asn1Encodable[]
				{
					asn1Object
				});
			}
		}

		public object[] GetValues()
		{
			if (this.ValueType == 1)
			{
				Asn1OctetString[] array = new Asn1OctetString[this.values.Count];
				for (int num = 0; num != array.Length; num++)
				{
					array[num] = (Asn1OctetString)this.values[num];
				}
				return array;
			}
			if (this.ValueType == 2)
			{
				DerObjectIdentifier[] array2 = new DerObjectIdentifier[this.values.Count];
				for (int num2 = 0; num2 != array2.Length; num2++)
				{
					array2[num2] = (DerObjectIdentifier)this.values[num2];
				}
				return array2;
			}
			DerUtf8String[] array3 = new DerUtf8String[this.values.Count];
			for (int num3 = 0; num3 != array3.Length; num3++)
			{
				array3[num3] = (DerUtf8String)this.values[num3];
			}
			return array3;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.policyAuthority != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(0, this.policyAuthority)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				new DerSequence(this.values)
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
