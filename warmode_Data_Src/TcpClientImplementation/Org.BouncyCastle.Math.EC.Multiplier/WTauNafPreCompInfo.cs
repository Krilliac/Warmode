using System;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
	public class WTauNafPreCompInfo : PreCompInfo
	{
		protected AbstractF2mPoint[] m_preComp;

		public virtual AbstractF2mPoint[] PreComp
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
	}
}
