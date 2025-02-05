# US 7.4.1

<!-- TOC -->
* [US 7.4.1](#us-741)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
    * [Plan Structure:](#plan-structure)
  * [4. Disaster Recovery Plan](#4-disaster-recovery-plan)
  * [1. Goals](#1-goals)
  * [2. Critical Services and MBCO Targets](#2-critical-services-and-mbco-targets)
  * [3. Emergency Response Procedures](#3-emergency-response-procedures)
    * [3.1 Activation](#31-activation)
    * [3.2 Initial Response](#32-initial-response)
  * [4. Backup and Recovery Procedures](#4-backup-and-recovery-procedures)
    * [4.1 Data Backup](#41-data-backup)
    * [4.2 Recovery Steps](#42-recovery-steps)
  * [5. Hot Site Plan](#5-hot-site-plan)
    * [5.1 Alternate Site](#51-alternate-site)
    * [5.2 Data Synchronization](#52-data-synchronization)
  * [6. Full System Restoration Steps](#6-full-system-restoration-steps)
  * [7. Regular Testing and Updates](#7-regular-testing-and-updates)
  * [8. Observations and Considerations](#8-observations-and-considerations)
  * [5. Observations](#5-observations)
<!-- TOC -->

## 1. Context

This user story focuses on developing a disaster recovery (DR) plan that meets the **Minimum Business Continuity Objective (MBCO)** defined in Sprint B.
The MBCO specifies the minimum operability level to maintain during disruptions, guiding recovery priorities and actions.

## 2. Requirements

**US 7.4.1:** As the organization's administrator, I want a disaster recovery plan that meets the MBCO defined in Sprint B.

**Acceptance Criteria:**
- **7.4.1.1.** A disaster recovery plan document is created and available.
- **7.4.1.2.** The plan identifies critical services and their MBCO targets.
- **7.4.1.3.** Emergency response procedures are included.
- **7.4.1.4.** Backup and recovery actions are clearly defined and tested.
- **7.4.1.5.** An alternate hot site plan is outlined for quick recovery.
- **7.4.1.6.** Full system restoration steps are described in detail.

**Dependencies/References:**
- Related to MBCO proposal (**US 6.4.5**).

**Client Clarifications:**

> **Question:** Should the disaster recovery plan account for both natural disasters and cybersecurity incidents? What types of disruptions/topics should be focused on?
> 
> **Answer:** The DRP should cover all catastrophes that may occur, provided they are adequately included. Disruptions/topics are those that are thought likely to occur, jeopardising the company's normal behaviour and business.

## 3. Analysis

A DR (disaster recovery) plan ensures that key operations can continue or resume quickly after a disruption. The plan will:
- Prioritize **critical services** aligned with MBCO targets.
- Include detailed **response procedures** to minimize risks to lives and systems.
- Define **backup operations** for essential data processing.
- Specify **hot site requirements** to maintain continuity during major outages.
- Outline **system restoration steps** to return to pre-disruption status.

### Plan Structure:

1. **Goals**: Define the main objectives of the DR plan.
2. **Application Profile**: Identify critical services and their roles.
3. **Backup Procedures**: Describe methods to secure essential data.
4. **Emergency Response**: Document steps to protect lives and limit damages.
5. **Recovery Actions**: Detail processes for restoring systems.
6. **Hot Site Plan**: Outline alternative site activation.
7. **Full System Restoration**: Provide instructions for complete recovery.

---

## 4. Disaster Recovery Plan

## 1. Goals

The primary goals of this Disaster Recovery (DR) Plan are:

1. Ensure continuity of critical hospital management services during and after a disruption.
2. Meet the Minimum Business Continuity Objective (MBCO) defined for the hospital environment.
3. Provide clear, actionable steps for recovery of services to pre-disruption status.

## 2. Critical Services and MBCO Targets

The following critical services and their corresponding Minimum Business Continuity Objectives (MBCO) will guide the recovery process:

| **Service**                  | **MTD**   | **MTPD**   | **Minimum Operation Goal**                               |
|------------------------------|-----------|------------|----------------------------------------------------------|
| Login                        | 1 minute  | 30 minutes | Ensure authorized access 24/7 with minimal downtime.     |
| Emergency Operation Requests | Immediate | 5 minutes  | Process emergency requests within 5 minutes.             |
| Operation Scheduling Changes | 2 minutes | 10 minutes | Process updates within an acceptable delay for accuracy. |
| Internal Network Access      | 1 minute  | 30 minutes | Maintain connectivity for access to core modules.        |
| VPN Access                   | 2 minutes | 15 minutes | Ensure secure remote access for key staff with fallback. |

## 3. Emergency Response Procedures

### 3.1 Activation

1. **Incident Identification:** Recognize disruption through automated alerts or reports.
2. **Notification:** Alert stakeholders (IT, hospital management, critical staff).
3. **DR Team Activation:** Mobilize the Disaster Recovery Team to implement the plan.

### 3.2 Initial Response

1. Assess the scope of the disruption (technical, environmental, or security-related).
2. Prioritize actions based on affected critical services.
3. Execute immediate containment measures to limit damage (e.g., isolating affected systems).

## 4. Backup and Recovery Procedures

### 4.1 Data Backup

- **Backup Frequency:** Data is backed up daily at 2:30 AM, with a full backup performed every Sunday at the same time, 2:30 AM.
- **Backup Methods:** Use redundant cloud storage with offsite replication.
- **Verification:** Conduct regular checks to ensure backups are functional.

### 4.2 Recovery Steps

1. **Data Restoration:** Use verified backups to recover lost or corrupted data.
2. **Database Recovery:** Prioritize core databases supporting critical services.
3. **Test Restored Data:** Validate data integrity and functionality post-recovery.

## 5. Hot Site Plan

### 5.1 Alternate Site

- **Location:** Pre-configured hot site located within a secure radius.
- **Infrastructure:**
  - Servers, storage, and networking equipment matching primary data center.
  - Pre-installed hospital management software.
- **Activation Time:** Ready for use within 30 minutes of disruption notification.

### 5.2 Data Synchronization

- Continuous replication of data to ensure hot site is current.
- Regular testing to validate hot site readiness.

## 6. Full System Restoration Steps

1. **Damage Assessment:** Evaluate affected infrastructure and identify restoration needs.
2. **Rebuild Systems:** Reinstall and reconfigure operating systems, applications, and services.
3. **Restore Data:** Use latest backups to reinstate critical data.
4. **Test Restored Systems:** Validate full functionality and operability of all critical services.
5. **Transition:** Redirect users back to the primary infrastructure post-validation.
6. **Post-Incident Review:** Document lessons learned and update the DR Plan.

## 7. Regular Testing and Updates

- **Testing Schedule:** Conduct quarterly drills to test the effectiveness of the DR plan.
- **Updates:** Revise the plan annually or as system changes occur.
- **Training:** Provide regular training for the DR Team and key staff.

## 8. Observations and Considerations

- Regular audits are essential to ensure compliance with MBCO targets.
- Expand testing to include various disruption scenarios.
- Maintain clear communication channels during a disruption.

---

## 5. Observations

- The DR plan must be clear and concise to ensure usability during emergencies.
- Regular updates are necessary to reflect changing infrastructure and risks.
- References:
  - [Kyndryl Disaster Recovery Plan Guide](https://www.kyndryl.com/us/en/learn/disaster-recovery-plan) 
  - [TechTarget Disaster Recovery Plan Guide](https://www.techtarget.com/searchdisasterrecovery/definition/disaster-recovery-plan)
  - [IBM Disaster Recovery Plan Guide](https://www.ibm.com/topics/disaster-recovery-plan)
  - [IBM Disaster Recovery Plan Example](https://www.ibm.com/docs/en/i/7.3?topic=system-example-disaster-recovery-plan)
