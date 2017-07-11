using System;

namespace LegacySystem
{
	public class Thread
	{
		public bool IsBackground
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotImplementedException("currently always on background");
			}
		}

		public Thread(ThreadStart start)
		{
			throw new NotSupportedException();
		}

		public Thread(ParameterizedThreadStart start)
		{
			throw new NotSupportedException();
		}

		public void Abort()
		{
			throw new NotSupportedException();
		}

		public bool Join(int ms)
		{
			throw new NotSupportedException();
		}

		public void Start()
		{
			throw new NotSupportedException();
		}

		public void Start(object param)
		{
			throw new NotSupportedException();
		}

		public static void Sleep(int ms)
		{
			throw new NotSupportedException();
		}
	}
}
