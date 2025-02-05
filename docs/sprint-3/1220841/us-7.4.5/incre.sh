#!/bin/bash

LOG_TAG="incremental-backup"
DATE=$(date +%Y-%m-%d)
INCREMENTAL_BACKUP_DIR="/backup/incremental"
BINLOG_DIR="/var/lib/mysql"
MYSQL_USER="root"
MYSQL_PASSWORD="hospital-project"
MYSQL_HOST="vs773.dei.isep.ipp.pt"
MYSQL_PORT="3306"

logger -t "$LOG_TAG" -p local2.info "Starting incremental backup script."

# Check if the incremental backup directory exists, and create it only if it does not
if [ ! -d "$INCREMENTAL_BACKUP_DIR" ]; then
  mkdir -p "$INCREMENTAL_BACKUP_DIR"
  logger -t "$LOG_TAG" -p local2.notice "Created incremental backup directory: $INCREMENTAL_BACKUP_DIR"
fi
# Determine the latest binary log position from the MySQL server
MASTER_STATUS=$(mysql -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" -h "$MYSQL_HOST" -P "$MYSQL_PORT" -e "SHOW MASTER STATUS;")
if [ -z "$MASTER_STATUS" ]; then
  logger -t "$LOG_TAG" -p local2.err "Could not retrieve master status from MySQL!"
  exit 1
fi
# Extract the binary log file and position
BINLOG_FILE=$(echo "$MASTER_STATUS" | awk 'NR==2 {print $1}')
BINLOG_POS=$(echo "$MASTER_STATUS" | awk 'NR==2 {print $2}')
if [ -z "$BINLOG_FILE" ] || [ -z "$BINLOG_POS" ]; then
  logger -t "$LOG_TAG" -p local2.err "Invalid binary log info retrieved!"
  exit 1
fi
logger -t "$LOG_TAG" -p local2.info "Incremental backup starting from binary log file: $BINLOG_FILE, position: $BINLOG_POS."
# Extract incremental changes from binary logs using mysqlbinlog
INCREMENTAL_FILE="$INCREMENTAL_BACKUP_DIR/incremental_backup_$DATE.sql"
mysqlbinlog --read-from-remote-server --host="$MYSQL_HOST" --port="$MYSQL_PORT" \
  --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --start-position="$BINLOG_POS" \
  "$BINLOG_FILE" > "$INCREMENTAL_FILE" 2>> /dev/null
if [ $? -eq 0 ]; then
  logger -t "$LOG_TAG" -p local2.info "Incremental backup completed: $INCREMENTAL_FILE."
else
  logger -t "$LOG_TAG" -p local2.err "Incremental backup failed."
  exit 1
fi
# Compress the incremental backup file
tar -czf "$INCREMENTAL_FILE.tar.gz" -C "$INCREMENTAL_BACKUP_DIR" "$(basename $INCREMENTAL_FILE)" 2>> /dev/null
if [ $? -eq 0 ]; then
  rm -f "$INCREMENTAL_FILE" # Remove the uncompressed file
  logger -t "$LOG_TAG" -p local2.info "Incremental backup compressed to $INCREMENTAL_FILE.tar.gz."
else
  logger -t "$LOG_TAG" -p local2.err "Compression failed."
  exit 1
fi
logger -t "$LOG_TAG" -p local2.notice "Incremental backup script finished."
