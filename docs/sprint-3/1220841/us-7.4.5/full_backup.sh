#!/bin/bash
LOG_TAG="backup-script"
LOG_FILE="/backup/log/$(date +%Y%m%d).log"
DATE=$(date +%Y-%m-%d)
DATEFORUS=$(date +%Y%m%d)
FULL_BACKUP_DIR="/backup/full"
MYSQL_USER="root"
MYSQL_PASSWORD="hospital-project"
MYSQL_HOST="vs773.dei.isep.ipp.pt"
MYSQL_PORT="3306"
REMOTE_VM_USER="root"
REMOTE_VM_HOST="vs251.dei.isep.ipp.pt"
REMOTE_VM_DIR="/backup/full"

logger -p local1.info -t "$LOG_TAG" "Starting full backup script..."

# Check if the full backup directory exists, and create it only if it does not
if [ ! -d "$FULL_BACKUP_DIR" ]; then
  mkdir -p "$FULL_BACKUP_DIR"
  logger -p local1.notice -t "$LOG_TAG" "Created full backup directory: $FULL_BACKUP_DIR"
fi

# Perform the full backup using mysqldump
BACKUP_FILE="$FULL_BACKUP_DIR/$DATEFORUS.sql"
logger -p local1.info -t "$LOG_TAG" "Starting mysqldump to $BACKUP_FILE..."

mysqldump --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --host="$MYSQL_HOST" \
  --single-transaction --flush-logs --all-databases > "$BACKUP_FILE"

if [ $? -eq 0 ]; then
  logger -p local1.info -t "$LOG_TAG" "Full backup completed successfully."
else
  logger -p local1.err -t "$LOG_TAG" "ERROR: Full backup failed."
  exit 1
fi

# Compress the full backup file
logger -p local1.info -t "$LOG_TAG" "Compressing full backup to $BACKUP_FILE.tar.gz..."
tar -czf "$BACKUP_FILE.tar.gz" -C "$FULL_BACKUP_DIR" "$(basename $BACKUP_FILE)"

if [ $? -eq 0 ]; then
  rm -f "$BACKUP_FILE"
  logger -p local1.info -t "$LOG_TAG" "Full backup compressed successfully to $BACKUP_FILE.tar.gz."
else
  logger -p local1.err -t "$LOG_TAG" "ERROR: Compression failed."
  exit 1
fi

# Transfer the backup file to the remote VM
logger -p local1.info -t "$LOG_TAG" "Transferring backup file to remote VM..."
scp -i ~/.ssh/vmConnection "$BACKUP_FILE.tar.gz" "$REMOTE_VM_USER@$REMOTE_VM_HOST:$REMOTE_VM_DIR"

if [ $? -eq 0 ]; then
  logger -p local1.info -t "$LOG_TAG" "Backup file transferred successfully to $REMOTE_VM_HOST:$REMOTE_VM_DIR."
else
  logger -p local1.err  -t "$LOG_TAG" "ERROR: Failed to transfer backup file to remote VM."
  exit 1
fi

logger local1.notice -t "$LOG_TAG" "Full backup script finished."


