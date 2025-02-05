# US 7.4.7

<!-- TOC -->
* [US 7.4.7](#us-747)
  * [1. Context](#1-context)
  * [2. Requirements](#2-requirements)
  * [3. Analysis](#3-analysis)
  * [4. Design](#4-design)
    * [4.1. Realization](#41-realization)
<!-- TOC -->


## 1. Context

This is the first time this US is being worked on.

## 2. Requirements

**US 7.4.7:** As the organisation's administrator, I want to be presented with a BIA (Business Impact Analysis) of the 
final solution, adapting if and where applicable the risk(s) identified in the previous sprint

**Acceptance Criteria:**

**US 7.4.7.1:** The BIA must include a summary of the previously identified risks and their associated impacts.

**US 7.4.7.2:** The BIA must outline mitigation or adaptation strategies for each identified risk.


**Dependencies/References:**

**6.4.4:** It's needed the risks previously identified.


## 3. Analysis

This user story focuses on providing a Business Impact Analysis (BIA) for the final solution. This BIA should address 
the risks identified in the previous sprint, highlighting their impact and any required adaptations.

The main points are:

* The BIA must integrate previously identified risks.
* Ensure that the BIA is clear, actionable, and aligns with organizational priorities.
* A structured approach will ensure the risks are effectively mitigated or adapted where necessary.

## 4. Design

### 4.1. Realization

In the previous sprint, there were identified 4 types of risks:

* Security risks --> improper access, data breaches, ddos attacks, hacking, operation system vulnerabilities, social engineering
* Operational risks --> service downtime, backup failures, physical damage to servers, failed or unplanned updates, communication failures between components
* Legal risks --> non-compliance with GDPR
* Third-Party Dependency risks --> ASP.NET service disruption

The Business Impact Analysis (BIA) will focus on summarizing these risks, evaluating their potential impact on the 
final solution, and presenting clear adaptation or mitigation strategies. Each risk category will be addressed with 
its associated likelihood, impact, and overall risk level, ensuring a structured approach to risk management.
