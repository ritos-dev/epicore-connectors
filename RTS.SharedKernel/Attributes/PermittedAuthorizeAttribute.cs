namespace RTS.SharedKernel.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PermittedAuthorizeAttribute : Attribute
    {
        public string Role { get; }

        public PermittedAuthorizeAttribute(string role)
        {
            Role = role;
        }
    }
}
