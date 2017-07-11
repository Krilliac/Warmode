using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class ExtendedKeyUsage : Asn1Encodable
	{
		internal readonly IDictionary usageTable = Platform.CreateHashtable();

		internal readonly Asn1Sequence seq;

		public int Count
		{
			get
			{
				return this.usageTable.Count;
			}
		}

		public static ExtendedKeyUsage GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return ExtendedKeyUsage.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static ExtendedKeyUsage GetInstance(object obj)
		{
			if (obj is ExtendedKeyUsage)
			{
				return (ExtendedKeyUsage)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new ExtendedKeyUsage((Asn1Sequence)obj);
			}
			if (obj is X509Extension)
			{
				return ExtendedKeyUsage.GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
			}
			throw new ArgumentException("Invalid ExtendedKeyUsage: " + obj.GetType().Name);
		}

		private ExtendedKeyUsage(Asn1Sequence seq)
		{
			this.seq = seq;
			foreach (object current in seq)
			{
				if (!(current is DerObjectIdentifier))
				{
					throw new ArgumentException("Only DerObjectIdentifier instances allowed in ExtendedKeyUsage.");
				}
				this.usageTable[current] = current;
			}
		}

		public ExtendedKeyUsage(params KeyPurposeID[] usages)
		{
			this.seq = new DerSequence(usages);
			for (int i = 0; i < usages.Length; i++)
			{
				KeyPurposeID keyPurposeID = usages[i];
				this.usageTable[keyPurposeID] = keyPurposeID;
			}
		}

		[Obsolete]
		public ExtendedKeyUsage(ArrayList usages) : this(usages)
		{
		}

		public ExtendedKeyUsage(IEnumerable usages)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (object current in usages)
			{
				Asn1Encodable instance = DerObjectIdentifier.GetInstance(current);
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					instance
				});
				this.usageTable[instance] = instance;
			}
			this.seq = new DerSequence(asn1EncodableVector);
		}

		public bool HasKeyPurposeId(KeyPurposeID keyPurposeId)
		{
			return this.usageTable.Contains(keyPurposeId);
		}

		[Obsolete("Use 'GetAllUsages'")]
		public ArrayList GetUsages()
		{
			return new ArrayList(this.usageTable.Values);
		}

		public IList GetAllUsages()
		{
			return Platform.CreateArrayList(this.usageTable.Values);
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}
	}
}
