using FluentAssertions;
using SPNotifications.Tests.Integration.Common;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SPNotifications.Tests.Integration.Controllers
{
    public class NotificationsControllerIntegrationTests
        : IntegrationTestBase,
          IClassFixture<CustomWebApplicationFactory>
    {
        public NotificationsControllerIntegrationTests(
            CustomWebApplicationFactory factory
        ) : base(factory)
        {
        }

        [Fact]
        public async Task PUT_MarkAsRead_WhenNotFound_ShouldReturn404()
        {
            // Act
            var response = await Client.PutAsync(
                $"/api/notifications/{Guid.NewGuid()}/read",
                null
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task POST_And_GET_ShouldWorkCorrectly()
        {
            // Arrange
            var dto = new
            {
                user = "Sistema",
                message = "Teste integração",
                type = "info"
            };

            // POST
            var postResponse = await Client.PostAsJsonAsync(
                "/api/notifications",
                dto
            );

            postResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // GET
            var getResponse = await Client.GetAsync("/api/notifications");

            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await getResponse.Content.ReadAsStringAsync();
            content.Should().Contain("Teste integração");
        }
    }
}
