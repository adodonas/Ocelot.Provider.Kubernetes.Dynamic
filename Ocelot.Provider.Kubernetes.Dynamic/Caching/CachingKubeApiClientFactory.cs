using KubeClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.Provider.Kubernetes.Dynamic.Interfaces;
using Ocelot.Provider.Kubernetes.Dynamic.Models;
using Ocelot.Provider.Kubernetes.Dynamic.Tokens;

namespace Ocelot.Provider.Kubernetes.Dynamic.Caching
{
    public class CachingKubeApiClientFactory : IKubeApiClientFactory
    {
        private readonly ILogger<CachingKubeApiClientFactory> _logger;
        private readonly IKubernetesTokenCache _tokenCache;
        private readonly IOptions<KubeClientOptions> _options;
        private readonly object _lock = new();
        private KubeApiClient? _cachedClient;

        public CachingKubeApiClientFactory(
            ILogger<CachingKubeApiClientFactory> logger,
            IKubernetesTokenCache tokenCache,
            IOptions<KubeClientOptions> options)
        {
            _logger = logger;
            _tokenCache = tokenCache;
            _options = options;

            _tokenCache.TokenChanged += OnTokenChanged;
        }

        private void OnTokenChanged()
        {
            lock (_lock)
            {
                _logger.LogInformation("Token changed. Invalidating cached KubeApiClient.");
                _cachedClient = null;
            }
        }

        public KubeApiClient Get()
        {
            lock (_lock)
            {
                if (_cachedClient != null)
                    return _cachedClient;

                var token = _tokenCache.GetToken();
                var info = TokenUtils.ParseToken(token);
                var kubeNamespace = info?.Kubernetes?.Namespace ?? _options.Value.KubeNamespace;

                var options = new KubeClientOptions
                {
                    ApiEndPoint = GetAudience(info),
                    KubeNamespace = kubeNamespace,
                    CertificationAuthorityCertificate = TokenUtils.LoadCaCertificate(),
                    AllowInsecure = _options.Value.AllowInsecure,
                    LoggerFactory = _options.Value.LoggerFactory,
                    AuthStrategy = KubeAuthStrategy.BearerToken,
                    AccessToken = token
                };

                _logger.LogInformation("Creating new KubeApiClient with updated token.");
                _cachedClient = KubeApiClient.Create(options);
                return _cachedClient;
            }
        }

        private Uri GetAudience(KubernetesTokenInfo? info)
        {
            var audience = info?.Audiences?
                .Select(a => a.Trim('"'))
                .FirstOrDefault(a => Uri.IsWellFormedUriString(a, UriKind.Absolute));

            if (audience == null)
            {
                _logger.LogWarning("No valid audience found. Falling back to configured ApiEndPoint.");
                audience = _options.Value.ApiEndPoint?.ToString() ?? "https://kubernetes.default.svc";
            }
            return new Uri(audience);
        }
    }
}