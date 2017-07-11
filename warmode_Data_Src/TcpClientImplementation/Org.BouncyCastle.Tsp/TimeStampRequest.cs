using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Tsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Tsp
{
	public class TimeStampRequest : X509ExtensionBase
	{
		private TimeStampReq req;

		private X509Extensions extensions;

		public int Version
		{
			get
			{
				return this.req.Version.Value.IntValue;
			}
		}

		public string MessageImprintAlgOid
		{
			get
			{
				return this.req.MessageImprint.HashAlgorithm.ObjectID.Id;
			}
		}

		public string ReqPolicy
		{
			get
			{
				if (this.req.ReqPolicy != null)
				{
					return this.req.ReqPolicy.Id;
				}
				return null;
			}
		}

		public BigInteger Nonce
		{
			get
			{
				if (this.req.Nonce != null)
				{
					return this.req.Nonce.Value;
				}
				return null;
			}
		}

		public bool CertReq
		{
			get
			{
				return this.req.CertReq != null && this.req.CertReq.IsTrue;
			}
		}

		internal X509Extensions Extensions
		{
			get
			{
				return this.req.Extensions;
			}
		}

		public virtual bool HasExtensions
		{
			get
			{
				return this.extensions != null;
			}
		}

		public TimeStampRequest(TimeStampReq req)
		{
			this.req = req;
			this.extensions = req.Extensions;
		}

		public TimeStampRequest(byte[] req) : this(new Asn1InputStream(req))
		{
		}

		public TimeStampRequest(Stream input) : this(new Asn1InputStream(input))
		{
		}

		private TimeStampRequest(Asn1InputStream str)
		{
			try
			{
				this.req = TimeStampReq.GetInstance(str.ReadObject());
			}
			catch (InvalidCastException arg)
			{
				throw new IOException("malformed request: " + arg);
			}
			catch (ArgumentException arg2)
			{
				throw new IOException("malformed request: " + arg2);
			}
		}

		public byte[] GetMessageImprintDigest()
		{
			return this.req.MessageImprint.GetHashedMessage();
		}

		public void Validate(IList algorithms, IList policies, IList extensions)
		{
			if (!algorithms.Contains(this.MessageImprintAlgOid))
			{
				throw new TspValidationException("request contains unknown algorithm.", 128);
			}
			if (policies != null && this.ReqPolicy != null && !policies.Contains(this.ReqPolicy))
			{
				throw new TspValidationException("request contains unknown policy.", 256);
			}
			if (this.Extensions != null && extensions != null)
			{
				foreach (DerObjectIdentifier derObjectIdentifier in this.Extensions.ExtensionOids)
				{
					if (!extensions.Contains(derObjectIdentifier.Id))
					{
						throw new TspValidationException("request contains unknown extension.", 8388608);
					}
				}
			}
			int digestLength = TspUtil.GetDigestLength(this.MessageImprintAlgOid);
			if (digestLength != this.GetMessageImprintDigest().Length)
			{
				throw new TspValidationException("imprint digest the wrong length.", 4);
			}
		}

		public byte[] GetEncoded()
		{
			return this.req.GetEncoded();
		}

		public virtual X509Extension GetExtension(DerObjectIdentifier oid)
		{
			if (this.extensions != null)
			{
				return this.extensions.GetExtension(oid);
			}
			return null;
		}

		public virtual IList GetExtensionOids()
		{
			return TspUtil.GetExtensionOids(this.extensions);
		}

		protected override X509Extensions GetX509Extensions()
		{
			return this.Extensions;
		}
	}
}
