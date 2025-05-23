package db

import (
	"context"
	"database/sql"
	"fmt"
	"time"

	_ "github.com/go-sql-driver/mysql"

	"ListingService/domain"
)

type MySQLRepository struct {
	DB *sql.DB
}

func NewMySQLRepository(dsn string) (*MySQLRepository, error) {
	db, err := sql.Open("mysql", dsn)
	if err != nil {
		return nil, fmt.Errorf("unable to open DB: %w", err)
	}

	// Connection pooling settings
	db.SetMaxOpenConns(25)
	db.SetMaxIdleConns(25)
	db.SetConnMaxLifetime(5 * time.Minute)

	// Ping the database to ensure connectivity
	if err := db.Ping(); err != nil {
		return nil, fmt.Errorf("unable to ping DB: %w", err)
	}

	return &MySQLRepository{DB: db}, nil
}

func (r *MySQLRepository) CreateJob(job *domain.Job) error {
	query := `INSERT INTO jobs (job_id, title, description, status, owner_id, created_at)
              VALUES (?, ?, ?, ?, ?, ?)`
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	_, err := r.DB.ExecContext(ctx, query,
		job.JobID,
		job.Title,
		job.Description,
		job.Status,
		job.OwnerID,
		job.CreatedAt,
	)
	return err
}

func (r *MySQLRepository) UpdateJob(job *domain.Job) error {
	query := `UPDATE jobs 
              SET title = ?, description = ?, status = ?, owner_id = ? 
              WHERE job_id = ?`
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	_, err := r.DB.ExecContext(ctx, query,
		job.Title,
		job.Description,
		job.Status,
		job.OwnerID,
		job.JobID,
	)
	return err
}

func (r *MySQLRepository) DeleteJob(jobID string) error {
	query := `DELETE FROM jobs WHERE job_id = ?`
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	_, err := r.DB.ExecContext(ctx, query, jobID)
	return err
}

func (r *MySQLRepository) ListJobs(ctx context.Context) ([]domain.Job, error) {
	query := `SELECT job_id, title, description, status, owner_id, created_at FROM jobs`
	rows, err := r.DB.QueryContext(ctx, query)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var jobs []domain.Job
	for rows.Next() {
		var job domain.Job
		if err := rows.Scan(
			&job.JobID,
			&job.Title,
			&job.Description,
			&job.Status,
			&job.OwnerID,
			&job.CreatedAt,
		); err != nil {
			return nil, err
		}
		jobs = append(jobs, job)
	}
	return jobs, nil
}
