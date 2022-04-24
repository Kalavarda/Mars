using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.Models.Commands
{
    public class GetMachineNameCommand: CommandBase
    {
        public override Task<SuccessResultBase> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = new GetMachineNameResult { MachineName = Environment.MachineName };
            return Task.FromResult<SuccessResultBase>(result);
        }
    }
}
