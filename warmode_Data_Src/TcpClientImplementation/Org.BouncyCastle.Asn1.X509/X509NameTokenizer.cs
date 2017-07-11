using System;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class X509NameTokenizer
	{
		private string value;

		private int index;

		private char separator;

		private StringBuilder buffer = new StringBuilder();

		public X509NameTokenizer(string oid) : this(oid, ',')
		{
		}

		public X509NameTokenizer(string oid, char separator)
		{
			this.value = oid;
			this.index = -1;
			this.separator = separator;
		}

		public bool HasMoreTokens()
		{
			return this.index != this.value.Length;
		}

		public string NextToken()
		{
			if (this.index == this.value.Length)
			{
				return null;
			}
			int num = this.index + 1;
			bool flag = false;
			bool flag2 = false;
			this.buffer.Remove(0, this.buffer.Length);
			while (num != this.value.Length)
			{
				char c = this.value[num];
				if (c == '"')
				{
					if (!flag2)
					{
						flag = !flag;
					}
					else
					{
						this.buffer.Append(c);
						flag2 = false;
					}
				}
				else if (flag2 || flag)
				{
					if (c == '#' && this.buffer[this.buffer.Length - 1] == '=')
					{
						this.buffer.Append('\\');
					}
					else if (c == '+' && this.separator != '+')
					{
						this.buffer.Append('\\');
					}
					this.buffer.Append(c);
					flag2 = false;
				}
				else if (c == '\\')
				{
					flag2 = true;
				}
				else
				{
					if (c == this.separator)
					{
						break;
					}
					this.buffer.Append(c);
				}
				num++;
			}
			this.index = num;
			return this.buffer.ToString().Trim();
		}
	}
}
