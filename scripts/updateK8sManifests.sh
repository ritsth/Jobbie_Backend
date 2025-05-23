#!/bin/bash
set -euo pipefail

# Check arguments
if [ "$#" -ne 3 ]; then
  echo "Usage: $0 <service_name> <image_repository> <tag>"
  exit 1
fi

# Environment variables
REPO_URL="https://x-access-token:$GITHUB_TOKEN@github.com/AdvancedUno/Jobbie_Backend.git"

# Clone repo
git clone "$REPO_URL" /tmp/temp_repo
cd /tmp/temp_repo

# Update image tag in Kubernetes manifest
sed -i "s|image: jobbieregistry.azurecr.io/.*|image: jobbieregistry.azurecr.io/$2:$3|g" k8s-specifications/$1-deployment.yaml

# Commit and push changes
git add .
git commit -m "Update image for $1 to $3"
git push

# Cleanup
rm -rf /tmp/temp_repo
