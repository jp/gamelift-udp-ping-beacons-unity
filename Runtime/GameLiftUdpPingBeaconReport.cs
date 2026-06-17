using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aurory.GameLiftUdpPingBeacons
{
    [Serializable]
    public sealed class GameLiftUdpPingBeaconResult
    {
        public GameLiftUdpPingBeaconEndpoint Endpoint;
        public string Network;
        public int Sent;
        public int Received;
        public int Lost;
        public double LossPercent;
        public double MinLatencyMs;
        public double AvgLatencyMs;
        public double MaxLatencyMs;
        public double P50LatencyMs;
        public double P95LatencyMs;
        public double JitterMs;
        public List<double> SamplesMs = new List<double>();
        public string Error;

        public string Status
        {
            get
            {
                if (!string.IsNullOrEmpty(Error))
                {
                    return Error;
                }

                if (Received == 0)
                {
                    return "no replies";
                }

                return LossPercent > 0 ? "packet loss" : "ok";
            }
        }
    }

    [Serializable]
    public sealed class GameLiftUdpPingBeaconReport
    {
        public string GeneratedAtUtc;
        public string Duration;
        public string Interval;
        public string Timeout;
        public string Family;
        public string Platform;
        public List<GameLiftUdpPingBeaconResult> Results = new List<GameLiftUdpPingBeaconResult>();

        public string ToJson(bool prettyPrint = true)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }
    }

    public readonly struct GameLiftUdpPingBeaconProgress
    {
        public readonly int Completed;
        public readonly int Total;
        public readonly GameLiftUdpPingBeaconResult Result;

        public float Percent => Total <= 0 ? 1f : (float)Completed / Total;

        public GameLiftUdpPingBeaconProgress(int completed, int total, GameLiftUdpPingBeaconResult result)
        {
            Completed = completed;
            Total = total;
            Result = result;
        }
    }
}
