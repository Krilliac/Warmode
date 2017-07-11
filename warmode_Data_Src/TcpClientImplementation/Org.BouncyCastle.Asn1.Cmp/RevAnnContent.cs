using Org.BouncyCastle.Asn1.Crmf;
using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Asn1.Cmp
{
	public class RevAnnContent : Asn1Encodable
	{
		private readonly PkiStatusEncodable status;

		private readonly CertId certId;

		private readonly DerGeneralizedTime willBeRevokedAt;

		private readonly DerGeneralizedTime badSinceDate;

		private readonly X509Extensions crlDetails;

		public virtual PkiStatusEncodable Status
		{
			get
			{
				return this.status;
			}
		}

		public virtual CertId CertID
		{
			get
			{
				return this.certId;
			}
		}

		public virtual DerGeneralizedTime WillBeRevokedAt
		{
			get
			{
				return this.willBeRevokedAt;
			}
		}

		public virtual DerGeneralizedTime BadSinceDate
		{
			get
			{
				return this.badSinceDate;
			}
		}

		public virtual X509Extensions CrlDetails
		{
			get
			{
				return this.crlDetails;
			}
		}

		private RevAnnContent(Asn1Sequence seq)
		{
			this.status = PkiStatusEncodable.GetInstance(seq[0]);
			this.certId = CertId.GetInstance(seq[1]);
			this.willBeRevokedAt = DerGeneralizedTime.GetInstance(seq[2]);
			this.badSinceDate = DerGeneralizedTime.GetInstance(seq[3]);
			if (seq.Count > 4)
			{
				this.crlDetails = X509Extensions.GetInstance(seq[4]);
			}
		}

		public static RevAnnContent GetInstance(object obj)
		{
			if (obj is RevAnnContent)
			{
				return (RevAnnContent)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RevAnnContent((Asn1Sequence)obj);
			}
			throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.status,
				this.certId,
				this.willBeRevokedAt,
				this.badSinceDate
			});
			asn1EncodableVector.AddOptional(new Asn1Encodable[]
			{
				this.crlDetails
			});
			return new DerSequence(asn1EncodableVector);
		}
	}
}
