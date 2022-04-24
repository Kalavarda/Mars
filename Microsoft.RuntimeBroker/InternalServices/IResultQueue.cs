using Microsoft.RuntimeBroker.Models.Commands;
using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.InternalServices
{
    internal interface IResultQueue
    {
        Task AddAsync(CommandBase command, SuccessResultBase execResult, Exception error);
        
        Task<ExecResult> GetAsync(CancellationToken cancellationToken);
        
        Task RemoveAsync(ExecResult result, CancellationToken cancellationToken);
    }
}
