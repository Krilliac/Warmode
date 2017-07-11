using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class RevRepContentBuilder
	{
		private readonly Asn1EncodableVector status = new Asn1EncodableVector(new Asn1Encodable[0]);

		private readonly Asn1EncodableVector revCerts = new Asn1EncodableVector(new Asn1Encodable[0]);

		private readonly Asn1EncodableVector crls = new Asn1EncodableVector(new Asn1Encodable[0]);

		public virtual RevRepContentBuilder Add(PkiStatusInfo status)
		{
			this.status.Add(new Asn1Encodable[]
			{
				status
			});
			return this;
		}

		public virtual RevRepContentBuilder Add(PkiStatusInfo status, CertId certId)
		{
			if (this.status.Count != this.revCerts.Count)
			{
				throw new InvalidOperationException("status and revCerts sequence must be in common order");
			}
			this.status.Add(new Asn1Encodable[]
			{
				status
			});
			this.revCerts.Add(new Asn1Encodable[]
			{
				certId
			});
			return this;
		}

		public virtual RevRepContentBuilder AddCrl(CertificateList crl)
		{
			this.crls.Add(new Asn1Encodable[]
			{
				crl
			});
			return this;
		}

		public virtual RevRepContent Build()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			asn1EncodableVector.Add(new Asn1Encodable[]
			{
				new DerSequence(this.status)
			});
			if (this.revCerts.Count != 0)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 0, new DerSequence(this.revCerts))
				});
			}
			if (this.crls.Count != 0)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerTaggedObject(true, 1, new DerSequence(this.crls))
				});
			}
			return RevRepContent.GetInstance(new DerSequence(asn1EncodableVector));
		}
	}
}
