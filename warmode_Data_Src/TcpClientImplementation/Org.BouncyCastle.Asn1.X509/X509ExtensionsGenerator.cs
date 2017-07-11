using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1.X509
{
	public class X509ExtensionsGenerator
	{
		private IDictionary extensions = Platform.CreateHashtable();

		private IList extOrdering = Platform.CreateArrayList();

		public bool IsEmpty
		{
			get
			{
				return this.extOrdering.Count < 1;
			}
		}

		public void Reset()
		{
			this.extensions = Platform.CreateHashtable();
			this.extOrdering = Platform.CreateArrayList();
		}

		public void AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable extValue)
		{
			byte[] derEncoded;
			try
			{
				derEncoded = extValue.GetDerEncoded();
			}
			catch (Exception arg)
			{
				throw new ArgumentException("error encoding value: " + arg);
			}
			this.AddExtension(oid, critical, derEncoded);
		}

		public void AddExtension(DerObjectIdentifier oid, bool critical, byte[] extValue)
		{
			if (this.extensions.Contains(oid))
			{
				throw new ArgumentException("extension " + oid + " already added");
			}
			this.extOrdering.Add(oid);
			this.extensions.Add(oid, new X509Extension(critical, new DerOctetString(extValue)));
		}

		public X509Extensions Generate()
		{
			return new X509Extensions(this.extOrdering, this.extensions);
		}
	}
}
