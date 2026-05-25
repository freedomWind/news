# Media Server Unity Integration

## Goal

Unity should not hardcode the local media server host inside mock data.

Mock/content data can use either:

- absolute URLs: `http://localhost:5234/api/media/variants/{variantId}/content`
- relative server URLs: `/api/media/variants/{variantId}/content`
- mock URLs: `mock://home-news-cover`

`MediaServerConfig` resolves relative server URLs at runtime.

## Runtime Config

`HomeDemoBootstrap` exposes:

```text
mediaServerConfig.enabled
mediaServerConfig.baseUrl
mediaServerConfig.resolveRelativeUrls
```

Default:

```text
enabled = true
baseUrl = http://localhost:5234
resolveRelativeUrls = true
```

## Editor Test

1. Start media server:

```powershell
cd app-server
$env:DOTNET_CLI_HOME="$PWD\.dotnet"
dotnet run --project src/NewsFramework.Media.Api --urls http://localhost:5234
```

2. Upload a test image and get a variant URL.

3. Put the relative URL into mock data:

```csharp
"/api/media/variants/{variantId}/content"
```

4. Run Unity Editor.

The media loader resolves it to:

```text
http://localhost:5234/api/media/variants/{variantId}/content
```

## Device Test

For real devices, `localhost` points to the device, not the development machine.

Set:

```text
mediaServerConfig.baseUrl = http://{dev-machine-lan-ip}:5234
```

Start server with:

```powershell
dotnet run --project src/NewsFramework.Media.Api --urls http://0.0.0.0:5234
```

Keep mock/content data as relative URLs.
