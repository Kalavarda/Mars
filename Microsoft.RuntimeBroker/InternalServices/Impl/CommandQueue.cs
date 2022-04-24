using Microsoft.RuntimeBroker.Models.Commands;

namespace Microsoft.RuntimeBroker.InternalServices.Impl
{
    internal class CommandQueue: ICommandQueue
    {
        private readonly ICollection<CommandBase> _commands = new List<CommandBase>();

        public Task AddAsync(CommandBase command, CancellationToken cancellationToken)
        {
            _commands.Add(command);
            return Task.CompletedTask;
        }

        public Task<CommandBase> GetAsync(CancellationToken cancellationToken)
        {
            return _commands.Count == 0
                ? Task.FromResult<CommandBase>(null)
                : Task.FromResult(_commands.First());
        }

        public Task RemoveAsync(CommandBase command, CancellationToken cancellationToken)
        {
            _commands.Remove(command);
            return Task.CompletedTask;
        }
    }
}
