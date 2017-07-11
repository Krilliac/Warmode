using System;
using System.IO;

namespace Org.BouncyCastle.Asn1.Cms
{
	public class SignedDataParser
	{
		private Asn1SequenceParser _seq;

		private DerInteger _version;

		private object _nextObject;

		private bool _certsCalled;

		private bool _crlsCalled;

		public DerInteger Version
		{
			get
			{
				return this._version;
			}
		}

		public static SignedDataParser GetInstance(object o)
		{
			if (o is Asn1Sequence)
			{
				return new SignedDataParser(((Asn1Sequence)o).Parser);
			}
			if (o is Asn1SequenceParser)
			{
				return new SignedDataParser((Asn1SequenceParser)o);
			}
			throw new IOException("unknown object encountered: " + o.GetType().Name);
		}

		public SignedDataParser(Asn1SequenceParser seq)
		{
			this._seq = seq;
			this._version = (DerInteger)seq.ReadObject();
		}

		public Asn1SetParser GetDigestAlgorithms()
		{
			return (Asn1SetParser)this._seq.ReadObject();
		}

		public ContentInfoParser GetEncapContentInfo()
		{
			return new ContentInfoParser((Asn1SequenceParser)this._seq.ReadObject());
		}

		public Asn1SetParser GetCertificates()
		{
			this._certsCalled = true;
			this._nextObject = this._seq.ReadObject();
			if (this._nextObject is Asn1TaggedObjectParser && ((Asn1TaggedObjectParser)this._nextObject).TagNo == 0)
			{
				Asn1SetParser result = (Asn1SetParser)((Asn1TaggedObjectParser)this._nextObject).GetObjectParser(17, false);
				this._nextObject = null;
				return result;
			}
			return null;
		}

		public Asn1SetParser GetCrls()
		{
			if (!this._certsCalled)
			{
				throw new IOException("GetCerts() has not been called.");
			}
			this._crlsCalled = true;
			if (this._nextObject == null)
			{
				this._nextObject = this._seq.ReadObject();
			}
			if (this._nextObject is Asn1TaggedObjectParser && ((Asn1TaggedObjectParser)this._nextObject).TagNo == 1)
			{
				Asn1SetParser result = (Asn1SetParser)((Asn1TaggedObjectParser)this._nextObject).GetObjectParser(17, false);
				this._nextObject = null;
				return result;
			}
			return null;
		}

		public Asn1SetParser GetSignerInfos()
		{
			if (!this._certsCalled || !this._crlsCalled)
			{
				throw new IOException("GetCerts() and/or GetCrls() has not been called.");
			}
			if (this._nextObject == null)
			{
				this._nextObject = this._seq.ReadObject();
			}
			return (Asn1SetParser)this._nextObject;
		}
	}
}
