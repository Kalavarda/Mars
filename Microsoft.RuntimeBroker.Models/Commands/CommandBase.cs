using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.Models.Commands
{
    public abstract class CommandBase
    {
        public uint Id { get; set; }

        public abstract Task<SuccessResultBase> ExecuteAsync(CancellationToken cancellationToken);
    }
}