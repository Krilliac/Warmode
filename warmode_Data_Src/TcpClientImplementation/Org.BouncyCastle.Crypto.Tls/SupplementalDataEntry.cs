using System;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class SupplementalDataEntry
	{
		protected readonly int mDataType;

		protected readonly byte[] mData;

		public virtual int DataType
		{
			get
			{
				return this.mDataType;
			}
		}

		public virtual byte[] Data
		{
			get
			{
				return this.mData;
			}
		}

		public SupplementalDataEntry(int dataType, byte[] data)
		{
			this.mDataType = dataType;
			this.mData = data;
		}
	}
}
