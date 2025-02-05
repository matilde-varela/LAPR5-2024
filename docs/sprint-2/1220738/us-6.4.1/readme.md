# US 6.4.1

<!-- TOC -->
- [US 6.4.1](#us-641)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
      - [Logical View](#logical-view)
      - [Process View](#process-view)
        - [Level 1](#level-1)
        - [Level 2](#level-2)
        - [Level 3](#level-3)
      - [Development View](#development-view)
      - [Physical View](#physical-view)
<!-- TOC -->

## 1. Context

This is the first time this US is being worked on.
This US pertains to the Admin.

## 2. Requirements

**US 6.4.1:** As system administrator, I want the deployment of one of the RFP modules in a DEI VM to be systematic, validating it on a scheduled bases with the test plan.

**Acceptance Criteria:**

**US 6.4.1.1 -** The deployment of one of the RFP modules in a DEI VM is to be systematic. 

**US 6.4.1.2 -** Validating the module should be doen through regular testing.

**Dependencies/References:**

USs related to the BackOffice are going to depend on this implementation. 

**Client Clarifications:**

> None yet.

## 3. Analysis

After some consideration, it was agreed on the MDBackOffice to be the module that will be implemented in the DEI VM. 

In an effort to make the implementation be systematic, a *script* and *scheduled events* are going to be created to regularly run tests to check on the status of the module.

## 4. Design

### 4.1. Realization

After questioning teachers and the client, this US will be implemented in the following manner:

1. A Linux DEI VM will be created to host the RFP Module.
2. With the help of the deployment scripts already available by BitBucket, we'll adapt them to the MDBackoffice Module.
3. A script, similar to a *pipeline*, will be created using Linux Scripts that will use the already existing tests to check on the good work of the module
4. These tests will be done regurlarly and so the cronstad will be used to check on the module every 2-3 days at 5AM, as to not disrupt the possible operations that could be scheduled.
5. When a commit is done, the script will check the changes, test them and if something does not pass the tests, it will rollback, again, to not affect the system's well being.

## 5. Implementation

### 5.1 Implementation - Repository Pull/Clone and Regular Testing

For this US we'll use a *Virtual Machine* from **DEI**, more especifically **"Apache and others on Debian 11 (bullseye) - All VNETs"** where we also installed dotnet 8.0 as well.

- **DNS Name** : vs251.dei.isep.ipp.pt
- **Access Port** : 22

#### 5.1.1 Important Directories

1. **/Hospital-Project/staging-application/ -** The directory where new commits and changes are sent to from the remote repository to be tested.

2. **/Hospital-Project/backup-application/ -** The directory where all files from production are sent when a deployment happens, just in case the files are corrupted and there is a need to return to the latest working version of the application.

3. **/Hospital-Project/production-application/ -** The directory where all files from staging are sent after being tested on.

4. **/etc/test-logs -** The directory where all the logs that are created when the staging/production folder is tested are sent to.

5. **/scripts/ -** The directory where the scripts for testing, pulling/cloning the repo are.


#### 5.1.2 Important Files

1. **deployment_script.sh -** It's a script made to either pull or clone from the repository depending on the state of the local */Hospital-Project/staging-application/* directory, it will then test the module for the backoffice and if all tests pass it will move the files to the */Hospital-Project/production-application/* directory, the directory where the app will be run:
````
#!/bin/bash
# Define paths
STAGING_DIR="/Hospital-Project/staging-application"
PRODUCTION_DIR="/Hospital-Project/production-application"
BACKUP_DIR="/Hospital-Project/backup-application"

# Check if the directory exists
if [ -d "$STAGING_DIR" ]; then
  echo "Directory exists, pulling latest changes..."
  cd "$STAGING_DIR" || exit 1  # Exit if the directory doesn't exist or cannot be accessed
  if [ ! -d ".git" ]; then
    echo "Error: The directory is not a Git repository!"
    exit 1
  fi
  git pull
else
  echo "Cloning repository..."
  git clone git@bitbucket.org:rita_barbosa/sem5pi-2425-dg38.git "$STAGING_DIR"
fi

set -e

echo "Running validation tests on staging..."

# Run validation tests
/scripts/validate_mdbackoffice.sh 1

if [ $? -ne 0 ]; then
    echo "Validation tests failed. Aborting deployment."
    exit 1
fi

echo "Staging tests passed."

echo "Starting deployment to production..."

cd ~

# Step 1: Ensure backup and production directories exist
mkdir -p "$BACKUP_DIR"
mkdir -p "$PRODUCTION_DIR"

# Step 2: Back up current production files
echo "Backing up current production files..."
rm -rf "$BACKUP_DIR"/*  # Clear the backup directory

# Check if the production directory has any files
if [ -z "$(ls -A ""$PRODUCTION_DIR"")" ]; then
  echo "Production directory is empty. Skipping backup."
else
  cp -r "$PRODUCTION_DIR"/* "$BACKUP_DIR/" || { echo "Backup failed. Aborting deployment."; exit 1; }
  echo "Backup successful."
fi

# Step 3: Clear the production directory
echo "Clearing the production directory..."
rm -rf "$PRODUCTION_DIR"/* || { echo "Failed to clear production folder. Aborting deployment."; exit 1; }

# Step 4: Copy files from staging to production
echo "Copying files from staging to production..."
cp -r "$STAGING_DIR"/* "$PRODUCTION_DIR/" || { echo "Failed to copy files to production. Aborting deployment."; exit 1; }

# Final Step: Confirm successful deployment
echo "Production deployment complete."
````

2. **validate_mdbackoffice.sh -** It's a script made to run either the backoffice from the staging directory or the production directory depending on input. Script will build and then test the module, creating a log file with the result in the *etc/test-logs/* directory:
````
#!/bin/bash

# Check if the first argument is provided
if [ -z "$1" ]; then
    echo "Please provide an argument (1 for staging, 2 for production)."
    exit 1
fi

# Define log file path with timestamp
LOG_DIR="/etc/test-logs"
mkdir -p "$LOG_DIR"  # Ensure the log directory exists

LOG_FILE="$LOG_DIR/build_test_log_$(date +"%Y%m%d_%H%M%S").log"

# Start logging
exec > >(tee -a "$LOG_FILE") 2>&1  # Redirect both stdout and stderr to the log file

# Set the directory based on the first argument
if [ "$1" -eq 1 ]; then
    DIR="/Hospital-Project/staging-application"
elif [ "$1" -eq 2 ]; then
    DIR="/Hospital-Project/production-application"
else
    echo "Invalid argument. Please use 1 for staging or 2 for production."
    exit 1
fi

# Build the project
echo "[Building...]"

cd "$DIR"

dotnet build sem5pi-24-25-dg38.sln

# Check if the build was successful
if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

# Run tests in the MDBackofficeTests folder
echo "[Running tests...]"

cd "$DIR/MDBackofficeTests"

dotnet test MDBackofficeTests.csproj --logger "console;verbosity=detailed"

# Check if tests were successful
if [ $? -ne 0 ]; then
    echo "Tests failed!"
    exit 1
fi

echo "[Build and Test completed successfully.]"
````

3. **/etc/crontad/ -** The file where the testing for the production will be set up to be done regularly at 05:00:
````
# /etc/crontab: system-wide crontab
# Unlike any other crontab you don't have to run the `crontab'
# command to install the new version when you edit this file
# and files in /etc/cron.d. These files also have username fields,
# that none of the other crontabs do.

SHELL=/bin/sh
PATH=/usr/local/sbin:/usr/local/bin:/sbin:/bin:/usr/sbin:/usr/bin

# Example of job definition:
# .---------------- minute (0 - 59)
# |  .------------- hour (0 - 23)
# |  |  .---------- day of month (1 - 31)
# |  |  |  .------- month (1 - 12) OR jan,feb,mar,apr ...
# |  |  |  |  .---- day of week (0 - 6) (Sunday=0 or 7) OR sun,mon,tue,wed,thu,fri,sat
# |  |  |  |  |
# *  *  *  *  * user-name command to be executed
17 *    * * *   root    cd / && run-parts --report /etc/cron.hourly
25 6    * * *   root    test -x /usr/sbin/anacron || ( cd / && run-parts --report /etc/cron.daily )
47 6    * * 7   root    test -x /usr/sbin/anacron || ( cd / && run-parts --report /etc/cron.weekly )
52 6    1 * *   root    test -x /usr/sbin/anacron || ( cd / && run-parts --report /etc/cron.monthly )
00 5    * * *   root    cd ~ && cd /scripts/ && ./validate_mdbackoffice 2
#
@reboot www-data /var/www/cgi-bin/servicesmanager BOOT >/dev/null 2>&1 &
#
````
