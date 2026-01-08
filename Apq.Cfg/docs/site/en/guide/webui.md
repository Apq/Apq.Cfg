# Web Management UI

Apq.Cfg.WebUI provides a web management interface for centralized configuration management across multiple applications.

## Overview

WebUI is a **pure static** web application that can be deployed to any static file hosting service:

- Centralized multi-application configuration management
- Tree view for configuration hierarchy
- Real-time editing with auto-save
- Sensitive value masking
- Export to JSON/ENV/CSV formats
- API Key / JWT Bearer authentication support

## Deployment

### Docker Deployment

```bash
docker run -d \
  --name apqcfg-webui \
  -p 8080:80 \
  amwpfiqvy/apqcfg-webui:latest
```

Visit http://localhost:8080

### Docker Compose

```yaml
version: '3.8'
services:
  apqcfg-webui:
    image: amwpfiqvy/apqcfg-webui:latest
    ports:
      - "8080:80"
    restart: unless-stopped
```

### Static File Deployment

Build artifacts are pure static files that can be deployed to:

- Nginx / Apache
- GitHub Pages / GitLab Pages
- Vercel / Netlify
- AWS S3 / Azure Blob Storage
- Any HTTP server

```bash
# Build
cd Apq.Cfg.WebUI
npm install
npm run build
# Output to dist/ directory
```

### Virtual Directory Deployment

WebUI uses relative paths (`base: './'`), supporting deployment to any virtual directory:

```
http://example.com/                    # Root
http://example.com/apqcfg/             # Virtual directory
http://example.com/admin/config/       # Multi-level virtual directory
```

## Data Storage

Application endpoint information (including credentials) is stored in browser **localStorage** and never uploaded to any server.

```typescript
interface AppEndpoint {
  id: string           // Unique identifier
  name: string         // Application name
  url: string          // API URL (e.g., http://localhost:5000/api/apqcfg)
  authType: AuthType   // Auth type: None | ApiKey | JwtBearer
  apiKey?: string      // API Key
  token?: string       // JWT Token
  description?: string // Notes
}
```

## Remote Application Requirements

WebUI accesses remote application configuration APIs directly from the browser, so remote applications need to:

1. **Enable CORS** to allow WebUI origin access
2. **Expose configuration API** (`/api/apqcfg/*`)

```csharp
// Configure CORS when adding WebApi
builder.Services.AddApqCfgWebApi(cfg, options =>
{
    options.EnableCors = true;  // Enabled by default
    options.CorsOrigins = ["http://your-webui-domain"];  // Default ["*"]
});
```

## Features

### Application Management

- Add, edit, delete application endpoints
- Test connection before saving
- Multiple authentication methods

### Configuration Editing

- Tree view for configuration hierarchy
- Real-time value editing
- Add new configuration items
- Delete configuration items
- Read-only source indicator

### Import/Export

- Export to JSON/ENV/CSV/KV formats
- Drag and drop file import
- Batch operations

### Security Features

- Automatic sensitive value masking
- Local credential storage
- API Key / JWT Bearer authentication support

## Supported Architectures

Docker image supports the following architectures:

- `linux/amd64`
- `linux/arm64`

## Related Links

- [Docker Hub](https://hub.docker.com/r/amwpfiqvy/apqcfg-webui)
- [Source Code](https://gitee.com/apq/Apq.Cfg/tree/master/Apq.Cfg.WebUI)

## Notes

1. **CORS Configuration**: Ensure remote applications correctly configure CORS to allow WebUI domain access
2. **HTTPS**: Use HTTPS in production environments
3. **Credential Security**: API Keys and Tokens are stored in browser local storage, protect accordingly
