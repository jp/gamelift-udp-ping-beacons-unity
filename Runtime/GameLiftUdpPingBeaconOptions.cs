using System;

namespace Aurory.GameLiftUdpPingBeacons
{
    public enum GameLiftUdpPingIpFamily
    {
        Auto,
        IPv4,
        IPv6
    }

    public sealed class GameLiftUdpPingBeaconOptions
    {
        public TimeSpan Duration = TimeSpan.FromSeconds(10);
        public TimeSpan Interval = TimeSpan.FromMilliseconds(500);
        public TimeSpan Timeout = TimeSpan.FromSeconds(1);
        public GameLiftUdpPingIpFamily Family = GameLiftUdpPingIpFamily.Auto;
        public bool IncludeSamples;
        public GameLiftUdpPingBeaconEndpoint[] Endpoints = GameLiftUdpPingBeacon.DefaultEndpoints;

        public void Validate()
        {
            if (Duration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(Duration), "Duration must be greater than zero.");
            }

            if (Interval <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(Interval), "Interval must be greater than zero.");
            }

            if (Timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(Timeout), "Timeout must be greater than zero.");
            }

            if (Endpoints == null)
            {
                throw new ArgumentNullException(nameof(Endpoints));
            }
        }
    }
}
