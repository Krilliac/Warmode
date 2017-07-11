using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class Target : Asn1Encodable, IAsn1Choice
	{
		public enum Choice
		{
			Name,
			Group
		}

		private readonly GeneralName targetName;

		private readonly GeneralName targetGroup;

		public virtual GeneralName TargetGroup
		{
			get
			{
				return this.targetGroup;
			}
		}

		public virtual GeneralName TargetName
		{
			get
			{
				return this.targetName;
			}
		}

		public static Target GetInstance(object obj)
		{
			if (obj is Target)
			{
				return (Target)obj;
			}
			if (obj is Asn1TaggedObject)
			{
				return new Target((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private Target(Asn1TaggedObject tagObj)
		{
			switch (tagObj.TagNo)
			{
			case 0:
				this.targetName = GeneralName.GetInstance(tagObj, true);
				return;
			case 1:
				this.targetGroup = GeneralName.GetInstance(tagObj, true);
				return;
			default:
				throw new ArgumentException("unknown tag: " + tagObj.TagNo);
			}
		}

		public Target(Target.Choice type, GeneralName name) : this(new DerTaggedObject((int)type, name))
		{
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.targetName != null)
			{
				return new DerTaggedObject(true, 0, this.targetName);
			}
			return new DerTaggedObject(true, 1, this.targetGroup);
		}
	}
}
