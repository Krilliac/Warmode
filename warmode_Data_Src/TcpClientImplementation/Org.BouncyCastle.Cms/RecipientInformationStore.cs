using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public class RecipientInformationStore
	{
		private readonly IList all;

		private readonly IDictionary table = Platform.CreateHashtable();

		public RecipientInformation this[RecipientID selector]
		{
			get
			{
				return this.GetFirstRecipient(selector);
			}
		}

		public int Count
		{
			get
			{
				return this.all.Count;
			}
		}

		public RecipientInformationStore(ICollection recipientInfos)
		{
			foreach (RecipientInformation recipientInformation in recipientInfos)
			{
				RecipientID recipientID = recipientInformation.RecipientID;
				IList list = (IList)this.table[recipientID];
				if (list == null)
				{
					list = (this.table[recipientID] = Platform.CreateArrayList(1));
				}
				list.Add(recipientInformation);
			}
			this.all = Platform.CreateArrayList(recipientInfos);
		}

		public RecipientInformation GetFirstRecipient(RecipientID selector)
		{
			IList list = (IList)this.table[selector];
			if (list != null)
			{
				return (RecipientInformation)list[0];
			}
			return null;
		}

		public ICollection GetRecipients()
		{
			return Platform.CreateArrayList(this.all);
		}

		public ICollection GetRecipients(RecipientID selector)
		{
			IList list = (IList)this.table[selector];
			if (list != null)
			{
				return Platform.CreateArrayList(list);
			}
			return Platform.CreateArrayList();
		}
	}
}
