package kafka

import (
	"context"
	"encoding/json"
	"fmt"
	"log"

	"ListingService/application"
	"ListingService/domain"

	"github.com/IBM/sarama"
)

type KafkaConsumer struct {
	consumerGroup sarama.ConsumerGroup
	groupID       string
	topic         string
	jobService    domain.JobService
}

func NewKafkaConsumer(brokers []string, groupID, topic string, jobService *application.JobService) (*KafkaConsumer, error) {
	cfg := sarama.NewConfig()
	cfg.Version = sarama.V2_6_0_0
	consumerGroup, err := sarama.NewConsumerGroup(brokers, groupID, cfg)
	if err != nil {
		return nil, fmt.Errorf("unable to create consumer group: %w", err)
	}

	return &KafkaConsumer{
		consumerGroup: consumerGroup,
		groupID:       groupID,
		topic:         topic,
		jobService:    jobService,
	}, nil
}

func (kc *KafkaConsumer) Consume() {
	ctx := context.Background()
	for {
		err := kc.consumerGroup.Consume(ctx, []string{kc.topic}, kc)
		if err != nil {
			log.Printf("Error consuming messages: %v\n", err)
		}
	}
}

func (kc *KafkaConsumer) Close() error {
	if err := kc.consumerGroup.Close(); err != nil {
		return fmt.Errorf("error closing Kafka consumer: %w", err)
	}
	return nil
}

func (kc *KafkaConsumer) Setup(_ sarama.ConsumerGroupSession) error {
	return nil
}

func (kc *KafkaConsumer) Cleanup(_ sarama.ConsumerGroupSession) error {
	return nil
}

func (kc *KafkaConsumer) ConsumeClaim(session sarama.ConsumerGroupSession, claim sarama.ConsumerGroupClaim) error {
	for msg := range claim.Messages() {
		var incoming struct {
			JobId       int64  `json:"jobId"`
			Title       string `json:"title"`
			Description string `json:"description"`
			Status      string `json:"status"`
			OwnerId     string `json:"ownerId"`
			CreatedAt   string `json:"createdAt"`
			Action      string `json:"action"`
		}
		if err := json.Unmarshal(msg.Value, &incoming); err != nil {
			log.Printf("Error unmarshalling message: %v\n", err)
			session.MarkMessage(msg, "")
			continue
		}

		switch incoming.Action {
		case "Create":
			_ = kc.jobService.CreateJob(domain.Job{
				ID:          incoming.JobId,
				Title:       incoming.Title,
				Description: incoming.Description,
				Status:      incoming.Status,
				OwnerID:     incoming.OwnerId,
			})
		case "Update":
			_ = kc.jobService.UpdateJob(domain.Job{
				ID:          incoming.JobId,
				Title:       incoming.Title,
				Description: incoming.Description,
				Status:      incoming.Status,
				OwnerID:     incoming.OwnerId,
			})
		case "Delete":
			_ = kc.jobService.DeleteJob(incoming.JobId)
		}

		session.MarkMessage(msg, "")
	}
	return nil
}
