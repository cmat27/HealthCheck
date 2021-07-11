using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheck
{
    public class ICPMHealthCheck :  IHealthCheck
    {
        private readonly string Host;
        private readonly int HealthyRoundtripTime;
        public ICPMHealthCheck(string host, int healthyRoundtripTime)
        {
            Host = host;
            HealthyRoundtripTime = healthyRoundtripTime;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(Host);
                switch (reply.Status)
                {
                    case IPStatus.Success:
                        var msg = $"ICPM to {Host} took {reply.RoundtripTime} ms.";
                            return (reply.RoundtripTime > HealthyRoundtripTime ? HealthCheckResult.Degraded(msg) : HealthCheckResult.Healthy(msg));
                    default:
                        var err = $"ICPM to {Host} failed: {reply.Status}";
                        return HealthCheckResult.Unhealthy(err);
                }

            }
            catch (System.Exception ex)
            {
                var err = $"ICPM to {Host} failed: {ex.Message}";
                return HealthCheckResult.Unhealthy(err);
            }
        }
    }
}
