namespace Ocelot.Provider.Kubernetes.Dynamic.Interfaces
{
    public interface IKubernetesTokenCache
    {
        string GetToken();
        event Action? TokenChanged;
    }
}