using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<SetFriendRequestAcceptEventReq>(Protocols.All)]
[Service("OidbSvcTrpcTcp.0xb5d_44")]
internal class SetFriendRequestAcceptService: OidbService<SetFriendRequestAcceptEventReq, SetFriendRequestAcceptEventResp, DB5DReqBody, DB5DRspBody>
{
    private protected override uint Command => 0xb5d;

    private protected override uint Service => 44;

    private protected override Task<DB5DReqBody> ProcessRequest(SetFriendRequestAcceptEventReq inviteAccept, BotContext context)
    {
        return Task.FromResult(new DB5DReqBody
        {
            Accept = 3,
            TargetUid = inviteAccept.TargetUid
        });
    }

    private protected override Task<SetFriendRequestAcceptEventResp> ProcessResponse(DB5DRspBody response, BotContext context)
    {
        return Task.FromResult(SetFriendRequestAcceptEventResp.Default);
    }
}