# US 7.4.2

<!-- TOC -->
* [US 7.4.2](#us-742)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
<!-- TOC -->


## 1. Context

This is the first time this US is being worked on.

## 2. Requirements

**US 7.4.2:** As the organisation's administrator, I want to be presented with a justification for the change(s) to be 
made to the infrastructure in order to ensure a MTD(Maximum Tolerable Downtime) of 20 minutes.

**Acceptance Criteria:**

**US 7.4.2.1:** The justification must outline the specific infrastructure changes required to achieve an MTD of 20 minutes.

**US 7.4.2.2:** The justification must include technical, operational, and cost considerations for the proposed changes.

**US 7.4.2.3:** The document must provide evidence or analysis demonstrating how the proposed changes will meet the MTD 
requirement.


**Dependencies/References:**

This user story does not have dependencies.

## 3. Analysis

The main objective of this user story is to ensure the organization’s infrastructure changes align with achieving an MTD
of 20 minutes. This requires identifying weaknesses in the current setup, proposing targeted solutions, and providing 
clear justifications for these changes.

Key considerations include evaluating system reliability, redundancy, and recovery mechanisms. Cost, resource impact, 
and operational feasibility must also be addressed to ensure the proposal is actionable and aligns with organizational goals.

## 4. Design

### 4.1. Realization

To achieve an MTD(Maximum Tolerable Downtime) of 20 minutes it is important to make the follow changes:

* **Assess Current Infrastructure:** Conduct a thorough review of the existing system to identify vulnerabilities, such as 
single points of failure or inadequate backup strategies.
* **Backup Systems:** Automate backups to occur at short intervals, ensuring recent data is always available for quick recovery.
* **Strengthen Disaster Recovery Protocols:** Establish geographically distributed data centers to prevent service disruptions
in case of regional failures. Implement automated failover systems to switch operations to backup sites with minimal delay.
* **Monitoring and Alerting Systems:** Introduce 24/7 monitoring tools to track system performance, resource utilization, 
and health status of all critical components. Set up automated alerts to notify administrators of potential issues before they lead to failures.
* **Perform Regular DR Drills:** Schedule regular testing of the disaster recovery process and simulate various failure 
scenarios to ensure the system can recover within the 20-minute window.
* **Optimize System Performance:** Identify and eliminate any performance bottlenecks in the system, including slow database
queries, inadequate server resources, or network congestion, to ensure that recovery processes are not delayed.
