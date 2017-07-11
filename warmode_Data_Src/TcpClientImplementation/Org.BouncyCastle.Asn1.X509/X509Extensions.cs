using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class X509Extensions : Asn1Encodable
	{
		public static readonly DerObjectIdentifier SubjectDirectoryAttributes = new DerObjectIdentifier("2.5.29.9");

		public static readonly DerObjectIdentifier SubjectKeyIdentifier = new DerObjectIdentifier("2.5.29.14");

		public static readonly DerObjectIdentifier KeyUsage = new DerObjectIdentifier("2.5.29.15");

		public static readonly DerObjectIdentifier PrivateKeyUsagePeriod = new DerObjectIdentifier("2.5.29.16");

		public static readonly DerObjectIdentifier SubjectAlternativeName = new DerObjectIdentifier("2.5.29.17");

		public static readonly DerObjectIdentifier IssuerAlternativeName = new DerObjectIdentifier("2.5.29.18");

		public static readonly DerObjectIdentifier BasicConstraints = new DerObjectIdentifier("2.5.29.19");

		public static readonly DerObjectIdentifier CrlNumber = new DerObjectIdentifier("2.5.29.20");

		public static readonly DerObjectIdentifier ReasonCode = new DerObjectIdentifier("2.5.29.21");

		public static readonly DerObjectIdentifier InstructionCode = new DerObjectIdentifier("2.5.29.23");

		public static readonly DerObjectIdentifier InvalidityDate = new DerObjectIdentifier("2.5.29.24");

		public static readonly DerObjectIdentifier DeltaCrlIndicator = new DerObjectIdentifier("2.5.29.27");

		public static readonly DerObjectIdentifier IssuingDistributionPoint = new DerObjectIdentifier("2.5.29.28");

		public static readonly DerObjectIdentifier CertificateIssuer = new DerObjectIdentifier("2.5.29.29");

		public static readonly DerObjectIdentifier NameConstraints = new DerObjectIdentifier("2.5.29.30");

		public static readonly DerObjectIdentifier CrlDistributionPoints = new DerObjectIdentifier("2.5.29.31");

		public static readonly DerObjectIdentifier CertificatePolicies = new DerObjectIdentifier("2.5.29.32");

		public static readonly DerObjectIdentifier PolicyMappings = new DerObjectIdentifier("2.5.29.33");

		public static readonly DerObjectIdentifier AuthorityKeyIdentifier = new DerObjectIdentifier("2.5.29.35");

		public static readonly DerObjectIdentifier PolicyConstraints = new DerObjectIdentifier("2.5.29.36");

		public static readonly DerObjectIdentifier ExtendedKeyUsage = new DerObjectIdentifier("2.5.29.37");

		public static readonly DerObjectIdentifier FreshestCrl = new DerObjectIdentifier("2.5.29.46");

		public static readonly DerObjectIdentifier InhibitAnyPolicy = new DerObjectIdentifier("2.5.29.54");

		public static readonly DerObjectIdentifier AuthorityInfoAccess = new DerObjectIdentifier("1.3.6.1.5.5.7.1.1");

		public static readonly DerObjectIdentifier SubjectInfoAccess = new DerObjectIdentifier("1.3.6.1.5.5.7.1.11");

		public static readonly DerObjectIdentifier LogoType = new DerObjectIdentifier("1.3.6.1.5.5.7.1.12");

		public static readonly DerObjectIdentifier BiometricInfo = new DerObjectIdentifier("1.3.6.1.5.5.7.1.2");

		public static readonly DerObjectIdentifier QCStatements = new DerObjectIdentifier("1.3.6.1.5.5.7.1.3");

		public static readonly DerObjectIdentifier AuditIdentity = new DerObjectIdentifier("1.3.6.1.5.5.7.1.4");

		public static readonly DerObjectIdentifier NoRevAvail = new DerObjectIdentifier("2.5.29.56");

		public static readonly DerObjectIdentifier TargetInformation = new DerObjectIdentifier("2.5.29.55");

		private readonly IDictionary extensions = Platform.CreateHashtable();

		private readonly IList ordering;

		public IEnumerable ExtensionOids
		{
			get
			{
				return new EnumerableProxy(this.ordering);
			}
		}

		public static X509Extensions GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return X509Extensions.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static X509Extensions GetInstance(object obj)
		{
			if (obj == null || obj is X509Extensions)
			{
				return (X509Extensions)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new X509Extensions((Asn1Sequence)obj);
			}
			if (obj is Asn1TaggedObject)
			{
				return X509Extensions.GetInstance(((Asn1TaggedObject)obj).GetObject());
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		private X509Extensions(Asn1Sequence seq)
		{
			this.ordering = Platform.CreateArrayList();
			foreach (Asn1Encodable asn1Encodable in seq)
			{
				Asn1Sequence instance = Asn1Sequence.GetInstance(asn1Encodable.ToAsn1Object());
				if (instance.Count < 2 || instance.Count > 3)
				{
					throw new ArgumentException("Bad sequence size: " + instance.Count);
				}
				DerObjectIdentifier instance2 = DerObjectIdentifier.GetInstance(instance[0].ToAsn1Object());
				bool critical = instance.Count == 3 && DerBoolean.GetInstance(instance[1].ToAsn1Object()).IsTrue;
				Asn1OctetString instance3 = Asn1OctetString.GetInstance(instance[instance.Count - 1].ToAsn1Object());
				this.extensions.Add(instance2, new X509Extension(critical, instance3));
				this.ordering.Add(instance2);
			}
		}

		public X509Extensions(IDictionary extensions) : this(null, extensions)
		{
		}

		public X509Extensions(IList ordering, IDictionary extensions)
		{
			if (ordering == null)
			{
				this.ordering = Platform.CreateArrayList(extensions.Keys);
			}
			else
			{
				this.ordering = Platform.CreateArrayList(ordering);
			}
			foreach (DerObjectIdentifier key in this.ordering)
			{
				this.extensions.Add(key, (X509Extension)extensions[key]);
			}
		}

		public X509Extensions(IList oids, IList values)
		{
			this.ordering = Platform.CreateArrayList(oids);
			int num = 0;
			foreach (DerObjectIdentifier key in this.ordering)
			{
				this.extensions.Add(key, (X509Extension)values[num++]);
			}
		}

		[Obsolete]
		public X509Extensions(Hashtable extensions) : this(null, extensions)
		{
		}

		[Obsolete]
		public X509Extensions(ArrayList ordering, Hashtable extensions)
		{
			if (ordering == null)
			{
				this.ordering = Platform.CreateArrayList(extensions.Keys);
			}
			else
			{
				this.ordering = Platform.CreateArrayList(ordering);
			}
			foreach (DerObjectIdentifier key in this.ordering)
			{
				this.extensions.Add(key, (X509Extension)extensions[key]);
			}
		}

		[Obsolete]
		public X509Extensions(ArrayList oids, ArrayList values)
		{
			this.ordering = Platform.CreateArrayList(oids);
			int num = 0;
			foreach (DerObjectIdentifier key in this.ordering)
			{
				this.extensions.Add(key, (X509Extension)values[num++]);
			}
		}

		[Obsolete("Use ExtensionOids IEnumerable property")]
		public IEnumerator Oids()
		{
			return this.ExtensionOids.GetEnumerator();
		}

		public X509Extension GetExtension(DerObjectIdentifier oid)
		{
			return (X509Extension)this.extensions[oid];
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			foreach (DerObjectIdentifier derObjectIdentifier in this.ordering)
			{
				X509Extension x509Extension = (X509Extension)this.extensions[derObjectIdentifier];
				Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[]
				{
					derObjectIdentifier
				});
				if (x509Extension.IsCritical)
				{
					asn1EncodableVector2.Add(new Asn1Encodable[]
					{
						DerBoolean.True
					});
				}
				asn1EncodableVector2.Add(new Asn1Encodable[]
				{
					x509Extension.Value
				});
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerSequence(asn1EncodableVector2)
				});
			}
			return new DerSequence(asn1EncodableVector);
		}

		public bool Equivalent(X509Extensions other)
		{
			if (this.extensions.Count != other.extensions.Count)
			{
				return false;
			}
			foreach (DerObjectIdentifier key in this.extensions.Keys)
			{
				if (!this.extensions[key].Equals(other.extensions[key]))
				{
					return false;
				}
			}
			return true;
		}

		public DerObjectIdentifier[] GetExtensionOids()
		{
			return X509Extensions.ToOidArray(this.ordering);
		}

		public DerObjectIdentifier[] GetNonCriticalExtensionOids()
		{
			return this.GetExtensionOids(false);
		}

		public DerObjectIdentifier[] GetCriticalExtensionOids()
		{
			return this.GetExtensionOids(true);
		}

		private DerObjectIdentifier[] GetExtensionOids(bool isCritical)
		{
			IList list = Platform.CreateArrayList();
			foreach (DerObjectIdentifier derObjectIdentifier in this.ordering)
			{
				X509Extension x509Extension = (X509Extension)this.extensions[derObjectIdentifier];
				if (x509Extension.IsCritical == isCritical)
				{
					list.Add(derObjectIdentifier);
				}
			}
			return X509Extensions.ToOidArray(list);
		}

		private static DerObjectIdentifier[] ToOidArray(IList oids)
		{
			DerObjectIdentifier[] array = new DerObjectIdentifier[oids.Count];
			oids.CopyTo(array, 0);
			return array;
		}
	}
}
