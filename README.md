# Fullstack Microservice App

## Structure
- **frontend**: Next.js + shadcn
- **backend**: Node.js API
- **auth-api**: .NET Core authentication
- **postgres**: Main application DB
- **clickhouse**: Analytical database for warehousing

## Deployment (Not implemented yet!)
- Environment overlays: `k8s/overlays/dev`, `staging`, `prod`
- Managed with Kubernetes + Kustomize
