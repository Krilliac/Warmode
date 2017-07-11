using System;

namespace Org.BouncyCastle.Asn1.Smime
{
	public class SmimeCapabilityVector
	{
		private readonly Asn1EncodableVector capabilities = new Asn1EncodableVector(new Asn1Encodable[0]);

		public void AddCapability(DerObjectIdentifier capability)
		{
			this.capabilities.Add(new Asn1Encodable[]
			{
				new DerSequence(capability)
			});
		}

		public void AddCapability(DerObjectIdentifier capability, int value)
		{
			this.capabilities.Add(new Asn1Encodable[]
			{
				new DerSequence(new Asn1Encodable[]
				{
					capability,
					new DerInteger(value)
				})
			});
		}

		public void AddCapability(DerObjectIdentifier capability, Asn1Encodable parameters)
		{
			this.capabilities.Add(new Asn1Encodable[]
			{
				new DerSequence(new Asn1Encodable[]
				{
					capability,
					parameters
				})
			});
		}

		public Asn1EncodableVector ToAsn1EncodableVector()
		{
			return this.capabilities;
		}
	}
}
