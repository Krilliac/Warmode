using System;
using System.Collections;
using System.Globalization;

namespace Org.BouncyCastle.Utilities
{
	internal abstract class Platform
	{
		internal static readonly string NewLine = Platform.GetNewLine();

		private static string GetNewLine()
		{
			return Environment.NewLine;
		}

		internal static int CompareIgnoreCase(string a, string b)
		{
			return string.Compare(a, b, true);
		}

		internal static string GetEnvironmentVariable(string variable)
		{
			string result;
			try
			{
				result = Environment.GetEnvironmentVariable(variable);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		internal static Exception CreateNotImplementedException(string message)
		{
			return new NotImplementedException(message);
		}

		internal static IList CreateArrayList()
		{
			return new ArrayList();
		}

		internal static IList CreateArrayList(int capacity)
		{
			return new ArrayList(capacity);
		}

		internal static IList CreateArrayList(ICollection collection)
		{
			return new ArrayList(collection);
		}

		internal static IList CreateArrayList(IEnumerable collection)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object current in collection)
			{
				arrayList.Add(current);
			}
			return arrayList;
		}

		internal static IDictionary CreateHashtable()
		{
			return new Hashtable();
		}

		internal static IDictionary CreateHashtable(int capacity)
		{
			return new Hashtable(capacity);
		}

		internal static IDictionary CreateHashtable(IDictionary dictionary)
		{
			return new Hashtable(dictionary);
		}

		internal static string ToLowerInvariant(string s)
		{
			return s.ToLower(CultureInfo.InvariantCulture);
		}

		internal static string ToUpperInvariant(string s)
		{
			return s.ToUpper(CultureInfo.InvariantCulture);
		}
	}
}
