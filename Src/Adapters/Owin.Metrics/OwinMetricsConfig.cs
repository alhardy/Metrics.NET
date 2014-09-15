using Metrics;
using Metrics.Core;
using Owin.Metrics.Middleware;
using System;
using System.Collections.Generic;

namespace Owin.Metrics
{
    public class OwinMetricsConfig
    {
        private readonly Action<object> middlewareRegistration;
        private readonly MetricsRegistry registry;
        private readonly Func<HealthStatus> healthStatus;
        private readonly Func<IDictionary<string, object>, string> metricNameResolver;

        public OwinMetricsConfig(Action<object> middlewareRegistration, MetricsRegistry registry, Func<HealthStatus> healthStatus,
            Func<IDictionary<string, object>, string> metricNameResolver)
        {
            this.middlewareRegistration = middlewareRegistration;
            this.registry = registry;
            this.healthStatus = healthStatus;
            this.metricNameResolver = metricNameResolver;
        }

        public OwinMetricsConfig WithRequestMetricsConfig(Action<OwinRequestMetricsConfig> config)
        {
            OwinRequestMetricsConfig requestConfig = new OwinRequestMetricsConfig(this.middlewareRegistration, this.registry, this.metricNameResolver);
            config(requestConfig);
            return this;
        }

        public OwinMetricsConfig WithMetricsEndpoint()
        {
            WithMetricsEndpoint(_ => { });
            return this;
        }

        public OwinMetricsConfig WithMetricsEndpoint(Action<OwinMetricsEndpointConfig> config)
        {
            OwinMetricsEndpointConfig endpointConfig = new OwinMetricsEndpointConfig();
            config(endpointConfig);
            var metricsEndpointMiddleware = new MetricsEndpointMiddleware(endpointConfig, this.registry, this.healthStatus);
            this.middlewareRegistration(metricsEndpointMiddleware);
            return this;
        }
    }
}
