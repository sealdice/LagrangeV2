using System.Text.Json.Serialization;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;

namespace Lagrange.Milky.Api.Handler.Friend;

[Api("accept_friend_request")]
public class AcceptFriendRequestHandler(BotContext bot) : IEmptyResultApiHandler<AcceptFriendRequestHandlerParameter>
{
    private readonly BotContext _bot = bot;

    public async Task HandleAsync(AcceptFriendRequestHandlerParameter parameter, CancellationToken token)
    {
        await _bot.SetFriendRequestAccept(parameter.InitiatorUid);
    }
}

public class AcceptFriendRequestHandlerParameter(string initiatorUid, bool isFiltered)
{
    [JsonRequired]
    [JsonPropertyName("initiator_uid")]
    public string InitiatorUid { get; init; } = initiatorUid;
    
    [JsonRequired]
    [JsonPropertyName("is_filtered")]
    public bool IsFiltered { get; init; } = isFiltered;
}
