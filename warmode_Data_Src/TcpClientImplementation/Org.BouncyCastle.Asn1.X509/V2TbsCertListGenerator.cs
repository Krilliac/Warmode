using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1.X509
{
	public class V2TbsCertListGenerator
	{
		private DerInteger version = new DerInteger(1);

		private AlgorithmIdentifier signature;

		private X509Name issuer;

		private Time thisUpdate;

		private Time nextUpdate;

		private X509Extensions extensions;

		private IList crlEntries;

		public void SetSignature(AlgorithmIdentifier signature)
		{
			this.signature = signature;
		}

		public void SetIssuer(X509Name issuer)
		{
			this.issuer = issuer;
		}

		public void SetThisUpdate(DerUtcTime thisUpdate)
		{
			this.thisUpdate = new Time(thisUpdate);
		}

		public void SetNextUpdate(DerUtcTime nextUpdate)
		{
			this.nextUpdate = ((nextUpdate != null) ? new Time(nextUpdate) : null);
		}

		public void SetThisUpdate(Time thisUpdate)
		{
			this.thisUpdate = thisUpdate;
		}

		public void SetNextUpdate(Time nextUpdate)
		{
			this.nextUpdate = nextUpdate;
		}

		public void AddCrlEntry(Asn1Sequence crlEntry)
		{
			if (this.crlEntries == null)
			{
				this.crlEntries = Platform.CreateArrayList();
			}
			this.crlEntries.Add(crlEntry);
		}

		public void AddCrlEntry(DerInteger userCertificate, DerUtcTime revocationDate, int reason)
		{
			this.AddCrlEntry(userCertificate, new Time(revocationDate), reason);
		}

		public void AddCrlEntry(DerInteger userCertificate, Time revocationDate, int reason)
		{
			this.AddCrlEntry(userCertificate, revocationDate, reason, null);
		}

		public void AddCrlEntry(DerInteger userCertificate, Time revocationDate, int reason, DerGeneralizedTime invalidityDate)
		{
			IList list = Platform.CreateArrayList();
			IList list2 = Platform.CreateArrayList();
			if (reason != 0)
			{
				CrlReason crlReason = new CrlReason(reason);
				try
				{
					list.Add(X509Extensions.ReasonCode);
					list2.Add(new X509Extension(false, new DerOctetString(crlReason.GetEncoded())));
				}
				catch (IOException arg)
				{
					throw new ArgumentException("error encoding reason: " + arg);
				}
			}
			if (invalidityDate != null)
			{
				try
				{
					list.Add(X509Extensions.InvalidityDate);
					list2.Add(new X509Extension(false, new DerOctetString(invalidityDate.GetEncoded())));
				}
				catch (IOException arg2)
				{
					throw new ArgumentException("error encoding invalidityDate: " + arg2);
				}
			}
			if (list.Count != 0)
			{
				this.AddCrlEntry(userCertificate, revocationDate, new X509Extensions(list, list2));
				return;
			}
			this.AddCrlEntry(userCertificate, revocationDate, null);
		}

		public void AddCrlEntry(DerInteger userCertificate, Time revocationDate, X509Extensions extensions)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				userCertificate,
				revocationDate
			});
			if (extensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					extensions
				});
			}
			this.AddCrlEntry(new DerSequence(asn1EncodableVector));
		}

		public void SetExtensions(X509Extensions extensions)
		{
			this.extensions = extensions;
		}

		public TbsCertificateList GenerateTbsCertList()
		{
			if (this.signature == null || this.issuer == null || this.thisUpdate == null)
			{
				throw new InvalidOperationException("Not all mandatory fields set in V2 TbsCertList generator.");
			}
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.version,
				this.signature,
				this.issuer,
				this.thisUpdate
			});
			if (this.nextUpdate != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.nextUpdate
				});
			}
			if (this.crlEntries != null)
			{
				Asn1Sequence[] array = new Asn1Sequence[this.crlEntries.Count];
				for (int i = 0; i < this.crlEntries.Count; i++)
				{
					array[i] = (Asn1Sequence)this.crlEntries[i];
				}
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerSequence(array)
				});
			}
			if (this.extensions != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(0, this.extensions)
				});
			}
			return new TbsCertificateList(new DerSequence(asn1EncodableVector));
		}
	}
}
