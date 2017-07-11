using System;

namespace Org.BouncyCastle.Asn1.X509
{
	public class X509Extension
	{
		internal bool critical;

		internal Asn1OctetString value;

		public bool IsCritical
		{
			get
			{
				return this.critical;
			}
		}

		public Asn1OctetString Value
		{
			get
			{
				return this.value;
			}
		}

		public X509Extension(DerBoolean critical, Asn1OctetString value)
		{
			if (critical == null)
			{
				throw new ArgumentNullException("critical");
			}
			this.critical = critical.IsTrue;
			this.value = value;
		}

		public X509Extension(bool critical, Asn1OctetString value)
		{
			this.critical = critical;
			this.value = value;
		}

		public Asn1Encodable GetParsedValue()
		{
			return X509Extension.ConvertValueToObject(this);
		}

		public override int GetHashCode()
		{
			int hashCode = this.Value.GetHashCode();
			if (!this.IsCritical)
			{
				return ~hashCode;
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			X509Extension x509Extension = obj as X509Extension;
			return x509Extension != null && this.Value.Equals(x509Extension.Value) && this.IsCritical == x509Extension.IsCritical;
		}

		public static Asn1Object ConvertValueToObject(X509Extension ext)
		{
			Asn1Object result;
			try
			{
				result = Asn1Object.FromByteArray(ext.Value.GetOctets());
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("can't convert extension", innerException);
			}
			return result;
		}
	}
}
