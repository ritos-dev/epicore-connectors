namespace RTS.SharedKernel.Commons
{
    public class RtsException: InvalidOperationException
    {
        public string Key { get; private set; }

        public const string DATA_NOT_FOUND = "DataNotFound";
        public const string INVALID_DATA = "InvalidData";
        public const string OPERATION_ERROR = "OperationError";
        public const string CRITICAL_ERROR = "CriticalError";

        public RtsException(string key) : base() 
        {
            Key = key;
        }
        public RtsException(string key, string message) : base(message) 
        {
            Key = key;
        }
        public RtsException(string key, string message, Exception inner) : base(message, inner) 
        {
            Key = key;
        }
    }
}