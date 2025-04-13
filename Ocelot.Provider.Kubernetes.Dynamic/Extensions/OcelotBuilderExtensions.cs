using KubeClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Kubernetes.Dynamic.Caching;
using Ocelot.Provider.Kubernetes.Dynamic.Interfaces;
using Ocelot.Provider.Kubernetes.Dynamic.Tokens;

namespace Ocelot.Provider.Kubernetes.Dynamic.Extentions
{
    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddDynamicKubernetes(this IOcelotBuilder builder)
        {
            var services = builder.Services;

            services.AddOptions<KubeClientOptions>()
                .Configure<IConfiguration>((opts, config) =>
                {
                    config.GetSection("Kubernetes").Bind(opts);
                });

            services.AddSingleton<IKubernetesTokenCache, KubernetesTokenCache>();
            services.AddSingleton<IKubeApiClientFactory, CachingKubeApiClientFactory>();

            return builder;
        }
    }
}
