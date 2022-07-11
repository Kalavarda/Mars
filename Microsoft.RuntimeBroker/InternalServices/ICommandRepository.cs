using Microsoft.RuntimeBroker.Models.Commands;
using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.InternalServices
{
    internal interface ICommandReadonlyRepository
    {
        Task<IReadOnlyCollection<CommandBase>> GetNeedToExecAsync(CancellationToken cancellationToken);
        
        Task<IReadOnlyDictionary<CommandBase, ExecResult>> GetCompletedAsync(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<DateTime>> GetExecTimesAsync(CommandBase command, CancellationToken cancellationToken);
    }

    internal interface ICommandRepository: ICommandReadonlyRepository
    {
        Task AddAsync(CommandBase command, CancellationToken cancellationToken);

        Task RemoveAsync(CommandBase command, CancellationToken cancellationToken);
        
        Task AddExecResultAsync(CommandBase command, SuccessResultBase result, CancellationToken cancellationToken);
        
        Task AddExecErrorAsync(CommandBase command, Exception error, CancellationToken cancellationToken);
    }
}
