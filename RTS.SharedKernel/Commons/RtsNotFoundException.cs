namespace RTS.SharedKernel.Commons
{
    public class RtsNotFoundException(string message) : RtsException(DATA_NOT_FOUND, message);
}
