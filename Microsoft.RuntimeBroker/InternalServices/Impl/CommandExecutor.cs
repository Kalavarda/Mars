namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandExecutor: ICommandExecutor
    {
        private readonly ICommandQueue _commandQueue;
        private readonly IResultQueue _resultQueue;

        public CommandExecutor(ICommandQueue commandQueue, IResultQueue resultQueue)
        {
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
            _resultQueue = resultQueue ?? throw new ArgumentNullException(nameof(resultQueue));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var command = await _commandQueue.GetAsync(cancellationToken);
            if (command == null)
                return;

            try
            {
                var result = await command.ExecuteAsync(cancellationToken);
                await _resultQueue.AddAsync(command, result, null);
                await _commandQueue.RemoveAsync(command, cancellationToken);
            }
            catch (Exception e)
            {
                await _resultQueue.AddAsync(command, null, e.GetBaseException());
            }
        }
    }
}
