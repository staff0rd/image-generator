# Enable buildx (if not already enabled)
docker buildx create --use

# Build for both architectures and push to registry
docker buildx build --platform linux/amd64,linux/arm64 \
  -t staff0rd/image-generator:4.1 \
  --push \
  .