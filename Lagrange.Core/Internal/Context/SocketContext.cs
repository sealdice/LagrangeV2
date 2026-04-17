using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Lagrange.Core.Common;
using Lagrange.Core.Internal.Network;

namespace Lagrange.Core.Internal.Context;

internal class SocketContext : IClientListener, IDisposable
{
    private const string Tag = nameof(SocketContext);
    private const int ProbeTimeout = 1000;
    
    public uint HeaderSize => 4;
    
    public bool Connected => _client.Connected;
    
    private readonly ClientListener _client;
    
    private readonly BotConfig _config;
    
    private readonly BotContext _context;
    
    public SocketContext(BotContext context)
    {
        _client = new CallbackClientListener(this);
        _config = context.Config;
        _context = context;
    }

    public uint GetPacketLength(ReadOnlySpan<byte> header) => BinaryPrimitives.ReadUInt32BigEndian(header);

    public void OnRecvPacket(ReadOnlySpan<byte> packet) => _context.PacketContext.DispatchPacket(packet);

    public void OnDisconnect()
    {
        
    }

    public void OnSocketError(Exception e, ReadOnlyMemory<byte> data)
    {
        
    }
    
    public async Task<bool> Connect()
    {
        if (_client.Connected) return true;
        
        var servers = await ResolveDns();
        if (_config.GetOptimumServer) await SortServers(servers);
        bool connected = await _client.Connect(servers[0]);
        
        if (connected) _context.LogInfo(Tag, "Connected to the server {0}", servers[0]);
        else _context.LogError(Tag, "Failed to connect to the server {0}", null,servers[0]);
        
        return connected;
    }
    
    public void Disconnect() => _client.Disconnect();
    
    public ValueTask<int> Send(ReadOnlyMemory<byte> packet) => _client.Send(packet);
    
    private async Task SortServers(string[] servers)
    {
        using var ping = new Ping();
        var responsive = new List<(long, string)>(servers.Length);
        var unresolved = new List<string>(servers.Length);
        bool pingUnavailable = false;
        
        foreach (var server in servers)
        {
            long? latency = null;

            if (!pingUnavailable)
            {
                try
                {
                    var reply = await ping.SendPingAsync(server, ProbeTimeout);
                    if (reply.Status == IPStatus.Success) latency = reply.RoundtripTime;
                }
                catch (Exception e) when (IsPingUnavailable(e))
                {
                    pingUnavailable = true;
                    _context.LogWarning(Tag,
                        "Ping probe is unavailable on this platform, falling back to TCP connect latency: {0}",
                        null,
                        e.Message);
                }
                catch (PingException)
                {
                    // Ignore and fall back to TCP probing below.
                }
            }

            latency ??= await ProbeTcpLatency(server);

            if (latency is long measuredLatency)
            {
                responsive.Add((measuredLatency, server));
                _context.LogDebug(Tag, "Server: {0} Latency: {1}ms", server, measuredLatency);
            }
            else unresolved.Add(server);
        }
        
        ApplyServerOrder(servers, responsive, unresolved);
    }

    internal static void ApplyServerOrder(string[] servers, List<(long Latency, string Server)> responsive, List<string> unresolved)
    {
        responsive.Sort((a, b) => a.Latency.CompareTo(b.Latency));

        int index = 0;
        foreach (var (_, server) in responsive) servers[index++] = server;
        foreach (var server in unresolved) servers[index++] = server;
    }

    private async Task<long?> ProbeTcpLatency(string server)
    {
        using var socket = new Socket(_config.UseIPv6Network ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        using var cts = new CancellationTokenSource(ProbeTimeout);

        try
        {
            long start = Stopwatch.GetTimestamp();
            await socket.ConnectAsync(server, GetServerPort(), cts.Token);
            return (long)Math.Ceiling(Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            return null;
        }
        catch (SocketException)
        {
            return null;
        }
    }

    private ushort GetServerPort() => (ushort)(_config.UseIPv6Network ? 14000 : 8080);

    private static bool IsPingUnavailable(Exception exception) =>
        exception is PlatformNotSupportedException ||
        exception is PingException { InnerException: PlatformNotSupportedException };
    
    private async Task<string[]> ResolveDns()
    {
        string host = _config.UseIPv6Network ? "msfwifiv6.3g.qq.com" : "msfwifi.3g.qq.com";
        var entry = await Dns.GetHostEntryAsync(host, _config.UseIPv6Network ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork);
        var result = new string[entry.AddressList.Length];

        for (int i = 0; i < entry.AddressList.Length; i++) result[i] = entry.AddressList[i].ToString();

        return result;
    }
    
    public void Dispose()
    {
        _client.Disconnect();
    }
}
