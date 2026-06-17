# GameLift UDP Ping Beacons for Unity

Unity runtime module for probing AWS GameLift UDP ping beacons from inside a game.

It mirrors the Go command line tool at https://github.com/jp/gamelift-udp-ping-beacons:

- same AWS GameLift UDP ping beacon endpoint list
- same `GLPING01` UDP probe payload shape
- parallel endpoint scans
- packet loss, min, average, p50, p95, max, and jitter metrics
- result ordering by availability, packet loss, average latency, then region code

## Install

Add the package through Unity Package Manager using the Git URL:

```json
{
  "dependencies": {
    "com.aurory.gamelift-udp-ping-beacons": "https://github.com/jp/gamelift-udp-ping-beacons-unity.git"
  }
}
```

Or use Unity's Package Manager window with **Add package from git URL...** and enter:

```text
https://github.com/jp/gamelift-udp-ping-beacons-unity.git
```

## Runtime Usage

```csharp
using System;
using Aurory.GameLiftUdpPingBeacons;
using UnityEngine;

public sealed class PingExample : MonoBehaviour
{
    public async void Run()
    {
        var options = new GameLiftUdpPingBeaconOptions
        {
            Duration = TimeSpan.FromSeconds(10),
            Interval = TimeSpan.FromMilliseconds(500),
            Timeout = TimeSpan.FromSeconds(1),
            Family = GameLiftUdpPingIpFamily.Auto
        };

        var report = await GameLiftUdpPingBeacon.RunAsync(options);
        Debug.Log(report.ToJson());
    }
}
```

Import the **Basic Display** sample for a ready-to-wire `MonoBehaviour` that can be attached to a scene object with a `UnityEngine.UI.Text` and optional `Button`.
