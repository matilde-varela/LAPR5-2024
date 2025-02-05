# US 6.4.5

<!-- TOC -->
* [US 6.4.5](#us-645)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [US 6.4.5 – Minimum Business Continuity Objective (MBCO) Proposal](#us-645--minimum-business-continuity-objective-mbco-proposal)
      * [Description](#description)
      * [System Context](#system-context)
      * [Identification of Critical Services](#identification-of-critical-services)
        * [1. Login](#1-login)
        * [2. Emergency Operation Requests Addition](#2-emergency-operation-requests-addition)
        * [3. Operation Scheduling Changes (Update and Deletion)](#3-operation-scheduling-changes-update-and-deletion)
        * [4. Internal Network Access](#4-internal-network-access)
        * [5. VPN Access](#5-vpn-access)
      * [Disruption Scenarios](#disruption-scenarios)
        * [1. Technical Failures](#1-technical-failures)
        * [2. Cybersecurity Incidents](#2-cybersecurity-incidents)
        * [3. External Dependencies](#3-external-dependencies)
        * [4. Planned Activities Gone Wrong](#4-planned-activities-gone-wrong)
        * [5. Environmental Factors](#5-environmental-factors)
        * [6. Human Error](#6-human-error)
<!-- TOC -->


## 1. Context

* This is the first time this user story is worked on.


* MBCO (Minimum Business Continuity Objective) specifies the minimum operability level that must be maintained during a disruption in the infrastructure.
 It guides what should be recovered and how extensive the recovery should be.

## 2. Requirements

**US 6.4.5:** As system administrator, I want to define the MBCO (Minimum Business Continuity Objective) to propose to stakeholders.

**Acceptance Criteria:**

No acceptance criteria yet.

**Dependencies/References:**

This user story is related to all user stories/functionalities of the Business Continuity Module, documenting the impacts
of disruptions in their operations and the recovery strategies taken into account.

**Client Clarifications:**

No client clarifications yet.

## 3. Analysis

An MBCO refers to the minimum level of operation that must be maintained during a disruption within the infrastructure.

The first step to define the MBCO is to identify which services/modules/functions are essential and must continue working
regardless of a disruption.

The proposal must be clear regarding the impact such disruptions bring to the system and associated risks, following the
definition of what each minimum operating level means for each affected component, such as conditioning of user access or
data availability. Ranking such elements based in need for continuity is advised.

Two more criteria must be explicitly defined for each critical component:
* **MTPD** (Maximum Tolerable Period of Disruption) - the maximum amount of performance time below the infrastructure requirements
* **MTD** (Maximum Tolerable Downtime) - the maximum amount of inoperability in the infrastructure

To recapitulate, the proposal must have the following structure:

> ### MBCO Proposal Structure
>
> - Introduction to the system context
>   - Identification of critical services and functions
> - Definition of minimum levels of operation
>   * service/function and minimum operation goal, MTD/MTPD metrics
> - Disruption Scenarios

## 4. Design

### US 6.4.5 – Minimum Business Continuity Objective (MBCO) Proposal

#### Description

This proposal describes the Minimum Business Continuity Objective (MBCO) for the system to ensure that critical functionalities remain operational during infrastructure disruptions. The proposal includes:

- Identification of critical services/resources.
- Metrics such as MTPD (Maximum Tolerable Period of Disruption) and MTD (Maximum Tolerable Downtime).
- Disruption scenarios and their impacts.

#### System Context

The application will be used in a hospital management environment, handling patient and staff accounts, as well as surgery scheduling.

#### Identification of Critical Services

Given the importance of the hospital environment, certain services are considered critical. Any underperformance or downtime for these services must be minimized. Below are the critical services and their corresponding MTPD and MTD values:

##### 1. Login

- **Why is it critical?**  
  This functionality is essential to ensure that only authorized users can access the system, maintaining security protocols and supporting business continuity.
- **Minimum operation goal:**  
  Ensure that login remains accessible 24/7, with redundancy to prevent unauthorized access attempts during any downtime.
- **MTD:** 1 minute
- **MTPD:** 30 minutes (allowing rapid restoration to avoid prolonged outages).

---

##### 2. Emergency Operation Requests Addition

- **Why is it critical?**  
  Necessary to allow emergency operation requests in the system, particularly during unpredictable disruptions.
- **Minimum operation goal:**  
  Provide an immediate way to log emergency requests, ensuring they are processed within a maximum of 5 minutes.
- **MTD:** Immediate (goal of zero delay for emergencies).
- **MTPD:** 5 minutes (minor tolerance for technical issues).

---

##### 3. Operation Scheduling Changes (Update and Deletion)

- **Why is it critical?**  
  Essential for updating or canceling operation requests in response to real-time changes in staff, availability, or emergencies.
- **Minimum operation goal:**  
  Ensure scheduling changes are processed within an acceptable delay to maintain updated information.
- **MTD:** 2 minutes
- **MTPD:** 10 minutes (allows minor delays but prioritizes timely updates for accuracy).

---

##### 4. Internal Network Access

- **Why is it critical?**  
  Essential for accessing resources necessary for operations, especially in local network settings where external connectivity may be limited.
- **Minimum operation goal:**  
  Maintain internal network connectivity with sufficient bandwidth to access core system modules.
- **MTD:** 1 minute
- **MTPD:** 30 minutes (limited delay acceptable due to fallback methods for access).

---

##### 5. VPN Access

- **Why is it critical?**  
  Ensures remote access for key staff and administrators to maintain continuity in remote locations.
- **Minimum operation goal:**  
  Maintain secure VPN access for a minimum of key staff, with fallback systems to ensure continuous and secure connections.
- **MTD:** 2 minutes
- **MTPD:** 15 minutes (critical, but redundancy exists for access tiers).

---

#### Disruption Scenarios

Disruptions refer to events or situations that impair or degrade normal system operations, infrastructure, or services. These scenarios can impact the system’s ability to perform expected functionalities, particularly critical services. Below are potential disruption scenarios:

##### 1. Technical Failures

- Server crashes.
- Database corruption or unavailability.
- Hardware malfunctions.

---

##### 2. Cybersecurity Incidents

- Unauthorized access (hacking).
- Ransomware attacks or data breaches.

---

##### 3. External Dependencies

- Cloud service interruptions (IAM, remote database, etc.).

---

##### 4. Planned Activities Gone Wrong

- Maintenance tasks causing unexpected downtime.
- Software deployments with errors/bugs.

---

##### 5. Environmental Factors

- Power outages.
- Natural disasters affecting physical data centers.

---

##### 6. Human Error

- Incorrect configurations.
- Accidental deletion of critical data.

---

This proposal outlines these critical services and disruption scenarios to provide a clear MBCO plan that ensures continuity in a hospital management environment.
