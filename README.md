# Transaction Service Project

## Overview

This project is a .NET 8 based microservices application that includes a Transaction Service and an Anti-Fraud Service. The services are containerized using Docker and orchestrated with Docker Compose. The application uses PostgreSQL for data storage and Kafka for messaging.

## Services

- **PostgreSQL**
  - **Database**: `transaction_db`
	- This database will store all transactions, regardless of their current status, along with their associated source and destination accounts.
- **Zookeeper**
- **Kafka**
  - Kafka will be used for communication between both services through the topics transaction-topic and transaction-status. 
- **Transaction Service**
  - This service will send transactions for validation to the anti-fraud service via a Kafka topic. The transactions will be validated, and their status will be updated accordingly. Additionally, this service has a direct connection to the database.	
- **Anti-Fraud Service**
  - This service will receive transactions with a pending status and perform the necessary validation to determine whether the final status of the transaction will be rejected or approved.	

## Prerequisites

- Docker
- Docker Compose

## Setup

1. Clone the repository:
git clone https://github.com/gomez25/YapeTechnicalChallenge.git cd <repository-directory>
1. 2. Build and start the services using Docker Compose:
docker-compose up --build## Usage

- **Transaction Service**: Accessible at `http://localhost:5001`
- **Anti-Fraud Service**: Accessible at `http://localhost:5002`

# Setup Guide for PostgreSQL and Kafka

## PostgreSQL

1. Start the PostgreSQL service
2. Into the container in the command line, run this:
	1. chmod +x ./docker-entrypoint-initdb.d/init-db.sh
	2. psql -U postgres -d transaction_db -f /docker-entrypoint-initdb.d/init-db.sh
3. Restart the Transaction service. 

## Kafka
1. Start the Kafka service
2. Into the container in the command line, run this:
	1. chmod +x ./docker-entrypoint-initdb.d/init-kafka.sh
	2. ./docker-entrypoint-initdb.d/init-kafka.sh
3. Restart the Transaction and Anti-Fraud service. 