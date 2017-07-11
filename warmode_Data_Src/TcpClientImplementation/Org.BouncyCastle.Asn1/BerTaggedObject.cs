using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Asn1
{
	public class BerTaggedObject : DerTaggedObject
	{
		public BerTaggedObject(int tagNo, Asn1Encodable obj) : base(tagNo, obj)
		{
		}

		public BerTaggedObject(bool explicitly, int tagNo, Asn1Encodable obj) : base(explicitly, tagNo, obj)
		{
		}

		public BerTaggedObject(int tagNo) : base(false, tagNo, BerSequence.Empty)
		{
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteTag(160, this.tagNo);
				derOut.WriteByte(128);
				if (!base.IsEmpty())
				{
					if (!this.explicitly)
					{
						IEnumerable enumerable;
						if (this.obj is Asn1OctetString)
						{
							if (this.obj is BerOctetString)
							{
								enumerable = (BerOctetString)this.obj;
							}
							else
							{
								Asn1OctetString asn1OctetString = (Asn1OctetString)this.obj;
								enumerable = new BerOctetString(asn1OctetString.GetOctets());
							}
						}
						else if (this.obj is Asn1Sequence)
						{
							enumerable = (Asn1Sequence)this.obj;
						}
						else
						{
							if (!(this.obj is Asn1Set))
							{
								throw Platform.CreateNotImplementedException(this.obj.GetType().Name);
							}
							enumerable = (Asn1Set)this.obj;
						}
						using (IEnumerator enumerator = enumerable.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Asn1Encodable obj = (Asn1Encodable)enumerator.Current;
								derOut.WriteObject(obj);
							}
							goto IL_119;
						}
					}
					derOut.WriteObject(this.obj);
				}
				IL_119:
				derOut.WriteByte(0);
				derOut.WriteByte(0);
				return;
			}
			base.Encode(derOut);
		}
	}
}
