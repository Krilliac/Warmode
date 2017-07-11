using System;
using System.Threading;

namespace Org.BouncyCastle.Crypto.Prng
{
	public class ThreadedSeedGenerator
	{
		private class SeedGenerator
		{
			private volatile int counter;

			private volatile bool stop;

			private void Run(object ignored)
			{
				while (!this.stop)
				{
					this.counter++;
				}
			}

			public byte[] GenerateSeed(int numBytes, bool fast)
			{
				ThreadPriority priority = Thread.CurrentThread.Priority;
				byte[] result;
				try
				{
					Thread.CurrentThread.Priority = ThreadPriority.Normal;
					result = this.DoGenerateSeed(numBytes, fast);
				}
				finally
				{
					Thread.CurrentThread.Priority = priority;
				}
				return result;
			}

			private byte[] DoGenerateSeed(int numBytes, bool fast)
			{
				this.counter = 0;
				this.stop = false;
				byte[] array = new byte[numBytes];
				int num = 0;
				int num2 = fast ? numBytes : (numBytes * 8);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.Run));
				for (int i = 0; i < num2; i++)
				{
					while (this.counter == num)
					{
						try
						{
							Thread.Sleep(1);
						}
						catch (Exception)
						{
						}
					}
					num = this.counter;
					if (fast)
					{
						array[i] = (byte)num;
					}
					else
					{
						int num3 = i / 8;
						array[num3] = (byte)((int)array[num3] << 1 | (num & 1));
					}
				}
				this.stop = true;
				return array;
			}
		}

		public byte[] GenerateSeed(int numBytes, bool fast)
		{
			return new ThreadedSeedGenerator.SeedGenerator().GenerateSeed(numBytes, fast);
		}
	}
}
