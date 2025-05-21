package kafka

import (
	"context"
	"testing"

	"ListingService/domain"

	"github.com/IBM/sarama"
	"github.com/stretchr/testify/assert"
)

type MockJobService struct {
	CreateJobFunc func(job domain.Job) error
	UpdateJobFunc func(job domain.Job) error
	DeleteJobFunc func(jobID int64) error
	ListJobsFunc  func(ctx context.Context) ([]domain.Job, error)
}

func (m *MockJobService) CreateJob(job domain.Job) error {
	return m.CreateJobFunc(job)
}

func (m *MockJobService) UpdateJob(job domain.Job) error {
	return m.UpdateJobFunc(job)
}

func (m *MockJobService) DeleteJob(jobID int64) error {
	return m.DeleteJobFunc(jobID)
}

func (m *MockJobService) ListJobs(ctx context.Context) ([]domain.Job, error) {
	return m.ListJobsFunc(ctx)
}

func TestKafkaConsumer_ConsumeClaim(t *testing.T) {
	// Create a mock ConsumerGroup
	mockConsumerGroup := &MockConsumerGroup{}

	// Create a mock JobService
	mockJobService := &MockJobService{
		CreateJobFunc: func(job domain.Job) error {
			assert.Equal(t, int64(1), job.ID)
			assert.Equal(t, "Software Engineer", job.Title)
			assert.Equal(t, "Develop software applications", job.Description)
			assert.Equal(t, "Open", job.Status)
			assert.Equal(t, "user123", job.OwnerID)
			return nil
		},
		ListJobsFunc: func(ctx context.Context) ([]domain.Job, error) {
			return []domain.Job{}, nil
		},
	}

	// Create a KafkaConsumer instance
	consumer := &KafkaConsumer{
		consumerGroup: mockConsumerGroup,
		groupID:       "test-group",
		topic:         "jobs-topic",
		jobService:    mockJobService, // This now works because jobService is an interface
	}

	// Simulate a message
	message := &sarama.ConsumerMessage{
		Value: []byte(`{
			"jobId": 1,
			"title": "Software Engineer",
			"description": "Develop software applications",
			"status": "Open",
			"ownerId": "user123",
			"createdAt": "2025-01-15T21:11:18Z",
			"action": "Create"
		}`),
	}

	// Create a mock ConsumerGroupSession and ConsumerGroupClaim
	session := &MockConsumerGroupSession{
		CommitFunc: func() {
			// Do nothing
		},
	}
	claim := &MockConsumerGroupClaim{
		MessagesFunc: func() <-chan *sarama.ConsumerMessage {
			ch := make(chan *sarama.ConsumerMessage, 1)
			ch <- message
			close(ch)
			return ch
		},
	}

	// Simulate ConsumeClaim
	err := consumer.ConsumeClaim(session, claim)
	assert.NoError(t, err)
}
