namespace RTS.SharedKernel.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermittedAppAuthorizeAttribute : Attribute
    {
        public string AppKeyHeader { get; }

        public PermittedAppAuthorizeAttribute(string appKeyHeader)
        {
            AppKeyHeader = appKeyHeader;
        }
    }
}
