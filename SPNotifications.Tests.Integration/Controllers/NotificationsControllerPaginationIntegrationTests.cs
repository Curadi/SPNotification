using FluentAssertions;
using SPNotifications.Tests.Integration.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace SPNotifications.Tests.Integration.Controllers
{
    public class NotificationsControllerPaginationIntegrationTests
    : IntegrationTestBase,
      IClassFixture<CustomWebApplicationFactory>
    {
        public NotificationsControllerPaginationIntegrationTests(
            CustomWebApplicationFactory factory
        ) : base(factory) { }

        [Fact]
        public async Task GET_ShouldRespectPagination()
        {
            // Arrange — cria 15 notificações
            for (int i = 1; i <= 15; i++)
            {
                await Client.PostAsJsonAsync("/api/notifications", new
                {
                    user = "Sistema",
                    message = $"Msg {i}",
                    type = "info"
                });
            }

            // Act
            var response = await Client.GetAsync("/api/notifications?page=2&pageSize=5");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content
                .ReadFromJsonAsync<PagedResultResponse>();

            result!.Items.Should().HaveCount(5);
            result.TotalCount.Should().Be(15);
        }

        [Fact]
        public async Task GET_ShouldFilterByRead()
        {
            // Arrange — cria notificação
            await Client.PostAsJsonAsync("/api/notifications", new
            {
                user = "Sistema",
                message = "Nao lida",
                type = "info"
            });

            // Busca todas
            var allResponse = await Client.GetAsync("/api/notifications");
            var allResult = await allResponse.Content
                .ReadFromJsonAsync<PagedResultResponse>();

            var firstItem = allResult!.Items.First();
            var firstId = ((JsonElement)firstItem).GetProperty("id").GetGuid();

            // Marca como lida
            await Client.PutAsync($"/api/notifications/{firstId}/read", null);

            // Act
            var response = await Client.GetAsync("/api/notifications?read=true");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content
                .ReadFromJsonAsync<PagedResultResponse>();

            result!.Items.Should().HaveCount(1);
        }


        [Fact]
        public async Task GET_ShouldFilterByType()
        {
            // Arrange
            await Client.PostAsJsonAsync("/api/notifications", new
            {
                user = "Sistema",
                message = "Info",
                type = "info"
            });

            await Client.PostAsJsonAsync("/api/notifications", new
            {
                user = "Sistema",
                message = "Warning",
                type = "warning"
            });

            // Act
            var response = await Client.GetAsync("/api/notifications?type=warning");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Warning");
            content.Should().NotContain("Info");
        }

        [Fact]
        public async Task GET_ShouldApplyReadAndTypeFilters()
        {
            await Client.PostAsJsonAsync("/api/notifications", new
            {
                user = "Sistema",
                message = "Info",
                type = "info"
            });

            await Client.PostAsJsonAsync("/api/notifications", new
            {
                user = "Sistema",
                message = "Warning",
                type = "warning"
            });

            var response = await Client.GetAsync("/api/notifications?read=false&type=warning");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Warning");
            content.Should().NotContain("Info");
        }



    }

    // DTO LOCAL PARA TESTE
    public class PagedResultResponse
    {
        public List<object> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }


}
