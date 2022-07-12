using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mars.Retranslator.DataModels;
using Microsoft.RuntimeBroker.Models;

namespace Mars.Retranslator.Services.Impl
{
    public class CommandRepository: ICommandRepository
    {
        private readonly IDictionary<uint, ICollection<CommandRecord>> _commands = new ConcurrentDictionary<uint, ICollection<CommandRecord>>();
        private readonly ICollection<Instance> _instances = new List<Instance>();
        private uint _id;

        public Task<CommandRecord> GetAsync(uint commandId, CancellationToken cancellationToken)
        {
            var command = _commands.Values.SelectMany(v => v).FirstOrDefault(c => c.Id == commandId);
            return Task.FromResult(command);
        }

        public async Task<CommandRecord> GetNextAsync(uint instanceId, CancellationToken cancellationToken)
        {
            if (!_commands.ContainsKey(instanceId))
                return null;

            return _commands[instanceId].FirstOrDefault(r => r.Status == CommandStatus.New);
        }

        public Task AddAsync(CommandRecord record, CancellationToken cancellationToken)
        {
            if (!_commands.ContainsKey(record.InstanceId))
                _commands.Add(record.InstanceId, new List<CommandRecord>());

            _commands[record.InstanceId].Add(record);
            _id++;
            record.Id = _id;

            return Task.CompletedTask;
        }

        public Task UpdateAsync(CommandRecord record, CancellationToken cancellationToken)
        {
            return Task.FromResult(record);
        }

        public Task<Instance> GetInstanceAsync(string machineName, CancellationToken cancellationToken)
        {
            var inst = _instances.FirstOrDefault(i => i.MachineName.Equals(machineName, StringComparison.InvariantCultureIgnoreCase));
            if (inst == null)
            {
                var id = 1u;
                if (_instances.Any())
                    id = _instances.Select(i => i.Id).Max() + 1;
                inst = new Instance
                {
                    Id = id,
                    MachineName = machineName
                };
                _instances.Add(inst);
            }
            return Task.FromResult(inst);
        }

        public Task<IReadOnlyCollection<CommandRecord>> GetRecordsAsync(uint instanceId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken)
        {
            if (!_commands.ContainsKey(instanceId))
                return Task.FromResult<IReadOnlyCollection<CommandRecord>>(Array.Empty<CommandRecord>());

            var records = _commands[instanceId]
                .Where(c => c.CreateStamp >= startDate)
                .Where(c => c.CreateStamp <= endDate)
                .ToArray();
            return Task.FromResult<IReadOnlyCollection<CommandRecord>>(records);
        }

        public Task<IReadOnlyCollection<Instance>> GetInstancesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<Instance>>(_instances.ToArray());
        }
    }
}
