using System;

namespace Facebook.Unity
{
	public interface IPayResult : IResult
	{
		long ErrorCode
		{
			get;
		}
	}
}
