#!/bin/bash

LOG_FILE="/backup/log/backups.log"
DATE=$(date +%Y-%m-%d)
FULL_BACKUP_DIR="/backup/full"
DIFFERENTIAL_DIR="/backup/differential"
MYSQL_USER="root"
MYSQL_PASSWORD="PF+w2gYZ+0Wz"
MYSQL_HOST="vsgate-s1.dei.isep.ipp.pt"
MYSQL_PORT="11433"

# Log start time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting differential backup script..." >> "$LOG_FILE"

# Check if the differential backup directory exists, and create it only if it does not
if [ ! -d "$DIFFERENTIAL_DIR" ]; then
  mkdir -p "$DIFFERENTIAL_DIR"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Created differential backup directory: $DIFFERENTIAL_DIR" >> "$LOG_FILE"
fi

# Find the latest full backup
LATEST_FULL_BACKUP=$(ls -d "$FULL_BACKUP_DIR"/full_backup_* | sort | tail -n 1)

if [ -z "$LATEST_FULL_BACKUP" ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: No full backup found. Cannot create a differential backup!" >> "$LOG_FILE"
  exit 1
fi

echo "[$(date '+%Y-%m-%d %H:%M:%S')] Using latest full backup: $LATEST_FULL_BACKUP" >> "$LOG_FILE"

# Perform the differential backup using Percona XtraBackup
xtrabackup --backup --target-dir="$DIFFERENTIAL_DIR/diff_backup_$DATE" \
  --incremental-basedir="$LATEST_FULL_BACKUP" \
  --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --host="$MYSQL_HOST" --port="$MYSQL_PORT" >> "$LOG_FILE" 2>&1

# Compress the differential backup directory
tar -czf "$DIFFERENTIAL_DIR/diff_backup_$DATE.tar.gz" -C "$DIFFERENTIAL_DIR" "diff_backup_$DATE" 2>> "$LOG_FILE"

# Remove the uncompressed differential backup directory to save space
rm -rf "$DIFFERENTIAL_DIR/diff_backup_$DATE"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Differential backup completed and compressed." >> "$LOG_FILE"

# Log end time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Differential backup script finished." >> "$LOG_FILE"
