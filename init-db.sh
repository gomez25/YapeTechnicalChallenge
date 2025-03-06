#!/bin/sh
set -e

echo "PostgreSQL is up - running migrations"

psql -U postgres <<EOF
-- Create the database if it doesn't exist
CREATE DATABASE transaction_db;
EOF

# Run migrations inside the target database
psql -U postgres -d transaction_db <<EOF
-- Enable pgcrypto extension for UUID generation
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE IF NOT EXISTS accounts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS transfer_types (
    id SERIAL PRIMARY KEY,
    type VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    amount DECIMAL NOT NULL,
    source_account_id UUID NOT NULL,
    target_account_id UUID NOT NULL,
    status VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    transfer_type_id INT NOT NULL,
    CONSTRAINT fk_source FOREIGN KEY (source_account_id) REFERENCES accounts(id) ON DELETE RESTRICT,
    CONSTRAINT fk_target FOREIGN KEY (target_account_id) REFERENCES accounts(id) ON DELETE RESTRICT,
    CONSTRAINT fk_transfer_type FOREIGN KEY (transfer_type_id) REFERENCES transfer_types(id)
);
EOF

echo "Database initialized successfully!"
