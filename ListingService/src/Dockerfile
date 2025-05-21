# Use a lightweight base image for Go
FROM golang:1.23-alpine AS builder

WORKDIR /app

# Copy go.mod and go.sum
COPY ./go.mod ./go.sum ./
RUN go mod download

# Copy source files
COPY . .

# Build the application
RUN CGO_ENABLED=0 GOOS=linux go build -o ListingService ./cmd/main.go

# Final stage: minimal image
FROM alpine:3.17
WORKDIR /app
COPY --from=builder /app/ListingService /app/ListingService

EXPOSE 8080
ENTRYPOINT ["/app/ListingService"]