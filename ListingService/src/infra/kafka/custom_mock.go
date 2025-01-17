package kafka

import (
	"context"
	"errors"

	"github.com/IBM/sarama"
)

// MockConsumerGroup is a mock implementation of sarama.ConsumerGroup.
type MockConsumerGroup struct {
	SetupFunc     func(sarama.ConsumerGroupSession) error
	CleanupFunc   func(sarama.ConsumerGroupSession) error
	ConsumeFunc   func(ctx context.Context, topics []string, handler sarama.ConsumerGroupHandler) error
	CloseFunc     func() error
	ErrorsFunc    func() <-chan error
	PauseFunc     func(map[string][]int32)
	ResumeFunc    func(map[string][]int32)
	PauseAllFunc  func()
	ResumeAllFunc func()
}

func (m *MockConsumerGroup) Setup(session sarama.ConsumerGroupSession) error {
	if m.SetupFunc != nil {
		return m.SetupFunc(session)
	}
	return nil
}

func (m *MockConsumerGroup) Cleanup(session sarama.ConsumerGroupSession) error {
	if m.CleanupFunc != nil {
		return m.CleanupFunc(session)
	}
	return nil
}

func (m *MockConsumerGroup) Consume(ctx context.Context, topics []string, handler sarama.ConsumerGroupHandler) error {
	if m.ConsumeFunc != nil {
		return m.ConsumeFunc(ctx, topics, handler)
	}
	return errors.New("ConsumeFunc not implemented")
}

func (m *MockConsumerGroup) Close() error {
	if m.CloseFunc != nil {
		return m.CloseFunc()
	}
	return nil
}

func (m *MockConsumerGroup) Errors() <-chan error {
	if m.ErrorsFunc != nil {
		return m.ErrorsFunc()
	}
	return nil
}

func (m *MockConsumerGroup) Pause(partitions map[string][]int32) {
	if m.PauseFunc != nil {
		m.PauseFunc(partitions)
	}
}

func (m *MockConsumerGroup) Resume(partitions map[string][]int32) {
	if m.ResumeFunc != nil {
		m.ResumeFunc(partitions)
	}
}

func (m *MockConsumerGroup) PauseAll() {
	if m.PauseAllFunc != nil {
		m.PauseAllFunc()
	}
}

func (m *MockConsumerGroup) ResumeAll() {
	if m.ResumeAllFunc != nil {
		m.ResumeAllFunc()
	}
}

// MockConsumerGroupSession is a mock implementation of sarama.ConsumerGroupSession.
type MockConsumerGroupSession struct {
	ClaimsFunc       func() map[string][]int32
	MemberIDFunc     func() string
	GenerationIDFunc func() int32
	MarkOffsetFunc   func(topic string, partition int32, offset int64, metadata string)
	CommitFunc       func()
	ResetOffsetFunc  func(topic string, partition int32, offset int64, metadata string)
	MarkMessageFunc  func(msg *sarama.ConsumerMessage, metadata string)
	ContextFunc      func() context.Context
}

func (m *MockConsumerGroupSession) Commit() {
	if m.CommitFunc != nil {
		m.CommitFunc()
	}
}

func (m *MockConsumerGroupSession) Claims() map[string][]int32 {
	if m.ClaimsFunc != nil {
		return m.ClaimsFunc()
	}
	return nil
}

func (m *MockConsumerGroupSession) MemberID() string {
	if m.MemberIDFunc != nil {
		return m.MemberIDFunc()
	}
	return ""
}

func (m *MockConsumerGroupSession) GenerationID() int32 {
	if m.GenerationIDFunc != nil {
		return m.GenerationIDFunc()
	}
	return 0
}

func (m *MockConsumerGroupSession) MarkOffset(topic string, partition int32, offset int64, metadata string) {
	if m.MarkOffsetFunc != nil {
		m.MarkOffsetFunc(topic, partition, offset, metadata)
	}
}

func (m *MockConsumerGroupSession) ResetOffset(topic string, partition int32, offset int64, metadata string) {
	if m.ResetOffsetFunc != nil {
		m.ResetOffsetFunc(topic, partition, offset, metadata)
	}
}

func (m *MockConsumerGroupSession) MarkMessage(msg *sarama.ConsumerMessage, metadata string) {
	if m.MarkMessageFunc != nil {
		m.MarkMessageFunc(msg, metadata)
	}
}

func (m *MockConsumerGroupSession) Context() context.Context {
	if m.ContextFunc != nil {
		return m.ContextFunc()
	}
	return context.Background()
}

// MockConsumerGroupClaim is a mock implementation of sarama.ConsumerGroupClaim.
type MockConsumerGroupClaim struct {
	TopicFunc               func() string
	PartitionFunc           func() int32
	InitialOffsetFunc       func() int64
	HighWaterMarkOffsetFunc func() int64
	MessagesFunc            func() <-chan *sarama.ConsumerMessage
}

func (m *MockConsumerGroupClaim) Topic() string {
	if m.TopicFunc != nil {
		return m.TopicFunc()
	}
	return ""
}

func (m *MockConsumerGroupClaim) Partition() int32 {
	if m.PartitionFunc != nil {
		return m.PartitionFunc()
	}
	return 0
}

func (m *MockConsumerGroupClaim) InitialOffset() int64 {
	if m.InitialOffsetFunc != nil {
		return m.InitialOffsetFunc()
	}
	return 0
}

func (m *MockConsumerGroupClaim) HighWaterMarkOffset() int64 {
	if m.HighWaterMarkOffsetFunc != nil {
		return m.HighWaterMarkOffsetFunc()
	}
	return 0
}

func (m *MockConsumerGroupClaim) Messages() <-chan *sarama.ConsumerMessage {
	if m.MessagesFunc != nil {
		return m.MessagesFunc()
	}
	return nil
}
