package db

import (
	"context"
	"database/sql"
	"fmt"
	"log"
	"time"

	_ "github.com/go-sql-driver/mysql"

	"ListingService/domain"
)

type MySQLRepository struct {
	DB *sql.DB
}

func NewMySQLRepository(dsn string) (*MySQLRepository, error) {
	log.Printf("Connecting to MySQL database at %s\n", dsn)
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
	query := `INSERT INTO jobs (id, title, description, status, owner_id, created_at)
              VALUES (?, ?, ?, ?, ?, ?)`
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	_, err := r.DB.ExecContext(ctx, query,
		job.ID,
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
              WHERE id = ?`
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	_, err := r.DB.ExecContext(ctx, query,
		job.Title,
		job.Description,
		job.Status,
		job.OwnerID,
		job.ID,
	)
	return err
}

func (r *MySQLRepository) DeleteJob(jobID int64) error {
	query := `DELETE FROM jobs WHERE id = ?`
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	_, err := r.DB.ExecContext(ctx, query, jobID)
	return err
}

func (r *MySQLRepository) ListJobs(ctx context.Context) ([]domain.Job, error) {
	query := `SELECT id, title, description, status, owner_id, created_at FROM jobs`
	rows, err := r.DB.QueryContext(ctx, query)
	if err != nil {
		return nil, err
	}
	defer rows.Close()

	var jobs []domain.Job
	for rows.Next() {
		var job domain.Job
		if err := rows.Scan(
			&job.ID,
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
