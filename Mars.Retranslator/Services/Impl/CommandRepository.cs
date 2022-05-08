using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.Retranslator.Services.Impl
{
    public class CommandRepository: ICommandRepository
    {
        private readonly ICollection<CommandBase> _commands = new List<CommandBase>();

        public Task<CommandBase> GetAsync(uint commandId, CancellationToken cancellationToken)
        {
            var command = _commands.FirstOrDefault(c => c.Id == commandId);
            return Task.FromResult(command);
        }
    }
}
