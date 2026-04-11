using System.Text.Json.Serialization;
using Lagrange.Core;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface;

namespace Lagrange.Milky.Api.Handler.Group;

[Api("accept_group_invitation")]
public class AcceptGroupInvitationRequestHandler(BotContext bot) : IEmptyResultApiHandler<AcceptGroupInvitationRequestParameter>
{
    private readonly BotContext _bot = bot;

    public async Task HandleAsync(AcceptGroupInvitationRequestParameter parameter, CancellationToken token)
    {
        await _bot.SetGroupNotification(parameter.GroupId, (ulong)parameter.InvitationSeq, BotGroupNotificationType.InviteSelf, false, GroupNotificationOperate.Allow, String.Empty);
    }
}

public class AcceptGroupInvitationRequestParameter(long groupId, long invitationSeq)
{
    [JsonRequired]
    [JsonPropertyName("group_id")]
    public long GroupId { get; init; } = groupId;

    [JsonRequired]
    [JsonPropertyName("invitation_seq")]
    public long InvitationSeq { get; init; } = invitationSeq;
}