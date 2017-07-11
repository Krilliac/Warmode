using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class DerSequence : Asn1Sequence
	{
		public static readonly DerSequence Empty = new DerSequence();

		public static DerSequence FromVector(Asn1EncodableVector v)
		{
			if (v.Count >= 1)
			{
				return new DerSequence(v);
			}
			return DerSequence.Empty;
		}

		public DerSequence() : base(0)
		{
		}

		public DerSequence(Asn1Encodable obj) : base(1)
		{
			base.AddObject(obj);
		}

		public DerSequence(params Asn1Encodable[] v) : base(v.Length)
		{
			for (int i = 0; i < v.Length; i++)
			{
				Asn1Encodable obj = v[i];
				base.AddObject(obj);
			}
		}

		public DerSequence(Asn1EncodableVector v) : base(v.Count)
		{
			foreach (Asn1Encodable obj in v)
			{
				base.AddObject(obj);
			}
		}

		internal override void Encode(DerOutputStream derOut)
		{
			MemoryStream memoryStream = new MemoryStream();
			DerOutputStream derOutputStream = new DerOutputStream(memoryStream);
			foreach (Asn1Encodable obj in this)
			{
				derOutputStream.WriteObject(obj);
			}
			derOutputStream.Close();
			byte[] bytes = memoryStream.ToArray();
			derOut.WriteEncoded(48, bytes);
		}
	}
}
