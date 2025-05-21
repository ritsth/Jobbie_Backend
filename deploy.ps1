kubectl apply -f AdminService/k8s/
kubectl apply -f AdminService/k8s/database/
kubectl apply -f AdminService/k8s/admin-api/
kubectl apply -f AdminService/k8s/admin-grpc/

kubectl apply -f JobService/k8s/
kubectl apply -f JobService/k8s/database/
kubectl apply -f JobService/k8s/job-service-api/
kubectl apply -f JobService/k8s/job-service-grpc/

kubectl apply -f ListingService/k8s/
kubectl apply -f ListingService/k8s/database/


kubectl apply -f k8s/
kubectl apply -f k8s/kafka/

echo "All manifests applied successfully!"