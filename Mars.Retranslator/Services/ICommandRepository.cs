using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mars.Retranslator.DataModels;
using Microsoft.RuntimeBroker.Models;

namespace Mars.Retranslator.Services
{
    public interface ICommandReadonlyRepository
    {
        Task<CommandRecord> GetAsync(uint commandId, CancellationToken cancellationToken);
        
        Task<Instance> GetInstanceAsync(string machineName, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<CommandRecord>> GetRecordsAsync(uint instanceId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken);
        
        Task<IReadOnlyCollection<Instance>> GetInstancesAsync(CancellationToken cancellationToken);
    }

    public interface ICommandRepository: ICommandReadonlyRepository
    {
        Task AddAsync(CommandRecord record, CancellationToken cancellationToken);

        Task UpdateAsync(CommandRecord record, CancellationToken cancellationToken);
    }
}
