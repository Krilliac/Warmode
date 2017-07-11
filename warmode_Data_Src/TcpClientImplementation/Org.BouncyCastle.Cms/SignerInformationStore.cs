using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Cms
{
	public class SignerInformationStore
	{
		private readonly IList all;

		private readonly IDictionary table = Platform.CreateHashtable();

		public int Count
		{
			get
			{
				return this.all.Count;
			}
		}

		public SignerInformationStore(SignerInformation signerInfo)
		{
			this.all = Platform.CreateArrayList(1);
			this.all.Add(signerInfo);
			SignerID signerID = signerInfo.SignerID;
			this.table[signerID] = this.all;
		}

		public SignerInformationStore(ICollection signerInfos)
		{
			foreach (SignerInformation signerInformation in signerInfos)
			{
				SignerID signerID = signerInformation.SignerID;
				IList list = (IList)this.table[signerID];
				if (list == null)
				{
					list = (this.table[signerID] = Platform.CreateArrayList(1));
				}
				list.Add(signerInformation);
			}
			this.all = Platform.CreateArrayList(signerInfos);
		}

		public SignerInformation GetFirstSigner(SignerID selector)
		{
			IList list = (IList)this.table[selector];
			if (list != null)
			{
				return (SignerInformation)list[0];
			}
			return null;
		}

		public ICollection GetSigners()
		{
			return Platform.CreateArrayList(this.all);
		}

		public ICollection GetSigners(SignerID selector)
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
