using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class TbsCertificateList : Asn1Encodable
	{
		private class RevokedCertificatesEnumeration : IEnumerable
		{
			private class RevokedCertificatesEnumerator : IEnumerator
			{
				private readonly IEnumerator e;

				public object Current
				{
					get
					{
						return new CrlEntry(Asn1Sequence.GetInstance(this.e.Current));
					}
				}

				internal RevokedCertificatesEnumerator(IEnumerator e)
				{
					this.e = e;
				}

				public bool MoveNext()
				{
					return this.e.MoveNext();
				}

				public void Reset()
				{
					this.e.Reset();
				}
			}

			private readonly IEnumerable en;

			internal RevokedCertificatesEnumeration(IEnumerable en)
			{
				this.en = en;
			}

			public IEnumerator GetEnumerator()
			{
				return new TbsCertificateList.RevokedCertificatesEnumeration.RevokedCertificatesEnumerator(this.en.GetEnumerator());
			}
		}

		internal Asn1Sequence seq;

		internal DerInteger version;

		internal AlgorithmIdentifier signature;

		internal X509Name issuer;

		internal Time thisUpdate;

		internal Time nextUpdate;

		internal Asn1Sequence revokedCertificates;

		internal X509Extensions crlExtensions;

		public int Version
		{
			get
			{
				return this.version.Value.IntValue + 1;
			}
		}

		public DerInteger VersionNumber
		{
			get
			{
				return this.version;
			}
		}

		public AlgorithmIdentifier Signature
		{
			get
			{
				return this.signature;
			}
		}

		public X509Name Issuer
		{
			get
			{
				return this.issuer;
			}
		}

		public Time ThisUpdate
		{
			get
			{
				return this.thisUpdate;
			}
		}

		public Time NextUpdate
		{
			get
			{
				return this.nextUpdate;
			}
		}

		public X509Extensions Extensions
		{
			get
			{
				return this.crlExtensions;
			}
		}

		public static TbsCertificateList GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return TbsCertificateList.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static TbsCertificateList GetInstance(object obj)
		{
			TbsCertificateList tbsCertificateList = obj as TbsCertificateList;
			if (obj == null || tbsCertificateList != null)
			{
				return tbsCertificateList;
			}
			if (obj is Asn1Sequence)
			{
				return new TbsCertificateList((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		internal TbsCertificateList(Asn1Sequence seq)
		{
			if (seq.Count < 3 || seq.Count > 7)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			int num = 0;
			this.seq = seq;
			if (seq[num] is DerInteger)
			{
				this.version = DerInteger.GetInstance(seq[num++]);
			}
			else
			{
				this.version = new DerInteger(0);
			}
			this.signature = AlgorithmIdentifier.GetInstance(seq[num++]);
			this.issuer = X509Name.GetInstance(seq[num++]);
			this.thisUpdate = Time.GetInstance(seq[num++]);
			if (num < seq.Count && (seq[num] is DerUtcTime || seq[num] is DerGeneralizedTime || seq[num] is Time))
			{
				this.nextUpdate = Time.GetInstance(seq[num++]);
			}
			if (num < seq.Count && !(seq[num] is DerTaggedObject))
			{
				this.revokedCertificates = Asn1Sequence.GetInstance(seq[num++]);
			}
			if (num < seq.Count && seq[num] is DerTaggedObject)
			{
				this.crlExtensions = X509Extensions.GetInstance(seq[num]);
			}
		}

		public CrlEntry[] GetRevokedCertificates()
		{
			if (this.revokedCertificates == null)
			{
				return new CrlEntry[0];
			}
			CrlEntry[] array = new CrlEntry[this.revokedCertificates.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new CrlEntry(Asn1Sequence.GetInstance(this.revokedCertificates[i]));
			}
			return array;
		}

		public IEnumerable GetRevokedCertificateEnumeration()
		{
			if (this.revokedCertificates == null)
			{
				return EmptyEnumerable.Instance;
			}
			return new TbsCertificateList.RevokedCertificatesEnumeration(this.revokedCertificates);
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.seq;
		}
	}
}
