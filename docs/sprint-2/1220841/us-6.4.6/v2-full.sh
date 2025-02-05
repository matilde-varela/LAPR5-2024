#!/bin/bash

LOG_FILE="/backup/log/full_backups.log"
DATE=$(date +%Y-%m-%d)
FULL_BACKUP_DIR="/backup/full"
MYSQL_USER="root"
MYSQL_PASSWORD="hospital-project"
MYSQL_HOST="vs773.dei.isep.ipp.pt"
MYSQL_PORT="3306"

# Log start time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting full backup script..." >> "$LOG_FILE"

# Check if the full backup directory exists, and create it only if it does not
if [ ! -d "$FULL_BACKUP_DIR" ]; then
  mkdir -p "$FULL_BACKUP_DIR"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Created full backup directory: $FULL_BACKUP_DIR" >> "$LOG_FILE"
fi

# Perform the full backup using mysqldump
BACKUP_FILE="$FULL_BACKUP_DIR/full_backup_$DATE.sql"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting mysqldump to $BACKUP_FILE..." >> "$LOG_FILE"
mysqldump --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --host="$MYSQL_HOST" --port="$MYSQL_PORT" \
  --single-transaction --flush-logs  --all-databases > "$BACKUP_FILE" 2>> "$LOG_FILE"

if [ $? -eq 0 ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup completed successfully." >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Full backup failed." >> "$LOG_FILE"
  exit 1
fi

# Compress the full backup file
tar -czf "$BACKUP_FILE.tar.gz" -C "$FULL_BACKUP_DIR" "$(basename $BACKUP_FILE)" 2>> "$LOG_FILE"

if [ $? -eq 0 ]; then
  rm -f "$BACKUP_FILE" # Remove the uncompressed file
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup compressed to $BACKUP_FILE.tar.gz." >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Compression failed." >> "$LOG_FILE"
  exit 1
fi

# Log end time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup script finished." >> "$LOG_FILE"
