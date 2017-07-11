using Org.BouncyCastle.Utilities;
using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class CrlDistPoint : Asn1Encodable
	{
		internal readonly Asn1Sequence seq;

		public static CrlDistPoint GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return CrlDistPoint.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static CrlDistPoint GetInstance(object obj)
		{
			if (obj is CrlDistPoint || obj == null)
			{
				return (CrlDistPoint)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new CrlDistPoint((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private CrlDistPoint(Asn1Sequence seq)
		{
			this.seq = seq;
		}

		public CrlDistPoint(DistributionPoint[] points)
		{
			this.seq = new DerSequence(points);
		}

		public DistributionPoint[] GetDistributionPoints()
		{
			DistributionPoint[] array = new DistributionPoint[this.seq.Count];
			for (int num = 0; num != this.seq.Count; num++)
			{
				array[num] = DistributionPoint.GetInstance(this.seq[num]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string newLine = Platform.NewLine;
			stringBuilder.Append("CRLDistPoint:");
			stringBuilder.Append(newLine);
			DistributionPoint[] distributionPoints = this.GetDistributionPoints();
			for (int num = 0; num != distributionPoints.Length; num++)
			{
				stringBuilder.Append("    ");
				stringBuilder.Append(distributionPoints[num]);
				stringBuilder.Append(newLine);
			}
			return stringBuilder.ToString();
		}
	}
}
