namespace Lagrange.Core.Internal.Events.System;

internal class SetFriendRequestAcceptEventReq(string targetUid) : ProtocolEvent
{
    public string TargetUid { get; } = targetUid;
}

internal class SetFriendRequestAcceptEventResp : ProtocolEvent
{
    public static readonly SetFriendRequestAcceptEventResp Default = new();
}