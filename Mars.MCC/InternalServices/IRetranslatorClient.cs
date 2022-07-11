using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;
using Microsoft.RuntimeBroker.Models;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Mars.MCC.InternalServices
{
    public interface IRetranslatorClient
    {
        Task<ResultDto<IReadOnlyCollection<Instance>>> GetInstancesAsync(CancellationToken cancellationToken);

        Task<ResultDto<bool>> SendCommandAsync(CommandBase command, CancellationToken cancellationToken);
    }
}
