#!/bin/bash
set -euo pipefail

if [ "$#" -ne 3 ]; then
  echo "Usage: $0 <service_folder> <image_repository> <tag>"
  exit 1
fi

SERVICE_FOLDER="$1"        
IMAGE_REPOSITORY="$2"      
IMAGE_TAG="$3"             

REPO_URL="https://x-access-token:$GITHUB_TOKEN@github.com/AdvancedUno/Jobbie_Backend.git"

# Clone repo
git clone "$REPO_URL" /tmp/temp_repo
cd /tmp/temp_repo

DEPLOYMENT_FILE="AdminService/k8s/$SERVICE_FOLDER/deployment.yaml"

# Update the image tag line
sed -i "s|image: .*|image: $IMAGE_REPOSITORY:$IMAGE_TAG|g" "$DEPLOYMENT_FILE"

# Git commit and push
git add "$DEPLOYMENT_FILE"
git commit -m "Update image for $SERVICE_FOLDER to $IMAGE_REPOSITORY:$IMAGE_TAG"
git push

# Cleanup
rm -rf /tmp/temp_repo
