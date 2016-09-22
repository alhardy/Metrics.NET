using FluentAssertions;
using Metrics.Visualization;
using Xunit;

namespace Metrics.Tests.Visualization
{
    public class MetricsEndpointHandlerTests
    {
        [Fact]
        public void MetricsEndpointHandler_CanProcessEndpoint()
        {
            var endpoint = new MetricsEndpoint("test", c => new MetricsEndpointResponse("text", "text/plain"));
            var handler = new MetricsEndpointHandler(new[] { endpoint });

            var response = handler.Process("test", null);
            response.Should().NotBeNull();
            response.Content.Should().Be("text");
            response.ContentType.Should().Be("text/plain");
        }

        [Fact]
        public void MetricsEndpointHandler_CannotProcessMissingEndpoint()
        {
            var endpoint = new MetricsEndpoint("test", c => new MetricsEndpointResponse("text", "text/plain"));
            var handler = new MetricsEndpointHandler(new[] { endpoint });

            var response = handler.Process("other", null);
            response.Should().BeNull();
        }
    }
}
