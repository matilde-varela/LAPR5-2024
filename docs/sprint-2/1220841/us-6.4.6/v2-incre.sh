#!/bin/bash

LOG_FILE="/backup/log/incremental_backups.log"
DATE=$(date +%Y-%m-%d)
INCREMENTAL_BACKUP_DIR="/backup/incremental"
BINLOG_DIR="/var/lib/mysql"
MYSQL_USER="root"
MYSQL_PASSWORD="hospital-project"
MYSQL_HOST="vs773.dei.isep.ipp.pt"
MYSQL_PORT="3306"

# Log start time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting incremental backup script..." >> "$LOG_FILE"

# Check if the incremental backup directory exists, and create it only if it does not
if [ ! -d "$INCREMENTAL_BACKUP_DIR" ]; then
  mkdir -p "$INCREMENTAL_BACKUP_DIR"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Created incremental backup directory: $INCREMENTAL_BACKUP_DIR" >> "$LOG_FILE"
fi

# Determine the latest binary log position from the MySQL server
MASTER_STATUS=$(mysql -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" -h "$MYSQL_HOST" -P "$MYSQL_PORT" -e "SHOW MASTER STATUS;")
if [ -z "$MASTER_STATUS" ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Could not retrieve master status from MySQL!" >> "$LOG_FILE"
  exit 1
fi

# Extract the binary log file and position
BINLOG_FILE=$(echo "$MASTER_STATUS" | awk 'NR==2 {print $1}')
BINLOG_POS=$(echo "$MASTER_STATUS" | awk 'NR==2 {print $2}')

if [ -z "$BINLOG_FILE" ] || [ -z "$BINLOG_POS" ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Invalid binary log info retrieved!" >> "$LOG_FILE"
  exit 1
fi

echo "[$(date '+%Y-%m-%d %H:%M:%S')] Incremental backup starting from binary log file: $BINLOG_FILE, position: $BINLOG_POS." >> "$LOG_FILE"

# Extract incremental changes from binary logs using mysqlbinlog
INCREMENTAL_FILE="$INCREMENTAL_BACKUP_DIR/incremental_backup_$DATE.sql"
mysqlbinlog --read-from-remote-server --host="$MYSQL_HOST" --port="$MYSQL_PORT" \
  --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --start-position="$BINLOG_POS" \
  "$BINLOG_FILE" > "$INCREMENTAL_FILE" 2>> "$LOG_FILE"

if [ $? -eq 0 ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Incremental backup completed: $INCREMENTAL_FILE." >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Incremental backup failed." >> "$LOG_FILE"
  exit 1
fi

# Compress the incremental backup file
tar -czf "$INCREMENTAL_FILE.tar.gz" -C "$INCREMENTAL_BACKUP_DIR" "$(basename $INCREMENTAL_FILE)" 2>> "$LOG_FILE"

if [ $? -eq 0 ]; then
  rm -f "$INCREMENTAL_FILE" # Remove the uncompressed file
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Incremental backup compressed to $INCREMENTAL_FILE.tar.gz." >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Compression failed." >> "$LOG_FILE"
  exit 1
fi

# Log end time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Incremental backup script finished." >> "$LOG_FILE"
