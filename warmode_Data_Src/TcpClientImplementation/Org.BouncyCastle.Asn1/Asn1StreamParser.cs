using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	public class Asn1StreamParser
	{
		private readonly Stream _in;

		private readonly int _limit;

		private readonly byte[][] tmpBuffers;

		public Asn1StreamParser(Stream inStream) : this(inStream, Asn1InputStream.FindLimit(inStream))
		{
		}

		public Asn1StreamParser(Stream inStream, int limit)
		{
			if (!inStream.CanRead)
			{
				throw new ArgumentException("Expected stream to be readable", "inStream");
			}
			this._in = inStream;
			this._limit = limit;
			this.tmpBuffers = new byte[16][];
		}

		public Asn1StreamParser(byte[] encoding) : this(new MemoryStream(encoding, false), encoding.Length)
		{
		}

		internal IAsn1Convertible ReadIndef(int tagValue)
		{
			int num = tagValue;
			if (num == 4)
			{
				return new BerOctetStringParser(this);
			}
			if (num == 8)
			{
				return new DerExternalParser(this);
			}
			switch (num)
			{
			case 16:
				return new BerSequenceParser(this);
			case 17:
				return new BerSetParser(this);
			default:
				throw new Asn1Exception("unknown BER object encountered: 0x" + tagValue.ToString("X"));
			}
		}

		internal IAsn1Convertible ReadImplicit(bool constructed, int tag)
		{
			if (!(this._in is IndefiniteLengthInputStream))
			{
				if (constructed)
				{
					if (tag == 4)
					{
						return new BerOctetStringParser(this);
					}
					switch (tag)
					{
					case 16:
						return new DerSequenceParser(this);
					case 17:
						return new DerSetParser(this);
					}
				}
				else
				{
					if (tag == 4)
					{
						return new DerOctetStringParser((DefiniteLengthInputStream)this._in);
					}
					switch (tag)
					{
					case 16:
						throw new Asn1Exception("sets must use constructed encoding (see X.690 8.11.1/8.12.1)");
					case 17:
						throw new Asn1Exception("sequences must use constructed encoding (see X.690 8.9.1/8.10.1)");
					}
				}
				throw new Asn1Exception("implicit tagging not implemented");
			}
			if (!constructed)
			{
				throw new IOException("indefinite length primitive encoding encountered");
			}
			return this.ReadIndef(tag);
		}

		internal Asn1Object ReadTaggedObject(bool constructed, int tag)
		{
			if (!constructed)
			{
				DefiniteLengthInputStream definiteLengthInputStream = (DefiniteLengthInputStream)this._in;
				return new DerTaggedObject(false, tag, new DerOctetString(definiteLengthInputStream.ToArray()));
			}
			Asn1EncodableVector asn1EncodableVector = this.ReadVector();
			if (this._in is IndefiniteLengthInputStream)
			{
				if (asn1EncodableVector.Count != 1)
				{
					return new BerTaggedObject(false, tag, BerSequence.FromVector(asn1EncodableVector));
				}
				return new BerTaggedObject(true, tag, asn1EncodableVector[0]);
			}
			else
			{
				if (asn1EncodableVector.Count != 1)
				{
					return new DerTaggedObject(false, tag, DerSequence.FromVector(asn1EncodableVector));
				}
				return new DerTaggedObject(true, tag, asn1EncodableVector[0]);
			}
		}

		public virtual IAsn1Convertible ReadObject()
		{
			int num = this._in.ReadByte();
			if (num == -1)
			{
				return null;
			}
			this.Set00Check(false);
			int num2 = Asn1InputStream.ReadTagNumber(this._in, num);
			bool flag = (num & 32) != 0;
			int num3 = Asn1InputStream.ReadLength(this._in, this._limit);
			if (num3 < 0)
			{
				if (!flag)
				{
					throw new IOException("indefinite length primitive encoding encountered");
				}
				IndefiniteLengthInputStream inStream = new IndefiniteLengthInputStream(this._in, this._limit);
				Asn1StreamParser asn1StreamParser = new Asn1StreamParser(inStream, this._limit);
				if ((num & 64) != 0)
				{
					return new BerApplicationSpecificParser(num2, asn1StreamParser);
				}
				if ((num & 128) != 0)
				{
					return new BerTaggedObjectParser(true, num2, asn1StreamParser);
				}
				return asn1StreamParser.ReadIndef(num2);
			}
			else
			{
				DefiniteLengthInputStream definiteLengthInputStream = new DefiniteLengthInputStream(this._in, num3);
				if ((num & 64) != 0)
				{
					return new DerApplicationSpecific(flag, num2, definiteLengthInputStream.ToArray());
				}
				if ((num & 128) != 0)
				{
					return new BerTaggedObjectParser(flag, num2, new Asn1StreamParser(definiteLengthInputStream));
				}
				if (flag)
				{
					int num4 = num2;
					if (num4 == 4)
					{
						return new BerOctetStringParser(new Asn1StreamParser(definiteLengthInputStream));
					}
					if (num4 == 8)
					{
						return new DerExternalParser(new Asn1StreamParser(definiteLengthInputStream));
					}
					switch (num4)
					{
					case 16:
						return new DerSequenceParser(new Asn1StreamParser(definiteLengthInputStream));
					case 17:
						return new DerSetParser(new Asn1StreamParser(definiteLengthInputStream));
					default:
						throw new IOException("unknown tag " + num2 + " encountered");
					}
				}
				else
				{
					int num5 = num2;
					if (num5 == 4)
					{
						return new DerOctetStringParser(definiteLengthInputStream);
					}
					IAsn1Convertible result;
					try
					{
						result = Asn1InputStream.CreatePrimitiveDerObject(num2, definiteLengthInputStream, this.tmpBuffers);
					}
					catch (ArgumentException exception)
					{
						throw new Asn1Exception("corrupted stream detected", exception);
					}
					return result;
				}
			}
		}

		private void Set00Check(bool enabled)
		{
			if (this._in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)this._in).SetEofOn00(enabled);
			}
		}

		internal Asn1EncodableVector ReadVector()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
			IAsn1Convertible asn1Convertible;
			while ((asn1Convertible = this.ReadObject()) != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					asn1Convertible.ToAsn1Object()
				});
			}
			return asn1EncodableVector;
		}
	}
}
