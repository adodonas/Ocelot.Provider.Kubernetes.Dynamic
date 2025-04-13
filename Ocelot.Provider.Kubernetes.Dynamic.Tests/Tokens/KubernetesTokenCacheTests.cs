using Microsoft.Extensions.Logging.Abstractions;
using Ocelot.Provider.Kubernetes.Dynamic.Tokens;

namespace Ocelot.Provider.Kubernetes.Dynamic.Tests.Tokens
{
    public class KubernetesTokenCacheTests
    {
        [Fact]
        public void GetToken_ShouldReturnInitialToken()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "test-token");
            var logger = NullLogger<KubernetesTokenCache>.Instance;
            var cache = new KubernetesTokenCache(logger, tempFile);

            // Act
            var token = cache.GetToken();

            // Assert
            Assert.Equal("test-token", token);

            // Cleanup
            File.Delete(tempFile);
        }

        [Fact]
        public void TokenChanged_ShouldTriggerOnFileUpdate()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "initial-token");
            var logger = NullLogger<KubernetesTokenCache>.Instance;
            var cache = new KubernetesTokenCache(logger, tempFile);

            bool eventTriggered = false;
            cache.TokenChanged += () => eventTriggered = true;

            // Act
            File.WriteAllText(tempFile, "updated-token");
            Thread.Sleep(1000);

            // Assert
            Assert.True(eventTriggered);
            Assert.Equal("updated-token", cache.GetToken());

            // Cleanup
            File.Delete(tempFile);
        }
    }
}