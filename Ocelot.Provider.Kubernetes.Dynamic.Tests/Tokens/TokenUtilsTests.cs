using Ocelot.Provider.Kubernetes.Dynamic.Tokens;

namespace Ocelot.Provider.Kubernetes.Dynamic.Tests.Tokens
{
    public class TokenUtilsTests
    {
        [Fact]
        public void ParseToken_ShouldReturnInfo_WhenValidToken()
        {
            const string token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
                "eyJhdWQiOlsiaHR0cHM6Ly9rdWJlcm5ldGVzLmRlZmF1bHQuc3ZjIl0sImt1YmVybmV0ZXMuaW8iOnsi" +
                "bmFtZXNwYWNlIjoiZGVmYXVsdCIsInBvZCI6eyJuYW1lIjoiYXBpIiwi" +
                "dWlkIjoiYXBpLXVpZCJ9LCJzZXJ2aWNlYWNjb3VudCI6eyJuYW1lIjoiZGVmYXVsdC1zYSIsInVpZCI6ImRzYS11aWQifX19." +
                "dGVzdHNpZ25hdHVyZQ";

            var result = TokenUtils.ParseToken(token);

            Assert.NotNull(result);
            Assert.NotNull(result!.Audiences);
            Assert.Contains("https://kubernetes.default.svc", result.Audiences);
            Assert.Equal("default", result.Kubernetes?.Namespace);
        }
    }
}