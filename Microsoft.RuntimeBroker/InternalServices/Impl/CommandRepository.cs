using Microsoft.RuntimeBroker.Models.Commands;
using Microsoft.RuntimeBroker.Models.Commands.Dto;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandRepository: ICommandRepository
    {
        private readonly IDictionary<CommandBase, ICollection<CommandExecutionResult>> _commands = new Dictionary<CommandBase, ICollection<CommandExecutionResult>>();

        public async Task AddAsync(CommandBase command, CancellationToken cancellationToken)
        {
            if (_commands.Keys.Any(c => c.Id == command.Id))
                return;

            _commands.Add(command, new List<CommandExecutionResult>());
        }

        public Task<IReadOnlyCollection<CommandBase>> GetNeedToExecAsync(CancellationToken cancellationToken)
        {
            var uncompleted = _commands
                .Where(p => p.Value.Count < 5)
                .Where(p => p.Value.Count == 0 || p.Value.All(r => r.Error != null))
                .Select(p => p.Key)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<CommandBase>>(uncompleted);
        }

        public async Task<IReadOnlyDictionary<CommandBase, ExecResult>> GetCompletedAsync(CancellationToken cancellationToken)
        {
            return _commands
                .Where(p => p.Value.Count >= 5 || p.Value.Any(r => r.SuccessResult != null))
                .ToDictionary(p => p.Key, CreateExecResult);
        }

        public Task<IReadOnlyCollection<DateTime>> GetExecTimesAsync(CommandBase command, CancellationToken cancellationToken)
        {
            var times = _commands[command]
                .Select(r => r.Time)
                .ToArray();
            return Task.FromResult<IReadOnlyCollection<DateTime>>(times);
        }

        private static ExecResult CreateExecResult(KeyValuePair<CommandBase, ICollection<CommandExecutionResult>> pair)
        {
            if (!pair.Value.Any())
                throw new NotImplementedException();

            var successResult = pair.Value
                .Select(r => r.SuccessResult)
                .FirstOrDefault();

            var error = successResult != null
                ? null
                : pair.Value.Select(r => r.Error).FirstOrDefault();

            return new ExecResult
            {
                CommandId = pair.Key.Id,
                Result = successResult,
                Error = error?.GetBaseException().Message
            };
        }

        public Task RemoveAsync(CommandBase command, CancellationToken cancellationToken)
        {
            _commands.Remove(command);
            return Task.CompletedTask;
        }

        public Task AddExecResultAsync(CommandBase command, SuccessResultBase result, CancellationToken cancellationToken)
        {
            var results = _commands[command];
            results.Add(new CommandExecutionResult
            {
                SuccessResult = result,
                Time = DateTime.Now
            });
            return Task.CompletedTask;
        }

        public Task AddExecErrorAsync(CommandBase command, Exception error, CancellationToken cancellationToken)
        {
            var results = _commands[command];
            results.Add(new CommandExecutionResult
            {
                Error = error,
                Time = DateTime.Now
            });
            return Task.CompletedTask;
        }
    }
}
