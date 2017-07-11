using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class BerOctetString : DerOctetString, IEnumerable
	{
		private const int MaxLength = 1000;

		private readonly IEnumerable octs;

		public static BerOctetString FromSequence(Asn1Sequence seq)
		{
			IList list = Platform.CreateArrayList();
			foreach (Asn1Encodable value in seq)
			{
				list.Add(value);
			}
			return new BerOctetString(list);
		}

		private static byte[] ToBytes(IEnumerable octs)
		{
			MemoryStream memoryStream = new MemoryStream();
			foreach (DerOctetString derOctetString in octs)
			{
				byte[] octets = derOctetString.GetOctets();
				memoryStream.Write(octets, 0, octets.Length);
			}
			return memoryStream.ToArray();
		}

		public BerOctetString(byte[] str) : base(str)
		{
		}

		public BerOctetString(IEnumerable octets) : base(BerOctetString.ToBytes(octets))
		{
			this.octs = octets;
		}

		public BerOctetString(Asn1Object obj) : base(obj)
		{
		}

		public BerOctetString(Asn1Encodable obj) : base(obj.ToAsn1Object())
		{
		}

		public override byte[] GetOctets()
		{
			return this.str;
		}

		public IEnumerator GetEnumerator()
		{
			if (this.octs == null)
			{
				return this.GenerateOcts().GetEnumerator();
			}
			return this.octs.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
		public IEnumerator GetObjects()
		{
			return this.GetEnumerator();
		}

		private IList GenerateOcts()
		{
			IList list = Platform.CreateArrayList();
			for (int i = 0; i < this.str.Length; i += 1000)
			{
				int num = Math.Min(this.str.Length, i + 1000);
				byte[] array = new byte[num - i];
				Array.Copy(this.str, i, array, 0, array.Length);
				list.Add(new DerOctetString(array));
			}
			return list;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			if (derOut is Asn1OutputStream || derOut is BerOutputStream)
			{
				derOut.WriteByte(36);
				derOut.WriteByte(128);
				foreach (DerOctetString obj in this)
				{
					derOut.WriteObject(obj);
				}
				derOut.WriteByte(0);
				derOut.WriteByte(0);
				return;
			}
			base.Encode(derOut);
		}
	}
}
