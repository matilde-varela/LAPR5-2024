#!/bin/bash
LOG_FILE="/backup/log/backups.log"
DATE=$(date +%Y-%m-%d)
BACKUP_DIR="/backup/full"
DIFFERENTIAL_DIR="/backup/differential"
MYSQL_USER="root"
MYSQL_PASSWORD="PF+w2gYZ+0Wz"
MYSQL_HOST="vsgate-s1.dei.isep.ipp.pt"
MYSQL_PORT="11433"

# Log start time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting full backup script..." >> "$LOG_FILE"

# Check if the backup directory exists, and create it only if it does not
if [ ! -d "$BACKUP_DIR" ]; then
  mkdir -p "$BACKUP_DIR"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Created backup directory: $BACKUP_DIR" >> "$LOG_FILE"
fi

# Perform the full backup using Percona XtraBackup
xtrabackup --backup --target-dir="$BACKUP_DIR/full_backup_$DATE" --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --host="$MYSQL_HOST" --port="$MYSQL_PORT" >> "$LOG_FILE" 2>&1

# Compress the full backup directory
tar -czf "$BACKUP_DIR/full_backup_$DATE.tar.gz" -C "$BACKUP_DIR" "full_backup_$DATE" 2>> "$LOG_FILE"

# Remove the uncompressed backup directory to save space
rm -rf "$BACKUP_DIR/full_backup_$DATE"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup completed and compressed." >> "$LOG_FILE"

# Delete the previous backup (now the second most recent file)
PREVIOUS_BACKUP=$(ls -t "$BACKUP_DIR" | grep -E '\.tar\.gz$' | tail -n +2 | head -n 1)
if [ -n "$PREVIOUS_BACKUP" ]; then
  rm -f "$BACKUP_DIR/$PREVIOUS_BACKUP"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Deleted previous backup file: $PREVIOUS_BACKUP" >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] No previous backup file found to delete." >> "$LOG_FILE"
fi

# Check if the differential backup directory exists
if [ -d "$DIFFERENTIAL_DIR" ]; then
  # Delete all files in the differential backup directory
  rm -rf "$DIFFERENTIAL_DIR"/* 2>> "$LOG_FILE"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Differential backups in $DIFFERENTIAL_DIR have been deleted." >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Differential backup directory $DIFFERENTIAL_DIR does not exist." >> "$LOG_FILE"
fi

# Log end time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup script finished." >> "$LOG_FILE"
