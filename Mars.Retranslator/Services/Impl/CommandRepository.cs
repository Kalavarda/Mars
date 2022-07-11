using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mars.Retranslator.DataModels;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.Retranslator.Services.Impl
{
    public class CommandRepository: ICommandRepository
    {
        private readonly IDictionary<string, ICollection<CommandRecord>> _commands = new ConcurrentDictionary<string, ICollection<CommandRecord>>();
        private uint _id;

        public Task<CommandRecord> GetAsync(uint commandId, CancellationToken cancellationToken)
        {
            var command = _commands.Values.SelectMany(v => v).FirstOrDefault(c => c.Id == commandId);
            return Task.FromResult(command);
        }

        public async Task<CommandRecord> GetNextAsync(string machineName, CancellationToken cancellationToken)
        {
            if (!_commands.ContainsKey(machineName))
            {
                _id++;
                var cmd = new GetSystemInfoCommand
                {
                    Id = _id,
                    CommandParameters = new SystemInfoCommandParameters
                    {
                        MachineName = true
                    }
                };
                var record = Map(cmd);
                record.Status = CommandStatus.New;
                _commands.Add(machineName, new List<CommandRecord> { record });
            }

            return _commands[machineName].FirstOrDefault(r => r.Status == CommandStatus.New);
        }

        public Task UpdateAsync(CommandRecord record, CancellationToken cancellationToken)
        {
            return Task.FromResult(record);
        }

        private static CommandRecord Map(CommandBase command)
        {
            if (command == null)
                return null;

            return new CommandRecord
            {
                Id = command.Id,
                Type = command.GetType().Name,
                Data = command.Serialize()
            };
        }
    }
}
