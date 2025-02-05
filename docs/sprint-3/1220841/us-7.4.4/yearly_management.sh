#!/bin/bash
# Define the source and destination paths
MONTHLY_FOLDER_PATH="/backup_management/monthly"
YEARLY_FOLDER_PATH="/backup_management/yearly"

# Ensure the yearly folder exists
if [ ! -d "$YEARLY_FOLDER_PATH" ]; then
  mkdir -p "$YEARLY_FOLDER_PATH"
fi
# Find the most recent backup in the monthly folder
LATEST_BACKUP=$(ls -t "$MONTHLY_FOLDER_PATH" | head -n 1)
# Check if a backup exists
if [ -n "$LATEST_BACKUP" ]; then
  # Copy the latest backup to the yearly folder
  cp "$MONTHLY_FOLDER_PATH/$LATEST_BACKUP" "$YEARLY_FOLDER_PATH/$LATEST_BACKUP"
  # Log the operation
  echo "Copied $LATEST_BACKUP to yearly folder on $(date +'%Y-%m-%d')" >> /var/log/yearly_backup.log
  # Delete all files in the monthly folder
  rm -rf "$MONTHLY_FOLDER_PATH"/*
  echo "Cleaned up monthly folder on $(date +'%Y-%m-%d')" >> /var/log/yearly_backup.log
else
  echo "No backups found in monthly folder on $(date +'%Y-%m-%d')" >> /var/log/yearly_backup.log
fi