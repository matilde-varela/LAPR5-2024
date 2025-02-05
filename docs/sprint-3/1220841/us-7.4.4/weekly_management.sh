#!/bin/bash

# Define the source and destination paths
FULL_BACKUP_PATH="/backups" ## to be defined
WEEKLY_FOLDER_PATH="/backup_management/weekly" ## to be defined

# Ensure the weekly folder exists
if [ ! -d "$WEEKLY_FOLDER_PATH" ]; then
  mkdir -p "$WEEKLY_FOLDER_PATH"
fi

# Get the latest backup in FULL_BACKUP_PATH
LATEST_BACKUP=$(ls -t "$FULL_BACKUP_PATH" | head -n 1)

# Check if a backup exists
if [ -n "$LATEST_BACKUP" ]; then
  # Move the latest backup to the weekly folder
  mv "$FULL_BACKUP_PATH/$LATEST_BACKUP" "$WEEKLY_FOLDER_PATH"

  # Log the operation (optional)
  echo "Moved latest backup '$LATEST_BACKUP' to weekly folder on $(date +"%Y-%m-%d %H:%M:%S")" >> /var/log/full_backup.log

else
  echo "No backup found in $FULL_BACKUP_PATH"
  exit 1
fi


