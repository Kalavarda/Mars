using Microsoft.RuntimeBroker.Models.Commands;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandReceiver: ICommandReceiver
    {
        private readonly ICommandQueue _queue;

        public CommandReceiver(ICommandQueue queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            await _queue.AddAsync(new GetMachineNameCommand(), cancellationToken);
        }
    }
}
