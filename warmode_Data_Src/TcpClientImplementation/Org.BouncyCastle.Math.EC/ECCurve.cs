using Org.BouncyCastle.Math.EC.Endo;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Math.Field;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Math.EC
{
	public abstract class ECCurve
	{
		public class Config
		{
			protected ECCurve outer;

			protected int coord;

			protected ECEndomorphism endomorphism;

			protected ECMultiplier multiplier;

			internal Config(ECCurve outer, int coord, ECEndomorphism endomorphism, ECMultiplier multiplier)
			{
				this.outer = outer;
				this.coord = coord;
				this.endomorphism = endomorphism;
				this.multiplier = multiplier;
			}

			public ECCurve.Config SetCoordinateSystem(int coord)
			{
				this.coord = coord;
				return this;
			}

			public ECCurve.Config SetEndomorphism(ECEndomorphism endomorphism)
			{
				this.endomorphism = endomorphism;
				return this;
			}

			public ECCurve.Config SetMultiplier(ECMultiplier multiplier)
			{
				this.multiplier = multiplier;
				return this;
			}

			public ECCurve Create()
			{
				if (!this.outer.SupportsCoordinateSystem(this.coord))
				{
					throw new InvalidOperationException("unsupported coordinate system");
				}
				ECCurve eCCurve = this.outer.CloneCurve();
				if (eCCurve == this.outer)
				{
					throw new InvalidOperationException("implementation returned current curve");
				}
				eCCurve.m_coord = this.coord;
				eCCurve.m_endomorphism = this.endomorphism;
				eCCurve.m_multiplier = this.multiplier;
				return eCCurve;
			}
		}

		public const int COORD_AFFINE = 0;

		public const int COORD_HOMOGENEOUS = 1;

		public const int COORD_JACOBIAN = 2;

		public const int COORD_JACOBIAN_CHUDNOVSKY = 3;

		public const int COORD_JACOBIAN_MODIFIED = 4;

		public const int COORD_LAMBDA_AFFINE = 5;

		public const int COORD_LAMBDA_PROJECTIVE = 6;

		public const int COORD_SKEWED = 7;

		protected readonly IFiniteField m_field;

		protected ECFieldElement m_a;

		protected ECFieldElement m_b;

		protected BigInteger m_order;

		protected BigInteger m_cofactor;

		protected int m_coord;

		protected ECEndomorphism m_endomorphism;

		protected ECMultiplier m_multiplier;

		public abstract int FieldSize
		{
			get;
		}

		public abstract ECPoint Infinity
		{
			get;
		}

		public virtual IFiniteField Field
		{
			get
			{
				return this.m_field;
			}
		}

		public virtual ECFieldElement A
		{
			get
			{
				return this.m_a;
			}
		}

		public virtual ECFieldElement B
		{
			get
			{
				return this.m_b;
			}
		}

		public virtual BigInteger Order
		{
			get
			{
				return this.m_order;
			}
		}

		public virtual BigInteger Cofactor
		{
			get
			{
				return this.m_cofactor;
			}
		}

		public virtual int CoordinateSystem
		{
			get
			{
				return this.m_coord;
			}
		}

		public static int[] GetAllCoordinateSystems()
		{
			return new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7
			};
		}

		protected ECCurve(IFiniteField field)
		{
			this.m_field = field;
		}

		public abstract ECFieldElement FromBigInteger(BigInteger x);

		public virtual ECCurve.Config Configure()
		{
			return new ECCurve.Config(this, this.m_coord, this.m_endomorphism, this.m_multiplier);
		}

		public virtual ECPoint ValidatePoint(BigInteger x, BigInteger y)
		{
			ECPoint eCPoint = this.CreatePoint(x, y);
			if (!eCPoint.IsValid())
			{
				throw new ArgumentException("Invalid point coordinates");
			}
			return eCPoint;
		}

		[Obsolete("Per-point compression property will be removed")]
		public virtual ECPoint ValidatePoint(BigInteger x, BigInteger y, bool withCompression)
		{
			ECPoint eCPoint = this.CreatePoint(x, y, withCompression);
			if (!eCPoint.IsValid())
			{
				throw new ArgumentException("Invalid point coordinates");
			}
			return eCPoint;
		}

		public virtual ECPoint CreatePoint(BigInteger x, BigInteger y)
		{
			return this.CreatePoint(x, y, false);
		}

		[Obsolete("Per-point compression property will be removed")]
		public virtual ECPoint CreatePoint(BigInteger x, BigInteger y, bool withCompression)
		{
			return this.CreateRawPoint(this.FromBigInteger(x), this.FromBigInteger(y), withCompression);
		}

		protected abstract ECCurve CloneCurve();

		protected internal abstract ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression);

		protected internal abstract ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression);

		protected virtual ECMultiplier CreateDefaultMultiplier()
		{
			GlvEndomorphism glvEndomorphism = this.m_endomorphism as GlvEndomorphism;
			if (glvEndomorphism != null)
			{
				return new GlvMultiplier(this, glvEndomorphism);
			}
			return new WNafL2RMultiplier();
		}

		public virtual bool SupportsCoordinateSystem(int coord)
		{
			return coord == 0;
		}

		public virtual PreCompInfo GetPreCompInfo(ECPoint point, string name)
		{
			this.CheckPoint(point);
			PreCompInfo result;
			lock (point)
			{
				IDictionary preCompTable = point.m_preCompTable;
				result = ((preCompTable == null) ? null : ((PreCompInfo)preCompTable[name]));
			}
			return result;
		}

		public virtual void SetPreCompInfo(ECPoint point, string name, PreCompInfo preCompInfo)
		{
			this.CheckPoint(point);
			lock (point)
			{
				IDictionary dictionary = point.m_preCompTable;
				if (dictionary == null)
				{
					dictionary = (point.m_preCompTable = Platform.CreateHashtable(4));
				}
				dictionary[name] = preCompInfo;
			}
		}

		public virtual ECPoint ImportPoint(ECPoint p)
		{
			if (this == p.Curve)
			{
				return p;
			}
			if (p.IsInfinity)
			{
				return this.Infinity;
			}
			p = p.Normalize();
			return this.ValidatePoint(p.XCoord.ToBigInteger(), p.YCoord.ToBigInteger(), p.IsCompressed);
		}

		public virtual void NormalizeAll(ECPoint[] points)
		{
			this.NormalizeAll(points, 0, points.Length, null);
		}

		public virtual void NormalizeAll(ECPoint[] points, int off, int len, ECFieldElement iso)
		{
			this.CheckPoints(points, off, len);
			int coordinateSystem = this.CoordinateSystem;
			if (coordinateSystem == 0 || coordinateSystem == 5)
			{
				if (iso != null)
				{
					throw new ArgumentException("not valid for affine coordinates", "iso");
				}
				return;
			}
			else
			{
				ECFieldElement[] array = new ECFieldElement[len];
				int[] array2 = new int[len];
				int num = 0;
				for (int i = 0; i < len; i++)
				{
					ECPoint eCPoint = points[off + i];
					if (eCPoint != null && (iso != null || !eCPoint.IsNormalized()))
					{
						array[num] = eCPoint.GetZCoord(0);
						array2[num++] = off + i;
					}
				}
				if (num == 0)
				{
					return;
				}
				ECAlgorithms.MontgomeryTrick(array, 0, num, iso);
				for (int j = 0; j < num; j++)
				{
					int num2 = array2[j];
					points[num2] = points[num2].Normalize(array[j]);
				}
				return;
			}
		}

		protected virtual void CheckPoint(ECPoint point)
		{
			if (point == null || this != point.Curve)
			{
				throw new ArgumentException("must be non-null and on this curve", "point");
			}
		}

		protected virtual void CheckPoints(ECPoint[] points)
		{
			this.CheckPoints(points, 0, points.Length);
		}

		protected virtual void CheckPoints(ECPoint[] points, int off, int len)
		{
			if (points == null)
			{
				throw new ArgumentNullException("points");
			}
			if (off < 0 || len < 0 || off > points.Length - len)
			{
				throw new ArgumentException("invalid range specified", "points");
			}
			for (int i = 0; i < len; i++)
			{
				ECPoint eCPoint = points[off + i];
				if (eCPoint != null && this != eCPoint.Curve)
				{
					throw new ArgumentException("entries must be null or on this curve", "points");
				}
			}
		}

		public virtual bool Equals(ECCurve other)
		{
			return this == other || (other != null && (this.Field.Equals(other.Field) && this.A.ToBigInteger().Equals(other.A.ToBigInteger())) && this.B.ToBigInteger().Equals(other.B.ToBigInteger()));
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ECCurve);
		}

		public override int GetHashCode()
		{
			return this.Field.GetHashCode() ^ Integers.RotateLeft(this.A.ToBigInteger().GetHashCode(), 8) ^ Integers.RotateLeft(this.B.ToBigInteger().GetHashCode(), 16);
		}

		protected abstract ECPoint DecompressPoint(int yTilde, BigInteger X1);

		public virtual ECEndomorphism GetEndomorphism()
		{
			return this.m_endomorphism;
		}

		public virtual ECMultiplier GetMultiplier()
		{
			ECMultiplier multiplier;
			lock (this)
			{
				if (this.m_multiplier == null)
				{
					this.m_multiplier = this.CreateDefaultMultiplier();
				}
				multiplier = this.m_multiplier;
			}
			return multiplier;
		}

		public virtual ECPoint DecodePoint(byte[] encoded)
		{
			int num = (this.FieldSize + 7) / 8;
			byte b = encoded[0];
			ECPoint eCPoint;
			switch (b)
			{
			case 0:
				if (encoded.Length != 1)
				{
					throw new ArgumentException("Incorrect length for infinity encoding", "encoded");
				}
				eCPoint = this.Infinity;
				goto IL_15B;
			case 2:
			case 3:
			{
				if (encoded.Length != num + 1)
				{
					throw new ArgumentException("Incorrect length for compressed encoding", "encoded");
				}
				int yTilde = (int)(b & 1);
				BigInteger x = new BigInteger(1, encoded, 1, num);
				eCPoint = this.DecompressPoint(yTilde, x);
				if (!eCPoint.SatisfiesCofactor())
				{
					throw new ArgumentException("Invalid point");
				}
				goto IL_15B;
			}
			case 4:
			{
				if (encoded.Length != 2 * num + 1)
				{
					throw new ArgumentException("Incorrect length for uncompressed encoding", "encoded");
				}
				BigInteger x2 = new BigInteger(1, encoded, 1, num);
				BigInteger y = new BigInteger(1, encoded, 1 + num, num);
				eCPoint = this.ValidatePoint(x2, y);
				goto IL_15B;
			}
			case 6:
			case 7:
			{
				if (encoded.Length != 2 * num + 1)
				{
					throw new ArgumentException("Incorrect length for hybrid encoding", "encoded");
				}
				BigInteger x3 = new BigInteger(1, encoded, 1, num);
				BigInteger bigInteger = new BigInteger(1, encoded, 1 + num, num);
				if (bigInteger.TestBit(0) != (b == 7))
				{
					throw new ArgumentException("Inconsistent Y coordinate in hybrid encoding", "encoded");
				}
				eCPoint = this.ValidatePoint(x3, bigInteger);
				goto IL_15B;
			}
			}
			throw new FormatException("Invalid point encoding " + b);
			IL_15B:
			if (b != 0 && eCPoint.IsInfinity)
			{
				throw new ArgumentException("Invalid infinity encoding", "encoded");
			}
			return eCPoint;
		}
	}
}
