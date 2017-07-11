using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities.Collections;
using System;

namespace Org.BouncyCastle.X509
{
	public abstract class X509ExtensionBase : IX509Extension
	{
		protected abstract X509Extensions GetX509Extensions();

		protected virtual ISet GetExtensionOids(bool critical)
		{
			X509Extensions x509Extensions = this.GetX509Extensions();
			if (x509Extensions != null)
			{
				HashSet hashSet = new HashSet();
				foreach (DerObjectIdentifier derObjectIdentifier in x509Extensions.ExtensionOids)
				{
					X509Extension extension = x509Extensions.GetExtension(derObjectIdentifier);
					if (extension.IsCritical == critical)
					{
						hashSet.Add(derObjectIdentifier.Id);
					}
				}
				return hashSet;
			}
			return null;
		}

		public virtual ISet GetNonCriticalExtensionOids()
		{
			return this.GetExtensionOids(false);
		}

		public virtual ISet GetCriticalExtensionOids()
		{
			return this.GetExtensionOids(true);
		}

		[Obsolete("Use version taking a DerObjectIdentifier instead")]
		public Asn1OctetString GetExtensionValue(string oid)
		{
			return this.GetExtensionValue(new DerObjectIdentifier(oid));
		}

		public virtual Asn1OctetString GetExtensionValue(DerObjectIdentifier oid)
		{
			X509Extensions x509Extensions = this.GetX509Extensions();
			if (x509Extensions != null)
			{
				X509Extension extension = x509Extensions.GetExtension(oid);
				if (extension != null)
				{
					return extension.Value;
				}
			}
			return null;
		}
	}
}
