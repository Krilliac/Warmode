using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Asn1.X509
{
	public class X509Name : Asn1Encodable
	{
		public static readonly DerObjectIdentifier C;

		public static readonly DerObjectIdentifier O;

		public static readonly DerObjectIdentifier OU;

		public static readonly DerObjectIdentifier T;

		public static readonly DerObjectIdentifier CN;

		public static readonly DerObjectIdentifier Street;

		public static readonly DerObjectIdentifier SerialNumber;

		public static readonly DerObjectIdentifier L;

		public static readonly DerObjectIdentifier ST;

		public static readonly DerObjectIdentifier Surname;

		public static readonly DerObjectIdentifier GivenName;

		public static readonly DerObjectIdentifier Initials;

		public static readonly DerObjectIdentifier Generation;

		public static readonly DerObjectIdentifier UniqueIdentifier;

		public static readonly DerObjectIdentifier BusinessCategory;

		public static readonly DerObjectIdentifier PostalCode;

		public static readonly DerObjectIdentifier DnQualifier;

		public static readonly DerObjectIdentifier Pseudonym;

		public static readonly DerObjectIdentifier DateOfBirth;

		public static readonly DerObjectIdentifier PlaceOfBirth;

		public static readonly DerObjectIdentifier Gender;

		public static readonly DerObjectIdentifier CountryOfCitizenship;

		public static readonly DerObjectIdentifier CountryOfResidence;

		public static readonly DerObjectIdentifier NameAtBirth;

		public static readonly DerObjectIdentifier PostalAddress;

		public static readonly DerObjectIdentifier DmdName;

		public static readonly DerObjectIdentifier TelephoneNumber;

		public static readonly DerObjectIdentifier Name;

		public static readonly DerObjectIdentifier EmailAddress;

		public static readonly DerObjectIdentifier UnstructuredName;

		public static readonly DerObjectIdentifier UnstructuredAddress;

		public static readonly DerObjectIdentifier E;

		public static readonly DerObjectIdentifier DC;

		public static readonly DerObjectIdentifier UID;

		private static readonly bool[] defaultReverse;

		public static readonly Hashtable DefaultSymbols;

		public static readonly Hashtable RFC2253Symbols;

		public static readonly Hashtable RFC1779Symbols;

		public static readonly Hashtable DefaultLookup;

		private readonly IList ordering = Platform.CreateArrayList();

		private readonly X509NameEntryConverter converter;

		private IList values = Platform.CreateArrayList();

		private IList added = Platform.CreateArrayList();

		private Asn1Sequence seq;

		public static bool DefaultReverse
		{
			get
			{
				return X509Name.defaultReverse[0];
			}
			set
			{
				X509Name.defaultReverse[0] = value;
			}
		}

		static X509Name()
		{
			X509Name.C = new DerObjectIdentifier("2.5.4.6");
			X509Name.O = new DerObjectIdentifier("2.5.4.10");
			X509Name.OU = new DerObjectIdentifier("2.5.4.11");
			X509Name.T = new DerObjectIdentifier("2.5.4.12");
			X509Name.CN = new DerObjectIdentifier("2.5.4.3");
			X509Name.Street = new DerObjectIdentifier("2.5.4.9");
			X509Name.SerialNumber = new DerObjectIdentifier("2.5.4.5");
			X509Name.L = new DerObjectIdentifier("2.5.4.7");
			X509Name.ST = new DerObjectIdentifier("2.5.4.8");
			X509Name.Surname = new DerObjectIdentifier("2.5.4.4");
			X509Name.GivenName = new DerObjectIdentifier("2.5.4.42");
			X509Name.Initials = new DerObjectIdentifier("2.5.4.43");
			X509Name.Generation = new DerObjectIdentifier("2.5.4.44");
			X509Name.UniqueIdentifier = new DerObjectIdentifier("2.5.4.45");
			X509Name.BusinessCategory = new DerObjectIdentifier("2.5.4.15");
			X509Name.PostalCode = new DerObjectIdentifier("2.5.4.17");
			X509Name.DnQualifier = new DerObjectIdentifier("2.5.4.46");
			X509Name.Pseudonym = new DerObjectIdentifier("2.5.4.65");
			X509Name.DateOfBirth = new DerObjectIdentifier("1.3.6.1.5.5.7.9.1");
			X509Name.PlaceOfBirth = new DerObjectIdentifier("1.3.6.1.5.5.7.9.2");
			X509Name.Gender = new DerObjectIdentifier("1.3.6.1.5.5.7.9.3");
			X509Name.CountryOfCitizenship = new DerObjectIdentifier("1.3.6.1.5.5.7.9.4");
			X509Name.CountryOfResidence = new DerObjectIdentifier("1.3.6.1.5.5.7.9.5");
			X509Name.NameAtBirth = new DerObjectIdentifier("1.3.36.8.3.14");
			X509Name.PostalAddress = new DerObjectIdentifier("2.5.4.16");
			X509Name.DmdName = new DerObjectIdentifier("2.5.4.54");
			X509Name.TelephoneNumber = X509ObjectIdentifiers.id_at_telephoneNumber;
			X509Name.Name = X509ObjectIdentifiers.id_at_name;
			X509Name.EmailAddress = PkcsObjectIdentifiers.Pkcs9AtEmailAddress;
			X509Name.UnstructuredName = PkcsObjectIdentifiers.Pkcs9AtUnstructuredName;
			X509Name.UnstructuredAddress = PkcsObjectIdentifiers.Pkcs9AtUnstructuredAddress;
			X509Name.E = X509Name.EmailAddress;
			X509Name.DC = new DerObjectIdentifier("0.9.2342.19200300.100.1.25");
			X509Name.UID = new DerObjectIdentifier("0.9.2342.19200300.100.1.1");
			bool[] array = new bool[1];
			X509Name.defaultReverse = array;
			X509Name.DefaultSymbols = new Hashtable();
			X509Name.RFC2253Symbols = new Hashtable();
			X509Name.RFC1779Symbols = new Hashtable();
			X509Name.DefaultLookup = new Hashtable();
			X509Name.DefaultSymbols.Add(X509Name.C, "C");
			X509Name.DefaultSymbols.Add(X509Name.O, "O");
			X509Name.DefaultSymbols.Add(X509Name.T, "T");
			X509Name.DefaultSymbols.Add(X509Name.OU, "OU");
			X509Name.DefaultSymbols.Add(X509Name.CN, "CN");
			X509Name.DefaultSymbols.Add(X509Name.L, "L");
			X509Name.DefaultSymbols.Add(X509Name.ST, "ST");
			X509Name.DefaultSymbols.Add(X509Name.SerialNumber, "SERIALNUMBER");
			X509Name.DefaultSymbols.Add(X509Name.EmailAddress, "E");
			X509Name.DefaultSymbols.Add(X509Name.DC, "DC");
			X509Name.DefaultSymbols.Add(X509Name.UID, "UID");
			X509Name.DefaultSymbols.Add(X509Name.Street, "STREET");
			X509Name.DefaultSymbols.Add(X509Name.Surname, "SURNAME");
			X509Name.DefaultSymbols.Add(X509Name.GivenName, "GIVENNAME");
			X509Name.DefaultSymbols.Add(X509Name.Initials, "INITIALS");
			X509Name.DefaultSymbols.Add(X509Name.Generation, "GENERATION");
			X509Name.DefaultSymbols.Add(X509Name.UnstructuredAddress, "unstructuredAddress");
			X509Name.DefaultSymbols.Add(X509Name.UnstructuredName, "unstructuredName");
			X509Name.DefaultSymbols.Add(X509Name.UniqueIdentifier, "UniqueIdentifier");
			X509Name.DefaultSymbols.Add(X509Name.DnQualifier, "DN");
			X509Name.DefaultSymbols.Add(X509Name.Pseudonym, "Pseudonym");
			X509Name.DefaultSymbols.Add(X509Name.PostalAddress, "PostalAddress");
			X509Name.DefaultSymbols.Add(X509Name.NameAtBirth, "NameAtBirth");
			X509Name.DefaultSymbols.Add(X509Name.CountryOfCitizenship, "CountryOfCitizenship");
			X509Name.DefaultSymbols.Add(X509Name.CountryOfResidence, "CountryOfResidence");
			X509Name.DefaultSymbols.Add(X509Name.Gender, "Gender");
			X509Name.DefaultSymbols.Add(X509Name.PlaceOfBirth, "PlaceOfBirth");
			X509Name.DefaultSymbols.Add(X509Name.DateOfBirth, "DateOfBirth");
			X509Name.DefaultSymbols.Add(X509Name.PostalCode, "PostalCode");
			X509Name.DefaultSymbols.Add(X509Name.BusinessCategory, "BusinessCategory");
			X509Name.DefaultSymbols.Add(X509Name.TelephoneNumber, "TelephoneNumber");
			X509Name.RFC2253Symbols.Add(X509Name.C, "C");
			X509Name.RFC2253Symbols.Add(X509Name.O, "O");
			X509Name.RFC2253Symbols.Add(X509Name.OU, "OU");
			X509Name.RFC2253Symbols.Add(X509Name.CN, "CN");
			X509Name.RFC2253Symbols.Add(X509Name.L, "L");
			X509Name.RFC2253Symbols.Add(X509Name.ST, "ST");
			X509Name.RFC2253Symbols.Add(X509Name.Street, "STREET");
			X509Name.RFC2253Symbols.Add(X509Name.DC, "DC");
			X509Name.RFC2253Symbols.Add(X509Name.UID, "UID");
			X509Name.RFC1779Symbols.Add(X509Name.C, "C");
			X509Name.RFC1779Symbols.Add(X509Name.O, "O");
			X509Name.RFC1779Symbols.Add(X509Name.OU, "OU");
			X509Name.RFC1779Symbols.Add(X509Name.CN, "CN");
			X509Name.RFC1779Symbols.Add(X509Name.L, "L");
			X509Name.RFC1779Symbols.Add(X509Name.ST, "ST");
			X509Name.RFC1779Symbols.Add(X509Name.Street, "STREET");
			X509Name.DefaultLookup.Add("c", X509Name.C);
			X509Name.DefaultLookup.Add("o", X509Name.O);
			X509Name.DefaultLookup.Add("t", X509Name.T);
			X509Name.DefaultLookup.Add("ou", X509Name.OU);
			X509Name.DefaultLookup.Add("cn", X509Name.CN);
			X509Name.DefaultLookup.Add("l", X509Name.L);
			X509Name.DefaultLookup.Add("st", X509Name.ST);
			X509Name.DefaultLookup.Add("serialnumber", X509Name.SerialNumber);
			X509Name.DefaultLookup.Add("street", X509Name.Street);
			X509Name.DefaultLookup.Add("emailaddress", X509Name.E);
			X509Name.DefaultLookup.Add("dc", X509Name.DC);
			X509Name.DefaultLookup.Add("e", X509Name.E);
			X509Name.DefaultLookup.Add("uid", X509Name.UID);
			X509Name.DefaultLookup.Add("surname", X509Name.Surname);
			X509Name.DefaultLookup.Add("givenname", X509Name.GivenName);
			X509Name.DefaultLookup.Add("initials", X509Name.Initials);
			X509Name.DefaultLookup.Add("generation", X509Name.Generation);
			X509Name.DefaultLookup.Add("unstructuredaddress", X509Name.UnstructuredAddress);
			X509Name.DefaultLookup.Add("unstructuredname", X509Name.UnstructuredName);
			X509Name.DefaultLookup.Add("uniqueidentifier", X509Name.UniqueIdentifier);
			X509Name.DefaultLookup.Add("dn", X509Name.DnQualifier);
			X509Name.DefaultLookup.Add("pseudonym", X509Name.Pseudonym);
			X509Name.DefaultLookup.Add("postaladdress", X509Name.PostalAddress);
			X509Name.DefaultLookup.Add("nameofbirth", X509Name.NameAtBirth);
			X509Name.DefaultLookup.Add("countryofcitizenship", X509Name.CountryOfCitizenship);
			X509Name.DefaultLookup.Add("countryofresidence", X509Name.CountryOfResidence);
			X509Name.DefaultLookup.Add("gender", X509Name.Gender);
			X509Name.DefaultLookup.Add("placeofbirth", X509Name.PlaceOfBirth);
			X509Name.DefaultLookup.Add("dateofbirth", X509Name.DateOfBirth);
			X509Name.DefaultLookup.Add("postalcode", X509Name.PostalCode);
			X509Name.DefaultLookup.Add("businesscategory", X509Name.BusinessCategory);
			X509Name.DefaultLookup.Add("telephonenumber", X509Name.TelephoneNumber);
		}

		public static X509Name GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return X509Name.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static X509Name GetInstance(object obj)
		{
			if (obj == null || obj is X509Name)
			{
				return (X509Name)obj;
			}
			if (obj != null)
			{
				return new X509Name(Asn1Sequence.GetInstance(obj));
			}
			throw new ArgumentException("null object in factory", "obj");
		}

		protected X509Name()
		{
		}

		protected X509Name(Asn1Sequence seq)
		{
			this.seq = seq;
			foreach (Asn1Encodable asn1Encodable in seq)
			{
				Asn1Set instance = Asn1Set.GetInstance(asn1Encodable.ToAsn1Object());
				for (int i = 0; i < instance.Count; i++)
				{
					Asn1Sequence instance2 = Asn1Sequence.GetInstance(instance[i].ToAsn1Object());
					if (instance2.Count != 2)
					{
						throw new ArgumentException("badly sized pair");
					}
					this.ordering.Add(DerObjectIdentifier.GetInstance(instance2[0].ToAsn1Object()));
					Asn1Object asn1Object = instance2[1].ToAsn1Object();
					if (asn1Object is IAsn1String && !(asn1Object is DerUniversalString))
					{
						string text = ((IAsn1String)asn1Object).GetString();
						if (text.StartsWith("#"))
						{
							text = "\\" + text;
						}
						this.values.Add(text);
					}
					else
					{
						this.values.Add("#" + Hex.ToHexString(asn1Object.GetEncoded()));
					}
					this.added.Add(i != 0);
				}
			}
		}

		public X509Name(IList ordering, IDictionary attributes) : this(ordering, attributes, new X509DefaultEntryConverter())
		{
		}

		public X509Name(IList ordering, IDictionary attributes, X509NameEntryConverter converter)
		{
			this.converter = converter;
			foreach (DerObjectIdentifier derObjectIdentifier in ordering)
			{
				object obj = attributes[derObjectIdentifier];
				if (obj == null)
				{
					throw new ArgumentException("No attribute for object id - " + derObjectIdentifier + " - passed to distinguished name");
				}
				this.ordering.Add(derObjectIdentifier);
				this.added.Add(false);
				this.values.Add(obj);
			}
		}

		public X509Name(IList oids, IList values) : this(oids, values, new X509DefaultEntryConverter())
		{
		}

		public X509Name(IList oids, IList values, X509NameEntryConverter converter)
		{
			this.converter = converter;
			if (oids.Count != values.Count)
			{
				throw new ArgumentException("'oids' must be same length as 'values'.");
			}
			for (int i = 0; i < oids.Count; i++)
			{
				this.ordering.Add(oids[i]);
				this.values.Add(values[i]);
				this.added.Add(false);
			}
		}

		public X509Name(string dirName) : this(X509Name.DefaultReverse, X509Name.DefaultLookup, dirName)
		{
		}

		public X509Name(string dirName, X509NameEntryConverter converter) : this(X509Name.DefaultReverse, X509Name.DefaultLookup, dirName, converter)
		{
		}

		public X509Name(bool reverse, string dirName) : this(reverse, X509Name.DefaultLookup, dirName)
		{
		}

		public X509Name(bool reverse, string dirName, X509NameEntryConverter converter) : this(reverse, X509Name.DefaultLookup, dirName, converter)
		{
		}

		public X509Name(bool reverse, IDictionary lookUp, string dirName) : this(reverse, lookUp, dirName, new X509DefaultEntryConverter())
		{
		}

		private DerObjectIdentifier DecodeOid(string name, IDictionary lookUp)
		{
			if (Platform.ToUpperInvariant(name).StartsWith("OID."))
			{
				return new DerObjectIdentifier(name.Substring(4));
			}
			if (name[0] >= '0' && name[0] <= '9')
			{
				return new DerObjectIdentifier(name);
			}
			DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)lookUp[Platform.ToLowerInvariant(name)];
			if (derObjectIdentifier == null)
			{
				throw new ArgumentException("Unknown object id - " + name + " - passed to distinguished name");
			}
			return derObjectIdentifier;
		}

		public X509Name(bool reverse, IDictionary lookUp, string dirName, X509NameEntryConverter converter)
		{
			this.converter = converter;
			X509NameTokenizer x509NameTokenizer = new X509NameTokenizer(dirName);
			while (x509NameTokenizer.HasMoreTokens())
			{
				string text = x509NameTokenizer.NextToken();
				int num = text.IndexOf('=');
				if (num == -1)
				{
					throw new ArgumentException("badly formated directory string");
				}
				string name = text.Substring(0, num);
				string text2 = text.Substring(num + 1);
				DerObjectIdentifier value = this.DecodeOid(name, lookUp);
				if (text2.IndexOf('+') > 0)
				{
					X509NameTokenizer x509NameTokenizer2 = new X509NameTokenizer(text2, '+');
					string value2 = x509NameTokenizer2.NextToken();
					this.ordering.Add(value);
					this.values.Add(value2);
					this.added.Add(false);
					while (x509NameTokenizer2.HasMoreTokens())
					{
						string text3 = x509NameTokenizer2.NextToken();
						int num2 = text3.IndexOf('=');
						string name2 = text3.Substring(0, num2);
						string value3 = text3.Substring(num2 + 1);
						this.ordering.Add(this.DecodeOid(name2, lookUp));
						this.values.Add(value3);
						this.added.Add(true);
					}
				}
				else
				{
					this.ordering.Add(value);
					this.values.Add(text2);
					this.added.Add(false);
				}
			}
			if (reverse)
			{
				IList list = Platform.CreateArrayList();
				IList list2 = Platform.CreateArrayList();
				IList list3 = Platform.CreateArrayList();
				int num3 = 1;
				for (int i = 0; i < this.ordering.Count; i++)
				{
					if (!(bool)this.added[i])
					{
						num3 = 0;
					}
					int index = num3++;
					list.Insert(index, this.ordering[i]);
					list2.Insert(index, this.values[i]);
					list3.Insert(index, this.added[i]);
				}
				this.ordering = list;
				this.values = list2;
				this.added = list3;
			}
		}

		public IList GetOidList()
		{
			return Platform.CreateArrayList(this.ordering);
		}

		public IList GetValueList()
		{
			return this.GetValueList(null);
		}

		public IList GetValueList(DerObjectIdentifier oid)
		{
			IList list = Platform.CreateArrayList();
			for (int num = 0; num != this.values.Count; num++)
			{
				if (oid == null || oid.Equals(this.ordering[num]))
				{
					string text = (string)this.values[num];
					if (text.StartsWith("\\#"))
					{
						text = text.Substring(1);
					}
					list.Add(text);
				}
			}
			return list;
		}

		public override Asn1Object ToAsn1Object()
		{
			if (this.seq == null)
			{
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[0]);
				Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
				DerObjectIdentifier derObjectIdentifier = null;
				for (int num = 0; num != this.ordering.Count; num++)
				{
					DerObjectIdentifier derObjectIdentifier2 = (DerObjectIdentifier)this.ordering[num];
					string value = (string)this.values[num];
					if (derObjectIdentifier != null && !(bool)this.added[num])
					{
						asn1EncodableVector.Add(new Asn1Encodable[]
						{
							new DerSet(asn1EncodableVector2)
						});
						asn1EncodableVector2 = new Asn1EncodableVector(new Asn1Encodable[0]);
					}
					asn1EncodableVector2.Add(new Asn1Encodable[]
					{
						new DerSequence(new Asn1Encodable[]
						{
							derObjectIdentifier2,
							this.converter.GetConvertedValue(derObjectIdentifier2, value)
						})
					});
					derObjectIdentifier = derObjectIdentifier2;
				}
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					new DerSet(asn1EncodableVector2)
				});
				this.seq = new DerSequence(asn1EncodableVector);
			}
			return this.seq;
		}

		public bool Equivalent(X509Name other, bool inOrder)
		{
			if (!inOrder)
			{
				return this.Equivalent(other);
			}
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			int count = this.ordering.Count;
			if (count != other.ordering.Count)
			{
				return false;
			}
			for (int i = 0; i < count; i++)
			{
				DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)this.ordering[i];
				DerObjectIdentifier obj = (DerObjectIdentifier)other.ordering[i];
				if (!derObjectIdentifier.Equals(obj))
				{
					return false;
				}
				string s = (string)this.values[i];
				string s2 = (string)other.values[i];
				if (!X509Name.equivalentStrings(s, s2))
				{
					return false;
				}
			}
			return true;
		}

		public bool Equivalent(X509Name other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			int count = this.ordering.Count;
			if (count != other.ordering.Count)
			{
				return false;
			}
			bool[] array = new bool[count];
			int num;
			int num2;
			int num3;
			if (this.ordering[0].Equals(other.ordering[0]))
			{
				num = 0;
				num2 = count;
				num3 = 1;
			}
			else
			{
				num = count - 1;
				num2 = -1;
				num3 = -1;
			}
			for (int num4 = num; num4 != num2; num4 += num3)
			{
				bool flag = false;
				DerObjectIdentifier derObjectIdentifier = (DerObjectIdentifier)this.ordering[num4];
				string s = (string)this.values[num4];
				for (int i = 0; i < count; i++)
				{
					if (!array[i])
					{
						DerObjectIdentifier obj = (DerObjectIdentifier)other.ordering[i];
						if (derObjectIdentifier.Equals(obj))
						{
							string s2 = (string)other.values[i];
							if (X509Name.equivalentStrings(s, s2))
							{
								array[i] = true;
								flag = true;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private static bool equivalentStrings(string s1, string s2)
		{
			string text = X509Name.canonicalize(s1);
			string text2 = X509Name.canonicalize(s2);
			if (!text.Equals(text2))
			{
				text = X509Name.stripInternalSpaces(text);
				text2 = X509Name.stripInternalSpaces(text2);
				if (!text.Equals(text2))
				{
					return false;
				}
			}
			return true;
		}

		private static string canonicalize(string s)
		{
			string text = Platform.ToLowerInvariant(s).Trim();
			if (text.StartsWith("#"))
			{
				Asn1Object asn1Object = X509Name.decodeObject(text);
				if (asn1Object is IAsn1String)
				{
					text = Platform.ToLowerInvariant(((IAsn1String)asn1Object).GetString()).Trim();
				}
			}
			return text;
		}

		private static Asn1Object decodeObject(string v)
		{
			Asn1Object result;
			try
			{
				result = Asn1Object.FromByteArray(Hex.Decode(v.Substring(1)));
			}
			catch (IOException ex)
			{
				throw new InvalidOperationException("unknown encoding in name: " + ex.Message, ex);
			}
			return result;
		}

		private static string stripInternalSpaces(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (str.Length != 0)
			{
				char c = str[0];
				stringBuilder.Append(c);
				for (int i = 1; i < str.Length; i++)
				{
					char c2 = str[i];
					if (c != ' ' || c2 != ' ')
					{
						stringBuilder.Append(c2);
					}
					c = c2;
				}
			}
			return stringBuilder.ToString();
		}

		private void AppendValue(StringBuilder buf, IDictionary oidSymbols, DerObjectIdentifier oid, string val)
		{
			string text = (string)oidSymbols[oid];
			if (text != null)
			{
				buf.Append(text);
			}
			else
			{
				buf.Append(oid.Id);
			}
			buf.Append('=');
			int num = buf.Length;
			buf.Append(val);
			int num2 = buf.Length;
			if (val.StartsWith("\\#"))
			{
				num += 2;
			}
			while (num != num2)
			{
				if (buf[num] == ',' || buf[num] == '"' || buf[num] == '\\' || buf[num] == '+' || buf[num] == '=' || buf[num] == '<' || buf[num] == '>' || buf[num] == ';')
				{
					buf.Insert(num++, "\\");
					num2++;
				}
				num++;
			}
		}

		public string ToString(bool reverse, IDictionary oidSymbols)
		{
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder = null;
			for (int i = 0; i < this.ordering.Count; i++)
			{
				if ((bool)this.added[i])
				{
					stringBuilder.Append('+');
					this.AppendValue(stringBuilder, oidSymbols, (DerObjectIdentifier)this.ordering[i], (string)this.values[i]);
				}
				else
				{
					stringBuilder = new StringBuilder();
					this.AppendValue(stringBuilder, oidSymbols, (DerObjectIdentifier)this.ordering[i], (string)this.values[i]);
					arrayList.Add(stringBuilder);
				}
			}
			if (reverse)
			{
				arrayList.Reverse();
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			if (arrayList.Count > 0)
			{
				stringBuilder2.Append(arrayList[0].ToString());
				for (int j = 1; j < arrayList.Count; j++)
				{
					stringBuilder2.Append(',');
					stringBuilder2.Append(arrayList[j].ToString());
				}
			}
			return stringBuilder2.ToString();
		}

		public override string ToString()
		{
			return this.ToString(X509Name.DefaultReverse, X509Name.DefaultSymbols);
		}
	}
}
