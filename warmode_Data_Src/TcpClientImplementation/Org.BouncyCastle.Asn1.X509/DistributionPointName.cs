using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class DistributionPointName : Asn1Encodable, IAsn1Choice
	{
		public const int FullName = 0;

		public const int NameRelativeToCrlIssuer = 1;

		internal readonly Asn1Encodable name;

		internal readonly int type;

		public int PointType
		{
			get
			{
				return this.type;
			}
		}

		public Asn1Encodable Name
		{
			get
			{
				return this.name;
			}
		}

		public static DistributionPointName GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DistributionPointName.GetInstance(Asn1TaggedObject.GetInstance(obj, true));
		}

		public static DistributionPointName GetInstance(object obj)
		{
			if (obj == null || obj is DistributionPointName)
			{
				return (DistributionPointName)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new DistributionPointName((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public DistributionPointName(int type, Asn1Encodable name)
		{
			this.type = type;
			this.name = name;
		}

		public DistributionPointName(GeneralNames name) : this(0, name)
		{
		}

		public DistributionPointName(Asn1TaggedObject obj)
		{
			this.type = obj.TagNo;
			if (this.type == 0)
			{
				this.name = GeneralNames.GetInstance(obj, false);
				return;
			}
			this.name = Asn1Set.GetInstance(obj, false);
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerTaggedObject(false, this.type, this.name);
		}

		public override string ToString()
		{
			string newLine = Platform.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DistributionPointName: [");
			stringBuilder.Append(newLine);
			if (this.type == 0)
			{
				this.appendObject(stringBuilder, newLine, "fullName", this.name.ToString());
			}
			else
			{
				this.appendObject(stringBuilder, newLine, "nameRelativeToCRLIssuer", this.name.ToString());
			}
			stringBuilder.Append("]");
			stringBuilder.Append(newLine);
			return stringBuilder.ToString();
		}

		private void appendObject(StringBuilder buf, string sep, string name, string val)
		{
			string value = "    ";
			buf.Append(value);
			buf.Append(name);
			buf.Append(":");
			buf.Append(sep);
			buf.Append(value);
			buf.Append(value);
			buf.Append(val);
			buf.Append(sep);
		}
	}
}
