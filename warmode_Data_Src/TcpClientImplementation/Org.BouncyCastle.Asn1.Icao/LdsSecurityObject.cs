using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.Icao
{
	public class LdsSecurityObject : Asn1Encodable
	{
		public const int UBDataGroups = 16;

		private DerInteger version = new DerInteger(0);

		private AlgorithmIdentifier digestAlgorithmIdentifier;

		private DataGroupHash[] datagroupHash;

		private LdsVersionInfo versionInfo;

		public BigInteger Version
		{
			get
			{
				return this.version.Value;
			}
		}

		public AlgorithmIdentifier DigestAlgorithmIdentifier
		{
			get
			{
				return this.digestAlgorithmIdentifier;
			}
		}

		public LdsVersionInfo VersionInfo
		{
			get
			{
				return this.versionInfo;
			}
		}

		public static LdsSecurityObject GetInstance(object obj)
		{
			if (obj is LdsSecurityObject)
			{
				return (LdsSecurityObject)obj;
			}
			if (obj != null)
			{
				return new LdsSecurityObject(Asn1Sequence.GetInstance(obj));
			}
			return null;
		}

		private LdsSecurityObject(Asn1Sequence seq)
		{
			if (seq == null || seq.Count == 0)
			{
				throw new ArgumentException("null or empty sequence passed.");
			}
			IEnumerator enumerator = seq.GetEnumerator();
			enumerator.MoveNext();
			this.version = DerInteger.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			this.digestAlgorithmIdentifier = AlgorithmIdentifier.GetInstance(enumerator.Current);
			enumerator.MoveNext();
			Asn1Sequence instance = Asn1Sequence.GetInstance(enumerator.Current);
			if (this.version.Value.Equals(BigInteger.One))
			{
				enumerator.MoveNext();
				this.versionInfo = LdsVersionInfo.GetInstance(enumerator.Current);
			}
			this.CheckDatagroupHashSeqSize(instance.Count);
			this.datagroupHash = new DataGroupHash[instance.Count];
			for (int i = 0; i < instance.Count; i++)
			{
				this.datagroupHash[i] = DataGroupHash.GetInstance(instance[i]);
			}
		}

		public LdsSecurityObject(AlgorithmIdentifier digestAlgorithmIdentifier, DataGroupHash[] datagroupHash)
		{
			this.version = new DerInteger(0);
			this.digestAlgorithmIdentifier = digestAlgorithmIdentifier;
			this.datagroupHash = datagroupHash;
			this.CheckDatagroupHashSeqSize(datagroupHash.Length);
		}

		public LdsSecurityObject(AlgorithmIdentifier digestAlgorithmIdentifier, DataGroupHash[] datagroupHash, LdsVersionInfo versionInfo)
		{
			this.version = new DerInteger(1);
			this.digestAlgorithmIdentifier = digestAlgorithmIdentifier;
			this.datagroupHash = datagroupHash;
			this.versionInfo = versionInfo;
			this.CheckDatagroupHashSeqSize(datagroupHash.Length);
		}

		private void CheckDatagroupHashSeqSize(int size)
		{
			if (size < 2 || size > 16)
			{
				throw new ArgumentException("wrong size in DataGroupHashValues : not in (2.." + 16 + ")");
			}
		}

		public DataGroupHash[] GetDatagroupHash()
		{
			return this.datagroupHash;
		}

		public override Asn1Object ToAsn1Object()
		{
			DerSequence derSequence = new DerSequence(this.datagroupHash);
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.digestAlgorithmIdentifier,
				derSequence
			});
			if (this.versionInfo != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.versionInfo
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
