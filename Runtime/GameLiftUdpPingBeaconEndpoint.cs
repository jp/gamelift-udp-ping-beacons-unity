using System;

namespace Aurory.GameLiftUdpPingBeacons
{
    [Serializable]
    public sealed class GameLiftUdpPingBeaconEndpoint
    {
        public string Continent;
        public string Name;
        public string Code;
        public string Address;
        public string Protocol;
        public bool IPv6Support;

        public GameLiftUdpPingBeaconEndpoint(string continent, string name, string code, string address, string protocol, bool ipv6Support)
        {
            Continent = continent;
            Name = name;
            Code = code;
            Address = address;
            Protocol = protocol;
            IPv6Support = ipv6Support;
        }
    }
}
