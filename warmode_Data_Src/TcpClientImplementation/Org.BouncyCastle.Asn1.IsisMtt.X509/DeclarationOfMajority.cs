using System;

namespace Org.BouncyCastle.Asn1.IsisMtt.X509
{
	public class DeclarationOfMajority : Asn1Encodable, IAsn1Choice
	{
		public enum Choice
		{
			NotYoungerThan,
			FullAgeAtCountry,
			DateOfBirth
		}

		private readonly Asn1TaggedObject declaration;

		public DeclarationOfMajority.Choice Type
		{
			get
			{
				return (DeclarationOfMajority.Choice)this.declaration.TagNo;
			}
		}

		public virtual int NotYoungerThan
		{
			get
			{
				DeclarationOfMajority.Choice tagNo = (DeclarationOfMajority.Choice)this.declaration.TagNo;
				if (tagNo == DeclarationOfMajority.Choice.NotYoungerThan)
				{
					return DerInteger.GetInstance(this.declaration, false).Value.IntValue;
				}
				return -1;
			}
		}

		public virtual Asn1Sequence FullAgeAtCountry
		{
			get
			{
				DeclarationOfMajority.Choice tagNo = (DeclarationOfMajority.Choice)this.declaration.TagNo;
				if (tagNo == DeclarationOfMajority.Choice.FullAgeAtCountry)
				{
					return Asn1Sequence.GetInstance(this.declaration, false);
				}
				return null;
			}
		}

		public virtual DerGeneralizedTime DateOfBirth
		{
			get
			{
				DeclarationOfMajority.Choice tagNo = (DeclarationOfMajority.Choice)this.declaration.TagNo;
				if (tagNo == DeclarationOfMajority.Choice.DateOfBirth)
				{
					return DerGeneralizedTime.GetInstance(this.declaration, false);
				}
				return null;
			}
		}

		public DeclarationOfMajority(int notYoungerThan)
		{
			this.declaration = new DerTaggedObject(false, 0, new DerInteger(notYoungerThan));
		}

		public DeclarationOfMajority(bool fullAge, string country)
		{
			if (country.Length > 2)
			{
				throw new ArgumentException("country can only be 2 characters");
			}
			DerPrintableString derPrintableString = new DerPrintableString(country, true);
			DerSequence obj;
			if (fullAge)
			{
				obj = new DerSequence(derPrintableString);
			}
			else
			{
				obj = new DerSequence(new Asn1Encodable[]
				{
					DerBoolean.False,
					derPrintableString
				});
			}
			this.declaration = new DerTaggedObject(false, 1, obj);
		}

		public DeclarationOfMajority(DerGeneralizedTime dateOfBirth)
		{
			this.declaration = new DerTaggedObject(false, 2, dateOfBirth);
		}

		public static DeclarationOfMajority GetInstance(object obj)
		{
			if (obj == null || obj is DeclarationOfMajority)
			{
				return (DeclarationOfMajority)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new DeclarationOfMajority((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private DeclarationOfMajority(Asn1TaggedObject o)
		{
			if (o.TagNo > 2)
			{
				throw new ArgumentException("Bad tag number: " + o.TagNo);
			}
			this.declaration = o;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.declaration;
		}
	}
}
