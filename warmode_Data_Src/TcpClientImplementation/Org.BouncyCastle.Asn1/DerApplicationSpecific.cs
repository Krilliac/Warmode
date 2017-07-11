using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class DerApplicationSpecific : Asn1Object
	{
		private readonly bool isConstructed;

		private readonly int tag;

		private readonly byte[] octets;

		public int ApplicationTag
		{
			get
			{
				return this.tag;
			}
		}

		internal DerApplicationSpecific(bool isConstructed, int tag, byte[] octets)
		{
			this.isConstructed = isConstructed;
			this.tag = tag;
			this.octets = octets;
		}

		public DerApplicationSpecific(int tag, byte[] octets) : this(false, tag, octets)
		{
		}

		public DerApplicationSpecific(int tag, Asn1Encodable obj) : this(true, tag, obj)
		{
		}

		public DerApplicationSpecific(bool isExplicit, int tag, Asn1Encodable obj)
		{
			Asn1Object asn1Object = obj.ToAsn1Object();
			byte[] derEncoded = asn1Object.GetDerEncoded();
			this.isConstructed = (isExplicit || asn1Object is Asn1Set || asn1Object is Asn1Sequence);
			this.tag = tag;
			if (isExplicit)
			{
				this.octets = derEncoded;
				return;
			}
			int lengthOfHeader = this.GetLengthOfHeader(derEncoded);
			byte[] array = new byte[derEncoded.Length - lengthOfHeader];
			Array.Copy(derEncoded, lengthOfHeader, array, 0, array.Length);
			this.octets = array;
		}

		public DerApplicationSpecific(int tagNo, Asn1EncodableVector vec)
		{
			this.tag = tagNo;
			this.isConstructed = true;
			MemoryStream memoryStream = new MemoryStream();
			for (int num = 0; num != vec.Count; num++)
			{
				try
				{
					byte[] derEncoded = vec[num].GetDerEncoded();
					memoryStream.Write(derEncoded, 0, derEncoded.Length);
				}
				catch (IOException innerException)
				{
					throw new InvalidOperationException("malformed object", innerException);
				}
			}
			this.octets = memoryStream.ToArray();
		}

		private int GetLengthOfHeader(byte[] data)
		{
			int num = (int)data[1];
			if (num == 128)
			{
				return 2;
			}
			if (num <= 127)
			{
				return 2;
			}
			int num2 = num & 127;
			if (num2 > 4)
			{
				throw new InvalidOperationException("DER length more than 4 bytes: " + num2);
			}
			return num2 + 2;
		}

		public bool IsConstructed()
		{
			return this.isConstructed;
		}

		public byte[] GetContents()
		{
			return this.octets;
		}

		public Asn1Object GetObject()
		{
			return Asn1Object.FromByteArray(this.GetContents());
		}

		public Asn1Object GetObject(int derTagNo)
		{
			if (derTagNo >= 31)
			{
				throw new IOException("unsupported tag number");
			}
			byte[] encoded = base.GetEncoded();
			byte[] array = this.ReplaceTagNumber(derTagNo, encoded);
			if ((encoded[0] & 32) != 0)
			{
				byte[] expr_2F_cp_0 = array;
				int expr_2F_cp_1 = 0;
				expr_2F_cp_0[expr_2F_cp_1] |= 32;
			}
			return Asn1Object.FromByteArray(array);
		}

		internal override void Encode(DerOutputStream derOut)
		{
			int num = 64;
			if (this.isConstructed)
			{
				num |= 32;
			}
			derOut.WriteEncoded(num, this.tag, this.octets);
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerApplicationSpecific derApplicationSpecific = asn1Object as DerApplicationSpecific;
			return derApplicationSpecific != null && (this.isConstructed == derApplicationSpecific.isConstructed && this.tag == derApplicationSpecific.tag) && Arrays.AreEqual(this.octets, derApplicationSpecific.octets);
		}

		protected override int Asn1GetHashCode()
		{
			return this.isConstructed.GetHashCode() ^ this.tag.GetHashCode() ^ Arrays.GetHashCode(this.octets);
		}

		private byte[] ReplaceTagNumber(int newTag, byte[] input)
		{
			int num = (int)(input[0] & 31);
			int num2 = 1;
			if (num == 31)
			{
				num = 0;
				int num3 = (int)(input[num2++] & 255);
				if ((num3 & 127) == 0)
				{
					throw new InvalidOperationException("corrupted stream - invalid high tag number found");
				}
				while (num3 >= 0 && (num3 & 128) != 0)
				{
					num |= (num3 & 127);
					num <<= 7;
					num3 = (int)(input[num2++] & 255);
				}
				num |= (num3 & 127);
			}
			byte[] array = new byte[input.Length - num2 + 1];
			Array.Copy(input, num2, array, 1, array.Length - 1);
			array[0] = (byte)newTag;
			return array;
		}
	}
}
