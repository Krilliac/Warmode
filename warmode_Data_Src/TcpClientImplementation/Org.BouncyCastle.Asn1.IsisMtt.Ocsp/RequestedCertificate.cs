using Org.BouncyCastle.Asn1.X509;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1.IsisMtt.Ocsp
{
	public class RequestedCertificate : Asn1Encodable, IAsn1Choice
	{
		public enum Choice
		{
			Certificate = -1,
			PublicKeyCertificate,
			AttributeCertificate
		}

		private readonly X509CertificateStructure cert;

		private readonly byte[] publicKeyCert;

		private readonly byte[] attributeCert;

		public RequestedCertificate.Choice Type
		{
			get
			{
				if (this.cert != null)
				{
					return RequestedCertificate.Choice.Certificate;
				}
				if (this.publicKeyCert != null)
				{
					return RequestedCertificate.Choice.PublicKeyCertificate;
				}
				return RequestedCertificate.Choice.AttributeCertificate;
			}
		}

		public static RequestedCertificate GetInstance(object obj)
		{
			if (obj == null || obj is RequestedCertificate)
			{
				return (RequestedCertificate)obj;
			}
			if (obj is Asn1Sequence)
			{
				return new RequestedCertificate(X509CertificateStructure.GetInstance(obj));
			}
			if (obj is Asn1TaggedObject)
			{
				return new RequestedCertificate((Asn1TaggedObject)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public static RequestedCertificate GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			if (!isExplicit)
			{
				throw new ArgumentException("choice item must be explicitly tagged");
			}
			return RequestedCertificate.GetInstance(obj.GetObject());
		}

		private RequestedCertificate(Asn1TaggedObject tagged)
		{
			switch (tagged.TagNo)
			{
			case 0:
				this.publicKeyCert = Asn1OctetString.GetInstance(tagged, true).GetOctets();
				return;
			case 1:
				this.attributeCert = Asn1OctetString.GetInstance(tagged, true).GetOctets();
				return;
			default:
				throw new ArgumentException("unknown tag number: " + tagged.TagNo);
			}
		}

		public RequestedCertificate(X509CertificateStructure certificate)
		{
			this.cert = certificate;
		}

		public RequestedCertificate(RequestedCertificate.Choice type, byte[] certificateOctets) : this(new DerTaggedObject((int)type, new DerOctetString(certificateOctets)))
		{
		}

		public byte[] GetCertificateBytes()
		{
			if (this.cert != null)
			{
				try
				{
					return this.cert.GetEncoded();
				}
				catch (IOException arg)
				{
					throw new InvalidOperationException("can't decode certificate: " + arg);
				}
			}
			if (this.publicKeyCert != null)
			{
				return this.publicKeyCert;
			}
			return this.attributeCert;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.publicKeyCert != null)
			{
				return new DerTaggedObject(0, new DerOctetString(this.publicKeyCert));
			}
			if (this.attributeCert != null)
			{
				return new DerTaggedObject(1, new DerOctetString(this.attributeCert));
			}
			return this.cert.ToAsn1Object();
		}
	}
}
