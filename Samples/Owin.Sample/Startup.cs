using Metrics;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Owin.Sample
{
    public class Startup
    {
        private static IApiExplorer apiExplorer;

        public void Configuration(IAppBuilder app)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            app.UseCors(CorsOptions.AllowAll);

            Metric.Config
                .WithAllCounters()
                .WithReporting(r => r.WithConsoleReport(TimeSpan.FromSeconds(30)))
                .WithOwin(middleware => app.Use(middleware), config => config
                    .WithRequestMetricsConfig(c => c.RegisterAllMetrics())
                    .WithMetricsEndpoint()
                , MetricNameResolver);

            var httpConfig = new HttpConfiguration();
            httpConfig.MapHttpAttributeRoutes();

            apiExplorer = httpConfig.Services.GetApiExplorer();

            app.UseWebApi(httpConfig);
        }

        public string MetricNameResolver(IDictionary<string, object> environment)
        {
            var getActions = apiExplorer.ApiDescriptions.Where(x => x.HttpMethod == HttpMethod.Get).FirstOrDefault();

            return environment["owin.RequestPath"].ToString().ToUpper();
        }
    }
}
