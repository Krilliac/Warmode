using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class WNafPreCompInfo : PreCompInfo
	{
		protected ECPoint[] m_preComp;

		protected ECPoint[] m_preCompNeg;

		protected ECPoint m_twice;

		public virtual ECPoint[] PreComp
		{
			get
			{
				return this.m_preComp;
			}
			set
			{
				this.m_preComp = value;
			}
		}

		public virtual ECPoint[] PreCompNeg
		{
			get
			{
				return this.m_preCompNeg;
			}
			set
			{
				this.m_preCompNeg = value;
			}
		}

		public virtual ECPoint Twice
		{
			get
			{
				return this.m_twice;
			}
			set
			{
				this.m_twice = value;
			}
		}
	}
}
