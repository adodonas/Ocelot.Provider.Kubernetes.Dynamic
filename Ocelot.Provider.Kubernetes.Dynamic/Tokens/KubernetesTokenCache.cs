using Microsoft.Extensions.Logging;
using Ocelot.Provider.Kubernetes.Dynamic.Interfaces;

namespace Ocelot.Provider.Kubernetes.Dynamic.Tokens
{
    public class KubernetesTokenCache : IKubernetesTokenCache
    {
        private string _cachedToken = string.Empty;
        private readonly string _tokenPath;
        private readonly FileSystemWatcher _watcher;
        private readonly object _lock = new();
        private readonly ILogger<KubernetesTokenCache> _logger;

        public event Action? TokenChanged;

        public KubernetesTokenCache(ILogger<KubernetesTokenCache> logger, string tokenPath = "/var/run/secrets/kubernetes.io/serviceaccount/token")
        {
            _logger = logger;
            _tokenPath = tokenPath;

            LoadToken();

            _watcher = new FileSystemWatcher(Path.GetDirectoryName(_tokenPath)!)
            {
                Filter = Path.GetFileName(_tokenPath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _watcher.Changed += (_, __) =>
            {
                _logger.LogInformation("Detected token file change. Attempting reload.");
                LoadToken();
            };

            _watcher.EnableRaisingEvents = true;
        }

        private void LoadToken()
        {
            lock (_lock)
            {
                try
                {
                    var newToken = File.ReadAllText(_tokenPath).Trim();
                    if (_cachedToken != newToken)
                    {
                        _logger.LogInformation("Kubernetes token reloaded from disk.");
                        _cachedToken = newToken;
                        TokenChanged?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to read token from path {Path}", _tokenPath);
                }
            }
        }

        public string GetToken()
        {
            lock (_lock)
            {
                return _cachedToken;
            }
        }
    }
}