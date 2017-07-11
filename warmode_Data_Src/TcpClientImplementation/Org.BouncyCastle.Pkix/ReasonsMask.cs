using Org.BouncyCastle.Asn1.X509;
using System;

namespace Org.BouncyCastle.Pkix
{
	internal class ReasonsMask
	{
		private int _reasons;

		internal static readonly ReasonsMask AllReasons = new ReasonsMask(33023);

		internal bool IsAllReasons
		{
			get
			{
				return this._reasons == ReasonsMask.AllReasons._reasons;
			}
		}

		public ReasonFlags Reasons
		{
			get
			{
				return new ReasonFlags(this._reasons);
			}
		}

		internal ReasonsMask(int reasons)
		{
			this._reasons = reasons;
		}

		internal ReasonsMask() : this(0)
		{
		}

		internal void AddReasons(ReasonsMask mask)
		{
			this._reasons |= mask.Reasons.IntValue;
		}

		internal ReasonsMask Intersect(ReasonsMask mask)
		{
			ReasonsMask reasonsMask = new ReasonsMask();
			reasonsMask.AddReasons(new ReasonsMask(this._reasons & mask.Reasons.IntValue));
			return reasonsMask;
		}

		internal bool HasNewReasons(ReasonsMask mask)
		{
			return (this._reasons | (mask.Reasons.IntValue ^ this._reasons)) != 0;
		}
	}
}
