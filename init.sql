-- Initialize the FingerPrint Verification Database
-- This script runs when the PostgreSQL container starts for the first time

-- Create the database if it doesn't exist (though it should already be created by POSTGRES_DB)
-- CREATE DATABASE IF NOT EXISTS FingerPrintVerificationDb;

-- Grant all privileges to the fingerprint_user
GRANT ALL PRIVILEGES ON DATABASE "FingerPrintVerificationDb" TO fingerprint_user;

-- Connect to the database
\c FingerPrintVerificationDb;

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- The Entity Framework will handle table creation via migrations
-- This file is mainly for any additional setup or permissions
