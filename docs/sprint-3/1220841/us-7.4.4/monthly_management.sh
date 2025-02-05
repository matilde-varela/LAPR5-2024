#!/bin/bash

# Define the source and destination paths
WEEKLY_FOLDER_PATH="/backup_management/weekly" 
MONTHLY_FOLDER_PATH="/backup_management/monthly"
 
# Ensure the monthly folder exists
if [ ! -d "$MONTHLY_FOLDER_PATH" ]; then
  mkdir -p "$MONTHLY_FOLDER_PATH"
fi

# Find the most recent backup in the weekly folder
LATEST_BACKUP=$(ls -t "$WEEKLY_FOLDER_PATH" | head -n 1)

if [ -n "$LATEST_BACKUP" ]; then
  # Move the latest backup to the monthly folder
  mv "$WEEKLY_FOLDER_PATH/$LATEST_BACKUP" "$MONTHLY_FOLDER_PATH/$LATEST_BACKUP"

  # Log the operation (optional)
  echo "Moved $LATEST_BACKUP to monthly folder on $(date +'%Y-%m-%d')" >> /var/log/monthly_backup.log

  # Delete all files in the weekly folder
  rm -rf "$WEEKLY_FOLDER_PATH"/*

  echo "Cleaned up weekly folder on $(date +'%Y-%m-%d')" >> /var/log/monthly_backup.log
else
  echo "No backups found in weekly folder on $(date +'%Y-%m-%d')" >> /var/log/monthly_backup.log
fi