using System;
using System.Linq;
using System.Text;
using System.Threading;
using Aurory.GameLiftUdpPingBeacons;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameLiftUdpPingBeaconDisplay : MonoBehaviour
{
    [SerializeField] private Text output;
    [SerializeField] private Button runButton;
    [SerializeField] private int durationSeconds = 10;
    [SerializeField] private int intervalMilliseconds = 500;
    [SerializeField] private int timeoutMilliseconds = 1000;
    [SerializeField] private GameLiftUdpPingIpFamily ipFamily = GameLiftUdpPingIpFamily.Auto;
    [SerializeField] private int maxDisplayedResults = 12;

    private CancellationTokenSource cancellationTokenSource;

    private void Awake()
    {
        if (runButton != null)
        {
            runButton.onClick.AddListener(Run);
        }
    }

    private void OnDestroy()
    {
        if (runButton != null)
        {
            runButton.onClick.RemoveListener(Run);
        }

        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
    }

    public async void Run()
    {
        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();

        if (runButton != null)
        {
            runButton.interactable = false;
        }

        SetOutput("Scanning GameLift UDP ping beacons...");

        var options = new GameLiftUdpPingBeaconOptions
        {
            Duration = TimeSpan.FromSeconds(durationSeconds),
            Interval = TimeSpan.FromMilliseconds(intervalMilliseconds),
            Timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds),
            Family = ipFamily
        };

        try
        {
            var progress = new Progress<GameLiftUdpPingBeaconProgress>(OnProgress);
            var report = await GameLiftUdpPingBeacon.RunAsync(options, progress, cancellationTokenSource.Token);
            SetOutput(BuildReportText(report));
        }
        catch (OperationCanceledException)
        {
            SetOutput("Scan cancelled.");
        }
        catch (Exception ex)
        {
            SetOutput("Scan failed: " + ex.Message);
        }
        finally
        {
            if (runButton != null)
            {
                runButton.interactable = true;
            }
        }
    }

    public void Cancel()
    {
        cancellationTokenSource?.Cancel();
    }

    private void OnProgress(GameLiftUdpPingBeaconProgress progress)
    {
        var last = progress.Result == null
            ? string.Empty
            : "\nLast: " + progress.Result.Endpoint.Code + " " + progress.Result.Status;

        SetOutput($"Scanning... {progress.Completed}/{progress.Total} ({progress.Percent:P0}){last}");
    }

    private string BuildReportText(GameLiftUdpPingBeaconReport report)
    {
        var ok = report.Results.Count(result => string.IsNullOrEmpty(result.Error) && result.Received > 0 && result.LossPercent <= 0);
        var degraded = report.Results.Count(result => string.IsNullOrEmpty(result.Error) && result.Received > 0 && result.LossPercent > 0);
        var down = report.Results.Count(result => !string.IsNullOrEmpty(result.Error) || result.Received == 0);

        var builder = new StringBuilder();
        builder.AppendLine("GameLift UDP ping beacon report");
        builder.AppendLine($"Healthy {ok}  Degraded {degraded}  Unavailable {down}");
        builder.AppendLine();
        builder.AppendLine("Code             Loss    Avg      P50      P95      Jitter   Status");

        foreach (var result in report.Results.Take(Math.Max(1, maxDisplayedResults)))
        {
            builder.Append(result.Endpoint.Code.PadRight(16));
            builder.Append(FormatPercent(result.LossPercent).PadLeft(7));
            builder.Append(FormatMs(result.AvgLatencyMs).PadLeft(9));
            builder.Append(FormatMs(result.P50LatencyMs).PadLeft(9));
            builder.Append(FormatMs(result.P95LatencyMs).PadLeft(9));
            builder.Append(FormatMs(result.JitterMs).PadLeft(9));
            builder.Append("  ");
            builder.AppendLine(result.Status);
        }

        return builder.ToString();
    }

    private static string FormatPercent(double percent)
    {
        return percent.ToString("0.0") + "%";
    }

    private static string FormatMs(double milliseconds)
    {
        return milliseconds <= 0 ? "-" : milliseconds.ToString("0.0") + "ms";
    }

    private void SetOutput(string text)
    {
        if (output != null)
        {
            output.text = text;
        }
        else
        {
            Debug.Log(text);
        }
    }
}
