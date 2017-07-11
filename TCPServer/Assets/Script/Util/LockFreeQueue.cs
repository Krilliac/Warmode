using System.Threading;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// LockFreeQueue
    /// Interlocked을 이용한 Thread Safe Queue
    /// Multi Push, Multi Pop이 가능.
    /// 다만, Pop에서 여러 쓰레드에서 경쟁이 붙는 경우 Null값이 리턴 될 수 있음.
    /// 
    /// Warning : 큐의 구현 특성상 long 범위가 넘어가면 뻑남.
    /// 현실적으로 넘어갈 일은 없기때문에 무시해도 좋지만 에러 가능성을 염두에 두어야함.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LockFreeQueue<T> where T : class
    {
        private long mHeadPos;
        private long mTailPos;
        private T[] mElements;

        private long mQueueMaxSize;
        private long mQueueSizeMask;

        private long PowerOfTwo(int e)
        {
            if (e == 0)
                return 1;

            return 2 * (PowerOfTwo(e - 1));
        }

        public LockFreeQueue(int capacityPower)
        {
            mQueueMaxSize = PowerOfTwo(capacityPower);
            mQueueSizeMask = mQueueMaxSize - 1;

            mElements = new T[mQueueMaxSize];

            mHeadPos = 0;
            mTailPos = 0;
        }

        /// <summary>
        /// Push
        /// Thread Safe
        /// Multi Push 가능
        /// </summary>
        /// <param name="newElem"></param>
        public void Push(T newElem)
        {
            long insertPos = Interlocked.Increment(ref mTailPos) - 1;

            mElements[insertPos & mQueueSizeMask] = newElem;
        }

        /// <summary>
        /// Pop
        /// Thread Safe
        /// Multi Pop 가능
        /// 쓰레드간 경쟁이 붙는 경우 Null이 반환 될 수 있음.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T popVal = Interlocked.Exchange<T>(ref mElements[mHeadPos & mQueueSizeMask], null);

            if (popVal != null)
            {
                Interlocked.Increment(ref mHeadPos);
            }

            return popVal;
        }

        public long getSize()
        {
            return mTailPos - mHeadPos;
        }

    }
}