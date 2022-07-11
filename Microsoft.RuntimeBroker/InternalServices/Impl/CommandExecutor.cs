using Microsoft.RuntimeBroker.Models.Commands;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandExecutor: ICommandExecutor
    {
        private readonly ICommandRepository _commandRepository;

        public CommandExecutor(ICommandRepository commandRepository)
        {
            _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
        }

        public async Task WorkAsync(CancellationToken cancellationToken)
        {
            var commands = await _commandRepository.GetNeedToExecAsync(cancellationToken);
            foreach (var command in commands)
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var nextTime = await GetNextTime(command, cancellationToken);
                    if (nextTime > DateTime.Now)
                        continue;

                    var result = await command.ExecuteAsync(cancellationToken);
                    await _commandRepository.AddExecResultAsync(command, result, cancellationToken);
                }
                catch (Exception e)
                {
                    await _commandRepository.AddExecErrorAsync(command, e.GetBaseException(), cancellationToken);
                }
        }

        private async Task<DateTime> GetNextTime(CommandBase command, CancellationToken cancellationToken)
        {
            var lastTime = DateTime.MinValue;
            var history = await _commandRepository.GetExecTimesAsync(command, cancellationToken);
            if (history.Any())
                lastTime = history.MaxBy(dt => dt);
            var count = history.Count;
            return lastTime + TimeSpan.FromMinutes(count * count);
        }
    }
}
