using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Asn1
{
	public class DerObjectIdentifier : Asn1Object
	{
		private const long LONG_LIMIT = 72057594037927808L;

		private readonly string identifier;

		private byte[] body;

		private static readonly DerObjectIdentifier[] cache = new DerObjectIdentifier[1024];

		public string Id
		{
			get
			{
				return this.identifier;
			}
		}

		public static DerObjectIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is DerObjectIdentifier)
			{
				return (DerObjectIdentifier)obj;
			}
			if (obj is byte[])
			{
				return DerObjectIdentifier.FromOctetString((byte[])obj);
			}
			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name, "obj");
		}

		public static DerObjectIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return DerObjectIdentifier.GetInstance(obj.GetObject());
		}

		public DerObjectIdentifier(string identifier)
		{
			if (identifier == null)
			{
				throw new ArgumentNullException("identifier");
			}
			if (!DerObjectIdentifier.IsValidIdentifier(identifier))
			{
				throw new FormatException("string " + identifier + " not an OID");
			}
			this.identifier = identifier;
		}

		internal DerObjectIdentifier(DerObjectIdentifier oid, string branchID)
		{
			if (!DerObjectIdentifier.IsValidBranchID(branchID, 0))
			{
				throw new ArgumentException("string " + branchID + " not a valid OID branch", "branchID");
			}
			this.identifier = oid.Id + "." + branchID;
		}

		public virtual DerObjectIdentifier Branch(string branchID)
		{
			return new DerObjectIdentifier(this, branchID);
		}

		public virtual bool On(DerObjectIdentifier stem)
		{
			string id = this.Id;
			string id2 = stem.Id;
			return id.Length > id2.Length && id[id2.Length] == '.' && id.StartsWith(id2);
		}

		internal DerObjectIdentifier(byte[] bytes)
		{
			this.identifier = DerObjectIdentifier.MakeOidStringFromBytes(bytes);
			this.body = Arrays.Clone(bytes);
		}

		private void WriteField(Stream outputStream, long fieldValue)
		{
			byte[] array = new byte[9];
			int num = 8;
			array[num] = (byte)(fieldValue & 127L);
			while (fieldValue >= 128L)
			{
				fieldValue >>= 7;
				array[--num] = (byte)((fieldValue & 127L) | 128L);
			}
			outputStream.Write(array, num, 9 - num);
		}

		private void WriteField(Stream outputStream, BigInteger fieldValue)
		{
			int num = (fieldValue.BitLength + 6) / 7;
			if (num == 0)
			{
				outputStream.WriteByte(0);
				return;
			}
			BigInteger bigInteger = fieldValue;
			byte[] array = new byte[num];
			for (int i = num - 1; i >= 0; i--)
			{
				array[i] = (byte)((bigInteger.IntValue & 127) | 128);
				bigInteger = bigInteger.ShiftRight(7);
			}
			byte[] expr_51_cp_0 = array;
			int expr_51_cp_1 = num - 1;
			expr_51_cp_0[expr_51_cp_1] &= 127;
			outputStream.Write(array, 0, array.Length);
		}

		private void DoOutput(MemoryStream bOut)
		{
			OidTokenizer oidTokenizer = new OidTokenizer(this.identifier);
			string text = oidTokenizer.NextToken();
			int num = int.Parse(text) * 40;
			text = oidTokenizer.NextToken();
			if (text.Length <= 18)
			{
				this.WriteField(bOut, (long)num + long.Parse(text));
			}
			else
			{
				this.WriteField(bOut, new BigInteger(text).Add(BigInteger.ValueOf((long)num)));
			}
			while (oidTokenizer.HasMoreTokens)
			{
				text = oidTokenizer.NextToken();
				if (text.Length <= 18)
				{
					this.WriteField(bOut, long.Parse(text));
				}
				else
				{
					this.WriteField(bOut, new BigInteger(text));
				}
			}
		}

		internal byte[] GetBody()
		{
			lock (this)
			{
				if (this.body == null)
				{
					MemoryStream memoryStream = new MemoryStream();
					this.DoOutput(memoryStream);
					this.body = memoryStream.ToArray();
				}
			}
			return this.body;
		}

		internal override void Encode(DerOutputStream derOut)
		{
			derOut.WriteEncoded(6, this.GetBody());
		}

		protected override int Asn1GetHashCode()
		{
			return this.identifier.GetHashCode();
		}

		protected override bool Asn1Equals(Asn1Object asn1Object)
		{
			DerObjectIdentifier derObjectIdentifier = asn1Object as DerObjectIdentifier;
			return derObjectIdentifier != null && this.identifier.Equals(derObjectIdentifier.identifier);
		}

		public override string ToString()
		{
			return this.identifier;
		}

		private static bool IsValidBranchID(string branchID, int start)
		{
			bool flag = false;
			int num = branchID.Length;
			while (--num >= start)
			{
				char c = branchID[num];
				if ('0' <= c && c <= '9')
				{
					flag = true;
				}
				else
				{
					if (c != '.')
					{
						return false;
					}
					if (!flag)
					{
						return false;
					}
					flag = false;
				}
			}
			return flag;
		}

		private static bool IsValidIdentifier(string identifier)
		{
			if (identifier.Length < 3 || identifier[1] != '.')
			{
				return false;
			}
			char c = identifier[0];
			return c >= '0' && c <= '2' && DerObjectIdentifier.IsValidBranchID(identifier, 2);
		}

		private static string MakeOidStringFromBytes(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			long num = 0L;
			BigInteger bigInteger = null;
			bool flag = true;
			for (int num2 = 0; num2 != bytes.Length; num2++)
			{
				int num3 = (int)bytes[num2];
				if (num <= 72057594037927808L)
				{
					num += (long)(num3 & 127);
					if ((num3 & 128) == 0)
					{
						if (flag)
						{
							if (num < 40L)
							{
								stringBuilder.Append('0');
							}
							else if (num < 80L)
							{
								stringBuilder.Append('1');
								num -= 40L;
							}
							else
							{
								stringBuilder.Append('2');
								num -= 80L;
							}
							flag = false;
						}
						stringBuilder.Append('.');
						stringBuilder.Append(num);
						num = 0L;
					}
					else
					{
						num <<= 7;
					}
				}
				else
				{
					if (bigInteger == null)
					{
						bigInteger = BigInteger.ValueOf(num);
					}
					bigInteger = bigInteger.Or(BigInteger.ValueOf((long)(num3 & 127)));
					if ((num3 & 128) == 0)
					{
						if (flag)
						{
							stringBuilder.Append('2');
							bigInteger = bigInteger.Subtract(BigInteger.ValueOf(80L));
							flag = false;
						}
						stringBuilder.Append('.');
						stringBuilder.Append(bigInteger);
						bigInteger = null;
						num = 0L;
					}
					else
					{
						bigInteger = bigInteger.ShiftLeft(7);
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal static DerObjectIdentifier FromOctetString(byte[] enc)
		{
			int hashCode = Arrays.GetHashCode(enc);
			int num = hashCode & 1023;
			DerObjectIdentifier result;
			lock (DerObjectIdentifier.cache)
			{
				DerObjectIdentifier derObjectIdentifier = DerObjectIdentifier.cache[num];
				if (derObjectIdentifier != null && Arrays.AreEqual(enc, derObjectIdentifier.GetBody()))
				{
					result = derObjectIdentifier;
				}
				else
				{
					result = (DerObjectIdentifier.cache[num] = new DerObjectIdentifier(enc));
				}
			}
			return result;
		}
	}
}
