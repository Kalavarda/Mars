namespace Microsoft.RuntimeBroker.InternalServices
{
    public interface ICommandReceiver
    {
        Task WorkAsync(CancellationToken cancellationToken);
    }
}
