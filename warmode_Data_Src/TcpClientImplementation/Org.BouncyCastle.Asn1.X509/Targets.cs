using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class Targets : Asn1Encodable
	{
		private readonly Asn1Sequence targets;

		public static Targets GetInstance(object obj)
		{
			if (obj is Targets)
			{
				return (Targets)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new Targets((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private Targets(Asn1Sequence targets)
		{
			this.targets = targets;
		}

		public Targets(Target[] targets)
		{
			this.targets = new DerSequence(targets);
		}

		public virtual Target[] GetTargets()
		{
			Target[] array = new Target[this.targets.Count];
			for (int i = 0; i < this.targets.Count; i++)
			{
				array[i] = Target.GetInstance(this.targets[i]);
			}
			return array;
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.targets;
		}
	}
}
