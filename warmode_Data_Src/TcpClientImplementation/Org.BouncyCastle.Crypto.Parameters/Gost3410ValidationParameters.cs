using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class Gost3410ValidationParameters
	{
		private int x0;

		private int c;

		private long x0L;

		private long cL;

		public int C
		{
			get
			{
				return this.c;
			}
		}

		public int X0
		{
			get
			{
				return this.x0;
			}
		}

		public long CL
		{
			get
			{
				return this.cL;
			}
		}

		public long X0L
		{
			get
			{
				return this.x0L;
			}
		}

		public Gost3410ValidationParameters(int x0, int c)
		{
			this.x0 = x0;
			this.c = c;
		}

		public Gost3410ValidationParameters(long x0L, long cL)
		{
			this.x0L = x0L;
			this.cL = cL;
		}

		public override bool Equals(object obj)
		{
			Gost3410ValidationParameters gost3410ValidationParameters = obj as Gost3410ValidationParameters;
			return gost3410ValidationParameters != null && gost3410ValidationParameters.c == this.c && gost3410ValidationParameters.x0 == this.x0 && gost3410ValidationParameters.cL == this.cL && gost3410ValidationParameters.x0L == this.x0L;
		}

		public override int GetHashCode()
		{
			return this.c.GetHashCode() ^ this.x0.GetHashCode() ^ this.cL.GetHashCode() ^ this.x0L.GetHashCode();
		}
	}
}
