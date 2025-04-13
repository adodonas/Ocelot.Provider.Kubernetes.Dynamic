using KubeClient;

namespace Ocelot.Provider.Kubernetes.Dynamic.Interfaces
{
    public interface IKubeApiClientFactory
    {
        KubeApiClient Get();
    }
}