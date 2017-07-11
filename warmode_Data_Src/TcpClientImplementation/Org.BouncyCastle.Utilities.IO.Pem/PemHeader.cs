using System;

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemHeader
	{
		private string name;

		private string val;

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual string Value
		{
			get
			{
				return this.val;
			}
		}

		public PemHeader(string name, string val)
		{
			this.name = name;
			this.val = val;
		}

		public override int GetHashCode()
		{
			return this.GetHashCode(this.name) + 31 * this.GetHashCode(this.val);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (!(obj is PemHeader))
			{
				return false;
			}
			PemHeader pemHeader = (PemHeader)obj;
			return object.Equals(this.name, pemHeader.name) && object.Equals(this.val, pemHeader.val);
		}

		private int GetHashCode(string s)
		{
			if (s == null)
			{
				return 1;
			}
			return s.GetHashCode();
		}
	}
}
