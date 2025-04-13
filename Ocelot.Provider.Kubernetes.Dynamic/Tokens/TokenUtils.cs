using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ocelot.Provider.Kubernetes.Dynamic.Models;

namespace Ocelot.Provider.Kubernetes.Dynamic.Tokens
{
    public static class TokenUtils
    {
        private static readonly object _certLock = new();
        private static X509Certificate2? _cachedCert;
        private static DateTime _lastModified;
        private const string _caCertPath = "/var/run/secrets/kubernetes.io/serviceaccount/ca.crt";
        private static ILogger? _logger;

        public static void InitializeLogger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("ApiGateway.Kubernetes.TokenUtils");
        }

        public static KubernetesTokenInfo? ParseToken(string token)
        {
            try
            {
                var segments = token.Split('.');
                if (segments.Length != 3)
                    return null;

                var payloadSegment = segments[1];
                var payloadBytes = Base64UrlDecode(payloadSegment);
                var json = Encoding.UTF8.GetString(payloadBytes);

                return JsonSerializer.Deserialize<KubernetesTokenInfo>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to decode token: {ex.Message}");
                return null;
            }
        }

        private static byte[] Base64UrlDecode(string input)
        {
            input = input.Replace('-', '+').Replace('_', '/');
            switch (input.Length % 4)
            {
                case 2: input += "=="; break;
                case 3: input += "="; break;
                case 0: break;
                default: throw new FormatException("Invalid base64url string");
            }
            return Convert.FromBase64String(input);
        }

        public static X509Certificate2? LoadCaCertificate()
        {
            if (!File.Exists(_caCertPath))
            {
                _logger?.LogWarning("CA certificate file not found at {Path}.", _caCertPath);
                return null;
            }

            lock (_certLock)
            {
                var fileInfo = new FileInfo(_caCertPath);
                var lastWrite = fileInfo.LastWriteTimeUtc;

                if (_cachedCert == null || lastWrite > _lastModified)
                {
                    try
                    {
                        _cachedCert?.Dispose();
                        _cachedCert = new X509Certificate2(File.ReadAllBytes(_caCertPath));
                        _lastModified = lastWrite;
                        _logger?.LogInformation("Loaded CA certificate from {Path} (updated: {Time})", _caCertPath, lastWrite);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Failed to load CA certificate from {Path}.", _caCertPath);
                        return null;
                    }
                }
                else
                {
                    _logger?.LogDebug("Using cached CA certificate from {Path} (unchanged)", _caCertPath);
                }

                return _cachedCert;
            }
        }
    }
}
