using Org.BouncyCastle.Asn1.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class Asn1InputStream : FilterStream
	{
		private readonly int limit;

		private readonly byte[][] tmpBuffers;

		internal static int FindLimit(Stream input)
		{
			if (input is LimitedInputStream)
			{
				return ((LimitedInputStream)input).GetRemaining();
			}
			if (input is MemoryStream)
			{
				MemoryStream memoryStream = (MemoryStream)input;
				return (int)(memoryStream.Length - memoryStream.Position);
			}
			return 2147483647;
		}

		public Asn1InputStream(Stream inputStream) : this(inputStream, Asn1InputStream.FindLimit(inputStream))
		{
		}

		public Asn1InputStream(Stream inputStream, int limit) : base(inputStream)
		{
			this.limit = limit;
			this.tmpBuffers = new byte[16][];
		}

		public Asn1InputStream(byte[] input) : this(new MemoryStream(input, false), input.Length)
		{
		}

		private Asn1Object BuildObject(int tag, int tagNo, int length)
		{
			bool flag = (tag & 32) != 0;
			DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(this.s, length);
			if ((tag & 64) != 0)
			{
				return new DerApplicationSpecific(flag, tagNo, definiteLengthInputStream.ToArray());
			}
			if ((tag & 128) != 0)
			{
				return new Asn1StreamParser(definiteLengthInputStream).ReadTaggedObject(flag, tagNo);
			}
			if (!flag)
			{
				return Asn1InputStream.CreatePrimitiveDerObject(tagNo, definiteLengthInputStream, this.tmpBuffers);
			}
			if (tagNo == 4)
			{
				return new BerOctetString(this.BuildDerEncodableVector(definiteLengthInputStream));
			}
			if (tagNo == 8)
			{
				return new DerExternal(this.BuildDerEncodableVector(definiteLengthInputStream));
			}
			switch (tagNo)
			{
			case 16:
				return this.CreateDerSequence(definiteLengthInputStream);
			case 17:
				return this.CreateDerSet(definiteLengthInputStream);
			default:
				throw new IOException("unknown tag " + tagNo + " encountered");
			}
		}

		internal Asn1EncodableVector BuildEncodableVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			Asn1Object asn1Object;
			while ((asn1Object = this.ReadObject()) != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					asn1Object
				});
			}
			return asn1EncodableVector;
		}

		internal virtual Asn1EncodableVector BuildDerEncodableVector(DefiniteLengthInputStream dIn)
		{
			return new Asn1InputStream(dIn).BuildEncodableVector();
		}

		internal virtual DerSequence CreateDerSequence(DefiniteLengthInputStream dIn)
		{
			return DerSequence.FromVector(this.BuildDerEncodableVector(dIn));
		}

		internal virtual DerSet CreateDerSet(DefiniteLengthInputStream dIn)
		{
			return DerSet.FromVector(this.BuildDerEncodableVector(dIn), false);
		}

		public Asn1Object ReadObject()
		{
			int num = this.ReadByte();
			if (num <= 0)
			{
				if (num == 0)
				{
					throw new IOException("unexpected end-of-contents marker");
				}
				return null;
			}
			else
			{
				int num2 = Asn1InputStream.ReadTagNumber(this.s, num);
				bool flag = (num & 32) != 0;
				int num3 = Asn1InputStream.ReadLength(this.s, this.limit);
				if (num3 >= 0)
				{
					Asn1Object result;
					try
					{
						result = this.BuildObject(num, num2, num3);
					}
					catch (ArgumentException exception)
					{
						throw new Asn1Exception("corrupted stream detected", exception);
					}
					return result;
				}
				if (!flag)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				IndefiniteLengthInputStream inStream = new IndefiniteLengthInputStream(this.s, this.limit);
				Asn1StreamParser parser = new Asn1StreamParser(inStream, this.limit);
				if ((num & 64) != 0)
				{
					return new BerApplicationSpecificParser(num2, parser).ToAsn1Object();
				}
				if ((num & 128) != 0)
				{
					return new BerTaggedObjectParser(true, num2, parser).ToAsn1Object();
				}
				int num4 = num2;
				if (num4 == 4)
				{
					return new BerOctetStringParser(parser).ToAsn1Object();
				}
				if (num4 == 8)
				{
					return new DerExternalParser(parser).ToAsn1Object();
				}
				switch (num4)
				{
				case 16:
					return new BerSequenceParser(parser).ToAsn1Object();
				case 17:
					return new BerSetParser(parser).ToAsn1Object();
				default:
					throw new IOException("unknown BER object encountered");
				}
			}
		}

		internal static int ReadTagNumber(Stream s, int tag)
		{
			int num = tag & 31;
			if (num == 31)
			{
				num = 0;
				int num2 = s.ReadByte();
				if ((num2 & 127) == 0)
				{
					throw new IOException("Corrupted stream - invalid high tag number found");
				}
				while (num2 >= 0 && (num2 & 128) != 0)
				{
					num |= (num2 & 127);
					num <<= 7;
					num2 = s.ReadByte();
				}
				if (num2 < 0)
				{
					throw new EndOfStreamException("EOF found inside tag value.");
				}
				num |= (num2 & 127);
			}
			return num;
		}

		internal static int ReadLength(Stream s, int limit)
		{
			int num = s.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException("EOF found when length expected");
			}
			if (num == 128)
			{
				return -1;
			}
			if (num > 127)
			{
				int num2 = num & 127;
				if (num2 > 4)
				{
					throw new IOException("DER length more than 4 bytes: " + num2);
				}
				num = 0;
				for (int i = 0; i < num2; i++)
				{
					int num3 = s.ReadByte();
					if (num3 < 0)
					{
						throw new EndOfStreamException("EOF found reading length");
					}
					num = (num << 8) + num3;
				}
				if (num < 0)
				{
					throw new IOException("Corrupted stream - negative length found");
				}
				if (num >= limit)
				{
					throw new IOException("Corrupted stream - out of bounds length found");
				}
			}
			return num;
		}

		internal static byte[] GetBuffer(DefiniteLengthInputStream defIn, byte[][] tmpBuffers)
		{
			int remaining = defIn.GetRemaining();
			if (remaining >= tmpBuffers.Length)
			{
				return defIn.ToArray();
			}
			byte[] array = tmpBuffers[remaining];
			if (array == null)
			{
				array = (tmpBuffers[remaining] = new byte[remaining]);
			}
			defIn.ReadAllIntoByteArray(array);
			return array;
		}

		internal static Asn1Object CreatePrimitiveDerObject(int tagNo, DefiniteLengthInputStream defIn, byte[][] tmpBuffers)
		{
			if (tagNo == 1)
			{
				return DerBoolean.FromOctetString(Asn1InputStream.GetBuffer(defIn, tmpBuffers));
			}
			if (tagNo == 6)
			{
				return DerObjectIdentifier.FromOctetString(Asn1InputStream.GetBuffer(defIn, tmpBuffers));
			}
			if (tagNo != 10)
			{
				byte[] array = defIn.ToArray();
				switch (tagNo)
				{
				case 2:
					return new DerInteger(array);
				case 3:
					return DerBitString.FromAsn1Octets(array);
				case 4:
					return new DerOctetString(array);
				case 5:
					return DerNull.Instance;
				case 12:
					return new DerUtf8String(array);
				case 18:
					return new DerNumericString(array);
				case 19:
					return new DerPrintableString(array);
				case 20:
					return new DerT61String(array);
				case 22:
					return new DerIA5String(array);
				case 23:
					return new DerUtcTime(array);
				case 24:
					return new DerGeneralizedTime(array);
				case 26:
					return new DerVisibleString(array);
				case 27:
					return new DerGeneralString(array);
				case 28:
					return new DerUniversalString(array);
				case 30:
					return new DerBmpString(array);
				}
				throw new IOException("unknown tag " + tagNo + " encountered");
			}
			return DerEnumerated.FromOctetString(Asn1InputStream.GetBuffer(defIn, tmpBuffers));
		}
	}
}
