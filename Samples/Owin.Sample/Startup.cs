using Metrics;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin.Metrics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

            var httpConfig = new HttpConfiguration();
            httpConfig.MapHttpAttributeRoutes();
            apiExplorer = httpConfig.Services.GetApiExplorer();
            httpConfig.EnsureInitialized();

            Metric.Config
                .WithAllCounters()
                .WithReporting(r => r.WithConsoleReport(TimeSpan.FromSeconds(30)))
                .WithOwin(middleware => app.Use(middleware), config => config
                    .WithRequestMetricsConfig(c => c.RegisterAllMetrics())
                    .WithMetricsEndpoint()
                , MetricNameResolver);



            app.UseWebApi(httpConfig);


        }

        public string MetricNameResolver(IDictionary<string, object> environment)
        {
            var request = new OwinRequest(environment);

            var description = apiExplorer.ApiDescriptions
                .FirstOrDefault(x =>
                {
                    var path = request.Uri.AbsolutePath.ToString(CultureInfo.InvariantCulture).TrimStart(new[] { '/' });
                    var routeTemplateSectionCount = x.Route.RouteTemplate.Split(new[] { '/' }).Count();
                    var pathSections = path.Split(new[] { '/' });
                    var pathSectionCount = pathSections.Count();
                    var actualPathWithoutRouteParams = pathSections
                        .Take(pathSectionCount - x.ParameterDescriptions.Count)
                        .Aggregate(string.Empty, (current, section) => current + ("/" + section)).TrimStart(new[] { '/' });

                    return x.HttpMethod.Method == request.Method
                        && x.Route.RouteTemplate.StartsWith(actualPathWithoutRouteParams)
                        && routeTemplateSectionCount == pathSectionCount;
                });

            if (description == null) return request.Method + " Unknown";

            return request.Method + " " + description.Route.RouteTemplate;
        }
    }
}
