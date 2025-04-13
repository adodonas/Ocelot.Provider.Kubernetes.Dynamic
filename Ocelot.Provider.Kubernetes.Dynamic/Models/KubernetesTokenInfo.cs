using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ocelot.Provider.Kubernetes.Dynamic.Models
{
    public class KubernetesTokenInfo
    {
        [JsonPropertyName("aud")]
        public List<string>? Audiences { get; set; }

        [JsonPropertyName("exp")]
        public long Expiration { get; set; }

        [JsonPropertyName("iss")]
        public string? Issuer { get; set; }

        [JsonPropertyName("kubernetes.io")]
        public KubernetesMetadata? Kubernetes { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraClaims { get; set; }

        public class KubernetesMetadata
        {
            [JsonPropertyName("namespace")]
            public string? Namespace { get; set; }

            [JsonPropertyName("pod")]
            public PodInfo? Pod { get; set; }

            [JsonPropertyName("serviceaccount")]
            public ServiceAccountInfo? ServiceAccount { get; set; }

            [JsonExtensionData]
            public Dictionary<string, JsonElement>? ExtraFields { get; set; }

            public class PodInfo
            {
                [JsonPropertyName("name")]
                public string? Name { get; set; }

                [JsonPropertyName("uid")]
                public string? Uid { get; set; }
            }

            public class ServiceAccountInfo
            {
                [JsonPropertyName("name")]
                public string? Name { get; set; }

                [JsonPropertyName("uid")]
                public string? Uid { get; set; }
            }
        }
    }
}