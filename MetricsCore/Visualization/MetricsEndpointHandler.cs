using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Metrics.Visualization
{
    public sealed class MetricsEndpointHandler
    {
        private readonly MetricsEndpoint[] endpoints;

        public MetricsEndpointHandler(IEnumerable<MetricsEndpoint> endpoints)
        {
            this.endpoints = endpoints.ToArray();
        }

        public MetricsEndpointResponse Process(string urlPath, HttpListenerContext context)
        {
            foreach (var endpoint in this.endpoints)
            {
                if (endpoint.IsMatch(urlPath))
                {
                    return endpoint.ProduceResponse(context);
                }
            }

            return null;
        }
    }
}
