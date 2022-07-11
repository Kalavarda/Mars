using Kalantyr.Web;

namespace Mars.Retranslator
{
    public static class Errors
    {
        public static Error AccessDenied { get; } = new()
        {
            Code = nameof(AccessDenied),
            Message = "Access denied"
        };
    }
}
