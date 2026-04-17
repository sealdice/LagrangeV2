using Lagrange.Core.Internal.Context;

namespace Lagrange.Core.Test;

[Parallelizable]
public class SocketContextTest
{
    [Test]
    public void ApplyServerOrder_ShouldKeepUnresolvedServersInOrder()
    {
        string[] servers = ["server-a", "server-b", "server-c"];

        SocketContext.ApplyServerOrder(servers,
            [(20, "server-c"), (10, "server-a")],
            ["server-b"]);

        Assert.That(servers, Is.EqualTo(new[] { "server-a", "server-c", "server-b" }));
    }

    [Test]
    public void ApplyServerOrder_ShouldLeaveOriginalServersWhenNothingResponds()
    {
        string[] servers = ["server-a", "server-b"];

        SocketContext.ApplyServerOrder(servers, [], ["server-a", "server-b"]);

        Assert.That(servers, Is.EqualTo(new[] { "server-a", "server-b" }));
    }
}
