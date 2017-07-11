using Org.BouncyCastle.Utilities;
using System;

namespace Org.BouncyCastle.Math.Field
{
	internal class GenericPolynomialExtensionField : IPolynomialExtensionField, IExtensionField, IFiniteField
	{
		protected readonly IFiniteField subfield;

		protected readonly IPolynomial minimalPolynomial;

		public virtual BigInteger Characteristic
		{
			get
			{
				return this.subfield.Characteristic;
			}
		}

		public virtual int Dimension
		{
			get
			{
				return this.subfield.Dimension * this.minimalPolynomial.Degree;
			}
		}

		public virtual IFiniteField Subfield
		{
			get
			{
				return this.subfield;
			}
		}

		public virtual int Degree
		{
			get
			{
				return this.minimalPolynomial.Degree;
			}
		}

		public virtual IPolynomial MinimalPolynomial
		{
			get
			{
				return this.minimalPolynomial;
			}
		}

		internal GenericPolynomialExtensionField(IFiniteField subfield, IPolynomial polynomial)
		{
			this.subfield = subfield;
			this.minimalPolynomial = polynomial;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			GenericPolynomialExtensionField genericPolynomialExtensionField = obj as GenericPolynomialExtensionField;
			return genericPolynomialExtensionField != null && this.subfield.Equals(genericPolynomialExtensionField.subfield) && this.minimalPolynomial.Equals(genericPolynomialExtensionField.minimalPolynomial);
		}

		public override int GetHashCode()
		{
			return this.subfield.GetHashCode() ^ Integers.RotateLeft(this.minimalPolynomial.GetHashCode(), 16);
		}
	}
}
