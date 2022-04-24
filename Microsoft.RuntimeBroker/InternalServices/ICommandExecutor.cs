namespace Microsoft.RuntimeBroker.InternalServices
{
    public interface ICommandExecutor
    {
        Task WorkAsync(CancellationToken cancellationToken);
    }
}
