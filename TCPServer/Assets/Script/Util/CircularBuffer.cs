using System;

namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// CircularBuffer
    /// 원형바이트버퍼
    /// & 연산을 이용한 원형 구현
    /// & 연산 특성상 (2^N)의 크기만 지원 
    /// </summary>
    public class CircularBuffer
    {
        private byte[] mBuffer = null;
        private int mCapacity = 0;
        private int mCapacityMask = 0;
        private int mHead = 0;
        private int mTail = 0;

        private int PowerOfTwo(int e)
        {
            if (e == 0)
                return 1;

            return 2 * (PowerOfTwo(e - 1));
        }

        public CircularBuffer(int capacityPower)
        {
            mCapacity = PowerOfTwo(capacityPower);
            mCapacityMask = mCapacity - 1;
            mBuffer = new byte[mCapacity];

        }

        public WeakReference GetBuffer()
        {
            return new WeakReference(mBuffer);
        }


        /// <summary>
        /// 버퍼에 저장된 사이즈
        /// </summary>
        /// <returns></returns>
        public int GetStoredSize()
        {
            if (mHead > mTail)
            {
                return mHead - mTail;
            }
            else if (mHead < mTail)
            {
                return (mCapacity - mTail) + mHead;
            }

            return 0;
        }


        /// <summary>
        /// Peek
        /// bytes 사이즈만큼 버퍼에서 읽어서 destBuf에 복사.
        /// 읽은 데이터는 삭제되지 않음.
        /// </summary>
        /// <param name="destBuf">The dest buf.</param>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public bool Peek(byte[] destBuf, int bytes)
        {
            if (bytes > GetStoredSize())
                return false;

            int readOffset = mTail + bytes;
            int afterReadBytes = readOffset > mCapacity ? readOffset & mCapacityMask : 0;
            int readBytes = bytes - afterReadBytes;
            Buffer.BlockCopy(mBuffer, mTail, destBuf, 0, readBytes);

            if (afterReadBytes > 0)
            {
                Buffer.BlockCopy(mBuffer, 0, destBuf, readBytes, afterReadBytes);
            }

            return true;
        }

        /// <summary>
        /// Read
        /// bytes 사이즈만큼 버퍼에서 읽어서 destBuf에 복사.
        /// 읽은 데이터는 버퍼에서 삭제
        /// </summary>
        /// <param name="destBuf">The dest buf.</param>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public bool Read(byte[] destBuf, int bytes)
        {
            if (bytes > GetStoredSize())
                return false;

            int readOffset = mTail + bytes;
            int afterReadBytes = readOffset > mCapacity ? readOffset & mCapacityMask : 0;
            int readBytes = bytes - afterReadBytes;
            Buffer.BlockCopy(mBuffer, mTail, destBuf, 0, readBytes);

            if (afterReadBytes > 0)
            {
                Buffer.BlockCopy(mBuffer, 0, destBuf, readBytes, afterReadBytes);
            }

            mTail = readOffset & mCapacityMask;

            return true;
        }



        public bool Write(byte[] data, int offset, int bytes)
        {
            /***
             * 저장 공간 초과
             */
            if (mCapacity < GetStoredSize() + bytes)
            {
                return false;
            }

            // 남은 쓰기 Offset
            int writeOffset = mHead + bytes;

            // Head가 Capacity를 넘어 갈 경우 Write Offset을 0로 바꾼 후 남은 데이터를 써야한다.
            int afterWriteBytes = writeOffset > mCapacity ? writeOffset & mCapacityMask : 0;

            // 우선 써야 할 데이터 용량 기록
            int writeBytes = bytes - afterWriteBytes;
            Buffer.BlockCopy(data, offset, mBuffer, mHead, writeBytes);

            // 써야할 데이터가 남았다면 Write Offset을 0으로 한 후 기록
            if (afterWriteBytes > 0)
            {
                Buffer.BlockCopy(data, offset + writeBytes, mBuffer, 0, afterWriteBytes);
            }

            mHead = writeOffset & mCapacityMask;

            return true;

        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        public bool Remove(int bytes)
        {
            if (bytes > GetStoredSize())
                return false;

            mTail = (mTail + bytes) & mCapacityMask;

            return true;
        }

        public void Clear()
        {
            mHead = 0;
            mTail = 0;
        }
    }
}
