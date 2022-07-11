using System.Threading;
using System.Threading.Tasks;
using Mars.Retranslator.DataModels;

namespace Mars.Retranslator.Services
{
    public interface ICommandRepository
    {
        Task<CommandRecord> GetAsync(uint commandId, CancellationToken cancellationToken);
        
        Task<CommandRecord> GetNextAsync(string machineName, CancellationToken cancellationToken);
        
        Task StoreAsync(CommandRecord record);
    }
}
