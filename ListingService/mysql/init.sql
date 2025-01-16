-- Check if the database already exists
CREATE DATABASE IF NOT EXISTS jobsdb;
USE jobsdb;

-- Check if the table already exists
CREATE TABLE IF NOT EXISTS jobs (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    status VARCHAR(50) NOT NULL,
    owner_id VARCHAR(255) NOT NULL,
    created_at DATETIME  DEFAULT CURRENT_TIMESTAMP
);

-- Insert sample data only if the table is empty
INSERT INTO jobs (title, description, status, owner_id)
SELECT * FROM (SELECT 'Software Engineer', 'Develop software applications', 'Open', 'user123') AS tmp
WHERE NOT EXISTS (SELECT 1 FROM jobs);