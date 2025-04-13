using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Kubernetes.Dynamic.Extentions;
using Ocelot.Provider.Kubernetes.Dynamic.Interfaces;

namespace Ocelot.Provider.Kubernetes.Dynamic.Tests.Extensions
{
    public class OcelotBuilderExtensionsTests
    {
        [Fact]
        public void AddDynamicKubernetes_ShouldRegisterRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            services.AddSingleton<IConfiguration>(configuration);

            // 🔧 Provide a mock for token cache to avoid real watcher
            var mockTokenCache = new Mock<IKubernetesTokenCache>();
            //
            mockTokenCache.Setup(m => m.GetToken()).Returns("fake-token");


            var builder = new OcelotBuilder(services, configuration);

            // Act
            builder.AddDynamicKubernetes();

            services.AddSingleton(mockTokenCache.Object);

            var provider = services.BuildServiceProvider();

            // Assert
            Assert.NotNull(provider.GetService<IKubeApiClientFactory>());
            Assert.NotNull(provider.GetService<IKubernetesTokenCache>());
        }

    }
}