using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public class ServerSrpParams
	{
		protected BigInteger m_N;

		protected BigInteger m_g;

		protected BigInteger m_B;

		protected byte[] m_s;

		public virtual BigInteger B
		{
			get
			{
				return this.m_B;
			}
		}

		public virtual BigInteger G
		{
			get
			{
				return this.m_g;
			}
		}

		public virtual BigInteger N
		{
			get
			{
				return this.m_N;
			}
		}

		public virtual byte[] S
		{
			get
			{
				return this.m_s;
			}
		}

		public ServerSrpParams(BigInteger N, BigInteger g, byte[] s, BigInteger B)
		{
			this.m_N = N;
			this.m_g = g;
			this.m_s = Arrays.Clone(s);
			this.m_B = B;
		}

		public virtual void Encode(Stream output)
		{
			TlsSrpUtilities.WriteSrpParameter(this.m_N, output);
			TlsSrpUtilities.WriteSrpParameter(this.m_g, output);
			TlsUtilities.WriteOpaque8(this.m_s, output);
			TlsSrpUtilities.WriteSrpParameter(this.m_B, output);
		}

		public static ServerSrpParams Parse(Stream input)
		{
			BigInteger n = TlsSrpUtilities.ReadSrpParameter(input);
			BigInteger g = TlsSrpUtilities.ReadSrpParameter(input);
			byte[] s = TlsUtilities.ReadOpaque8(input);
			BigInteger b = TlsSrpUtilities.ReadSrpParameter(input);
			return new ServerSrpParams(n, g, s, b);
		}
	}
}
