
# 🛠️ Jobbie Backend Deployment Instructions

Follow these steps **carefully** to build and deploy the Jobbie backend. ⚠️ **Do not commit or push any changes** made during this process.

---

## 📁 Step 1: Update Repository Name

Edit the following file based on your OS:

- **Windows**: `scripts/build_and_push.ps1`
- **Mac/Linux**: `scripts/build_and_push.sh`

Replace every instance of: advanceduno


with **your own Docker repository name** (e.g., `yourdockerusername`).

---

## 🔧 Step 2: Build and Push Docker Images

Run the script:

- **Windows**:
  ```powershell
  ./scripts/build_and_push.ps1
Mac/Linux:

  ```shell
./scripts/build_and_push.sh
 ```

If you encounter a Dockerfile error, locate the line:

```code
RUN dotnet publish AdminService.csproj -c Release -o /app
```
and update it to:

```
RUN dotnet publish /src/AdminService/src/AdminService/AdminService.csproj -c Release -o /app
This fixes the project path.
```

🚫 Do NOT commit or push this Dockerfile change.

🧾 Step 3: Update Kubernetes Deployment Files
Go to each service's deployment.yaml file (inside the k8s/ folder) and update the image reference.

Change this:

```
image: jobbieregistry.azurecr.io/jobbie_adminservice_grpc:245
```
To this:

```
image: yourdockerusername/jobbie_adminservice_grpc:latest
```
Replace yourdockerusername with your Docker Hub or container registry name, and update the tag if needed.

🚫 Do NOT commit or push these changes.

🚀 Step 4: Deploy to Kubernetes
Run the deployment script:

```
./scripts/deploy.ps1
```
✅ Make sure Docker Desktop is running before executing the script.

⚠️ Final Reminder
DO NOT COMMIT OR PUSH:

Any Dockerfile changes

Any updated Kubernetes deployment.yaml files

Your modified build script with your repo name

Use git add carefully and exclude all temporary changes above.
