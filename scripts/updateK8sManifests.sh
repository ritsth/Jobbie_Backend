#!/bin/bash
set -euo pipefail

if [ "$#" -ne 4 ]; then
  echo "Usage: $0 <service_root_folder> <service_folder> <image_repository> <tag>"
  exit 1
fi

SERVICE_ROOT="$1"         # e.g., JobService or AdminService
SERVICE_FOLDER="$2"       # e.g., job-api or admin-grpc
IMAGE_REPOSITORY="$3"     # e.g., advanceduno/jobservice
IMAGE_TAG="$4"            # e.g., 14

REPO_URL="https://x-access-token:$GITHUB_TOKEN@github.com/AdvancedUno/Jobbie_Backend.git"

# Clone repo
git clone "$REPO_URL" /tmp/temp_repo
cd /tmp/temp_repo

# Configure Git identity
git config user.name "GitHub Actions"
git config user.email "actions@github.com"

# Build dynamic path
DEPLOYMENT_FILE="$SERVICE_ROOT/k8s/$SERVICE_FOLDER/deployment.yaml"

# Validate file exists
if [ ! -f "$DEPLOYMENT_FILE" ]; then
  echo "Error: Deployment file $DEPLOYMENT_FILE not found!"
  exit 1
fi

# Replace image tag in deployment file
sed -i "s|image: .*|image: $IMAGE_REPOSITORY:$IMAGE_TAG|g" "$DEPLOYMENT_FILE"

# Commit and push
git add "$DEPLOYMENT_FILE"
git commit -m "Update image for $SERVICE_FOLDER to $IMAGE_REPOSITORY:$IMAGE_TAG"
git push

# Cleanup
rm -rf /tmp/temp_repo
