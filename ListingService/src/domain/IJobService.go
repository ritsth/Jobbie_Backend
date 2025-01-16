package domain

import "context"

type JobService interface {
	CreateJob(job Job) error
	UpdateJob(job Job) error
	DeleteJob(jobID int64) error
	ListJobs(ctx context.Context) ([]Job, error)
}
