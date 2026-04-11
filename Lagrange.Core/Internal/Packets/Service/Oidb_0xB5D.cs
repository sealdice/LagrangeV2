using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class DB5DReqBody
{
    [ProtoMember(1)] public uint Accept { get; set; }  // 3 for accept, 5 for reject
    
    [ProtoMember(2)] public string TargetUid { get; set; }
}

[ProtoPackable]
internal partial class DB5DRspBody;