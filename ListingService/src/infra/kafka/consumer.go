package kafka

import (
	"ListingService/application"
	"ListingService/domain"
	"context"
	"encoding/json"
	"fmt"
	"log"
	"time"

	"github.com/IBM/sarama"
)

// KafkaConsumer represents a Kafka consumer that processes job-related messages.
type KafkaConsumer struct {
	consumerGroup sarama.ConsumerGroup
	groupID       string
	topic         string
	jobService    domain.JobService
}

// NewKafkaConsumer creates a new Kafka consumer instance.
func NewKafkaConsumer(brokers []string, groupID, topic string, jobService *application.JobService) (*KafkaConsumer, error) {
	cfg := sarama.NewConfig()
	cfg.Version = sarama.V2_6_0_0 // Set Kafka protocol version

	// Create a new consumer group
	consumerGroup, err := sarama.NewConsumerGroup(brokers, groupID, cfg)
	if err != nil {
		return nil, fmt.Errorf("unable to create consumer group: %w", err)
	}

	log.Printf("Successfully created Kafka consumer group for topic: %s\n", topic)

	return &KafkaConsumer{
		consumerGroup: consumerGroup,
		groupID:       groupID,
		topic:         topic,
		jobService:    jobService,
	}, nil
}

// Consume starts consuming messages from the Kafka topic.
func (kc *KafkaConsumer) Consume() {
	ctx := context.Background()
	for {
		log.Printf("Starting to consume messages from topic: %s\n", kc.topic)
		err := kc.consumerGroup.Consume(ctx, []string{kc.topic}, kc)
		if err != nil {
			log.Printf("Error consuming messages: %v\n", err)
		}
	}
}

// Close shuts down the Kafka consumer gracefully.
func (kc *KafkaConsumer) Close() error {
	log.Println("Closing Kafka consumer...")
	if err := kc.consumerGroup.Close(); err != nil {
		return fmt.Errorf("error closing Kafka consumer: %w", err)
	}
	log.Println("Kafka consumer closed successfully")
	return nil
}

// Setup is called when the consumer group session is being set up.
func (kc *KafkaConsumer) Setup(_ sarama.ConsumerGroupSession) error {
	log.Println("Consumer group session is being set up")
	return nil
}

// Cleanup is called when the consumer group session is ending.
func (kc *KafkaConsumer) Cleanup(_ sarama.ConsumerGroupSession) error {
	log.Println("Consumer group session is ending")
	return nil
}

// ConsumeClaim processes messages from a Kafka topic partition.
func (kc *KafkaConsumer) ConsumeClaim(session sarama.ConsumerGroupSession, claim sarama.ConsumerGroupClaim) error {
	log.Printf("Starting to process messages from partition: %v\n", claim.Partition())

	for msg := range claim.Messages() {
		log.Printf("Received message: Topic=%s, Partition=%d, Offset=%d\n", msg.Topic, msg.Partition, msg.Offset)

		var incoming struct {
			JobId       string    `json:"jobId"`
			Title       string    `json:"title"`
			Description string    `json:"description"`
			Status      string    `json:"status"`
			OwnerId     string    `json:"ownerId"`
			CreatedAt   time.Time `json:"createdAt"`
			Action      string    `json:"action"`
		}

		// Unmarshal the incoming message
		if err := json.Unmarshal(msg.Value, &incoming); err != nil {
			log.Printf("Error unmarshalling message: %v\n", err)
			session.MarkMessage(msg, "") // Mark the message as processed
			continue
		}

		log.Printf("Processing message with action: %s, JobId: %d\n", incoming.Action, incoming.JobId)

		// Handle the message based on the action
		switch incoming.Action {
		case "create":
			log.Printf("Creating job with ID: %d\n", incoming.JobId)
			parsedCreatedAt, err := time.Parse(time.RFC3339, incoming.CreatedAt.Format(time.RFC3339))
			if err != nil {
				log.Printf("Invalid createdAt format: %v\n", err)
				session.MarkMessage(msg, "") // Mark the message as processed
				continue
			}

			// Pass the parsed createdAt to the job service
			err = kc.jobService.CreateJob(domain.Job{
				JobID:       incoming.JobId,
				Title:       incoming.Title,
				Description: incoming.Description,
				Status:      incoming.Status,
				OwnerID:     incoming.OwnerId,
				CreatedAt:   parsedCreatedAt,
			})
			if err != nil {
				log.Printf("Error creating job: %v\n", err)
			} else {
				log.Printf("Successfully created job with ID: %d\n", incoming.JobId)
			}

		case "update":
			log.Printf("Updating job with ID: %d\n", incoming.JobId)
			err := kc.jobService.UpdateJob(domain.Job{
				JobID:          incoming.JobId,
				Title:       incoming.Title,
				Description: incoming.Description,
				Status:      incoming.Status,
				OwnerID:     incoming.OwnerId,
			})
			if err != nil {
				log.Printf("Error updating job: %v\n", err)
			} else {
				log.Printf("Successfully updated job with ID: %d\n", incoming.JobId)
			}

		case "delete":
			log.Printf("Deleting job with ID: %d\n", incoming.JobId)
			err := kc.jobService.DeleteJob(incoming.JobId)
			if err != nil {
				log.Printf("Error deleting job: %v\n", err)
			} else {
				log.Printf("Successfully deleted job with ID: %d\n", incoming.JobId)
			}

		default:
			log.Printf("Unknown action: %s\n", incoming.Action)
		}

		// Mark the message as processed
		session.MarkMessage(msg, "")
		log.Printf("Message processed: Topic=%s, Partition=%d, Offset=%d\n", msg.Topic, msg.Partition, msg.Offset)
	}

	log.Printf("Stopped processing messages from partition: %v\n", claim.Partition())
	return nil
}
