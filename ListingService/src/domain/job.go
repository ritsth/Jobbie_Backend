package domain

import "time"

type Job struct {
	ID          int64     `json:"id"`
	Title       string    `json:"title"`
	Description string    `json:"description"`
	Status      string    `json:"status"`
	OwnerID     string    `json:"owner_id"`
	CreatedAt   time.Time `json:"created_at"`
}
