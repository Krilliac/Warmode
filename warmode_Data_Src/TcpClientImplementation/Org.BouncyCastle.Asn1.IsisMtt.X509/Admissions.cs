using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class Admissions : Asn1Encodable
	{
		private readonly GeneralName admissionAuthority;

		private readonly NamingAuthority namingAuthority;

		private readonly Asn1Sequence professionInfos;

		public virtual GeneralName AdmissionAuthority
		{
			get
			{
				return this.admissionAuthority;
			}
		}

		public virtual NamingAuthority NamingAuthority
		{
			get
			{
				return this.namingAuthority;
			}
		}

		public static Admissions GetInstance(object obj)
		{
			if (obj == null || obj is Admissions)
			{
				return (Admissions)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Admissions((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private Admissions(Asn1Sequence seq)
		{
			if (seq.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			Asn1Encodable asn1Encodable = (Asn1Encodable)enumerator.Current;
			if (asn1Encodable is Asn1TaggedObject)
			{
				switch (((Asn1TaggedObject)asn1Encodable).TagNo)
				{
				case 0:
					this.admissionAuthority = GeneralName.GetInstance((Asn1TaggedObject)asn1Encodable, true);
					break;
				case 1:
					this.namingAuthority = NamingAuthority.GetInstance((Asn1TaggedObject)asn1Encodable, true);
					break;
				default:
					throw new ArgumentException("Bad tag number: " + ((Asn1TaggedObject)asn1Encodable).TagNo);
				}
				enumerator.MoveNext();
				asn1Encodable = (Asn1Encodable)enumerator.Current;
			}
			if (asn1Encodable is Asn1TaggedObject)
			{
				int tagNo = ((Asn1TaggedObject)asn1Encodable).TagNo;
				if (tagNo != 1)
				{
					throw new ArgumentException("Bad tag number: " + ((Asn1TaggedObject)asn1Encodable).TagNo);
				}
				this.namingAuthority = NamingAuthority.GetInstance((Asn1TaggedObject)asn1Encodable, true);
				enumerator.MoveNext();
				asn1Encodable = (Asn1Encodable)enumerator.Current;
			}
			this.professionInfos = Asn1Sequence.GetInstance(asn1Encodable);
			if (enumerator.MoveNext())
			{
				throw new ArgumentException("Bad object encountered: " + enumerator.Current.GetType().Name);
			}
		}

		public Admissions(GeneralName admissionAuthority, NamingAuthority namingAuthority, ProfessionInfo[] professionInfos)
		{
			this.admissionAuthority = admissionAuthority;
			this.namingAuthority = namingAuthority;
			this.professionInfos = new DerSequence(professionInfos);
		}

		public ProfessionInfo[] GetProfessionInfos()
		{
			ProfessionInfo[] array = new ProfessionInfo[this.professionInfos.Count];
			int num = 0;
			foreach (Asn1Encodable obj in this.professionInfos)
			{
				array[num++] = ProfessionInfo.GetInstance(obj);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.admissionAuthority != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, this.admissionAuthority)
				});
			}
			if (this.namingAuthority != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, this.namingAuthority)
				});
			}
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				this.professionInfos
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
