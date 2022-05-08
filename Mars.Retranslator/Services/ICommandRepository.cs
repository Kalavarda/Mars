using System.Threading;
using System.Threading.Tasks;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.Retranslator.Services
{
    public interface ICommandRepository
    {
        Task<CommandBase> GetAsync(uint commandId, CancellationToken cancellationToken);
    }
}
