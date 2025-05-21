# Build and push JobService.Api
docker build -t advanceduno/jobserviceapi:latest -f JobService/src/JobService.Api/Dockerfile JobService/src/
docker push advanceduno/jobserviceapi:latest

# Build and push JobService.Grpc
docker build -t advanceduno/jobservicegrpc:latest -f JobService/src/JobService.Grpc/Dockerfile JobService/src/
docker push advanceduno/jobservicegrpc:latest

# # Build and push ListingService
docker build -t advanceduno/listingservice:latest -f ListingService/src/Dockerfile ListingService/src/
docker push advanceduno/listingservice:latest

# # Build and push AdminService.Api
docker build -t advanceduno/adminserviceapi:latest -f AdminService/src/AdminService/Dockerfile AdminService/src/
docker push advanceduno/adminserviceapi:latest

# # Build and push AdminService.Grpc
docker build -t advanceduno/adminservicegrpc:latest -f AdminService/src/AdminGrpcService/Dockerfile AdminService/src/
docker push advanceduno/adminservicegrpc:latest

Write-Output "All images built and pushed successfully!"