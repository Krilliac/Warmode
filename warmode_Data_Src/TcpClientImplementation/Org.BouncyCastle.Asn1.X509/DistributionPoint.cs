using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class DistributionPoint : Asn1Encodable
	{
		internal readonly DistributionPointName distributionPoint;

		internal readonly ReasonFlags reasons;

		internal readonly GeneralNames cRLIssuer;

		public DistributionPointName DistributionPointName
		{
			get
			{
				return this.distributionPoint;
			}
		}

		public ReasonFlags Reasons
		{
			get
			{
				return this.reasons;
			}
		}

		public GeneralNames CrlIssuer
		{
			get
			{
				return this.cRLIssuer;
			}
		}

		public static DistributionPoint GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DistributionPoint.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static DistributionPoint GetInstance(object obj)
		{
			if (obj == null || obj is DistributionPoint)
			{
				return (DistributionPoint)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new DistributionPoint((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid DistributionPoint: " + obj.GetType().Name);
		}

		private DistributionPoint(Asn1Sequence seq)
		{
			for (int num = 0; num != seq.Count; num++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[num]);
				switch (instance.TagNo)
				{
				case 0:
					this.distributionPoint = DistributionPointName.GetInstance(instance, true);
					break;
				case 1:
					this.reasons = new ReasonFlags(DerBitString.GetInstance(instance, false));
					break;
				case 2:
					this.cRLIssuer = GeneralNames.GetInstance(instance, false);
					break;
				}
			}
		}

		public DistributionPoint(DistributionPointName distributionPointName, ReasonFlags reasons, GeneralNames crlIssuer)
		{
			this.distributionPoint = distributionPointName;
			this.reasons = reasons;
			this.cRLIssuer = crlIssuer;
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (this.distributionPoint != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(0, this.distributionPoint)
				});
			}
			if (this.reasons != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, this.reasons)
				});
			}
			if (this.cRLIssuer != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 2, this.cRLIssuer)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}

		public override string ToString()
		{
			string newLine = Platform.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DistributionPoint: [");
			stringBuilder.Append(newLine);
			if (this.distributionPoint != null)
			{
				this.appendObject(stringBuilder, newLine, "distributionPoint", this.distributionPoint.ToString());
			}
			if (this.reasons != null)
			{
				this.appendObject(stringBuilder, newLine, "reasons", this.reasons.ToString());
			}
			if (this.cRLIssuer != null)
			{
				this.appendObject(stringBuilder, newLine, "cRLIssuer", this.cRLIssuer.ToString());
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
