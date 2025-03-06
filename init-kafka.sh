#!/bin/sh
set -e  # Exit on error

echo "Kafka is up - Creating topics if they don't exist"

# Use kafka-topics command directly
if ! kafka-topics --list --bootstrap-server kafka:9092 | grep -wq "transaction-topic"; then
    kafka-topics --create --bootstrap-server kafka:9092 --topic transaction-topic --partitions 3 --replication-factor 1
    echo "Created topic: transaction-topic"
fi

if ! kafka-topics --list --bootstrap-server kafka:9092 | grep -wq "transaction-status"; then
    kafka-topics --create --bootstrap-server kafka:9092 --topic transaction-status --partitions 3 --replication-factor 1
    echo "Created topic: transaction-status"
fi

echo "Kafka topics created successfully!"
