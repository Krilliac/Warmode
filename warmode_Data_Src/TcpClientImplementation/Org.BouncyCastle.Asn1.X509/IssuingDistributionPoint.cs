using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class IssuingDistributionPoint : Asn1Encodable
	{
		private readonly DistributionPointName _distributionPoint;

		private readonly bool _onlyContainsUserCerts;

		private readonly bool _onlyContainsCACerts;

		private readonly ReasonFlags _onlySomeReasons;

		private readonly bool _indirectCRL;

		private readonly bool _onlyContainsAttributeCerts;

		private readonly Asn1Sequence seq;

		public bool OnlyContainsUserCerts
		{
			get
			{
				return this._onlyContainsUserCerts;
			}
		}

		public bool OnlyContainsCACerts
		{
			get
			{
				return this._onlyContainsCACerts;
			}
		}

		public bool IsIndirectCrl
		{
			get
			{
				return this._indirectCRL;
			}
		}

		public bool OnlyContainsAttributeCerts
		{
			get
			{
				return this._onlyContainsAttributeCerts;
			}
		}

		public DistributionPointName DistributionPoint
		{
			get
			{
				return this._distributionPoint;
			}
		}

		public ReasonFlags OnlySomeReasons
		{
			get
			{
				return this._onlySomeReasons;
			}
		}

		public static IssuingDistributionPoint GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return IssuingDistributionPoint.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static IssuingDistributionPoint GetInstance(object obj)
		{
			if (obj == null || obj is IssuingDistributionPoint)
			{
				return (IssuingDistributionPoint)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new IssuingDistributionPoint((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public IssuingDistributionPoint(DistributionPointName distributionPoint, bool onlyContainsUserCerts, bool onlyContainsCACerts, ReasonFlags onlySomeReasons, bool indirectCRL, bool onlyContainsAttributeCerts)
		{
			this._distributionPoint = distributionPoint;
			this._indirectCRL = indirectCRL;
			this._onlyContainsAttributeCerts = onlyContainsAttributeCerts;
			this._onlyContainsCACerts = onlyContainsCACerts;
			this._onlyContainsUserCerts = onlyContainsUserCerts;
			this._onlySomeReasons = onlySomeReasons;
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			if (distributionPoint != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, distributionPoint)
				});
			}
			if (onlyContainsUserCerts)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 1, DerBoolean.True)
				});
			}
			if (onlyContainsCACerts)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 2, DerBoolean.True)
				});
			}
			if (onlySomeReasons != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 3, onlySomeReasons)
				});
			}
			if (indirectCRL)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 4, DerBoolean.True)
				});
			}
			if (onlyContainsAttributeCerts)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(false, 5, DerBoolean.True)
				});
			}
			this.seq = new DerSequence(asn1EncodableVector);
		}

		private IssuingDistributionPoint(Asn1Sequence seq)
		{
			this.seq = seq;
			for (int num = 0; num != seq.Count; num++)
			{
				Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[num]);
				switch (instance.TagNo)
				{
				case 0:
					this._distributionPoint = DistributionPointName.GetInstance(instance, true);
					break;
				case 1:
					this._onlyContainsUserCerts = DerBoolean.GetInstance(instance, false).IsTrue;
					break;
				case 2:
					this._onlyContainsCACerts = DerBoolean.GetInstance(instance, false).IsTrue;
					break;
				case 3:
					this._onlySomeReasons = new ReasonFlags(DerBitString.GetInstance(instance, false));
					break;
				case 4:
					this._indirectCRL = DerBoolean.GetInstance(instance, false).IsTrue;
					break;
				case 5:
					this._onlyContainsAttributeCerts = DerBoolean.GetInstance(instance, false).IsTrue;
					break;
				default:
					throw new ArgumentException("unknown tag in IssuingDistributionPoint");
				}
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}

		public override string ToString()
		{
			string newLine = Platform.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("IssuingDistributionPoint: [");
			stringBuilder.Append(newLine);
			if (this._distributionPoint != null)
			{
				this.appendObject(stringBuilder, newLine, "distributionPoint", this._distributionPoint.ToString());
			}
			if (this._onlyContainsUserCerts)
			{
				this.appendObject(stringBuilder, newLine, "onlyContainsUserCerts", this._onlyContainsUserCerts.ToString());
			}
			if (this._onlyContainsCACerts)
			{
				this.appendObject(stringBuilder, newLine, "onlyContainsCACerts", this._onlyContainsCACerts.ToString());
			}
			if (this._onlySomeReasons != null)
			{
				this.appendObject(stringBuilder, newLine, "onlySomeReasons", this._onlySomeReasons.ToString());
			}
			if (this._onlyContainsAttributeCerts)
			{
				this.appendObject(stringBuilder, newLine, "onlyContainsAttributeCerts", this._onlyContainsAttributeCerts.ToString());
			}
			if (this._indirectCRL)
			{
				this.appendObject(stringBuilder, newLine, "indirectCRL", this._indirectCRL.ToString());
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
