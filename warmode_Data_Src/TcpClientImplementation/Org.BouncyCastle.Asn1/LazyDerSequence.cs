using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	internal class LazyDerSequence : DerSequence
	{
		private byte[] encoded;

		public override Asn1Encodable this[int index]
		{
			get
			{
				this.Parse();
				return base[index];
			}
		}

		public override int Count
		{
			get
			{
				this.Parse();
				return base.Count;
			}
		}

		internal LazyDerSequence(byte[] encoded)
		{
			this.encoded = encoded;
		}

		private void Parse()
		{
			lock (this)
			{
				if (this.encoded != null)
				{
					Asn1InputStream asn1InputStream = new LazyAsn1InputStream(this.encoded);
					Asn1Object obj;
					while ((obj = asn1InputStream.ReadObject()) != null)
					{
						base.AddObject(obj);
					}
					this.encoded = null;
				}
			}
		}

		public override IEnumerator GetEnumerator()
		{
			this.Parse();
			return base.GetEnumerator();
		}

		internal override void Encode(DerOutputStream derOut)
		{
			lock (this)
			{
				if (this.encoded == null)
				{
					base.Encode(derOut);
				}
				else
				{
					derOut.WriteEncoded(48, this.encoded);
				}
			}
		}
	}
}
