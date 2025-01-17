package application

import (
	"ListingService/domain"
	"ListingService/infra/db"
	"context"
	"fmt"
)

type JobService struct {
	repo *db.MySQLRepository
}

func NewJobService(repo *db.MySQLRepository) *JobService {
	return &JobService{repo: repo}
}

func (s *JobService) CreateJob(job domain.Job) error {
	if job.Title == "" || job.Description == "" || job.Status == "" || job.OwnerID == "" {
		return fmt.Errorf("invalid job data")
	}
	return s.repo.CreateJob(&job)
}

func (s *JobService) UpdateJob(job domain.Job) error {
	if job.Title == "" || job.Description == "" || job.Status == "" || job.OwnerID == "" {
		return fmt.Errorf("invalid job data")
	}
	return s.repo.UpdateJob(&job)
}

func (s *JobService) DeleteJob(jobID int64) error {
	return s.repo.DeleteJob(jobID)
}

func (s *JobService) ListJobs(ctx context.Context) ([]domain.Job, error) {
	return s.repo.ListJobs(ctx)
}
