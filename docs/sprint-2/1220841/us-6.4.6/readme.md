# US 6.4.6

<!-- TOC -->
- [US 6.4.6](#us-646)
  - [1. Context](#1-context)
  - [2. Requirements](#2-requirements)
  - [3. Analysis](#3-analysis)
  - [4. Design](#4-design)
    - [4.1. Realization](#41-realization)
  - [5. Implementation](#5-implementation)
  - [5. Testing](#5-testing)
  - [6. Observations](#6-observations)
<!-- TOC -->

## 1. Context

This is the first time this user story is worked on.

## 2. Requirements

**US 6.4.6:** As system administrator, I want a backup strategy to be proposed, justified and implemented that minimizes RPO (Recovery Point Objective) and WRT (Work Recovery Time).

## 3. Analysis

The goal of this user story is to create and implement a backup plan that minimizes the Recovery Point Objective (RPO) - the maximum tolerable period of data loss - and the Work Recovery Time (WRT), the time required to restore and verify the data and application.

To achieve this, it will be essential to determine the most effective approach, whether it involves full, partial, or a combination of backup types. Additionally, an optimal backup schedule should be set, with RPO and WRT kept to a minimum at all times.

## 4. Design

### 4.1. Realization

- **Adopted Strategy:** To minimize both the Recovery Point Objective (RPO) and Work Recovery Time (WRT), we decided on a combination of full and differential backups. Full backups will be conducted on weekends, while differential backups will take place on weekdays. This approach strikes a balance between maintaining data integrity and ensuring efficient recovery.

  - **Full Backup (Integral Backup):** Scheduled every Sunday at 2 a.m., this process provides a comprehensive snapshot of all data, creating a solid baseline for recovery. The 2 a.m. timing coincides with minimal hospital activity, typically when only emergency services are active. Due to the length of time required for a full backup, scheduling it during low-activity periods on weekends is optimal.

  - **Differential Backup:** Performed at 2 a.m. on weekdays, this method captures only data that has changed since the last full backup, thereby reducing both backup time and storage requirements compared to daily full backups. The chosen time aligns with the hospital’s low-activity periods. This strategy ensures faster restoration since only one full backup and one differential backup are needed, as opposed to incremental backups, which may require restoring up to six separate backups in the worst case.

- **Analysis of RPO and WRT:**

  - **RPO (Recovery Point Objective):** This strategy may result in a maximum data loss of up to 24 hours on weekdays if an incident occurs shortly before the differential backup. In such cases, only data from that day would be lost.
  - **WRT (Work Recovery Time):** The recovery process involves restoring the last full backup followed by the most recent differential. This method generally ensures faster restoration compared to incremental backups.

## 5. Implementation

Here's a complete solution for automating full and differential MySQL backups, including tar compression and cron scheduling.

It's important to notice that it was necessary to install a package:

```
apt update
apt install default-mysql-client
```

1. **Create the Full Backup Script**

   - [`full_backup.sh`](full-backup.sh)

2. **Create the Differential Backup Script**

   - [`differential_backup.sh`](differential-backup.sh)

3. **Make Scripts Executable**

The appropriate permissions were set to run the scripts using `chmod +x`.

5. **Schedule Cron Jobs**

To automate the backups I edited the file `/etc/crontab`.

**Note:** This is the format of the file:

```
MIN HOUR DOM MON DAY COMMAND

MIN: Minute field (0–59)
HOUR: Hour field (0–23)
DOM: Day of Month (1–31)
MON: Month (1–12, or abbreviated month names like jan, feb, etc.)
DAY: Day of the Week (0–7, where both 0 and 7 represent Sunday)
COMMAND: The command or script to be executed
```

Taking that into account, the following lines were added:

```
# Full backup at 2 a.m. every Sunday
0 2 * * 0  root  /scripts/full-backup.sh >> /tmp/cron_test.log 2>&1

# Differential backup at 2 a.m. on weekdays (Monday to Friday)
0 2 * * 1-6 root /scripts/differential-backup.sh >> /tmp/cron_test.log 2>&1
```

## 5. Testing

In order to test the solution, the scripts were manually executed once to ensure they work as expected:

## 6. Observations

- Here are some of the websites used as reference to implement this funcionality:

1. [How to set up automatic remote backups for Linux servers using rsync and SSH.](https://www.simplified.guide/linux/automatic-backup)<https://www.simplified.guide/linux/automatic-backup>
2. [Automated Backup and Rotation in Linux.](https://medium.com/@rohitdoshi9/automated-backup-and-rotation-in-linux-12ea9c545f12)
3. [Automating MySQL Database Backups with a Bash Script](https://medium.com/@innovativejude.tech/automating-mysql-database-backups-with-a-bash-script-a-step-by-step-guide-a4b5cdbebf77)
4. [Automate backup from remote mysql](https://unix.stackexchange.com/questions/346355/automate-backup-from-remote-mysql)
5. [Understanding Crontab in Linux](https://tecadmin.net/crontab-in-linux-with-20-examples-of-cron-schedule/)
