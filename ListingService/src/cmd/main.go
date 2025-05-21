package main

import (
	"context"
	"fmt"
	"log"
	"net/http"
	"os"
	"os/signal"
	"sync"
	"syscall"
	"time"

	"ListingService/application"
	"ListingService/infra/db"
	infraHTTP "ListingService/infra/http"
	"ListingService/infra/kafka"

	"github.com/gorilla/handlers"
)

func main() {
	// Load configuration from environment variables
	mysqlUser := os.Getenv("DB_USER")
	mysqlPassword := os.Getenv("DB_PASSWORD")
	mysqlDSN := fmt.Sprintf("%s:%s@tcp(mysql-listservice-headless:3306)/listdb?parseTime=true", mysqlUser, mysqlPassword)

	log.Printf("MySQL DSN: %s", mysqlDSN)
	if mysqlUser == "" || mysqlPassword == "" {
		log.Fatalf("Database credentials are missing.")
	}

	kafkaBrokers := []string{os.Getenv("KAFKA_BROKER")}
	if len(kafkaBrokers[0]) == 0 {
		kafkaBrokers[0] = "kafka:9092"
	}

	kafkaTopic := os.Getenv("KAFKA_TOPIC")
	if kafkaTopic == "" {
		kafkaTopic = "jobs-topic"
	}

	kafkaGroupID := os.Getenv("KAFKA_GROUP_ID")
	if kafkaGroupID == "" {
		kafkaGroupID = "listing-service-group"
	}

	// Initialize MySQL repository
	repo, err := db.NewMySQLRepository(mysqlDSN)
	if err != nil {
		log.Fatalf("Error creating MySQL repository: %v", err)
	}

	// Initialize Job service
	jobService := application.NewJobService(repo)

	// Initialize Kafka consumer
	consumer, err := kafka.NewKafkaConsumer(kafkaBrokers, kafkaGroupID, kafkaTopic, jobService)
	if err != nil {
		log.Fatalf("Error creating Kafka consumer: %v", err)
	}

	// Start Kafka consumer in a goroutine
	var wg sync.WaitGroup
	wg.Add(1)
	go func() {
		defer wg.Done()
		consumer.Consume()
	}()

	// Initialize HTTP server
	handler := infraHTTP.NewHandler(jobService)
	router := infraHTTP.NewRouter(handler)

	srv := &http.Server{
		Handler:      handlers.CORS(handlers.AllowedOrigins([]string{"*"}))(router),
		Addr:         ":8080",
		WriteTimeout: 15 * time.Second,
		ReadTimeout:  15 * time.Second,
	}

	// Graceful shutdown
	stop := make(chan os.Signal, 1)
	signal.Notify(stop, os.Interrupt, syscall.SIGTERM)

	go func() {
		log.Println("Listing service is running on port 8080")
		if err := srv.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Fatalf("Error starting server: %v", err)
		}
	}()

	<-stop
	log.Println("Shutting down server...")

	ctx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	if err := srv.Shutdown(ctx); err != nil {
		log.Fatalf("Error shutting down server: %v", err)
	}

	// Close Kafka consumer
	if err := consumer.Close(); err != nil {
		log.Fatalf("Error closing Kafka consumer: %v", err)
	}

	wg.Wait()
	log.Println("Server stopped gracefully")
}
