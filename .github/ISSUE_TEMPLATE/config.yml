name: Task Checklist
description: Create a task issue with subtasks as a checklist
title: "[TASK] <Short, actionable title>"
labels: ["todo", "help wanted"]
assignees: []

body:
  - type: markdown
    attributes:
      value: |
        ### ✅ Overview

        This issue is a checklist of subtasks that need to be completed. Team members can check them off as they go.

  - type: textarea
    id: checklist
    attributes:
      label: Checklist of Subtasks
      description: Add your subtasks in checklist format. Example:
      placeholder: |
        - [ ] Setup API route for job posting
        - [ ] Add input validation logic
        - [ ] Write unit tests for job route
        - [ ] Document API usage in README
      render: markdown
    validations:
      required: true

  - type: input
    id: related-files
    attributes:
      label: Related Code Area (Optional)
      placeholder: e.g., JobService/api/job.py

  - type: input
    id: tracking-pr
    attributes:
      label: Link to PR (if started)
      placeholder: https://github.com/your-org/your-repo/pull/42

  - type: textarea
    id: notes
    attributes:
      label: Additional Notes
      description: Add any blockers, dependencies, or context for the team
