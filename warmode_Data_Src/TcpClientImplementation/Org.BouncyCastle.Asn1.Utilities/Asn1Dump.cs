using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Asn1.Utilities
{
	public sealed class Asn1Dump
	{
		private const string Tab = "    ";

		private const int SampleSize = 32;

		private static readonly string NewLine = Platform.NewLine;

		private Asn1Dump()
		{
		}

		private static void AsString(string indent, bool verbose, Asn1Object obj, StringBuilder buf)
		{
			if (obj is Asn1Sequence)
			{
				string text = indent + "    ";
				buf.Append(indent);
				if (obj is BerSequence)
				{
					buf.Append("BER Sequence");
				}
				else if (obj is DerSequence)
				{
					buf.Append("DER Sequence");
				}
				else
				{
					buf.Append("Sequence");
				}
				buf.Append(Asn1Dump.NewLine);
				using (IEnumerator enumerator = ((Asn1Sequence)obj).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Asn1Encodable asn1Encodable = (Asn1Encodable)enumerator.Current;
						if (asn1Encodable == null || asn1Encodable is Asn1Null)
						{
							buf.Append(text);
							buf.Append("NULL");
							buf.Append(Asn1Dump.NewLine);
						}
						else
						{
							Asn1Dump.AsString(text, verbose, asn1Encodable.ToAsn1Object(), buf);
						}
					}
					return;
				}
			}
			if (obj is DerTaggedObject)
			{
				string text2 = indent + "    ";
				buf.Append(indent);
				if (obj is BerTaggedObject)
				{
					buf.Append("BER Tagged [");
				}
				else
				{
					buf.Append("Tagged [");
				}
				DerTaggedObject derTaggedObject = (DerTaggedObject)obj;
				buf.Append(derTaggedObject.TagNo.ToString());
				buf.Append(']');
				if (!derTaggedObject.IsExplicit())
				{
					buf.Append(" IMPLICIT ");
				}
				buf.Append(Asn1Dump.NewLine);
				if (derTaggedObject.IsEmpty())
				{
					buf.Append(text2);
					buf.Append("EMPTY");
					buf.Append(Asn1Dump.NewLine);
					return;
				}
				Asn1Dump.AsString(text2, verbose, derTaggedObject.GetObject(), buf);
				return;
			}
			else
			{
				if (obj is BerSet)
				{
					string text3 = indent + "    ";
					buf.Append(indent);
					buf.Append("BER Set");
					buf.Append(Asn1Dump.NewLine);
					using (IEnumerator enumerator2 = ((Asn1Set)obj).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Asn1Encodable asn1Encodable2 = (Asn1Encodable)enumerator2.Current;
							if (asn1Encodable2 == null)
							{
								buf.Append(text3);
								buf.Append("NULL");
								buf.Append(Asn1Dump.NewLine);
							}
							else
							{
								Asn1Dump.AsString(text3, verbose, asn1Encodable2.ToAsn1Object(), buf);
							}
						}
						return;
					}
				}
				if (obj is DerSet)
				{
					string text4 = indent + "    ";
					buf.Append(indent);
					buf.Append("DER Set");
					buf.Append(Asn1Dump.NewLine);
					using (IEnumerator enumerator3 = ((Asn1Set)obj).GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Asn1Encodable asn1Encodable3 = (Asn1Encodable)enumerator3.Current;
							if (asn1Encodable3 == null)
							{
								buf.Append(text4);
								buf.Append("NULL");
								buf.Append(Asn1Dump.NewLine);
							}
							else
							{
								Asn1Dump.AsString(text4, verbose, asn1Encodable3.ToAsn1Object(), buf);
							}
						}
						return;
					}
				}
				if (obj is DerObjectIdentifier)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"ObjectIdentifier(",
						((DerObjectIdentifier)obj).Id,
						")",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerBoolean)
				{
					buf.Append(string.Concat(new object[]
					{
						indent,
						"Boolean(",
						((DerBoolean)obj).IsTrue,
						")",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerInteger)
				{
					buf.Append(string.Concat(new object[]
					{
						indent,
						"Integer(",
						((DerInteger)obj).Value,
						")",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is BerOctetString)
				{
					byte[] octets = ((Asn1OctetString)obj).GetOctets();
					string text5 = verbose ? Asn1Dump.dumpBinaryDataAsString(indent, octets) : "";
					buf.Append(string.Concat(new object[]
					{
						indent,
						"BER Octet String[",
						octets.Length,
						"] ",
						text5,
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerOctetString)
				{
					byte[] octets2 = ((Asn1OctetString)obj).GetOctets();
					string text6 = verbose ? Asn1Dump.dumpBinaryDataAsString(indent, octets2) : "";
					buf.Append(string.Concat(new object[]
					{
						indent,
						"DER Octet String[",
						octets2.Length,
						"] ",
						text6,
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerBitString)
				{
					DerBitString derBitString = (DerBitString)obj;
					byte[] bytes = derBitString.GetBytes();
					string text7 = verbose ? Asn1Dump.dumpBinaryDataAsString(indent, bytes) : "";
					buf.Append(string.Concat(new object[]
					{
						indent,
						"DER Bit String[",
						bytes.Length,
						", ",
						derBitString.PadBits,
						"] ",
						text7,
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerIA5String)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"IA5String(",
						((DerIA5String)obj).GetString(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerUtf8String)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"UTF8String(",
						((DerUtf8String)obj).GetString(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerPrintableString)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"PrintableString(",
						((DerPrintableString)obj).GetString(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerVisibleString)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"VisibleString(",
						((DerVisibleString)obj).GetString(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerBmpString)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"BMPString(",
						((DerBmpString)obj).GetString(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerT61String)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"T61String(",
						((DerT61String)obj).GetString(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerUtcTime)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"UTCTime(",
						((DerUtcTime)obj).TimeString,
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerGeneralizedTime)
				{
					buf.Append(string.Concat(new string[]
					{
						indent,
						"GeneralizedTime(",
						((DerGeneralizedTime)obj).GetTime(),
						") ",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is BerApplicationSpecific)
				{
					buf.Append(Asn1Dump.outputApplicationSpecific("BER", indent, verbose, (BerApplicationSpecific)obj));
					return;
				}
				if (obj is DerApplicationSpecific)
				{
					buf.Append(Asn1Dump.outputApplicationSpecific("DER", indent, verbose, (DerApplicationSpecific)obj));
					return;
				}
				if (obj is DerEnumerated)
				{
					DerEnumerated derEnumerated = (DerEnumerated)obj;
					buf.Append(string.Concat(new object[]
					{
						indent,
						"DER Enumerated(",
						derEnumerated.Value,
						")",
						Asn1Dump.NewLine
					}));
					return;
				}
				if (obj is DerExternal)
				{
					DerExternal derExternal = (DerExternal)obj;
					buf.Append(indent + "External " + Asn1Dump.NewLine);
					string text8 = indent + "    ";
					if (derExternal.DirectReference != null)
					{
						buf.Append(text8 + "Direct Reference: " + derExternal.DirectReference.Id + Asn1Dump.NewLine);
					}
					if (derExternal.IndirectReference != null)
					{
						buf.Append(text8 + "Indirect Reference: " + derExternal.IndirectReference.ToString() + Asn1Dump.NewLine);
					}
					if (derExternal.DataValueDescriptor != null)
					{
						Asn1Dump.AsString(text8, verbose, derExternal.DataValueDescriptor, buf);
					}
					buf.Append(string.Concat(new object[]
					{
						text8,
						"Encoding: ",
						derExternal.Encoding,
						Asn1Dump.NewLine
					}));
					Asn1Dump.AsString(text8, verbose, derExternal.ExternalContent, buf);
					return;
				}
				buf.Append(indent + obj.ToString() + Asn1Dump.NewLine);
			}
		}

		private static string outputApplicationSpecific(string type, string indent, bool verbose, DerApplicationSpecific app)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (app.IsConstructed())
			{
				try
				{
					Asn1Sequence instance = Asn1Sequence.GetInstance(app.GetObject(16));
					stringBuilder.Append(string.Concat(new object[]
					{
						indent,
						type,
						" ApplicationSpecific[",
						app.ApplicationTag,
						"]",
						Asn1Dump.NewLine
					}));
					foreach (Asn1Encodable asn1Encodable in instance)
					{
						Asn1Dump.AsString(indent + "    ", verbose, asn1Encodable.ToAsn1Object(), stringBuilder);
					}
				}
				catch (IOException value)
				{
					stringBuilder.Append(value);
				}
				return stringBuilder.ToString();
			}
			return string.Concat(new object[]
			{
				indent,
				type,
				" ApplicationSpecific[",
				app.ApplicationTag,
				"] (",
				Hex.ToHexString(app.GetContents()),
				")",
				Asn1Dump.NewLine
			});
		}

		[Obsolete("Use version accepting Asn1Encodable")]
		public static string DumpAsString(object obj)
		{
			if (obj is Asn1Encodable)
			{
				StringBuilder stringBuilder = new StringBuilder();
				Asn1Dump.AsString("", false, ((Asn1Encodable)obj).ToAsn1Object(), stringBuilder);
				return stringBuilder.ToString();
			}
			return "unknown object type " + obj.ToString();
		}

		public static string DumpAsString(Asn1Encodable obj)
		{
			return Asn1Dump.DumpAsString(obj, false);
		}

		public static string DumpAsString(Asn1Encodable obj, bool verbose)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Asn1Dump.AsString("", verbose, obj.ToAsn1Object(), stringBuilder);
			return stringBuilder.ToString();
		}

		private static string dumpBinaryDataAsString(string indent, byte[] bytes)
		{
			indent += "    ";
			StringBuilder stringBuilder = new StringBuilder(Asn1Dump.NewLine);
			for (int i = 0; i < bytes.Length; i += 32)
			{
				if (bytes.Length - i > 32)
				{
					stringBuilder.Append(indent);
					stringBuilder.Append(Hex.ToHexString(bytes, i, 32));
					stringBuilder.Append("    ");
					stringBuilder.Append(Asn1Dump.calculateAscString(bytes, i, 32));
					stringBuilder.Append(Asn1Dump.NewLine);
				}
				else
				{
					stringBuilder.Append(indent);
					stringBuilder.Append(Hex.ToHexString(bytes, i, bytes.Length - i));
					for (int num = bytes.Length - i; num != 32; num++)
					{
						stringBuilder.Append("  ");
					}
					stringBuilder.Append("    ");
					stringBuilder.Append(Asn1Dump.calculateAscString(bytes, i, bytes.Length - i));
					stringBuilder.Append(Asn1Dump.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		private static string calculateAscString(byte[] bytes, int off, int len)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = off; num != off + len; num++)
			{
				char c = (char)bytes[num];
				if (c >= ' ' && c <= '~')
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
