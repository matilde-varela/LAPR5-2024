# US 7.4.3

<!-- TOC -->
- [US 7.4.3](#us-7.4.3)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
    - [3.1 System Sequence Diagram](#31-system-sequence-diagram)
    - [3.2 Domain Model](#32-domain-model)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
    - [4.2. Class Diagram](#42-class-diagram)
    - [4.3. Applied Patterns](#43-applied-patterns)
    - [4.4. Tests](#44-tests)
  - [5. Implementation](#5-implementation)
  - [6. Integration/Demonstration](#6-integrationdemonstration)
  - [7. Observations](#7-observations)
<!-- TOC -->


## 1. Context

This is the first time this US is being worked on.
It relates to the System Administrator.

## 2. Requirements

**US 7.4.3:** As a system administrator, I want to make a backup copy of the DB(s) to a Cloud environment using a script that renames it to the format yyyymmdd where is the name of the database, yyyy is the year the copy was made, mm is the month the copy was made and dd is the day the copy was made.

**Acceptance Criteria:**

**US 7.4.3.1:** The **yyyy** in the name format is for the year of the creation of the backup.

**US 7.4.3.2:** The **mm** in the name format is for the month of the creation of the backup.

**US 7.4.3.3:** The **dd** in the name format is for the day of the creation of the backup.

**US 7.4.3.4:** The backups are to be stored in a separate VM from the VM where the MongoDB DB is, the MDBackoffice DB is and the MDBackoffice as well.

**US 7.4.3.5:** In case there's an error while doing the backup, it should alert the admin and stop the action.

**US 7.4.3.6:** Also in case of an error it should log it in the appropriate folder, using the methods learned in class.

**Dependencies/References:**

**US 7.4.4-6 and 7.4.12:** They depend on this US as it creates the process to backup or even the backups themselves that will be later used or changed in later USs.

**Client Clarifications:**

>**Question:** None yet.
>
>**Answer:** None yet.

## 3. Analysis

This User Story will be a continuation of an earlier US, 6.4.6, since it implements a well established strategy.

This User Story will add the automatization of the backups depending on the database, it will change the location of the backups to a remote VM as to make sure they will not be lost in case of a malfunction of their current location.

This User Story will involve the system logs from Linux and possibly a FTS to send the files to other VMs.

## 4. Design

### 4.1. Realization

The design for this user story involves creating a script that automates the process of backing up databases to a Cloud environment with date-formatted naming. The script will identify the database(s) to back up, generate a backup file, and rename the file according to the format yyyyMMdd. The current date will be used for the yyyyMMdd part of the filename to ensure uniqueness and clarity.

The script will then transfer the backup to the specified Cloud environment using appropriate API commands or CLI tools for the chosen provider. It will rely on preconfigured authentication credentials and permissions for secure and successful uploads.

Error handling will be implemented to manage issues like connection failures, insufficient permissions, or backup failures, with logs generated for debugging and audit purposes. 

## 5. Implementation

In an effort to make an secure, simple connection with the VMs, a security key pair was created and shared with both VMs so there is no need to add the password everytime the script is run.

Changes were made to the backup file scripts in the MySQL virtual machine as to ensure it creates the backups and stores them in the machine and also sends them to the Backoffice virtual machine:

````
#!/bin/bash

LOG_FILE="/backup/log/$(date +%Y%m%d).log"
DATE=$(date +%Y-%m-%d)
DATEFORUS=$(date +%Y%m%d)
FULL_BACKUP_DIR="/backup/full"
MYSQL_USER="root"
MYSQL_PASSWORD="hospital-project"
MYSQL_HOST="vs773.dei.isep.ipp.pt"
MYSQL_PORT="3306"
REMOTE_VM_USER="root"       # Replace with the username for the remote VM
REMOTE_VM_HOST="vs251.dei.isep.ipp.pt"    # Replace with the remote VM's IP or hostname
REMOTE_VM_DIR="/backup/full"    # Replace with the destination directory on the remote VM

# Log start time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting full backup script..." >> "$LOG_FILE"

# Check if the full backup directory exists, and create it only if it does not
if [ ! -d "$FULL_BACKUP_DIR" ]; then
  mkdir -p "$FULL_BACKUP_DIR"
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Created full backup directory: $FULL_BACKUP_DIR" >> "$LOG_FILE"
fi

# Perform the full backup using mysqldump
BACKUP_FILE="$FULL_BACKUP_DIR/$DATEFORUS.sql"
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Starting mysqldump to $BACKUP_FILE..." >> "$LOG_FILE"
mysqldump --user="$MYSQL_USER" --password="$MYSQL_PASSWORD" --host="$MYSQL_HOST" \
  --single-transaction --flush-logs --all-databases > "$BACKUP_FILE" 2>> "$LOG_FILE"

if [ $? -eq 0 ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup completed successfully." >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Full backup failed." >> "$LOG_FILE"
  exit 1
fi

# Compress the full backup file
tar -czf "$BACKUP_FILE.tar.gz" -C "$FULL_BACKUP_DIR" "$(basename $BACKUP_FILE)" >> "$LOG_FILE" 2>&1

if [ $? -eq 0 ]; then
  rm -f "$BACKUP_FILE" # Remove the uncompressed file
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup compressed to $BACKUP_FILE.tar.gz" >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Compression failed." >> "$LOG_FILE"
  exit 1
fi

# Transfer the backup file to the remote VM
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Transferring backup file to remote VM..." >> "$LOG_FILE"
scp -i ~/.ssh/vmConnection "$BACKUP_FILE.tar.gz" "$REMOTE_VM_USER@$REMOTE_VM_HOST:$REMOTE_VM_DIR" >> "$LOG_FILE" 2>&1

if [ $? -eq 0 ]; then
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] Backup file transferred successfully to $REMOTE_VM_HOST:$REMOTE_VM_DIR" >> "$LOG_FILE"
else
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] ERROR: Failed to transfer backup file to remote VM." >> "$LOG_FILE"
  exit 1
fi

# Log end time
echo "[$(date '+%Y-%m-%d %H:%M:%S')] Full backup script finished." >> "$LOG_FILE"
````

It sends the files to the other VM using ssh (scp).

## 6. Testing

//TO BE DONE