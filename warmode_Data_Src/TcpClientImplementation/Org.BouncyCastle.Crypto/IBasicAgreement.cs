using Org.BouncyCastle.Math;
using System;

namespace Org.BouncyCastle.Crypto
{
	public interface IBasicAgreement
	{
		void Init(ICipherParameters parameters);

		int GetFieldSize();

		BigInteger CalculateAgreement(ICipherParameters pubKey);
	}
}
