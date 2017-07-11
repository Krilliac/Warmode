using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace BestHTTP.PlatformSupport.Cryptography
{
	[ComVisible(true)]
	public abstract class MD5 : HashAlgorithm
	{
		protected MD5()
		{
			this.HashSizeValue = 128;
		}

		public new static MD5 Create()
		{
			return new MD5CryptoServiceProvider();
		}
	}
}
