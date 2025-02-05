# US 7.4.5

<!-- TOC -->
- [US 7.4.5](#us-745)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [4. Design](#4-design)
  - [Implementation](#implementation)
<!-- TOC -->


## 1. Context

This user story is being implemented for the first time.

## 2. Requirements

**US 7.4.5:** As a system administrator I want the US process of the DB backup to be kept in the Linux log, in an appropriate context, and alert the administrator when accessing the console if a serious failure occurs in this process.

## 3. Analysis

In this user story, we need to configure the logging service to support this feature. The log settings will be adjusted to ensure proper saving, and in case of failure, the administrator will be notified upon accessing the console.

## 4. Design

The script used for the database backup process described in user story [7.4.3](../../1220738/us-7.4.3/readme.md)  will be updated to handle logging more effectively. We will use '-' to indicate that the action needs to be executed synchronously.

In the event of an error with severity error or higher, the administrator must be notified. To achieve this, we will implement a script that will be triggered to retrieve logs with the specified severity. Additionally, we will modify the administrator's `.bash_profile` file to ensure the logs are captured as intended.

## Implementation


1. Updated the backup scripts: [full_backup](full_backup.sh) e [incremental_backup](incre_backup.sh).

2. To notify the administrator, changed the file `.bashrc` by adding:

```console
# Display new logs with severity "err" or higher upon login
LAST_LOGIN_FILE="/root/.last_login_time"
LOG_TAG_INCRE="incremental-backup"
LOG_TAG_FULL="full-backup"

# Get the last login time or set to epoch if not available
if [ ! -f "$LAST_LOGIN_FILE" ]; then
  echo "1970-01-01 00:00:00" > "$LAST_LOGIN_FILE"
fi
LAST_LOGIN_TIME=$(cat "$LAST_LOGIN_FILE")

# Update the last login time
date "+%Y-%m-%d %H:%M:%S" > "$LAST_LOGIN_FILE"

# Fetch new logs with severity "err" or higher and format them
LOGS=$(journalctl -p err --since "$LAST_LOGIN_TIME" -t "$LOG_TAG_INCRE" -t "$LOG_TAG_FULL" | grep -v "Journal begins")

if [ -z "$LOGS" ]; then
    echo "[ ALERT ] No new errors since your last login."
else
    echo "[ ALERT ] New Error(s):"
    echo "$LOGS" | nl -w 2 -s ". "  # Format errors with numbers
fi
```