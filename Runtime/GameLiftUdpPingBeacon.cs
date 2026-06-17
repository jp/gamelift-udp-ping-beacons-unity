using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Aurory.GameLiftUdpPingBeacons
{
    public static class GameLiftUdpPingBeacon
    {
        private const string PayloadMagic = "GLPING01";
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static readonly GameLiftUdpPingBeaconEndpoint[] DefaultEndpoints =
        {
            new GameLiftUdpPingBeaconEndpoint("North America", "US East (N. Virginia)", "us-east-1", "gamelift-ping.us-east-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("North America", "US East (Ohio)", "us-east-2", "gamelift-ping.us-east-2.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("North America", "US West (N. California)", "us-west-1", "gamelift-ping.us-west-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("North America", "US West (Oregon)", "us-west-2", "gamelift-ping.us-west-2.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("North America", "Canada (Central)", "ca-central-1", "gamelift-ping.ca-central-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US West (Los Angeles)", "us-west-2-lax-1", "gamelift-ping-lax.us-west-2.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US East (Chicago)", "us-east-1-chi-1", "gamelift-ping-chi.us-east-1.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US East (Houston)", "us-east-1-iah-1", "gamelift-ping-iah.us-east-1.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US East (Dallas)", "us-east-1-dfw-1", "gamelift-ping-dfw.us-east-1.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US West (Denver)", "us-west-2-den-1", "gamelift-ping-den.us-west-2.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US East (Atlanta)", "us-east-1-atl-1", "gamelift-ping-atl.us-east-1.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US West (Phoenix)", "us-west-2-phx-1", "gamelift-ping-phx.us-west-2.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("US Local Zones", "US East (Kansas City)", "us-east-1-mci-1", "gamelift-ping-mci.us-east-1.api.aws:7770", "UDP", false),
            new GameLiftUdpPingBeaconEndpoint("South America", "South America (Sao Paulo)", "sa-east-1", "gamelift-ping.sa-east-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Europe", "Europe (Ireland)", "eu-west-1", "gamelift-ping.eu-west-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Europe", "Europe (London)", "eu-west-2", "gamelift-ping.eu-west-2.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Europe", "Europe (Paris)", "eu-west-3", "gamelift-ping.eu-west-3.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Europe", "Europe (Frankfurt)", "eu-central-1", "gamelift-ping.eu-central-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Europe", "Europe (Milan)", "eu-south-1", "gamelift-ping.eu-south-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Europe", "Europe (Stockholm)", "eu-north-1", "gamelift-ping.eu-north-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Malaysia)", "ap-southeast-5", "gamelift-ping.ap-southeast-5.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Mumbai)", "ap-south-1", "gamelift-ping.ap-south-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Hong Kong)", "ap-east-1", "gamelift-ping.ap-east-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Thailand)", "ap-southeast-7", "gamelift-ping.ap-southeast-7.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Osaka)", "ap-northeast-3", "gamelift-ping.ap-northeast-3.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Seoul)", "ap-northeast-2", "gamelift-ping.ap-northeast-2.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Singapore)", "ap-southeast-1", "gamelift-ping.ap-southeast-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Sydney)", "ap-southeast-2", "gamelift-ping.ap-southeast-2.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Asia Pacific", "Asia Pacific (Tokyo)", "ap-northeast-1", "gamelift-ping.ap-northeast-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Middle East", "Middle East (Bahrain)", "me-south-1", "gamelift-ping.me-south-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Africa", "Africa (Cape Town)", "af-south-1", "gamelift-ping.af-south-1.api.aws:7770", "UDP", true),
            new GameLiftUdpPingBeaconEndpoint("Africa", "Africa (Lagos)", "af-south-1-los-1", "gamelift-ping-los.af-south-1.api.aws:7770", "UDP", false)
        };

        public static async Task<GameLiftUdpPingBeaconReport> RunAsync(
            GameLiftUdpPingBeaconOptions options = null,
            IProgress<GameLiftUdpPingBeaconProgress> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            options = options ?? new GameLiftUdpPingBeaconOptions();
            options.Validate();

            var endpoints = options.Endpoints;
            var results = new GameLiftUdpPingBeaconResult[endpoints.Length];
            var completed = 0;
            var tasks = new Task[endpoints.Length];

            for (var i = 0; i < endpoints.Length; i++)
            {
                var index = i;
                tasks[index] = Task.Run(async () =>
                {
                    var result = await PingEndpointAsync(endpoints[index], options, cancellationToken).ConfigureAwait(false);
                    if (!options.IncludeSamples)
                    {
                        result.SamplesMs.Clear();
                    }

                    results[index] = result;
                    var done = Interlocked.Increment(ref completed);
                    if (progress != null)
                    {
                        progress.Report(new GameLiftUdpPingBeaconProgress(done, endpoints.Length, result));
                    }
                }, CancellationToken.None);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var sorted = results
                .OrderBy(r => string.IsNullOrEmpty(r.Error) ? 0 : 1)
                .ThenBy(r => r.LossPercent)
                .ThenBy(r => r.AvgLatencyMs)
                .ThenBy(r => r.Endpoint.Code, StringComparer.Ordinal)
                .ToList();

            return new GameLiftUdpPingBeaconReport
            {
                GeneratedAtUtc = DateTime.UtcNow.ToString("o"),
                Duration = options.Duration.ToString(),
                Interval = options.Interval.ToString(),
                Timeout = options.Timeout.ToString(),
                Family = options.Family.ToString().ToLowerInvariant(),
                Platform = Environment.OSVersion.Platform.ToString(),
                Results = sorted
            };
        }

        private static async Task<GameLiftUdpPingBeaconResult> PingEndpointAsync(
            GameLiftUdpPingBeaconEndpoint endpoint,
            GameLiftUdpPingBeaconOptions options,
            CancellationToken cancellationToken)
        {
            var result = new GameLiftUdpPingBeaconResult
            {
                Endpoint = endpoint,
                Network = NetworkNameFor(endpoint, options.Family)
            };

            if (string.IsNullOrEmpty(result.Network))
            {
                result.Error = "endpoint is not marked as IPv6-capable";
                return result;
            }

            if (!TrySplitHostPort(endpoint.Address, out var host, out var port))
            {
                result.Error = "invalid endpoint address";
                return result;
            }

            IPEndPoint remoteEndPoint;
            try
            {
                remoteEndPoint = await ResolveEndpointAsync(host, port, endpoint, options.Family).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                return result;
            }

            using (var udp = new UdpClient(remoteEndPoint.AddressFamily))
            {
                try
                {
                    udp.Connect(remoteEndPoint);
                }
                catch (Exception ex)
                {
                    result.Error = ex.Message;
                    return result;
                }

                var deadline = DateTime.UtcNow + options.Duration;
                var nextSend = DateTime.UtcNow;

                for (var seq = 0; DateTime.UtcNow < deadline && !cancellationToken.IsCancellationRequested; seq++)
                {
                    var now = DateTime.UtcNow;
                    if (now < nextSend)
                    {
                        await DelayUntilAsync(nextSend, cancellationToken).ConfigureAwait(false);
                        if (DateTime.UtcNow >= deadline)
                        {
                            break;
                        }
                    }

                    result.Sent++;
                    var payload = MakePayload(seq);
                    var sentAt = DateTime.UtcNow;
                    var stopwatch = Stopwatch.StartNew();

                    try
                    {
                        await udp.SendAsync(payload, payload.Length).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex.Message;
                        break;
                    }

                    var remaining = deadline - DateTime.UtcNow;
                    var receiveTimeout = remaining < options.Timeout ? remaining : options.Timeout;
                    if (receiveTimeout <= TimeSpan.Zero)
                    {
                        break;
                    }

                    try
                    {
                        var received = ReceiveWithTimeout(udp, receiveTimeout);
                        if (received)
                        {
                            stopwatch.Stop();
                            result.Received++;
                            result.SamplesMs.Add(stopwatch.Elapsed.TotalMilliseconds);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Error = ex.Message;
                        break;
                    }

                    nextSend = sentAt + options.Interval;
                }

                FinalizeResult(result);
                return result;
            }
        }

        private static async Task<IPEndPoint> ResolveEndpointAsync(
            string host,
            int port,
            GameLiftUdpPingBeaconEndpoint endpoint,
            GameLiftUdpPingIpFamily family)
        {
            if (family == GameLiftUdpPingIpFamily.IPv6 && !endpoint.IPv6Support)
            {
                throw new InvalidOperationException("endpoint is not marked as IPv6-capable");
            }

            var addresses = await Dns.GetHostAddressesAsync(host).ConfigureAwait(false);
            IEnumerable<IPAddress> candidates = addresses;
            if (family == GameLiftUdpPingIpFamily.IPv4)
            {
                candidates = candidates.Where(address => address.AddressFamily == AddressFamily.InterNetwork);
            }
            else if (family == GameLiftUdpPingIpFamily.IPv6)
            {
                candidates = candidates.Where(address => address.AddressFamily == AddressFamily.InterNetworkV6);
            }
            else if (!endpoint.IPv6Support)
            {
                candidates = candidates.Where(address => address.AddressFamily == AddressFamily.InterNetwork);
            }

            var selected = candidates.FirstOrDefault();
            if (selected == null)
            {
                throw new SocketException((int)SocketError.HostNotFound);
            }

            return new IPEndPoint(selected, port);
        }

        private static bool ReceiveWithTimeout(UdpClient udp, TimeSpan timeout)
        {
            udp.Client.ReceiveTimeout = Math.Max(1, (int)Math.Ceiling(timeout.TotalMilliseconds));
            var anyAddress = udp.Client.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any;
            var sender = new IPEndPoint(anyAddress, 0);
            try
            {
                var buffer = udp.Receive(ref sender);
                return buffer != null && buffer.Length > 0;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut || ex.SocketErrorCode == SocketError.WouldBlock)
            {
                return false;
            }
        }

        private static string NetworkNameFor(GameLiftUdpPingBeaconEndpoint endpoint, GameLiftUdpPingIpFamily family)
        {
            switch (family)
            {
                case GameLiftUdpPingIpFamily.IPv4:
                    return "udp4";
                case GameLiftUdpPingIpFamily.IPv6:
                    return endpoint.IPv6Support ? "udp6" : string.Empty;
                default:
                    return endpoint.IPv6Support ? "udp" : "udp4";
            }
        }

        private static byte[] MakePayload(int seq)
        {
            var payload = new byte[PayloadMagic.Length + 16];
            for (var i = 0; i < PayloadMagic.Length; i++)
            {
                payload[i] = (byte)PayloadMagic[i];
            }

            WriteUInt64BigEndian(payload, PayloadMagic.Length, (ulong)seq);
            WriteUInt64BigEndian(payload, PayloadMagic.Length + 8, UnixTimeNanoseconds());
            return payload;
        }

        private static void WriteUInt64BigEndian(byte[] buffer, int offset, ulong value)
        {
            for (var i = 7; i >= 0; i--)
            {
                buffer[offset + i] = (byte)(value & 0xff);
                value >>= 8;
            }
        }

        private static ulong UnixTimeNanoseconds()
        {
            var ticksSinceUnixEpoch = DateTime.UtcNow.Ticks - UnixEpoch.Ticks;
            return (ulong)ticksSinceUnixEpoch * 100UL;
        }

        private static async Task DelayUntilAsync(DateTime targetUtc, CancellationToken cancellationToken)
        {
            var delay = targetUtc - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
        }

        private static void FinalizeResult(GameLiftUdpPingBeaconResult result)
        {
            result.Lost = result.Sent - result.Received;
            if (result.Sent > 0)
            {
                result.LossPercent = (double)result.Lost / result.Sent * 100.0;
            }

            if (result.SamplesMs.Count == 0)
            {
                return;
            }

            var sorted = result.SamplesMs.OrderBy(sample => sample).ToList();
            result.MinLatencyMs = sorted[0];
            result.MaxLatencyMs = sorted[sorted.Count - 1];
            result.AvgLatencyMs = sorted.Average();
            result.P50LatencyMs = Percentile(sorted, 50);
            result.P95LatencyMs = Percentile(sorted, 95);
            result.JitterMs = MeanAbsoluteDelta(result.SamplesMs);
        }

        private static double Percentile(IReadOnlyList<double> sorted, double percentile)
        {
            if (sorted.Count == 0)
            {
                return 0;
            }

            if (sorted.Count == 1)
            {
                return sorted[0];
            }

            var rank = percentile / 100.0 * (sorted.Count - 1);
            var low = (int)Math.Floor(rank);
            var high = (int)Math.Ceiling(rank);
            if (low == high)
            {
                return sorted[low];
            }

            var weight = rank - low;
            return sorted[low] * (1.0 - weight) + sorted[high] * weight;
        }

        private static double MeanAbsoluteDelta(IReadOnlyList<double> samples)
        {
            if (samples.Count < 2)
            {
                return 0;
            }

            var total = 0.0;
            for (var i = 1; i < samples.Count; i++)
            {
                total += Math.Abs(samples[i] - samples[i - 1]);
            }

            return total / (samples.Count - 1);
        }

        private static bool TrySplitHostPort(string address, out string host, out int port)
        {
            host = null;
            port = 0;
            var separator = address.LastIndexOf(':');
            if (separator <= 0 || separator == address.Length - 1)
            {
                return false;
            }

            host = address.Substring(0, separator);
            return int.TryParse(address.Substring(separator + 1), out port);
        }
    }
}
