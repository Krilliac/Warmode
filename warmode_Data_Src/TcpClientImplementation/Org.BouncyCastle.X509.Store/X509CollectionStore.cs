using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.X509.Store
{
	internal class X509CollectionStore : IX509Store
	{
		private ICollection _local;

		internal X509CollectionStore(ICollection collection)
		{
			this._local = Platform.CreateArrayList(collection);
		}

		public ICollection GetMatches(IX509Selector selector)
		{
			if (selector == null)
			{
				return Platform.CreateArrayList(this._local);
			}
			IList list = Platform.CreateArrayList();
			foreach (object current in this._local)
			{
				if (selector.Match(current))
				{
					list.Add(current);
				}
			}
			return list;
		}
	}
}
