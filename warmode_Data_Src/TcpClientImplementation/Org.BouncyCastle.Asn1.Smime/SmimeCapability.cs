using Org.BouncyCastle.Asn1.Pkcs;
using System;

namespace Org.BouncyCastle.Asn1.Smime
{
	public class SmimeCapability : Asn1Encodable
	{
		public static readonly DerObjectIdentifier PreferSignedData = PkcsObjectIdentifiers.PreferSignedData;

		public static readonly DerObjectIdentifier CannotDecryptAny = PkcsObjectIdentifiers.CannotDecryptAny;

		public static readonly DerObjectIdentifier SmimeCapabilitiesVersions = PkcsObjectIdentifiers.SmimeCapabilitiesVersions;

		public static readonly DerObjectIdentifier DesCbc = new DerObjectIdentifier("1.3.14.3.2.7");

		public static readonly DerObjectIdentifier DesEde3Cbc = PkcsObjectIdentifiers.DesEde3Cbc;

		public static readonly DerObjectIdentifier RC2Cbc = PkcsObjectIdentifiers.RC2Cbc;

		private DerObjectIdentifier capabilityID;

		private Asn1Object parameters;

		public DerObjectIdentifier CapabilityID
		{
			get
			{
				return this.capabilityID;
			}
		}

		public Asn1Object Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public SmimeCapability(Asn1Sequence seq)
		{
			this.capabilityID = (DerObjectIdentifier)seq[0].ToAsn1Object();
			if (seq.Count > 1)
			{
				this.parameters = seq[1].ToAsn1Object();
			}
		}

		public SmimeCapability(DerObjectIdentifier capabilityID, Asn1Encodable parameters)
		{
			if (capabilityID == null)
			{
				throw new ArgumentNullException("capabilityID");
			}
			this.capabilityID = capabilityID;
			if (parameters != null)
			{
				this.parameters = parameters.ToAsn1Object();
			}
		}

		public static SmimeCapability GetInstance(object obj)
		{
			if (obj == null || obj is SmimeCapability)
			{
				return (SmimeCapability)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new SmimeCapability((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid SmimeCapability");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.capabilityID
			});
			if (this.parameters != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.parameters
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
