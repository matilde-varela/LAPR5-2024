#!/bin/bash

# Path to the auth.log file
LOG_FILE="/var/log/auth.log"

# Check if the log file exists
if [ ! -f "$LOG_FILE" ]; then
    echo "Log file not found: $LOG_FILE"
    exit 1
fi

echo "Users with more than 3 failed login attempts:"

# Parse the log file and find users with more than 3 failed attempts
grep "Failed password" "$LOG_FILE" | awk '{print $(NF-5)}' | sort | uniq -c | awk '$1 >= 3' | while read -r count user; do
    echo "User: $user - Failed Attempts: $count"
done
