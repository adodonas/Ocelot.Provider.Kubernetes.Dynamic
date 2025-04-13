# Ocelot.Provider.Kubernetes.Dynamic

Dynamic Kubernetes service discovery for Ocelot with automatic token rotation.

This package extends [Ocelot.Provider.Kubernetes](https://www.nuget.org/packages/Ocelot.Provider.Kubernetes) by enabling seamless token refresh, 
critical for AKS environments where service account tokens rotate hourly.

---

## Features

- Auto-refresh for Kubernetes API tokens
- Secure TLS support with `ca.crt`
- Plug-and-play with Ocelot service discovery
- Includes full unit test coverage

---

## NuGet

**Package**: [Ocelot.Provider.Kubernetes.Dynamic](https://www.nuget.org/packages/Ocelot.Provider.Kubernetes.Dynamic)

```bash
dotnet add package Ocelot.Provider.Kubernetes.Dynamic
