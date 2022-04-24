using Microsoft.RuntimeBroker.Models.Commands;

namespace Microsoft.RuntimeBroker.InternalServices
{
    internal interface ICommandQueue
    {
        Task AddAsync(CommandBase command, CancellationToken cancellationToken);

        Task<CommandBase> GetAsync(CancellationToken cancellationToken);
        
        Task RemoveAsync(CommandBase command, CancellationToken cancellationToken);
    }
}
