package http

import (
	"context"
	"encoding/json"
	"log"
	"net/http"
	"time"

	"ListingService/application"
)

type Handler struct {
	jobService *application.JobService
}

func NewHandler(jobService *application.JobService) *Handler {
	return &Handler{jobService: jobService}
}

func (h *Handler) GetAllJobs(w http.ResponseWriter, r *http.Request) {
	ctx, cancel := context.WithTimeout(r.Context(), 5*time.Second)
	defer cancel()

	jobs, err := h.jobService.ListJobs(ctx)
	if err != nil {
		log.Printf("Error fetching jobs: %v\n", err)
		http.Error(w, "Failed to fetch jobs", http.StatusInternalServerError)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	if err := json.NewEncoder(w).Encode(jobs); err != nil {
		log.Printf("Error encoding jobs: %v\n", err)
		http.Error(w, "Error encoding jobs", http.StatusInternalServerError)
	}
}
