
---
# Ocelot.Provider.Kubernetes.Dynamic

---

## Highlights

- Automatic detection of token rotation
- Rebuilds Kubernetes API client transparently
- Secure TLS validation with `ca.crt`
- Minimal plug-and-play configuration

---

## Usage

### In Program.cs

```csharp
builder.Services
    .AddOcelot()
    .AddDynamicKubernetes();
```

### In appsettings.json

```json
"Kubernetes": {
  "ApiEndPoint": "https://kubernetes.default.svc",
  "KubeNamespace": "default",
  "AllowInsecure": false
}
```
