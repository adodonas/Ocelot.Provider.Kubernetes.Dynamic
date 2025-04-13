using KubeClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Ocelot.Provider.Kubernetes.Dynamic.Caching;
using Ocelot.Provider.Kubernetes.Dynamic.Interfaces;

namespace Ocelot.Provider.Kubernetes.Dynamic.Tests.Caching
{
    public class CachingKubeApiClientFactoryTests
    {
        [Fact]
        public void Get_ShouldReturnCachedClientUntilTokenChanges()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
                        "eyJhdWQiOlsiaHR0cHM6Ly9rdWJlcm5ldGVzLmRlZmF1bHQuc3ZjIl0sImt1YmVybmV0ZXMuaW8iOnsi" +
                        "bmFtZXNwYWNlIjoiZGVmYXVsdCJ9fQ." +
                        "signature";

            var tokenCache = new Mock<IKubernetesTokenCache>();
            tokenCache.Setup(t => t.GetToken()).Returns(token);

            var options = Options.Create(new KubeClientOptions
            {
                ApiEndPoint = new Uri("https://kubernetes.default.svc"),
                KubeNamespace = "default",
                AllowInsecure = false,
                LoggerFactory = NullLoggerFactory.Instance
            });

            var factory = new CachingKubeApiClientFactory(
                NullLogger<CachingKubeApiClientFactory>.Instance,
                tokenCache.Object,
                options);

            // Act
            var client1 = factory.Get();
            var client2 = factory.Get();

            // Assert
            Assert.Same(client1, client2);
        }

        [Fact]
        public void OnTokenChanged_ShouldInvalidateClient()
        {
            // Arrange
            var token1 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
                         "eyJhdWQiOlsiaHR0cHM6Ly9rdWJlcm5ldGVzLmRlZmF1bHQuc3ZjIl0sImt1YmVybmV0ZXMuaW8iOnsi" +
                         "bmFtZXNwYWNlIjoiZGVmYXVsdCJ9fQ." +
                         "sig1";
            var token2 = token1.Replace("sig1", "sig2");

            var tokenCache = new Mock<IKubernetesTokenCache>();
            tokenCache.SetupSequence(t => t.GetToken())
                      .Returns(token1)
                      .Returns(token2);

            var options = Options.Create(new KubeClientOptions
            {
                ApiEndPoint = new Uri("https://kubernetes.default.svc"),
                KubeNamespace = "default",
                AllowInsecure = false,
                LoggerFactory = NullLoggerFactory.Instance
            });

            var factory = new CachingKubeApiClientFactory(
                NullLogger<CachingKubeApiClientFactory>.Instance,
                tokenCache.Object,
                options);

            var client1 = factory.Get();
            factory.Get(); // cache

            // Act: trigger token change manually
            tokenCache.Raise(t => t.TokenChanged += null);
            var client2 = factory.Get();

            // Assert client was invalidated and rebuilt
            Assert.NotSame(client1, client2);
        }
    }
}