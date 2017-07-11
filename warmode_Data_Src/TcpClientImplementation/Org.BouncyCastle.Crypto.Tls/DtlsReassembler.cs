using Org.BouncyCastle.Utilities;
using System;
using System.Collections;

namespace Org.BouncyCastle.Crypto.Tls
{
	internal class DtlsReassembler
	{
		private class Range
		{
			private int mStart;

			private int mEnd;

			public int Start
			{
				get
				{
					return this.mStart;
				}
				set
				{
					this.mStart = value;
				}
			}

			public int End
			{
				get
				{
					return this.mEnd;
				}
				set
				{
					this.mEnd = value;
				}
			}

			internal Range(int start, int end)
			{
				this.mStart = start;
				this.mEnd = end;
			}
		}

		private readonly byte mMsgType;

		private readonly byte[] mBody;

		private readonly IList mMissing = Platform.CreateArrayList();

		internal byte MsgType
		{
			get
			{
				return this.mMsgType;
			}
		}

		internal DtlsReassembler(byte msg_type, int length)
		{
			this.mMsgType = msg_type;
			this.mBody = new byte[length];
			this.mMissing.Add(new DtlsReassembler.Range(0, length));
		}

		internal byte[] GetBodyIfComplete()
		{
			if (this.mMissing.Count != 0)
			{
				return null;
			}
			return this.mBody;
		}

		internal void ContributeFragment(byte msg_type, int length, byte[] buf, int off, int fragment_offset, int fragment_length)
		{
			int num = fragment_offset + fragment_length;
			if (this.mMsgType != msg_type || this.mBody.Length != length || num > length)
			{
				return;
			}
			if (fragment_length == 0)
			{
				if (fragment_offset == 0 && this.mMissing.Count > 0)
				{
					DtlsReassembler.Range range = (DtlsReassembler.Range)this.mMissing[0];
					if (range.End == 0)
					{
						this.mMissing.RemoveAt(0);
					}
				}
				return;
			}
			for (int i = 0; i < this.mMissing.Count; i++)
			{
				DtlsReassembler.Range range2 = (DtlsReassembler.Range)this.mMissing[i];
				if (range2.Start >= num)
				{
					return;
				}
				if (range2.End > fragment_offset)
				{
					int num2 = Math.Max(range2.Start, fragment_offset);
					int num3 = Math.Min(range2.End, num);
					int length2 = num3 - num2;
					Array.Copy(buf, off + num2 - fragment_offset, this.mBody, num2, length2);
					if (num2 == range2.Start)
					{
						if (num3 == range2.End)
						{
							this.mMissing.RemoveAt(i--);
						}
						else
						{
							range2.Start = num3;
						}
					}
					else
					{
						if (num3 != range2.End)
						{
							this.mMissing.Insert(++i, new DtlsReassembler.Range(num3, range2.End));
						}
						range2.End = num2;
					}
				}
			}
		}

		internal void Reset()
		{
			this.mMissing.Clear();
			this.mMissing.Add(new DtlsReassembler.Range(0, this.mBody.Length));
		}
	}
}
