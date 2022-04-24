namespace Microsoft.RuntimeBroker.InternalServices
{
    public interface IResultSender
    {
        Task WorkAsync(CancellationToken cancellationToken);
    }
}
