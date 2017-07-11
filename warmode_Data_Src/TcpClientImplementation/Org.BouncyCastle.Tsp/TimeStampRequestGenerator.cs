using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampRequestGenerator
	{
		private DerObjectIdentifier reqPolicy;

		private DerBoolean certReq;

		private IDictionary extensions = Platform.CreateHashtable();

		private IList extOrdering = Platform.CreateArrayList();

		public void SetReqPolicy(string reqPolicy)
		{
			this.reqPolicy = new DerObjectIdentifier(reqPolicy);
		}

		public void SetCertReq(bool certReq)
		{
			this.certReq = DerBoolean.GetInstance(certReq);
		}

		[Obsolete("Use method taking DerObjectIdentifier")]
		public void AddExtension(string oid, bool critical, Asn1Encodable value)
		{
			this.AddExtension(oid, critical, value.GetEncoded());
		}

		[Obsolete("Use method taking DerObjectIdentifier")]
		public void AddExtension(string oid, bool critical, byte[] value)
		{
			DerObjectIdentifier derObjectIdentifier = new DerObjectIdentifier(oid);
			this.extensions[derObjectIdentifier] = new X509Extension(critical, new DerOctetString(value));
			this.extOrdering.Add(derObjectIdentifier);
		}

		public virtual void AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable extValue)
		{
			this.AddExtension(oid, critical, extValue.GetEncoded());
		}

		public virtual void AddExtension(DerObjectIdentifier oid, bool critical, byte[] extValue)
		{
			this.extensions.Add(oid, new X509Extension(critical, new DerOctetString(extValue)));
			this.extOrdering.Add(oid);
		}

		public TimeStampRequest Generate(string digestAlgorithm, byte[] digest)
		{
			return this.Generate(digestAlgorithm, digest, null);
		}

		public TimeStampRequest Generate(string digestAlgorithmOid, byte[] digest, BigInteger nonce)
		{
			if (digestAlgorithmOid == null)
			{
				throw new ArgumentException("No digest algorithm specified");
			}
			DerObjectIdentifier objectID = new DerObjectIdentifier(digestAlgorithmOid);
			AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(objectID, DerNull.Instance);
			MessageImprint messageImprint = new MessageImprint(hashAlgorithm, digest);
			X509Extensions x509Extensions = null;
			if (this.extOrdering.Count != 0)
			{
				x509Extensions = new X509Extensions(this.extOrdering, this.extensions);
			}
			DerInteger nonce2 = (nonce == null) ? null : new DerInteger(nonce);
			return new TimeStampRequest(new TimeStampReq(messageImprint, this.reqPolicy, nonce2, this.certReq, x509Extensions));
		}

		public virtual TimeStampRequest Generate(DerObjectIdentifier digestAlgorithm, byte[] digest)
		{
			return this.Generate(digestAlgorithm.Id, digest);
		}

		public virtual TimeStampRequest Generate(DerObjectIdentifier digestAlgorithm, byte[] digest, BigInteger nonce)
		{
			return this.Generate(digestAlgorithm.Id, digest, nonce);
		}
	}
}
