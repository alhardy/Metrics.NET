using Metrics;
using System;
using System.Collections.Generic;

namespace Owin.Metrics
{
    public static class OwinMetrics
    {
        public static MetricsConfig WithOwin(this MetricsConfig config, Action<object> middlewareRegistration, Action<OwinMetricsConfig> owinConfig,
            Func<IDictionary<string, object>, string> metricNameResolver)
        {
            OwinMetricsConfig owin = new OwinMetricsConfig(middlewareRegistration, config.Registry, config.HealthStatus, metricNameResolver);
            owinConfig(owin);
            return config;
        }
    }
}
