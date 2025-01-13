-- Create the database
CREATE DATABASE IF NOT EXISTS AdminJobDB;

-- Use the new database
USE AdminJobDB;

-- Create the AdminJobs table
CREATE TABLE IF NOT EXISTS AdminJobs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Status ENUM('Pending', 'Approved', 'Rejected', 'Deleted') DEFAULT 'Pending',
    OwnerId INT NOT NULL,
    CreatedDateTime DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Create a user with the mysql_native_password plugin
CREATE USER IF NOT EXISTS 'Admin'@'%' IDENTIFIED WITH mysql_native_password BY 'Admin';

-- Grant privileges
GRANT ALL PRIVILEGES ON AdminJobDB.* TO 'Admin'@'%';
FLUSH PRIVILEGES;

