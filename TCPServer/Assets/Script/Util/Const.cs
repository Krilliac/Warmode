namespace UNITY_TCPCLIENT
{
    /// <summary>
    /// 2015/12/05 Hyeon
    /// Const
    /// </summary>
    public class Const<T>
    {
        public T Value { get; private set; }

        public Const() { }

        public Const(T value)
            : this()
        {
            this.Value = value;
        }
    }
}

